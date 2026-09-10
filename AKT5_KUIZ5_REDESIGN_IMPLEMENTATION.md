# Activity 5 — Kuiz 5 Redesign Implementation Plan

## Status

Planning only. No Unity scene, script, callback, quiz data, audio, or visual asset is changed by this document.

## Goal

Redesign `Assets/Scenes/Aktivity/akt5/kuiz5.unity` to match the supplied cheerful science-quiz reference:

- warm green/cream/orange Activity 5 background;
- clear `KUIZ` heading;
- `Soalan 1/3` progress badge;
- `SP 3.1 | SK 3.1.4` curriculum badge;
- large question panel;
- rounded answer cards with letter badges;
- a prominent `PILIH JAWAPAN` action button;
- audio and Back controls;
- Activity 5 microorganism, microscope, water-drop, bubble, atom, DNA, petri-dish, and magnifier decoration.

The redesign must preserve the existing quiz controller, scoring, sound feedback, question progression, and result flow.

## Read-only scene audit

Active scene: `Assets/Scenes/Aktivity/akt5/kuiz5.unity`.

Current relevant structure:

```text
Canvas
├── Title
├── Score
├── Back
├── Content
│   ├── QuizController
│   └── bg
│       ├── Soalan
│       └── OptionsButtons
└── WinPanel

QuestionClasses
Sounds
EventSystem
Main Camera
Directional Light
```

The live `QuizController` currently has:

- three question records;
- four answer choices per record;
- four wired answer buttons;
- existing correct/wrong audio sources;
- an existing result panel;
- no action/submit button assigned;
- no narration button or narration source assigned;
- null question text in all three question records;
- `correctIndex = 0` in every record.

## Important design decision: retain four answers

The supplied reference visually shows three answer cards (`A–C`), but Kuiz 5 is already configured for four choices (`A–D`).

This implementation will retain **four** answer cards so the redesign does not silently remove gameplay choices. The cards will be slightly shorter and arranged as one vertical stack while preserving the reference’s overall hierarchy.

## Pre-existing content issue to repair

The current question strings are empty. The controller therefore falls back to the same generic question, `Apakah definisi mikroorganisma?`, while the choices discuss Activity 5 growth factors. The first two stored correct-answer indices also do not match the apparent correct choices.

The implementation should populate and validate the following three question records:

### Question 1

**Question**

> Antara berikut, keadaan manakah paling sesuai untuk pertumbuhan mikroorganisma?

**Choices**

1. Kehadiran cahaya yang terang
2. Keadaan suhu yang sangat rendah
3. Kehadiran nutrien dan kelembapan yang mencukupi
4. Keadaan persekitaran yang sangat berasid

**Correct index:** `2` — choice C.

### Question 2

**Question**

> Mengapakah kulapuk mudah tumbuh pada roti lembap yang dibiarkan di atas meja?

**Choices**

1. Kerana roti tersebut disimpan di tempat yang gelap.
2. Kerana roti membekalkan nutrien dan mempunyai kelembapan.
3. Kerana suhu di atas meja adalah sangat rendah.
4. Kerana udara di sekeliling roti adalah sangat kering.

**Correct index:** `1` — choice B.

### Question 3

**Question**

> Situasi manakah menunjukkan suhu mempengaruhi pertumbuhan mikroorganisma?

**Choices**

1. Susu menjadi basi apabila dibiarkan di luar peti sejuk.
2. Buah-buahan yang dikeringkan tahan lebih lama.
3. Penjerukan buah mangga menggunakan cuka menghalang pertumbuhan bakteria.
4. Roti yang lembap ditumbuhi tompokan hitam.

**Correct index:** `0` — choice A.

This content repair is limited to making the existing Activity 5 choices coherent. It does not add or remove questions.

## Proposed 16:9 layout

Target reference resolution: **1920 × 1080**.

### 1. Background and decoration

- Reuse `Assets/Images/Akt5CenterUI/Background/akt5_factor_background_v2.png`.
- Reuse the established green and purple microorganism mascots, water-drop mascot, microscope, petri-dish, bubbles, atom, DNA, and magnifier artwork where available.
- Keep large mascots semi-transparent behind the question/answer area.
- Keep foreground decorations outside all text and button hit areas.

### 2. Top row

- Top-left: `Dengar audio` pill with a round green speaker icon.
- Top-centre: dynamic `Soalan 1/3` indicator.
- Top-right: `SP 3.1 | SK 3.1.4`.
- Main heading beneath: `KUIZ`, centred in dark green with a cream outline/shadow.

### 3. Question card

- Large cream rounded card with dark outline and soft shadow.
- Dynamic question text centred with sufficient padding.
- TextMeshPro auto-sizing within a controlled range so all three questions fit without clipping.

### 4. Answer stack

- Four cream rounded answer cards labelled `A`, `B`, `C`, and `D`.
- Letter badge: bright green circle, white letter, dark outline.
- Answer text: left-aligned dark green.
- Whole card is clickable; decorative children do not block raycasts.
- Optional icon slot remains available but hidden when a question has no choice icon.
- Visual states remain driven by `QuizController`:
  - idle: cream;
  - selected: light green;
  - correct: success green;
  - wrong: coral/red flash;
  - disabled: visibly muted without making the text unreadable.

### 5. Action button

- Large green button centred at the bottom.
- Initial label: `PILIH JAWAPAN`.
- After a correct answer: `SETERUSNYA`.
- Yellow/orange forward triangle at the right.
- Enable `QuizController.useSubmitConfirmation` and bind:
  - `nextButton`;
  - `actionButtonLabel`.

### 6. Back button

- Reuse the existing `Canvas/Back` object and its `BackController` rather than replacing its navigation logic.
- Restyle and reposition it to the bottom-left safe area.
- Keep its current destination unchanged.

### 7. Audio button

- Create and bind the visual `Dengar audio` control to `QuizController.PlayCurrentQuestionAudio`.
- Because all three `narrationClip` fields are currently empty, the button must appear disabled until clips are assigned.
- Do not imply audio is available when no narration clip exists.
- Malay narration generation is a separate optional content task, not required for the visual redesign.

### 8. Result overlay

- Preserve scoring and the existing `WinPanel` flow.
- Rebuild the result presentation using the same background, rounded cream panel, celebratory stars, score text, and existing continuation/back actions.
- Keep the overlay inactive until all three questions are completed.

## Planned hierarchy

```text
Canvas
├── [legacy visual children retained but inactive]
├── Content
│   ├── QuizController                    [kept active]
│   └── bg                               [legacy visual child inactive]
├── Back                                 [existing callback retained and restyled]
└── Akt5Kuiz5ThemeRoot
    ├── Background
    ├── BackgroundCharacters
    ├── DecorativeLayer
    ├── AudioButton
    ├── QuestionIndicator
    ├── CurriculumBadge
    ├── QuizTitle
    ├── QuestionCard
    │   └── QuestionText
    ├── AnswerStack
    │   ├── AnswerA
    │   │   ├── LetterBadge
    │   │   │   └── LetterLabel
    │   │   ├── ChoiceIcon
    │   │   └── AnswerLabel
    │   ├── AnswerB
    │   ├── AnswerC
    │   └── AnswerD
    ├── PilihJawapanButton
    │   ├── Label
    │   └── ForwardIcon
    └── ResultOverlay
```

## Builder and preservation strategy

Create an idempotent Editor builder:

- Script: `Assets/Editor/Akt5Kuiz5UIBuilder.cs`
- Menu: `Tools > MicroFun > Activity 5 > Build Kuiz 5 Theme`
- Generated root: `Canvas/Akt5Kuiz5ThemeRoot`
- Backup: `Assets/Scenes/Aktivity/akt5/Backups/kuiz5_PreThemeRedesign_20260904.unity`

Builder responsibilities:

1. Verify the active project and required `kuiz5` hierarchy.
2. Create the backup once.
3. Remove only the previously generated `Akt5Kuiz5ThemeRoot`.
4. Preserve existing controller hosts, sound objects, scoring objects, EventSystem, camera, light, and Back callback.
5. Disable only the old visual hierarchy.
6. Build the entire replacement UI in one editor-script pass.
7. Rebind the existing `QuizController` to:
   - new question text;
   - four new answer buttons;
   - new indicator;
   - new action button and label;
   - optional choice-icon images;
   - audio button/source when available;
   - new result overlay.
8. Enable submit confirmation.
9. Populate the three audited question strings and corrected indices.
10. Save the scene with the quiz root active and result overlay inactive.

All listener lists created by the builder must be cleared/rebuilt deterministically so rerunning it cannot duplicate callbacks.

## Asset strategy

No new generated artwork is required for the first implementation pass. The visual theme can be assembled from assets already used by the Activity 5 and other quiz redesigns:

- Activity 5 background;
- green/purple microorganisms;
- water-drop mascot;
- microscope and petri dish;
- bubbles, stars, DNA, atom, magnifier;
- flat white button/panel sprites;
- `FredokaOne SDF.asset`;
- existing audio, Back, check, arrow, and speaker icons.

If a required icon is unavailable, create it from simple Unity UI shapes before requesting a generated raster asset. Keep all words and letters in TextMeshPro.

## Implementation phases and effort

Recommended model: **Codex Sol**, **High** effort.

| Phase | Work | Effort |
|---|---|---|
| 1. Backup and final binding audit | Preserve scene and record all current controller, sound, Back, and result references | Medium |
| 2. Builder implementation | Construct the full visual hierarchy and rebind the live controller in one editor pass | High |
| 3. Content repair | Populate the three question strings and validate correct indices | Medium |
| 4. Interaction integration | Submit-confirmation flow, answer states, next-question flow, audio disabled state, result panel | High |
| 5. Visual polish | Typography, spacing, decoration opacity, responsive anchors, shadows, outlines | High |
| 6. Verification | Compilation, Play Mode, all questions, wrong/correct paths, result, Back navigation, idempotency | High |

Estimated implementation time: **2–3 focused hours**.

## Verification checklist

- Unity compiles with no new errors.
- `Akt5Kuiz5ThemeRoot` exists exactly once after repeated builder runs.
- No legacy quiz controller, score controller, sound source, EventSystem, or Back callback is lost.
- The scene displays `Soalan 1/3`, `2/3`, and `3/3` correctly.
- All four answer choices fit at 1920 × 1080 with no overlap or clipping.
- Answer letter badges update as `A–D`.
- Selecting a choice highlights it without immediately submitting.
- `PILIH JAWAPAN` remains disabled until a choice is selected.
- Wrong choices flash red and remain retryable.
- A correct choice turns green and changes the action to `SETERUSNYA`.
- Score increases exactly once for each correct answer.
- The result overlay appears after Question 3.
- `Dengar audio` is disabled when the current question has no narration clip.
- Back navigation still reaches its original destination.
- The scene validates with no missing scripts or broken references.
- A 16:9 Play Mode screenshot is checked against the supplied reference.

## Acceptance criteria

The finished Kuiz 5 scene should feel like the same product as the redesigned Activity 5 pages while remaining a fully working three-question quiz. The only deliberate visual departure from the reference is the retained fourth answer card, required by the existing quiz content and controller.
