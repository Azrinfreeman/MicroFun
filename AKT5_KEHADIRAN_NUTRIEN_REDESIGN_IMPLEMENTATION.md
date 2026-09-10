# Activity 5 — Kehadiran Nutrien Redesign Implementation Plan

## Status

Implemented in `Assets/Scenes/Aktivity/akt5/akt5.unity` on 2026-09-03.

The page now uses `Akt5KehadiranNutrienThemeRoot`, retains the legacy hierarchy inactive, opens from `Canvas/Faktor`, and returns to `Canvas/Faktor` through the new Back button.

## Objective

Redesign `Canvas/Kehadiran Nutrien` in `Assets/Scenes/Aktivity/akt5/akt5.unity` to closely follow the supplied bright science-learning reference while staying consistent with the Activity 5 theme already built for `Center`, `Pengenalan`, `Faktor`, and `Kehadiran Udara`.

The reference image displays `AKTIVITI 4`, but this page belongs to Activity 5. The implementation will therefore use `AKTIVITI 5` and preserve the current Activity 5 navigation flow.

## Current-scene audit

- The destination already exists at `Canvas/Kehadiran Nutrien` and is inactive in the saved default scene state.
- The redesigned `Faktor` page already opens this exact destination.
- The current page contains seven direct children:
  - legacy `Top` prefab;
  - legacy `Center` object;
  - legacy `Bottom` object;
  - two `DialogParentLeft Variant` prefabs;
  - two `DialogParentRight Variant` prefabs.
- Existing lesson content already covers these ideas:
  - adding sugar when using yeast;
  - yeast needing nutrients to grow and become active;
  - comparing microorganism growth in distilled water and fresh milk;
  - observing growth after three days.
- Existing project art includes `student1.png`, `student2.png`, a nutrition icon, and Activity 5 background/decorative assets. No suitable production-ready distilled-water, fresh-milk, and yeast-plus-sugar comparison set was found in the initial asset search.
- Unity MCP is installed and responding, but no Unity Editor session was connected during this planning audit. A live hierarchy/callback verification is therefore required before implementation.

## Target layout

Use a 1920 × 1080 reference layout with responsive anchors through the existing Canvas Scaler.

### 1. Background and decoration

- Reuse the established Activity 5 green-to-yellow-to-orange science background.
- Use bubbles, stars, DNA/science motifs, leaves, microscopes, the water mascot, and friendly microorganisms as decorative framing.
- Keep all decorations outside the main reading zones and disable their raycast targets.
- Maintain generous safe margins so the page remains readable at normal Game View resolution.

### 2. Header

- Centred activity badge: `AKTIVITI 5`.
- Large glossy purple title banner: `KEHADIRAN NUTRIEN`.
- Main question card beneath the title:

  `Mengapa mikroorganisma memerlukan nutrien?`

- Use editable TextMeshPro text; do not bake wording into generated images.

### 3. Left dialogue column

Build three compact speech cards with pupil/teacher cutouts:

1. **Murid**

   `Mengapakah kita perlu memasukkan gula semasa eksperimen yis?`

2. **Rakan**

   `Yis memerlukan nutrien untuk tumbuh dan menjadi aktif.`

3. **Guru**

   `Bandingkan pertumbuhan mikroorganisma dalam air suling dan susu segar.`

Use consistent blue, green, and orange speaker labels. Character art must remain secondary to the text and must not overlap the central comparison panel.

### 4. Central comparison panel

Create one large rounded comparison panel divided into two equal sections:

- **AIR SULING**
  - clear glass of distilled water;
  - cool blue card treatment;
  - short label `air suling` beneath the image.
- **SUSU SEGAR**
  - fresh-milk carton and glass;
  - light green card treatment;
  - short label `susu segar` beneath the image.
- Centre divider: circular `VS` badge.
- Small inset on the milk/nutrient side:
  - title `YIS + GULA`;
  - spoonful of sugar and yeast granules;
  - caption `Sumber nutrien menjadikan yis aktif dan tumbuh.`

The visual treatment should present this as a simple classroom comparison and discussion prompt. It must not claim that nutrient level is the only experimental variable unless the underlying lesson setup controls the other differences between distilled water and fresh milk.

### 5. Right concept summary

Use three stacked, non-navigation concept chips:

1. `Nutrien`
2. `Yis aktif`
3. `Pertumbuhan meningkat`

Each chip receives a simple editable icon plus a green, blue, or purple colour treatment matching the reference. These chips summarize the learning sequence and do not change pages.

### 6. Bottom learning cards

Add two full-width learning prompts beneath the comparison:

- **IDEA UTAMA**

  `Mikroorganisma memerlukan nutrien sebagai sumber makanan untuk tumbuh, membiak dan menjadi aktif.`

- **SOALAN PENCETUS**

  `Medium manakah lebih sesuai untuk pertumbuhan mikroorganisma: air suling atau susu segar? Mengapa?`

These are informational reflection cards, not scored quiz controls.

### 7. Navigation

- Add a visible top-left Back button matching the other redesigned Activity 5 pages.
- Back action:
  1. deactivate `Canvas/Kehadiran Nutrien`;
  2. activate `Canvas/Faktor`.
- Preserve any currently valid lesson-progression callback only after it is confirmed in the live Unity hierarchy; do not invent a new destination.
- Add the project’s standard button interaction feedback to the Back button.

## Planned hierarchy

```text
Canvas
└── Kehadiran Nutrien
    ├── [legacy objects retained but hidden]
    └── Akt5KehadiranNutrienThemeRoot
        ├── Background
        ├── Decorations
        ├── Header
        │   ├── ActivityBadge
        │   ├── TitleBanner
        │   └── MainQuestion
        ├── DialogueColumn
        │   ├── PupilCard
        │   ├── FriendCard
        │   └── TeacherCard
        ├── ComparisonPanel
        │   ├── DistilledWaterCard
        │   ├── VersusBadge
        │   ├── FreshMilkCard
        │   └── YeastSugarInset
        ├── ConceptSummary
        │   ├── NutrientChip
        │   ├── ActiveYeastChip
        │   └── GrowthChip
        ├── MainIdeaCard
        ├── TriggerQuestionCard
        └── BackButton
```

## Asset strategy

### Reuse first

- `Assets/Images/Akt5CenterUI/Background/akt5_factor_background_v2.png`
- `Assets/Images/2d/student1.png`
- `Assets/Images/2d/student2.png`
- `Assets/Images/UI Flat/icon_fun_nutrition.png`
- Existing Activity 5 microorganism, bubble, star, DNA, leaf, microscope, water mascot, badge, arrow, and panel sprites where their resolution and transparency are suitable.
- Reuse the same TMP font, button feedback, outline, shadow, and panel-building helpers used by the current Activity 5 builders.

### Generate only if needed during execution

Use built-in ImageGen for isolated bitmap cutouts only, with no typography:

1. Clear glass of distilled water, cheerful educational 3D-cartoon style, front three-quarter view, isolated transparent background, no text.
2. Fresh milk carton with a glass of milk, same camera angle and lighting, isolated transparent background, no brand or text.
3. Spoonful of sugar with a small group of yeast granules, friendly educational style, isolated transparent background, no text.
4. Optional pupil/friend/teacher cutouts only if the current project characters cannot be composed cleanly.

Store new assets under:

`Assets/Images/Akt5KehadiranNutrienUI/`

Import each generated image as a single UI Sprite with alpha transparency, mipmaps disabled, appropriate max size, and project-compatible compression.

## Implementation approach

### Phase 1 — Live audit and preservation

- Connect Unity MCP and confirm the active scene is `akt5.unity`.
- Re-read `Canvas/Kehadiran Nutrien`, its direct children, and all persistent callbacks in the live Editor.
- Create a dated backup before mutation:

  `Assets/Scenes/Aktivity/akt5/Backups/akt5_PreKehadiranNutrienRedesign_20260903.unity`

- Record any valid progression logic that must survive the redesign.

**Codex Sol effort:** medium

### Phase 2 — Asset preparation

- Test the existing character and science assets at final Game View scale.
- Generate only the missing water, milk, and yeast-plus-sugar cutouts.
- Verify transparent edges, consistent perspective, colour harmony, and adequate resolution before importing.

**Codex Sol effort:** high

### Phase 3 — Idempotent Unity builder

Create:

`Assets/Editor/Akt5KehadiranNutrienUIBuilder.cs`

Add the menu action:

`Tools > MicroFun > Activity 5 > Build Kehadiran Nutrien Theme`

The builder will:

- validate the active scene and required `Canvas` paths;
- remove only an existing `Akt5KehadiranNutrienThemeRoot` before rebuilding;
- retain and hide legacy page objects non-destructively;
- construct the background, header, dialogue cards, comparison, summaries, prompts, and Back button;
- use TMP for all instructional text;
- add persistent Back callbacks;
- save only after the build completes successfully;
- restore the saved default state with `Canvas/Faktor` active and `Canvas/Kehadiran Nutrien` inactive.

**Codex Sol effort:** high

### Phase 4 — Functional integration

- Verify `Faktor > Kehadiran Nutrien` opens the redesigned page.
- Verify Back returns to `Faktor` and no hidden legacy graphics intercept input.
- Preserve any confirmed progression behavior without adding an unrequested route.
- Confirm the right-side concept chips are non-navigation elements.

**Codex Sol effort:** medium

### Phase 5 — Visual QA and polish

- Compile with zero new errors.
- Test the normal project Game View and at 16:9.
- Check text wrapping, safe margins, contrast, icon clarity, and panel spacing.
- Ensure the three dialogue cards and both bottom prompts remain readable without crowding.
- Compare the final capture with the reference for hierarchy, colour balance, and visual rhythm.
- Run the builder twice and confirm that only one generated root remains.

**Codex Sol effort:** high

## Acceptance criteria

- The page clearly belongs to Activity 5 and displays `AKTIVITI 5`, not the `AKTIVITI 4` text shown in the reference.
- `KEHADIRAN NUTRIEN` is the dominant title and all Malay text is editable and fully visible.
- The distilled-water versus fresh-milk comparison is understandable at a glance.
- The yeast-plus-sugar inset clearly reinforces the nutrient concept.
- Dialogue, main idea, and trigger question are readable at normal play resolution.
- The redesigned page opens from `Faktor`, and Back returns to `Faktor`.
- No legacy UI overlaps the redesign or blocks input.
- Rebuilding is idempotent and produces no duplicate generated hierarchy.
- The scene saves cleanly with no new Console errors.

## Recommended Codex Sol setting

- Model: `gpt-5.6-sol`
- Overall reasoning effort: **high**
- Use **medium** effort for the live hierarchy/callback audit and functional wiring.
- Use **high** effort for image matching, dense layout construction, educational text fit, and visual QA.
- Estimated implementation effort: approximately **2–3 focused hours**, depending on how many comparison/character assets require generation.

## Implementation result

- Builder created: `Assets/Editor/Akt5KehadiranNutrienUIBuilder.cs`.
- Scene backup created: `Assets/Scenes/Aktivity/akt5/Backups/akt5_PreKehadiranNutrienRedesign_20260903.unity`.
- Three transparent ImageGen assets added under `Assets/Images/Akt5KehadiranNutrienUI/`:
  - `distilled_water_glass_v1.png`;
  - `fresh_milk_carton_glass_v1.png`;
  - `yeast_sugar_v1.png`.
- All instructional wording remains editable TextMeshPro text rather than baked image text.
- The generated hierarchy contains one theme root with 74 descendants and is safe to rebuild idempotently.
- `Faktor > Kehadiran Nutrien` deactivates `Faktor` and activates the Nutrien page.
- The Nutrien Back button deactivates the page and reactivates `Faktor`.
- The saved default state keeps `Canvas/Faktor` active and `Canvas/Kehadiran Nutrien` inactive.
- Unity compiled the cleaned builder without recent C# errors.
- Visual QA confirmed the dialogue column, comparison panel, concept chips, learning cards, and generated transparent illustrations render together in the intended Activity 5 theme.
- Unity MCP remained disconnected during execution, so hierarchy and callback verification was completed from the serialized scene and Unity Editor log. A D3D12 Game View maximize attempt caused an Editor crash during the final interactive test; Unity recovered with the already-saved scene intact. No scene or script corruption was detected.
