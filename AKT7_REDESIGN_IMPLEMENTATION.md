# Activity 7 Centre Menu Redesign — Implementation Plan

## Status

Planning document only. Do not modify the scene, scripts, assets, callbacks, or Build Settings until the user explicitly approves execution.

## Objective

Redesign the centre menu of `Assets/Scenes/Aktivity/akt7/akt7.unity` to closely match the supplied 1920×1080 reference:

- warm yellow–lime–peach science background;
- `AKTIVITI 7` header badge;
- large `Mikroorganisma & Keburukan` title;
- Malay explanatory subtitle;
- four large colour-coded menu buttons;
- microscope, petri dish, microorganisms, water-drop, magnifier, bubbles, stars, atom, and DNA decorations;
- curriculum card showing `SP: 3.1` and `SK: 3.1.1, 3.1.5`;
- tactile rounded button styling consistent with the redesigned Activity 8 screens.

The menu remains fully interactive Unity UI. Text and callbacks must not be baked into the background image.

## Current Scene Audit

- Active scene: `Assets/Scenes/Aktivity/akt7/akt7.unity`.
- Canvas reference resolution: `1920 × 1080`, Scale With Screen Size.
- The scene contains a large number of hidden lesson panels and 10 video-player objects.
- The existing global `Canvas/Image` uses `Assets/Videos/New Render Texture.renderTexture`.
- `Canvas/Center` is the active centre-menu panel.
- Existing menu hierarchy:

```text
Canvas
├── Image                         (shared video/render-texture background)
├── Center                        (active centre menu)
│   ├── Title
│   └── GameObject                (legacy vertical menu container)
│       ├── Button                (inactive duplicate)
│       ├── Button (1)            PENGENALAN
│       ├── Button (2)            KEBURUKKAN
│       ├── Button (3)            MICRO-SCENE
│       ├── Button (4)            KUIZ
│       └── HomeButton
├── Pengenalan                    (inactive lesson panel)
├── Keburukan                     (inactive lesson panel)
├── ...other inactive lesson panels...
└── Kuiz                          (inactive legacy quiz panel)
```

## Confirmed Navigation Mapping

The current persistent listeners must be preserved:

| New visible button | Existing control | Current callback | Destination/behaviour |
|---|---|---|---|
| `PENGENALAN` | `Button (1)` | `Center.SetActive(false)` and `Pengenalan.SetActive(true)` | Opens the existing Pengenalan panel inside `akt7`. |
| `KEBURUKAN` | `Button (2)` | `Center.SetActive(false)` and `Keburukan.SetActive(true)` | Opens the existing Keburukan panel inside `akt7`. |
| `MICRO-SCENE` | `Button (3)` | `SceneController.ChangeScene("microscene7")` | Loads the existing Micro-Scene scene. |
| `KUIZ` | `Button (4)` | `SceneController.AddScene("Kuiz7")` | Loads the quiz additively using the existing SceneController flow. |
| Home | `HomeButton` | `SceneController.ChangeScene("Activities")` | Returns to the Activities scene. |

Build Settings contain:

- `Assets/Scenes/Aktivity/akt7/akt7.unity`;
- `Assets/Scenes/Aktivity/akt7/microscene7.unity`;
- `Assets/Scenes/Aktivity/akt7/kuiz7.unity`.

During QA, verify whether the existing `Kuiz7` callback casing resolves correctly to `kuiz7`. Normalize the serialized argument only if the current callback fails in Play Mode.

`SceneController.oldCanvas`, `oldCamera`, and `oldEventSystem` are already correctly assigned and must remain unchanged.

## Scope Boundary

Only the centre menu is redesigned.

- Build the new visual background as a child of `Canvas/Center`, not as a replacement for the global `Canvas/Image`.
- When `Center` is hidden, the generated centre-menu background disappears with it and the existing lesson/video presentation remains intact.
- Do not modify the inactive Pengenalan, Keburukan, food-production, organic-fertilizer, wastewater, respiration, movement, growth, fungi, MicroLab, protozoa, virus, water, medicine, or legacy quiz panels.
- Do not disable or remove any video-player objects.
- Do not change `SceneController.cs` unless an actual navigation defect is demonstrated during Play Mode testing.

## Visual Specification

### Art direction

- Use a pale lime lower-left region, warm yellow/cream centre, and soft peach right edge.
- Keep background contrast low behind the title and menu controls.
- Use deep green heading text with a cream outline and soft shadow.
- Use Fredoka/rounded TMP typography already present in the project.
- Give menu buttons a glossy layered appearance using existing nine-sliced button sprites, outline, lower-edge shadow, and subtle highlight strips.
- Decorative art must use transparent backgrounds, preserve aspect ratio, disable raycasts, and remain outside the main reading column.

### Reference layout at 1920×1080

| Element | Approximate placement | Planned size |
|---|---:|---:|
| Activity badge | top centre, y `+475` | `430 × 78` |
| Main title | centre, y `+355` | `1500 × 135` |
| Subtitle | centre, y `+265` | `1250 × 55` |
| Pengenalan button | centre-right, y `+105` | `830 × 122` |
| Keburukan button | centre-right, y `-35` | `830 × 122` |
| Micro-Scene button | centre-right, y `-175` | `830 × 122` |
| Kuiz button | centre-right, y `-315` | `830 × 122` |
| Curriculum card | bottom centre, y `-465` | `390 × 112` |
| Home button | lower-left safe corner | compact `72 × 72` round control |

The menu column may shift slightly right to leave room for the microscope and green microorganism artwork while maintaining symmetrical title alignment.

## Button Design

Each button will retain its existing `Button` component and persistent listeners.

### Pengenalan

- Green layered base.
- Open-book icon in a circular icon pod.
- White `PENGENALAN` label.
- Gold circular play-arrow pod on the right.

### Keburukan

- Orange layered base.
- Purple microorganism icon.
- Correct the visible spelling from `KEBURUKKAN` to `KEBURUKAN` without changing the destination.
- Gold play-arrow pod.

### Micro-Scene

- Blue layered base.
- Video/clapper icon using `ic_video_flat.png` or a newly generated transparent clapperboard icon if the existing asset is visually inadequate.
- White `MICRO-SCENE` label.
- Gold play-arrow pod.

### Kuiz

- Pink/coral layered base.
- Quiz-checklist icon.
- White `KUIZ` label.
- Gold play-arrow pod.

### Interaction feedback

- Reuse `ButtonInteractionFeedback` for pointer/touch press scaling.
- Configure colour-tint hover/pressed states without washing out the button artwork.
- Maintain minimum 110 px touch height at the reference resolution.
- Preserve keyboard/controller navigation order: Pengenalan → Keburukan → Micro-Scene → Kuiz → Home.

## Planned Hierarchy

```text
Canvas
├── Image                                  (unchanged shared background)
├── Center
│   ├── Akt7CenterBackdrop                 (generated; first sibling)
│   │   ├── WarmBackground
│   │   ├── LeftDecor
│   │   │   ├── Microscope
│   │   │   ├── PetriDish
│   │   │   └── GreenMicrobe
│   │   ├── RightDecor
│   │   │   ├── PurpleMicrobe
│   │   │   ├── WaterDrop
│   │   │   └── Magnifier
│   │   └── AmbientDecor
│   ├── Akt7Header
│   │   ├── ActivityBadge
│   │   ├── MainTitle
│   │   ├── Subtitle
│   │   └── HeaderStars
│   ├── Akt7Menu
│   │   ├── PengenalanButton               (reuses Button (1))
│   │   ├── KeburukanButton                (reuses Button (2))
│   │   ├── MicroSceneButton               (reuses Button (3))
│   │   └── KuizButton                     (reuses Button (4))
│   ├── CurriculumCard
│   ├── HomeButton                         (reuses existing button/callback)
│   └── LegacyCenterControls
│       └── InactiveDuplicateButton         (retained inactive)
└── Existing lesson panels and systems      (unchanged)
```

## Assets

### Reuse

- `Assets/Images/UI Flat/icon_book_open.png`
- `Assets/Images/UI Flat/char_purple_microbe.png`
- `Assets/Images/UI Flat/ic_video_flat.png`
- `Assets/Images/UI Flat/icon_quiz_checklist.png`
- `Assets/Images/UI Flat/char_green_microbe.png`
- `Assets/Images/UI Flat/char_water_drop.png`
- `Assets/Images/UI Flat/img_microscope_large.png`
- `Assets/Images/UI Flat/img_petri_bakteria.png`
- `Assets/Images/UI Flat/icon_magnify_glass.png`
- existing white, green, blue, orange, pink, and gold sliced UI sprites;
- `Assets/Fonts/FredokaOne SDF.asset`.

### Planned new assets

Create `Assets/Images/Akt7CenterUI/` containing:

- `Background/akt7_center_background.png` — a clean 1920×1080 science-themed background with no embedded labels or buttons;
- optional `Icons/icon_micro_scene_clapper.png` if the existing video icon does not match the reference closely enough;
- any additional transparent bubbles, DNA, atom, or star decorations that cannot be reproduced cleanly with existing assets;
- `QA/` screenshots for comparison and final verification.

Generated art must not contain text. All Malay labels, curriculum codes, and button titles remain editable TMP objects.

If a new background is not generated, use the existing Activity 8 centre background only as a temporary layout reference, not as a permanent Activity 7-labelled asset.

## Implementation Strategy

### 1. Safety backup

Before mutation, duplicate:

`Assets/Scenes/Aktivity/akt7/akt7.unity`

to a timestamped backup such as:

`Assets/Scenes/Aktivity/akt7/akt7_PreCenterRedesign_YYYYMMDD.unity`

### 2. Idempotent editor builder

Create `Assets/Editor/Akt7CenterMenuUIBuilder.cs` with a menu action such as:

`MicroFun/Activity 7/Build Reference Centre Menu`

The builder will:

- verify that `akt7` is the active scene;
- validate all required legacy objects and callbacks before changing anything;
- preserve the four visible buttons and Home button;
- preserve their persistent listener lists exactly;
- move the controls into a generated hierarchy and restyle them;
- remove only earlier generated Activity 7 centre roots when rerun;
- retain the inactive duplicate button under `LegacyCenterControls`;
- add background and decorative elements below the controls;
- set all decoration raycasts off;
- normalize `CanvasScaler.matchWidthOrHeight` to `0.5` only if responsive tests show a benefit;
- save the scene only after a post-build callback audit succeeds;
- be safe to rerun without duplicate objects, listeners, outlines, shadows, or feedback components.

### 3. Callback preservation audit

Before and after the build, serialize the `Button.onClick` persistent calls and compare:

- target instance;
- method name;
- argument;
- call order;
- listener count.

The build must abort and avoid saving if an expected listener is missing.

### 4. Responsive layout

- Use centre-anchored reference positions at 1920×1080.
- Keep the main title and menu within safe bounds at 1366×768 and 2560×1440.
- Permit decorative artwork to crop at non-16:9 ratios.
- Keep button text auto-sizing within a controlled range.
- Do not allow decorations to overlap button hit areas.

## Implementation Phases and Codex Sol Effort

Recommended default: **`gpt-5.6-sol` with `high` reasoning effort**.

| Phase | Work | Suggested effort |
|---|---|---|
| 0. Baseline and safety | Backup scene, capture current hierarchy/callback audit, check console | Medium |
| 1. Asset preparation | Reuse/import or generate Activity 7 background and transparent decorations | High |
| 2. Builder implementation | Create the idempotent centre-menu builder and reconstruct the visual hierarchy | High |
| 3. Callback verification | Compare listener targets, methods, arguments, and additive-load references | High |
| 4. Visual refinement | Play Mode comparison, spacing, typography, cropping, and resolution passes | High |
| 5. Navigation QA | Exercise Pengenalan, Keburukan, microscene7, kuiz7, and Activities routes | High |

Use **`xhigh`** only if a fully bespoke background and several new transparent illustrations must be generated and iterated in the same execution. The Unity UI reconstruction itself should use `high`.

## Verification Plan

1. Confirm zero new C# compile errors.
2. Validate `akt7` for missing scripts and broken prefab references.
3. Confirm the centre-menu builder can run twice without duplicates.
4. Capture a 1920×1080 Game-view comparison screenshot.
5. Test the Pengenalan button:
   - `Center` becomes inactive;
   - `Pengenalan` becomes active;
   - its return control restores `Center`.
6. Test the Keburukan button:
   - `Center` becomes inactive;
   - `Keburukan` becomes active;
   - its return control restores `Center`.
7. Test Micro-Scene loads `microscene7` successfully.
8. Test Kuiz loads the existing Activity 7 quiz successfully and verify the `Kuiz7`/`kuiz7` casing.
9. Test Home loads `Activities`.
10. Verify all inactive lesson panels and all video-player references remain unchanged.
11. Verify generated decorations do not receive raycasts.
12. Test 1920×1080, 2560×1440, 1366×768, and a tall tablet ratio.
13. Check console for runtime errors after each destination test.

## Acceptance Criteria

- The centre menu closely matches the supplied Activity 7 reference.
- The title reads `Mikroorganisma & Keburukan`.
- The subtitle reads `Kenali mikroorganisma dan kesan keburukannya dalam kehidupan seharian!`.
- The four visible buttons read `PENGENALAN`, `KEBURUKAN`, `MICRO-SCENE`, and `KUIZ`.
- The curriculum card reads `SP: 3.1` and `SK: 3.1.1, 3.1.5`.
- Every pre-existing destination still opens correctly.
- Hidden lesson panels and video playback remain unaffected.
- The scene contains no missing scripts, null required references, or duplicated generated roots.
- No unrelated scenes or shared controllers are changed.

## Files Expected to Change During Execution

- `Assets/Scenes/Aktivity/akt7/akt7.unity`
- `Assets/Editor/Akt7CenterMenuUIBuilder.cs` (new)
- `Assets/Images/Akt7CenterUI/` (new/reused assets and QA captures)
- timestamped `akt7` scene backup

`SceneController.cs` is not expected to change.

## Explicit Non-Goals

- Do not redesign the internal Pengenalan or Keburukan lesson panels in this pass.
- Do not redesign `microscene7` or `kuiz7`.
- Do not alter Activity 7 curriculum content.
- Do not change Build Settings.
- Do not delete or consolidate video-player objects.
- Do not replace persistent listeners with runtime `GameObject.Find` wiring.
- Do not bake text into generated artwork.

