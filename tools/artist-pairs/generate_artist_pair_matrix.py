#!/usr/bin/env python3
"""Generate the artist pair matrix from the UI-aligned roster."""

from __future__ import annotations

import argparse
import csv
import json
import math
import unicodedata
from dataclasses import dataclass
from datetime import UTC, datetime
from itertools import combinations
from pathlib import Path


REPO_ROOT = Path(__file__).parent.parent.parent
ROSTER_PATH = REPO_ROOT / "PromptForge.Core" / "Data" / "artist_influences_ui_roster.json"
MATRIX_PATH = REPO_ROOT / "PromptForge.Core" / "Data" / "prompt_forge_artist_pair_matrix.json"
CSV_PATH = REPO_ROOT / "docs" / "artist-pairs" / "prompt_forge_artist_pair_matrix.csv"

CATEGORY_DEFINITIONS = {
    "near-overlap": "Little meaningful separation. Blending mostly reinforces a shared lane and can often be compressed.",
    "complementary": "Clear shared ground plus additive differences. Usually safe to blend.",
    "tense-but-usable": "The pair can work, but the prompt should give each artist a domain or priority.",
    "high-conflict": "The pair pushes incompatible visual regimes at the same time and usually needs strong constraint or should be avoided.",
}

INFERENCE_BASIS = "Derived heuristically from hallmarks, composition, palette, surface, and mood fields in the UI-aligned artist roster."

MANUAL_ALIASES = {
    "joseph_mallord_william_turner": ["j_m_w_turner", "jmw_turner", "j.m.w. turner"],
    "michelangelo_buonarroti": ["michelangelo"],
    "hokusai": ["katsushika_hokusai", "katsushika hokusai"],
    "n_c_wyeth": ["n.c._wyeth", "n.c. wyeth"],
    "charles_amable_lenoir": ["charlesamable_lenoir"],
    "jacques_louis_david": ["jacqueslouis_david"],
    "jean_auguste_dominique_ingres": ["jeanaugustedominique_ingres"],
    "jean_baptiste_simeon_chardin": ["jeanbaptistesimeon_chardin"],
    "jean_francois_millet": ["jeanfrancois_millet"],
    "jean_honore_fragonard": ["jeanhonore_fragonard"],
    "jean_leon_gerome": ["jeanleon_gerome"],
    "jules_cyrille_cave": ["julescyrille_cave"],
    "lawrence_alma_tadema": ["lawrence_almatadema"],
    "louis_leopold_boilly": ["louisleopold_boilly"],
    "pierre_auguste_cot": ["pierreauguste_cot"],
    "pierre_auguste_renoir": ["pierreauguste_renoir"],
    "william_adolphe_bouguereau": ["williamadolphe_bouguereau"],
}

SIGNAL_RULES = {
    "grounded_realism": ["realism", "observational", "naturalism", "figurative", "courtly", "historic", "academic", "anatom"],
    "dream_logic": ["dream", "symbol", "myth", "allegor", "surreal", "visionary"],
    "ornamental": ["ornament", "decorative", "arabesque", "halo", "art nouveau", "pattern"],
    "abstraction": ["abstract", "geometric", "reduction", "cubist", "nonrepresent"],
    "graphic_print": ["print", "engraver", "woodblock", "graphic"],
    "line_control": ["line", "drawing", "contour", "draftsmanship", "linear"],
    "soft_atmosphere": ["atmospher", "luminous", "mist", "haze", "glow", "soft"],
    "panoramic_density": ["dense", "complex", "panoramic", "crowd", "spatial complexity"],
    "minimal_staging": ["minimal", "isolated", "empty", "sparse", "poster"],
    "portrait_staging": ["portrait", "figure", "single figure", "salon"],
    "illustrative_storytelling": ["illustr", "story", "narrative", "mythic", "theatrical"],
    "muted_palette": ["muted", "pastel", "tonal", "restrained", "gray", "smoky"],
    "warm_palette": ["warm", "gold", "amber", "crimson", "sunlit"],
    "cool_palette": ["cool", "azure", "blue", "steel", "silver", "moonlit"],
    "tactile_paint": ["brush", "paint", "impasto", "tactile", "painterly"],
    "smooth_finish": ["smooth", "polished", "silky", "refined"],
    "psychic_pressure": ["dread", "anxiety", "pressure", "ominous", "haunt"],
    "serenity": ["serene", "calm", "quiet", "poetic", "grace"],
    "playful": ["playful", "whims", "comic", "delight"],
    "ideal_beauty": ["idealized", "beauty", "classicism", "luxurious", "graceful"],
    "organic_flow": ["organic", "flowing", "wave", "botanical", "fluid"],
}

CONFLICT_RULES = [
    (("grounded_realism", "dream_logic"), "literal depiction vs dream-symbol logic"),
    (("grounded_realism", "abstraction"), "literal depiction vs abstract reduction"),
    (("grounded_realism", "ornamental"), "observed realism vs ornamental flattening"),
    (("panoramic_density", "minimal_staging"), "panoramic density vs isolated minimal staging"),
    (("illustrative_storytelling", "grounded_realism"), "story-illustration simplification vs pure observational realism"),
    (("soft_atmosphere", "line_control"), "soft atmospheric transitions vs hard line control"),
    (("ideal_beauty", "psychic_pressure"), "ideal beauty vs harsh psychic pressure"),
    (("serenity", "psychic_pressure"), "serenity vs dread"),
    (("playful", "psychic_pressure"), "playfulness vs menace"),
    (("organic_flow", "abstraction"), "organic flow vs geometric reduction"),
]

SIGNAL_LABELS = {
    "grounded_realism": "grounded realism",
    "dream_logic": "symbolic or dream logic",
    "ornamental": "ornamental decorative design",
    "abstraction": "abstraction/geometric reduction",
    "graphic_print": "graphic/print discipline",
    "line_control": "line-led drawing control",
    "soft_atmosphere": "soft atmospheric transitions",
    "panoramic_density": "panoramic density",
    "minimal_staging": "isolated minimal staging",
    "portrait_staging": "portrait-first staging",
    "illustrative_storytelling": "illustrative storytelling",
    "muted_palette": "muted or pastel color",
    "warm_palette": "warm palette bias",
    "cool_palette": "cool palette bias",
    "tactile_paint": "rough tactile paint",
    "smooth_finish": "smooth polished finish",
    "psychic_pressure": "dark or ominous pressure",
    "serenity": "serenity",
    "playful": "playfulness",
    "ideal_beauty": "ideal beauty",
    "organic_flow": "organic flowing forms",
}


@dataclass(frozen=True)
class Artist:
    key: str
    name: str
    normalized_name: str
    hallmarks: tuple[str, ...]
    composition: tuple[str, ...]
    palette: tuple[str, ...]
    surface: tuple[str, ...]
    mood: tuple[str, ...]
    phrases: tuple[str, ...]
    signals: frozenset[str]


def normalize_text(value: str) -> str:
    normalized = unicodedata.normalize("NFKD", value)
    out: list[str] = []
    for ch in normalized:
        if ord(ch) < 128:
            lower = ch.lower()
            if lower.isalnum():
                out.append(lower)
            else:
                out.append(" ")
    return " ".join("".join(out).split())


def keyify(value: str) -> str:
    return "_".join(part for part in normalize_text(value).split() if part)


def read_roster() -> list[Artist]:
    payload = json.loads(ROSTER_PATH.read_text(encoding="utf-8"))
    artists: list[Artist] = []
    for item in payload["artists"]:
        phrases = tuple(
            phrase.strip()
            for phrase in [*item.get("hallmarks", []), *item.get("composition", []), *item.get("palette", []), *item.get("surface", []), *item.get("mood", [])]
            if phrase and phrase.strip()
        )
        signal_set = infer_signals(phrases)
        artists.append(
            Artist(
                key=item["artist_key"],
                name=item["artist_name"],
                normalized_name=item.get("normalized_name", normalize_text(item["artist_name"])),
                hallmarks=tuple(item.get("hallmarks", [])),
                composition=tuple(item.get("composition", [])),
                palette=tuple(item.get("palette", [])),
                surface=tuple(item.get("surface", [])),
                mood=tuple(item.get("mood", [])),
                phrases=phrases,
                signals=frozenset(signal_set),
            )
        )
    return artists


def infer_signals(phrases: tuple[str, ...]) -> set[str]:
    signals: set[str] = set()
    normalized_phrases = [normalize_text(phrase) for phrase in phrases]
    for signal, keywords in SIGNAL_RULES.items():
        for phrase in normalized_phrases:
            if any(keyword in phrase for keyword in keywords):
                signals.add(signal)
                break
    return signals


def shared_traits_for(left: Artist, right: Artist) -> list[str]:
    shared = []
    exact_left = {normalize_text(phrase): phrase for phrase in left.phrases}
    exact_right = {normalize_text(phrase): phrase for phrase in right.phrases}
    for phrase in sorted(set(exact_left).intersection(exact_right)):
        shared.append(exact_left[phrase])

    for signal in sorted(left.signals.intersection(right.signals)):
        label = SIGNAL_LABELS.get(signal)
        if label and label not in shared:
            shared.append(label)
    return shared[:4]


def conflict_signals_for(left: Artist, right: Artist) -> list[str]:
    conflicts = []
    for (a, b), label in CONFLICT_RULES:
        if (a in left.signals and b in right.signals) or (b in left.signals and a in right.signals):
            conflicts.append(label)
    return conflicts


def pair_scores(left: Artist, right: Artist, shared_traits: list[str], conflict_signals: list[str]) -> tuple[int, int, str]:
    shared_signals = len(left.signals.intersection(right.signals))
    union_signals = len(left.signals.union(right.signals))
    jaccard = (shared_signals / union_signals) if union_signals else 0.0
    affinity = max(0, min(100, int(round((shared_signals * 18) + (len(shared_traits) * 7) + (jaccard * 28) - (len(conflict_signals) * 10)))))
    difficulty = max(0, min(100, int(round(18 + (len(conflict_signals) * 22) + max(0, 48 - affinity) * 0.55))))

    if shared_signals >= 4 and jaccard >= 0.55 and len(conflict_signals) <= 1:
        category = "near-overlap"
    elif len(conflict_signals) >= 3 or difficulty >= 78:
        category = "high-conflict"
    elif len(conflict_signals) >= 1 or difficulty >= 46:
        category = "tense-but-usable"
    else:
        category = "complementary"

    return affinity, difficulty, category


def summarize_domains(artist: Artist) -> str:
    priority = [
        "grounded_realism",
        "dream_logic",
        "ornamental",
        "abstraction",
        "graphic_print",
        "line_control",
        "soft_atmosphere",
        "portrait_staging",
        "illustrative_storytelling",
        "organic_flow",
    ]
    labels = [SIGNAL_LABELS[signal] for signal in priority if signal in artist.signals]
    if not labels:
        return "visual direction"
    return labels[0] if len(labels) == 1 else f"{labels[0]} and {labels[1]}"


def build_effect_text(category: str, left: Artist, right: Artist, shared_traits: list[str], conflict_signals: list[str]) -> str:
    if category == "near-overlap":
        if shared_traits:
            return f"These artists already share {', '.join(shared_traits[:2])}. Prompt language can usually compress into one primary lane without losing much."
        return "These artists already sit in adjacent territory. Prompt language can usually compress into one primary lane without losing much."
    if category == "complementary":
        if shared_traits:
            return f"Prompt can fuse shared {', '.join(shared_traits[:2])} with {summarize_domains(left)} and {summarize_domains(right)}. This usually creates a readable hybrid instead of two competing instructions."
        return "Prompt can distribute ownership cleanly across adjacent traits. The result is usually additive rather than contradictory."
    if category == "high-conflict":
        if conflict_signals:
            return f"Prompt needs hard domain separation because it is asking for {conflict_signals[0]}" + (f", plus {conflict_signals[1]}." if len(conflict_signals) > 1 else ".")
        return "Prompt needs hard domain separation because the pair pushes incompatible style regimes at the same time."
    return f"Prompt should assign ownership explicitly, usually letting one artist drive {summarize_domains(left)} and the other drive {summarize_domains(right)}. Without that split, the blend can wobble."


def build_struggle_text(category: str, conflict_signals: list[str]) -> str:
    if category == "near-overlap":
        return "Main risk is over-compression: the model may collapse the blend into one artist and flatten the distinction."
    if category == "complementary":
        if conflict_signals:
            return f"Main risk is mild drift between {conflict_signals[0]}. Usually manageable unless both influences are pushed too hard."
        return "Main risk is generic averaging: the model may collapse the blend into a safer middle and lose whichever artist is weaker."
    if category == "high-conflict":
        if conflict_signals:
            return f"Model will usually flatten or ignore one artist because {conflict_signals[0]}" + (f" and {conflict_signals[1]}" if len(conflict_signals) > 1 else "") + " are hard to resolve in one image."
        return "Model will usually flatten or ignore one artist because the two style regimes are hard to resolve in one image."
    if conflict_signals:
        return f"Model will tend to average or drop one side when forced to hold {conflict_signals[0]}" + (f" and {conflict_signals[1]}" if len(conflict_signals) > 1 else "") + " at the same time."
    return "Model may average or discard one artist unless the prompt gives each artist a clearer role."


def build_alias_map(artists: list[Artist]) -> dict[str, str]:
    aliases: dict[str, str] = {}
    for artist in artists:
        aliases[artist.key] = artist.key
        aliases[keyify(artist.name)] = artist.key
        aliases[keyify(artist.normalized_name)] = artist.key
    for canonical_key, alias_values in MANUAL_ALIASES.items():
        for alias in alias_values:
            aliases[keyify(alias)] = canonical_key
    return dict(sorted((alias, key) for alias, key in aliases.items() if alias and key))


def generate_matrix(artists: list[Artist]) -> dict:
    pairs = []
    for left, right in combinations(artists, 2):
        shared_traits = shared_traits_for(left, right)
        conflict_signals = conflict_signals_for(left, right)
        affinity, difficulty, category = pair_scores(left, right, shared_traits, conflict_signals)
        pairs.append(
            {
                "artist_a": left.name,
                "artist_b": right.name,
                "artist_a_key": left.key,
                "artist_b_key": right.key,
                "category": category,
                "affinity_score": affinity,
                "difficulty_score": difficulty,
                "shared_traits": shared_traits,
                "conflict_signals": conflict_signals,
                "effect_on_prompt_generation": build_effect_text(category, left, right, shared_traits, conflict_signals),
                "what_models_struggle_with": build_struggle_text(category, conflict_signals),
                "inference_basis": INFERENCE_BASIS,
            }
        )

    return {
        "schema_version": "1.0",
        "record_type": "artist_pair_matrix",
        "source_file": "artist_influences_ui_roster.json",
        "source_artist_count": len(artists),
        "pair_count": len(pairs),
        "generated_at_utc": datetime.now(UTC).isoformat(),
        "artist_aliases": build_alias_map(artists),
        "category_definitions": CATEGORY_DEFINITIONS,
        "important_note": "This is a text-profile heuristic matrix, not a render-tested verdict. Use it as a starting source of truth, then calibrate with real generations for edge cases.",
        "pairs": pairs,
    }


def write_csv(matrix: dict) -> None:
    CSV_PATH.parent.mkdir(parents=True, exist_ok=True)
    with CSV_PATH.open("w", encoding="utf-8", newline="") as handle:
        writer = csv.writer(handle)
        writer.writerow([
            "artist_a",
            "artist_b",
            "artist_a_key",
            "artist_b_key",
            "category",
            "affinity_score",
            "difficulty_score",
            "shared_traits",
            "conflict_signals",
            "effect_on_prompt_generation",
            "what_models_struggle_with",
            "inference_basis",
        ])
        for pair in matrix["pairs"]:
            writer.writerow([
                pair["artist_a"],
                pair["artist_b"],
                pair["artist_a_key"],
                pair["artist_b_key"],
                pair["category"],
                pair["affinity_score"],
                pair["difficulty_score"],
                pair["shared_traits"],
                pair["conflict_signals"],
                pair["effect_on_prompt_generation"],
                pair["what_models_struggle_with"],
                pair["inference_basis"],
            ])


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Generate the artist pair matrix from the UI roster")
    parser.add_argument("--skip-csv", action="store_true", help="Skip CSV export")
    return parser.parse_args()


def main() -> int:
    args = parse_args()
    artists = read_roster()
    matrix = generate_matrix(artists)
    MATRIX_PATH.write_text(json.dumps(matrix, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
    if not args.skip_csv:
        write_csv(matrix)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
