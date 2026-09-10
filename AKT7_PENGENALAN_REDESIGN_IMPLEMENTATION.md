# Activity 7 Pengenalan Redesign — Implementation Plan

## Status

Planning document only. Do not modify the Unity scene, scripts, callbacks, images, audio, or Build Settings until the user explicitly approves execution.

## Objective

Redesign `Canvas/Pengenalan` in `Assets/Scenes/Aktivity/akt7/akt7.unity` to closely match the supplied 1920×1080 reference while retaining the existing Activity 7 navigation flow.

The redesigned presentation will contain:

- a warm lime–cream–peach science background;
- an aqua `AKTIVITI 7` badge;
- a large green `Pengenalan` title;
- the subtitle `Mikroorganisma juga memberikan kesan keburukan.` inside a rounded cream panel;
- a cheerful schoolboy illustration on the left with a question symbol;
- a large pink thought cloud containing the editable question;
- microorganism mascots, microscope, petri dish, water-drop, atom, DNA, stars, and bubbles around the edges;
- discreet Home and Next controls using the current callbacks.

All Malay text remains editable TextMeshPro content. Text will not be baked into generated artwork.

## Live Scene Audit

- Active scene: `Assets/Scenes/Aktivity/akt7/akt7.unity`.
- Unity version: `6000.4.5f1`.
- Editor is ready, not compiling, and not in Play Mode.
- Target panel: `Canvas/Pengenalan`.
- Canvas reference resolution remains `1920 × 1080` with Scale With Screen Size.
- The global `Canvas/Image`, lesson panels, video players, `SceneController`, EventSystem, camera, and Activity 7 centre menu are outside this redesign scope.

Current relevant hierarchy:

```text
Canvas
└── Pengenalan
    ├── Top
    │   └── DialogButton
    ├── Title
    │   └── Text                       "PENGENALAN"
    ├── Content
    │   ├── RawImage                   inactive
    │   ├── RawImage (1)               inactive
    │   ├── RawImage (2)               inactive
    │   ├── Center
    │   │   ├── Button (2)             inactive → Bernafas
    │   │   ├── Button (3)             inactive → Bergerak
    │   │   └── Button (4)             inactive → Bertumbuh
    │   └── Image
    │       └── Text (TMP)              current subtitle
    ├── Bottom
    │   ├── NextButton
    │   └── HomeButton
    ├── DialogParentRight Variant
    ├── DialogParentLeft Variant
    ├── DialogParentRight Variant (1)
    ├── DialogParentLeft Variant (1)
    └── DialogParentRight Variant (2)
```

The dialog variants are retained for compatibility but will not cover the new reference presentation on entry. Their serialized objects and internal callbacks will not be deleted.

## Existing Callback Contract

The following persistent listeners must remain unchanged:

| Control | Existing callback | Required result |
|---|---|---|
| `Bottom/NextButton` | `Pengenalan.SetActive(false)` | Hide Pengenalan. |
| `Bottom/NextButton` | `Keburukan.SetActive(true)` | Continue to the existing Keburukan panel. |
| `Bottom/HomeButton` | `Pengenalan.SetActive(false)` | Hide Pengenalan. |
| `Bottom/HomeButton` | `Center.SetActive(true)` | Return to the redesigned Activity 7 centre menu. |
| `Top/DialogButton` | `DialogParentRight Variant.SetActive(true)` | Preserve the legacy dialog entry point, although it will be hidden in the reference layout unless explicitly restored later. |

The inactive Bernafas, Bergerak, and Bertumbuh buttons and their callbacks are preserved under the legacy hierarchy. They will not be promoted into the new reference layout.

## Visual Specification

### Art direction

- Match the current Activity 7 centre menu's rounded, playful science style.
- Use a pale green left edge, warm cream centre, and pale peach right edge.
- Use dark green rounded typography with a white outline and soft olive shadow.
- Keep the main reading path clear: activity badge → title → subtitle → question cloud.
- Avoid decorative artwork behind the question text.
- Keep all decorative images non-interactive with raycasts disabled.

### Text content

```text
AKTIVITI 7
Pengenalan
Mikroorganisma juga memberikan kesan keburukan.

Bagaimanakah
mikroorganisma
memberikan kesan
buruk kepada kita?
```

The question may use four lines at 1920×1080 and dynamically reduce font size at narrower resolutions.

### Approximate 1920×1080 layout

| Element | Placement | Planned size |
|---|---:|---:|
| Activity badge | top centre, y `-35` | `500 × 92` |
| Main title | top centre, y `-125` | `1120 × 150` |
| Subtitle panel | centre, y `-300` | `1120 × 78` |
| Student illustration | lower-left | approximately `570 × 650` |
| Question symbol | above student | approximately `190 × 190` |
| Thought cloud | centre-right | approximately `890 × 500` |
| Purple microbe | upper-right | approximately `240 × 240` |
| Microscope | lower-right | approximately `360 × 440` |
| Petri dish | bottom-right/centre | approximately `310 × 190` |
| Green microbe | lower-left edge | approximately `210 × 210` |
| Water-drop mascot | lower centre-left | approximately `190 × 230` |
| Home control | bottom-left safe corner | `86 × 86` |
| Next control | bottom-right safe corner | `112 × 86` |

Home and Next will be visually quieter than the presentation but remain large enough for touch input.

## Asset Strategy

### Reuse candidates

- `Assets/Fonts/FredokaOne SDF.asset`
- `Assets/Images/UI Flat/btn_white_flat.png`
- `Assets/Images/UI Flat/char_green_microbe.png`
- `Assets/Images/UI Flat/char_purple_microbe.png`
- `Assets/Images/UI Flat/char_water_drop.png`
- `Assets/Images/UI Flat/img_microscope_large.png`
- `Assets/Images/UI Flat/img_petri_bakteria.png`
- existing Home and play/arrow icons;
- `Assets/Images/UI Flat 1/boy3.png`, only if visual inspection confirms that its pose and resolution are suitable.

### Planned Activity 7 assets

Create `Assets/Images/Akt7PengenalanUI/` with:

```text
Background/akt7_pengenalan_background.png
Characters/akt7_pengenalan_student.png
Panels/akt7_pengenalan_thought_cloud.png
Decor/question_mark_bubble.png
QA/
```

Asset rules:

- The background contains only the colour gradient, bubbles, stars, atom, DNA, and other low-contrast ambient decoration.
- The student, thought cloud, and question symbol use transparent backgrounds.
- No generated image contains labels, curriculum codes, letters, or question text.
- Transparent assets use Sprite/Single import mode, alpha transparency, bilinear filtering, clamped wrapping, no mipmaps, and appropriate max texture size.
- Existing project sprites are preferred where they match the reference cleanly; new art is generated only for the student/question pose and cloud shape if required.

## Planned Hierarchy

```text
Canvas
└── Pengenalan
    ├── Akt7PengenalanRedesignRoot       generated; first sibling
    │   ├── Background
    │   ├── AmbientDecor
    │   ├── Header
    │   │   ├── ActivityBadge
    │   │   ├── MainTitle
    │   │   └── SubtitlePanel
    │   ├── StudentGroup
    │   │   ├── Student
    │   │   └── QuestionBubble
    │   ├── QuestionCloud
    │   │   └── QuestionText
    │   ├── EdgeCharacters
    │   │   ├── GreenMicrobe
    │   │   ├── PurpleMicrobe
    │   │   ├── WaterDrop
    │   │   ├── Microscope
    │   │   └── PetriDish
    │   ├── HomeButtonRelay
    │   └── NextButtonRelay
    ├── Title                            retained, hidden
    ├── Content                          retained, hidden
    ├── Top                              retained, hidden
    ├── Bottom                           retained as callback source
    └── LegacyDialogParents              retained, inactive on entry
```

The generated controls will use `CenterMenuButtonRelay` or an equivalent serialized relay to invoke the current Home and Next buttons without copying or replacing their persistent listeners.

## Implementation Strategy

### Phase 0 — Baseline and safety

1. Confirm `akt7` is the active scene and the editor is not in Play Mode.
2. Capture a baseline screenshot of the current Pengenalan panel.
3. Serialize the Pengenalan panel's Home, Next, Dialog, and inactive lesson-button callbacks.
4. Duplicate the scene to:

   `Assets/Scenes/Aktivity/akt7/akt7_PrePengenalanRedesign_20260831.unity`

5. Confirm the existing Activity 7 centre-menu backup remains untouched.

### Phase 1 — Visual asset preparation

1. Inspect `boy3.png` and the existing science sprites at full resolution.
2. Generate only the missing transparent student/question/cloud artwork.
3. Create a clean text-free background matching the reference palette.
4. Import and verify transparency before assigning assets to UI Images.

### Phase 2 — Idempotent Unity builder

Create:

`Assets/Editor/Akt7PengenalanUIBuilder.cs`

with a menu action such as:

`Tools/MicroFun/Rebuild Akt7 Pengenalan UI`

The builder will:

- require the exact `akt7` scene path;
- validate all legacy controls and destinations before mutation;
- delete only a previous `Akt7PengenalanRedesignRoot` when rerun;
- build the new background, title, subtitle, question cloud, student, and edge artwork;
- keep all display images raycast-disabled;
- hide legacy Title, Content, Top, and dialog presentation without deleting them;
- preserve the existing Bottom button objects as inactive callback sources;
- create visible Home and Next relay buttons;
- preserve callback target, method, argument, listener count, and order;
- save only after a callback audit succeeds;
- leave `SceneController`, Build Settings, other panels, and video objects unchanged.

### Phase 3 — Interaction treatment

- Reuse `ButtonInteractionFeedback` for touch/pointer press scaling if already available.
- Disable automatic navigation paths into hidden legacy controls.
- Set explicit keyboard/controller navigation from Home to Next and back.
- Preserve a minimum 80 px touch area at the reference resolution.
- Do not add an audio button because the supplied reference does not show one and no Activity 7 narration clip has been confirmed.

### Phase 4 — Visual refinement

- Compare the Play Mode result against the supplied reference at 1920×1080.
- Adjust title outline, cloud scale, student pose, microscope balance, and subtitle spacing.
- Ensure the question remains readable with no sprite overlap.
- Check 1366×768, 2560×1440, and a tall-tablet aspect ratio.

### Phase 5 — Functional QA

- Home returns to `Canvas/Center` and shows the redesigned Activity 7 menu.
- Next opens `Canvas/Keburukan`.
- Re-entering Pengenalan resets it to the clean reference presentation.
- No dialog overlay appears unexpectedly on entry.
- Existing dialog and inactive lesson objects remain serialized and recoverable.
- No hidden lesson or video object is modified.

## Codex Sol Effort Recommendation

Recommended execution model: **`gpt-5.6-sol` with `high` reasoning effort**.

| Phase | Suggested effort | Reason |
|---|---|---|
| Baseline/callback audit | Medium | Read-only scene and listener inspection. |
| Asset selection/generation | High | Student pose, transparent edges, and reference matching need visual judgment. |
| Builder implementation | High | Must be idempotent and preserve serialized navigation. |
| Visual refinement | High | Requires iterative Game-view comparison. |
| Navigation and regression QA | High | Covers panel transitions, re-entry state, and responsive layouts. |

Use `xhigh` only if several custom character variants must be generated and compared. The Unity hierarchy and callback work itself should remain at `high`.

## Verification Checklist

1. Zero new compilation errors.
2. Scene validation reports no missing scripts or broken references.
3. The builder can run twice while leaving exactly one generated root.
4. Activity badge reads `AKTIVITI 7`.
5. Main title reads `Pengenalan`.
6. Subtitle text exactly matches the reference.
7. Question text remains TMP and is not baked into the cloud image.
8. Home opens Center and Next opens Keburukan.
9. Callback snapshots match before and after implementation.
10. The Pengenalan screen resets correctly each time it opens.
11. Decorative Images have raycasts disabled.
12. No unexpected legacy dialog overlay appears.
13. Existing Activity 7 centre menu remains visually and functionally unchanged.
14. Console is checked after compilation and after each Play Mode transition.

## Acceptance Criteria

- Pengenalan closely matches the supplied Activity 7 reference at 1920×1080.
- The schoolboy, thought cloud, microorganism characters, microscope, petri dish, and ambient science decoration frame the content without obscuring it.
- All displayed text is editable and correctly spelled.
- Home and Next remain functional and touch-friendly.
- Legacy content and dialog objects are preserved rather than deleted.
- No unrelated scene, panel, controller, video, or Build Settings entry changes.
- The result remains usable at common desktop and tablet aspect ratios.

## Expected Files During Execution

- `Assets/Scenes/Aktivity/akt7/akt7.unity`
- `Assets/Scenes/Aktivity/akt7/akt7_PrePengenalanRedesign_20260831.unity`
- `Assets/Editor/Akt7PengenalanUIBuilder.cs`
- `Assets/Images/Akt7PengenalanUI/`
- `Assets/Screenshots/akt7_pengenalan_*.png`

## Explicit Non-Goals

- Do not redesign Keburukan in this pass.
- Do not redesign Micro-Scene or Kuiz 7.
- Do not modify the Activity 7 centre menu.
- Do not change `SceneController.cs` or Build Settings.
- Do not delete the legacy dialog chain or inactive lesson buttons.
- Do not introduce narration without a confirmed Activity 7 audio clip.
- Do not bake Malay text into generated artwork.

