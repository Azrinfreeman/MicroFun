# MicroScene4 — visual redesign implementation plan

Status: IMPLEMENTED AND VERIFIED — 7 September 2026, after approval.  
Target: Assets/Scenes/Aktivity/akt4/Microscene4.unity  
Scope: presentation only; preserve every existing game mechanic.  
Reference: supplied blue/orange petri-dish simulation illustration.

## 1. Verified current scene

Read-only Unity MCP inspection confirmed the active scene is Microscene4, in Edit Mode, without unsaved changes at inspection time.

Existing gameplay:
- SimulationManager: water present/absent, air sufficient/limited, and 1/2/3-day selection.
- Environment A always uses water + sufficient air. Environment B uses the selected factors.
- Mainkan Masa runs a 3-second simulation with progress slider/day text. Repeated play requests during an active run are ignored.
- Three microbes initially; maximum 18. Existing random placement and visual scale are retained.
- Results and feedback are written into existing TMP references; all inspected SimulationManager references are assigned.
- Two observation drop slots and four draggable answers: bertambah, sedikit, tidak_berubah, berkurang.
- Slot 1 expects bertambah; slot 2 expects berkurang. Preserve these IDs and answer evaluation.
- BackController supplies runtime navigation: additive unload when another scene is loaded, otherwise the serialized fallback akt1. Do not silently change that fallback during a styling task.

Relevant files:
- Assets/Scenes/Aktivity/akt4/SimulationManager.cs
- Assets/Scripts/DraggableAnswer.cs
- Assets/Scripts/DropSlot4.cs
- Assets/Scripts/BackController.cs
- Assets/Prefabs/MicrobeImage.prefab

## 2. Visual direction

Match the reference's friendly illustrated science style:
- Pale green/cream background with restrained microbes, bubbles and plant decorations.
- Navy activity badge, large rounded white title with navy outline, dark-green subtitle.
- Blue Persekitaran A card and orange Persekitaran B card.
- Glass petri-dish illustrations behind the real spawned microbes.
- Large green play control with white triangle, clock accent and MAINKAN MASA label.
- Rounded cream information panels, clear blue/orange accents and readable Malay text.
- Green observation/comparison styling, with the existing interactive answer exercise retained.

Use AKTIVITI 4 and MICRO-SCENE for the heading. The reference's MICRO-SCENE 1 numbering is not authority to rename the current scene or activity.

## 3. Layout adapted to the actual mechanics

The reference is a style guide, not a replacement for functional controls.

Use a 1920 × 1080 design with:
- Header and reserved top-left KEMBALI control.
- Middle-left: Persekitaran A, fixed-condition label, petri dish, live microbe container and result text.
- Middle-right: Persekitaran B, selected-condition labels, petri dish, live microbe container and result text.
- Middle-centre: the existing water, air and day selectors, green Mainkan Masa button, existing progress slider/day text.
- Lower full-width observation area: both original questions, two drop slots, four draggable answer chips and shared feedback.

Keep every existing functional control visible and accessible. If fitting requires less decoration or a smaller header, reduce those first; do not hide the exercise behind a new screen, remove factor controls, or require new navigation.

Use approximately 28–34 design pixels for body text and 26–30 for controls; validate actual wrapping. Keep the editable text separate from illustrations.

## 4. Reference details that must not change the game

| Reference element | Planned adaptation |
|---|---|
| A has water + air | Matches existing fixed control environment. |
| B says no water / limited air | Show the actual selected B conditions, including water + sufficient air when selected. |
| B always has dry ground | Use neutral/changeable decoration; do not visually imply dryness when water is selected. |
| Bilangan mikroorganisma | Represent through the existing spawned microbes/results; do not invent a separate unverified counter. |
| Saiz koloni | Not a calculated mechanic. Do not add a numeric colony-size metric. Existing random sprite scale is not colony growth data. |
| Kadar pembiakan | Do not invent a live rate or alter growth equations. |
| Observe after 1–3 days | Retain the current day buttons and simulated day labels, not real elapsed days. |
| Bandingkan button/banner | Style the existing comparison/observation area; no new scoring, modal or exercise replacement. |

## 5. Gameplay invariants

Do not change SimulationManager's formulas, thresholds, timers, defaults, spawn limits, selected factors, progress behavior or replay behavior.

Expected final microbe counts with current settings:

| B conditions | 1 day | 2 days | 3 days |
|---|---:|---:|---:|
| Water + sufficient air | 8 | 13 | 18 |
| Water + limited air | 5 | 7 | 9 |
| No water + sufficient air | 4 | 5 | 6 |
| No water + limited air | 2 | 2 | 2 |

A remains 8 / 13 / 18 respectively, regardless of B selection. Preserve existing result wording, including A's current fixed “Pertumbuhan: Banyak” label.

Preserve:
- All seven factor/day button callbacks and the play callback, without duplicate listeners.
- Existing result, progress, factor-label and feedback references.
- Both containers as the objects cleared/spawned by the simulation.
- DraggableAnswer IDs, CanvasGroup behavior, return-to-origin behavior and DropSlot4 validation.
- Current feedback timing and correct/incorrect states.
- The real Back object/component; it receives its listener at runtime, so copying only persistent events would lose navigation.

Any pre-existing gameplay inconsistency discovered during verification must be reported separately, not silently fixed in this visual pass.

## 6. Scene and asset implementation

After approval:
1. Record the current hierarchy, serialized gameplay references, callbacks, Canvas settings and baseline behavior.
2. Save a unique dated scene-copy backup including any newly unsaved work; abort if backup fails.
3. Add a dedicated editor styling builder for Microscene4, guarded against other scene paths.
4. Prefer restyling/repositioning existing UI objects over replacing them. Preserve object identity for referenced controls.
5. Add decorative objects under clearly owned roots, outside the microbe containers. The simulation destroys every child of those containers, so dish art, labels and frames must not be children there.
6. Keep microbe containers' local spawn coordinates and dimensions compatible with the existing -150..150 / -60..60 bounds.
7. Reuse appropriate art; generate only missing petri-dish/environment illustrations after approval. No labels or pre-painted counted microbes inside dish backgrounds.
8. If changing the microbe appearance, make a scene-specific visual prefab variant. Retain its RectTransform and behavior, avoid modifying the shared original used by other scenes.
9. Use conditional sprite imports, genuine alpha checks and correct non-power-of-two dimensions. Keep originals and use versioned asset names.
10. Save only Microscene4 after successful checks. Re-running the builder must not duplicate decoration or listeners.

No global Canvas, shared material, shared font asset or project setting changes are planned. A small presentation-only component may reflect selected factors if needed, but must not calculate growth or modify simulation state.

## 7. Critical drag-and-drop layout constraint

DraggableAnswer reparents dragged chips to the root Canvas and divides pointer movement by canvas.scaleFactor.

Therefore:
- Do not apply the previous lesson's additional nested page-scale fitter to draggable content blindly.
- Preserve the root Canvas coordinate system for chips and slots, or use compatible anchors/layout with no extra ancestor scaling.
- Keep drop-slot hit areas exposed. Decorative Images must have raycastTarget=false.
- Preserve CanvasGroup and parent relationships needed for original-position restoration.
- Verify dragging at both target aspect ratios; a screenshot alone is insufficient.

## 8. Efficient Unity MCP workflow

- Use the Unity MCP skill and batch_execute for independent audits and final checks.
- Reuse this audit; inspect only newly changed or unresolved objects.
- Group source/asset changes, compile once if needed, then run a single bulk styling action.
- Avoid repetitive hierarchy/console checks and unnecessary refreshes.
- Use actual Game View ScreenCapture including overlay UI for final previews.
- Wait for Canvas layout and rendering to settle after resolution changes. Temporarily enable background rendering only for verification if necessary, then restore it.
- Restore original editor scene, visibility, Game View settings and Edit Mode after testing.

## 9. Acceptance tests after implementation

Visual:
- Blue/orange two-environment composition, outlined title, green play control and illustrated dish backgrounds resemble the reference.
- All selectors, progress, results, observation slots, answer chips, feedback and Back remain visible.
- No clipping, overflow, distorted artwork or click-blocking decorations at 1920×1080 and 16:10.
- Spawned microbes stay visibly inside the dish areas for the entire existing random-position/scale range.

Mechanical:
- Compare before/after runs for all 12 B-condition/day combinations using the table above.
- Confirm initial counts, progress start/end, selected-day labels, replay and double-click protection.
- Changing B factors must not affect A.
- Test correct and incorrect answers for both slots, release outside a slot, and dragging after layout resizing.
- Verify navigation in the normal additive entry flow and the existing standalone fallback.
- Check for missing references/scripts and new console errors, without clearing unrelated logs.
- Compare original serialized gameplay settings/callbacks to the backup.
- Deliver final screenshots and actual test results; record any pre-existing failures separately.

## Current execution status

Implemented in `Assets/Scenes/Aktivity/akt4/Microscene4.unity`.

### Delivered

- Illustrated blue/orange environment cards with empty glass-dish artwork behind the existing live microbes.
- Navy-outlined title, activity badge, green play control, rounded selectors, observation chips, and decorative microbes.
- All original interactive objects preserved; no gameplay source or shared microbe-prefab changes.
- Scene-only editor builder: `Assets/Editor/MicroScene4StyleBuilder.cs`. It validates the target, creates a dated scene backup, and applies the layout in one bulk Unity MCP operation. Re-running updates named decoration instead of duplicating it.
- Editor-only regression helper: `Assets/Editor/MicroScene4StyleQA.cs`.
- New artwork and title material: `Assets/Images/MicroScene4UI/`.
- Baseline backup: `Assets/Scenes/Aktivity/akt4/Microscene4_before_style_20260907_064332.unity`. Additional pre-apply backups remain available.

### Verification results

- Before and after: all 12 water/air/day combinations passed the expected growth table.
- Initial 3/3 counts, repeat-play guard, replay, final progress and selected-day labels passed.
- Both slots passed correct and incorrect answers; outside-drop restoration passed.
- Drag displacement and return behavior passed at 1920x1080 and 1164x728 (16:10 Game View).
- No offscreen buttons at Full HD; all active button centres passed UI raycast accessibility checks at 16:10.
- Screenshots reviewed at both sizes: no control clipping; existing random microbe visuals remain in the illustrated dishes in the reviewed runs.
- Compared 35 serialized gameplay settings, object/asset references, persistent callbacks, answer IDs, Back fallback and Canvas scaling entries against the original backup: all matched.
- Original AKT4 entry button opened the simulation additively. Back unloaded it and restored the AKT4 canvas, camera and EventSystem.
- Standalone Back returned to the existing `akt1` fallback, intentionally unchanged.
- No compilation errors or missing scripts. Existing video assets produced WindowsMediaFoundation colour-primaries messages (logged as errors) and H.264 timestamp warnings during navigation; videos were not modified.
- Original Game View size restored; returned to Microscene4 in Edit Mode.

### Previews

- [Full HD](Captures/MicroScene4/MicroScene4_1920x1080.png)
- [16:10](Captures/MicroScene4/MicroScene4_16x10.png)

### Artwork provenance and workflow

Used the unity-mcp-orchestrator skill for the scene-safe, bulk-editor workflow and imagegen skill for the two empty petri-dish illustrations. Generated artwork is `petri_atlas_v1.png`, sliced non-destructively into two Sprite assets. Labels, buttons and counted microbes are real Unity UI objects, not baked into the illustration. Existing project artwork and fonts were reused without modifying shared source assets.
