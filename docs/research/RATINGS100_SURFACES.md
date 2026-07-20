# Ratings100 game-surface research

## Scope

Static inspection of the generated IL2CPP interop assembly from the locally installed
game on 2026-07-20. This documents callable surfaces; it does not yet validate runtime
values or behavior. Decompiled game source and game binaries are not included in this
repository.

## Rating and attribute APIs

The following game APIs expose floating-point values and are candidates for the
canonical data source:

- `DataPlayer.Rating`
- `DataPlayer.GetEffectiveAttribute(AttributeType)`
- `Attributes.GetValue(AttributeType)`
- `Attributes.GetAverageAllStats()`
- `Attributes.GetWeightedRating()`
- `RatingEngine.GetRoleRating(DataPlayer)`
- `RatingEngine.ComputeRating(DataPlayer, EffectiveRole)` and its role-specific
  implementations

The actual value range and the relationship between `DataPlayer.Rating`, weighted
ratings and role ratings must be measured in a running career before choosing a
conversion formula.

## Initial UI patch targets

- `PlayerOverview`: overall rating is represented by a `UnityEngine.UI.Image`.
- `SquadPlayerRow.Init(DataPlayer)`: rating and potential are image fields.
- `ScoutingRow.Init(DataUser)`: rating, rating background, potential and potential
  background are image fields.
- `TransferRow.Init(ContractOffer)`: rating is an image field with a containing game
  object.
- `AttributeLine.Setup*`: attribute values are already rendered through `TMP_Text` and
  expose current, effective and base values.
- `HomePlayerRow.Init(int, string, string, int, string)`: at least one UI path already
  accepts an integer rating.

## Architecture decision for version 0.1

Do not patch `DataPlayer.Rating`, `RatingEngine` or saved player data. Changing those
surfaces could affect simulation, AI or economy even if the original goal is visual.

Instead, use Harmony postfixes on UI initialization/setup methods and a pure shared
converter. Rating images can be hidden or supplemented with a text label; attribute
rows can reuse their existing text component. Every patch must check the expected
method and fields and disable only itself when the game changes.

## Next experiment

Build a read-only diagnostic plugin that logs, for representative players:

- `DataPlayer.Rating`
- role rating
- weighted and average attribute ratings
- base and effective attributes
- the screen and player identifier used for each sample

Once the observed range and precision are known, define the 0–100 mapping, add pure
unit tests, and patch `SquadPlayerRow` plus `PlayerOverview` first.
