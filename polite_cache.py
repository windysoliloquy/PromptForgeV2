#!/usr/bin/env python3
"""Cache Master Apollon's famous artists article with artist-sorted images.

Behavior:
- fetches the article page once
- extracts artist blocks from the article structure
- picks a single canonical image URL from each block (highest-width img srcset candidate)
- downloads images slowly and sequentially
- stores images under one folder per artist
- skips duplicate images by canonical URL and by content hash
"""

from __future__ import annotations

import argparse
import hashlib
import html
import json
import mimetypes
import random
import re
import sys
import time
import unicodedata
from dataclasses import dataclass
from datetime import UTC, datetime
from html.parser import HTMLParser
from pathlib import Path
from urllib import parse, request, robotparser

DEFAULT_URL = "https://masterapollon.com/the-100-most-famous-artists-of-all-time/"
DEFAULT_USER_AGENT = (
    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) "
    "AppleWebKit/537.36 (KHTML, like Gecko) "
    "Chrome/136.0.0.0 Safari/537.36"
)


@dataclass
class ArtistBlock:
    rank: int
    artist_name: str
    image_url: str | None


class ArtistArticleParser(HTMLParser):
    def __init__(self, base_url: str) -> None:
        super().__init__()
        self.base_url = base_url
        self.blocks: list[ArtistBlock] = []
        self._block_depth = 0
        self._in_block = False
        self._current_heading_chunks: list[str] = []
        self._capture_heading = False
        self._current_best_image: tuple[int, str] | None = None

    def handle_starttag(self, tag: str, attrs: list[tuple[str, str | None]]) -> None:
        attrs_dict = dict(attrs)
        class_attr = attrs_dict.get("class") or ""

        if tag == "div" and "wp-block-media-text" in class_attr and not self._in_block:
            self._in_block = True
            self._block_depth = 1
            self._current_heading_chunks = []
            self._capture_heading = False
            self._current_best_image = None
            return

        if self._in_block:
            self._block_depth += 1
            if tag == "h2" and "wp-block-heading" in class_attr:
                self._capture_heading = True
            elif tag == "img":
                best_url = choose_best_image_candidate(self.base_url, attrs_dict)
                if best_url:
                    width = image_width_score(attrs_dict, best_url)
                    if self._current_best_image is None or width >= self._current_best_image[0]:
                        self._current_best_image = (width, best_url)

    def handle_endtag(self, tag: str) -> None:
        if self._capture_heading and tag == "h2":
            self._capture_heading = False

        if self._in_block:
            self._block_depth -= 1
            if self._block_depth == 0:
                self._finish_block()
                self._in_block = False

    def handle_data(self, data: str) -> None:
        if self._capture_heading:
            self._current_heading_chunks.append(data)

    def _finish_block(self) -> None:
        heading = clean_heading("".join(self._current_heading_chunks))
        match = re.match(r"^(\d+)\s*-\s*(.+?)(?:\s*\((?:.|\s)*\))?$", heading)
        if not match:
            return
        rank = int(match.group(1))
        artist_name = match.group(2).strip()
        image_url = self._current_best_image[1] if self._current_best_image else None
        self.blocks.append(ArtistBlock(rank=rank, artist_name=artist_name, image_url=image_url))


def clean_heading(value: str) -> str:
    value = html.unescape(value).replace("\xa0", " ")
    value = re.sub(r"\s+", " ", value).strip()
    return value


def choose_best_image_candidate(base_url: str, attrs: dict[str, str | None]) -> str | None:
    candidates: list[tuple[int, str]] = []
    srcset = attrs.get("srcset") or ""
    for part in srcset.split(","):
        piece = part.strip()
        if not piece:
            continue
        bits = piece.split()
        url = parse.urljoin(base_url, bits[0])
        width = 0
        if len(bits) > 1 and bits[1].endswith("w") and bits[1][:-1].isdigit():
            width = int(bits[1][:-1])
        candidates.append((width, url))

    src = attrs.get("src")
    if src:
        candidates.append((0, parse.urljoin(base_url, src)))

    if not candidates:
        return None
    candidates.sort(key=lambda item: item[0])
    return candidates[-1][1]


def image_width_score(attrs: dict[str, str | None], image_url: str) -> int:
    srcset = attrs.get("srcset") or ""
    for part in srcset.split(","):
        piece = part.strip()
        if not piece:
            continue
        bits = piece.split()
        url = parse.urljoin(DEFAULT_URL, bits[0])
        if url == image_url and len(bits) > 1 and bits[1].endswith("w") and bits[1][:-1].isdigit():
            return int(bits[1][:-1])
    return 0


def ascii_slug(value: str) -> str:
    normalized = unicodedata.normalize("NFKD", value)
    ascii_text = normalized.encode("ascii", "ignore").decode("ascii")
    ascii_text = re.sub(r"[^A-Za-z0-9]+", "_", ascii_text).strip("_")
    return ascii_text or "artist"


def extension_for_url(url: str, content_type: str) -> str:
    suffix = Path(parse.urlsplit(url).path).suffix
    if suffix:
        return suffix
    guessed = mimetypes.guess_extension((content_type or "").split(";")[0].strip())
    return guessed or ".bin"


def build_opener(user_agent: str) -> request.OpenerDirector:
    opener = request.build_opener()
    opener.addheaders = [
        ("User-Agent", user_agent),
        ("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8"),
        ("Accept-Language", "en-US,en;q=0.9"),
        ("Cache-Control", "no-cache"),
        ("Pragma", "no-cache"),
    ]
    return opener


def read_robots(url: str, user_agent: str, timeout: float) -> robotparser.RobotFileParser:
    parsed = parse.urlsplit(url)
    robots_url = parse.urlunsplit((parsed.scheme, parsed.netloc, "/robots.txt", "", ""))
    rp = robotparser.RobotFileParser()
    rp.set_url(robots_url)
    opener = build_opener(user_agent)
    try:
        with opener.open(robots_url, timeout=timeout) as response:
            body = response.read().decode("utf-8", errors="replace")
        rp.parse(body.splitlines())
    except Exception:
        rp.parse([])
    return rp


def fetch(opener: request.OpenerDirector, url: str, timeout: float) -> tuple[int, str, bytes]:
    with opener.open(url, timeout=timeout) as response:
        status = getattr(response, "status", 200)
        content_type = response.headers.get("Content-Type", "")
        body = response.read()
    return status, content_type, body


def polite_sleep(min_delay: float, max_delay: float) -> None:
    delay = random.uniform(min_delay, max_delay)
    print(f"[sleep] {delay:.2f}s")
    time.sleep(delay)


def parse_artists(article_url: str, html_text: str) -> list[ArtistBlock]:
    parser = ArtistArticleParser(article_url)
    parser.feed(html_text)
    return parser.blocks


def download_artist_images(article_url: str, output_dir: Path, min_delay: float, max_delay: float, timeout: float, user_agent: str) -> int:
    output_dir.mkdir(parents=True, exist_ok=True)
    source_dir = output_dir / "source"
    artists_dir = output_dir / "artists"
    source_dir.mkdir(parents=True, exist_ok=True)
    artists_dir.mkdir(parents=True, exist_ok=True)

    opener = build_opener(user_agent)
    robots = read_robots(article_url, user_agent, timeout)

    if not robots.can_fetch(user_agent, article_url):
        raise RuntimeError(f"robots.txt disallows article fetch: {article_url}")

    print(f"[fetch page] {article_url}")
    status, content_type, body = fetch(opener, article_url, timeout)
    html_text = body.decode("utf-8", errors="replace")

    article_file = source_dir / "the-100-most-famous-artists-of-all-time.html"
    article_file.write_text(html_text, encoding="utf-8")

    page_meta = {
        "url": article_url,
        "status": status,
        "content_type": content_type,
        "fetched_at_utc": datetime.now(UTC).isoformat(),
        "html_file": str(article_file.relative_to(output_dir)),
    }
    (source_dir / "page_meta.json").write_text(json.dumps(page_meta, indent=2), encoding="utf-8")

    artist_blocks = parse_artists(article_url, html_text)
    print(f"[parse] found {len(artist_blocks)} artist blocks")

    url_seen: set[str] = set()
    hash_seen: set[str] = set()
    artist_manifest: list[dict] = []

    for index, block in enumerate(sorted(artist_blocks, key=lambda item: item.rank), start=1):
        folder_name = f"{block.rank:03d}_{ascii_slug(block.artist_name)}"
        folder_path = artists_dir / folder_name
        folder_path.mkdir(parents=True, exist_ok=True)

        artist_entry = {
            "rank": block.rank,
            "artist_name": block.artist_name,
            "folder": str(folder_path.relative_to(output_dir)),
            "images": [],
        }

        if not block.image_url:
            artist_entry["warning"] = "No image found in article block"
            artist_manifest.append(artist_entry)
            continue

        if not robots.can_fetch(user_agent, block.image_url):
            artist_entry["warning"] = f"robots.txt disallows image: {block.image_url}"
            artist_manifest.append(artist_entry)
            continue

        if block.image_url in url_seen:
            artist_entry["warning"] = f"Duplicate image URL skipped: {block.image_url}"
            artist_manifest.append(artist_entry)
            continue

        print(f"[fetch image] {block.rank:03d} {block.artist_name} :: {block.image_url}")
        img_status, img_type, img_body = fetch(opener, block.image_url, timeout)
        digest = hashlib.sha256(img_body).hexdigest()
        if digest in hash_seen:
            artist_entry["warning"] = f"Duplicate image content skipped: {block.image_url}"
            artist_manifest.append(artist_entry)
        else:
            hash_seen.add(digest)
            url_seen.add(block.image_url)
            ext = extension_for_url(block.image_url, img_type)
            file_name = f"{block.rank:03d}_{ascii_slug(block.artist_name)}_01{ext}"
            image_path = folder_path / file_name
            image_path.write_bytes(img_body)
            metadata = {
                "rank": block.rank,
                "artist_name": block.artist_name,
                "source_url": block.image_url,
                "status": img_status,
                "content_type": img_type,
                "sha256": digest,
                "saved_file": str(image_path.relative_to(output_dir)),
                "fetched_at_utc": datetime.now(UTC).isoformat(),
            }
            (folder_path / "image_01.json").write_text(json.dumps(metadata, indent=2), encoding="utf-8")
            artist_entry["images"].append(metadata)
            artist_entry["image_count"] = 1

        artist_manifest.append(artist_entry)
        if index < len(artist_blocks):
            polite_sleep(min_delay, max_delay)

    manifest = {
        "source": page_meta,
        "artist_count": len(artist_blocks),
        "saved_image_count": sum(len(item.get("images", [])) for item in artist_manifest),
        "generated_at_utc": datetime.now(UTC).isoformat(),
        "artists": artist_manifest,
    }
    (output_dir / "artists_manifest.json").write_text(json.dumps(manifest, indent=2), encoding="utf-8")
    print(f"[done] saved {manifest['saved_image_count']} unique artist image(s) into {artists_dir}")
    return 0


def parse_args(argv: list[str]) -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Polite artist-image cache for the Master Apollon article")
    parser.add_argument("--url", default=DEFAULT_URL, help="Article URL to cache")
    parser.add_argument("--output-dir", default=r"C:\Users\windy\OneDrive\Desktop\Prompt Forge\cache\masterapollon", help="Output directory")
    parser.add_argument("--min-delay", type=float, default=1.5, help="Minimum seconds between image requests")
    parser.add_argument("--max-delay", type=float, default=3.5, help="Maximum seconds between image requests")
    parser.add_argument("--timeout", type=float, default=30.0, help="Per-request timeout")
    parser.add_argument("--user-agent", default=DEFAULT_USER_AGENT, help="HTTP User-Agent")
    return parser.parse_args(argv)


def main(argv: list[str]) -> int:
    args = parse_args(argv)
    return download_artist_images(
        article_url=args.url,
        output_dir=Path(args.output_dir),
        min_delay=max(args.min_delay, 0.5),
        max_delay=max(args.max_delay, max(args.min_delay, 0.5)),
        timeout=max(args.timeout, 5.0),
        user_agent=args.user_agent,
    )


if __name__ == "__main__":
    sys.exit(main(sys.argv[1:]))
