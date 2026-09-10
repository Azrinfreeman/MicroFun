# Activity 6 — Four Kegunaan Detail Pages Redesign

## Status

Planning only. This document does not authorize or perform scene, script, prefab, or art changes.

## Goal

Redesign these four existing sections in `Assets/Scenes/Aktivity/akt6/akt6.unity` so they use the same polished Activity 6 visual language already established on the Center, Pengenalan, and Kegunaan pages:

1. `Canvas/Pembuatan Makanan`
2. `Canvas/Penghasilan Baja Organik`
3. `Canvas/Perawatan Sisa Kumbahan`
4. `Canvas/Penghasilan Ubat Ubatan`

The four pages should feel like one family while retaining a clear subject identity through their accent colour, illustration, and supporting science props.

## Existing Scene Findings

- The current scene is `Assets/Scenes/Aktivity/akt6/akt6.unity`.
- Pembuatan Makanan, Penghasilan Baja Organik, and Perawatan Sisa Kumbahan use an older `Top / Center / Bottom` structure with multiple `DialogController` and `Animator` objects.
- Penghasilan Ubat Ubatan uses a different `Title / Content / Button` structure, but also contains four dialog objects.
- The first three pages already have working `Bottom/HomeButton` objects. These callbacks must be preserved and exposed through the redesigned Back buttons.
- Existing dialog text, sequencing, video/audio behaviour, and serialized callbacks are treated as functional content and must not be rewritten or disconnected during the visual rebuild.

## Shared Visual Direction

All four pages will use a common 1920 × 1080 composition:

- Soft lime-to-cream-to-peach Activity 6 background.
- Decorative bubbles, leaves, sparkles, laboratory glassware, and friendly microorganism mascots.
- Green `AKTIVITI 6` pill at the top.
- Large dark-green, white-outlined page title.
- A short Malay subtitle beneath the title.
- One large rounded white/cream learning panel with soft shadow.
- A large topic illustration on one side and the existing lesson/dialogue content on the other.
- Chunky child-friendly typography consistent with the pages already rebuilt.
- Clearly visible Back button in the top-left, with the same blue/teal style used on the recent Activity 6 pages.
- Consistent `Dengar audio` control when an existing audio action is available. No new audio dependency will be invented.
- Safe spacing for 16:9 displays and no important content near the canvas edge.

## Page Identity

| Section | Accent | Main illustration | Supporting details |
|---|---|---|---|
| Pembuatan Makanan | Bright blue | Bread dough, bread, cheese, yoghurt | Baker/chef mascot, fermentation bubbles, helpful microbes |
| Penghasilan Baja Organik | Pink/magenta | Compost, organic-fertilizer bag, healthy plant | Soil cutaway, leaves, decomposer microbes |
| Perawatan Sisa Kumbahan | Teal | Wastewater-treatment tanks and clean-water droplet | Before/after water flow, pipes, helpful bacteria |
| Penghasilan Ubat Ubatan | Warm orange | Medicine bottle, blister pack, capsules | Laboratory flask, microscope, medicine-producing microbes |

Accent colours match the four quadrants/buttons on the redesigned Kegunaan page, creating a direct visual connection between the menu choice and its detail page.

## Proposed Page Layout

Each page will follow the same hierarchy and layout:

```text
<Existing Section>
└── Akt6KegunaanDetailReferenceRoot
    ├── Background
    ├── DecorativeArt
    ├── ActivityBadge
    ├── PageTitle
    ├── Subtitle
    ├── HeroIllustration
    ├── LessonPanel
    │   ├── StepBadge
    │   ├── LessonTitle
    │   ├── ExistingLessonTextHost
    │   ├── ProgressDots
    │   └── ContinueButton
    ├── AudioButton (only when backed by an existing action)
    └── BackButton
```

The generated root will be rebuilt deterministically by an Editor builder. Existing legacy objects will be retained as callback/content sources rather than deleted.

## Content and Interaction Behaviour

### Initial state

- Show the page title, hero illustration, and first existing educational message.
- Display progress such as `1 / 4` based on the existing dialog count.
- Keep the lesson panel readable over the illustrated background.

### Continue/progression

- The visible Continue button will relay the matching existing serialized Button callback or dialog action.
- Dialog order will remain exactly as currently configured.
- The redesign will update the visible lesson text when the active legacy dialog changes.
- Existing `DialogController` and `Animator` behaviour will remain operational unless testing proves a small adapter is required.

### Back behaviour

- Every detail page receives a visible Back button.
- Back returns to `Canvas/Kegunaan` using the section's existing Home/Back callback where available.
- For Penghasilan Ubat Ubatan, the currently inactive `Back` control or the known page-switch callback will be used as the relay source after callback inspection.
- Back must work from every dialog step, not only the first page state.

### Video and audio

- Existing VideoPlayer objects and clips remain untouched.
- If a lesson step currently opens or plays a video, the redesigned control will relay that exact action.
- Audio controls will only be shown where a valid existing audio callback/clip is confirmed.

## Art Assets

Create transparent-background, child-friendly 3D/cartoon illustrations for each topic. Generated images must contain no embedded instructional text so Malay labels remain editable in TextMeshPro.

Planned folders:

```text
Assets/Images/Akt6KegunaanDetailsUI/
├── Shared/
├── PembuatanMakanan/
├── BajaOrganik/
├── SisaKumbahan/
└── UbatUbatan/
```

Where practical, reuse the existing Activity 6 background, bubbles, leaves, microscope, flask, and button sprites to keep the scene visually consistent and reduce texture memory.

Image import settings:

- Texture Type: Sprite (2D and UI)
- Alpha Is Transparency: enabled for cut-outs
- Compression: Normal/High Quality after visual review
- Max Size: normally 2048 for hero art and 1024 or below for supporting props
- No opaque white/checkerboard background around icons

## Implementation Architecture

Create one reusable editor builder rather than four unrelated scripts:

`Assets/Editor/Akt6KegunaanDetailPagesUIBuilder.cs`

Responsibilities:

- Validate that the active scene is Activity 6.
- Create a timestamped backup scene before the first mutation.
- Rebuild the generated roots safely and idempotently.
- Apply the shared layout and page-specific accent configuration.
- Preserve existing lesson text and callbacks.
- Create visible controls that relay existing serialized Button events.
- Hide superseded legacy visuals without deleting functional source objects.
- Mark the scene dirty and save only after a successful build.

If required, add a small runtime adapter:

`Assets/Scripts/UI/Akt6KegunaanDetailPagePresenter.cs`

The adapter may synchronize the visible title/text/progress with the existing active dialog. It must not duplicate scene-navigation logic already stored in UnityEvents.

Suggested editor menu:

`Tools/MicroFun/Activity 6/Build Four Kegunaan Detail Pages`

## Implementation Phases and Codex Sol Effort

| Phase | Work | Codex Sol effort |
|---|---|---|
| 1. Functional audit | Record all four dialog sequences, texts, Button UnityEvents, video/audio links, and return behaviour | High |
| 2. Visual assets | Generate/import four hero illustrations and shared decorative assets; remove unwanted backgrounds | High |
| 3. Shared builder | Implement the reusable builder, per-page configuration, safe legacy hiding, and back-button relays | High |
| 4. Interaction adapter | Add synchronization only where the legacy DialogController cannot directly drive the new visible UI | High |
| 5. Page verification | Test every lesson step, Back action, video/audio action, and page transition in Play Mode | XHigh |
| 6. Final polish | Check 16:9 layout, icon transparency, text overflow, button hit areas, hierarchy cleanliness, and console errors | High |

Recommended overall setting: **Codex Sol — High**. Use **XHigh** for the Play Mode integration pass because the four pages contain different legacy structures and serialized dialog callbacks.

## Verification Checklist

- All four pages visibly match the established Activity 6 theme.
- Each page is immediately identifiable by its assigned accent colour and illustration.
- No embedded text appears inside generated bitmap art.
- Malay text remains editable as TextMeshPro content.
- Existing lesson copy remains complete and in the original order.
- Existing dialog animations and media actions still work.
- Back returns to the redesigned Kegunaan page from every page and every step.
- Buttons have large, unobstructed hit areas and correct hover/pressed feedback.
- No old UI leaks through the new design.
- No transparent icon has a white or checkerboard box.
- The scene has no missing scripts, broken references, or new console errors.
- The result is captured in Play Mode at 1920 × 1080 for visual approval.

## Out of Scope

- Rewriting curriculum content.
- Replacing existing videos or narration without a separate request.
- Changing the Kegunaan menu, Center, Pengenalan, or quiz pages.
- Deleting legacy dialog objects before the redesigned pages have passed full regression testing.

## Execution Gate

Implementation begins only after explicit approval to proceed. The first execution step will be a callback/content audit and scene backup; visual generation and hierarchy changes will follow after that audit.
