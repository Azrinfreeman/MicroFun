# Activity 5 — Faktor Redesign

## Status

Implemented in `Assets/Scenes/Aktivity/akt5/akt5.unity`.

## Design

The Faktor page now uses the same colorful Activity 5 science theme as the Center and Pengenalan pages. It contains four large factor buttons:

1. `Suhu`
2. `Kehadiran Udara`
3. `Kehadiran Nutrien`
4. `Keasidan yang Sesuai`

The `Kehadiran Air` button is intentionally omitted. Its existing content page remains in the scene but is inactive and cannot be reached from the redesigned Faktor menu.

## Navigation

- Each factor button hides `Canvas/Faktor` and activates its matching existing content page.
- The `AKTIVITI` back button hides `Canvas/Faktor` and returns to `Canvas/Center`.
- Every generated button stores two persistent Unity callbacks.

## Files

- Builder: `Assets/Editor/Akt5FaktorUIBuilder.cs`
- Scene: `Assets/Scenes/Aktivity/akt5/akt5.unity`
- Backup: `Assets/Scenes/Aktivity/akt5/Backups/akt5_PreFaktorRedesign_20260902.unity`
- Shared background: `Assets/Images/Akt5CenterUI/Background/akt5_factor_background_v2.png`

## Rebuild

Run `Tools > MicroFun > Activity 5 > Build Faktor Theme` inside Unity. The builder is idempotent: it replaces `Akt5FaktorThemeRoot` instead of duplicating it.

## Visual polish pass

- Removed the three extra foreground decorations that duplicated artwork already present in the shared background.
- Narrowed and repositioned the Keasidan label area so it no longer overlaps the pH icon or arrow.
- Reduced the Keasidan label font size and tightened the arrow badge.
- Increased header and instruction readability for future builder runs.

## Effort used

- Existing hierarchy and destination-page audit: medium.
- Four-button themed UI implementation: high.
- Persistent callback wiring and omission of `Kehadiran Air`: medium.
- Compilation, serialized hierarchy, and cleanup verification: medium.
