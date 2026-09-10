# Aktiviti 8 — Micro Thinker UI Redesign Implementation Plan

Status: **Planning only — do not execute yet**

Target scene: `Assets/Scenes/Aktivity/akt8/akt8.unity`

Target hierarchy: `Canvas/MicroThinker`

Reference resolution: **1920 × 1080 (16:9)**

Recommended Codex Sol effort: **High**

## 1. Objective

Redesign the current `MicroThinker` section to closely reproduce the supplied reference:

- soft lime, cream, and peach science-themed background;
- large green **Micro Thinker** title and Aktiviti 8 badge;
- instruction card on the left;
- large central mind-map board;
- four colored branches: **Jenis**, **Contoh**, **Kepentingan**, and **Ciri**;
- three editable answer fields for each branch;
- professor-brain mascot and “Exercise Your Mind” treatment on the right;
- “Aktiviti individu” badge at the bottom;
- decorative microbiology objects around the perimeter.

The foreground UI will remain native Unity uGUI/TMP so labels and input fields stay sharp and functional at different resolutions. Generated raster art will be used only for the illustrated background and decorative cutouts.

## 2. Current Scene Audit

`Canvas/MicroThinker` currently contains:

- `RedesignRoot` — active wheel/spinner UI with `SpinControl`, a PUTAR button, pointer, and result label;
- `Center` — disabled legacy selection UI;
- `Button` — disabled legacy SPIN button;
- `Bottom` — existing navigation container;
- four disabled legacy result panels:
  - `MerosakkanMakanan`;
  - `MereputkanGigi`;
  - `Merosakkan Tanaman`;
  - `Menyebabkan Keracunan Makanan`;
- `HomeButton` — active and currently returns from `MicroThinker` to `Canvas/Center`.

Important behavior constraints:

- Preserve the existing `HomeButton` return behavior.
- Do not connect or change the center-menu `MICRO THINKER` button callback yet. It must remain without a callback until requested separately.
- Do not delete the existing spinner or legacy content immediately. Move it under a disabled backup container so recovery remains possible.

## 3. Proposed Hierarchy

```text
Canvas
└── MicroThinker
    ├── MicroThinkerMindMapRoot
    │   ├── Background
    │   ├── DecorativeLayer
    │   │   ├── BlueBacterium
    │   │   ├── GreenMicrobe
    │   │   ├── Sparkles
    │   │   ├── LeftLightbulb
    │   │   ├── BottomMicroscope
    │   │   ├── TestTubeRack
    │   │   ├── RightFlaskAndLeaves
    │   │   └── FloatingMagnifier
    │   ├── Header
    │   │   ├── ActivityBadge
    │   │   ├── Title
    │   │   └── SubtitleCapsule
    │   ├── InstructionsCard
    │   │   ├── BulbIcon
    │   │   ├── CardTitle
    │   │   ├── Step01
    │   │   ├── Step02
    │   │   ├── Step03
    │   │   └── LeafAccent
    │   ├── MindMapBoard
    │   │   ├── BoardBackground
    │   │   ├── ConnectorLayer
    │   │   ├── CentralNode
    │   │   ├── JenisBranch
    │   │   │   ├── BranchIcon
    │   │   │   ├── BranchLabel
    │   │   │   └── AnswerFields
    │   │   │       ├── Answer01
    │   │   │       ├── Answer02
    │   │   │       └── Answer03
    │   │   ├── ContohBranch
    │   │   ├── KepentinganBranch
    │   │   └── CiriBranch
    │   ├── RightMascotPanel
    │   │   ├── ProfessorBrain
    │   │   ├── IdeaBulb
    │   │   └── ExerciseYourMindText
    │   └── IndividualActivityBadge
    ├── HomeButton
    └── LegacyMicroThinkerContent   [inactive]
        ├── RedesignRoot
        ├── Center
        ├── Button
        ├── Bottom
        └── LegacyResultPanels
```

## 4. Visual Specification

### Background

- Full-screen text-free illustration.
- Pale yellow/cream center for readability.
- Light lime glow on the left and warm peach glow on the right.
- Decorative science artwork kept near the outer edges so it does not compete with the mind map.
- No generated text inside the background asset.

### Header

- Top-center pale-green capsule: `AKTIVITI 8`.
- Main title: `Micro Thinker`, dark-to-light green styling with white outline and soft shadow.
- Subtitle inside a cream dashed capsule:
  `Lengkapkan peta minda berdasarkan pembelajaran tentang mikroorganisma.`

### Instruction Card

- Rounded cream panel with green outline and soft shadow.
- Green pill title: `Arahan:`
- Three numbered steps:
  1. `Baca semula maklumat dalam modul.`
  2. `Lengkapkan peta minda.`
  3. `Kongsi jawapan bersama rakan.`
- Decorative lightbulb above the card and leaf accent at its lower-right corner.

### Mind-Map Board

- Large warm-white rounded board with subtle beige border and shadow.
- Center node: green capsule labeled `Mikroorganisma`.
- Four branch nodes and connector colors:
  - `Jenis` — orange/yellow;
  - `Contoh` — pink;
  - `Kepentingan` — cyan/blue;
  - `Ciri` — lavender/purple.
- Each branch receives three rounded answer fields matching its branch color.
- Small branch illustrations:
  - microscope for Jenis;
  - petri dish for Contoh;
  - health shield for Kepentingan;
  - magnifying glass for Ciri.
- Curved connectors should terminate cleanly at node edges without crossing labels.

### Right Mascot and Footer

- Friendly professor-brain mascot with glasses and a raised finger.
- Small glowing idea bulb above the raised finger.
- Native TMP treatment for:
  - `Exercise`
  - `Your Mind`
- Bottom-center cream badge with green person icon and `Aktiviti individu`.

## 5. Interaction Scope

The first implementation will provide UI interaction without inventing new scoring rules:

- Use 12 `TMP_InputField` answer boxes: three for each branch.
- Keep inputs single-line with centered text and automatic font-size reduction.
- Add clear selected/focused colors that follow each branch palette.
- Preserve entered values while the MicroThinker panel remains active.
- Do not add answer validation, scoring, completion popups, or scene transitions unless requested later.
- Preserve the existing Home button callback.
- Keep the center-menu `MICRO THINKER` button callback empty.

## 6. Asset Plan

Create project-bound assets under:

```text
Assets/Images/Akt8MicroThinkerUI/
├── Background/
├── Characters/
├── Icons/
├── Decorations/
└── QA/
```

Planned generated raster assets:

1. text-free lime/cream/peach microbiology background;
2. professor-brain mascot with transparent background;
3. blue bacterium and green microorganism cutouts;
4. microscope, test-tube rack, flask/leaves, and lightbulb cutouts;
5. mind-map icons: microscope, petri dish, shield, and magnifier.

All cutouts must use genuine alpha transparency. Unity import settings will use Sprite (2D and UI), alpha transparency enabled, no mipmaps, and appropriate max texture sizes.

## 7. Technical Implementation

### Repeatable Builder

Create:

`Assets/Editor/Akt8MicroThinkerUIBuilder.cs`

Responsibilities:

- confirm the active scene is `akt8`;
- find `Canvas/MicroThinker` safely;
- create a scene backup before the first rebuild;
- move existing MicroThinker content into `LegacyMicroThinkerContent` and disable it;
- retain `HomeButton` and its persistent callbacks;
- rebuild `MicroThinkerMindMapRoot` deterministically;
- assign sprites, fonts, colors, anchors, shadows, and input-field settings;
- save the scene;
- be safe to run repeatedly without duplicating objects.

### Curved Connectors

Create:

`Assets/Scripts/UI/MindMapConnectorGraphic.cs`

This lightweight uGUI `MaskableGraphic` will draw colored cubic Bézier connectors between the center, branch nodes, and answer fields. It avoids baking connectors into the background and keeps the layout responsive.

### Optional Data Controller

Create:

`Assets/Scripts/Akt8/MicroThinkerMindMapController.cs`

Initial responsibility is limited to exposing and reading the 12 input values. It will not validate answers or trigger completion unless that gameplay is later specified.

## 8. Responsive Layout

- Maintain the existing 1920 × 1080 Canvas reference resolution.
- Use proportional anchors for the instruction card, board, and right mascot panel.
- Use `AspectRatioFitter` only for decorative sprites that require fixed proportions.
- Use nine-sliced sprites or native rounded-panel sprites for scalable cards and nodes.
- Verify at 16:9 and at the current Unity Game-view resolution.
- Prevent input fields and labels from colliding at smaller window sizes.

## 9. Execution Phases and Codex Sol Effort

| Phase | Work | Effort |
|---|---|---|
| 1 | Back up the scene and finalize the existing hierarchy/callback audit | Medium |
| 2 | Generate, inspect, crop, and import the background and transparent cutouts | High |
| 3 | Implement the repeatable builder and responsive uGUI hierarchy | High |
| 4 | Add Bézier connectors and 12 TMP input fields | High |
| 5 | Preserve Home navigation and isolate legacy content | Medium |
| 6 | Compile, rebuild, enter Play Mode, and perform visual/interaction QA | High |
| 7 | Iterate against the supplied reference and save final screenshots | High |

Overall recommended Codex Sol setting: **High effort**. Use **xHigh** only if pixel-level visual iteration across multiple resolutions is required.

## 10. Verification Checklist

- [ ] Scene backup created before hierarchy changes.
- [ ] `MicroThinkerMindMapRoot` fills the Canvas at 16:9.
- [ ] Header, subtitle, instruction card, board, mascot, and footer match the reference composition.
- [ ] All generated cutouts have true transparent backgrounds.
- [ ] Twelve input fields are usable and remain within their rounded boxes.
- [ ] Curved connectors align with all nodes and answer fields.
- [ ] Existing Home navigation still returns to `Canvas/Center`.
- [ ] Center-menu `MICRO THINKER` button still has zero callbacks.
- [ ] Legacy spinner and result content remain recoverable but inactive.
- [ ] No missing scripts, missing sprites, or broken persistent listeners.
- [ ] No new Unity compilation errors.
- [ ] Final Play Mode screenshot reviewed against the supplied reference.

## 11. Out of Scope Until Separately Requested

- connecting the center-menu `MICRO THINKER` button;
- answer correctness rules or a predefined answer key;
- scoring, timers, lives, or completion screens;
- audio narration or sound effects;
- deleting the legacy MicroThinker content permanently.

