# AKT4 Kehadiran Air — Implementation Plan

Status: Implemented and verified after approval.  
Date: 2026-09-07  
Scene: `Assets/Scenes/Aktivity/akt4/akt4.unity`  
Scope: `Canvas/Kehadiran Air`  
Reference: supplied Kehadiran Air illustration.

## Outcome

Match the reference's green science header, three numbered speech cards, biscuit jar, side-by-side dry/wet bread comparison, and purple inquiry footer. Include a clearly visible back button returning to Faktor.

## Verified current state

- The active scene is AKT4 in Edit Mode.
- `Assets/Editor/Akt4KehadiranAirUIBuilder.cs` already exists. It currently uses `air_mockup_bg.png` as a large background and creates `Back_Redesign`.
- `Back_Redesign` already relays to the retained `Back` button. Its callbacks hide Kehadiran Air and show Faktor.
- `Title_Redesign` also exists but is not cleaned up by the current builder; inspect and explicitly retire its duplicate visuals during the update.
- Four retained dialog panels contain the question, moisture explanation, investigation instructions, and inquiry task from the reference. They are currently inactive.
- Existing lesson illustrations include `Assets/Images/2d/biskutkering.png` and `Assets/Images/2d/rotiA-rotiB.png`.
- Existing student sprites are `Assets/Images/2d/student1.png` and `student2.png`.

## Layout specification

Use a 1920 × 1080 design area, fitted proportionally within the existing Canvas. Keep the shared Canvas settings unchanged to protect other AKT4 pages.

| Region | Planned presentation |
|---|---|
| Header | Wide lime-green rounded banner; dark-green title with white outline; water-drop mascot and magnifying glass accents |
| Subtitle | Cream strip: “Air mempengaruhi pertumbuhan mikroorganisma.” |
| Left column | Three cream speech cards with blue, pink, and green borders; numbered circular badges; characters aligned beside their corresponding cards |
| Upper right | Biscuit jar illustration, green and purple microbes, decorative bubbles |
| Lower right | Equal-sized Roti A and Roti B cards, orange/blue label tabs, plates of bread, vertical dashed separator |
| Footer | Purple clipboard badge, “Tugasan Inkuiri” label, and readable task text |
| Back | Top-left reserved area: left arrow + “KEMBALI”, approximately 220 × 72 design pixels, minimum 24 px edge clearance |

Reserve space for Back before positioning decorations. Keep the title and footer inside the safe design area. At narrower aspect ratios, fit the content without stretching the illustrations or cropping the back button.

Approximate text sizes: title 78–90; speech/body 30–34; bread captions 30–34; inquiry text 28–32. Body text uses a readable rounded font already installed in the project; retain Fredoka One for headings where appropriate. Adjust wrapping through actual rendered previews.

## Display copy

1. **Blue card:** “Mengapakah roti yang telah dikeringkan ini tahan lebih lama daripada roti biasa? Adakah ketiadaan air menyebabkan mikroorganisma tidak tumbuh?”
2. **Pink card:** “Saya juga berfikir begitu. Roti biasa mempunyai kelembapan yang menyebabkan roti tersebut mudah ditumbuhi kulat.”
3. **Green card:** “Mari kita lakukan penyiasatan dengan menggunakan roti kering. Kita gunakan dua keping roti kering yang dilabelkan Roti A dan Roti B, tetapi hanya Roti B telah dicelup ke dalam air.”

- Roti A: **tanpa air** — orange emphasis.
- Roti B: **dicelup ke dalam air** — blue emphasis.
- Inquiry: “Berdasarkan bahan yang dicadangkan, rancang dan jalankan eksperimen untuk menyiasat sama ada kehadiran air mempengaruhi pertumbuhan mikroorganisma.”

Use blue emphasis for air, pink for kelembapan, purple for kulat/mikroorganisma, and orange/blue for Roti A/B. Retain the original serialized dialog data; the display copy corrects spelling and capitalization only.

## Artwork and rendering

- Inspect the existing background, biscuit, bread, and character images together before implementation. Their paths are verified; their visual suitability is not yet verified.
- Reuse suitable water-drop, microbe, magnifier, clipboard, and arrow sprites from the project.
- Use separate editable text, cards, and labels. Do not overlay duplicate text on a background that already contains the reference's captions.
- If `air_mockup_bg.png` contains a complete poster, preserve it and use a clean background for the new layout.
- Reuse the combined bread image only if its dry/wet distinction and labels stay legible. Create separate artwork variants if the existing illustration cannot represent the reference accurately.
- Reuse artwork where it matches; prepare missing character or bread artwork using ImageGen after approval if needed. Generated art should omit lesson text, which will remain editable in Unity. Pixel-identical illustration matching is not assumed.
- Use clean rounded panel sprites with white edges, thin colored outlines, and soft shadows. Avoid inheriting the main menu's heavy grey panel borders.
- Keep decorative graphics non-interactive and retain sprite aspect ratios. Group small decorations to minimize hierarchy and rendering cost.

## Implementation approach

Update the existing `Akt4KehadiranAirUIBuilder.cs` rather than adding a competing builder.

1. Confirm the current scene and record the panel's active states and back callback targets. If the scene has unsaved changes, preserve them in the backup before rebuilding.
2. Create a unique dated backup of the current scene, including the latest main-menu and other user changes. Abort the rebuild if the backup fails.
3. Prepare any required dedicated assets under `Assets/Images/Akt4KehadiranAirUI/`; preserve existing artwork files.
4. Build the complete page under `KehadiranAirReferenceRoot`: Header, SpeechCards, BiscuitIllustration, BreadComparison, InquiryFooter, Decorations, BackButton.
5. Validate the new root before hiding the known previous redesign visuals. Retain original dialog objects and source controls with their serialized callbacks.
6. Reuse `CenterMenuButtonRelay` for Back, pointing directly to legacy `Back`, never to another generated relay. Display exactly one back button.
7. Preserve the current static-page behavior. Do not introduce automatic dialog playback, extra lesson stages, or unrelated navigation changes.
8. Save only the AKT4 scene after successful checks. A second builder run must update its own generated root without duplicating controls or overwriting the backup.

## Efficient Unity MCP workflow

- Reuse this audit; perform one fresh readiness check and a compact target/asset check after approval.
- Use direct file patches for the builder and Unity MCP for execution and scene inspection.
- Batch independent discovery and verification with `batch_execute` (current limit: 25 commands). Unity may serialize the batch internally.
- Compile once after grouped script changes; check compilation errors before invoking the builder. Refresh only if import has not occurred.
- Build the entire page through one editor menu action.
- Verify the completed hierarchy once, then inspect a Game View capture including overlay UI.
- Test back navigation and re-entry in Play Mode; restore the original editor state afterward.
- Compare 1920 × 1080 and one narrower aspect ratio. Group corrections before rebuilding and recapture only affected checks.
- Check console errors once after compilation and once after runtime verification. Investigate any new failures without clearing unrelated logs.

## Acceptance checks

- Three numbered cards, biscuit jar, correct dry/wet bread comparison, subtitle, and inquiry footer are present.
- All Malay text is readable, correctly wrapped, and matches the approved copy.
- Back is visible, unobstructed, and returns to Faktor; opening Kehadiran Air again works.
- No duplicate old title/background/Back visuals or click-blocking decoration remain visible.
- Existing dialog data, unrelated lesson content, and other AKT4 sections remain intact.
- No missing scripts, broken references, or new console errors are introduced.
- Deliver a final screenshot and update this document with actual verification results and any remaining differences from the reference.

## Implementation results — 2026-09-07

- Rebuilt the page using Unity MCP and the existing editor builder. Verification used batch_execute.
- Added editable Malay text, numbered cards, header, separate bread illustrations, inquiry footer, and a top-left KEMBALI button relaying to the retained Back control.
- Preserved original dialogs and old visuals (inactive), other pages, and shared Canvas settings. Dated scene-copy backups were saved before each build.
- Added KehadiranAirLayout for proportional page-only fitting. Rebuilding leaves one generated root.
- Compilation succeeded. Runtime checks passed for Back to Faktor, re-entry via its existing Button (2), Back hit testing, zero overflowing text, and zero missing images.
- Visually inspected Full HD 1920x1080 and narrower 16:10. The narrower page is letterboxed without cropping.
- Final screenshots: Assets/Screenshots/Akt4KehadiranAir/kehadiran_air_final_1920-1.png and kehadiran_air_verified_16x10.png.
- Unity returned to Edit Mode and its original Game View size selection.
- Runtime console contains WindowsMediaFoundation color-primaries errors for existing video files and intermittent audio buffer overflow reports. These were not fixed; the console is not error-free.

### Artwork and reference differences

The built-in image-generation tool created Assets/Images/Akt4KehadiranAirUI/air_characters_bread_v2.png. Prompts requested an isolated 3-by-2 atlas of Malaysian schoolboy, hijab-wearing schoolgirl, male teacher, red-lidded biscuit jar, dry bread and wet bread on plates, with no lesson text. A transparency correction still produced opaque artwork, so the final edit requested a uniform warm-cream backdrop while preserving subjects and positions.

The final UI therefore uses cream-framed character portraits and cream-backed illustration panels, not exact transparent reference cutouts. Decorations and header styling are simplified; this is a reference-inspired editable implementation, not a pixel-identical poster. Existing icon sprites were reused.
