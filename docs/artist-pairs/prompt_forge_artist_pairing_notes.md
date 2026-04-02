# Prompt Forge — Artist Pairing Notes

This pass converts the uploaded artist profiles into a **heuristic pair matrix** for two-artist blending.

## What the matrix is
A first-pass source of truth for Prompt Forge that classifies every possible two-artist pair from the uploaded profile set.

It uses only the structured profile fields:
- hallmarks
- composition
- palette
- surface
- mood

## What the categories mean
- **near-overlap** — very little meaningful separation; both artists largely pull in the same lane
- **complementary** — additive blend; traits can coexist without constant contradiction
- **tense-but-usable** — workable, but the prompt should split ownership or reduce one artist
- **high-conflict** — style regimes collide hard; expect averaging, flattening, or one artist being dropped

## Integration suggestion
Use the matrix to drive:
1. a UI warning badge
2. a short tooltip
3. internal weighting logic

Suggested behavior:
- near-overlap: compress language and lower total artist verbosity
- complementary: allow normal blend phrasing
- tense-but-usable: assign domain ownership (for example palette vs composition, or mood vs surface)
- high-conflict: warn the user and reduce the weaker artist unless explicitly forced

## Important limitation
This is **not** a render-tested verdict.
It is a structured inference from text profiles only.
Use it as a seed source of truth, then harden it against empirical generations.

## Files
- `prompt_forge_artist_pair_matrix.json`
- `prompt_forge_artist_pair_matrix.csv`
