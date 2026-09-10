# Aktiviti 8 Center Menu — Implementation Plan

## Goal

Redesign the current Aktiviti 8 landing menu so it closely matches the supplied **“Micro Squad & Micro Thinker”** reference while preserving the existing scene flow and previously implemented Pengenalan and Micro Squad sections.

This document is planning only. No scene, script, prefab, navigation, or visual asset changes are authorized until execution is explicitly approved.

## Target

- Project: `MicroFun`
- Unity: `6000.4.5f1`
- Scene: `Assets/Scenes/Aktivity/akt8/akt8.unity`
- Target hierarchy: `Canvas/Center`
- Reference resolution: `1920 × 1080`, landscape 16:9
- UI system: uGUI Canvas, CanvasScaler, TextMeshPro, Input System UI module

## Read-only scene audit

The current scene is idle and ready for tools. `Canvas/Center` is the active landing section and contains:

- `Title`
- `GameObject`, containing six menu buttons and `HomeButton`

Current menu inventory:

| Existing object | Current label | Existing navigation |
|---|---|---|
| `Button` | PENGENALAN | Contains a broken/null second target; should not be reused as the primary route |
| `Button (1)` | PENGENALAN | Working: hides `Center`, shows `Pengenalan` |
| `Button (2)` | MICRO SQUAD | Working: hides `Center`, shows `Microsquad` |
| `Button (3)` | MICRO THINKER | No persistent callback |
| `Button (4)` | KUIZ | Working: hides `Center`, calls `SceneController.AddScene` |
| `Button (5)` | TAMAT | Working: hides `Center`, calls `SceneController.AddScene` |
| `HomeButton` | Home icon | Working: calls `SceneController.ChangeScene` |

Additional findings:

- Existing `Canvas/Pengenalan` and `Canvas/Microsquad` sections must remain untouched.
- There is no scene or asset currently named `Pembentangan`.
- There is no scene or asset currently named `Micro Thinker`.
- The existing Micro Thinker button has no navigation event.
- The build currently contains `akt8.unity`, `kuiz8.unity`, and `Ending.unity`, but no separate Pembentangan or Micro Thinker scene.

## Reference-led visual specification

### Background

Create a text-free 16:9 educational science illustration with:

- warm cream/yellow center;
- lime-green lower-left;
- peach/orange right side;
- soft waves, bubbles, stars, atom and DNA line art;
- green microbe mascot on the left;
- purple microbe mascot on the right;
- microscope and petri dish at lower-left;
- blue water-drop mascot at lower-right;
- magnifying glass with microorganisms at lower-right.

All decoration must stay near the outer edges, leaving the header and center menu area clean.

### Header

- Top badge: `AKTIVITI 8`
- Main title: `Micro Squad & Micro Thinker`
- Subtitle: `Mari bekerjasama, berbentang dan berfikir secara kreatif!`
- Green rounded Fredoka-style lettering with a white border and soft shadow.

### Main menu

Four centered horizontal pill buttons, approximately 820–880 px wide:

1. Green: `PENGENALAN`, book icon
2. Orange: `MICRO SQUAD`, group icon
3. Blue: `PEMBENTANGAN`, presentation-board icon
4. Pink: `MICRO THINKER`, brain/lightbulb icon

Each button contains:

- a circular icon well on the left;
- bold white TextMeshPro label with dark soft shadow;
- a circular yellow/orange play button on the right;
- layered highlight, outline, and bottom shadow;
- pointer hover/pressed feedback without changing navigation semantics.

### Curriculum badge

Bottom-center cream card:

- `SP: 3.1`
- `SK: 3.1.1, 3.1.5`

### Existing Home control

Keep the working `HomeButton`, but restyle and reposition it unobtrusively within the safe area. Do not remove or recreate its existing persistent callback.

## Asset strategy

### Generated assets

Use built-in ImageGen with the supplied reference as a visual guide.

Proposed paths:

- `Assets/Images/Akt8CenterMenuUI/Background/akt8_center_background.png`
- `Assets/Images/Akt8CenterMenuUI/Icons/icon_pengenalan.png`
- `Assets/Images/Akt8CenterMenuUI/Icons/icon_micro_squad.png`
- `Assets/Images/Akt8CenterMenuUI/Icons/icon_pembentangan.png`
- `Assets/Images/Akt8CenterMenuUI/Icons/icon_micro_thinker.png`
- `Assets/Images/Akt8CenterMenuUI/QA/`

Generation requirements:

- background contains no text, buttons, labels, logos, or watermark;
- icons are isolated, transparent-background, square cutouts;
- icon art uses the same glossy, child-friendly 3D style;
- important visual elements stay inside a 10% safe margin;
- all Malay text remains native TMP text in Unity for clarity and editability.

### Native Unity UI

Build button shells, outlines, shadows, play discs, header badge, and curriculum badge from uGUI Images and the existing sliced white-panel sprite. Do not bake text or button chrome into the generated background.

## Hierarchy design

Create the following under `Canvas/Center`:

```text
Center
├── CenterMenuRedesignRoot
│   ├── Background
│   ├── Header
│   │   ├── ActivityBadge
│   │   ├── Title
│   │   └── Subtitle
│   ├── MenuStack
│   │   ├── PengenalanButton
│   │   ├── MicroSquadButton
│   │   ├── PembentanganButton
│   │   └── MicroThinkerButton
│   ├── CurriculumBadge
│   └── DecorativeOverlay
├── Title                         (legacy, disabled)
└── GameObject                    (legacy container retained)
```

The legacy container remains in the scene so persistent events and prior layout can be recovered. Working buttons may be reparented into `MenuStack` rather than cloned where doing so best preserves serialized callbacks.

## Navigation preservation

### Known routes

- Pengenalan: reuse `Button (1)` and its current `Center.SetActive(false)` plus `Pengenalan.SetActive(true)` listeners.
- Micro Squad: reuse `Button (2)` and its current `Center.SetActive(false)` plus `Microsquad.SetActive(true)` listeners.
- Home: reuse `HomeButton` and its existing `SceneController.ChangeScene` listener.

### Unresolved routes

- Pembentangan: no destination currently exists.
- Micro Thinker: the current button has zero persistent listeners and no matching scene/section was found.

Do not invent either route. Before functional completion, one of the following must be supplied or discovered for each:

- an existing Canvas section to activate;
- a scene name/path for `SceneController.AddScene` or `ChangeScene`;
- a new section/scene explicitly authorized as part of the task.

Until routes are resolved, the implementation may render these two entries as reference-matching buttons but must either disable interaction or show a clear `Akan datang` response. They must not silently do nothing.

### Legacy Kuiz and Tamat controls

The reference omits Kuiz and Tamat. To match it closely:

- hide their existing visual objects from `Center`;
- retain both objects and all persistent callbacks in the hierarchy;
- do not delete or repurpose their navigation events;
- expose them again only if the user requests a secondary footer or overflow menu.

## Implementation phases

### Phase 1 — Safety and final audit

Codex Sol effort: **Medium**

1. Confirm the active scene and idle editor state.
2. Save a timestamped pre-redesign copy of `akt8.unity`.
3. Capture a Play Mode baseline screenshot.
4. Record the persistent listeners for all `Center` buttons.
5. Resolve or explicitly mark the Pembentangan and Micro Thinker destinations.

### Phase 2 — Visual asset generation

Codex Sol effort: **High**

1. Generate one text-free 16:9 background from the supplied reference.
2. Generate four transparent icon cutouts.
3. Import with correct Sprite settings, alpha handling, filtering, and max texture size.
4. Verify clean edges and consistent character scale.

### Phase 3 — Native UI construction

Codex Sol effort: **High**

1. Add `Assets/Editor/Akt8CenterMenuUIBuilder.cs`.
2. Build `CenterMenuRedesignRoot` at 1920×1080 reference resolution.
3. Create the header, four menu bars, curriculum badge, and decorative hierarchy.
4. Use `FredokaOne SDF.asset` for editable text.
5. Make only actual button target graphics raycastable.
6. Preserve the legacy hierarchy in disabled form.
7. Save through a repeatable menu command such as `Tools > MicroFun > Rebuild Akt8 Center Menu`.

### Phase 4 — Navigation wiring

Codex Sol effort: **High**

1. Reuse the known Pengenalan, Micro Squad, and Home callbacks.
2. Wire Pembentangan only after its destination is resolved.
3. Wire Micro Thinker only after its destination is resolved.
4. Prevent duplicate listeners if the builder is run repeatedly.
5. Ensure each section can return to `Center` without losing the redesigned state.

### Phase 5 — Interaction polish

Codex Sol effort: **Medium**

1. Add subtle scale/brightness feedback on hover and press.
2. Keep animation unscaled-time safe.
3. Ensure pressed states do not move labels or icons out of alignment.
4. Verify all play icons and left icon wells remain centered at different Game View sizes.

### Phase 6 — Verification

Codex Sol effort: **High**

1. Validate scripts and wait for compilation/domain reload.
2. Confirm zero new compilation errors.
3. Enter Play Mode and capture a full 16:9 screenshot.
4. Inspect the header, every icon, button padding, shadows, and curriculum badge.
5. Click-test all four entries plus Home.
6. Confirm correct panel or scene transitions.
7. Test return navigation to `Center`.
8. Test at 1920×1080 and one smaller 16:9 Game View size.
9. Save final QA captures under `Assets/Images/Akt8CenterMenuUI/QA/`.

## Acceptance criteria

- The active `Center` screen closely matches the supplied reference at 16:9.
- The title, subtitle, four colored menu bars, icons, play discs, and SP/SK badge are visually clear and correctly aligned.
- No generated text is used; all visible copy is TMP text.
- Pengenalan, Micro Squad, and Home retain their current working behavior.
- Pembentangan and Micro Thinker never behave as silent dead buttons.
- Kuiz and Tamat callbacks remain recoverable even if their visuals are hidden.
- Existing Pengenalan and Microsquad section content is unchanged.
- No decorative Image intercepts button raycasts.
- The scene compiles and enters Play Mode without new code errors.
- A timestamped scene backup and final Play Mode screenshot exist.

## Scope boundaries

This redesign does not include:

- changing the Pengenalan puzzle screen;
- changing the Micro Squad wheel screen;
- redesigning Kuiz or Ending;
- modifying video-player content;
- deleting legacy navigation objects;
- creating a new Pembentangan or Micro Thinker learning activity without separate authorization.

## Recommended effort

- Recommended Codex Sol reasoning effort: **High**
- Use **High** for asset generation, layout construction, callback preservation, and final interaction QA.
- Use **Medium** for scene inventory, import configuration, and cosmetic animation tuning.
- Estimated implementation time after route decisions: **90–150 minutes**.

## Decision required before execution

Provide or confirm the destinations for:

1. `PEMBENTANGAN`
2. `MICRO THINKER`

All visual work can be prepared without those answers, but functional navigation should not be guessed.

