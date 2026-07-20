# Ratings100 — first version

## Goal

Replace the player-facing star scale with a FIFA-like 0–100 scale while leaving the
underlying save values and game simulation unchanged.

## Version 0.1 scope

- Display a 0–100 overall rating wherever the game currently shows player or staff stars.
- Display potential stars and individual attributes on the same scale.
- Use one deterministic conversion function across every patched screen.
- Keep raw game values untouched.
- Allow the mod to be disabled without changing a save.
- Log every patched UI surface and fail closed when compatibility checks fail.

## Explicitly out of scope

- Potential and development mechanics (only their displayed values are converted).
- Market value calculation.
- Match rating or HLTV formulas.
- AI squad-building changes.
- Writing converted values back into career data.

## Discovery checklist

- [x] Locate candidate canonical rating and attribute APIs.
- [x] Locate the main UI methods that render ratings in squad, scouting, transfers and profile UI.
- [x] Adopt the game's native 0–20 person/attribute scale requested for version 0.1.
- [x] Define a direct `display = round(raw × 5)` conversion clamped to 0–100.
- [x] Cover the primary profile, squad, scouting, transfer and training surfaces.
- [ ] Perform visual runtime validation after restarting the currently running game.

Version 0.1 deliberately uses no normalization or hidden weighting. A native rating
of 16.5 is shown as 83; 20 is shown as 100. The conversion is presentation-only.

## Acceptance criteria

- The same player receives the same displayed rating on every screen.
- No save file changes after opening and closing patched screens.
- Missing or changed game methods disable the affected patch instead of crashing.
- The mapping is documented with examples and covered by pure unit tests.

## Implementation status

`MetaBench.Ratings100` 0.1.0 builds against the installed IL2CPP interop assemblies.
Pure conversion tests pass, and the built DLL is installed locally. Runtime loading
and layout validation require a game restart because BepInEx cannot hot-load a plugin
into the game process that was already running during installation.
