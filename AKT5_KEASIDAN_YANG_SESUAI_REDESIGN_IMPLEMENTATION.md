# Activity 5 — Keasidan Yang Sesuai Redesign Implementation Plan

## Status

Implemented in `Assets/Scenes/Aktivity/akt5/akt5.unity` on 2026-09-04.

The page now uses `Akt5KeasidanYangSesuaiThemeRoot`, retains the seven legacy children inactive, opens from `Canvas/Faktor`, and returns to `Canvas/Faktor` through the new Back button.

## Objective

Redesign `Canvas/Keasidan Yang Sesuai` in `Assets/Scenes/Aktivity/akt5/akt5.unity` so it closely follows the supplied bright science-learning reference and remains visually consistent with the Activity 5 pages already built for `Center`, `Pengenalan`, `Faktor`, `Kehadiran Udara`, and `Kehadiran Nutrien`.

The reference artwork displays `AKTIVITI 4`, but this destination belongs to Activity 5. The implemented page will therefore display `AKTIVITI 5` and keep the existing Activity 5 navigation flow.

## Current-scene audit

- The destination already exists at `Canvas/Keasidan Yang Sesuai` and is inactive in the saved default scene state.
- The redesigned `Canvas/Faktor` page already opens this exact destination from its `KEASIDAN YANG SESUAI` button.
- The current page has seven direct children: a legacy top/header prefab, a `Center` container, a `Bottom` container, and four legacy dialogue-prefab instances.
- Existing lesson content already covers:
  - why pickled mango remains usable for a longer period;
  - the idea that pickling acidity is unsuitable for microorganism growth;
  - comparing apple slices in vinegar and ordinary water;
  - investigating suitable acidity as a growth factor.
- Reusable Activity 5 assets already include the established background, pupils, a scientist/teacher character, microbes, microscope, water mascot, bubbles, stars, leaves, DNA decorations, and button/panel styling.
- No production-ready isolated pickled-mango jar or matched apple-in-vinegar versus apple-in-water comparison set was found in the initial asset audit.
- Unity MCP is installed and responding, but no Unity Editor session was connected during this planning audit. The live hierarchy and persistent callbacks must be verified again before implementation begins.

## Target visual direction

- Use a 1920 × 1080 reference layout through the existing responsive Canvas Scaler.
- Retain the established green-to-yellow-to-orange Activity 5 background.
- Match the reference's friendly primary-school science style: glossy rounded cards, bold outlines, soft shadows, high contrast, bubbles, stars, leaves, DNA motifs, and cheerful microorganism characters.
- Keep the reading path clear from header, to question, to dialogue, to experiment comparison, to the two summary prompts.
- All words remain editable TextMeshPro text. No lesson text is baked into generated images.

## Target layout

### 1. Background and decorative framing

- Reuse `Assets/Images/Akt5CenterUI/Background/akt5_factor_background_v2.png` when it remains sharp at the final resolution.
- Frame the page with the existing Activity 5 microbes, microscope, water mascot, yogurt cup, bubbles, stars, leaves, and DNA/science decorations.
- Keep decorative art outside the main reading zones, use safe margins, and disable `raycastTarget` on non-interactive images.
- Use decoration density similar to the reference without allowing characters to obscure dialogue or experiment labels.

### 2. Header

- Centred badge: `AKTIVITI 5`.
- Large glossy purple banner: `KEASIDAN YANG SESUAI`.
- Main question card beneath the banner:

  `Bagaimanakah keasidan mempengaruhi pertumbuhan mikroorganisma?`

- Use strong white/dark-purple title contrast, white outer edging, and a soft drop shadow consistent with the earlier Activity 5 redesigns.

### 3. Left dialogue column

Build three compact speech cards with circular pupil/teacher portraits and colour-coded speaker tabs:

1. **Murid**

   `Bagaimanakah mangga jeruk kekal elok untuk jangka masa yang lama?`

2. **Rakan**

   `Keasidan air jeruk tidak sesuai untuk pertumbuhan mikroorganisma.`

3. **Guru**

   `Mari bandingkan hirisan buah dalam larutan cuka dan air biasa.`

Use blue, green, and orange speaker treatments. Text must wrap cleanly at normal Game View size, and portraits must remain secondary to the dialogue.

### 4. Central visual comparison

Create one broad comparison area containing three clearly separated examples:

- **Mangga Jeruk**
  - transparent cutout of a glass jar containing sliced pickled green mango;
  - green label chip above the jar;
  - small leaf accents around the base where they do not hide the jar.
- **Hirisan Buah dalam Cuka**
  - fresh-looking apple slice submerged in a clear vinegar solution;
  - blue label chip;
  - green circular tick badge to communicate slower spoilage.
- **Hirisan Buah dalam Air Biasa**
  - visibly browned apple slice submerged in plain water;
  - purple label chip;
  - red circular cross badge to communicate faster visible deterioration.

The two apple visuals must use the same container, camera angle, scale, lighting, and slice composition so pupils can compare the condition rather than the artwork. The comparison will be presented as a simplified classroom illustration, not proof that pH is the only changing variable unless the underlying experiment controls all other conditions.

### 5. Concept sequence chips

Add three non-navigation summary chips below the experiment visuals:

1. `Keasidan tinggi (pH rendah)`
2. `Pertumbuhan berkurang`
3. `Makanan tahan lebih lama`

Use red/orange, blue, and green treatments respectively. The added `(pH rendah)` clarification avoids teaching the common misconception that a larger pH value means greater acidity.

### 6. Bottom learning cards

Add two wide reflection cards:

- **IDEA UTAMA**

  `Keadaan yang terlalu berasid boleh menghalang pertumbuhan mikroorganisma. Oleh itu, makanan berjeruk boleh tahan lebih lama.`

- **SOALAN PENCETUS**

  `Mengapakah hirisan buah dalam cuka lebih lambat rosak?`

These cards are informational and must not behave as scored quiz controls.

### 7. Navigation and interaction

- Add a visible top-left Back button matching the other redesigned Activity 5 pages.
- Back action:
  1. deactivate `Canvas/Keasidan Yang Sesuai`;
  2. activate `Canvas/Faktor`.
- Preserve any valid existing lesson-progression callback only after confirming it in the live Editor; do not invent a new destination.
- Only the Back button is required to be interactive on this informational page. Dialogue cards, comparison visuals, and concept chips do not navigate.
- Apply the project's established hover/press feedback and ensure hidden legacy graphics cannot intercept input.

## Planned hierarchy

```text
Canvas
└── Keasidan Yang Sesuai
    ├── [legacy objects retained but hidden]
    └── Akt5KeasidanYangSesuaiThemeRoot
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
        ├── AcidityComparison
        │   ├── PickledMangoCard
        │   ├── VinegarAppleCard
        │   └── PlainWaterAppleCard
        ├── ConceptSequence
        │   ├── HighAcidityChip
        │   ├── ReducedGrowthChip
        │   └── LongerKeepingChip
        ├── MainIdeaCard
        ├── TriggerQuestionCard
        └── BackButton
```

## Asset strategy

### Reuse first

- `Assets/Images/Akt5CenterUI/Background/akt5_factor_background_v2.png`
- `Assets/Images/2d/student1.png`
- `Assets/Images/2d/student2.png`
- `Assets/Images/scientist.png`
- Existing Activity 5 microorganism, microscope, water mascot, yogurt, bubble, star, leaf, DNA, badge, arrow, tick, cross, panel, and icon sprites where their transparency and resolution are suitable.
- Reuse the same TMP font, outlines, shadows, rounded panels, and button feedback used by the current Activity 5 builders.

### Generate only if needed during execution

Use the built-in ImageGen workflow only for missing bitmap illustrations, one asset per generation call and with no typography:

1. Glass jar filled with sliced pickled green mango, friendly polished educational 3D-cartoon style, front three-quarter view, isolated transparent background, no label, no logo.
2. Fresh apple slice submerged in clear vinegar inside a transparent tumbler, same front three-quarter camera angle and lighting as item 3, isolated transparent background, no text.
3. Browned apple slice submerged in plain water inside the same transparent tumbler, matching item 2 exactly in composition, isolated transparent background, no text.

Store approved new assets under:

`Assets/Images/Akt5KeasidanUI/`

Import each as a single UI Sprite with alpha transparency, mipmaps disabled, sensible max size, and project-compatible compression. Visually inspect every generated image before use; reject baked backgrounds, malformed containers, illegible pseudo-text, or mismatched perspectives.

## Implementation approach

### Phase 1 — Live audit and preservation

- Connect Unity MCP and confirm the active scene is `Assets/Scenes/Aktivity/akt5/akt5.unity`.
- Re-read `Canvas/Keasidan Yang Sesuai`, `Canvas/Faktor`, and all persistent callbacks in the live Editor.
- Create a dated scene backup before mutation:

  `Assets/Scenes/Aktivity/akt5/Backups/akt5_PreKeasidanYangSesuaiRedesign_20260903.unity`

- Record any valid progression logic that must survive the redesign.

**Codex Sol effort:** medium

### Phase 2 — Asset preparation

- Test existing Activity 5 characters and decorations at final Game View scale.
- Generate only the missing pickled-mango and matched apple-comparison cutouts.
- Verify clean transparent edges, consistent perspective, colour harmony, and adequate resolution before import.

**Codex Sol effort:** high

### Phase 3 — Idempotent Unity builder

Create:

`Assets/Editor/Akt5KeasidanYangSesuaiUIBuilder.cs`

Add the menu action:

`Tools > MicroFun > Activity 5 > Build Keasidan Yang Sesuai Theme`

The builder will:

- validate the active scene and required `Canvas` paths;
- remove only an existing `Akt5KeasidanYangSesuaiThemeRoot` before rebuilding;
- retain and hide legacy page objects non-destructively;
- construct the background, decorations, header, dialogue cards, experiment visuals, concept chips, learning cards, and Back button;
- use TextMeshPro for every lesson label and sentence;
- add persistent Back callbacks;
- disable raycast targets on decorative and informational images;
- save only after the build completes successfully;
- restore the saved default state with `Canvas/Faktor` active and `Canvas/Keasidan Yang Sesuai` inactive.

**Codex Sol effort:** high

### Phase 4 — Functional integration

- Verify `Faktor > Keasidan Yang Sesuai` opens the redesigned page.
- Verify Back returns to `Faktor`.
- Confirm only the intended controls receive clicks and no hidden legacy content blocks input.
- Preserve confirmed existing behavior without adding unrequested scene transitions or quiz logic.

**Codex Sol effort:** medium

### Phase 5 — Visual and educational QA

- Compile with zero new errors.
- Test the normal project Game View and 16:9 layouts.
- Check title fit, Malay text wrapping, contrast, icon clarity, safe margins, and card spacing.
- Confirm that the vinegar and plain-water images are visually matched and their tick/cross states are immediately understandable.
- Verify the explanation consistently links stronger acidity with lower pH, reduced microorganism growth, and longer food preservation without claiming that all microorganisms respond identically.
- Compare a final Game View capture with the reference for hierarchy, colour balance, and visual rhythm.
- Run the builder twice and confirm that exactly one generated theme root remains.

**Codex Sol effort:** high

## Acceptance criteria

- The page displays `AKTIVITI 5`, not the `AKTIVITI 4` text shown in the reference.
- `KEASIDAN YANG SESUAI` is the dominant title and all Malay text is editable and fully visible.
- The pickled-mango example and the vinegar-versus-water comparison are understandable at a glance.
- The summary sequence clearly communicates `Keasidan tinggi (pH rendah) → Pertumbuhan berkurang → Makanan tahan lebih lama`.
- Dialogue, main idea, and trigger question remain readable at normal play resolution.
- The page opens from `Canvas/Faktor`, and Back returns to `Canvas/Faktor`.
- No legacy UI overlaps the redesign or intercepts clicks.
- Rebuilding is idempotent and produces no duplicate generated hierarchy.
- The scene saves cleanly with no new Console errors.

## Recommended Codex Sol setting

- Model: `gpt-5.6-sol`
- Overall reasoning effort: **high**
- Use **medium** effort for the live hierarchy/callback audit and functional wiring.
- Use **high** effort for matched image generation, dense layout construction, scientific wording, text fit, and visual QA.
- Estimated implementation effort: approximately **2–3 focused hours**, depending on how many comparison assets require generation and revision.

## Execution boundary

Satisfied by the user's explicit proceed instruction before the builder, asset generation, callback wiring, and scene save were performed.

## Implementation result

- Builder created: `Assets/Editor/Akt5KeasidanYangSesuaiUIBuilder.cs`.
- Scene backup created: `Assets/Scenes/Aktivity/akt5/Backups/akt5_PreKeasidanYangSesuaiRedesign_20260903.unity`.
- Three transparent built-in ImageGen assets were added under `Assets/Images/Akt5KeasidanUI/`:
  - `pickled_mango_jar_v1.png` — polished isolated jar of sliced pickled green mango;
  - `apple_in_vinegar_v1.png` — isolated clear tumbler with a fresh apple half;
  - `apple_in_plain_water_browned_v1.png` — isolated matching classroom comparison with visibly browned apple flesh.
- The generated images were produced with the built-in ImageGen workflow. Prompts specified scientific-educational Unity sprites, polished soft 3D-cartoon styling, true alpha transparency, a centred single-object composition, no typography, no branding, and no background. Two checkerboard-background edit attempts were rejected and were not imported.
- Corner-alpha inspection confirmed genuine transparency for all three imported PNG files.
- All lesson wording remains editable TextMeshPro text rather than baked image text.
- The generated hierarchy contains one theme root with 80 descendants and zero active legacy children.
- Running the builder twice confirmed idempotent rebuilding with exactly one generated root.
- Play Mode navigation tests passed:
  - `Faktor > Keasidan Yang Sesuai` produced `page=True, faktor=False`;
  - the new Back button produced `page=False, faktor=True`.
- The saved default state keeps `Canvas/Faktor` active and `Canvas/Keasidan Yang Sesuai` inactive.
- The final Play Mode capture is saved at `Assets/Screenshots/akt5_keasidan_redesign_final.png`.
- Unity script validation reported zero diagnostics; the final Console query reported zero errors.
- Unity scene validation reported zero issues, zero missing scripts, and zero broken prefabs.
