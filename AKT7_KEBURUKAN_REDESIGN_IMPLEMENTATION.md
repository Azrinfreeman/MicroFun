# Activity 7 Keburukan Redesign — Implementation Plan

## Status

Planning document only. Do not modify the Unity scene, scripts, callbacks, images, detail panels, audio, or Build Settings until the user explicitly approves execution.

## Objective

Redesign `Canvas/Keburukan` in `Assets/Scenes/Aktivity/akt7/akt7.unity` to closely match the supplied Activity 7 reference while expanding its three-card visual concept into **four illustrated, interactive cards**.

The four cards will represent the four working Keburukan buttons already present in the scene:

1. `Merosakkan makanan`
2. `Mereputkan gigi`
3. `Menyebabkan keracunan makanan`
4. `Merosakkan tanaman`

The fourth card will use a new illustration of a diseased or wilting plant with spotted/retting leaves and visible microorganism characters, matching the visual storytelling used by the first three reference cards.

All labels and numbers remain editable Unity TextMeshPro content. No Malay text will be baked into generated images.

## Live Scene Audit

- Active scene: `Assets/Scenes/Aktivity/akt7/akt7.unity`.
- Unity version: `6000.4.5f1`.
- Editor is ready, not compiling, and not in Play Mode.
- Target panel: `Canvas/Keburukan`.
- Canvas continues to use a `1920 × 1080` Scale With Screen Size layout.
- The current visible title incorrectly reads `KEGUNAAN MIKROORGANISMA`; the generated redesign will display `Keburukan Mikroorganisma` without altering unrelated lesson content.
- The Activity 7 centre menu and redesigned Pengenalan panel are outside this change scope.

Current relevant hierarchy:

```text
Canvas
└── Keburukan
    ├── Center
    │   ├── Title
    │   │   └── Text                       "KEGUNAAN MIKROORGANISMA"
    │   ├── Button                         inactive → Fungi
    │   ├── Button (1)                     inactive → Kehadiran Air
    │   ├── Button (2)                     MEROSAKKAN MAKANAN
    │   ├── Button (5)                     MEREPUTKAN GIGI
    │   ├── Button (6)                     MENYEBABKAN KERACUNAN MAKANAN
    │   ├── Button (3)                     MEROSAKKAN TANAMAN
    │   └── Button (4)                     inactive → Virus
    ├── Bottom
    │   ├── NextButton                     inactive
    │   └── HomeButton                     active
    ├── MerosakkanMakanan                  detail panel
    ├── MereputkanGigi                     detail panel
    ├── Merosakkan Tanaman                 detail panel
    └── Menyebabkan Keracunan Makanan      detail panel
```

## Confirmed Card and Callback Mapping

| New card | Existing source button | Persistent callback | Existing destination panel |
|---|---|---|---|
| Card 1 — Merosakkan makanan | `Center/Button (2)` | `MerosakkanMakanan.SetActive(true)` | `Canvas/Keburukan/MerosakkanMakanan` |
| Card 2 — Mereputkan gigi | `Center/Button (5)` | `MereputkanGigi.SetActive(true)` | `Canvas/Keburukan/MereputkanGigi` |
| Card 3 — Menyebabkan keracunan makanan | `Center/Button (6)` | `Menyebabkan Keracunan Makanan.SetActive(true)` | `Canvas/Keburukan/Menyebabkan Keracunan Makanan` |
| Card 4 — Merosakkan tanaman | `Center/Button (3)` | `Merosakkan Tanaman.SetActive(true)` | `Canvas/Keburukan/Merosakkan Tanaman` |
| Home | `Bottom/HomeButton` | `Keburukan.SetActive(false)` and `Center.SetActive(true)` | Activity 7 centre menu |

The inactive Fungi, Kehadiran Air, and Virus buttons and their persistent listeners will remain serialized but hidden. They are not part of the four-card redesign.

## Existing Detail Content to Preserve

### Merosakkan makanan

Current explanation:

`Mikroorganisma seperti bakteria dan kulat menyebabkan makanan menjadi basi, berbau, dan berkulat sehingga tidak selamat dimakan.`

### Mereputkan gigi

Current explanation:

`Bakteria dalam mulut memecahkan sisa makanan dan menghasilkan asid yang merosakkan enamel gigi, seterusnya menyebabkan gigi berlubang.`

### Menyebabkan keracunan makanan

Current explanation:

`Sesetengah bakteria menghasilkan toksin yang boleh menyebabkan sakit perut, muntah, dan cirit birit selepas makanan tercemar dimakan.`

### Merosakkan tanaman

Current explanation:

`Kulat dan bakteria tertentu menjangkiti tumbuhan, menyebabkan daun layu, bertompok, atau reput sehingga menjejaskan hasil pertanian.`

These panels, close buttons, descriptions, and object names must remain unchanged during the menu-card redesign.

## Visual Specification

### Header

- Orange/gold rounded `AKTIVITI 7` badge centred at the top.
- Large green `Keburukan Mikroorganisma` title with white outline and soft shadow.
- Cream instruction pill with green outline:

  `Pilih kad untuk meneroka kesan keburukan mikroorganisma.`

- Small decorative microorganism icons may appear at both ends of the instruction pill, but they must not receive raycasts.

### Four-card adaptation

The reference shows three large cards. To accommodate the real fourth button while retaining the same composition, use **four equal-width cards in a single horizontal row**.

Each card contains:

- a rounded cream/pastel frame;
- coloured border and soft lower shadow;
- circular numbered badge overlapping the top edge;
- a dedicated illustration occupying the upper 65–70% of the card;
- a glossy coloured label panel at the bottom;
- editable centred TMP text;
- the original destination invoked through a serialized relay;
- button hover, press, and touch-scale feedback.

### Card treatments

| No. | Label | Colour family | Illustration brief |
|---:|---|---|---|
| 1 | `Merosakkan\nmakanan` | amber/orange | Mouldy bread, spoiled tomato, mouldy cheese, subtle odour lines, friendly microorganisms. |
| 2 | `Mereputkan\ngigi` | coral/red | Sad decayed tooth with a dark cavity and mischievous bacteria/microorganism. |
| 3 | `Menyebabkan\nkeracunan makanan` | purple | Child holding an upset stomach beside contaminated food and microorganisms. |
| 4 | `Merosakkan\ntanaman` | green/teal | Wilting crop or potted plant with yellowing, spotted, or rotting leaves; nearby fungi/bacteria characters. |

The fourth illustration should communicate plant disease immediately without relying on its label.

### Background and decoration

- Pale lime left edge blending through warm cream to soft peach on the right.
- Green and purple microorganism mascots near the upper corners.
- Atom, DNA, bubbles, and small stars around the header.
- Laboratory flask at the lower-left edge.
- Magnifier/petri-dish decoration at the lower-right edge.
- Decorative artwork must remain behind the cards and outside all hit areas.

## Approximate 1920×1080 Layout

| Element | Placement | Planned size |
|---|---:|---:|
| Activity badge | top centre, y `-30` | `450 × 105` |
| Main title | top centre, y `-145` | `1500 × 120` |
| Instruction pill | top centre, y `-278` | `1160 × 76` |
| Four-card row | centre, starting y `-395` | approximately `1690 × 560` |
| Individual card | four across | approximately `395 × 535` |
| Number badge | overlapping each card top | `84 × 84` |
| Illustration area | upper section per card | approximately `340 × 310` |
| Label panel | card bottom | approximately `360 × 145` |
| Home button | top-left or lower-left safe corner | `88 × 88` |

The card row may reduce gaps before reducing card width. Labels use controlled auto-sizing so Card 3 remains readable without crowding.

## Planned Asset Strategy

Create:

```text
Assets/Images/Akt7KeburukanUI/
├── Background/akt7_keburukan_background.png
├── Cards/card_spoiled_food.png
├── Cards/card_tooth_decay.png
├── Cards/card_food_poisoning.png
├── Cards/card_plant_damage.png
└── QA/
```

Asset rules:

- Use built-in ImageGen during execution to create a clean, text-free background and four transparent card illustrations.
- Use the supplied screenshot as the composition and art-direction reference.
- Generate each card picture independently so it can be resized or replaced without rebuilding the entire background.
- All card illustrations must have genuine transparent backgrounds and clean soft edges.
- No generated image may contain labels, card numbers, letters, logos, or watermarks.
- Import illustrations as Sprite/Single with alpha transparency, bilinear filtering, no mipmaps, clamped wrapping, and suitable max texture size.
- Reuse `Assets/Fonts/FredokaOne SDF.asset`, existing sliced button panels, Home icon, and project interaction feedback components.

## Planned Hierarchy

```text
Canvas
└── Keburukan
    ├── Akt7KeburukanRedesignRoot       generated; first sibling
    │   ├── Background
    │   ├── AmbientDecor
    │   ├── Header
    │   │   ├── ActivityBadge
    │   │   ├── MainTitle
    │   │   └── InstructionPill
    │   ├── CardRow
    │   │   ├── Card01SpoiledFood
    │   │   │   ├── NumberBadge
    │   │   │   ├── Illustration
    │   │   │   └── LabelPanel
    │   │   ├── Card02ToothDecay
    │   │   ├── Card03FoodPoisoning
    │   │   └── Card04PlantDamage
    │   └── HomeButtonRelay
    ├── Center                           retained, hidden
    ├── Bottom                           retained as callback source
    ├── MerosakkanMakanan                unchanged
    ├── MereputkanGigi                   unchanged
    ├── Menyebabkan Keracunan Makanan   unchanged
    └── Merosakkan Tanaman               unchanged
```

The visible cards will use `CenterMenuButtonRelay` or an equivalent serialized relay to invoke the original four source buttons. Persistent listeners will not be recreated manually or replaced with runtime object searches.

## Implementation Strategy

### Phase 0 — Safety and baseline

1. Confirm `akt7` is active and the editor is outside Play Mode.
2. Capture the current Keburukan screen.
3. Snapshot all four card callbacks, Home callbacks, and detail-panel close callbacks.
4. Create a separate backup:

   `Assets/Scenes/Aktivity/akt7/akt7_PreKeburukanRedesign_20260831.unity`

5. Confirm the existing centre-menu and Pengenalan backups remain untouched.

### Phase 1 — Generate and import visual assets

1. Generate a text-free Keburukan background based on the supplied reference.
2. Generate four independent transparent illustrations, including the new plant-damage artwork.
3. Inspect each asset for unwanted text, cropped subjects, and opaque/checkered backgrounds.
4. Import and verify alpha before building the card hierarchy.

### Phase 2 — Idempotent Unity builder

Create:

`Assets/Editor/Akt7KeburukanUIBuilder.cs`

with a menu item such as:

`Tools/MicroFun/Rebuild Akt7 Keburukan UI`

The builder will:

- require the exact Activity 7 scene path;
- validate the four source buttons, Home button, and detail destinations;
- delete only a prior `Akt7KeburukanRedesignRoot` when rerun;
- build the background, header, instruction pill, and four-card row;
- keep display images raycast-disabled;
- create one visible Button per card and one visible Home control;
- wire each visible control to its existing source button through serialized relays;
- hide `Center` and the visual portion of `Bottom` without deleting either;
- leave all detail panels and their close controls untouched;
- preserve source listener count, target, method, argument, and order;
- save only after callback verification passes.

### Phase 3 — Interaction and accessibility

- Reuse `ButtonInteractionFeedback` for touch/pointer press scaling.
- Provide clear hover and pressed states without obscuring the card art.
- Use minimum 350 px card width and full-card hit areas at the reference resolution.
- Establish controller navigation order Card 1 → Card 2 → Card 3 → Card 4 → Home.
- Ensure number badges, illustrations, and label text do not block card raycasts.

### Phase 4 — Visual refinement

- Compare the Game view with the supplied screenshot at 1920×1080.
- Balance the four-card row so it still reads like the reference rather than a compressed menu.
- Tune Card 3 label auto-sizing independently because it is the longest.
- Check edge decoration does not collide with Cards 1 or 4.
- Test 1366×768, 2560×1440, and a tall-tablet ratio.

### Phase 5 — Functional QA

- Card 1 opens `MerosakkanMakanan`.
- Card 2 opens `MereputkanGigi`.
- Card 3 opens `Menyebabkan Keracunan Makanan`.
- Card 4 opens `Merosakkan Tanaman`.
- Every detail panel's existing close button hides only its own panel.
- Home hides Keburukan and restores the Activity 7 centre menu.
- Returning from a detail panel leaves the four-card Keburukan screen usable.
- Re-entering Keburukan presents the clean four-card layout.

## Codex Sol Effort Recommendation

Recommended execution model: **`gpt-5.6-sol` with `high` reasoning effort**.

| Phase | Suggested effort | Reason |
|---|---|---|
| Callback/detail-panel audit | Medium | Existing mappings are direct but must be preserved exactly. |
| Background and four illustrations | High | Requires consistent characters, transparency, and instant visual communication. |
| Builder implementation | High | Four relays, responsive cards, and idempotence. |
| Visual refinement | High | Four cards must remain legible while matching a three-card reference. |
| Play Mode regression QA | High | Tests all four detail panels, close controls, and Home flow. |

Use `xhigh` only if multiple illustration variants are needed to achieve consistent art across all four cards.

## Verification Checklist

1. Zero new C# compilation errors.
2. Scene validation reports no missing scripts or broken references.
3. Running the builder twice leaves exactly one generated root and four visible cards.
4. Title reads `Keburukan Mikroorganisma`.
5. Instruction text matches the reference wording.
6. All four labels are editable TMP text.
7. Card 4 clearly depicts microorganism damage to plants.
8. Each card invokes the correct existing detail panel.
9. All four source listener snapshots match before and after building.
10. Home returns to the redesigned Activity 7 centre menu.
11. Existing detail descriptions and close buttons remain unchanged.
12. Inactive Fungi, Kehadiran Air, and Virus controls remain preserved.
13. Decorative Images have raycasts disabled.
14. No generated bitmap contains text or numbers.
15. Console is checked after compilation and after every panel transition.

## Acceptance Criteria

- Keburukan closely matches the supplied Activity 7 reference.
- The three reference concepts are retained and a visually consistent fourth plant-damage card is added.
- Four cards fit in one row at 1920×1080 without clipped text or overlapping hit areas.
- Each card opens its current detail panel correctly.
- Home navigation remains functional.
- Existing detail content, close callbacks, and hidden legacy buttons remain intact.
- No unrelated panel, video, controller, scene, or Build Settings entry changes.

## Expected Files During Execution

- `Assets/Scenes/Aktivity/akt7/akt7.unity`
- `Assets/Scenes/Aktivity/akt7/akt7_PreKeburukanRedesign_20260831.unity`
- `Assets/Editor/Akt7KeburukanUIBuilder.cs`
- `Assets/Images/Akt7KeburukanUI/`
- `Assets/Screenshots/akt7_keburukan_*.png`

## Explicit Non-Goals

- Do not redesign the four detail panels in this pass.
- Do not alter their Malay explanations.
- Do not redesign Pengenalan, Micro-Scene, Kuiz 7, or the Activity 7 centre menu.
- Do not expose inactive Fungi, Kehadiran Air, or Virus controls.
- Do not change `SceneController.cs` or Build Settings.
- Do not replace persistent listeners with runtime `GameObject.Find` wiring.
- Do not bake labels or card numbers into generated artwork.

