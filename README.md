# MetaBench EM26 Mods

Open-source mods and tooling for Esports Manager 2026.

## Current focus

The first original MetaBench mod is **Ratings100**: a presentation layer that
replaces the static star display with transparent 0–100 ratings without changing
save data or simulation balance.

## Repository layout

- `src/` — original MetaBench mods and shared code.
- `docs/` — roadmap, architecture decisions, research notes and compatibility data.
- `tools/` — build and packaging helpers that do not contain game files.

## Safety rules

- No game assemblies, saves, third-party mod binaries or decompiled sources are committed.
- Presentation-only features stay separate from simulation-changing features.
- Every write to career state must be explicit, configurable and reversible.
- Native match totals and synthetic round data are never mixed without a clear label.

## Status

- AutoTraining 1.2.0 has been installed and validated as an external baseline mod.
- Ratings100 0.1.0 is implemented, tested, built and installed for local runtime validation.
- Dynamic development, market value, HLTV-like ratings and the career wiki follow later.

See [the roadmap](docs/ROADMAP.md) and [Ratings100 specification](docs/RATINGS100.md).
