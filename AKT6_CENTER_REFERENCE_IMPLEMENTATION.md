# Activity 6 Center Page Reference Redesign — Implementation Plan

Status: **Planning only. Do not execute scene, script, callback, or image changes yet.**

## Requested Outcome

Redesign `Canvas/Center` in Activity 6 to follow the supplied reference while applying the requested content changes:

- keep the large `AKTIVITI 6` and `Kebaikan Mikroorganisma` presentation;
- keep the microscope, laboratory glassware, books, water-drop character, purple microorganism, leaves, bubbles, and warm science-classroom theme;
- remove the `Peranan` button completely;
- rename the reference's `Kebaikan` button to `Kegunaan`;
- show only three primary buttons:
  1. `Pengenalan`
  2. `Kegunaan`
  3. `Kuiz`

The attached image is an approved visual reference, not an instruction document.

## Live Scene Audit

Unity MCP confirmed:

- Active scene: `Assets/Scenes/Aktivity/akt6/akt6.unity`
- Target: `Canvas/Center`
- Existing menu container: `Canvas/Center/GameObject`
- Current active navigation already matches the required functional structure:
  - `Button (1)` — `PENGENALAN` → opens `Canvas/Pengenalan`
  - `Button (2)` — `KEGUNAAN` → opens `Canvas/Kegunaan`
  - `Button (4)` — `KUIZ` → invokes the existing quiz scene callback
  - `HomeButton` → invokes the existing scene-change callback
- No active Peranan button currently exists. Therefore, the redesigned UI must simply omit it rather than delete or repurpose another control.
- Two inactive legacy buttons also exist. They remain serialized and hidden.

## Safety and Preservation

Before implementation:

1. Ensure `akt6.unity` is active and Unity is not in Play Mode.
2. Duplicate the scene to:
   `Assets/Scenes/Aktivity/akt6/akt6_PreCenterReferenceRedesign_20260901.unity`
3. Record the persistent listeners on `Button (1)`, `Button (2)`, `Button (4)`, and `HomeButton`.
4. Preserve the original `Canvas/Center/Title` and `Canvas/Center/GameObject` hierarchy.
5. Disable the legacy presentation only after all new relay controls are built.
6. Never bind any new button to an inactive or semantically unrelated legacy control.

## Artwork Strategy

The supplied reference cannot be used unchanged because it visibly contains four buttons, including the unwanted `Peranan` button and the old `Kebaikan` label.

### Clean background generation

Create a reference-derived background edit that:

- preserves the title, subtitle, characters, microscope, books, glassware, plants, bubbles, tabletop, and standards badge;
- removes the complete four-button stack from the middle;
- reconstructs the cleared area with the same pale green-to-peach background and lighting;
- adds no replacement buttons and no new text;
- remains 1920 × 1080.

Output:

`Assets/Images/Akt6CenterUI/Background/akt6_center_reference_clean.png`

The edited raster supplies the detailed illustration, while Unity-native buttons provide accurate Malay labels, scalable interaction states, and preserved callbacks.

### Import settings

- Texture Type: Sprite (2D and UI)
- Sprite Mode: Single
- Alpha Is Transparency: off
- Mip Maps: off
- Wrap Mode: Clamp
- Filter Mode: Bilinear
- Compression: Uncompressed
- Max Size: 2048

## Final Layout

### Background and header

- Full-screen reference-derived background.
- Keep the green `AKTIVITI 6` badge at top center.
- Keep the large two-line title `Kebaikan Mikroorganisma`.
- Keep the subtitle:
  `Mari terokai bagaimana mikroorganisma membantu kehidupan kita.`
- Keep the standards badge at the lower right:
  `SP: 3.1`
  `SK: 3.1.1, 3.1.5`

### Three-button stack

Place a vertically centered stack in the cleared central area:

| Button | Color | Icon | Callback source |
|---|---|---|---|
| `Pengenalan` | bright cyan/blue | `Assets/Images/UI Flat/icon_book_open.png` | existing `Button (1)` |
| `Kegunaan` | fresh green | `Assets/Images/UI Flat/char_green_microbe.png` or a generated transparent leaf icon | existing `Button (2)` |
| `Kuiz` | purple | `Assets/Images/UI Flat/icon_quiz_checklist.png` | existing `Button (4)` |

Recommended button geometry at 1920 × 1080:

- center x: approximately `0`;
- width: `640–680 px`;
- height: `100–112 px`;
- vertical gap: `18–24 px`;
- stack centered around y `-95 px` in Canvas coordinates.

Each button includes:

- glossy rounded panel using `btn_green_flat.png` or `btn_white_flat.png` as a sliced sprite and tinting it per row;
- white exterior outline;
- darker lower edge/shadow;
- circular icon medallion on the left;
- centered Fredoka label in white with a dark soft shadow;
- white circular arrow area on the right using `icon_play.png`;
- no fourth or placeholder row.

### Home behavior

The reference does not show a separate Home control. Preserve navigation without disturbing the composition by making the top-center `AKTIVITI 6` badge a subtle Home hotspot that relays to the original `HomeButton` callback. Apply a small hover glow to communicate interactivity.

## Proposed Generated Hierarchy

```text
Canvas
└── Center
    ├── Akt6CenterReferenceRoot
    │   ├── ReferenceBackground
    │   ├── ActivityBadgeHomeHotspot
    │   └── MenuStack
    │       ├── PengenalanButton
    │       │   ├── IconMedallion
    │       │   ├── Label
    │       │   └── Arrow
    │       ├── KegunaanButton
    │       │   ├── IconMedallion
    │       │   ├── Label
    │       │   └── Arrow
    │       └── KuizButton
    │           ├── IconMedallion
    │           ├── Label
    │           └── Arrow
    ├── Title                         [retained, disabled]
    └── GameObject                    [retained, disabled]
```

## Callback Strategy

Use `CenterMenuButtonRelay` for every generated button:

- `PengenalanButton` relays to `Canvas/Center/GameObject/Button (1)`.
- `KegunaanButton` relays to `Canvas/Center/GameObject/Button (2)`.
- `KuizButton` relays to `Canvas/Center/GameObject/Button (4)`.
- `ActivityBadgeHomeHotspot` relays to `Canvas/Center/GameObject/HomeButton`.

This preserves the original persistent listeners and avoids duplicating scene names or navigation logic inside the builder.

The inactive legacy `Button`, inactive `Button (3)`, and any Peranan-like content are not exposed in the redesigned screen.

## Interaction States

- Normal: full intended color.
- Hover: approximately 8–12% brighter with a slight white sheen.
- Pressed: approximately 8% darker and offset downward by 1–2 px if a presenter is used.
- Disabled: desaturated at approximately 55% alpha.
- Navigation mode: `None` unless keyboard/gamepad support is explicitly added later.
- All decorative Images: `raycastTarget = false`.

## Editor Builder

Create:

`Assets/Editor/Akt6CenterUIBuilder.cs`

Menu command:

`Tools/MicroFun/Activity 6/Build Center Reference UI`

Builder responsibilities:

1. Require the exact Activity 6 scene.
2. Configure and load the clean background and shared sprites.
3. Find the four original callback sources.
4. Fail safely if any required source is missing.
5. Remove only an existing `Akt6CenterReferenceRoot`.
6. Build the full-screen reference background and three-button stack.
7. Assign each relay through `SerializedObject`.
8. Disable the legacy `Title` and `GameObject` presentation.
9. Configure the Canvas scaler for 1920 × 1080, match `0.5`.
10. Mark and save the scene.
11. Log a precise success message.

The builder must be idempotent and safe to rerun.

## Optional Runtime Presenter

Only if stronger button feedback is required, add:

`Assets/Scripts/UI/Akt6CenterPresenter.cs`

It may provide hover scale and pressed motion. It must not own navigation or scene-loading logic.

## Execution Phases and Codex Sol Effort

| Phase | Work | Recommended model / effort |
|---|---|---|
| 1. Safeguard | Confirm callbacks and create the dated scene backup | `gpt-5.6-sol — High` |
| 2. Background edit | Produce the button-free reference-derived background | `gpt-5.6-sol — High` |
| 3. Builder | Implement the three-button idempotent Center builder | `gpt-5.6-sol — High` |
| 4. Scene build | Execute the builder and save `akt6.unity` | `gpt-5.6-sol — High` |
| 5. Functional QA | Test Pengenalan, Kegunaan, Kuiz, and Home | `gpt-5.6-sol — High` |
| 6. Visual QA | Compare screenshots at 1920 × 1080 and test common 16:9 sizes | `gpt-5.6-sol — High` |

Recommended overall setting: **Codex Sol with High effort**.

Estimated implementation time: **30–50 minutes**, including the clean-background generation and visual iteration.

## Verification Checklist

- [ ] Only `Pengenalan`, `Kegunaan`, and `Kuiz` are visible.
- [ ] No `Peranan` label, icon, row, or empty gap remains.
- [ ] No visible `Kebaikan` button label remains; it reads `Kegunaan`.
- [ ] Pengenalan opens `Canvas/Pengenalan`.
- [ ] Kegunaan opens `Canvas/Kegunaan`.
- [ ] Kuiz invokes the original quiz scene callback.
- [ ] Activity badge invokes the original Home callback.
- [ ] The background contains no remnants of the removed four-button reference stack.
- [ ] The three-button stack is centered and balanced with surrounding artwork.
- [ ] Rebuilding twice produces exactly one generated root.
- [ ] Scene validation reports no missing scripts or broken prefabs.
- [ ] No compile errors appear in the Unity Console.

## Deliverables

- Updated `Assets/Scenes/Aktivity/akt6/akt6.unity` Center page.
- Dated backup scene.
- `Assets/Editor/Akt6CenterUIBuilder.cs`.
- Clean reference-derived background under `Assets/Images/Akt6CenterUI/Background/`.
- Any generated transparent icon under `Assets/Images/Akt6CenterUI/Icons/`.
- Final Play Mode screenshot under `Assets/Screenshots/`.

## Out of Scope

- Changing the Activity 6 learning standards.
- Changing Pengenalan or Kegunaan content screens.
- Rewriting quiz logic or quiz questions.
- Adding a replacement Peranan route.
- Deleting legacy serialized controls.
