# Activity 5 — Pengenalan Redesign

## Status

Implemented and verified in `Assets/Scenes/Aktivity/akt5/akt5.unity`.

## Design implemented

- Activity 5 badge and green `PENGENALAN` header.
- Central prompt: `Apakah yang menyebabkan sup ini cepat basi?`
- Four simultaneous Ali/Aina dialogue cards discussing temperature, nutrients, and acidity.
- New transparent character-and-soup artwork positioned behind the discussion UI.
- Three factor chips: `Suhu`, `Nutrien`, and `Keasidan`.
- Bottom trigger question for classroom discussion.
- Existing `AKTIVITI` back navigation and `FAKTOR` forward navigation retained.

## Implementation

- Builder: `Assets/Editor/Akt5PengenalanUIBuilder.cs`
- Generated artwork: `Assets/Images/Akt5PengenalanUI/akt5_pengenalan_students_soup_v2.png`
- Theme background: `Assets/Images/Akt5CenterUI/Background/akt5_factor_background_v2.png`
- Preview: `Assets/Screenshots/Akt5PengenalanUI/akt5_pengenalan_final_v2.png`
- Scene backup: `Assets/Scenes/Aktivity/akt5/Backups/akt5_PrePengenalanRedesign_20260902.unity`

The builder creates one idempotent `Akt5PengenalanThemeRoot`, hides the previous Pengenalan presentation objects without deleting them, preserves the original navigation callbacks, and saves the scene.

## Verification

- Builder re-run: one theme root; no duplicate generated hierarchy.
- `AKTIVITI`: returns from Pengenalan to Center.
- `FAKTOR`: moves from Pengenalan to Faktor.
- Both buttons retain two persistent callbacks.
- Unity scene validation: zero missing scripts, broken prefabs, or other scene issues.
- Builder compilation: zero errors.

## Effort used

- Visual hierarchy and asset audit: medium.
- Character/soup artwork generation and alpha correction: high.
- Unity UI builder and layout tuning: high.
- Runtime navigation and idempotency checks: medium.

## Maintenance

Run `Tools > MicroFun > Activity 5 > Build Pengenalan Discussion Theme` whenever this page needs to be rebuilt. Update the constants near the top of `Akt5PengenalanUIBuilder.cs` to swap assets while keeping the hierarchy and callbacks intact.
