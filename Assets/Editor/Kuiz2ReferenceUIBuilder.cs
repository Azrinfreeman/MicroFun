using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class Kuiz2ReferenceUIBuilder
{
    static readonly Color Cream = new Color32(255, 246, 208, 255);
    static readonly Color Navy = new Color32(7, 50, 104, 255);
    static readonly Color Green = new Color32(0, 86, 31, 255);

    [MenuItem("MicroFun/Build Kuiz 2 Reference UI")]
    public static void Build()
    {
        var canvas = GameObject.Find("Canvas");
        var content = GameObject.Find("Canvas/Content");
        if (canvas == null || content == null) throw new Exception("Kuiz 2 Canvas/Content was not found.");

        var quiz = content.GetComponent<QuizController>();
        if (quiz == null) throw new Exception("Kuiz 2 QuizController was not found.");

        var existing = canvas.transform.Find("Kuiz2GeneratedReference");
        if (existing != null) UnityEngine.Object.DestroyImmediate(existing.gameObject);

        var title = canvas.transform.Find("Title");
        var score = canvas.transform.Find("Score");
        var legacyBack = canvas.transform.Find("Back");
        if (title != null) title.gameObject.SetActive(false);
        if (score != null) score.gameObject.SetActive(false);
        if (legacyBack != null) legacyBack.gameObject.SetActive(false);
        foreach (Transform child in content.transform) child.gameObject.SetActive(false);

        var root = New("Kuiz2GeneratedReference", canvas.transform);
        Fill(root);
        MakeBackground(root);
        MakeTitle(root);
        MakeAudio(root);
        var question = MakeQuestionPanel(root);
        var progress = MakeProgress(root);
        var answers = MakeAnswers(root);
        var next = MakeNext(root);
        MakeHome(root);

        var serialized = new SerializedObject(quiz);
        serialized.FindProperty("questionText").objectReferenceValue = question;
        serialized.FindProperty("indicatorText").objectReferenceValue = progress;
        serialized.FindProperty("nextButton").objectReferenceValue = next;
        var list = serialized.FindProperty("buttonAnswers");
        list.arraySize = answers.Length;
        for (int i = 0; i < answers.Length; i++) list.GetArrayElementAtIndex(i).objectReferenceValue = answers[i].transform;
        serialized.ApplyModifiedPropertiesWithoutUndo();

        EditorUtility.SetDirty(quiz);
        EditorUtility.SetDirty(canvas);
        Debug.Log("Kuiz 2 reference UI built.");
    }

    static RectTransform New(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        var rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        return rt;
    }

    static void Fill(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
    }

    static void Place(RectTransform rt, Vector2 position, Vector2 size)
    {
        rt.anchorMin = rt.anchorMax = new Vector2(.5f, .5f);
        rt.pivot = new Vector2(.5f, .5f);
        rt.anchoredPosition = position; rt.sizeDelta = size;
    }

    static Sprite DefaultSprite()
    {
        return AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
    }

    static Image Panel(Transform parent, string name, Vector2 position, Vector2 size, Color color)
    {
        var rt = New(name, parent);
        Place(rt, position, size);
        var image = rt.gameObject.AddComponent<Image>();
        image.sprite = DefaultSprite();
        image.type = Image.Type.Sliced;
        image.color = color;
        return image;
    }

    static Image Disc(Transform parent, string name, Vector2 position, Vector2 size, Color color)
    {
        var rt = New(name, parent);
        Place(rt, position, size);
        var image = rt.gameObject.AddComponent<Image>();
        image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        image.type = Image.Type.Simple;
        image.color = color;
        return image;
    }

    static TextMeshProUGUI Text(Transform parent, string name, string value, Vector2 position, Vector2 size, float fontSize, TextAlignmentOptions alignment, Color color)
    {
        var rt = New(name, parent);
        Place(rt, position, size);
        var text = rt.gameObject.AddComponent<TextMeshProUGUI>();
        text.font = TMP_Settings.defaultFontAsset;
        text.text = value;
        text.fontSize = fontSize;
        text.fontStyle = FontStyles.Bold;
        text.color = color;
        text.alignment = alignment;
        text.enableWordWrapping = true;
        text.raycastTarget = false;
        text.margin = new Vector4(10, 4, 10, 4);
        return text;
    }

    static Image SpriteImage(Transform parent, string name, string path, Vector2 position, Vector2 size, Color tint)
    {
        var rt = New(name, parent);
        Place(rt, position, size);
        var image = rt.gameObject.AddComponent<Image>();
        image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        image.preserveAspect = true;
        image.color = tint;
        image.raycastTarget = false;
        return image;
    }

    static void MakeBackground(Transform root)
    {
        var bg = Panel(root, "Backdrop", Vector2.zero, new Vector2(1920, 1080), new Color32(255, 211, 128, 255));
        bg.raycastTarget = false;
        SpriteImage(root, "GreenMicrobe", "Assets/Images/UI Flat/icon_cute_microbe.png", new Vector2(-680, 300), new Vector2(150, 150), Color.white);
        SpriteImage(root, "PurpleMicrobe", "Assets/Images/UI Flat/icon_cute_microbe.png", new Vector2(650, 305), new Vector2(150, 150), new Color32(220, 126, 255, 255));
        SpriteImage(root, "WaterDrop", "Assets/Images/UI Flat/char_water_drop.png", new Vector2(790, 190), new Vector2(105, 140), Color.white);
        SpriteImage(root, "Flask", "Assets/Images/UI Flat/icon_flask.png", new Vector2(-790, -340), new Vector2(120, 145), Color.white);
        SpriteImage(root, "PetriDish", "Assets/Images/UI Flat/icon_teroka_petri.png", new Vector2(260, -385), new Vector2(145, 100), Color.white);
    }

    static void MakeTitle(Transform root)
    {
        var outer = Disc(root, "TitlePillOuter", new Vector2(0, 445), new Vector2(410, 86), Color.white);
        var inner = Disc(outer.transform, "TitlePill", Vector2.zero, new Vector2(388, 66), new Color32(255, 251, 230, 255));
        var shadow = outer.gameObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(0,0,0,.35f);
        shadow.effectDistance = new Vector2(0,-5);
        Text(outer.transform, "Title", "KUIZ", Vector2.zero, new Vector2(340, 66), 48, TextAlignmentOptions.Center, Green);
        Text(root, "Instruction", "Pilih jawapan yang paling tepat.", new Vector2(0, 345), new Vector2(700, 52), 28, TextAlignmentOptions.Center, Navy);
    }

    static void MakeAudio(Transform root)
    {
        var outer = Panel(root, "AudioButton", new Vector2(-742, 441), new Vector2(320, 70), new Color32(255, 251, 232, 255));
        outer.gameObject.AddComponent<Button>();
        outer.gameObject.AddComponent<AnimatedButtonFeedback>();
        var disc = Disc(outer.transform, "SpeakerDisc", new Vector2(-126, 0), new Vector2(82, 82), new Color32(29, 123, 35, 255));
        SpriteImage(disc.transform, "Speaker", "Assets/Images/UI Flat/icon_speaker.png", Vector2.zero, new Vector2(50, 50), Color.white);
        Text(outer.transform, "Label", "Dengar audio", new Vector2(42, 0), new Vector2(210, 52), 29, TextAlignmentOptions.Center, new Color32(8,79,29,255));
    }

    static TextMeshProUGUI MakeProgress(Transform root)
    {
        var outer = Panel(root, "ProgressOuter", new Vector2(0, 235), new Vector2(420, 72), Color.white);
        var inner = Panel(outer.transform, "ProgressPill", Vector2.zero, new Vector2(400, 54), new Color32(50, 173, 21, 255));
        var shadow = outer.gameObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(0,0,0,.28f);
        shadow.effectDistance = new Vector2(0,-4);
        return Text(outer.transform, "ProgressText", "Soalan 1 daripada 3", Vector2.zero, new Vector2(370, 54), 30, TextAlignmentOptions.Center, Color.white);
    }

    static TextMeshProUGUI MakeQuestionPanel(Transform root)
    {
        var outer = Panel(root, "QuestionOuter", new Vector2(0, -25), new Vector2(1480, 690), new Color32(255, 253, 231, 255));
        var outline = outer.gameObject.AddComponent<Outline>();
        outline.effectColor = new Color32(128, 130, 102, 255);
        outline.effectDistance = new Vector2(4, -4);
        return Text(outer.transform, "QuestionText", "Apakah definisi mikroorganisma?", new Vector2(0, 200), new Vector2(1000, 110), 42, TextAlignmentOptions.Center, Green);
    }

    static Button[] MakeAnswers(Transform root)
    {
        var answerY = new float[] { 20, -130, -280 };
        var colors = new Color[] {
            new Color32(94, 193, 52, 255),
            new Color32(39, 174, 214, 255),
            new Color32(240, 173, 0, 255)
        };
        var buttons = new Button[3];
        for (int i = 0; i < buttons.Length; i++)
        {
            var outer = Panel(root, "Answer" + i, new Vector2(0, answerY[i]), new Vector2(1040, 118), new Color32(255, 252, 236, 255));
            var outline = outer.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color32(49, 48, 40, 255);
            outline.effectDistance = new Vector2(3, -3);
            var button = outer.gameObject.AddComponent<Button>();
            button.targetGraphic = outer;
            outer.gameObject.AddComponent<AnimatedButtonFeedback>();

            Text(outer.transform, "AnswerText", "Jawapan", new Vector2(-35, 0), new Vector2(650, 84), 28, TextAlignmentOptions.MidlineLeft, Green);
            var badgeOuter = Disc(outer.transform, "LetterBadge", new Vector2(-460, 0), new Vector2(96, 96), Color.white);
            var badge = Disc(badgeOuter.transform, "BadgeFill", Vector2.zero, new Vector2(78, 78), colors[i]);
            Text(badgeOuter.transform, "LetterLabel", ((char)('A' + i)).ToString(), Vector2.zero, new Vector2(76, 76), 48, TextAlignmentOptions.Center, Color.white);
            buttons[i] = button;
        }
        return buttons;
    }

    static Button MakeNext(Transform root)
    {
        var outer = Panel(root, "SeterusnyaButton", new Vector2(735, -455), new Vector2(300, 88), new Color32(255, 251, 232, 255));
        var button = outer.gameObject.AddComponent<Button>();
        button.targetGraphic = outer;
        outer.gameObject.AddComponent<AnimatedButtonFeedback>();
        var disc = Disc(outer.transform, "ArrowDisc", new Vector2(106, 0), new Vector2(74, 74), new Color32(41, 157, 31, 255));
        Text(disc.transform, "Arrow", "›", Vector2.zero, new Vector2(56, 56), 58, TextAlignmentOptions.Center, Color.white);
        Text(outer.transform, "Label", "Seterusnya", new Vector2(-30, 0), new Vector2(190, 62), 26, TextAlignmentOptions.Center, DarkText());
        return button;
    }

    static void MakeHome(Transform root)
    {
        var outer = Panel(root, "LamanUtamaButton", new Vector2(-735, -455), new Vector2(330, 88), new Color32(255, 251, 232, 255));
        outer.gameObject.AddComponent<Button>();
        outer.gameObject.AddComponent<AnimatedButtonFeedback>();
        outer.gameObject.AddComponent<Kuiz2HomeButtonRoute>();
        var disc = Disc(outer.transform, "HomeDisc", new Vector2(-118, 0), new Vector2(74, 74), new Color32(22, 149, 45, 255));
        SpriteImage(disc.transform, "HomeIcon", "Assets/Images/UI Flat/icon_home.png", Vector2.zero, new Vector2(48, 48), Color.white);
        Text(outer.transform, "Label", "Laman Utama", new Vector2(35, 0), new Vector2(210, 62), 26, TextAlignmentOptions.Center, DarkText());
    }

    static Color DarkText()
    {
        return new Color32(7, 79, 29, 255);
    }
}
