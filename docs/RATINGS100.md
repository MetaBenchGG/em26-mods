# Ratings100 — first version

## Goal

Replace the player-facing star scale with a FIFA-like 0–100 scale while leaving the
underlying save values and game simulation unchanged.

## Version 0.1 scope

- Display a 0–100 overall rating wherever the game currently shows player stars.
- Display 0–100 values for individual player attributes where practical.
- Use one deterministic conversion function across every patched screen.
- Keep raw game values untouched.
- Allow the mod to be disabled without changing a save.
- Log every patched UI surface and fail closed when compatibility checks fail.

## Explicitly out of scope

- Potential and development changes.
- Market value calculation.
- Match rating or HLTV formulas.
- AI squad-building changes.
- Writing converted values back into career data.

## Discovery checklist

- [x] Locate candidate canonical rating and attribute APIs.
- [x] Locate the main UI methods that render ratings in squad, scouting, transfers and profile UI.
- [ ] Measure the canonical raw player rating and attribute ranges at runtime.
- Verify whether hidden precision exists behind the star display.
- Capture representative players across the full rating range.
- Define the conversion only after those raw ranges are verified.

Static inspection confirms that ratings are stored or calculated as floating-point
values, but it does not prove their runtime range. The first executable build will
therefore be a read-only diagnostic probe; the 0–100 conversion remains deliberately
undefined until its logs have been checked against the game UI.

## Acceptance criteria

- The same player receives the same displayed rating on every screen.
- No save file changes after opening and closing patched screens.
- Missing or changed game methods disable the affected patch instead of crashing.
- The mapping is documented with examples and covered by pure unit tests.
