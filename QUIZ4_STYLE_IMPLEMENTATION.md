# Quiz4 — UI theme implementation plan

Status: IMPLEMENTED AND VERIFIED — 7 September 2026, after approval.  
Target: `Assets/Scenes/Aktivity/akt4/quiz4.unity`  
Scope: match the completed AKT4/Microscene4 visual theme; preserve quiz mechanics.

## 1. Verified baseline

Read-only Unity MCP inspection found quiz4 active in Edit Mode, with no unsaved changes. All QuizManager4 object references are assigned.

Existing flow:

**Start → 3 multiple-choice questions → drag-and-drop → reflection → result**

| Screen | Existing function | Proposed presentation |
|---|---|---|
| StartPanel | MULA starts the quiz | AKTIVITI 4 badge, outlined MICRO-FUN QUIZ title, friendly microbe artwork, large green MULA button |
| QuizPanel | Question image, question, three answers, feedback | Cream question card, blue image frame, three equally styled blue answer cards, reserved feedback area |
| DragDropPanel | Water/Rock/Plastic chips and one slot | Cream instruction card, blue drop zone, rounded factor chips, green/orange accents |
| ReflectionPanel | Three reflection choices lead to results | Rounded cream card with three equally weighted illustrated emotion choices |
| ResultPanel | Score, Restart and existing Back | Large score card, green RESTART button, navy KEMBALI button |

Use the existing panels and state transitions. Do not combine stages, add a next button, introduce a timer, or add scoring features.

## 2. Theme and layout

- Reuse the pale lime/cream/peach background, navy outlined headings, green primary actions, blue/orange accents, white edging and rounded cards from the completed UI.
- Keep decorative microbes and bubbles around the margins, away from question images and interaction areas.
- Use FredokaOne for headings and short controls; use the existing readable body font for long English questions and answers.
- Keep all three multiple-choice answers visually neutral and equally prominent; colours must not reveal correctness.
- Preserve question images and their meaning. Frame them with aspect ratio maintained; do not replace them with decorative microbes or generic generated art.
- Keep current question/answer language, wording and reflection values. This is not a translation pass.
- Correct the Back control's misleading visible RESTART label to KEMBALI; keep its existing object and navigation callback.
- Remove only confirmed unused placeholder text such as the reflection panel's static “New Text”; do not remove manager-owned feedback fields.

Use the existing 1920×1080 Canvas reference, width matching (match = 0), and no extra scaled parent around interactive content.

Suggested 1920×1080 layout:
- Header/margins: top 180–210 pixels, shared visual treatment across all five screens.
- Main content: roughly 1680×680 centred card.
- Multiple choice: image area on the left, question and three wide stacked answers on the right. Reserve a separate feedback line below.
- Drag stage: instruction above the target; three source chips below with enough room for long labels.
- Reflection: prompt above three equal choice cards.
- Result: score and two separate actions centrally aligned.

Aim for 30–36 design pixels for body text and at least 60-pixel-high controls. Test the longest answer (“Because microorganisms grow on it”) without truncation. Reduce decorations before reducing readability.

## 3. Mechanics that must remain unchanged

Relevant sources:
- `Assets/Scripts/QuizManager4.cs`
- `Assets/Scripts/DraggableItem.cs`
- `Assets/Scripts/DropSlot4_4.cs`
- `Assets/Scripts/BackController.cs`

### Multiple choice

- Three fixed questions, in the current order; three answers each.
- Correct answer index is 0 for all three. Do not reorder answers or randomise them.
- One point for each correct answer.
- All answer buttons become non-interactable after a selection.
- Existing correct/incorrect feedback and 1.2-second automatic advance remain.
- QuizManager4 replaces runtime answer listeners when displaying each question; retain the referenced buttons instead of adding parallel answer handlers.

### Drag-and-drop

- Exact item IDs remain `Water`, `Rock`, `Plastic`.
- The slot expects `Water`.
- Incorrect drops return to the starting position and allow another attempt; they do not advance the quiz.
- Correct Water adds one point, stays in the slot, locks the slot and schedules reflection after 1.2 seconds.
- Keep the CanvasGroups, raycast behaviour, source parents and slot references.
- DraggableItem caches its starting position/parent in Start, reparents to its Canvas when dragged, and divides movement by canvas.scaleFactor. Author the layout before runtime; do not introduce nested scaling or late runtime repositioning.
- Decorations must use raycastTarget=false. Retain a hittable slot Image; its instruction must not obscure an accepted chip.

### Reflection, result and navigation

- Preserve the three callback strings: “I understand well”, “I understand a little”, “I need more practice”.
- Reflection is unscored and opens results.
- Result score denominator remains 4; no percentage, stars, pass threshold or extra reward.
- RESTART continues calling RestartQuiz/StartQuiz.
- Existing Back is `Canvas/ResultPanel/RestartButton (1)`, with BackController and standalone fallback `akt1`.
- Back remains available on the result screen only. Adding exit controls to earlier stages would change navigation scope and is not included.
- Preserve additive return behaviour and runtime Back listener registration.

## 4. Existing issues to keep separate

Source inspection indicates that RestartQuiz resets score/question index but does not reset DropSlot4_4.alreadyAnswered or restore the accepted draggable item. A second run may become stuck at the drag stage. This has not been runtime-tested during planning.

After approval, reproduce this in the baseline and report it separately. Do not silently repair it in a presentation-only pass; a gameplay fix needs separate approval.

Other intentional preservation:
- Mixed English/Malay text remains.
- All correct MCQ answers remain first.
- The existing image assignments remain, even where their filenames may look surprising.
- Result wording remains the current wording, regardless of score.

## 5. Assets and implementation boundaries

Reuse without modifying shared assets:
- `Assets/Images/Akt4KehadiranAirUI/air_warm_gradient.asset`
- `Assets/Images/Akt4KehadiranAirUI/air_rounded.asset`
- Existing `Assets/Images/UI Flat/` microbes and quiz/checklist icons.
- Existing bubble artwork and fonts.

No new generated artwork is expected. Create quiz-specific materials only if needed; do not alter shared heading materials/fonts or previously finished scenes.

After approval, add a scene-guarded, rerunnable editor builder such as `Assets/Editor/Quiz4StyleBuilder.cs`. Style existing functional objects in place and add named decorative roots. Never duplicate gameplay listeners or remove gameplay components.

## 6. Efficient execution after approval

1. Reuse this audit; check only changed/unsaved state and unresolved references.
2. Record callbacks, settings, initial panel states and baseline behaviour.
3. Save a unique dated scene-copy backup before changes; stop if backup fails.
4. Make grouped source/asset changes. Import/compile only if necessary.
5. Apply all five screens in one bulk Unity MCP builder call; use batch_execute for independent audits/checks.
6. Review every screen in Play Mode, then fix only failed visual checks.
7. Save only quiz4. Restore editor settings and Edit Mode.
8. Update this document with actual results, limitations, backup location and screenshots.

No repeated hierarchy reads, per-object tool loops, unnecessary refreshes or unrelated project changes.

## 7. Acceptance tests after implementation

- Same five-screen sequence and original active/inactive states.
- Each MCQ's three options: expected score change, feedback, button lockout and 1.2-second advance.
- Fresh-run all-correct score 4/4; all-wrong MCQs followed by correct Water score 1/4. Wrong drag attempts add no points.
- Incorrect Rock/Plastic drops, outside release, correct Water, and repeat-drop lockout.
- Each reflection choice reaches results without changing score.
- Restart reproduces baseline behaviour; any pre-existing reset failure is explicitly reported.
- Back works through the original additive AKT4 entry and standalone fallback.
- All five screens reviewed at 1920×1080 and 16:10; no clipped questions, distorted images, hidden feedback or blocked controls.
- Drag movement, return positions and slot hit testing at both sizes.
- No missing references/scripts or new styling-related errors; compare preserved callbacks/settings with the backup.
- Confirm other AKT4 scenes and shared assets remain unchanged.

## Current execution status

Implemented and saved all five panels in quiz4. The sections above preserve the approved plan; actual results and exceptions follow.

### Delivered

- Matching pale gradient, navy outlined title, cream cards, rounded blue choices, green primary actions and decorative microbes.
- Three equal illustrated emotion choices; original reflection callback strings retained.
- Existing question images preserved and framed without stretching.
- Existing Back button relabelled KEMBALI, with its original navigation behaviour.
- Editor-only bulk builder: `Assets/Editor/Quiz4StyleBuilder.cs`.
- Editor-only regression helper: `Assets/Editor/Quiz4StyleQA.cs`.
- No gameplay source changes, new runtime component, generated artwork, shared asset edits or edits to other scenes.
- Baseline backup: `Assets/Scenes/Aktivity/akt4/quiz4_before_style_20260907_081241.unity`. Additional dated pre-apply backups remain.

### Verified results

- Baseline and styled runs exercised every answer option for each question, scoring, button lockout, double-click protection, feedback and delayed progression.
- Correct-answer run produced 4/4. Both all-wrong-MCQ runs followed by Water produced 1/4.
- Wrong Rock/Plastic drops and outside release returned correctly without scoring. Water scored once; repeat drops were ignored.
- All three reflection choices reached the result without changing score.
- Final Full HD harness: 183 passing checks, no unexpected failures.
- 16:10 harness: 149 passing checks, no unexpected failures.
- Those counts include repeated gameplay, text-fit, button-bounds and raycast checks, not separate test scenarios.
- All five screens and all three question/image combinations reviewed at 1920x1080 and 1164x728. The initial header-box overflow was corrected.
- Drag movement and return behaviour passed at both sizes. Drop-zone hit tests passed after allowing layout to settle.
- Compared 31 gameplay references, item IDs, persistent callbacks, Back fallback and Canvas settings to the baseline backup: exact match.
- Standalone Back returned to akt1. Original AKT4 entry loaded quiz4 additively; a result-panel navigation isolation check confirmed Back unloads quiz4 and restores the AKT4 canvas, camera and EventSystem.
- No missing scripts or compilation errors. Navigation through existing scenes logged WindowsMediaFoundation colour-primaries errors from unchanged video files.
- Game View size and background-rendering setting restored; quiz4 left saved, clean, in Edit Mode.

### Confirmed existing issues — deliberately not fixed

1. **Restart:** baseline and styled tests both confirmed that the answered drop slot remains locked. A second run cannot complete its drag stage. Restart still resets score and question index as before.
2. **Drag feedback target:** the existing DropSlot4_4.feedbackText reference points to ReflectionPanel/DragFeedbackText, not the drag panel's similarly named text. Consequently drag feedback is not visible on the drag screen and the success message appears on reflection. That reference is preserved. The initial placeholder was cleared, but the referenced component remains active and receives the original runtime messages.

These require separately approved gameplay/wiring fixes; they are not styling regressions.

### Final previews

- [Start](D:/UProject/Microfun/MicroFun/Captures/Quiz4/FullHD_final_start.png)
- [Question](D:/UProject/Microfun/MicroFun/Captures/Quiz4/FullHD_final_question2.png)
- [Drag-and-drop](D:/UProject/Microfun/MicroFun/Captures/Quiz4/FullHD_final_drag.png)
- [Reflection](D:/UProject/Microfun/MicroFun/Captures/Quiz4/FullHD_final_reflection.png)
- [Result](D:/UProject/Microfun/MicroFun/Captures/Quiz4/FullHD_final_result.png)
- [16:10 question](D:/UProject/Microfun/MicroFun/Captures/Quiz4/Wide16x10_question2.png)

The unity-mcp-orchestrator skill guided the backup, bulk scene-builder operation, reuse of existing objects/assets, and before/after verification.
