# Activity 5 — Kehadiran Udara Redesign Implementation Plan

## Status

Implemented in `Assets/Scenes/Aktivity/akt5/akt5.unity` on 2026-09-03.

The page now uses the generated `Akt5KehadiranUdaraThemeRoot`, retains the legacy hierarchy inactive, and returns to `Canvas/Faktor` through the new Back button.

## Objective

Redesign `Canvas/Kehadiran Udara` in `Assets/Scenes/Aktivity/akt5/akt5.unity` to closely match the supplied reference while retaining the established Activity 5 visual language and the existing educational intent.

The completed page will present a clear comparison between apple slices stored **with vacuum packaging** and **without vacuum**, showing that reduced air slows microorganism growth and food spoilage.

## Current-scene audit

- The destination page already exists at `Canvas/Kehadiran Udara` and is opened by the redesigned `Faktor` menu.
- The page contains legacy title/content objects, three old content buttons, two dialogue prefabs, and existing question/answer dialogue data.
- `Assets/Images/2d/apple vacuum.jpg` and asparagus artwork already exist and will be assessed before generating replacements.
- The legacy `Back` object is inactive and currently targets `Canvas/Center`; the redesigned page should instead return to `Canvas/Faktor` because that is its immediate parent menu.
- Existing objects will be retained but hidden wherever practical. The redesign will live under one generated root so it can be rebuilt or removed safely.

## Target layout

The page will use a 1920 × 1080 reference layout with responsive anchors through the existing Canvas Scaler.

### 1. Background and decoration

- Reuse the green-to-yellow-to-orange Activity 5 science background where suitable.
- Keep bubbles, small stars, DNA/science motifs, friendly microorganisms, and the water mascot decorative and non-interactive.
- Decorations will stay outside the main reading zones and will not block buttons or text.

### 2. Header

- Small centered badge: `AKTIVITI 5`.
- Large glossy green banner: `KEHADIRAN UDARA`.
- Book/flask/leaf accents may be reused from the established Activity 5 UI kit.

### 3. Left discussion column

Use three readable speech cards, supported by schoolchild character art:

1. Question card:
   `Sayur asparagus ini dipek di dalam pek vakum. Adakah sayur-sayuran yang dipek vakum tahan lebih lama?`
2. Answer card:
   `Ya! Sayur-sayuran yang dipek vakum boleh tahan lebih lama.`
3. Explanation card:
   `Ini kerana mikroorganisma tidak dapat tumbuh jika tiada udara dalam pek vakum.`

Important terms such as `pek vakum`, `tahan lebih lama`, `mikroorganisma`, and `tiada udara` will use controlled accent colours. Text remains TextMeshPro text rather than being baked into images.

### 4. Experiment comparison panel

- Section tab: `Eksperimen Mudah`.
- Instruction: `Bandingkan hirisan epal yang disimpan dengan dan tanpa vakum.`
- Left card:
  - Label: `Pek vakum`
  - Image: fresh apple slices sealed inside a transparent vacuum bag
  - Caption: `Hirisan epal dalam pek vakum kekal segar lebih lama kerana tiada udara untuk mikroorganisma tumbuh.`
  - Green positive badge
- Centre divider: large `VS` with a dashed vertical separator.
- Right card:
  - Label: `Tanpa vakum`
  - Image: visibly browned apple slices in a clear open-air container
  - Caption: `Hirisan epal tanpa vakum cepat menjadi perang dan rosak kerana mikroorganisma boleh tumbuh dengan adanya udara.`
  - Red negative badge

The two cards will have matching dimensions and visual weight so the comparison is understandable without requiring interaction.

### 5. Bottom prompt

Display a lightbulb prompt card with:

`Bagaimanakah kehadiran udara mempengaruhi pertumbuhan mikroorganisma?`

This is an instructional reflection prompt, not a scored quiz control.

### 6. Navigation

- Add a clearly visible top-left back button consistent with the other redesigned Activity 5 pages.
- Back action:
  1. deactivate `Canvas/Kehadiran Udara`;
  2. activate `Canvas/Faktor`.
- Preserve any existing next/progression behavior only if it is currently required by the lesson flow; do not invent a new destination.
- Add `ButtonInteractionFeedback` to the back button for consistent hover/click response.

## Planned hierarchy

```text
Canvas
└── Kehadiran Udara
    ├── [legacy objects retained but hidden]
    └── Akt5KehadiranUdaraThemeRoot
        ├── Background
        ├── Decorations
        ├── Header
        │   ├── ActivityBadge
        │   └── TitleBanner
        ├── DiscussionColumn
        │   ├── StudentBoy
        │   ├── QuestionCard
        │   ├── StudentGirl
        │   ├── AnswerCard
        │   └── ExplanationCard
        ├── ExperimentPanel
        │   ├── ExperimentTab
        │   ├── InstructionText
        │   ├── VacuumCard
        │   ├── VersusDivider
        │   └── NoVacuumCard
        ├── ReflectionPrompt
        └── BackButton
```

## Asset strategy

### Reuse first

- `Assets/Images/Akt5CenterUI/Background/akt5_factor_background_v2.png`
- `Assets/Images/2d/apple vacuum.jpg`
- Existing Activity 5 character, microorganism, bubble, book, flask, leaf, lightbulb, badge, and back-arrow art where resolution and transparency are acceptable.

### Generate only if needed during execution

Use ImageGen for isolated bitmap artwork, not for the entire UI and not for typography:

1. Fresh apple slices tightly sealed in a transparent vacuum bag, cheerful educational 3D-cartoon illustration, front view, isolated transparent background, no text.
2. Browned apple slices in a clear food container exposed to air, same camera angle and style, isolated transparent background, no text.
3. Optional Malay primary-school boy and girl cutouts only if the current project characters cannot be reused consistently.

Generated assets will be stored in a dedicated versioned folder such as:

`Assets/Images/Akt5KehadiranUdaraUI/`

All generated images will be imported as UI sprites with alpha transparency, sensible max size, and compression settings appropriate for the project target.

## Implementation approach

### Phase 1 — Preservation and preparation

- Confirm the active scene and Canvas path.
- Create a dated scene backup:
  `Assets/Scenes/Aktivity/akt5/Backups/akt5_PreKehadiranUdaraRedesign_20260902.unity`.
- Record the existing page objects and persistent callbacks before editing.
- Confirm which existing artwork can be reused at production quality.

**Codex Sol effort:** medium

### Phase 2 — Asset preparation

- Crop/prepare existing apple and character art when adequate.
- Generate only missing comparison cutouts through ImageGen.
- Remove unwanted backgrounds, keep transparent edges clean, and verify both apple images use the same visual perspective.
- Import and configure sprites in Unity.

**Codex Sol effort:** high

### Phase 3 — Idempotent Unity builder

Create `Assets/Editor/Akt5KehadiranUdaraUIBuilder.cs` with a menu action similar to:

`Tools > MicroFun > Activity 5 > Build Kehadiran Udara Theme`

The builder will:

- validate the active `akt5` scene and required paths;
- remove only an existing `Akt5KehadiranUdaraThemeRoot` before rebuilding;
- keep unrelated/user-authored scene content intact;
- hide legacy page visuals non-destructively;
- build panels, TMP text, images, outlines, shadows, and navigation;
- use persistent Unity callbacks for the back button;
- mark and save the scene only after a successful build.

**Codex Sol effort:** high

### Phase 4 — Functional integration

- Verify `Faktor > Kehadiran Udara` opens the redesigned page.
- Wire the new Back button to `Faktor`, replacing the inactive legacy route to `Center`.
- Confirm there are no invisible raycast blockers from hidden legacy objects.
- Preserve any valid lesson progression callback discovered during final implementation.

**Codex Sol effort:** medium

### Phase 5 — Visual QA and polish

- Compile with zero new errors.
- Test in Play Mode at the project’s normal Game View and at 16:9.
- Check text wrapping, minimum readable font size, contrast, panel spacing, and safe margins.
- Confirm the two experiment images are instantly distinguishable as fresh/vacuum versus browned/no-vacuum.
- Confirm a single generated root exists after repeated builder runs.
- Capture a final screenshot and compare it against the reference for hierarchy, balance, and colour—not pixel-for-pixel copied typography.

**Codex Sol effort:** high

## Acceptance criteria

- The page clearly matches the supplied bright Activity 5 science theme.
- `KEHADIRAN UDARA` is the dominant title and remains readable at normal play resolution.
- All Malay text is correct, fully visible, and editable through TextMeshPro.
- The vacuum/no-vacuum comparison is understandable at a glance.
- The page opens from `Faktor`, and Back returns to `Faktor`.
- No legacy UI overlaps or intercepts input.
- No duplicate generated hierarchy appears after rebuilding.
- The scene saves cleanly and produces no new Console errors.

## Recommended Codex Sol setting

- Model: `gpt-5.6-sol`
- Overall reasoning effort: **high**
- Use **medium** for the initial hierarchy/callback audit and **high** for asset matching, layout construction, and visual QA.
- Estimated implementation effort: approximately **1.5–2.5 focused hours**, depending on whether the existing apple and character art can be reused or must be regenerated.

## Implementation result

- Builder created: `Assets/Editor/Akt5KehadiranUdaraUIBuilder.cs`.
- Scene backup created: `Assets/Scenes/Aktivity/akt5/Backups/akt5_PreKehadiranUdaraRedesign_20260902.unity`.
- Existing `Assets/Images/2d/apple vacuum.jpg` reused for the vacuum comparison.
- A transparent no-vacuum browned-apple cutout was generated with built-in ImageGen and saved as `Assets/Images/Akt5KehadiranUdaraUI/apple_slices_no_vacuum_v1.png`.
- Unsupported check/cross font glyphs found during visual QA were replaced with a project tick sprite and deterministic UI cross bars.
- Repeated builder runs leave exactly one theme root, one vacuum card, and one no-vacuum card.
- The saved default state keeps `Canvas/Faktor` active and `Canvas/Kehadiran Udara` inactive.
- Unity compilation and builder verification completed without new C# errors or builder exceptions.
