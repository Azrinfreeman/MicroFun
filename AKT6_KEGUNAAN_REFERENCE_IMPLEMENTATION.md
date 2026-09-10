# Activity 6 Kegunaan Reference Redesign — Implementation Plan

## Status

Planning only. This document does not perform any Unity scene, script, prefab, callback, image-generation, or Build Settings changes.

The Unity editor connection was unavailable during this planning pass, so the exact active-scene path and serialized callback targets must be confirmed at the beginning of execution. The visual and technical plan is based on the supplied reference and the project's established Activity UI-builder pattern.

## Goal

Redesign the current Activity 6 `Kegunaan` section to closely match the supplied reference while preserving all existing educational content, tab callbacks, detail panels, navigation, and completion behavior.

The screen will communicate the broader theme `Kebaikan Mikroorganisma`, with `Kegunaan Mikroorganisma` as the central hub and four interactive use categories:

1. `Pembuatan makanan`
2. `Penghasilan ubat-ubatan`
3. `Penghasilan baja organik`
4. `Perawatan sisa kumbahan`

## Reference Interpretation

The attached image is a visual reference only. Its artwork and labels do not contain operational instructions.

The target composition contains:

- a warm green-to-cream-to-peach science background;
- a green `AKTIVITI 6` badge;
- a large glossy title, `Kebaikan Mikroorganisma`;
- four color-coded rounded tab cards arranged around a central circular hub;
- category-specific illustrations;
- arrow affordances on all four tabs;
- science and nature decorations around the safe edges; and
- a bottom instruction pill: `Klik setiap tab untuk melihat penerangan.`

The finished Unity UI will reproduce this hierarchy and color language without baking functional text into background images.

## First Execution Step: Live Scene Audit

Before changing anything, execution must:

1. Identify the active scene containing the `Kegunaan` section and record its exact asset path.
2. Inspect the Canvas, `Kegunaan` root, legacy tab buttons, detail panels, Back/Home control, SceneController, and EventSystem.
3. Record every persistent button callback, serialized target, tab-to-panel mapping, animation, audio source, and active state.
4. Confirm whether Kegunaan is a panel inside a larger Activity 6 scene or a standalone scene.
5. Confirm additive and standalone navigation behavior.
6. Create a dated scene backup beside the source scene, using a name such as:
   `*_PreKegunaanReferenceRedesign_20260901.unity`.

No callback will be inferred from object names when the serialized source can be inspected directly.

## Visual Design Specification

### Canvas and scaling

- Reference resolution: `1920 × 1080`.
- Canvas Scaler: Scale With Screen Size.
- Match width/height: approximately `0.5`.
- Safe outer margin: 32–48 reference pixels.
- Main controls remain usable at 16:9, 16:10, and 4:3.

### Background

- Pale green on the left, warm cream in the center, and peach/orange on the right.
- Subtle leaves and plant elements to reinforce beneficial uses.
- Bubbles, sparkles, microscope, petri dish, science books, and laboratory glassware around the safe edges.
- A restrained wooden tabletop along the lower edge, matching the reference without covering the instruction pill.
- Decorations are non-interactive and have `Raycast Target` disabled.

### Header

- Centered glossy green badge: `AKTIVITI 6`.
- Large title: `Kebaikan Mikroorganisma`.
- Rounded educational typography using the project's Fredoka font.
- Dark-to-light green title treatment with cream outline and soft shadow.
- Decorative leaves positioned around the badge/title, never inside text bounds.

### Central hub

- Large circular center panel displaying:
  `Kegunaan\nMikroorganisma`.
- White/cream inner disc with leaf accents.
- Four-color outer ring matching the connected tabs:
  - blue;
  - orange;
  - pink;
  - teal.
- Decorative connector bridges create the appearance that the four tabs join the hub.
- The hub is informational and does not intercept button input.

### Four category tabs

Each category becomes a large rounded horizontal button with:

- strong category color;
- glossy highlight and dark lower shadow;
- white label with a subtle dark outline;
- transparent-background category illustration;
- circular white arrow affordance;
- full-card hit target; and
- clear hover, pressed, selected, and disabled states.

#### Tab mapping

| Position | Color | Label | Planned illustration |
|---|---|---|---|
| Top-left | Blue | Pembuatan makanan | Bread, cheese, yoghurt/fermented food |
| Top-right | Orange | Penghasilan ubat-ubatan | Medicine bottle, tablets, capsules |
| Bottom-left | Pink | Penghasilan baja organik | Organic-fertilizer bag, compost/soil |
| Bottom-right | Teal | Perawatan sisa kumbahan | Treatment tank/facility and water drop |

The illustrations may be reused from existing project assets or generated as clean text-free transparent PNGs during execution.

### Instruction pill

- Bottom-center cream rounded panel.
- Circular green touch/click icon at the left.
- Text: `Klik setiap tab untuk melihat penerangan.`
- Green dashed or outlined border and soft shadow.
- Non-interactive unless the current scene already assigns behavior to this element.

## Interaction Design

### Tab behavior

The four original tab callbacks remain authoritative.

Preferred approach:

- restyle and reposition the existing functional `Button` objects.

Fallback approach:

- keep the source buttons active but visually hidden;
- create visible reference-style buttons;
- forward each click through `CenterMenuButtonRelay` or a Kegunaan-specific relay;
- preserve interactability and callback order.

Each tab must open the same explanation panel or state it opens today.

### Visual states

| State | Treatment |
|---|---|
| Normal | Saturated category color and white highlight |
| Hover/focus | Slight scale lift and brighter rim |
| Pressed | Small compression with stronger lower shadow |
| Selected/open | Glowing ring or check accent |
| Disabled | Lower saturation and reduced contrast |

Keyboard/gamepad navigation will follow a predictable clockwise or row-based sequence.

### Detail panels

If each tab currently opens a detail panel, those panels will receive the same Activity 6 theme:

- dimmed background overlay;
- large cream rounded information card;
- category-colored title band;
- original Malay explanation text;
- original illustrations or video content;
- clear circular close/back button; and
- input blocked behind the open panel.

Existing educational wording will not be rewritten without a separate content request.

## Proposed Hierarchy

```text
Canvas
└── Kegunaan
    ├── Akt6KegunaanReferenceRoot
    │   ├── Background
    │   │   ├── GradientPlate
    │   │   ├── Tabletop
    │   │   └── EdgeDecorations
    │   ├── Header
    │   │   ├── ActivityBadge
    │   │   └── MainTitle
    │   ├── HubAndTabs
    │   │   ├── ConnectorLayer
    │   │   ├── FoodTab
    │   │   ├── MedicineTab
    │   │   ├── FertilizerTab
    │   │   ├── WastewaterTab
    │   │   └── CentralHub
    │   ├── InstructionPill
    │   └── BackOrHomePresentation
    ├── LegacyFunctionalControls       (preserved or restyled)
    └── ExistingDetailPanels           (preserved and restyled)
```

The exact parenting will be chosen after the live audit. If animations or scripts depend on fixed Transform paths, the redesign will frame controls in place rather than reparent them.

## Technical Implementation

### Idempotent editor builder

Add:

- `Assets/Editor/Akt6KegunaanUIBuilder.cs`

Menu item:

- `Tools/MicroFun/Activity 6/Build Kegunaan Reference UI`

The builder will:

1. Validate the active scene and locate the real Kegunaan root.
2. Verify all four source buttons and their target panels.
3. Refuse to continue when a required callback or target is missing.
4. Remove only an earlier `Akt6KegunaanReferenceRoot` before rebuilding.
5. Create the background, header, hub, connectors, visible tabs, and instruction pill.
6. Restyle or relay the original functional controls.
7. Preserve Home/Back behavior.
8. Disable raycasts on every decorative image.
9. Mark and save only the intended scene.
10. Remain safe to run repeatedly without duplicate roots, listeners, EventSystems, or AudioListeners.

### Optional runtime presenter

Add only if the existing controller cannot expose selection state cleanly:

- `Assets/Scripts/UI/Akt6KegunaanPresenter.cs`

Possible responsibilities:

- synchronize selected-tab visuals with the active detail panel;
- disable background tab input while a detail panel is open;
- restore selection state when a panel closes; and
- support keyboard/gamepad navigation.

It will not replace the current educational controller or scene-routing logic.

## Asset Strategy

Target folder for new visual assets:

- `Assets/Images/Akt6KegunaanUI/`

Priority order:

1. Reuse Activity 6 artwork already assigned to the four content categories.
2. Reuse compatible science decorations and rounded panels from `Assets/Images/UI Flat/` and the completed Activity 7/8 redesigns.
3. Generate only missing category illustrations and a text-free background plate.
4. Keep all functional labels in TextMeshPro.

Any generated PNG will be:

- text-free;
- transparent where used as an icon;
- imported as a UI Sprite;
- configured without mipmaps;
- checked for alpha fringes; and
- compressed conservatively for clean classroom projection.

## Implementation Phases and Codex Sol Effort

| Phase | Work | Recommended effort |
|---|---|---|
| 1. Live audit and backup | Resolve scene path, hierarchy, callbacks, panels, navigation, and create backup | `gpt-5.6-sol` — High |
| 2. Asset preparation | Reuse or generate background and four transparent category illustrations | `gpt-5.6-sol` — High |
| 3. UI foundation | Builder, background, header, hub, connectors, scaler, and instruction pill | `gpt-5.6-sol` — High |
| 4. Functional tabs | Reposition/restyle or relay four source buttons and selection states | `gpt-5.6-sol` — High |
| 5. Detail panels | Apply matching modal/detail-panel styling without changing content | `gpt-5.6-sol` — High |
| 6. QA and polish | All callbacks, navigation contexts, resolutions, screenshot comparison | `gpt-5.6-sol` — High |

Recommended default: **gpt-5.6-sol with High effort**. Use `xhigh` only if all four category illustrations require custom generation or the existing hierarchy is tightly coupled to animations.

## Verification Checklist

### Scene integrity

- No compilation errors or missing serialized references.
- One intended reference root.
- No duplicate EventSystem, Canvas, AudioListener, or click listener.
- Builder is safe to run twice.
- Original scene remains recoverable from the dated backup.

### Functional testing

- Food tab opens the original food explanation.
- Medicine tab opens the original medicine explanation.
- Organic-fertilizer tab opens the original fertilizer explanation.
- Wastewater tab opens the original wastewater explanation.
- Each detail panel closes correctly.
- Background tabs cannot be clicked through an open panel.
- Back/Home works in standalone and additive contexts.
- Existing audio, animation, completion, or PlayerPrefs behavior remains intact.

### Visual QA

- Capture at `1920 × 1080` and compare with the reference.
- Check 1600×900, 1366×768, 1280×720, 1920×1200, and 1024×768.
- No title, tab label, central-hub text, or illustration clipping.
- Tabs never overlap the central hub or instruction pill.
- Decorative images never block button input.
- Text remains readable at classroom viewing distance.

## Acceptance Criteria

The redesign is complete when:

1. The Kegunaan section closely matches the supplied reference composition.
2. All four category tabs retain their original destinations and content.
3. The center hub and colored connector design clearly communicates the four-part relationship.
4. Existing detail panels and navigation remain functional.
5. The layout is responsive and accessible.
6. The scene produces no new console errors or validation issues.
7. The builder is reproducible and idempotent.
8. A dated backup and final Game View screenshot are delivered.

## Out of Scope

- Rewriting the Activity 6 educational content.
- Changing tab-to-topic mappings.
- Adding new uses of microorganisms.
- Changing Build Settings or unrelated Activity scenes.
- Replacing functional animations, audio, or videos without evidence they are broken.

## Expected Deliverables After Approval

- Updated source scene containing the current `Kegunaan` section.
- Dated pre-redesign scene backup.
- `Assets/Editor/Akt6KegunaanUIBuilder.cs`.
- Optional `Assets/Scripts/UI/Akt6KegunaanPresenter.cs`.
- Any required assets under `Assets/Images/Akt6KegunaanUI/`.
- Final 1920×1080 Game View screenshot.
- Short validation report covering the four tabs, detail panels, and navigation.
