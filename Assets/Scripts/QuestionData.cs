// ============================================================
// QuestionData.cs
// All data structures for the quiz game.
// Attach to no GameObject — this is a pure data file.
// ============================================================
using UnityEngine;

// ── Scene V1: Multiple Choice Question ──────────────────────
[System.Serializable]
public class MCQQuestion
{
    [TextArea(2, 4)]
    public string questionText; // The question shown to the player

    public string[] options; // Exactly 4 answer strings

    public int correctAnswerIndex; // 0=A, 1=B, 2=C, 3=D

    public string imageName; // Filename in Resources/Images/ (no extension)

    [TextArea(1, 3)]
    public string feedbackCorrect; // Shown when player is right

    [TextArea(1, 3)]
    public string feedbackWrong; // Shown when player is wrong
}

// ── Scene V2: Scenario-Based Question ───────────────────────
[System.Serializable]
public class ScenarioQuestion
{
    [TextArea(2, 4)]
    public string scenarioDescription; // Context paragraph (e.g. "You see mouldy food...")

    [TextArea(2, 4)]
    public string questionText; // The actual question

    public string imageName; // Scenario image filename in Resources/Images/

    public string[] options; // 4 answer choices

    public int correctAnswerIndex;

    [TextArea(2, 5)]
    public string explanation; // Shown after any answer is selected
}

// ── Scene V3: Reflection Statement ──────────────────────────
[System.Serializable]
public class ReflectionStatement
{
    [TextArea(1, 3)]
    public string statementText; // The self-reflection statement
}

// Response types for reflection (stored as int)
public enum ReflectionResponse
{
    Understand = 0, // 😊
    Unsure = 1, // 😐
    DontUnderstand = 2, // 😟
}

// ── JSON-Serialisable Save Container ────────────────────────
// Used by GameManager to write reflection data to PlayerPrefs
[System.Serializable]
public class ReflectionSaveEntry
{
    public string statement;
    public int response; // Cast from ReflectionResponse enum
}

[System.Serializable]
public class ReflectionSaveData
{
    public ReflectionSaveEntry[] entries;
}
