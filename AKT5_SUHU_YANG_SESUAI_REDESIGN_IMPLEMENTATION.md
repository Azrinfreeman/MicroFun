# Activity 5 — `Suhu Yang Sesuai` Redesign Implementation Plan

## Status

Planning only. No Unity scene, script, callback, or image asset is changed by this document.

## Goal

Redesign the existing `Canvas/Suhu Yang Sesuai` page in `Assets/Scenes/Aktivity/akt5/akt5.unity` so it follows the same cheerful Activity 5 visual language as the supplied reference, while replacing every air/vacuum example with a scientifically accurate temperature lesson for Year 6 pupils.

The page will explain **why temperature is a growth factor**: temperature changes how quickly microorganisms can grow and reproduce. Chilling normally slows growth; it does not mean that all microorganisms are killed.

## Read-only audit completed

- Active Unity scene: `Assets/Scenes/Aktivity/akt5/akt5.unity`.
- Target page exists at `Canvas/Suhu Yang Sesuai` and is currently active.
- The existing page still contains the legacy groups `Top`, `Center`, `Bottom`, `DialogParentLeft Variant`, `DialogParentRight Variant`, and `DialogParentRight Variant (1)`.
- The redesign will preserve those objects but disable them beneath the target page.
- `Canvas/Faktor` already exists and will remain the destination of the Back button.

## Scientific basis and wording guardrails

The lesson copy will be based on the following official guidance:

- The FDA explains that keeping food properly chilled is one of the best ways to prevent or slow bacterial growth and recommends a refrigerator temperature of **4°C or below**. It also notes that freezing does not kill most bacteria, although it stops growth while frozen: [FDA — Are You Storing Food Safely?](https://www.fda.gov/consumers/consumer-updates/are-you-storing-food-safely)
- The WHO includes keeping food at safe temperatures among its five core food-safety messages: [WHO — Five Keys to Safer Food](https://www.who.int/publications/i/item/9789241594639)

The UI must therefore avoid these misconceptions:

- Do not say that cold kills all microorganisms.
- Do not say that a higher temperature always causes faster growth; very high temperatures can inhibit or destroy many microorganisms.
- Do not ask pupils to taste or smell an experimental food sample.
- Use “banyak mikroorganisma” rather than implying that every species grows under the same conditions.

## Proposed 16:9 composition

Target reference resolution: **1920 × 1080**.

1. **Background and frame**
   - Reuse the Activity 5 green-to-cream-to-orange background, bubbles, sparkles, DNA motif, leaves, and microorganism mascots already established in the other Activity 5 pages.
   - Keep the decoration away from text and controls.
   - Use warm red/orange temperature accents with cool blue comparison accents.

2. **Header**
   - Top badge: `AKTIVITI 5`.
   - Main ribbon: `SUHU YANG SESUAI`.
   - Supporting question: `Bagaimanakah suhu mempengaruhi pertumbuhan mikroorganisma?`
   - Add a thermometer icon to distinguish this page from the air, nutrient, and acidity pages.

3. **Left dialogue column**
   - Three stacked speech cards using the pupil/friend/teacher illustration treatment from the established theme.
   - Keep sentences short enough to remain readable at the game’s scaled resolution.

4. **Main comparison panel**
   - Pink tab label: `Eksperimen Mudah`.
   - Instruction: `Bandingkan sup yang disimpan pada suhu rendah dengan sup pada suhu bilik untuk tempoh yang sama.`
   - Two equally sized cards separated by `VS`:
     - Cool-blue card: `Dalam Peti Sejuk (≤4°C)`.
     - Warm-orange card: `Suhu Bilik`.
   - Use the same covered soup container in both images so temperature is the only obvious variable.
   - Add a small safety label: `Pemerhatian sahaja — jangan rasa sampel.`

5. **Concept summary**
   - Present a simple cause-and-effect sequence:
     - `Suhu rendah` → `Pertumbuhan perlahan` → `Makanan tahan lebih lama`.
     - `Suhu sesuai` → `Pembiakan lebih cepat` → `Makanan cepat rosak`.

6. **Bottom learning cards**
   - `IDEA UTAMA` card for the complete conclusion.
   - `SOALAN PENCETUS` card for pupil discussion.

7. **Back navigation**
   - Place the standard round green Back button at the top-left safe area.
   - It will deactivate `Canvas/Suhu Yang Sesuai` and reactivate `Canvas/Faktor`.
   - No scene loading is required.

## Final Malay copy

### Dialogue cards

**Murid**

> Mengapakah sup yang dibiarkan pada suhu bilik lebih cepat basi?

**Rakan**

> Pada suhu yang sesuai, banyak mikroorganisma membiak dengan lebih cepat.

**Guru**

> Bandingkan sup yang disimpan di dalam peti sejuk dengan sup pada suhu bilik.

### Comparison cards

**Dalam Peti Sejuk (≤4°C)**

> Suhu rendah memperlahankan pertumbuhan mikroorganisma. Sup kekal baik lebih lama jika disimpan dengan betul.

**Suhu Bilik**

> Pada suhu yang sesuai, mikroorganisma boleh membiak dengan lebih cepat dan sup lebih cepat rosak.

### Main idea

> Suhu mempengaruhi kadar pertumbuhan mikroorganisma. Suhu rendah biasanya memperlahankan pertumbuhan, manakala suhu yang sesuai membolehkan banyak mikroorganisma membiak dengan lebih cepat.

### Trigger question

> Mengapakah sup yang disimpan di dalam peti sejuk lebih lambat basi?

### Safety note

> Pemerhatian sahaja — jangan rasa sampel eksperimen.

## Planned Unity hierarchy

```text
Canvas
└── Suhu Yang Sesuai
    ├── [legacy children retained but inactive]
    └── Akt5SuhuYangSesuaiThemeRoot
        ├── Background
        ├── DecorativeLayer
        ├── BackButton
        ├── ActivityBadge
        ├── TitleRibbon
        ├── SupportingQuestion
        ├── DialogueColumn
        │   ├── PupilCard
        │   ├── FriendCard
        │   └── TeacherCard
        ├── TemperatureComparison
        │   ├── ExperimentTab
        │   ├── Instruction
        │   ├── RefrigeratedCard
        │   ├── VersusBadge
        │   ├── RoomTemperatureCard
        │   └── SafetyNote
        ├── ConceptSummary
        ├── MainIdeaCard
        └── TriggerQuestionCard
```

## Asset plan

### Reuse first

- Activity 5 background and decorative assets already used by the Faktor pages.
- Existing pupil, friend, teacher, microscope, water-drop, green-microbe, and purple-microbe artwork.
- Existing TextMeshPro font/material setup and standard Back icon.

### Generate only if no suitable project asset exists

Create assets under `Assets/Images/Akt5SuhuUI/`:

- `akt5_suhu_soup_refrigerated_v1.png` — a clean, covered bowl/container of soup with subtle cool cues; transparent background; no text.
- `akt5_suhu_soup_room_temperature_v1.png` — the same container and camera angle, showing mild visual spoilage cues without grotesque mould; transparent background; no text.
- `akt5_suhu_thermometer_badge_v1.png` — friendly red-and-blue thermometer icon; transparent background; no text.

Generation rules:

- Generate the refrigerated container first and use it as the visual reference for the room-temperature variant so the comparison remains controlled.
- Keep all educational text in TextMeshPro, not inside raster images.
- Require true alpha transparency; reject checkerboard pixels baked into an image.
- Match the rounded, glossy, child-friendly 3D illustration style used across the existing Activity 5 redesigns.
- Import as Sprite (2D and UI), preserve alpha, disable mipmaps for UI, and use lossless/high-quality compression appropriate to the target platform.

## Implementation approach

Create an idempotent editor builder:

- Script: `Assets/Editor/Akt5SuhuYangSesuaiUIBuilder.cs`
- Menu: `Tools > MicroFun > Activity 5 > Build Suhu Yang Sesuai Theme`
- Generated root: `Canvas/Suhu Yang Sesuai/Akt5SuhuYangSesuaiThemeRoot`

Builder responsibilities:

1. Confirm that `akt5.unity`, `Canvas`, `Suhu Yang Sesuai`, and `Faktor` are present.
2. Save a pre-change backup at `Assets/Scenes/Aktivity/akt5/Backups/akt5_PreSuhuYangSesuaiRedesign_20260904.unity` if it does not already exist.
3. Remove only a previously generated `Akt5SuhuYangSesuaiThemeRoot` before rebuilding.
4. Preserve and deactivate all legacy direct children of `Suhu Yang Sesuai`.
5. Build the new hierarchy at 1920 × 1080 using anchored `RectTransform` layouts.
6. Use TextMeshPro for all copy, with Malay diacritics and symbols verified.
7. Wire the Back button to show `Faktor` and hide `Suhu Yang Sesuai`.
8. Preserve the existing Faktor-page button that opens `Suhu Yang Sesuai`; repair it only if inspection proves its callback is broken.
9. Save the scene and restore the default authoring state: `Faktor` active, `Suhu Yang Sesuai` inactive.

## Phases and Codex Sol effort

Recommended model: **Codex Sol** with **High** effort for the complete implementation.

| Phase | Work | Effort |
|---|---|---|
| 1. Preservation audit | Inspect current hierarchy, callbacks, reusable assets, and create the scene backup | Medium |
| 2. Content layout | Build the new root, typography, cards, comparison grid, and responsive anchors | High |
| 3. Visual assets | Reuse project art; generate and import the matched soup/thermometer assets only where needed | High |
| 4. Navigation | Preserve the Faktor entry button and wire the Back button | Medium |
| 5. Visual polish | Tune spacing, text wrapping, outlines, shadows, colour contrast, and safe-area placement | High |
| 6. Verification | Compile, validate scene, test both navigation directions, inspect 16:9 Game view, and confirm idempotency | High |

Estimated implementation time: **2–3 focused hours**, depending on the number of image-generation iterations required.

## Verification checklist

- Unity compiles with no new errors.
- `Akt5SuhuYangSesuaiThemeRoot` exists exactly once.
- All legacy target-page children remain present and inactive.
- All text is readable with no clipping, overlap, or unexpected wrapping at 1920 × 1080.
- The two soup illustrations use the same container, framing, and scale.
- The copy says cold **slows** growth and never claims that it kills every microorganism.
- `≤4°C` renders correctly.
- The safety note is visible and does not encourage tasting spoiled food.
- The Faktor `SUHU` button opens the page.
- The Back button returns to `Faktor`.
- Re-running the builder produces no duplicate roots or event listeners.
- The saved scene opens with `Faktor` active and `Suhu Yang Sesuai` inactive.
- A final 16:9 Game-view screenshot is compared with the supplied reference for hierarchy, density, and theme consistency.

## Acceptance criteria

The redesign is complete when the page visibly belongs to the established Activity 5 theme, explains temperature using an accurate refrigerator-versus-room-temperature comparison, provides safe Year 6 wording, retains reversible legacy content, and passes navigation, hierarchy, compilation, and visual checks.

## Not executed in this planning pass

- No image was generated.
- No Unity object was created, disabled, deleted, or moved.
- No callback was changed.
- No scene or script was modified.
