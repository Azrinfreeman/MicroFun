# MicroFun

**A Bahasa Melayu science learning game about microorganisms, built with Unity 6.**

MicroFun brings together illustrated lessons, microorganism exploration, quizzes, interactive exercises, and reflection activities. The project is organised into **eight activity groups**, reached through a shared activity menu, with supporting lesson and quiz scenes.

**Current status:** a development project with substantial learning content and UI tooling. This documentation review confirmed source/configuration details but did not open Unity or run the game. Existing plans and reports describe different development stages; they are not evidence that every proposed redesign is complete. See [validation status](docs/VALIDATION.md).

## Learning and interaction

- **Microorganism exploration:** lesson scenes for fungi, bacteria, algae, protozoa, and viruses.
- **Growth conditions:** pages and exercises about conditions such as water, air, temperature, nutrients, and acidity. The Activity 5 factor menu intentionally offers four factors; its older water page is retained but inactive according to its implementation record.
- **Uses and harmful effects:** activity content about the roles of microorganisms and their effects on food, health, and the environment.
- **Quizzes and scenarios:** answer selection, feedback, scoring, and scenario explanations, including the three-scene Activity 6 quiz/application/reflection sequence.
- **Hands-on UI:** drag-and-drop cards/answers, magnifier interactions, model rotation, and an Activity 8 recognition puzzle with slot validation and completion feedback.
- **Media and reflection:** assigned narration clips, video play/pause and seeking, discussion/presentation pages, and emoji-based self-reflection.

These descriptions come from scene inventory, application scripts, UI-builder text, and implementation records. They do not establish teaching-content approval, curriculum endorsement, or completed device testing.

## Engineering highlights

| Area | Implementation |
| --- | --- |
| Editor | Unity **6000.4.5f1** |
| Rendering | Universal Render Pipeline **17.4.0** |
| Interface | uGUI **2.0.0**, TextMesh Pro labels, Canvas layouts, reusable button feedback |
| Content structure | Eight activity groups and **40 enabled scenes** in the tracked Build Settings |
| Interaction | Pointer drag/drop handlers, puzzle slot checks, audio lifecycle controls, and scene navigation |
| Quiz flow | Multiple quiz controllers, serializable question/scenario/reflection data, score and feedback UI |
| Authoring | Activity-specific Editor UI builders and existing style-QA helpers |
| Persistence | Activity 6's GameManager saves score totals and reflection JSON in PlayerPrefs |

Several quiz/scoring implementations coexist. Verify each activity's score and navigation lifecycle rather than assuming all activities share one progress system. No application test suite was found in the reviewed tracked inventory, although Unity Test Framework **1.6.0** is declared.

## Open the project

1. Clone the repository:

   ```sh
   git clone https://github.com/Azrinfreeman/MicroFun.git
   ```

2. Install Unity **6000.4.5f1** through Unity Hub and add the repository root. The project has substantial art, video, and historical scene assets; allow time and disk space for a full clone and first import.
3. Let Unity resolve packages and import assets. The manifest includes Unity MCP as a Git dependency, so resolving that tooling package requires network access.
4. Open `Assets/Scenes/StartScene.unity`, enter Play mode, and follow the start flow to `Activities`. Check the Console before exploring individual activities.
5. Confirm lesson navigation, answer submission, score/reset behavior, and assigned media in the activities you plan to demonstrate. Native input and layout behavior still need checks on the intended device.

Do not run UI-builder menu commands merely to open the project: they are authoring tools that can modify scenes. Use an isolated copy or branch when experimenting with them. No GitHub release build was listed at documentation-review time.

## Source guide

| Path | Responsibility |
| --- | --- |
| [`Assets/Scenes/`](Assets/Scenes/) | Start/menu scenes and activity lesson, exercise, and quiz scenes |
| [`Assets/Scripts/`](Assets/Scripts/) | Shared navigation, quiz/scoring, media, drag/drop, puzzle, and presentation components |
| [`Assets/Editor/`](Assets/Editor/) | Activity-specific UI builders and style-QA tools |
| [`Assets/Scripts/ScenarioManager.cs`](Assets/Scripts/ScenarioManager.cs) / [`ReflectionManager.cs`](Assets/Scripts/ReflectionManager.cs) | Scenario explanations and self-reflection flow |
| [`Assets/Scripts/GameManager.cs`](Assets/Scripts/GameManager.cs) / [`QuestionData.cs`](Assets/Scripts/QuestionData.cs) | Activity 6 score/reflection state and serializable data structures |
| [`Assets/Scripts/PengenalanPuzzleController.cs`](Assets/Scripts/PengenalanPuzzleController.cs) | Activity 8 puzzle completion and feedback |
| [`Assets/Scripts/AudioNarrationButton.cs`](Assets/Scripts/AudioNarrationButton.cs) / [`VideoPlayerController.cs`](Assets/Scripts/VideoPlayerController.cs) | Assigned narration and video playback controls |
| [`Assets/Images/`](Assets/Images/) / [`Assets/Audio/`](Assets/Audio/) / [`Assets/Videos/`](Assets/Videos/) | Teaching and presentation media |

Activity-specific managers also live under `Assets/Scenes/Aktivity/`. Preserve scene references and `.meta` identities when changing assets. Backups and historical material remain in the repository; the enabled scene list identifies the configured build flow.

## Development records

The root contains focused plans and implementation reports. Start with the relevant record, inspect its status, and confirm important details against current code and scenes:

- [Pengenalan implementation report](PENGENALAN_UI_IMPLEMENTATION_REPORT.md): implemented puzzle/UI work, with a final manual drag check recorded as pending.
- [Activity 5 factor redesign](AKT5_FAKTOR_REDESIGN_IMPLEMENTATION.md): implemented menu changes and retained inactive content.
- [Activities UI plan](ACTIVITIES_UI_IMPLEMENTATION.md): planning-stage layout and navigation findings.
- [Contribution guidance](AGENTS.md): repository instructions for future work.

This documentation update preserves all gameplay, learning text, answer data, media, scenes, and historical records. No top-level software license is present; retain bundled third-party notices and confirm permissions before redistributing code or teaching assets.
