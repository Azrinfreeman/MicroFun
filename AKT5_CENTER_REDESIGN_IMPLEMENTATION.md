# Activity 5 Center — Reference-Style Redesign Implementation

## Status

Corrected and implemented on 2 September 2026 using the user's final Activity 5 reference. The center now uses a factor-themed edge illustration, a large lesson heading, four vertically stacked navigation capsules, and a curriculum badge. Existing navigation callbacks remain preserved.

## Final Reference Correction

- Removed the central student hero, orbital route, numbered badges, and split-column layout from the earlier reference.
- Added the title `FAKTOR PERTUMBUHAN MIKROORGANISMA`.
- Added four stacked buttons: Pengenalan, Faktor, Micro-Scene, and Kuiz.
- Applied green, orange, blue, and pink button treatments with circular icons and gold arrows.
- Added the `SP: 3.1 / SK: 3.1.4` badge.
- Replaced the background with `Assets/Images/Akt5CenterUI/Background/akt5_factor_background_v2.png`.
- Preserved the back route to Activities and all four existing section routes.

## Goal

Redesign the Activity 5 center scene to follow the user's final factor-themed reference: a prominent Activity 5 header, large microorganism-growth title, four stacked navigation capsules, curriculum badge, friendly mascots, and science-factor props around a clear central UI area.

The reference controls the visual language and composition. It does **not** authorize copying conflicting wording such as `AKTIVITI 7`, nor inventing destinations that do not exist in Activity 5. Existing Activity 5 labels, scene routes, completion state, and callbacks remain the source of truth.

## Target

- Primary scene: `Assets/Scenes/Aktivity/akt5/akt5.unity`
- Known related scenes to verify during audit:
  - `Assets/Scenes/Aktivity/akt5/microscene5.unity`
  - `Assets/Scenes/Aktivity/akt5/kuiz5.unity`
- Target canvas: 1920 × 1080, `Scale With Screen Size`, 0.5 width/height match
- Typography: existing Fredoka One TMP asset used by the other redesigned activities

## Reference-to-Activity-5 Mapping

| Reference element | Activity 5 implementation |
|---|---|
| Large `AKTIVITI` heading | `AKTIVITI 5` badge and the existing Activity 5 lesson title |
| Eight colored numbered capsules | Reuse only the actual Activity 5 sections/buttons found in the hierarchy; distribute them across the left and right columns |
| Central student illustration | New text-free science exploration hero image inside a layered circular frame |
| Colored orbital connector | Decorative progress route linking the real Activity 5 buttons without blocking clicks |
| Highlighted completed/current item | Visual state driven by existing completion data if available; otherwise a static current-page highlight only |
| Mascots, bubbles, leaves, sparkles | Shared decorative background layer with safe UI margins |
| Check mark and trophy treatment | Apply only where an existing completion/challenge destination supports it |

## Phase 1 — Read-Only Scene and Navigation Audit

Before any write operation:

1. Connect to the correct Unity instance and confirm the editor is not compiling or in Play Mode.
2. Inspect the complete `akt5` hierarchy, including inactive objects.
3. Identify:
   - Canvas and scaler settings
   - Current title and subtitle objects
   - Every navigation button
   - Button labels and icons
   - Persistent and runtime-wired callbacks
   - Back-button destination
   - Completion/progress components
   - Scene loader and fade panel
4. Confirm every referenced destination exists in Build Settings.
5. Inspect `microscene5` and `kuiz5` only far enough to verify the intended routes.
6. Record all serialized references before restructuring.

No existing button will be replaced until its callback strategy is known. Runtime-wired buttons will be restyled in place.

## Phase 2 — Art Direction and Assets

Create project-bound raster assets without embedded text or buttons:

### Shared background

- Soft lime-green left side, warm cream center, peach-orange right side
- Friendly microorganism mascots around the outer edges
- Bubbles, sparkles, leaves, atom/DNA motifs, microscope and petri-dish accents
- Calm central and navigation areas for readable Unity UI
- No words, numbers, logos, or watermark

Suggested destination:

`Assets/Images/Akt5CenterUI/Background/akt5_center_background.png`

### Central hero

- Diverse primary-school students exploring microorganisms together
- Microscope, magnifying glass, notebook, flask, and petri dish
- Transparent or clean isolated composition suitable for a circular mask
- No text or logos

Suggested destination:

`Assets/Images/Akt5CenterUI/Hero/akt5_students_science_hero.png`

### Reused project assets

Prefer existing transparent science icons for books, microscope, clipboard, quiz, magnifier, and trophy when they already match the project style. Generate only missing assets, and import them as sprites with alpha transparency preserved.

## Phase 3 — Reusable Unity Editor Builder

Create:

`Assets/Editor/Akt5CenterMenuUIBuilder.cs`

Menu command:

`Tools/MicroFun/Activity 5/Build Center Reference Theme`

Builder responsibilities:

1. Refuse to run during Play Mode.
2. Create a recoverable scene backup before editing.
3. Load `akt5.unity` and remember the previously active scene.
4. Configure the Canvas for 1920 × 1080 responsive scaling.
5. Create an idempotent root such as `Akt5CenterThemeRoot`.
6. Add the generated background as the first UI sibling.
7. Construct the header, central hero frame, connector route, and decorative layers.
8. Restyle and reposition the existing buttons without destroying serialized references.
9. Preserve all `Button.onClick`, SceneLoader, audio, and completion-state bindings.
10. Add or repair a visible back button only after its correct destination is confirmed.
11. Keep the fade panel as the final sibling so transitions remain functional.
12. Save, validate, and restore the previously active scene.

The builder must be safe to rerun: generated objects are replaced by name, while original functional objects are retained.

## Phase 4 — Planned Layout

### Header

- Small top badge: `MODUL SAINS TAHUN 6`
- Main title: `AKTIVITI 5`
- Optional lesson subtitle sourced from the current Activity 5 scene
- Back button in the upper-left safe area

### Central focus

- Large circular hero frame centered slightly below the title
- White/cream inner ring with green, blue, orange, and purple orbit accents
- Central illustration cropped with a circular mask
- Small connector nodes aligned with the surrounding buttons

### Navigation capsules

- Existing Activity 5 sections split into balanced left and right columns
- Large rounded glossy capsules with:
  - Number or section marker
  - Transparent icon
  - Existing label
  - Optional arrow/check indicator
- Palette rotation: purple, blue, green, yellow, teal, orange, pink, and royal blue
- Minimum practical hit area and no decorative image blocking raycasts

If Activity 5 contains fewer than eight real sections, the layout will use fewer, larger capsules. Empty or fake buttons will not be introduced merely to match the reference count.

## Phase 5 — Navigation and State Preservation

The implementation must preserve:

- Current destinations for every section
- `microscene5` and `kuiz5` routes where already assigned
- Existing additive or single-scene loading behavior
- Fade transitions
- Audio callbacks
- Completion/check state
- Back navigation

If the current scene has broken or missing bindings, repair them only when the intended target can be proven from scripts, Build Settings, or neighboring Activity scenes. Any ambiguous destination will be reported rather than guessed.

## Phase 6 — Visual and Functional QA

### Edit Mode

- Builder compiles with no new errors
- Scene validation reports no missing scripts or broken references
- Generated sprites use the correct import type and transparency
- No duplicate generated roots after rerunning the builder

### Runtime

- Capture a 16:9 Game View screenshot
- Verify all navigation buttons click once and load the intended destinations
- Verify the back button returns to the confirmed parent scene
- Verify fade panels do not block interaction when transparent
- Verify completion states remain visible and accurate
- Verify title and labels remain readable at 1920 × 1080 and common 16:9 resolutions
- Check for clipped text, overlapping capsules, stretched icons, and accidental raycast blockers
- Confirm the Unity console contains no new errors

## Deliverables After Approval

- Updated `akt5.unity`
- Pre-redesign scene backup
- `Akt5CenterMenuUIBuilder.cs`
- Generated background and central hero artwork
- Imported sprite settings
- Runtime QA screenshots
- Final validation and callback audit summary

## Codex Sol Effort Recommendation

| Phase | Recommended effort | Reason |
|---|---:|---|
| Hierarchy and callback audit | High | Prevents loss of existing navigation and serialized references |
| Background and hero artwork | High | Requires a polished match to the established child-friendly theme |
| Editor builder and responsive layout | High | Multiple layered elements and idempotent scene construction |
| Navigation/completion preservation | XHigh | Use when callbacks are runtime-wired, ambiguous, or state-dependent |
| Runtime QA and refinement | High | Requires screenshot-driven iteration and destination testing |

Recommended default: **Codex Sol — High effort**.

Use **XHigh** during the navigation/state phase only if the audit finds unclear scene destinations, duplicated managers, or completion logic spread across multiple scripts.

## Out of Scope

- Creating nonexistent Activity 5 lesson modules
- Renaming scenes without evidence
- Rewriting lesson content or quiz questions
- Changing unrelated Activity scenes
- Replacing working navigation architecture solely for visual reasons

## Execution Result

Completed after user approval. The final scene contains four real Activity 5 destinations—Pengenalan, Faktor, Micro-Scene, and Kuiz—plus a back button to Activities. Scene validation reports zero issues.
