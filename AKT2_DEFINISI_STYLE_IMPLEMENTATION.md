# AKT2 — Pengenalan / Definisi redesign plan

Status: IMPLEMENTED AND VERIFIED — 7 September 2026, after approval.  
Scene: `Assets/Scenes/Aktivity/akt2/akt2.unity`  
Target: `Canvas/Definisi`  
Reference: supplied DEFINISI illustration.

## 1. Target and scope

Unity MCP inspection confirms AKT2 is currently open, with Definisi visible and PengenalanAlga hidden. This plan interprets “current pengenalan section” as the visible microorganism-definition page. It does not modify PengenalanAlga or redirect the menu to a different section.

The scene currently has unsaved changes. Leave them untouched during planning. After approval, create a dated scene-copy backup that includes the current in-memory state before applying scoped changes.

Match the reference's design while keeping the educational context: a simple Malay introduction to what microorganisms are and how they are observed. Do not substitute algae-specific content or the earlier yeast/growth-factor lesson.

## 2. Proposed page content

Heading: **DEFINISI**

Three editable text cards:

1. **Mikroorganisma ialah hidupan seni yang sangat kecil.**
2. **Kebanyakannya tidak dapat dilihat dengan mata kasar.**
3. **Mikroorganisma boleh diperhatikan menggunakan mikroskop.**

This retains the current definition's core ideas and separates them into readable statements. “Kebanyakannya” avoids an absolute claim that every microorganism is invisible to the naked eye. Cartoon microbes are illustrative, not literal depictions of size or appearance.

Keep the section identity and existing destinations unchanged. Do not add quiz questions, growth-factor claims, scoring or experiments.

## 3. Visual layout

Adapt the reference composition to the existing Canvas rather than stretching its approximately 4:3 image to fill a wide screen.

- Soft cream/yellow background blending to pale green and peach.
- Centred cream pill title with dark-green rounded lettering, white edging and a soft shadow.
- Left side: three vertically stacked rounded cards, tinted light green, warm yellow and pale blue.
- Each card: a small framed icon on the left and large dark-green Malay text on the right.
- Icons: friendly microorganism, eye/magnifier, microscope.
- Right side: large microscope illustration and circular magnified view of varied microbes.
- Restrained foliage/bubbles at the edges; reserve clear space for controls.
- Bottom navigation: orange Home at the left and green Next at the right, using the real existing buttons.

For a 1920×1080 layout, start with approximately 50–55% width for text and 40–45% for art, with a protected header/footer. Confirm the actual Canvas settings before implementation. Use generous text margins and approximately 34–42 design-pixel body text; reduce artwork before shrinking text.

Do not flatten the page into a screenshot: text and buttons stay real editable Unity UI.

## 4. Adapt reference controls to actual functions

| Reference detail | Existing behaviour | Planned treatment |
|---|---|---|
| “Dengar audio” | Top/DialogButton opens DialogParent; no AudioSource found under Definisi | Style as a matching green information control labelled **Baca penerangan**, with a book/info icon. Do not pretend it plays audio. |
| Home icon | Hides Definisi and shows Center | Restyle existing HomeButton; preserve both callbacks. |
| Right arrow | Hides Definisi and shows Jenis | Restyle existing bottom NextButton; preserve destination. |
| Left/back arrow | No separate Back control found under Definisi | Omit the extra arrow by default; do not invent another destination or duplicate Home. |
| Definition cards | Current text lives in a typewriter dialogue | Show the three summary cards on the main page; keep the dialogue as an accessible explanation overlay. |

Actual narration or a new Back route would be an additional feature, requiring a separately agreed audio source/destination. No audio generation is included in this plan.

## 5. Preserve and align the dialogue

Verified controller: `Assets/Scripts/DialogController.cs`.

Current dialogue has two entries covering the definition and microscope observation, with typingSpeed 0.05 and PressToStart false.

Planned content-only update to this instance's two entries:

1. “Mikroorganisma ialah hidupan seni yang sangat kecil. Kebanyakannya tidak dapat dilihat dengan mata kasar.”
2. “Mikroorganisma boleh diperhatikan menggunakan mikroskop.”

This aligns the explanation with the three visible cards and corrects the current “diperhatian” typo without changing the controller's sequence.

Preserve:
- Existing DialogParent, textDialog, internal NextButton and ExitButton objects.
- The internal NextButton as textDialog's first child: Awake explicitly resolves GetChild(0).
- Runtime listener registration, typewriter timing, next/exit visibility and current reopening behaviour.
- Existing DialogButton open and ExitButton close callbacks.
- The difference between the dialogue's internal Next button and the page-level Next-to-Jenis button.

Restyle the dialogue as a readable cream overlay above the cards, without letting its decorations block controls. Do not edit the shared DialogController script or other panels' dialogue lists.

Inspector JSON showed zero reference IDs, but the controller assigns references in Awake. Do not treat that serialization display alone as evidence of missing runtime bindings.

## 6. Asset strategy

Reuse suitable existing project assets first:
- Rounded card shapes and warm gradient used in previous UI.
- Existing microscope, microorganism and eye/magnifier illustrations.
- Existing fonts and navigation icons, without modifying their shared materials.

Inspect available microscope artwork for adequate resolution and style. If the reference-like microscope/magnified-view composition cannot be assembled cleanly, generate only the missing decorative illustration after approval using the imagegen skill. No baked text/buttons; keep generated art isolated to this section.

Prefer a clearly educational magnified view; do not imply that an ordinary microscope reveals every type of microbe in the same detail.

## 7. Efficient implementation after approval

1. Recheck only changed state, relevant references, Canvas dimensions and the current target.
2. Save a unique backup of the unsaved scene state; abort if the backup fails.
3. Use a scene/path-guarded editor builder for Canvas/Definisi.
4. Create/reposition cards and decorative art in one bulk Unity MCP operation. Reuse existing functional objects and named generated roots for safe reruns.
5. Apply only the approved definition text changes to this section's content.
6. Use batch_execute for independent inspections/checks; compile/import only when necessary.
7. Verify the finished page and dialogue, then fix only failed checks.
8. Save the scoped scene changes, preserving all unrelated in-memory work; restore the original editor mode/settings.
9. Update this document with screenshots, actual results and any remaining limitations.

Do not change AKT2 menu routing, PengenalanAlga, Jenis content, other activities, quiz logic, project settings or shared assets.

## 8. Acceptance checks

- Reference-style title, three coloured definition cards, microscope and circular microbe illustration.
- Exact approved Malay copy, no truncated lines or stretched art.
- Layout reviewed at 1920×1080, 16:10 and 4:3/reference-like proportions; adapt without hiding text or controls.
- Home returns to Center; page Next opens Jenis; existing menu entry still opens Definisi.
- Baca penerangan opens the real dialogue; both entries type correctly; internal Next and Exit work.
- Reopening the dialogue matches the original controller behaviour unless a separate fix is approved.
- No fake audio button, invented Back route, duplicated callbacks or decoration blocking pointer input.
- No missing scripts/references or new section-related console errors.
- Other scene sections and unsaved user changes remain intact.

## Planning status

The approved redesign is implemented and saved. The sections above record the plan; actual results follow.

## Implementation results

- Three editable Malay definition cards, reference-style title, microscope and circular microorganism artwork.
- Existing project illustrations reused; no new artwork generation or shared-asset changes.
- Baca penerangan opens the original typewriter dialogue, restyled as a cream overlay.
- Both dialogue entries updated to the approved definition wording.
- Original page Home/Next and dialogue Next/Exit controls retained, including the required first-child lookup.
- Existing dialogue visibility on scene startup preserved; it was already open in the supplied state.
- Editor builder: `Assets/Editor/Akt2DefinitionStyleBuilder.cs`.
- Presentation-only fitting: `Assets/Scripts/Akt2DefinitionFit.cs`. Fits only Definisi; the shared CanvasScaler remains unchanged.
- Baseline backup, including unsaved starting work: `Assets/Scenes/Aktivity/akt2/akt2_before_definition_20260907_134042.unity`. Dated pre-apply copies also remain.

## Verification

- Main page text, button bounds and button hit tests passed at 1920x1080, 1164x728 (16:10) and 1280x960 (4:3).
- Screenshots reviewed at all three sizes; no clipped definition text or stretched illustrations.
- Both dialogue entries completed typing; internal Next advanced to the second entry, and Close hid the overlay.
- Reopening retained index 1 and retyped the last entry, matching the original controller behaviour.
- The dialogue backdrop was extended to cover the full viewport at narrower ratios.
- Home hides Definisi and activates Center. Page Next hides Definisi and activates Jenis.
- Original definition menu callback works when its existing menu container is enabled in a runtime-only isolation check.
- Compared the saved scene to the in-memory-state backup, including prefab overrides: all 1,349 serialized documents outside Definisi are unchanged.
- No shared DialogController or menu-routing source edits.
- No compilation errors. Existing media playback logged WindowsMediaFoundation colour-primaries errors and an AudioSampleProvider buffer-overflow message during testing; media files were not changed.
- Original Game View/background-rendering settings restored and temporary 4:3 view removed. Returned to AKT2 Edit Mode.

## Existing menu visibility limitation

The baseline backup confirms both `Canvas/Center/GameObject` and `Canvas/Center/Akt2GeneratedReference` were disabled. Home activates Center but does not enable those children, so its menu can remain hidden. This is unchanged starting-state behaviour, not a redesign regression. The route itself was verified by temporarily enabling the generated menu during Play Mode only. Those temporary test changes were not saved.

## Previews

- [Full HD page](D:/UProject/Microfun/MicroFun/Captures/Akt2Definition/Definition_1920.png)
- [16:10 page](D:/UProject/Microfun/MicroFun/Captures/Akt2Definition/Definition_16x10.png)
- [4:3 page](D:/UProject/Microfun/MicroFun/Captures/Akt2Definition/Definition_4x3.png)
- [Explanation overlay](D:/UProject/Microfun/MicroFun/Captures/Akt2Definition/Definition_dialog_1920.png)
- [4:3 overlay](D:/UProject/Microfun/MicroFun/Captures/Akt2Definition/Definition_dialog_4x3_final.png)

The unity-mcp-orchestrator skill guided the backup, scoped bulk builder, asset reuse and before/after verification.
