# Kuiz 7 Reference Redesign — Implementation Plan

## Status

Planning only. This document does not authorize or perform any Unity scene, script, prefab, build-settings, or image-asset changes.

## Goal

Redesign `Assets/Scenes/Aktivity/akt7/kuiz7.unity` to closely match the supplied reference while preserving the scene's existing quiz data, scoring, navigation, audio, question progression, and result flow.

The target is a cheerful 16:9 science-learning interface with:

- a warm cream/yellow/peach background;
- a top-left `Dengar audio` control;
- a live `Soalan 1/10` progress badge;
- a top-right `SP 3.1 | SK 3.1.4` badge;
- a large central `KUIZ` heading;
- one cream question card;
- three answer cards with circular `A`, `B`, and `C` markers;
- a large green `PILIH JAWAPAN` confirmation button;
- a small back button at bottom-left; and
- Activity 7 science decorations around, but never over, the playable controls.

## Reference Interpretation

The attached screenshot is a visual target only. Placeholder text such as `SOALAN APPEAR DI SINI` and `Button` will not be copied into the finished scene. The live question and answer strings already supplied by Kuiz 7 will populate those areas.

The screenshot appears to be an in-Editor capture with temporary or overlapping cut-out assets. The finished implementation will retain its overall composition while cleaning up transparency, layering, cropping, and consistency.

## Implementation Approach

### 1. Safety backup and scene audit

Before changing the scene during the approved execution phase:

1. Open `kuiz7.unity` and inspect its hierarchy, canvases, quiz controller, question source, audio source, answer buttons, submit button, feedback/result panels, and navigation callbacks.
2. Record every persistent button callback and every serialized controller reference.
3. Create a recoverable backup:
   `Assets/Scenes/Aktivity/akt7/kuiz7_PreReferenceRedesign_20260831.unity`.
4. Confirm the actual question count and preserve that value as the progress denominator. The design shows `/10`, but the UI must use the real dataset count.

No quiz logic will be rewritten merely to support the new appearance.

### 2. Create an idempotent editor builder

Add:

- `Assets/Editor/Kuiz7ReferenceUIBuilder.cs`

The builder will expose a Unity menu command such as:

- `Tools/MicroFun/Activity 7/Build Kuiz 7 Reference UI`

It will:

- require `kuiz7.unity` to be the active scene;
- create or rebuild a single `Kuiz7ReferenceRoot` beneath the scene canvas;
- find and preserve the current functional quiz controls;
- restyle or reparent them where safe;
- reconnect serialized references and callbacks explicitly;
- avoid duplicate roots, listeners, EventSystems, and AudioListeners when run again; and
- mark and save only the intended scene and newly created assets.

The new root will be isolated so the redesign can be removed or rebuilt without damaging the underlying quiz controller.

### 3. Proposed hierarchy

```text
Canvas
└── Kuiz7ReferenceRoot
    ├── Background
    │   ├── GradientBackdrop
    │   ├── SoftShapes
    │   └── ScienceDecorations
    ├── Header
    │   ├── AudioButton
    │   ├── ProgressPill
    │   ├── CurriculumPill
    │   └── QuizTitle
    ├── QuizContent
    │   ├── QuestionCard
    │   │   └── QuestionText
    │   ├── AnswerA
    │   │   ├── LetterBadge
    │   │   └── AnswerText
    │   ├── AnswerB
    │   │   ├── LetterBadge
    │   │   └── AnswerText
    │   └── AnswerC
    │       ├── LetterBadge
    │       └── AnswerText
    ├── Footer
    │   ├── BackButton
    │   └── ConfirmButton
    └── FeedbackLayer
        ├── CorrectFeedback
        ├── IncorrectFeedback
        └── ResultPanel
```

Existing feedback/result objects may remain outside this root if moving them would risk breaking their controller references. Their visuals can still be restyled in place.

## Visual Specification

### Canvas and scaling

- Reference resolution: `1920 × 1080`.
- `Canvas Scaler`: Scale With Screen Size.
- Match width/height: approximately `0.5`.
- Safe outer margin: at least 32 reference pixels.
- The layout must remain usable at 16:9, 16:10, and 4:3 without answer text colliding with decorations.

### Background

- Full-screen cream-to-yellow-to-peach gradient.
- Pale green tint on the left and pale orange tint on the right.
- Subtle bubbles, atom, DNA, sparkles, microbes, microscope, petri dish, water-drop character, and magnifying-glass accents.
- Decorations are non-interactive, have `Raycast Target` disabled, and stay below the main UI.
- Character art will be placed to frame the quiz rather than sit behind question or answer text.

### Header

- Audio control: white/cream rounded pill with dark-green border and circular green speaker badge.
- Progress badge: cream rounded pill with `Soalan X/Y`, bound to the live question index and real total.
- Curriculum badge: cream rounded pill showing `SP 3.1 | SK 3.1.4`.
- Title: `KUIZ`, centered, dark green with a light outline/shadow treatment.

### Question card

- Large centered cream card below the title.
- Dark charcoal-green outline, soft lower shadow, and generous internal padding.
- Question text uses the existing TextMeshPro object or a replacement bound to the same controller field.
- Auto-size will be constrained so long Malay questions wrap cleanly without shrinking below an accessible minimum.

### Answer cards

- Three full-width cream bars stacked vertically.
- Large circular green letter markers on the left.
- Answer text remains left aligned and vertically centered.
- The complete row is clickable, not only the letter badge or label.
- Minimum target height will remain comfortable for touch input.

State styling:

| State | Card | Letter badge | Interaction |
|---|---|---|---|
| Normal | Cream | Green | Enabled |
| Hover/focus | Warm pale green | Brighter green | Enabled |
| Selected | Pale green with stronger green outline | Bright green/check accent | Enabled |
| Correct | Green success treatment | Check mark | Locked after submission |
| Incorrect | Soft coral/red treatment | Cross accent | Locked after submission |
| Disabled | Reduced saturation | Muted | Disabled |

Selection styling will be visual only until the existing quiz controller confirms the answer.

### Confirmation and back controls

- `PILIH JAWAPAN` is a large green rounded button centered near the bottom with an orange forward arrow.
- It is disabled until an answer is selected, unless the current controller intentionally handles empty submissions differently.
- Existing submit/next behavior must remain intact.
- The bottom-left back control preserves the scene's current navigation destination and callback.

### Typography

- Reuse `Assets/Fonts/FredokaOne SDF.asset` where appropriate for the rounded educational style.
- Dark green body text; white button labels.
- TextMeshPro materials may add restrained outline/shadow, but readability takes priority over matching the screenshot's heavy effect.

## Asset Strategy

Create or consolidate UI art under:

- `Assets/Images/Akt7Kuiz7UI/`

Priority order during execution:

1. Reuse compatible transparent Activity 7 artwork already created for the Activity 7 menu, Pengenalan, and Keburukan scenes.
2. Reuse generic panels, shadows, borders, arrows, speaker icons, and button pieces from `Assets/Images/UI Flat/` where they visually fit.
3. Generate only missing text-free decorative art or a clean background plate.
4. Do not generate text inside images; all labels remain editable TextMeshPro content.

Every newly imported PNG will be configured as a UI Sprite with alpha transparency, appropriate pixels-per-unit, no mipmaps where unnecessary, and compression settings that avoid visible halos.

## Functional Integration

The redesign must preserve:

- the existing Kuiz 7 question collection and answer order;
- current correct-answer values and scoring;
- question-index advancement;
- question progress display;
- answer selection and submit behavior;
- correct/incorrect feedback timing;
- final score/result presentation;
- replay, return, and next-scene actions;
- back-button behavior;
- audio playback and mute/replay behavior, if a real clip is assigned; and
- any PlayerPrefs or completion-state updates already used by the project.

Preferred integration method:

- Reuse the current live `Button` and TMP components and restyle/reparent them.

Fallback method:

- Keep the source controls active but visually hidden, then connect visible presentation buttons through a small relay component. This will only be used if the controller depends on fixed references that cannot safely be reassigned.

The audio button will not pretend to work. If the scene has no assigned narration clip, it will be visibly disabled or omitted until audio is provided.

## Implementation Phases and Codex Sol Effort

| Phase | Work | Recommended effort |
|---|---|---|
| 1. Audit | Inspect hierarchy, scripts, serialized references, callbacks, question count, and result flow | `gpt-5.6-sol` — High |
| 2. Art preparation | Reuse existing artwork; generate or clean only missing transparent decorations/background | `gpt-5.6-sol` — High |
| 3. UI builder | Implement idempotent builder, layout, styling, and responsive anchors | `gpt-5.6-sol` — High |
| 4. Logic binding | Reconnect question, answer, progress, audio, submit, feedback, result, and back controls | `gpt-5.6-sol` — High |
| 5. QA and polish | Compile, Play Mode test all questions, screenshot comparisons, and resolution checks | `gpt-5.6-sol` — High |

Use `xhigh` only if the execution phase requires several new custom art generations or the current quiz controller is unusually coupled to the old hierarchy. High effort should otherwise be sufficient.

## Verification Checklist

### Editor validation

- No compiler errors or new warnings.
- Scene contains one EventSystem and the intended canvas configuration.
- Builder can be run twice without duplication or lost callbacks.
- No missing scripts, sprites, TMP fonts, or serialized references.
- All decorative images have raycast disabled.

### Play Mode validation

- Audio button behaves exactly as its assigned audio allows.
- Progress starts at `Soalan 1/Y` and updates once per question.
- A, B, and C rows are fully clickable.
- Selecting one answer clearly deselects the others.
- Confirmation cannot accidentally submit twice.
- Correct and incorrect feedback use the right answer state.
- The quiz advances through the complete dataset.
- Final result, completion state, replay/return actions, and scene navigation still work.
- Back button returns to the existing intended Activity 7 destination.

### Visual validation

- Capture Game View at `1920 × 1080` and compare against the reference.
- Check 1600×900, 1366×768, 1280×720, 1920×1200, and 1024×768.
- No text clipping, icon background boxes, alpha fringes, or decorative overlap.
- Question and answer text remain readable at normal classroom viewing distance.
- The confirmation button remains visible without covering answer C.

## Acceptance Criteria

The work is complete when:

1. Kuiz 7 closely matches the supplied composition and color language.
2. Live questions and answers replace all screenshot placeholders.
3. Existing quiz behavior and data are preserved end to end.
4. All buttons have clear normal, selected, pressed, and disabled states.
5. The UI is usable at the supported aspect ratios.
6. The scene runs with no new console errors.
7. The redesign is reproducible through the idempotent builder.
8. The original scene remains recoverable from the dated backup.

## Out of Scope

- Rewriting quiz questions or answer content.
- Changing correct answers or scoring rules.
- Adding narration that has not been supplied.
- Changing Build Settings or unrelated scenes.
- Replacing the existing result flow unless it is broken and separately approved.

## Expected Deliverables After Approval

- Updated `Assets/Scenes/Aktivity/akt7/kuiz7.unity`.
- Backup scene at `Assets/Scenes/Aktivity/akt7/kuiz7_PreReferenceRedesign_20260831.unity`.
- `Assets/Editor/Kuiz7ReferenceUIBuilder.cs`.
- Any required assets under `Assets/Images/Akt7Kuiz7UI/`.
- Final 1920×1080 Game View screenshot for review.
- A short validation report covering callbacks, all questions, feedback, result flow, navigation, and tested resolutions.
