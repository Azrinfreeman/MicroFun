# AKT4 Pengenalan — Implementation Plan

Status: **Plan only — not implemented**  
Target scene: `Assets/Scenes/Aktivity/akt4/akt4.unity`  
Target panel: `Canvas/Pengenalan`  
Reference: supplied “AKTIVITI 4 — PENGENALAN” image, 16:9 layout

## 1. Objective

Redesign only the AKT4 Pengenalan section to closely match the supplied educational-poster reference while preserving the existing Pengenalan flow, dialog data, navigation, and other AKT4 sections.

The resulting page will present:

- A small `AKTIVITI 4` badge above a large green `PENGENALAN` banner.
- A cream/pink question card on the left.
- A pale-blue answer card with a `JAWAPAN` header on the right.
- The existing Malay question and answer text.
- Child-friendly science characters and microorganism decorations.
- A bottom `Idea utama` summary card.
- Existing Home/dialog actions in visually secondary controls.

## 2. Current Scene Findings

- Unity `6000.4.5f1`; the active scene is `akt4.unity`.
- The project uses uGUI, TextMeshPro, and a `1920 × 1080` Canvas reference resolution.
- Existing panel hierarchy:
  - `Canvas/Pengenalan/Top/DialogButton`
  - `Canvas/Pengenalan/Title`
  - `Canvas/Pengenalan/Content/Center`
  - `Canvas/Pengenalan/Content/Center/Button (2)`
  - `Canvas/Pengenalan/Content/Center/Button (3)`
  - `Canvas/Pengenalan/Content/Center/Button (4)`
  - `Canvas/Pengenalan/Bottom/NextButton`
  - `Canvas/Pengenalan/Bottom/HomeButton`
  - `Canvas/Pengenalan/DialogParent`
- Existing serialized text already matches the requested lesson:
  - Question: `Mengapa yis dalam kotak ini tidak bertumbuh?`
  - Answer: `Yis dalam kotak ini tidak bertumbuh kerana masih tidak aktif. Mikroorganisma akan bertumbuh jika semua keperluan untuk pertumbuhannya mencukupi.`
- `DialogParent` uses the existing `DialogController`; its content and typing behavior will not be rewritten.
- `Button (3)` and `Button (4)` are staged/inactive legacy content (`BERGERAK`, `BERTUMBUH`) and will be retained rather than deleted.

## 3. Reused Assets

The first pass will reuse project artwork and construct the remaining shapes with uGUI/TMP. No AI image generation is required.

| Purpose | Asset |
|---|---|
| Background gradient | `Assets/Images/Akt4CenterUI/Background/akt4_center_background.png` |
| Rounded cards and banners | `Assets/Images/UI Flat/btn_white_flat.png` |
| Header book | `Assets/Images/UI Flat/icon_book_open.png` |
| Answer lightbulb | `Assets/Images/UI Flat/icon_lightbulb.png` |
| Educator character | `Assets/Images/scientist.png` |
| Student candidate | `Assets/Images/2d/student1.png` |
| Student fallback | `Assets/Images/2d/milk_boy.png` |
| Bread/package illustration | `Assets/Images/UI Flat/img_fun_bread.png` |
| Yeast/sugar visual | `Assets/Images/Akt5KehadiranNutrienUI/yeast_sugar_v1.png` |
| Purple microbe | `Assets/Images/UI Flat/char_purple_microbe.png` |
| Green microbe | `Assets/Images/UI Flat/char_green_microbe.png` |
| Water-drop character | `Assets/Images/UI Flat/char_water_drop.png` |
| Flask decoration | `Assets/Images/UI Flat/icon_flask.png` |
| Home icon | `Assets/Images/UI Flat/icon_home.png` |
| Rounded decorative circle | `Assets/Images/UI Flat/ui_circle_white.png` |
| Font | `Assets/Fonts/FredokaOne SDF.asset` |

The student sprite will be selected by best visual crop during the first preview; this does not change layout or functionality.

## 4. Planned Generated Hierarchy

Create one idempotent generated root inside `Canvas/Pengenalan`:

```text
Akt4PengenalanRedesignRoot
├── Background
├── Decorations
│   ├── Bubble_*
│   ├── Star_*
│   ├── PurpleMicrobe
│   ├── WaterDrop
│   └── Flask
├── Header
│   ├── ActivityBadge
│   │   └── Text
│   └── PengenalanBanner
│       ├── BookMedallion
│       │   └── BookIcon
│       └── TitleText
├── QuestionCard
│   ├── QuestionBadge
│   │   └── QuestionMark
│   ├── QuestionText
│   ├── Student
│   └── YeastPackage
│       ├── PackageLabel
│       ├── BreadIcon
│       └── YeastGranules
├── AnswerCard
│   ├── AnswerHeader
│   │   ├── LightbulbIcon
│   │   └── HeaderText
│   ├── AnswerOne
│   ├── Divider
│   ├── AnswerTwo
│   └── Educator
├── MainIdeaCard
│   ├── TargetBadge
│   ├── MainIdeaLabel
│   └── MainIdeaText
├── DialogReplayButton
└── HomeButton
```

The old `Title`, `Content`, `Top`, and `Bottom` visuals will remain serialized as hidden source controls. `DialogParent` remains available for the existing dialog flow.

## 5. Visual Specifications

Base layout uses the existing `1920 × 1080` reference resolution.

### Header

- Activity badge: centered near the top, approximately `330 × 72`, cream fill with green dashed-style edge treatment.
- Main banner: approximately `850 × 120`, green body, white rim, soft shadow.
- Book medallion overlaps the left edge of the banner.
- `PENGENALAN`: white Fredoka One, approximately `72–80 pt`, dark-green outline/shadow.

### Question card

- Position: left half below the header.
- Size: approximately `830 × 590`.
- Cream fill with pink outer border and a secondary dashed-looking inner edge.
- Circular pink question badge overlaps the top edge.
- Question text: centered, dark brown, approximately `42–48 pt`.
- Student character sits at lower left.
- Yellow yeast-package composite sits at lower center/right with `YIS` as the package label.
- The whole card can act as the generated relay for legacy `Button (2)` if its callback is required.

### Answer card

- Position: right half below the header.
- Size: approximately `850 × 590`.
- Very light-blue fill with blue outer and inner edge treatments.
- `JAWAPAN` tab overlaps the top edge and includes the lightbulb icon.
- Two check-mark bullets with a dashed divider.
- Rich-text emphasis:
  - `tidak aktif` in pink/red.
  - `bertumbuh` in green.
  - `keperluan` in purple.
- Educator character occupies the lower-right area without covering text.

### Main idea card

- Bottom-center card approximately `1120 × 115`.
- Cream fill, orange edge, soft shadow.
- Text: `Idea utama: Mikroorganisma memerlukan keadaan yang sesuai untuk bertumbuh.`
- Green `Idea utama:` label and dark-blue body copy.

### Decorative treatment

- Lime-to-cream-to-peach background shared with the new AKT4 menu.
- Low-opacity bubbles and colorful star/plus accents around safe margins.
- Purple microbe near the top right, water drop near the lower right, and flask near the lower left.
- All decoration images use `raycastTarget = false`.

## 6. Functionality Preservation

- Keep every existing Pengenalan object and serialized callback.
- Generated Home control relays to `Canvas/Pengenalan/Bottom/HomeButton` through `CenterMenuButtonRelay`.
- Generated dialog/replay control relays to `Canvas/Pengenalan/Top/DialogButton`.
- Retain `DialogParent`, `DialogController.dialogList`, typewriter speed, Next dialog button, and Exit dialog button unchanged.
- Retain inactive staged buttons `Button (3)` and `Button (4)` for the existing lesson sequence.
- Do not alter `SceneController`, video players, destination panels, Micro-Scene, Faktor, or Kuiz.
- If a generated question-card click is enabled, it relays only to existing `Button (2)`; no callback data is duplicated.
- The static answer remains visible as in the reference, while the dialog replay remains available as an optional secondary interaction.

## 7. Implementation Method

Use the project’s established editor-builder convention:

1. Create `Assets/Editor/Akt4PengenalanUIBuilder.cs` through Unity MCP.
2. The builder will:
   - Verify the active scene, target hierarchy, and required sprites/font.
   - Create a one-time scene backup at `Assets/Scenes/Aktivity/akt4/Backups/akt4_PrePengenalanRedesign_20260904.unity`.
   - Remove only an older `Akt4PengenalanRedesignRoot` so reruns are safe.
   - Build the complete Pengenalan hierarchy in one editor operation.
   - Read the existing question and dialog-answer strings where practical rather than duplicating lesson data.
   - Attach relay components for Home, dialog replay, and the question card when applicable.
   - Hide only the legacy Pengenalan visual groups after the new root is complete.
   - Preserve `DialogParent` and save the scene.
3. Execute the builder once after compilation succeeds.

This approach is faster and safer than many small GameObject edits and keeps the design reproducible.

## 8. Optimized Unity MCP Sequence

When implementation is approved:

1. Read `mcpforunity://editor/state` once.
2. Batch-check the target objects and required assets.
3. Create the editor builder in one script operation.
4. Wait for compilation and read only error logs.
5. Execute one menu action to build the page.
6. Use one `batch_execute` verification pass for:
   - Generated-root lookup.
   - Required child/button lookup.
   - Relay-source checks.
   - Scene validation.
7. Enter Play Mode once and capture one `960–1280 px` preview.
8. Test Home/dialog/question interactions without persisting runtime state.
9. If visual correction is needed, apply grouped anchored text edits and rebuild once.
10. Capture one final screenshot, exit Play Mode, and perform one final error check.

Efficiency constraints:

- No repeated full hierarchy reads.
- No unnecessary AssetDatabase refresh after script creation.
- `batch_execute` for independent searches and checks.
- One grouped visual correction pass at most.
- Screenshots limited to the resolution required for comparison.

## 9. Verification Checklist

- [ ] Pengenalan page closely matches the supplied two-card reference.
- [ ] Header displays `AKTIVITI 4` and `PENGENALAN` correctly.
- [ ] Question text is correct and fully readable.
- [ ] Both answer statements are correct and fully readable.
- [ ] Colored emphasis matches the reference intent.
- [ ] Main-idea sentence is correct.
- [ ] Characters and decorations keep their aspect ratios.
- [ ] Educator/student artwork does not obstruct text.
- [ ] Home retains its existing action.
- [ ] Dialog replay opens the existing dialog flow.
- [ ] Existing staged Pengenalan buttons and dialog data remain serialized.
- [ ] No decorative element intercepts clicks.
- [ ] Layout remains readable at 16:9 and one narrower test aspect.
- [ ] Target scene validation reports no missing scripts or broken prefabs.
- [ ] No new console errors are introduced.

## 10. Acceptance Criteria

The work is complete when the Pengenalan page is recognizably faithful to the supplied reference, presents the correct lesson content, preserves all existing Pengenalan navigation/dialog behavior, and passes the verification checklist without introducing scene errors.

## 11. Not Included Without Separate Approval

- Redesigning Faktor, Micro-Scene, Kuiz, or other Pengenalan stages.
- Rewriting `DialogController` or lesson text.
- Fixing pre-existing issues in additive Micro-Scene/Quiz scenes.
- Replacing existing project artwork with newly generated character art.
- Changing curriculum content or scene-loading behavior.

