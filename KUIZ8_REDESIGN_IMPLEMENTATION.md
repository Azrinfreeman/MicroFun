# Kuiz 8 Reference Redesign — Implementation Plan

## Status

Planning document only. No scene, script, prefab, audio, image, or build-setting changes are authorized by this document.

## Objective

Redesign `Assets/Scenes/Aktivity/akt8/kuiz8.unity` to closely match the supplied 1920×1080 reference:

- warm science-themed yellow/green/peach background;
- large centred `KUIZ` heading;
- dynamic `Soalan X/10` progress pill;
- `SP 3.1 | SK 3.1.4` curriculum pill;
- large cream question card;
- three full-width A/B/C answer cards;
- a prominent green `PILIH JAWAPAN` action button;
- friendly microorganism/science decorations around the safe content area;
- consistent rounded, tactile, child-friendly visual language used by the other Activity 8 redesigns.

The target is close visual fidelity, not a single flattened screenshot. Question text, answer state, score, buttons, audio, and completion UI must remain live Unity controls.

## Current Scene Audit

- Active scene: `Assets/Scenes/Aktivity/akt8/kuiz8.unity`.
- Canvas reference resolution: `1920 × 1080`, `Scale With Screen Size`.
- Existing logic:
  - `QuizController` is attached to `Canvas/Content`.
  - `ScoreController` and `SceneController` are attached to `Canvas`.
  - `ScoreInit` controls the existing completion panel.
  - Wrong/correct/yay sound sources already exist under `Sounds`.
- Quiz data currently contains 10 entries and each entry has three answer choices.
- `QuizController.buttonAnswers` still has four button references. The fourth will remain serialized but hidden when a question has only three choices.
- Existing `QuizController` immediately validates an answer-card click. The reference instead shows a separate `PILIH JAWAPAN` confirmation button, so the interaction requires a small controlled extension.
- Content warning: the ten serialized `question` fields are currently null and all ten `correctIndex` values are `0`. The visual redesign must not invent or silently overwrite curriculum content. Question prompts and answer keys should be corrected only from an approved Activity 8 source/module.
- There is no dedicated Activity 8 quiz narration audio in `Assets/Audio` at present.

## Visual Specification

### Global art direction

- Use a 16:9 warm gradient: pale lime at the lower-left, soft yellow through the centre, and peach at the right.
- Add subtle translucent bubbles, DNA/atom line art, stars, and small circular particles.
- Keep all decorative images non-raycastable and outside the primary reading path.
- Use the existing `FredokaOne SDF` family for headings and rounded readable TMP text for questions/answers.
- Primary text: deep green (`#07501C` approximately).
- Primary action: layered green with a darker lower edge and soft shadow.
- Cards: warm cream/white with a thin pale-gold outline and restrained shadow.
- Correct state: bright soft green; wrong state: coral/red; selected-but-not-submitted state: lime outline and light green wash.

### Reference layout at 1920×1080

| Region | Planned placement | Behaviour |
|---|---:|---|
| Audio control | top-left, approximately `350 × 90` | Speaker icon plus `Dengar audio`; plays the current question narration when a clip exists. |
| Question progress | top-centre, approximately `285 × 62` | Dynamic `Soalan 1/10` through `Soalan 10/10`. |
| Curriculum badge | top-right, approximately `390 × 62` | Static `SP 3.1 | SK 3.1.4`. |
| Main title | centred near y `160`, approximately `470 × 135` | Large green `KUIZ` with a cream outline/shadow and decorative stars. |
| Question card | centred near y `330`, approximately `1080 × 170` | Dynamic Malay question text, up to two lines. |
| Answer A | centred near y `535`, approximately `1120 × 130` | Letter badge, answer text, optional contextual icon. |
| Answer B | centred near y `690`, approximately `1120 × 130` | Same structure; selected-state sample matches the reference. |
| Answer C | centred near y `845`, approximately `1120 × 130` | Same structure. |
| Confirm/next action | bottom-centre, approximately `760 × 105` | `PILIH JAWAPAN`, then `SETERUSNYA` after a correct answer. |

The central reading column will remain clear at 16:9, 16:10, and common tablet aspect ratios. Decorations can crop at the edges; question and answer controls cannot.

## Planned Hierarchy

```text
Canvas
├── Kuiz8Backdrop
│   ├── GradientBackground
│   ├── LeftDecor
│   ├── RightDecor
│   └── AmbientDecor
├── Kuiz8Header
│   ├── AudioButton
│   ├── ProgressPill
│   ├── CurriculumPill
│   ├── QuizTitle
│   └── TitleStars
├── Content                         (retain QuizController)
│   └── QuizCardRoot
│       ├── QuestionCard
│       │   └── QuestionLabel       (retain questionText reference)
│       ├── AnswerCards
│       │   ├── AnswerA             (retain buttonAnswers[0])
│       │   ├── AnswerB             (retain buttonAnswers[1])
│       │   ├── AnswerC             (retain buttonAnswers[2])
│       │   └── AnswerD             (retained, inactive unless required)
│       └── SubmitButton
├── WinPanel                        (retain ScoreInit)
│   └── RedesignedResultCard
├── Back                            (retain navigation component/callback)
└── EventSystem                     (unchanged)
```

Existing controller references will be reassigned through serialized properties rather than replaced with fragile `GameObject.Find` dependencies.

## Assets

### Reuse where suitable

- `Assets/Images/UI Flat/char_green_microbe.png`
- `Assets/Images/UI Flat/char_purple_microbe.png`
- `Assets/Images/UI Flat/char_blue_microbe.png`
- `Assets/Images/UI Flat/char_water_drop.png`
- `Assets/Images/UI Flat/icon_flask.png`
- `Assets/Images/UI Flat/img_microscope_large.png`
- `Assets/Images/UI Flat/img_petri_bakteria.png`
- existing sliced white/green button sprites and TMP fonts

### Planned new assets

Create `Assets/Images/Akt8QuizUI/` with:

- `bg_kuiz8_science_1920x1080.png` — clean warm backdrop with no embedded text or controls;
- transparent decorative sprites only when the existing project art cannot reproduce the reference cleanly;
- optional choice illustrations grouped by question under `ChoiceIcons/Q01` through `ChoiceIcons/Q10`;
- QA captures under `Assets/Images/Akt8QuizUI/QA/`.

Generated art must not contain text. Text stays in TMP so it remains editable, localizable, crisp, and dynamic.

For the reference question about microorganism growth, the icon slots should support light, temperature, air, nutrients/petri dish, and food-colour illustrations. For other questions, icon slots may be hidden if an approved matching asset is unavailable.

## Script and Builder Work

### 1. Add an idempotent editor builder

Create `Assets/Editor/Kuiz8ReferenceUIBuilder.cs` with a menu action such as:

`MicroFun/Activity 8/Build Kuiz 8 Reference UI`

Responsibilities:

- verify the active scene is `kuiz8`;
- create a timestamped scene backup before the first mutation;
- remove only a previous generated `Kuiz8Backdrop`/`Kuiz8Header` root, never unrelated user objects;
- preserve `QuizController`, `ScoreController`, `ScoreInit`, `SceneController`, sound objects, and existing button references;
- normalize the Canvas scaler to 1920×1080 and match width/height at `0.5` for balanced resizing;
- remove conflicting layout components only from the legacy quiz-content containers;
- construct the new hierarchy and assign all serialized references;
- make every decorative image `raycastTarget = false`;
- configure nine-sliced card/button sprites, TMP styling, shadows, and interaction feedback;
- rebuild safely when run again without duplicating objects or listeners;
- mark and save the scene only after reference validation succeeds.

Do not apply the older generic `QuizReferenceLayoutEditorBuilder` directly to `kuiz8`; its assumptions and object paths do not fully match this scene.

### 2. Extend `QuizController` without replacing its scoring model

Add a two-step selection flow:

1. `SelectPendingAnswer(int index)` highlights a choice but does not score it.
2. `SubmitPendingAnswer()` validates the selected choice when `PILIH JAWAPAN` is pressed.
3. A wrong choice flashes coral/red, plays the existing wrong sound, and remains retryable.
4. A correct choice turns green, locks all answer cards, increments the score once, plays the correct sound, and changes the action label to `SETERUSNYA`.
5. Pressing the action again loads the next question; after question 10 it opens the result panel.

Add serialized references for:

- submit/next button;
- submit button label;
- optional question narration source/button;
- optional answer icon images.

Keep the existing `SelectAnswer` API as a compatibility wrapper where practical so other quiz scenes are not broken by the Activity 8 redesign.

### 3. Optional presentation data

Add optional per-question presentation fields without changing the existing choice strings:

- `AudioClip narrationClip`;
- `Sprite questionIllustration`;
- `List<Sprite> choiceIcons`.

Every field must be nullable. Missing media hides its visual slot and must never block the quiz.

### 4. Audio button

- If approved narration clips are supplied/generated, `Dengar audio` plays the current question clip and prevents overlapping playback.
- If no Activity 8 narration exists, keep the button visually present but clearly disabled rather than reusing unrelated Activity 1 audio.
- Correct/wrong/yay effects remain separate and unchanged.

### 5. Completion panel

Restyle `WinPanel` to the same visual system:

- `Tahniah!` heading;
- percentage score from the existing scoring flow;
- primary `Kembali`/exit action using the existing callback;
- optional `Ulang Kuiz` action that safely reloads `kuiz8` and resets score state.

## Content Integrity Pass

Visual work and curriculum work must remain separate.

- Preserve the current ten choice lists during the UI build.
- Do not assume answer A is correct simply because all current indices are zero.
- Obtain the ten approved Malay question prompts and answer key from the existing Activity 8 module/source.
- Populate `question` and `correctIndex` only after that source is identified.
- Verify the reference sample question displays as `Soalan 5/10` and that its approved correct answer is highlighted only after submission.

## Responsive and Accessibility Requirements

- Minimum answer-card hit area: 110 px high at the 1920×1080 reference.
- All controls support mouse and touch.
- Selected, correct, wrong, and disabled states must differ by more than colour alone: outline, icon/check/cross, and interaction lock are also used.
- TMP auto-sizing uses controlled minimums to prevent tiny text.
- Long Malay questions may use two lines; long answers may wrap to two lines without overlapping their badges/icons.
- Decorative sprites never intercept input.
- The visible focus order follows A → B → C → submit.
- Safe-area handling is included for mobile/tablet displays.

## Implementation Phases and Codex Sol Effort

Recommended default: **`gpt-5.6-sol` with `high` reasoning effort**.

| Phase | Work | Suggested effort |
|---|---|---|
| 0. Safety and baseline | Backup `kuiz8`, record hierarchy/references, capture baseline, verify console | Medium |
| 1. Visual asset preparation | Reuse/import/generate background and transparent decorations; set sprite import metadata | High |
| 2. Idempotent UI builder | Build the hierarchy, layout, cards, headers, responsive anchors, and result panel | High |
| 3. Interaction extension | Pending selection, submit/next state machine, feedback, audio hook, compatibility wrapper | High |
| 4. Curriculum data audit | Resolve null prompts and verify answer keys from an approved source | High; use `xhigh` only if the source material is ambiguous |
| 5. QA and polish | Compile checks, scene validation, Play Mode interaction tests, resolution captures, final alignment pass | High |

Use **`xhigh`** for the full execution only if all ten question-specific illustrations and Malay narration clips are also expected in the same pass. The UI-only redesign does not require `xhigh`.

## Verification Plan

1. Compile with zero new C# errors.
2. Validate `kuiz8` for missing scripts, null controller references, broken sprites, and duplicate generated roots.
3. Run all ten questions in Play Mode.
4. Confirm progress reads `Soalan 1/10` through `Soalan 10/10`.
5. Confirm selecting a card does not score before `PILIH JAWAPAN`.
6. Confirm a wrong submit does not increment score and remains retryable.
7. Confirm a correct submit increments score exactly once and enables `SETERUSNYA`.
8. Confirm the fourth legacy answer reference remains safe/inactive for three-choice questions.
9. Confirm the result panel percentage matches the final score.
10. Confirm back/exit navigation still works.
11. Test 1920×1080, 2560×1440, 1366×768, and a tall tablet aspect ratio.
12. Capture final comparison screenshots and perform a visual pass against the supplied reference.

## Acceptance Criteria

- The Game view closely matches the supplied Kuiz reference at 1920×1080.
- Header, question, A/B/C answers, and action button are real dynamic UI, not baked into the background.
- No decorative element blocks input or overlaps readable content.
- All ten questions can be completed without exceptions or stuck states.
- Existing score and sound effects still work.
- The scene contains no missing scripts or broken serialized references.
- Re-running the builder produces the same hierarchy without duplicates.
- No unrelated scenes or shared quiz behaviours are regressed.

## Files Expected to Change During Execution

- `Assets/Scenes/Aktivity/akt8/kuiz8.unity`
- `Assets/Editor/Kuiz8ReferenceUIBuilder.cs` (new)
- `Assets/Scripts/QuizController.cs` (compatible extension)
- optional small presentation-data/helper script if required
- `Assets/Images/Akt8QuizUI/` (new generated/reused UI assets and QA captures)
- optional `Assets/Audio/Akt8Quiz/` only if approved narration is included

## Explicit Non-Goals

- Do not change Build Settings.
- Do not alter other quiz scenes to match Kuiz 8.
- Do not replace the existing score system wholesale.
- Do not fabricate curriculum prompts, correct answers, or narration text.
- Do not bake live question/answer text into generated images.

