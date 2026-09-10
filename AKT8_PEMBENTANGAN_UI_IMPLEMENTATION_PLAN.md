# Aktiviti 8 — Pembentangan UI Implementation Plan

## Status

Planning only. This document does not change the Unity scene, generate assets, edit callbacks, or enter Play Mode.

## Objective

Redesign the current Aktiviti 8 **Pembentangan** section to closely match the supplied 1920×1080 reference while preserving the project's existing navigation and content.

- Scene: `Assets/Scenes/Aktivity/akt8/akt8.unity`
- Target page: `Canvas/Pembentangan`
- Design resolution: 1920×1080, 16:9

## Exact UI Copy

All wording will be native TextMeshPro text, not baked into generated images:

- `AKTIVITI 8`
- `Pembentangan`
- `Bentangkan hasil perbincangan tentang mikroorganisma kumpulan anda.`
- `Dengar audio`
- `Tugasan`
- `Terangkan ciri, contoh dan kepentingan mikroorganisma yang dipilih.`
- `Aktiviti pembentangan berkumpulan`
- `Ciri`, `Contoh`, `Kepentingan`

## Current Scene Audit

Unity MCP inspection found:

- `Canvas/Pembentangan` exists and is currently active.
- Its visible child is `PengenalanRedesignRoot`, containing the Pengenalan puzzle and `Dengar audio` control.
- The container also has inactive legacy `Title`, `Content`, `Bottom`, and five dialogue panels.
- The existing Home route hides `Pembentangan` and shows `Center`.
- The existing Next route hides `Pembentangan` and shows `Microsquad`.
- The audio component has no narration clip assigned.
- The redesigned center-menu `PembentanganButton` currently has zero callbacks.
- The separate center-menu `MicroThinker` button also remains callback-free, as previously requested.

The implementation must isolate and preserve the shared Pengenalan/legacy objects. Nothing will be deleted.

## Proposed Hierarchy

```text
Canvas/Pembentangan
├── PreservedPembentanganContent        (inactive)
│   ├── PengenalanRedesignRoot
│   ├── Title
│   ├── Content
│   ├── Bottom
│   └── DialogParent...
└── PembentanganRedesignRoot            (new)
    ├── Background
    ├── DecorativeLayer
    ├── AudioNarrationButton
    ├── Header
    │   ├── ActivityBadge
    │   ├── Title
    │   ├── Subtitle
    │   └── MicroSquadLogo
    ├── TaskCard
    ├── PresentationArtwork
    ├── GroupActivityBadge
    ├── BottomNavigation
    │   ├── CiriButton
    │   ├── ContohButton
    │   └── KepentinganButton
    ├── TopicPanels
    │   ├── CiriPanel
    │   ├── ContohPanel
    │   └── KepentinganPanel
    └── HomeButton
```

## Visual Implementation

### Background and composition

- Pale-yellow center with lime lower-left and peach-orange right gradients.
- Subtle bubbles, atom/DNA line art, sparkles, and microorganism decorations.
- Central classroom presentation is the focal point; task card remains left and group badge right.
- A clean bottom strip holds the three category buttons.

### Header

- Rounded cream `AKTIVITI 8` badge at top center.
- Large green `Pembentangan` title with a thick cream/white edge and soft shadow.
- Dark-green subtitle centered beneath it.
- Compact Micro Squad mark at upper-right.

### Main artwork

- Generate a friendly Malaysian primary-school classroom scene with pupils presenting around a whiteboard.
- Keep the generated board text-free; overlay `MIKROORGANISMA`, `CIRI`, `CONTOH`, and `KEPENTINGAN` in Unity.
- Match the clean, colorful educational 3D/cartoon style used by the existing Aktiviti 8 redesigns.

### Cards and buttons

- Warm-white task card with green heading, dotted divider, clipboard badge, and centered instruction.
- Warm-white group badge with a transparent group icon.
- Green `Ciri` button with magnifier icon.
- Blue `Contoh` button with petri-dish icon.
- Orange `Kepentingan` button with shield/health icon.
- All icons use true alpha transparency with no square background.
- Reuse the project's animated button feedback where compatible.

## Assets

```text
Assets/Images/Akt8PembentanganUI/
├── Background/
├── Characters/
├── Icons/
└── QA/
```

Planned assets:

- One text-free 1920×1080 background plate.
- One transparent classroom-presentation illustration.
- Transparent clipboard, group, magnifier, petri-dish, shield, speaker, microscope, and mascot decorations as required.

Reuse suitable existing Aktiviti 8 assets, including the cleaned Pembentangan menu icon. New textures will use UI import settings, alpha transparency where applicable, and no mipmaps unless QA shows a need.

## Interaction Plan

### Audio

- Reuse `AudioNarrationButton` for play/pause, label updates, and stopping when the page closes.
- The visual control can be finished immediately.
- With no current clip, it remains safely non-playing until audio is supplied or separately approved for generation.
- Suggested narration: `Bentangkan hasil perbincangan tentang ciri, contoh dan kepentingan mikroorganisma kumpulan anda.`

### Category buttons

- The three bottom buttons switch between `CiriPanel`, `ContohPanel`, and `KepentinganPanel`.
- Use one small controller instead of multiple fragile persistent `SetActive` chains.
- If approved educational content is unavailable, use neutral prompts and do not invent scientific answers.

### Navigation

- Preserve Home: close `Pembentangan`, show `Center`.
- Preserve the legacy Next-to-Microsquad callback in the parked content; do not show a Next button because the reference has none.
- During implementation, wire the redesigned center-menu `PembentanganButton` to open the new page and close `Center`.
- Keep the center-menu `MicroThinker` button callback-free.
- Audit all listeners before and after the build so unrelated callbacks remain unchanged.

## Technical Implementation

Create a repeatable builder:

- `Assets/Editor/Akt8PembentanganUIBuilder.cs`

Optional runtime controller:

- `Assets/Scripts/Akt8/PembentanganSectionController.cs`

Builder workflow:

1. Verify the expected scene and hierarchy.
2. Create `Assets/Scenes/Aktivity/akt8/akt8_PrePembentanganRedesign_20260830.unity`.
3. Move existing children into `PreservedPembentanganContent` without deleting them.
4. Rebuild `PembentanganRedesignRoot` idempotently.
5. Construct the reference layout with uGUI and TextMeshPro.
6. Assign imported/generated assets with preserved aspect ratios.
7. Attach audio and category-switching behavior.
8. Preserve or deliberately rebind only the documented navigation callbacks.
9. Save, compile, and select the new root for inspection.

## Responsive Layout

- Keep the current Canvas Scaler with a 1920×1080 reference.
- Use normalized anchors for major regions.
- Protect the central illustration inside the 16:9 safe area.
- Scale artwork before shrinking text on narrower screens.
- Prevent the task card and group badge from overlapping header or bottom navigation.
- Keep button labels on one line at 1920×1080 and use conservative TMP auto-sizing below it.

## Phases and Codex Sol Effort

| Phase | Work | Effort |
|---|---|---|
| 1. Safety and hierarchy | Backup, callback audit, isolate shared content | Medium |
| 2. Asset production | Background, classroom art, mascots, transparent icons | High |
| 3. UI construction | Responsive uGUI/TMP hierarchy and visual matching | High |
| 4. Interaction | Audio, category switching, Home and menu routes | Medium–High |
| 5. QA and polish | Compile, Play Mode, screenshots, visual iterations | High |

Recommended Codex Sol setting for the full implementation: **High effort**. Use **Medium effort** only for later spacing, color, or text adjustments.

## Verification Checklist

- [ ] Scene backup exists before hierarchy changes.
- [ ] Existing Pengenalan and legacy objects remain recoverable.
- [ ] No missing scripts or compile errors.
- [ ] Generated artwork contains no accidental UI text.
- [ ] Icons have transparent backgrounds.
- [ ] All Malay wording matches the reference.
- [ ] Artwork remains sharp and proportional.
- [ ] All three bottom buttons fit without clipping.
- [ ] Home returns to the center menu.
- [ ] Pembentangan menu button opens this page.
- [ ] MicroThinker menu button still has no callback.
- [ ] Audio fails safely when no clip is assigned.
- [ ] Unrelated callbacks remain unchanged.
- [ ] Play Mode is compared at 1920×1080, 1366×768, and one taller aspect ratio.

## Out of Scope Unless Separately Approved

- Final Malay voice-over recording or generation.
- Inventing curriculum answers or assessment content.
- Scoring, timers, save data, or teacher reporting.
- Deleting preserved legacy content.
- Changing Micro Squad, Micro Thinker, or other activity pages.

## Completion Definition

The page closely matches the reference, all wording is native and editable, the three controls behave as approved, navigation is reliable, audio is ready for a clip, preserved content remains recoverable, and Play Mode visual/console checks pass.
