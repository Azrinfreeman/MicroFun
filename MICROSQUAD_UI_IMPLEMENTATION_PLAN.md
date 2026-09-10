# Micro Squad UI Redesign — Implementation Plan

Status: **planning only**  
Target scene: `Assets/Scenes/Aktivity/akt8/akt8.unity`  
Target section: `Canvas/Microsquad`  
Reference: supplied “Aktiviti 8 — Micro Squad” wheel screen

No Unity scene, script, prefab, or art asset changes are included in this planning pass.

## Goal

Rebuild the active Micro Squad section so it closely matches the reference: a warm science-themed 16:9 screen with a large green title, an instruction card on the left, a five-category microorganism wheel on the right, a fixed pointer, and a large red **PUTAR** button.

The result should retain the existing scene navigation and `Button -> SpinControl.Spin()` entry point while replacing the current sparse layout with a polished, child-friendly interface.

## Current project baseline

Read-only Unity inspection found:

- The active scene is `Assets/Scenes/Aktivity/akt8/akt8.unity`.
- `Canvas/Microsquad` is active and stretches to the full screen.
- The Canvas already uses `Scale With Screen Size` with a `1920 × 1080` reference resolution.
- The current wheel is `Canvas/Microsquad/Center/circcle`.
- The wheel is one image: `Assets/Scenes/Aktivity/akt8/puzzle/puzzles/wheel.png`.
- The wheel has `Rigidbody2D` and `SpinControl` components.
- The current red button is wired to `SpinControl.Spin()`.
- `SpinControl.CalculateResult()` currently only logs the stopping angle; it does not map the angle to a microorganism category or raise a result event.
- The section contains an active Home button and an inactive Next button that must not be broken.
- Several inactive legacy buttons and six inactive marker images remain under the current center layout.

## Visual target

### Composition

- Full-screen cream-to-yellow/green/peach background with soft radial light, subtle texture, floating bubbles, dots, stars, atom/DNA line art, and curved color fields near the bottom corners.
- Top-centered `AKTIVITI 8` capsule.
- Large `Micro Squad` title in deep-to-bright green with a thick white outline and soft drop shadow.
- Malay subtitle: `Putar roda untuk memilih mikroorganisma kumpulan anda.`
- Left instruction card with:
  - `Arahan` header and clipboard/pencil icon.
  - Step 1: `Klik butang PUTAR untuk memilih satu mikroorganisma.`
  - Dotted separator.
  - Step 2: `Setiap kumpulan akan mendapat 1 mikroorganisma.`
  - Bottom `Aktiviti kolaboratif` badge.
- Right five-segment wheel:
  - Bakteria — yellow segment.
  - Fungi — pink segment.
  - Alga — green segment.
  - Protozoa — purple segment.
  - Virus — blue segment.
- Gold rim, center hub, fixed red pointer, and large red `PUTAR` button.
- Decorative edge illustrations: green microbe, petri dish, water-drop mascot, purple virus, and small science accents.

### Layout at the 1920 × 1080 reference resolution

- Header group: centered, occupying roughly the top 28%.
- Instruction card: left-middle, approximately `580 × 510`, anchored to the left-center safe area.
- Wheel: right-middle, approximately `630–680 px` square.
- Spin button: centered beneath the wheel, minimum `360 × 110` with a large hit target.
- Decorative objects remain outside the primary reading and interaction zones.
- Preserve an accessible Home control. Restyle and position it as a small unobtrusive top-left or bottom-left navigation button so it does not compete with the reference composition.

## Technical approach

### 1. Preserve and isolate existing behavior

- Duplicate the scene before editing as `akt8_PreMicroSquadRedesign_20260829.unity`.
- Work only under `Canvas/Microsquad`; do not alter unrelated Activity 8 sections.
- Retain the current public `SpinControl.Spin()` method so the serialized button callback stays valid.
- Keep the existing Home/Next navigation callbacks and modal references intact.
- Move unused legacy visual objects into a disabled `LegacyVisuals` container instead of deleting them during the first pass.

### 2. Build a clean responsive hierarchy

Proposed hierarchy:

```text
Canvas/Microsquad
├── Background
│   ├── GradientBackdrop
│   ├── ScienceLineArt
│   └── DecorativeSprites
├── Header
│   ├── ActivityBadge
│   ├── Title
│   └── Subtitle
├── Content
│   ├── InstructionsCard
│   │   ├── CardBackground
│   │   ├── CardHeader
│   │   ├── StepOne
│   │   ├── Divider
│   │   ├── StepTwo
│   │   └── CollaborationBadge
│   └── WheelArea
│       ├── Pointer
│       ├── WheelRotator
│       │   ├── WheelFace
│       │   ├── CategoryIcons
│       │   ├── CategoryLabels
│       │   └── CenterHub
│       └── SpinButton
├── Navigation
│   ├── HomeButton
│   └── NextButton
└── LegacyVisuals (inactive)
```

- Use anchors for the header, left card, and wheel area; do not depend on fixed world positions.
- Keep the existing CanvasScaler settings unless visual QA exposes a scaling issue.
- Use `AspectRatioFitter` only for the wheel/art that must remain square; avoid nested layout groups on animated wheel objects.
- Ensure decorative Images have `raycastTarget = false`.

### 3. Asset generation and preparation

Generate cohesive transparent-background PNG illustrations based on the supplied visual direction:

- Five wheel organisms: bacteria, fungus, algae cells, protozoan, virus.
- Three edge mascots: green microbe, blue water drop, purple virus.
- Petri dish decoration.
- Optional atom/DNA line-art accents.
- Optional soft paper/science backdrop without any baked text.

Planned asset folder:

```text
Assets/Images/MicroSquadUI/
├── Background/
├── Characters/
├── Wheel/
├── Icons/
└── Navigation/
```

Asset rules:

- All readable wording stays as TextMeshPro text, not baked into images.
- Generate illustrations at 2× their intended on-screen size for clean high-DPI output.
- Use transparent padding consistently so icons align reliably.
- Import UI art as Sprite (2D and UI), enable alpha transparency, disable mipmaps for flat UI art, and use compression appropriate to the build target.
- Use 9-sliced sprites for the card, capsules, and red button where practical.

### 4. Typography and styling

- Reuse the project’s rounded Fredoka-style TMP font asset if available.
- Title: heavy rounded face, green gradient or layered green text, white outline, and soft shadow.
- Body: dark forest green for strong contrast on cream.
- Wheel labels: bold dark green or white with a thin dark outline depending on segment brightness.
- All Malay text remains editable in the Inspector and should be grouped for later localization.
- Avoid generated lettering so spelling and kerning remain controllable.

### 5. Wheel implementation

The visual reskin and result behavior should be handled separately.

#### Required visual work

- Replace the current wheel bitmap with a true five-segment face matching the reference palette.
- Keep the pointer outside `WheelRotator` so it remains stationary.
- Rotate one parent (`WheelRotator`) containing the face, icons, and labels to prevent visual drift.
- Keep label/icon orientation readable by pre-rotating their containers within each segment.
- Add rim highlights, center hub depth, and a subtle shadow without blocking raycasts.

#### Recommended behavior upgrade

- Retain `SpinControl.Spin()` as the button-facing API.
- Change the internal spin from free Rigidbody stopping to a deterministic eased rotation, or snap the final physics angle to a segment center after deceleration.
- Select one of five categories uniformly.
- Animate approximately 3–5 turns over 3.5–5 seconds with an ease-out curve.
- Disable the spin button while spinning and restore it after result delivery.
- Land the chosen segment center under the fixed pointer.
- Add a serializable category list and an `OnCategorySelected` UnityEvent.
- Map results to `Bakteria`, `Fungi`, `Alga`, `Protozoa`, and `Virus`.
- Keep sound and particle hooks optional so the screen works without them.

This behavior upgrade is recommended because the current implementation only logs an angle and the target UI communicates a meaningful five-way selection. If the requested scope is strictly visual, the existing Rigidbody behavior can remain for the first milestone.

### 6. Interaction polish

- Spin button states: normal, highlighted, pressed, and disabled.
- Add a small press scale (`1.0 -> 0.96 -> 1.0`) and restrained highlight.
- Optional pointer tick sound while segment boundaries pass the pointer.
- Optional result pulse/glow on the selected segment.
- Respect keyboard/controller navigation through the EventSystem.
- Minimum interactive target size: 64 px at the reference resolution.
- Do not place important controls inside decorative edge-safe zones.

## Phases, effort, and deliverables

“Reasoning effort” refers to the recommended Codex effort setting for that phase. Time is an implementation estimate, not elapsed AI runtime.

| Phase | Work | Recommended reasoning effort | Estimated implementation time |
|---|---|---:|---:|
| 0. Safety and baseline | Scene backup, callback inventory, current Game-view capture, console baseline | Medium | 0.5–1 h |
| 1. Art direction | Color/type tokens, 1920×1080 wireframe, hierarchy plan | High | 1–1.5 h |
| 2. Asset generation | Generate/import mascots, organism icons, petri dish, background accents; alpha cleanup | High | 2.5–4 h |
| 3. Native UI build | Background, header, instruction card, badges, responsive layout, navigation integration | High | 3–4.5 h |
| 4. Wheel reskin | Five segments, rim/hub/pointer, icon and label placement, spin button | High | 2–3 h |
| 5. Wheel behavior | Deterministic five-way selection, button lock, result event, legacy hook preservation | High | 2–3 h |
| 6. Polish | Button states, subtle motion, optional audio/VFX hooks | Medium | 1–2 h |
| 7. QA | Compile/console checks, Play Mode interaction, multi-resolution screenshots, fixes | High | 1.5–2.5 h |

Estimated total:

- **Visual-only implementation:** approximately **10–15 hours**.
- **Visual plus robust five-category result behavior:** approximately **13–20 hours**.
- **Recommended overall Codex effort:** **High**.

## Verification plan

### Functional

- `PUTAR` triggers exactly one spin per click.
- Repeated clicks during a spin are ignored or the button is visibly disabled.
- The wheel stops with one segment centered under the pointer.
- Each of the five categories can be selected.
- Category mapping remains correct after multiple full rotations.
- Home and existing Activity 8 navigation still work.
- No unrelated section under the Activity 8 Canvas changes state unexpectedly.

### Visual

- Compare Game-view captures to the reference at:
  - 1920 × 1080.
  - 1366 × 768.
  - 2560 × 1440.
  - 3840 × 2160.
  - One 16:10 resolution to catch edge-cropping.
- Confirm title, instruction card, wheel, and button retain the same visual hierarchy.
- Confirm all organism labels remain readable and upright while stationary.
- Confirm edge mascots do not overlap controls or text.
- Confirm no generated image contains accidental lettering or visual artifacts.

### Technical

- Zero new Console errors after compilation and Play Mode testing.
- No missing sprite, TMP font, material, or serialized references.
- No decorative Image intercepts pointer input.
- No duplicated EventSystem.
- Scene validation passes and the final scene is saved only after review.

## Acceptance criteria

- The Micro Squad section is immediately recognizable as the supplied reference at a glance.
- Layout, color, typography, instruction card, five-part wheel, pointer, and red `PUTAR` button closely match the reference composition.
- Text remains native TMP and editable.
- The UI scales cleanly from HD to 4K without overlaps or cropping.
- Existing scene navigation remains functional.
- The wheel remains interactive; with the behavior upgrade, it returns one valid microorganism category per spin.
- No other Activity 8 section is visually or functionally regressed.

## Execution boundary

This document is the complete proposed implementation. Execution should begin only after approval. The first execution action will be a scene backup and baseline screenshot; generated art, hierarchy changes, script edits, and scene saving will follow only in their listed phases.
