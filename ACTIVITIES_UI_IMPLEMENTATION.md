# Activities Scene UI — Implementation Plan

## Status

Planning only. No Unity scene, script, prefab, material, or image asset has been changed as part of this document.

## Recommended effort

- **Codex reasoning effort:** High
- **Implementation complexity:** Medium–high
- **Estimated production time:** 10–16 hours, including asset generation, Unity integration, interaction polish, and multi-resolution QA
- **Expected delivery shape:** one focused implementation pass followed by one visual-polish/QA pass

The largest variable is image-generation iteration. The Unity hierarchy already contains the eight activity buttons and central composition, so the functional restructuring should be modest.

## Goal

Restyle `Assets/Scenes/Activities.unity` so it closely matches the supplied 16:9 reference:

- warm cream-to-lime-to-orange illustrated background;
- large playful green title and compact activity badge;
- Malay subtitle beneath the title;
- circular central illustration of students exploring microorganisms;
- eight colorful pill-shaped activity buttons arranged around the central illustration;
- dotted green connector paths and circular junctions;
- Activity 8 shown as the prominent selected/completed item;
- decorative microscope, petri dish, leaves, bubbles, sparkles, and cute microorganisms around the edges;
- polished hover/press feedback while retaining the scene's current navigation behavior.

The UI should feel like the reference at first glance while remaining clean, responsive, and editable in Unity. Text and interactive controls will remain native Unity UI rather than being baked into one generated image.

## Read-only scene findings

The active scene is `Assets/Scenes/Activities.unity` in Unity `6000.4.5f1`.

Current relevant structure:

```text
Activities
├── Main Camera
├── Directional Light
├── Canvas
│   ├── Image                  (full-screen RawImage using the current video RenderTexture)
│   ├── Image (1)              (903 × 903 central composition)
│   │   ├── Image              (ring.png)
│   │   ├── middlePerson       (middlepeople.png)
│   │   ├── Right
│   │   │   └── Activity buttons 1–4
│   │   └── Left
│   │       └── Activity buttons 8–5
│   └── Button                 (separate navigation button)
├── Video Player
└── EventSystem
```

Useful existing implementation details:

- The Canvas already uses `Scale With Screen Size` at a `1920 × 1080` reference resolution.
- The Canvas owns `SceneController` and an existing Animator controller.
- The button order already matches the reference: Activities 1–4 on the right and Activities 8–5 on the left.
- Existing labels use `Assets/Fonts/FredokaOne SDF.asset`, which is well suited to the playful rounded typography in the reference.
- The central art currently uses `Assets/Images/ring.png` and `Assets/Images/middlepeople.png`.
- A useful existing art library is available under `Assets/Images/UI Flat`, including microbe characters, microscope artwork, icons, backgrounds, and rounded UI sprites.
- Current button bodies are simple solid-color built-in UI sprites. Their visuals should be replaced, but the existing Button GameObjects should be retained wherever possible to protect existing callbacks and navigation.

## Visual specification

### Composition

Use a `1920 × 1080` authored layout with the central portrait slightly below screen center. Keep all important interaction inside a central 16:9 safe composition.

Approximate authored positions, expressed as Canvas-space guidance rather than final locked values:

| Element | Approximate placement | Approximate size |
|---|---:|---:|
| Top badge | `(0, 470)` | `350 × 78` |
| Main title | `(0, 365)` | `1350 × 150` |
| Subtitle | `(0, 285)` | `1150 × 58` |
| Central portrait | `(0, -55)` | `465 × 465` |
| Activity 1 | `(-430, 185)` | `340 × 100` |
| Activity 8 selected | `(-500, 35)` | `430 × 125` |
| Activity 7 | `(-470, -125)` | `340 × 100` |
| Activity 6 | `(-390, -280)` | `340 × 100` |
| Activity 2 | `(410, 185)` | `340 × 100` |
| Activity 3 | `(500, 35)` | `340 × 100` |
| Activity 4 | `(505, -125)` | `340 × 100` |
| Activity 5 | `(410, -280)` | `340 × 100` |

Final positions should be tuned against an actual Game-view capture, not accepted solely from these numbers.

### Copy

- Badge: `AKTIVITI 8`
- Title: `Jom Teroka Aktiviti 8!`
- Subtitle: `Aktiviti pengukuhan untuk menguji kefahaman tentang mikroorganisma.`
- Buttons: `Aktiviti 1` through `Aktiviti 8`

### Color direction

- Background cream: `#FFF8DE`
- Soft lime field: `#EAF7A8`
- Warm lower glow: `#FFC47C`
- Main green: `#229A24`
- Deep green text/shadow: `#075A20`
- Navy subtitle: `#102D5A`
- Connector green: `#52A929`
- Button families: lime, teal, amber, violet, blue, pink, cyan, and selected green

Exact values may shift slightly after comparing imported sprites under Unity color space.

## Asset strategy

### Generate as original raster art

Generate new artwork in the same cheerful educational 3D-cartoon direction, without generating any text:

1. `activities_bg_decor_1920x1080.png`
   - cream/lime/orange gradient;
   - edge decorations only;
   - microscope at lower left, petri dish at lower right;
   - leaves, bubbles, sparkles, and friendly microorganisms;
   - deliberately clear central/header/button zones;
   - no lettering, logos, UI buttons, or baked connector lines.

2. `activities_center_students_1024.png`
   - three Malaysian primary-school students exploring a petri dish in a classroom;
   - warm, friendly 3D-cartoon rendering;
   - centered group with enough margin for a circular crop;
   - no text;
   - generated separately so it remains easy to replace.

3. Eight transparent icon assets, authored consistently at `256 × 256`:
   - clipboard/pencil;
   - magnifying glass;
   - microscope;
   - shield/question mark;
   - puzzle pieces;
   - light bulb;
   - chart;
   - check mark.

Before generating duplicates, compare the existing `Assets/Images/UI Flat` icons and reuse any that already match the target quality. The existing microscope and microbe artwork can also be composited into the decorative background if their style is consistent.

### Build natively in Unity

Do not bake the title, subtitle, button text, state, or interaction into raster artwork. Build these with TextMeshPro and sliced UI sprites so they stay sharp and localizable.

Each pill button should contain:

```text
ActivityButton
├── Shadow
├── OuterFrame
├── Fill
├── InnerHighlight
├── IconBadge
│   ├── BadgeShadow
│   ├── BadgeFill
│   └── Icon
├── Label (TMP)
└── OptionalSelectedGlow
```

Use one reusable button prefab with configurable palette, icon, label, and selected state. Prefer 9-sliced rounded sprites for the frame/fill. Add a subtle lower shadow, white rim, upper highlight, and restrained gradient to reproduce the dimensional candy-like finish.

## Proposed scene hierarchy

Retain the existing Canvas and functional Button objects, then organize the visuals as follows:

```text
Canvas
├── Background
│   ├── BackgroundGradient
│   └── EdgeDecorations
├── Header
│   ├── ActivityBadge
│   ├── MainTitle
│   └── Subtitle
├── ActivityMap
│   ├── Connectors
│   ├── CenterPortrait
│   │   ├── PortraitShadow
│   │   ├── PortraitMask
│   │   ├── StudentsImage
│   │   ├── InnerWhiteRing
│   │   └── OuterGreenRing
│   ├── LeftButtons
│   │   ├── Activity01
│   │   ├── Activity08_Selected
│   │   ├── Activity07
│   │   └── Activity06
│   └── RightButtons
│       ├── Activity02
│       ├── Activity03
│       ├── Activity04
│       └── Activity05
├── Navigation
│   └── ExistingBackButton
└── OptionalFrontSparkles
```

The current full-screen video can remain in the project but should be disabled or placed behind an opaque static background for this design unless motion is specifically desired.

## Implementation phases

### Phase 1 — Preserve and baseline

- Record the current active scene, button hierarchy, Button callbacks, Animator bindings, and navigation behavior.
- Save a reversible scene backup or rely on a clean version-control checkpoint before visual restructuring.
- Capture a Game-view baseline at 1920 × 1080.
- Confirm each Activity button's intended destination before changing hierarchy names or references.

### Phase 2 — Create/import visual assets

- Reuse suitable existing UI Flat art.
- Generate only the missing background, portrait, and icons.
- Import with correct alpha, max texture sizes, sprite mode, compression, and pivots.
- Create sliced button/frame sprites and TMP material presets.
- Keep generated source files in a dedicated folder such as `Assets/Images/ActivitiesUI`.

### Phase 3 — Rebuild presentation in place

- Replace the video-backed presentation with the new decorative background.
- Add the header badge, main title, and subtitle using FredokaOne TMP.
- Convert the central composition to a circular masked portrait with multiple rings and shadow.
- Reposition the current left/right groups to match the reference.
- Restyle existing Button roots using the reusable pill-button visual structure.
- Make Activity 8 approximately 20–25% larger than the regular buttons and display the check badge/glow.
- Add dotted connector paths and circular nodes behind buttons and portrait.
- Keep raycast targets only on required interactive graphics.

### Phase 4 — Interaction polish

- Preserve existing click destinations and `SceneController` behavior.
- Add gentle hover/focus scaling (`1.00 → 1.035`), press compression (`1.00 → 0.97`), and a short highlight response.
- Add a subtle idle glow/pulse to selected Activity 8; avoid constant movement elsewhere.
- Configure explicit keyboard/gamepad navigation in the visual order 1→8 or the product's required progression order.
- Respect `interactable = false` if any activities are locked in the current logic.

### Phase 5 — Verification

- Check Unity compilation and resolve all errors.
- Confirm all nine existing buttons still navigate correctly.
- Verify no decorative image blocks pointer input.
- Test at `1920 × 1080`, `1366 × 768`, `2560 × 1440`, and `3840 × 2160`.
- Inspect at least one wider and one taller aspect ratio for clipping.
- Capture final Game-view screenshots and compare side-by-side with the reference.
- Run available EditMode/PlayMode tests and perform a short Play Mode interaction pass.

## Responsive behavior

- Keep the existing `1920 × 1080` Canvas reference resolution.
- Use `Match Width Or Height` near `0.5`; tune only after testing target devices.
- Place the full-screen background in cover mode so it never exposes empty edges.
- Put the header and activity map under separate anchored containers.
- Scale the complete ActivityMap uniformly on non-16:9 displays rather than allowing buttons to drift independently.
- Keep decorative edge art disposable: it may crop slightly, but title, subtitle, portrait, buttons, and navigation must remain fully visible.

## Risks and mitigations

| Risk | Mitigation |
|---|---|
| Generated art contains accidental text or unusable spacing | Generate decoration, portrait, and icons separately; keep all typography native in Unity. |
| Existing callbacks break during hierarchy cleanup | Restyle current Button GameObjects in place and record bindings before any rename/reparent operation. |
| The design looks good only at 4K | Author at the Canvas reference resolution and verify multiple Game-view resolutions before sign-off. |
| Too many large transparent textures increase memory | Crop transparent bounds, use sensible max sizes, atlas small icons, and use platform-appropriate compression. |
| Decorative art blocks clicks | Disable `Raycast Target` on every non-interactive Image and TMP element. |
| Visual result becomes a flat screenshot | Keep buttons, text, selection, connectors, portrait rings, and feedback as layered Unity UI. |

## Acceptance criteria

The implementation is complete when:

- the first impression closely matches the supplied reference's composition, color, and playful educational style;
- all reference copy is present and readable;
- all eight activities appear in the correct left/right arrangement;
- Activity 8 is clearly selected and visually dominant;
- the central circular student/microorganism illustration and connector map are present;
- edge decorations frame the interface without obstructing interaction;
- all current navigation remains functional;
- there are no new Unity console errors;
- the layout remains usable at the specified test resolutions;
- final screenshots have been reviewed against the reference and given a final spacing/color polish pass.

## Execution boundary

Approval of this document authorizes a later implementation pass. Until then, no modifications should be made to `Activities.unity`, scripts, imported assets, button bindings, or animation controllers.
