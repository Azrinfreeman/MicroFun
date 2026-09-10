# Pengenalan UI Redesign — Implementation Plan

Status: **planning only**  
Recommended model: **Codex Sol (gpt-5.6-sol)**  
Target scene: Assets/Scenes/Aktivity/akt8/akt8.unity  
Target section: Canvas/Pengenalan  
Reference: supplied “Aktiviti 8 — Pengenalan” puzzle screen

No Unity scene, script, hierarchy, audio, or art changes are included in this planning pass.

## Goal

Rebuild the Activity 8 Pengenalan section so it closely matches the supplied reference:

- Warm microorganism-themed background.
- Top-left “Dengar audio” control.
- Centered “AKTIVITI 8” badge, large “Pengenalan” title, and Malay subtitle.
- Left instruction card.
- Large interactive 3×3 jigsaw puzzle on the right.
- Eight pieces initially assembled and one loose piece that the learner drags into the missing slot.
- Existing Activity 8 navigation and dialog flow remain functional.

## Current project baseline

Read-only Unity MCP inspection found:

- The active scene is Assets/Scenes/Aktivity/akt8/akt8.unity.
- The Canvas already uses a responsive 1920 × 1080 reference resolution.
- Canvas/Pengenalan currently contains Title, Content, Bottom, and five inactive dialog overlays.
- Bottom contains active Home and Next buttons.
- The five dialogs use DialogController and Animator and must be preserved.
- The puzzle is Canvas/Pengenalan/Content/Center/puzzles.
- Existing puzzle assets include puzzle.png, puzzle_empty.png, and nine piece sprites named 1.png through 9.png.
- The current hierarchy already resembles the reference’s one-piece challenge:
  - Eight board pieces are active.
  - The center piece is inactive.
  - A separate Image (3) uses puzzles/5.png and appears to be the loose piece.
- No puzzle piece currently has a drag handler, drop handler, CanvasGroup, or completion controller.
- The full-answer Image is semi-transparent but still has raycastTarget enabled, which can interfere with pointer input.
- AudioNarrationButton.cs exists and already supports play/pause labels.
- No Pengenalan-specific narration AudioClip was found.
- Existing generic drag scripts are not a complete match:
  - DraggableItem can move an item but has no correct-slot or completion logic.
  - DropSlot expects DraggableCard, not a puzzle piece.

## Visual target

### Composition

- Cream/yellow backdrop with lime-green lower-left and peach lower-right fields.
- Bubbles, sparkles, atom line art, and restrained science decoration.
- Blue water-drop mascot at upper-left.
- Green microorganism mascot at upper-right.
- Laboratory flask at lower-left.
- Petri dish at lower-right.
- Decoration remains outside the reading and interaction zones.

### Header

- Top-center cream capsule: “AKTIVITI 8”.
- Large rounded green title: “Pengenalan”.
- Subtitle: “Susun puzzle untuk mengenal pasti mikroorganisma.”
- All words remain native TextMeshPro text.

### Audio control

- Top-left green-and-cream pill with a circular speaker icon.
- Default label: “Dengar audio”.
- Playing label: “Jeda audio”.
- Visible hover, pressed, playing, and disabled states.
- Minimum hit target: 64 px at the reference resolution.

### Instruction card

- Rounded cream card on the left.
- Clipboard/pencil illustration or native icon.
- Header: “Arahan”.
- Instruction: “Lengkapkan puzzle untuk melihat gambar mikroorganisma.”
- Bottom badge: “Aktiviti hands-on digital”.

### Puzzle

- Large blue/purple framed puzzle on the right.
- Three columns × three rows.
- Eight pieces begin assembled.
- One loose purple piece starts outside the lower-right of the board.
- The missing slot has a subtle translucent guide.
- Puzzle artwork uses friendly cartoon microorganisms and a strong visual “identify/avoid” theme without generated text.

## Proposed hierarchy

    Canvas/Pengenalan
    ├── PengenalanRedesignRoot
    │   ├── Background
    │   │   ├── GeneratedBackdrop
    │   │   └── DecorativeAccents
    │   ├── AudioButton
    │   │   ├── SpeakerIcon
    │   │   └── Label
    │   ├── Header
    │   │   ├── ActivityBadge
    │   │   ├── Title
    │   │   └── Subtitle
    │   ├── InstructionArea
    │   │   ├── InstructionCard
    │   │   └── HandsOnBadge
    │   └── PuzzleArea
    │       ├── BoardShadow
    │       ├── BoardFrame
    │       ├── CompletedArtwork
    │       ├── Slots
    │       │   └── Slot01 ... Slot09
    │       ├── PlacedPieces
    │       │   └── Piece01 ... Piece09
    │       ├── LoosePieceLayer
    │       │   └── Piece05
    │       └── CompletionFX
    ├── Bottom
    │   ├── HomeButton
    │   └── NextButton
    └── Existing dialog overlays

The redesign root renders underneath the dialog overlays so the existing dialogs can still open above it.

## Implementation approach

### 1. Safety and isolation

- Duplicate the scene as akt8_PrePengenalanRedesign_20260829.unity.
- Work only under Canvas/Pengenalan.
- Preserve all five dialog objects, Animator references, and controller fields.
- Preserve serialized Home and Next callbacks.
- Move old visuals into a disabled LegacyPengenalanVisuals group rather than deleting them initially.
- Keep the existing Micro Squad redesign untouched.

### 2. Generated assets

Use built-in ImageGen for project-bound PNG assets:

- Text-free 16:9 Pengenalan backdrop matching the reference.
- One completed puzzle illustration without letters, labels, or watermark.
- Optional transparent clipboard/pencil icon.
- Optional transparent speaker icon if no suitable project icon exists.

Planned folder:

    Assets/Images/PengenalanUI/
    ├── Background/
    ├── Puzzle/
    ├── Icons/
    └── QA/

Rules:

- No generated lettering.
- Keep clear title, instruction, and puzzle zones.
- Generate at least 2× intended display size.
- Preserve transparency for standalone icons.
- Import as Sprite (2D and UI), enable alpha, disable mipmaps, and use suitable UI compression.

### 3. Native Unity UI

- Build all text with FredokaOne SDF.
- Use anchors rather than fixed world positions.
- Use 9-sliced sprites for cards, capsules, and buttons.
- Decorative Images use raycastTarget = false.
- Interactive buttons and the loose puzzle piece use raycastTarget = true.
- Verify the top GraphicRaycaster hit for every interactive control.
- Preserve the current CanvasScaler unless visual QA shows a problem.

### 4. Puzzle asset preparation

Preferred approach:

- Generate one completed puzzle illustration.
- Use the existing nine puzzle-piece silhouettes as masks, or recreate a consistent nine-piece mask set.
- Produce nine aligned sprites from the same master image.
- Keep transparent bounds and pivots consistent with the board slots.
- Retain the existing one-piece challenge:
  - Pieces 1–4 and 6–9 are locked.
  - Piece 5 starts loose.
  - Slot 5 is the active drop target.

This matches both the reference and the existing scene structure.

### 5. Puzzle behavior

Add purpose-built scripts:

- PengenalanPuzzleController
  - Owns puzzle state.
  - Resets when the section is re-entered.
  - Controls the agreed Next-button progression.
  - Reveals completed artwork.
  - Raises an OnPuzzleCompleted UnityEvent.
- DraggablePuzzlePiece
  - Implements IBeginDragHandler, IDragHandler, and IEndDragHandler.
  - Stores starting parent and position.
  - Moves to a top drag layer while dragging.
  - Uses CanvasGroup so the drop slot can receive the pointer.
  - Returns smoothly after an invalid drop.
- PuzzleDropZone
  - Implements IDropHandler.
  - Accepts only the expected piece ID.
  - Snaps the correct piece into the slot.
  - Rejects incorrect pieces without changing state.

Completion behavior:

- Lock the placed piece.
- Hide the slot guide.
- Fade the completed artwork to full opacity.
- Play a short success pulse or sparkle.
- Optionally play a success sound.
- Show “Puzzle lengkap!”.
- Continue to the existing dialog or enable Next according to the lesson flow.

### 6. Input safety

This is a high-priority phase because the current puzzle has several full-size raycastable Images.

- Disable raycasts on the background, answer overlay, board frame, and decoration.
- Keep only the loose piece, slot, Audio button, Home, and Next raycastable.
- During dragging, set the piece CanvasGroup blocksRaycasts to false.
- Restore blocksRaycasts after release.
- Test via the actual GraphicRaycaster and EventSystem path, not only direct callback invocation.
- Test mouse and touch-style pointer events.

### 7. Audio narration

- Reuse AudioNarrationButton.
- Add an AudioSource to the top-left control.
- Produce or assign a Malay narration clip using:

  “Pengenalan. Susun puzzle untuk mengenal pasti mikroorganisma. Lengkapkan puzzle untuk melihat gambar mikroorganisma.”

- Verify play, pause, resume, finish, and section-disable states.
- If narration generation is approved, use a clear, warm Malaysian Malay educational voice.

### 8. Navigation and dialogs

- Keep Home available but visually unobtrusive.
- Preserve Next’s existing callback.
- During execution, verify whether Next should remain available or unlock only after completion.
- Keep all dialogs as later siblings so they render above the redesign.
- Ensure completion cannot invoke dialogs twice.

## Codex Sol effort by phase

Use gpt-5.6-sol.

| Phase | Work | Codex Sol reasoning effort | Estimated implementation time |
|---|---|---:|---:|
| 0. Safety and baseline | Backup, callback inventory, screenshot, puzzle-state audit | **Medium** | 0.5–1 h |
| 1. Visual specification | Wireframe, visual tokens, safe zones, hierarchy | **High** | 1–1.5 h |
| 2. Asset generation | Background, puzzle artwork, icons, import preparation | **High** | 2.5–4 h |
| 3. Puzzle-piece preparation | Align/slice pieces, masks, slots, alpha QA | **XHigh** | 2–4 h |
| 4. Native UI build | Header, audio control, card, badges, puzzle frame, navigation | **High** | 3–4.5 h |
| 5. Puzzle programming | Drag/drop, snap, reset, completion event and result state | **XHigh** | 2.5–4 h |
| 6. Audio | Narration generation/import and play/pause integration | **High** | 1–2 h |
| 7. Dialog/navigation integration | Preserve callbacks, routing, Next behavior | **High** | 1–2 h |
| 8. QA and polish | EventSystem tests, resolutions, audio, dialogs, console | **XHigh** | 2–3 h |

Estimated total:

- **Visual redesign with the current static puzzle:** approximately **10–15 hours**.
- **Full interactive puzzle, narration, and dialog integration:** approximately **15–22 hours**.

Recommended Codex Sol setting:

- Use **High** for the overall implementation when cost or speed is the priority.
- Use **XHigh** for puzzle-piece preparation, pointer-input implementation, and final QA.
- For one uninterrupted “implement and verify everything” run, use **XHigh**.
- **Ultra is not necessary** for this scope.

## Verification plan

### Visual

- Compare against the reference at 1920×1080, 1366×768, 2560×1440, 3840×2160, and one 16:10 resolution.
- Confirm the title, card, puzzle, audio control, flask, petri dish, and mascots preserve the reference hierarchy.

### Puzzle interaction

- GraphicRaycaster’s top hit is the loose piece.
- The piece follows mouse or touch at every Canvas scale.
- Invalid release returns it to the exact start position.
- Correct release snaps into slot 5.
- Completion fires exactly once.
- The piece cannot be dragged again after completion.
- Reopening Pengenalan resets cleanly.

### Audio

- Audio button receives pointer events.
- Play, pause, resume, finish, and disable states update correctly.
- Narration instances do not overlap.
- A missing clip warns clearly without breaking the screen.

### Integration

- Home and Next still work.
- Dialog overlays remain above the redesign.
- Other Activity 8 sections do not change.
- No duplicate EventSystem.
- No generated image contains accidental lettering.
- Scene validation finds no missing scripts or broken references.
- Zero new Pengenalan-related Console errors or warnings.

## Acceptance criteria

- The screen is immediately recognizable as the supplied reference.
- Every displayed word is editable TMP text.
- The learner can drag the loose piece into the correct slot with mouse or touch.
- Completion is revealed clearly and only once.
- The audio button plays and pauses narration.
- Existing Home, Next, and dialog behavior remains functional.
- The UI scales from HD to 4K.
- Micro Squad and other Activity 8 sections are not regressed.

## Execution boundary

Execution begins only after approval. The first action will be a new scene backup and baseline screenshot. Asset generation, scripts, hierarchy changes, audio generation, scene saving, and Play Mode testing follow only after that backup.
