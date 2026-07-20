# MetaBench Ratings100

Displays Esports Manager 2026 person ratings, potential and attributes on a 0–100
scale. The conversion is intentionally direct: every original 0–20 value is
multiplied by five.

All displayed values are rounded to whole numbers. Potential uses its native 0–1
storage range and is displayed as 0–100. Star graphics are fully hidden rather than
left behind the numeric label.

The mod changes UI only. It does not write converted values to saves and does not
change match simulation, development, transfers or AI decisions.

## Build

Install BepInEx IL2CPP and start the game at least once, then set the game path:

```sh
export EM26_GAME_DIR="/path/to/Esports Manager 2026"
dotnet build src/MetaBench.Ratings100/MetaBench.Ratings100.csproj -c Release
```

Copy `MetaBench.Ratings100.dll` to:

```text
Esports Manager 2026/BepInEx/plugins/MetaBench.Ratings100/
```

## Covered UI

- Player and staff overviews.
- Squad player and staff rows.
- Scouting rows and the legacy scouting table.
- Transfer history/list rows.
- Organization CEO rating.
- Member selector and home player ranking rows.
- Player potential stars.
- Player/staff attribute lines, comparison lines and training attribute rows.
