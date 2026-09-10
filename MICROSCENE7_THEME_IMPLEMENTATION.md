# Micro Scene 7 Theme Redesign — Implementation Plan

## Status

Planning only. No Unity scene, script, callback, asset, animation, video, or Build Settings changes are performed by this document.

## Target Scene

- Scene: `Assets/Scenes/Aktivity/akt7/microscene7.unity`
- Proposed backup: `Assets/Scenes/Aktivity/akt7/microscene7_PreThemeRedesign_20260901.unity`
- Proposed builder: `Assets/Editor/Akt7MicroSceneUIBuilder.cs`
- Optional new art folder: `Assets/Images/Akt7MicroSceneUI/`

## Goal

Bring Micro Scene 7 into the same visual family as the redesigned Activity 7 menu, Pengenalan, Keburukan, and Kuiz 7 scenes while retaining its existing interactive discovery behavior.

The finished screen should feel like a polished Activity 7 science exhibit:

- warm cream, pale-green, and peach background;
- bold rounded dark-green typography;
- `AKTIVITI 7` badge and `MICRO-SCENE` heading;
- friendly science decorations and transparent character artwork;
- large rounded discovery cards;
- clear hover/tap states and microorganism labels;
- themed page navigation;
- readable explanatory pop-ups; and
- consistent back navigation.

## Existing Scene Contract

The live scene audit found the following behavior that must be preserved.

### Primary content

- Page 1 is `Canvas/Content` and contains six interactive examples:
  - Bread
  - Dirt
  - RawChicken
  - Dough
  - Water
  - Streptococcus
- Page 2 is `Canvas/Content (1)` and currently contains one interactive Spirillum example.
- `next` and `back` buttons toggle the two pages and their own visibility through four existing persistent callbacks each.

### Hover and reveal behavior

- Each example contains a `collider2d` object with `OnHoverMouse`.
- Hovering activates the card's `Content` overlay.
- Leaving the card plays the existing `ExitContent` Animator state.
- Some reveal labels or buttons open additional explanatory content.
- `GameObjectControl.Disappear()` supports the existing reveal animation lifecycle.

### Detail pop-ups

- `GigiReput` contains the title `GIGI REPUT` and its explanatory text.
- `RotiReput` contains the title `PEREPUTAN ROTI` and its explanatory text.
- Each has its own close button and active-state callback.
- The current serialized mappings will be recorded before rebuilding. Any apparently mismatched label-to-popup mapping will be preserved unless it is confirmed as an actual content bug during execution.

### Background and media

- The background uses a looping video clip named `33131-395456708` rendered into `New Render Texture`.
- Two `VideoPlayer` objects reference the same clip and RenderTexture; only one is currently active.
- Two full-screen `RawImage` objects reference that RenderTexture; only one is currently active.
- The redesign will preserve the working animated background path and will not duplicate video playback.

### Navigation and input

- The Canvas contains `CustomCursor`.
- The top Back button contains `BackController` and also has one persistent scene callback.
- Execution must test additive and standalone scene use so the redesign does not trigger navigation twice.
- The scene uses the existing Input System EventSystem.

## Design Direction

### 1. Background treatment

Retain the animated RenderTexture as the background when it plays correctly, then place a themed, non-interactive wash above it:

- pale green on the left;
- warm cream in the center;
- peach/orange on the right;
- reduced video contrast so foreground text remains readable;
- light bubbles, DNA, atom, sparkles, microbes, microscope, petri dish, and water-drop decoration around the safe edges.

If the looping video is visually incompatible or unreliable in a build, the approved execution phase may replace only its visual layer with a static text-free Activity 7 background while leaving the original video objects recoverable in the backup.

All decorative images will have `Raycast Target` disabled.

### 2. Header

Create a consistent Activity 7 header:

- centered rounded badge: `AKTIVITI 7`;
- large title: `MICRO-SCENE`;
- subtitle: `Terokai situasi dan kenali mikroorganisma yang terlibat.`;
- compact progress/page badge: `Halaman 1/2`;
- themed back/home control at the top-left.

The title and labels remain editable TextMeshPro content rather than text baked into images.

### 3. Discovery-card layout

Replace the legacy layout groups visually with a deliberate responsive grid.

Page 1:

- two rows of three large cards;
- each card has a rounded cream frame, colored accent, image viewport, and label area;
- sufficient gaps for hover animation and touch selection;
- examples retain their original image, mask, animator, collider, and reveal `Content` hierarchy.

Page 2:

- center the current Spirillum card in a featured-card layout;
- reserve additional balanced space for future examples without inventing content now;
- show the same page header and navigation language.

The six existing page-one objects and page-two Spirillum object will be restyled and repositioned rather than replaced wherever possible. This keeps their Animator and `OnHoverMouse` references intact.

### 4. Interaction states

Each discovery card will provide clear feedback:

| State | Treatment |
|---|---|
| Normal | Cream card, soft shadow, colored accent |
| Hover/focus | Slight scale lift, brighter outline, visible microorganism reveal |
| Pressed/tap | Short compressed state with stronger accent |
| Reveal active | Label chip and pointer/tail clearly connected to the example |
| Unavailable | Muted card without interaction |

Desktop hover behavior will be retained. Touch and click behavior will also be tested; if hover-only interaction is inaccessible on touch devices, a small compatibility component will expose the same reveal on click without changing the educational content.

### 5. Page controls

- Restyle `next` and `back` into green/cream rounded navigation buttons.
- Use `SETERUSNYA` and `SEBELUMNYA` labels with clear directional arrows.
- Preserve the four existing active-state callbacks on each control.
- Show `Halaman 1/2` or `Halaman 2/2` using a small presenter bound to the active content page.
- Keep the page buttons outside the card grid so they never cover hover targets.

### 6. Detail pop-ups

Restyle `GigiReput` and `RotiReput` as themed modal cards:

- dimmed translucent backdrop;
- large cream rounded panel;
- green Activity 7 title treatment;
- separated `Punca / sebab` and pupil-friendly explanation sections;
- improved line spacing and readable maximum width;
- clear circular close button;
- modal placed above all discovery cards and decorative artwork;
- input blocked behind the modal.

The current Malay copy will be preserved exactly unless a separate content-edit request is made.

## Proposed Hierarchy

```text
Canvas
├── AnimatedBackgroundSource          (preserved)
├── Akt7MicroSceneThemeRoot
│   ├── BackgroundWash
│   ├── EdgeDecorations
│   ├── Header
│   │   ├── BackButtonPresentation
│   │   ├── ActivityBadge
│   │   ├── Title
│   │   ├── Subtitle
│   │   └── PageIndicator
│   ├── PageFrame
│   │   ├── PageOnePresentation
│   │   └── PageTwoPresentation
│   └── PageNavigation
│       ├── PreviousButtonPresentation
│       └── NextButtonPresentation
├── Content                           (preserved functional page 1)
├── Content (1)                       (preserved functional page 2)
├── GigiReput                         (preserved, restyled)
└── RotiReput                         (preserved, restyled)
```

If moving the existing content objects would disturb their Animator paths, the presentation root will frame them in place rather than reparent them.

## Technical Implementation

### Idempotent builder

Add `Akt7MicroSceneUIBuilder.cs` with a menu item such as:

- `Tools/MicroFun/Activity 7/Build Micro Scene 7 Theme`

The builder will:

1. Require `microscene7.unity` to be active.
2. Verify the Canvas, two content pages, navigation buttons, detail pop-ups, EventSystem, RenderTexture background, and all seven hotspot objects.
3. Record persistent button callbacks and serialized `OnHoverMouse` references.
4. Remove only a previous `Akt7MicroSceneThemeRoot` before rebuilding.
5. Create the header, background wash, decorations, card frames, page indicator, and navigation presentation.
6. Reposition/restyle the existing functional card objects.
7. Restyle both existing modals without replacing their text or close callbacks.
8. Configure the Canvas Scaler for `1920 × 1080`, match value approximately `0.5`.
9. Disable raycasts on every purely decorative image.
10. Save only after reference validation succeeds.

The builder must be safe to run repeatedly without duplicate listeners, EventSystems, VideoPlayers, AudioListeners, or theme roots.

### Optional presenter component

Add a lightweight runtime component only if required:

- `Assets/Scripts/UI/Akt7MicroScenePresenter.cs`

Responsibilities may include:

- updating `Halaman X/2` when navigation is pressed;
- translating pointer click into the existing hover reveal for touch devices;
- closing open reveals when changing pages; and
- ensuring only one detail modal is active.

It must not replace `OnHoverMouse`, `GameObjectControl`, the card Animators, or current navigation callbacks.

## Asset Strategy

Priority order:

1. Reuse Activity 7 background, microscope, microbes, petri dish, water-drop, arrows, home/back, and rounded panel assets already in the project.
2. Reuse `Assets/Images/UI Flat/` button and panel sprites.
3. Preserve the original example imagery inside Bread, Dirt, RawChicken, Dough, Water, Streptococcus, and Spirillum.
4. Generate only missing text-free decorations or a static fallback background if necessary.
5. Keep all labels and explanatory text in TextMeshPro.

Any generated transparent PNG will be placed under `Assets/Images/Akt7MicroSceneUI/`, imported as a UI Sprite, configured without mipmaps, and checked for alpha halos.

## Implementation Phases and Codex Sol Effort

| Phase | Work | Recommended effort |
|---|---|---|
| 1. Backup and callback audit | Duplicate scene; record hotspots, Animators, callbacks, modals, cursor, and video references | `gpt-5.6-sol` — High |
| 2. Theme foundation | Background wash, Activity 7 header, typography, decorations, and scaler | `gpt-5.6-sol` — High |
| 3. Card redesign | Responsive two-page card layout while preserving original interactive children | `gpt-5.6-sol` — High |
| 4. Navigation and modals | Page indicator, themed controls, double-navigation check, modal restyle | `gpt-5.6-sol` — High |
| 5. Interaction QA | Hover, click/touch, animations, pop-ups, page switching, cursor, and additive navigation tests | `gpt-5.6-sol` — High |
| 6. Visual polish | 1920×1080 comparison and multi-aspect correction | `gpt-5.6-sol` — High |

`High` effort is recommended. Use `xhigh` only if the hover system must be extended substantially for touch devices or custom card artwork must be regenerated.

## Verification Checklist

### Scene integrity

- One Canvas, EventSystem, Main Camera, and AudioListener in standalone use.
- No missing scripts, sprites, Animators, RenderTextures, or video clips.
- Exactly one active background VideoPlayer path.
- Builder runs twice without duplication or broken references.
- Scene validates cleanly.

### Interaction

- All six page-one hotspots reveal the correct overlay.
- Spirillum reveals correctly on page two.
- Existing exit animation runs when the pointer leaves.
- Detail buttons open the intended explanatory pop-up.
- Both modal close buttons work.
- Next and previous page buttons preserve their existing active-state behavior.
- Page indicator updates correctly.
- Touch/click fallback works if required.
- Custom cursor activates and resets correctly.
- Back works once in standalone mode and once in additive mode without double navigation.

### Visual QA

- Capture at `1920 × 1080`.
- Check 1600×900, 1366×768, 1280×720, 1920×1200, and 1024×768.
- No card, label, pointer-tail, or modal text clipping.
- Decorative art never blocks a collider or button.
- Page navigation remains visible at every tested aspect ratio.
- Explanatory text remains comfortable to read at classroom distance.

## Acceptance Criteria

The redesign is complete when:

1. Micro Scene 7 clearly matches the established Activity 7 visual theme.
2. The two-page content structure and all seven discoveries remain functional.
3. Existing hover animations and detail pop-ups work without lost references.
4. Navigation works in both standalone and additive loading contexts.
5. The looping background is preserved or safely replaced with an approved fallback.
6. The scene has no new console errors or validation issues.
7. The layout is responsive and touch-safe.
8. The original scene remains recoverable from the dated backup.

## Out of Scope

- Rewriting the Malay learning content.
- Adding new microorganism examples to page two.
- Replacing the existing video clip solely for aesthetic reasons.
- Changing Activity 7 scene routing or Build Settings.
- Changing the educational mapping of cards and pop-ups without confirming it is incorrect.

## Expected Deliverables After Approval

- Updated `Assets/Scenes/Aktivity/akt7/microscene7.unity`.
- Backup `Assets/Scenes/Aktivity/akt7/microscene7_PreThemeRedesign_20260901.unity`.
- `Assets/Editor/Akt7MicroSceneUIBuilder.cs`.
- Optional `Assets/Scripts/UI/Akt7MicroScenePresenter.cs` if page/touch synchronization is required.
- Any new art under `Assets/Images/Akt7MicroSceneUI/`.
- Final 1920×1080 Game View screenshot.
- Short validation report covering all seven hotspots, both pages, both modals, cursor behavior, video background, and navigation.
