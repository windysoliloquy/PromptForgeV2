#!/usr/bin/env python3
"""Sync artist-pair source artifacts from the app's real UI roster.

Outputs:
- PromptForge.Core/Data/artist_influences_ui_roster.json
- docs/artist-pairs/ui_artist_matrix_coverage_report.json
- docs/artist-pairs/ui_artist_matrix_coverage_report.md
"""

from __future__ import annotations

import argparse
import json
import unicodedata
from dataclasses import dataclass
from datetime import UTC, datetime
from pathlib import Path


REPO_ROOT = Path(__file__).parent.parent.parent
PROFILES_PATH = REPO_ROOT / "PromptForge.Core" / "Data" / "artist_profiles.json"
MATRIX_PATH = REPO_ROOT / "PromptForge.Core" / "Data" / "prompt_forge_artist_pair_matrix.json"
FULL_ROSTER_PATH = REPO_ROOT / "PromptForge.Core" / "Data" / "artist_influences_ui_roster.json"
REPORT_JSON_PATH = REPO_ROOT / "docs" / "artist-pairs" / "ui_artist_matrix_coverage_report.json"
REPORT_MD_PATH = REPO_ROOT / "docs" / "artist-pairs" / "ui_artist_matrix_coverage_report.md"
LEGACY_SOURCE_NAME = "artist_influences_normalized.json"
MOJIBAKE_CHARS = {"\u00C3", "\u00C2", "\u00D0", "\u00D1", "\uFFFD", "\u0102", "\u0192", "\u00AD"}


@dataclass(frozen=True)
class ArtistProfileRecord:
    display_name: str
    artist_key: str
    normalized_name: str
    hallmarks: list[str]
    composition: list[str]
    palette: list[str]
    surface: list[str]
    mood: list[str]
    source_name: str


def clean_display_name(value: str) -> str:
    cleaned = value.strip()
    for _ in range(3):
        if not any(ch in MOJIBAKE_CHARS for ch in cleaned):
            break
        try:
            repaired = cleaned.encode("latin-1").decode("utf-8")
        except UnicodeError:
            break
        if repaired == cleaned:
            break
        cleaned = repaired
    return cleaned


def choose_display_name(value: str) -> str:
    cleaned = clean_display_name(value)
    if any(ch in MOJIBAKE_CHARS for ch in cleaned):
        parts = normalize_name(cleaned).split()
        return " ".join(part.capitalize() for part in parts)
    return cleaned


def normalize_name(value: str) -> str:
    cleaned = clean_display_name(value)
    normalized = unicodedata.normalize("NFKD", cleaned)
    ascii_chars: list[str] = []
    for ch in normalized:
        if ord(ch) < 128:
            lower = ch.lower()
            if lower.isalnum():
                ascii_chars.append(lower)
            elif lower.isspace() or not lower.isalnum():
                ascii_chars.append(" ")
    return " ".join("".join(ascii_chars).split())


def artist_key_for(value: str) -> str:
    return "_".join(part for part in normalize_name(value).split(" ") if part)


def name_quality_score(value: str) -> int:
    suspicious_penalty = sum(1 for ch in value if ch in MOJIBAKE_CHARS or ch == "?") * 10
    diacritic_bonus = sum(1 for ch in value if ord(ch) > 127 and ch not in MOJIBAKE_CHARS)
    return diacritic_bonus - suspicious_penalty


def load_ui_profiles() -> list[ArtistProfileRecord]:
    raw_profiles = json.loads(PROFILES_PATH.read_text(encoding="utf-8-sig"))
    grouped: dict[str, list[dict]] = {}
    for profile in raw_profiles:
        grouped.setdefault(normalize_name(profile["Name"]), []).append(profile)

    selected_profiles: list[ArtistProfileRecord] = []
    for normalized_name, profiles in grouped.items():
        best = sorted(
            profiles,
            key=lambda profile: (-name_quality_score(profile["Name"]), clean_display_name(profile["Name"]).lower()),
        )[0]
        cleaned_name = choose_display_name(best["Name"])
        selected_profiles.append(
            ArtistProfileRecord(
                display_name=cleaned_name,
                artist_key=artist_key_for(cleaned_name),
                normalized_name=normalized_name,
                hallmarks=best.get("Hallmarks", []),
                composition=best.get("Composition", []),
                palette=best.get("Palette", []),
                surface=best.get("Surface", []),
                mood=best.get("Mood", []),
                source_name=best["Name"],
            )
        )

    return sorted(selected_profiles, key=lambda profile: profile.normalized_name)


def load_matrix_roster() -> tuple[dict[str, str], dict]:
    matrix = json.loads(MATRIX_PATH.read_text(encoding="utf-8-sig"))
    roster: dict[str, str] = {}
    for entry in matrix.get("pairs", []):
        left_name = choose_display_name(entry["artist_a"])
        right_name = choose_display_name(entry["artist_b"])
        left_key = entry.get("artist_a_key") or artist_key_for(left_name)
        right_key = entry.get("artist_b_key") or artist_key_for(right_name)
        roster.setdefault(left_key, left_name)
        roster.setdefault(right_key, right_name)
    return dict(sorted(roster.items())), matrix


def build_report(ui_profiles: list[ArtistProfileRecord], matrix_roster: dict[str, str], matrix: dict) -> dict:
    ui_by_key = {profile.artist_key: profile for profile in ui_profiles}
    missing_from_matrix = [
        {
            "artist_key": profile.artist_key,
            "display_name": profile.display_name,
            "source_name": profile.source_name,
        }
        for profile in ui_profiles
        if profile.artist_key not in matrix_roster
    ]
    extra_in_matrix = [
        {
            "artist_key": artist_key,
            "matrix_display_name": matrix_roster[artist_key],
        }
        for artist_key in sorted(matrix_roster)
        if artist_key not in ui_by_key
    ]
    mismatches = []
    for artist_key, profile in ui_by_key.items():
        matrix_name = matrix_roster.get(artist_key)
        if matrix_name and matrix_name != profile.display_name:
            mismatch_flags = []
            if normalize_name(matrix_name) == profile.normalized_name:
                mismatch_flags.append("display-name mismatch")
            if profile.source_name != profile.display_name:
                mismatch_flags.append("ui-source mojibake repaired")
            if matrix_name != clean_display_name(matrix_name):
                mismatch_flags.append("matrix mojibake repaired")
            mismatches.append(
                {
                    "artist_key": artist_key,
                    "ui_display_name": profile.display_name,
                    "ui_source_name": profile.source_name,
                    "matrix_display_name": matrix_name,
                    "issues": mismatch_flags or ["name mismatch"],
                }
            )

    return {
        "generated_at_utc": datetime.now(UTC).isoformat(),
        "ui_artist_count": len(ui_profiles) + 1,
        "ui_artist_count_excluding_none": len(ui_profiles),
        "current_matrix_source_count": matrix.get("source_artist_count", 0),
        "current_matrix_pair_count": matrix.get("pair_count", 0),
        "legacy_matrix_source_file": matrix.get("source_file", LEGACY_SOURCE_NAME),
        "legacy_matrix_source_file_present_in_repo": False,
        "ui_artists": [
            {
                "artist_key": profile.artist_key,
                "display_name": profile.display_name,
                "source_name": profile.source_name,
            }
            for profile in ui_profiles
        ],
        "artists_in_ui_not_in_matrix_source": missing_from_matrix,
        "artists_in_matrix_source_not_in_ui": extra_in_matrix,
        "name_mismatches_and_aliases": mismatches,
        "encoding_mismatch_examples": [
            mismatch
            for mismatch in mismatches
            if "ui-source mojibake repaired" in mismatch["issues"] or "matrix mojibake repaired" in mismatch["issues"]
        ],
        "joaquin_sorolla": next(
            (
                {
                    "artist_key": profile.artist_key,
                    "display_name": profile.display_name,
                    "source_name": profile.source_name,
                    "defined_in": str(PROFILES_PATH),
                    "present_in_matrix": profile.artist_key in matrix_roster,
                }
                for profile in ui_profiles
                if profile.artist_key == "joaquin_sorolla"
            ),
            None,
        ),
    }


def write_full_roster(ui_profiles: list[ArtistProfileRecord]) -> None:
    FULL_ROSTER_PATH.parent.mkdir(parents=True, exist_ok=True)
    payload = {
        "schema_version": "1.0",
        "record_type": "artist_influence_roster",
        "source_file": "artist_profiles.json",
        "source_artist_count": len(ui_profiles),
        "generated_at_utc": datetime.now(UTC).isoformat(),
        "artists": [
            {
                "artist_key": profile.artist_key,
                "artist_name": profile.display_name,
                "normalized_name": profile.normalized_name,
                "hallmarks": profile.hallmarks,
                "composition": profile.composition,
                "palette": profile.palette,
                "surface": profile.surface,
                "mood": profile.mood,
            }
            for profile in ui_profiles
        ],
    }
    FULL_ROSTER_PATH.write_text(json.dumps(payload, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")


def write_report(report: dict) -> None:
    REPORT_JSON_PATH.parent.mkdir(parents=True, exist_ok=True)
    REPORT_JSON_PATH.write_text(json.dumps(report, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")

    missing_lines = "\n".join(
        f"- `{item['artist_key']}` - {item['display_name']}" for item in report["artists_in_ui_not_in_matrix_source"]
    )
    extra_lines = "\n".join(
        f"- `{item['artist_key']}` - {item['matrix_display_name']}" for item in report["artists_in_matrix_source_not_in_ui"]
    )
    mismatch_lines = "\n".join(
        f"- `{item['artist_key']}` - UI: {item['ui_display_name']} | Matrix: {item['matrix_display_name']} | Issues: {', '.join(item['issues'])}"
        for item in report["name_mismatches_and_aliases"]
    )
    joaquin = report.get("joaquin_sorolla") or {}
    markdown = f"""# UI vs Matrix Coverage Report

- UI artist count: {report['ui_artist_count']} (including `None`)
- UI artist count excluding `None`: {report['ui_artist_count_excluding_none']}
- Current matrix source count: {report['current_matrix_source_count']}
- Current matrix pair count: {report['current_matrix_pair_count']}
- Legacy matrix source file: `{report['legacy_matrix_source_file']}`
- Legacy matrix source file present in repo: `{report['legacy_matrix_source_file_present_in_repo']}`
- Expanded full-roster source path: `{FULL_ROSTER_PATH}`

## Artists in UI but not in current matrix source
{missing_lines or "- None"}

## Artists in current matrix source but not in UI
{extra_lines or "- None"}

## Name mismatches, aliases, and encoding issues
{mismatch_lines or "- None"}

## Joaquin Sorolla
- Display name in UI: `{joaquin.get('display_name', '')}`
- Source representation: `{joaquin.get('source_name', '')}`
- Stable key: `{joaquin.get('artist_key', '')}`
- Defined in: `{joaquin.get('defined_in', '')}`
- Present in matrix: `{joaquin.get('present_in_matrix', False)}`
"""
    REPORT_MD_PATH.write_text(markdown, encoding="utf-8")


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Sync artist pair source files from the UI roster")
    parser.add_argument("--skip-report", action="store_true", help="Do not write the coverage report")
    parser.add_argument("--skip-full-roster", action="store_true", help="Do not write the expanded full-roster source file")
    return parser.parse_args()


def main() -> int:
    args = parse_args()
    ui_profiles = load_ui_profiles()
    matrix_roster, matrix = load_matrix_roster()
    report = build_report(ui_profiles, matrix_roster, matrix)
    if not args.skip_full_roster:
        write_full_roster(ui_profiles)
    if not args.skip_report:
        write_report(report)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
