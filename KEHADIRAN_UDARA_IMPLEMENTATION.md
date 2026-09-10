# Kehadiran Udara — reference redesign plan

Status: Implemented after explicit approval for **AKT4 Kehadiran Udara**, 2026-09-07.

## Confirm the lesson before implementation

The user requested **Kehadiran Udara**, but the attached reference is titled **AKTIVITI 5 — SUHU YANG SESUAI** and teaches refrigeration/temperature using milk.

Default interpretation: apply its visual style to Kehadiran Udara, retaining that section's air-presence content. Do not replace an air lesson with a temperature lesson without confirmation.

The inspected builder, Assets/Editor/Akt5KehadiranUdaraUIBuilder.cs, targets Assets/Scenes/Aktivity/akt5/akt5.unity and Canvas/Kehadiran Udara. An identically named section also exists in AKT4 from the previous audit. Confirm the intended activity before changing either scene. The builder's existence does not prove its current live scene state.

## Proposed appearance

- 1920 × 1080 landscape design, proportionally fitted inside this page only.
- Lime-to-cream-to-peach background; restrained bubbles, stars, leaves and friendly microbes.
- Small activity badge above a broad glossy dark-green title banner, white rounded title text and book medallion.
- Left: student character beside a blue-bordered cream question bubble.
- Centre: large lesson illustration, with a blue caption tab. For Kehadiran Udara, retain the existing vacuum/non-vacuum food comparison rather than showing a refrigerator.
- Right: explaining character beside a green-bordered cream answer bubble and tick badge.
- Bottom: large cream reflection/inquiry panel, orange dashed border, lightbulb badge, divider and experiment prompt; magnifier decoration at the right.
- Top-left: visible arrow + KEMBALI button returning to the existing Faktor page.

Use editable TMP text and separate UI panels. Do not use the complete reference poster as a background. Keep body text approximately 30–34 design pixels and title approximately 74–86; verify actual wrapping rather than allowing ellipsis.

## Content handling

After target confirmation, inspect the selected page's serialized text and callbacks once. Preserve its current lesson sequence and scientific content. Highlight key words in blue, green, purple and orange to match the reference.

If the intended target is actually Suhu Yang Sesuai, revise this plan for its existing section and use the milk/refrigerator discussion from the reference. Do not implement that alternative inside Kehadiran Udara.

## Artwork

Reuse suitable existing book, question, tick, lightbulb, magnifier, water-drop and microbe assets. Inspect current food and character artwork together before deciding what needs replacement.

Generate missing polished cartoon illustrations only after approval. Keep lesson copy out of artwork. Verify real alpha transparency; do not accept a painted checkerboard as transparency. If cutouts are unavailable, disclose any cream-backed portrait treatment before claiming a close match.

Keep original image files. Use versioned new assets under the selected activity's KehadiranUdaraUI directory. Disable power-of-two rescaling for non-square atlases, use normalized crop coordinates, preserve aspect ratios, and verify actual imported dimensions.

## Safe implementation

1. Use the Unity MCP skill and read editor readiness. Confirm target scene, page, assets, existing Back/Faktor links and initial visible states.
2. Preserve unsaved scenes before switching. Save a unique dated copy of the target scene including unsaved changes; abort if backup fails.
3. Update the existing builder for the confirmed activity instead of creating a competing builder.
4. Remove its shared-Canvas reconfiguration and unconditional refresh behavior from this workflow. Keep layout scaling local to the page.
5. Build the full replacement under a temporary generated root in one operation. Validate required assets and callbacks before retiring the previous root.
6. Retain legacy controls/dialog data; deactivate only specifically identified obsolete visuals. Do not blanket-disable unknown child objects.
7. Preserve Back navigation and page re-entry. Keep existing interaction behavior; do not introduce automatic dialog playback or new lesson stages.
8. Save only the target scene after successful checks. Re-running the builder must leave exactly one generated root and preserve backups.

## Efficient MCP execution after approval

- One compact initial audit; batch independent inspections with batch_execute.
- Group source edits and asset imports, compile once, then execute one bulk builder action.
- Wait for layout to settle after changing Game View size before inspecting or capturing.
- Batch final hierarchy, missing-reference, text-fit and navigation checks.
- Capture actual overlay UI in Play Mode at 1920 × 1080 and 16:10.
- Check compilation errors once and runtime console once; distinguish existing media issues from new failures.
- Restore Edit Mode, original scene/visibility and Game View settings.

## Acceptance

- Confirmed activity and lesson remain correct.
- Header, two speech panels, central lesson illustration and inquiry footer follow the reference composition.
- Text stays editable, legible and fully visible; artwork is not distorted or accidentally cropped.
- Back is unobstructed and returns to Faktor; re-entry works.
- No duplicate titles, generated roots, broken references or new UI errors.
- Other activities and shared Canvas settings are unchanged.
- Deliver screenshots and update this document with actual checks and any visual differences.

## Confirmed implementation

The user selected AKT4 Kehadiran Udara. Only Assets/Scenes/Aktivity/akt4/akt4.unity was rebuilt. AKT5 and Suhu Yang Sesuai were not changed.

- Added Assets/Editor/Akt4KehadiranUdaraUIBuilder.cs because no AKT4-specific Udara builder existed. The AKT5 builder was left untouched.
- Added Assets/Scripts/Akt4UdaraPresentation.cs. Two readable spreads retain the lesson order: asparagus question/hypothesis, then apple experiment/reflection. TERUSKAN advances; SEBELUM returns; ULANG restarts. Re-entry resets to the discussion.
- Preserved all four legacy DialogController objects and their serialized content/callbacks, inactive. The new presentation replaces the typewriter/close-button display with editable speech cards, without altering original dialogue data.
- Retained the pupil's hypothesis and explicitly labeled it as a hypothesis to test, not an experimentally confirmed result. Corrected spelling/repeated words in display copy.
- Back relays directly to the retained AKT4 Back button and returns to Faktor.
- Reused existing portrait, icon, rounded-panel and gradient assets read-only, plus the page-local fitting component. Shared Canvas settings and the completed Kehadiran Air builder were not changed.
- Saved unique dated akt4_before_udara scene-copy backups before rebuilding, including the user's initial unsaved scene state.
- Repeated build verification left one generated root.

## Actual verification

- Compilation: no script errors.
- Both spreads: no overflowing text at 1920x1080 or 1164x728 (16:10).
- Next, Previous, Repeat, Back, Faktor re-entry/reset: passed.
- Back raycast: BackButton is the first hit.
- Generated subtree: no missing scripts or missing sprite/font references.
- Visually inspected both Full HD spreads and the narrower letterboxed layout.
- Used actual ScreenCapture previews for final checks; inline camera previews had different color rendering. Temporarily enabled background rendering to avoid stale frames, then restored it.
- Restored Edit Mode and the original Game View size selection. Original page visibility is retained.
- Existing WindowsMediaFoundation color-primaries errors for three existing video files remain; baseline also contained audio buffer overflow reports. No new UI-related errors were observed.

### Final screenshots

- Assets/Screenshots/Akt4KehadiranUdara/udara_discussion_verified.png
- Assets/Screenshots/Akt4KehadiranUdara/udara_investigation_verified.png
- Assets/Screenshots/Akt4KehadiranUdara/udara_narrow_verified.png

### Artwork provenance and differences

Built-in ImageGen produced Assets/Images/Akt4KehadiranUdaraUI/udara_food_atlas_v1.png. No API/CLI fallback was used. Three sprite regions reference its imported dimensions without power-of-two distortion. The artwork uses a cream backdrop deliberately; existing cream-framed student portraits and simplified decorative icons make this reference-inspired, not pixel-identical.

Final generation prompt:

> Use case: scientific-educational. Create ONE game illustration atlas, exactly 1536x1024 landscape, divided invisibly into THREE equal vertical cells of 512x1024. Solid uniform warm ivory #FFFCF3 background across entire image, not transparent. Each cell contains a single centered illustration with 40px blank margin all around, objects do not cross cell boundaries. LEFT CELL: green fresh asparagus spears tightly vacuum sealed in clear plastic food packaging, visible crinkled glossy shrink-wrapped plastic with sealed edges, upright bunch, fills central 430x760 region. MIDDLE CELL: several fresh pale apple slices with red peel tightly vacuum sealed in clear flattened food bag, visible crinkled plastic and sealed edges, arranged upright, fills central 430x640 region. RIGHT CELL: identical fresh pale apple slices with red peel on a small shallow cream ceramic plate, uncovered and exposed to air, three-quarter overhead view, fills central 430x430 region. All food fresh, no mold, no spoilage, no insects, no implied experiment result. Polished friendly 3D cartoon primary school science textbook illustration, crisp clean shapes, rounded highlights, soft self shadows only. Absolutely NO words, letters, numbers, labels, grid lines, icons, decorative elements, hands, characters, or background scenes. Keep subject identity and comparison obvious.
