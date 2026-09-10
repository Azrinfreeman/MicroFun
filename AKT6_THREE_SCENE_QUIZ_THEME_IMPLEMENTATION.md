# Activity 6 — Three-Scene Quiz Theme Implementation

## Status

Implemented on 2026-09-01. The three quiz scenes were redesigned, backed up, runtime-checked, and validated through Unity MCP.

## Goal

Give the three Activity 6 quiz scenes the same polished, child-friendly science theme already established across the rebuilt Activity 6 Center, Pengenalan, Kegunaan, and detail pages:

1. `Assets/Scenes/Aktivity/akt6/SceneV1.unity`
2. `Assets/Scenes/Aktivity/akt6/SceneV2.unity`
3. `Assets/Scenes/Aktivity/akt6/SceneV3.unity`

The scenes will look and behave like consecutive pages of one quiz rather than three unrelated scenes.

## Confirmed Existing Structure

- All three scene assets exist under `Assets/Scenes/Aktivity/akt6/`.
- `SceneV1` contains:
  - `QuizManager` with `QuizManager` and `SceneLoader` components.
  - `Canvas/QuizPanel/QuestionText`.
  - Optional `QuestionImage`.
  - Four runtime-managed answer buttons: A, B, C, and D.
  - `FeedbackText`, `ScoreText`, `NextButton`, and `FadePanel`.
- The answer buttons do not rely on persistent Inspector callbacks; the quiz manager wires them at runtime. The existing Button components and serialized references must therefore be retained.
- SceneV1 currently contains one question and transitions to `SceneV2` on completion.
- The remaining scene content, final completion destination, score persistence, and any Back action will be fully audited before the visual mutation begins.

## Shared Visual Direction

All three scenes will share one 1920 × 1080 visual system:

- Soft lime-green to cream to peach-orange science background.
- Friendly green and purple microorganism mascots at the outer edges.
- Bubbles, leaves, sparkles, atom/DNA line art, microscope, and laboratory props.
- Fredoka One typography matching the rebuilt Activity 6 pages.
- Green `AKTIVITI 6` badge.
- Large dark-green `KUIZ` title with white outline and soft shadow.
- Top-centre progress badge:
  - SceneV1: `Soalan 1/3`
  - SceneV2: `Soalan 2/3`
  - SceneV3: `Soalan 3/3`
- Top-right curriculum badge: `SP 3.1 | SK 3.1.4`, subject to confirmation from the existing lesson data.
- Large cream question card with a coloured border.
- Optional framed question illustration without changing the current question-image logic.
- Four large answer cards in a balanced 2 × 2 layout.
- Circular A/B/C/D medallions on the answer cards.
- A prominent green Next/Continue control using the existing `NextButton`.
- Visible Back button in the top-left.
- Consistent hover, selected, correct, incorrect, and disabled states.

## Scene-to-Scene Identity

The layout remains identical across all three scenes. Only the progress badge and a small accent shift distinguish the stages:

| Scene | Progress | Accent |
|---|---:|---|
| SceneV1 | 1 / 3 | Sky blue |
| SceneV2 | 2 / 3 | Teal |
| SceneV3 | 3 / 3 | Warm purple |

Correct answers remain green and incorrect answers remain red regardless of scene accent.

## Proposed Hierarchy

The builder will add a generated visual root behind the functional quiz controls:

```text
Canvas
├── Akt6QuizThemeRoot
│   ├── Background
│   ├── ActivityBadge
│   ├── ProgressBadge
│   ├── CurriculumBadge
│   ├── QuizTitle
│   ├── QuestionCard
│   ├── AnswerAreaDecoration
│   ├── FeedbackPanel
│   └── BackButton
└── QuizPanel
    ├── QuestionText
    ├── QuestionImage
    ├── AnswerButtonsPanel
    │   ├── AnswerButton_A
    │   ├── AnswerButton_B
    │   ├── AnswerButton_C
    │   └── AnswerButton_D
    ├── FeedbackText
    ├── ScoreText
    ├── NextButton
    └── FadePanel
```

`QuizPanel` and its referenced controls will be restyled and repositioned in place. They will not be replaced, ensuring the existing runtime listener setup remains valid.

## Answer Button Design

Each answer card will include:

- White/cream rounded panel.
- Scene-accent border and soft shadow.
- A coloured circular letter medallion.
- Large editable TextMeshPro answer text.
- Full-card hit area.
- Hover highlight and pressed scale/tint feedback.
- Correct state: green border, pale-green fill, and check indicator.
- Incorrect state: red border, pale-red fill, and cross indicator.
- Disabled state after a selection, matching current quiz-manager behaviour.

The existing `correctColor`, `wrongColor`, and `defaultColor` values will be aligned with the new palette rather than bypassed.

## Question and Feedback Behaviour

- Existing question copy, options, correct-answer indices, images, and feedback strings remain serialized on `QuizManager`.
- Question text remains editable TextMeshPro content and will not be baked into artwork.
- The question card supports long Malay sentences with auto-sizing and safe margins.
- If `QuestionImage` has no sprite, the question card expands into the available space without leaving an empty frame.
- Correct/wrong feedback appears inside a dedicated rounded panel.
- `NextButton` remains hidden or disabled until the current quiz logic permits it.
- Scene transitions and score persistence remain controlled by the existing managers.

## Navigation

Expected progression:

```text
Activity 6 Center → SceneV1 → SceneV2 → SceneV3 → Activity 6
```

Before implementation:

- Confirm SceneV2 and SceneV3 `sceneToLoadOnComplete` values.
- Confirm whether `_GameManager` persists the score between scenes.
- Confirm the intended landing page after SceneV3.
- Confirm whether a Back action already exists.

Implementation rules:

- SceneV1 Back returns to the Activity 6 Center.
- SceneV2 Back returns to SceneV1 unless the existing design specifies Activity 6 Center.
- SceneV3 Back returns to SceneV2 unless the existing design specifies Activity 6 Center.
- The final destination will use the existing serialized transition when valid.
- No raw scene-name string will be changed until the target is confirmed in Build Settings.

## Artwork

Create or reuse one clean Activity 6 quiz background with:

- No embedded words, numbers, signs, buttons, or watermark.
- A clear central area for live Unity UI.
- Decorative characters confined to the outer edges.
- 16:9 composition and sufficient contrast for dark-green text.

Planned folder:

```text
Assets/Images/Akt6QuizUI/
├── Background/
├── Icons/
└── Shared/
```

Existing Activity 6 mascots, bubbles, button sprites, Fredoka font, and laboratory props should be reused where practical. New raster assets will only be generated when an existing asset cannot match the established theme.

## Builder Architecture

Create:

`Assets/Editor/Akt6ThreeSceneQuizUIBuilder.cs`

Suggested menu:

`Tools/MicroFun/Activity 6/Build Three-Scene Quiz Theme`

Builder responsibilities:

- Audit and validate all three scene paths before mutation.
- Create timestamped backups of SceneV1, SceneV2, and SceneV3.
- Open each scene one at a time in the Editor.
- Create or rebuild `Akt6QuizThemeRoot` idempotently.
- Restyle the existing quiz controls without breaking serialized references.
- Apply the shared CanvasScaler reference resolution of 1920 × 1080.
- Assign scene-specific progress/accent configuration.
- Preserve `QuizManager`, `SceneLoader`, `GameManager`, answer arrays, question data, score data, and transition strings.
- Save only after validation succeeds.
- Restore the originally active scene when the batch finishes.

Use a small configuration structure:

```text
Scene path
Progress label
Accent colour
Back destination
Expected completion destination
```

No runtime adapter should be added unless testing proves the existing manager cannot expose a required visual state.

## Implementation Phases and Codex Sol Effort

| Phase | Work | Codex Sol effort |
|---|---|---|
| 1. Three-scene audit | Inspect hierarchies, question data, scene transitions, score persistence, and Build Settings | High |
| 2. Shared art preparation | Reuse or generate the clean quiz background and supporting icons | High |
| 3. Builder implementation | Build the shared visual root and restyle all referenced quiz controls | High |
| 4. Navigation integration | Add Back actions and verify SceneV1 → V2 → V3 → final destination | XHigh |
| 5. Quiz behaviour testing | Test every answer, correct/wrong feedback, Next state, score, and completion transition | XHigh |
| 6. Visual QA | Check 16:9 scaling, long text, optional images, button states, and screenshots | High |

Recommended overall setting: **Codex Sol — High**. Use **XHigh** during navigation and score-persistence testing because the quiz is split across three independent scenes.

## Verification Checklist

- SceneV1, SceneV2, and SceneV3 visibly belong to the same Activity 6 quiz.
- Progress badges correctly show 1/3, 2/3, and 3/3.
- Question and option text remain editable.
- All four answer buttons remain linked to the correct `QuizManager` array entries.
- Correct and incorrect states display clearly.
- Feedback text remains readable and does not overlap the question or controls.
- Next becomes available at the correct time.
- Score persists correctly across all three scenes.
- SceneV1 transitions to SceneV2.
- SceneV2 transitions to SceneV3.
- SceneV3 reaches the confirmed final destination.
- Back navigation works from each scene.
- No old UI leaks through the redesigned presentation.
- No important content is clipped at 1920 × 1080 or common 16:9 resolutions.
- No missing scripts, broken references, or new console errors.
- All three scenes pass Unity scene validation.

## Out of Scope

- Rewriting quiz questions or changing correct answers.
- Combining the three scenes into one scene.
- Replacing `QuizManager`, `SceneLoader`, or score logic unless a verified defect requires it.
- Adding new questions without a separate request.
- Changing other Activity 6 pages.

## Execution Gate

Implementation begins only after explicit approval to proceed. The first execution step will audit SceneV2, SceneV3, Build Settings, transitions, and score persistence, then create backups before any visual changes.
