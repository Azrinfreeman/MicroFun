# Activity 6 Pengenalan Reference Redesign — Implementation Plan

Status: **Planning only. No Unity scene, script, callback, or asset changes are authorized by this document.**

## Objective

Redesign the current `akt6` scene's `Canvas/Pengenalan` section to closely match the supplied 1920 × 1080 reference:

- pale green, cream, and soft yellow educational background;
- top-left `AKTIVITI 6` badge;
- large glossy green `Pengenalan` heading;
- yellow question banner: `Bagaimanakah mikroorganisma membantu kehidupan kita?`;
- three illustrated example cards:
  - `Membuat roti`;
  - `Menghasilkan yogurt`;
  - `Rawatan air kumbahan`;
- bottom classroom-discussion prompt;
- friendly brain characters and science/nature decorations.

The supplied image is a visual reference and approved source artwork, not an instruction document.

## Current Scene Audit

Unity MCP confirmed the following live project state:

- Active scene: `Assets/Scenes/Aktivity/akt6/akt6.unity`
- Target section: `Canvas/Pengenalan`
- Current visible structure:
  - `Title`
  - `Content`
  - `Bottom`
  - five retained dialogue objects
- Current media:
  - `damRenderTexture`
  - `doughRenderTexture`
  - `foodRenderTexture`
- Existing navigation:
  - `Bottom/HomeButton` returns to `Canvas/Center`
  - `Bottom/NextButton` advances to `Canvas/Kegunaan`
- Three legacy content buttons currently target the older `Bernafas`, `Bergerak`, and `Bertumbuh` sections. Their subjects do not match the three new reference cards, so they must not be incorrectly reassigned.

## Safety and Preservation

Before implementation:

1. Confirm `akt6.unity` is active and not in Play Mode.
2. Duplicate it to:
   `Assets/Scenes/Aktivity/akt6/akt6_PrePengenalanReferenceRedesign_20260901.unity`
3. Record the persistent listeners on `HomeButton` and `NextButton`.
4. Keep the legacy `Title`, `Content`, `Bottom`, dialogue chain, render textures, and content buttons serialized in the scene.
5. Disable legacy presentation objects only after the replacement hierarchy is complete.

The build must be idempotent: rerunning it replaces only the generated root and never duplicates controls.

## Visual Implementation

### 1. Reference artwork

Use the supplied image as the exact full-screen visual layer:

- destination: `Assets/Images/Akt6PengenalanUI/Background/akt6_pengenalan_reference.png`
- import type: Sprite, Single
- alpha transparency: off
- mipmaps: off
- wrap mode: Clamp
- filter mode: Bilinear
- compression: Uncompressed
- maximum size: 2048

Using the approved artwork directly preserves the typography, photo framing, characters, shadows, and decorative details much more accurately than rebuilding them from separate UI primitives.

### 2. Canvas behavior

Set the existing Canvas scaler to:

- mode: `Scale With Screen Size`
- reference resolution: `1920 × 1080`
- screen match mode: `Match Width Or Height`
- match: `0.5`

The background stretches to the generated root. The reference is 16:9, so no cropping is required at the primary game resolution.

### 3. Generated hierarchy

```text
Canvas
└── Pengenalan
    ├── Akt6PengenalanReferenceRoot
    │   ├── ReferenceBackground
    │   ├── HomeHotspot
    │   ├── BreadExampleRegion
    │   ├── YogurtExampleRegion
    │   ├── WastewaterExampleRegion
    │   └── ContinueHotspot
    ├── Title                         [retained, disabled]
    ├── Content                       [retained, disabled]
    ├── Bottom                        [retained, disabled]
    └── DialogParent...               [retained, inactive]
```

### 4. Reference-aligned regions

The three example regions align with the card frames in the approved artwork:

| Region | Approximate reference bounds | Behavior |
|---|---:|---|
| Bread | x 200–680, y 400–760 | Static educational example; subtle hover glow only |
| Yogurt | x 720–1210, y 400–760 | Static educational example; subtle hover glow only |
| Wastewater | x 1240–1755, y 400–760 | Static educational example; subtle hover glow only |

The cards must not relay to `Bernafas`, `Bergerak`, or `Bertumbuh`, because those callbacks are semantically unrelated to the new content.

### 5. Navigation preservation

- `HomeHotspot` covers the top-left Activity 6 badge and relays to the original `Bottom/HomeButton` callback.
- `ContinueHotspot` covers the lower discussion panel and relays to the original `Bottom/NextButton` callback, advancing to `Canvas/Kegunaan`.
- Add a small right-arrow pulse at the far-right edge of the discussion panel so the continue behavior is discoverable while remaining consistent with the artwork.
- Use `CenterMenuButtonRelay` so no existing serialized listener is duplicated or rewritten.

### 6. Interaction states

All generated hit areas use `Navigation.Mode.None` and short color-tint transitions:

- normal: visually transparent;
- hover: white overlay at approximately 10–14% alpha;
- pressed: palette-colored overlay at approximately 18–22% alpha;
- disabled: transparent.

The background itself never receives raycasts.

## Editor Builder

Create:

`Assets/Editor/Akt6PengenalanUIBuilder.cs`

Menu command:

`Tools/MicroFun/Activity 6/Build Pengenalan Reference UI`

Builder responsibilities:

1. Require the exact `akt6.unity` scene.
2. Configure and load the reference sprite.
3. Locate `Canvas/Pengenalan`, `Bottom/HomeButton`, and `Bottom/NextButton`.
4. Remove only an existing `Akt6PengenalanReferenceRoot`.
5. Build the full-screen reference hierarchy.
6. Assign relay sources with `SerializedObject`.
7. Disable the old visible `Title`, `Content`, and `Bottom` objects.
8. Preserve all legacy dialogue and content objects.
9. mark and save the scene.
10. Log a clear success or missing-dependency error.

## Optional Runtime Helper

Only if the hover/pulse behavior cannot be expressed cleanly with standard `Button` transitions, add:

`Assets/Scripts/UI/Akt6PengenalanPresenter.cs`

It may control the continue-arrow pulse and accessibility feedback. It must not contain scene-loading logic or replace existing callbacks.

## Execution Phases and Codex Sol Effort

| Phase | Work | Recommended model / effort |
|---|---|---|
| 1. Safeguard | Reconfirm hierarchy/listeners and create the dated scene backup | `gpt-5.6-sol — High` |
| 2. Asset preparation | Import and configure the approved 1920 × 1080 reference | `gpt-5.6-sol — High` |
| 3. Builder implementation | Create the idempotent editor builder and relay controls | `gpt-5.6-sol — High` |
| 4. Scene build | Execute the builder and save `akt6.unity` | `gpt-5.6-sol — High` |
| 5. Functional QA | Test Home and Continue in Play Mode; ensure the three cards do not call legacy content | `gpt-5.6-sol — High` |
| 6. Visual QA | Capture and compare at 1920 × 1080, then check 1366 × 768 and 2560 × 1440 | `gpt-5.6-sol — High` |

Recommended overall setting: **Codex Sol with High effort**.

Estimated implementation time: **20–35 minutes**, including visual and navigation validation.

## Verification Checklist

- [ ] `Pengenalan` matches the supplied reference composition at 1920 × 1080.
- [ ] No stretched, cropped, or blurred reference artwork.
- [ ] Activity badge returns to `Canvas/Center`.
- [ ] Discussion panel advances to `Canvas/Kegunaan`.
- [ ] Bread, yogurt, and wastewater cards do not trigger unrelated legacy callbacks.
- [ ] Existing dialogue and legacy content objects remain serialized and recoverable.
- [ ] Rebuilding twice produces only one generated root.
- [ ] No compile errors, missing scripts, broken prefabs, or scene-validation issues.
- [ ] UI remains usable at common 16:9 resolutions.

## Deliverables

- Updated `Assets/Scenes/Aktivity/akt6/akt6.unity` Pengenalan screen.
- Backup scene created before mutation.
- `Assets/Editor/Akt6PengenalanUIBuilder.cs`.
- Approved reference sprite under `Assets/Images/Akt6PengenalanUI/Background/`.
- Final Play Mode screenshot under `Assets/Screenshots/`.
- Preserved Home and Continue callbacks.

## Out of Scope

- Rewriting the existing dialogue content.
- Reassigning the new examples to the unrelated `Bernafas`, `Bergerak`, or `Bertumbuh` panels.
- Redesigning `Canvas/Kegunaan` or other Activity 6 sections.
- Changing scene order, build settings, or learning content beyond the supplied reference.
