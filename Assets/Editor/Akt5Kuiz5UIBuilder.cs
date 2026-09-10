using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Akt5Kuiz5UIBuilder
{
    const string ScenePath = "Assets/Scenes/Aktivity/akt5/kuiz5.unity";
    const string BackupPath = "Assets/Scenes/Aktivity/akt5/Backups/kuiz5_PreThemeRedesign_20260904.unity";
    const string RootName = "Akt5Kuiz5ThemeRoot";

    const string BackgroundPath = "Assets/Images/Akt5CenterUI/Background/akt5_factor_background_v2.png";
    const string GreenMicrobePath = "Assets/Images/UI Flat/char_green_microbe.png";
    const string PurpleMicrobePath = "Assets/Images/UI Flat/char_purple_microbe.png";
    const string WaterPath = "Assets/Images/UI Flat/char_water_drop.png";
    const string MicroscopePath = "Assets/Images/UI Flat/img_microscope_cartoon.png";
    const string PetriPath = "Assets/Images/UI Flat/img_petri_bakteria.png";
    const string MagnifierPath = "Assets/Images/UI Flat/icon_magnify_glass.png";
    const string SpeakerPath = "Assets/Images/UI Flat/icon_speaker.png";
    const string ArrowPath = "Assets/Images/UI Flat/icon_arrow_left.png";
    const string WhitePath = "Assets/Images/UI Flat/btn_white_flat.png";
    const string CirclePath = "Assets/Images/UI Flat/ui_circle_white.png";
    const string FontPath = "Assets/Fonts/FredokaOne SDF.asset";

    static readonly Color Ink = new Color32(5, 73, 29, 255);
    static readonly Color Dark = new Color32(38, 40, 29, 255);
    static readonly Color Green = new Color32(35, 177, 16, 255);
    static readonly Color DarkGreen = new Color32(7, 108, 25, 255);
    static readonly Color Cream = new Color32(255, 250, 229, 255);
    static readonly Color PaleCream = new Color32(255, 246, 207, 255);
    static readonly Color Yellow = new Color32(255, 178, 17, 255);
    static readonly Color Blue = new Color32(40, 145, 228, 255);
    static readonly Color Purple = new Color32(145, 61, 215, 255);
    static readonly Color Coral = new Color32(239, 95, 75, 255);
    static TMP_FontAsset font;

    [MenuItem("Tools/MicroFun/Activity 5/Build Kuiz 5 Theme")]
    public static void Build()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogError("[Akt5Kuiz5] Exit Play Mode first.");
            return;
        }

        font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        if (font == null) throw new InvalidOperationException("Fredoka One font is missing.");

        BackupScene();

        Scene scene = SceneManager.GetActiveScene().path == ScenePath
            ? SceneManager.GetActiveScene()
            : EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

        GameObject canvasObject = GameObject.Find("Canvas");
        if (canvasObject == null) throw new InvalidOperationException("Canvas not found.");

        Transform canvas = canvasObject.transform;
        Transform content = canvas.Find("Content");
        Transform legacyBack = canvas.Find("Back");
        if (content == null || legacyBack == null)
            throw new InvalidOperationException("Canvas/Content or Canvas/Back not found.");

        QuizController controller = content.GetComponent<QuizController>();
        if (controller == null) throw new InvalidOperationException("QuizController not found on Canvas/Content.");

        ConfigureCanvas(canvasObject.GetComponent<Canvas>());
        BuildTheme(canvas, content, legacyBack, controller);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("[Akt5Kuiz5] Kuiz 5 theme built and controller rebound successfully.");
    }

    static void BackupScene()
    {
        const string parent = "Assets/Scenes/Aktivity/akt5";
        const string folder = parent + "/Backups";
        if (!AssetDatabase.IsValidFolder(folder)) AssetDatabase.CreateFolder(parent, "Backups");
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(BackupPath) == null)
            AssetDatabase.CopyAsset(ScenePath, BackupPath);
    }

    static void ConfigureCanvas(Canvas canvas)
    {
        if (canvas == null) throw new InvalidOperationException("Canvas component not found.");
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>() ?? canvas.gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = .5f;
    }

static void BuildTheme(Transform canvas, Transform content, Transform legacyBack, QuizController controller)
    {
        Delete(canvas, RootName);
        Delete(canvas, "Akt5Kuiz5ResultOverlay");
        RectTransform root = NewRect(RootName, canvas);
        Full(root);

        foreach (Transform child in canvas)
        {
            if (child == root || child == content || child == legacyBack) continue;
            child.gameObject.SetActive(false);
        }

        content.gameObject.SetActive(true);
        foreach (Transform child in content) child.gameObject.SetActive(false);

        Image background = NewImage("Background", root, BackgroundPath, Color.white,
            Vector2.zero, new Vector2(1920, 1080), false);
        background.preserveAspect = false;
        Full(background.rectTransform);

        BuildDecorations(root);
        BuildHeader(root, out TextMeshProUGUI indicator, out Button audioButton, out AudioSource narrationSource);
        BuildQuestion(root, out TextMeshProUGUI questionText);
        List<Transform> answerTransforms;
        List<Image> choiceIcons;
        BuildAnswers(root, out answerTransforms, out choiceIcons);
        BuildActionButton(root, out Button actionButton, out TextMeshProUGUI actionLabel);
        Transform resultOverlay = BuildResultOverlay(canvas);
        RestyleBack(legacyBack);
        BindController(controller, questionText, indicator, answerTransforms, choiceIcons,
            actionButton, actionLabel, audioButton, narrationSource, resultOverlay);
        ConfigureQuestions(controller);

        resultOverlay.gameObject.SetActive(false);
        root.SetAsLastSibling();
        legacyBack.SetAsLastSibling();
        resultOverlay.SetAsLastSibling();
    }

    static void BuildDecorations(Transform root)
    {
        Image greenLarge = NewImage("GreenMicrobeLarge", root, GreenMicrobePath,
            new Color(1, 1, 1, .43f), new Vector2(-735, 120), new Vector2(330, 330), false);
        greenLarge.preserveAspect = true;

        Image purpleLarge = NewImage("PurpleMicrobeLarge", root, PurpleMicrobePath,
            new Color(1, 1, 1, .43f), new Vector2(725, 110), new Vector2(330, 330), false);
        purpleLarge.preserveAspect = true;

        AddDecoration("GreenMicrobeSmall", root, GreenMicrobePath,
            new Vector2(-800, 310), new Vector2(145, 145), .9f);
        AddDecoration("PurpleMicrobeSmall", root, PurpleMicrobePath,
            new Vector2(800, 315), new Vector2(140, 140), .9f);
        AddDecoration("Microscope", root, MicroscopePath,
            new Vector2(-820, -355), new Vector2(200, 245), .72f);
        AddDecoration("PetriDish", root, PetriPath,
            new Vector2(-665, -440), new Vector2(205, 130), .72f);
        AddDecoration("WaterMascot", root, WaterPath,
            new Vector2(700, -365), new Vector2(145, 185), .75f);
        AddDecoration("Magnifier", root, MagnifierPath,
            new Vector2(835, -390), new Vector2(220, 220), .82f);

        BuildSpark(root, "BlueSpark", new Vector2(-455, 357), Blue);
        BuildSpark(root, "PurpleSpark", new Vector2(455, 355), Purple);
        BuildSpark(root, "GreenSpark", new Vector2(385, 285), Green);
    }

    static void AddDecoration(string name, Transform parent, string path, Vector2 position, Vector2 size, float alpha)
    {
        Image image = NewImage(name, parent, path, new Color(1, 1, 1, alpha), position, size, false);
        image.preserveAspect = true;
    }

    static void BuildHeader(Transform root, out TextMeshProUGUI indicator, out Button audioButton,
        out AudioSource narrationSource)
    {
        GameObject audioObject = NewButton("AudioButton", root, new Vector2(-760, 490),
            new Vector2(350, 72), Cream, Ink);
        audioButton = audioObject.GetComponent<Button>();
        Image audioCircle = NewImage("IconCircle", audioObject.transform, CirclePath, DarkGreen,
            new Vector2(-140, 0), new Vector2(62, 62), false);
        audioCircle.preserveAspect = true;
        OutlineGraphic(audioCircle.gameObject, Color.white, 2);
        Image speaker = NewImage("Speaker", audioCircle.transform, SpeakerPath, Color.white,
            Vector2.zero, new Vector2(42, 42), false);
        speaker.preserveAspect = true;
        NewText("Label", audioObject.transform, "Dengar audio", 24, Ink,
            TextAlignmentOptions.Center, new Vector2(30, 0), new Vector2(255, 52));

        narrationSource = audioObject.AddComponent<AudioSource>();
        narrationSource.playOnAwake = false;

        Image progress = Panel("QuestionIndicator", root, new Vector2(0, 492),
            new Vector2(310, 64), Cream, Dark, 3);
        indicator = NewText("Text", progress.transform, "Soalan 1/3", 28, Ink,
            TextAlignmentOptions.Center, Vector2.zero, new Vector2(285, 52));

        Image curriculum = Panel("CurriculumBadge", root, new Vector2(700, 492),
            new Vector2(415, 64), Cream, Dark, 3);
        NewText("Text", curriculum.transform, "SP 3.1  |  SK 3.1.4", 27, Ink,
            TextAlignmentOptions.Center, Vector2.zero, new Vector2(390, 52));

        TextMeshProUGUI title = NewText("QuizTitle", root, "KUIZ", 78, Ink,
            TextAlignmentOptions.Center, new Vector2(0, 390), new Vector2(520, 110));
        OutlineGraphic(title.gameObject, Color.white, 3);
        TextShadow(title, new Color(0, 0, 0, .22f), new Vector2(4, -5));
    }

    static void BuildQuestion(Transform root, out TextMeshProUGUI questionText)
    {
        Image card = Panel("QuestionCard", root, new Vector2(0, 267),
            new Vector2(1120, 132), Cream, Dark, 4);
        questionText = NewText("QuestionText", card.transform,
            "Soalan akan dipaparkan di sini.", 34, Ink, TextAlignmentOptions.Center,
            Vector2.zero, new Vector2(1040, 102));
        questionText.enableAutoSizing = true;
        questionText.fontSizeMin = 25;
        questionText.fontSizeMax = 34;
        questionText.overflowMode = TextOverflowModes.Ellipsis;
    }

static void BuildAnswers(Transform root, out List<Transform> answerTransforms,
        out List<Image> choiceIcons)
    {
        answerTransforms = new List<Transform>();
        choiceIcons = new List<Image>();
        float[] yPositions = { 120f, -5f, -130f, -255f };

        for (int i = 0; i < 4; i++)
        {
            string letter = ((char)('A' + i)).ToString();
            GameObject answer = NewButton("Answer" + letter, root, new Vector2(0, yPositions[i]),
                new Vector2(1120, 100), Cream, Dark);
            answerTransforms.Add(answer.transform);

            TextMeshProUGUI answerText = NewText("AnswerLabel", answer.transform, "Jawapan",
                27, Ink, TextAlignmentOptions.MidlineLeft, new Vector2(25, 0), new Vector2(850, 74));
            answerText.enableAutoSizing = true;
            answerText.fontSizeMin = 20;
            answerText.fontSizeMax = 27;
            answerText.overflowMode = TextOverflowModes.Ellipsis;

            Image badge = NewImage("LetterBadge", answer.transform, CirclePath, Green,
                new Vector2(-485, 0), new Vector2(78, 78), false);
            badge.preserveAspect = true;
            OutlineGraphic(badge.gameObject, DarkGreen, 3);
            TextMeshProUGUI badgeText = NewText("LetterLabel", badge.transform, letter, 42,
                Color.white, TextAlignmentOptions.Center, Vector2.zero, new Vector2(62, 58));
            TextShadow(badgeText, new Color(0, 0, 0, .25f), new Vector2(2, -3));

            Image icon = NewImage("ChoiceIcon", answer.transform, CirclePath, Color.white,
                new Vector2(465, 0), new Vector2(68, 68), false);
            icon.preserveAspect = true;
            icon.gameObject.SetActive(false);
            choiceIcons.Add(icon);

            if (answer.GetComponent<ButtonInteractionFeedback>() == null)
                answer.AddComponent<ButtonInteractionFeedback>();
        }
    }

static void BuildActionButton(Transform root, out Button actionButton, out TextMeshProUGUI label)
    {
        GameObject go = NewButton("PilihJawapanButton", root, new Vector2(0, -422),
            new Vector2(760, 92), Green, DarkGreen);
        actionButton = go.GetComponent<Button>();
        if (go.GetComponent<ButtonInteractionFeedback>() == null)
            go.AddComponent<ButtonInteractionFeedback>();

        label = NewText("Label", go.transform, "PILIH JAWAPAN", 39, Color.white,
            TextAlignmentOptions.Center, new Vector2(-20, 0), new Vector2(640, 68));
        TextShadow(label, new Color(0, 0, 0, .28f), new Vector2(3, -4));

        Image arrowCircle = NewImage("ArrowCircle", go.transform, CirclePath, Yellow,
            new Vector2(320, 0), new Vector2(60, 60), false);
        arrowCircle.preserveAspect = true;
        OutlineGraphic(arrowCircle.gameObject, Color.white, 2);
        Image arrow = NewImage("ForwardArrow", arrowCircle.transform, ArrowPath, Color.white,
            Vector2.zero, new Vector2(31, 31), false);
        arrow.preserveAspect = true;
        arrow.rectTransform.localRotation = Quaternion.Euler(0, 0, 180);
    }

    static Transform BuildResultOverlay(Transform root)
    {
        RectTransform overlay = NewRect("Akt5Kuiz5ResultOverlay", root);
        Full(overlay);
        Image dim = overlay.gameObject.AddComponent<Image>();
        dim.color = new Color(1, .93f, .72f, .97f);
        dim.raycastTarget = true;

        AddDecoration("ResultGreenMicrobe", overlay, GreenMicrobePath,
            new Vector2(-620, 35), new Vector2(260, 260), 1f);
        AddDecoration("ResultPurpleMicrobe", overlay, PurpleMicrobePath,
            new Vector2(620, 40), new Vector2(250, 250), 1f);

        Image panel = Panel("ResultCard", overlay, Vector2.zero,
            new Vector2(850, 560), Cream, DarkGreen, 5);
        NewText("Title", panel.transform, "TAHNIAH!", 62, Ink,
            TextAlignmentOptions.Center, new Vector2(0, 205), new Vector2(700, 90));
        NewText("Subtitle", panel.transform, "Anda telah melengkapkan Kuiz Aktiviti 5.",
            27, Dark, TextAlignmentOptions.Center, new Vector2(0, 125), new Vector2(690, 62));

        Image scoreCircle = NewImage("ScoreCircle", panel.transform, CirclePath, Green,
            new Vector2(0, -15), new Vector2(220, 220), false);
        scoreCircle.preserveAspect = true;
        OutlineGraphic(scoreCircle.gameObject, Color.white, 5);
        TextMeshProUGUI scoreText = NewText("ScoreText", scoreCircle.transform, "0 %",
            52, Color.white, TextAlignmentOptions.Center, Vector2.zero, new Vector2(180, 90));

        GameObject exit = NewButton("ExitButton", panel.transform, new Vector2(0, -205),
            new Vector2(380, 82), Green, DarkGreen);
        Button exitButton = exit.GetComponent<Button>();
        if (exit.GetComponent<ButtonInteractionFeedback>() == null)
            exit.AddComponent<ButtonInteractionFeedback>();
        exit.AddComponent<BackController>();
        NewText("Label", exit.transform, "KEMBALI", 34, Color.white,
            TextAlignmentOptions.Center, Vector2.zero, new Vector2(330, 60));

        ScoreInit scoreInit = overlay.gameObject.AddComponent<ScoreInit>();
        scoreInit.titleText = scoreText;
        scoreInit.exitButton = exitButton;

        overlay.SetAsLastSibling();
        return overlay;
    }

    static void RestyleBack(Transform legacyBack)
    {
        legacyBack.gameObject.SetActive(true);
        SetRect(legacyBack, new Vector2(-870, -485), new Vector2(72, 72));

        Image image = legacyBack.GetComponent<Image>();
        if (image != null)
        {
            image.sprite = LoadSprite(CirclePath);
            image.type = Image.Type.Simple;
            image.preserveAspect = true;
            image.color = DarkGreen;
            image.raycastTarget = true;
        }

        OutlineGraphic(legacyBack.gameObject, Color.white, 3);
        DropShadow(legacyBack.gameObject);

        foreach (Transform child in legacyBack)
            child.gameObject.SetActive(false);
        Delete(legacyBack, "ThemeArrow");

        Image arrow = NewImage("ThemeArrow", legacyBack, ArrowPath, Color.white,
            Vector2.zero, new Vector2(38, 38), false);
        arrow.preserveAspect = true;

        Button button = legacyBack.GetComponent<Button>();
        if (button != null)
        {
            button.targetGraphic = image;
            SetButtonColors(button, DarkGreen, Lighter(DarkGreen, .15f),
                Darker(DarkGreen, .12f), Color.gray);
        }

        if (legacyBack.GetComponent<ButtonInteractionFeedback>() == null)
            legacyBack.gameObject.AddComponent<ButtonInteractionFeedback>();
    }

    static void BindController(QuizController controller, TextMeshProUGUI questionText,
        TextMeshProUGUI indicator, List<Transform> answers, List<Image> choiceIcons,
        Button actionButton, TextMeshProUGUI actionLabel, Button audioButton,
        AudioSource narrationSource, Transform resultOverlay)
    {
        controller.buttonAnswers = answers;
        controller.questionText = questionText.transform;
        controller.WinPanel = resultOverlay;
        controller.indicatorText = indicator;

        SerializedObject serialized = new SerializedObject(controller);
        serialized.FindProperty("nextButton").objectReferenceValue = actionButton;
        serialized.FindProperty("useSubmitConfirmation").boolValue = true;
        serialized.FindProperty("actionButtonLabel").objectReferenceValue = actionLabel;
        serialized.FindProperty("narrationButton").objectReferenceValue = audioButton;
        serialized.FindProperty("narrationSource").objectReferenceValue = narrationSource;

        SerializedProperty iconList = serialized.FindProperty("choiceIconImages");
        iconList.arraySize = choiceIcons.Count;
        for (int i = 0; i < choiceIcons.Count; i++)
            iconList.GetArrayElementAtIndex(i).objectReferenceValue = choiceIcons[i];

        serialized.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(controller);
    }

    static void ConfigureQuestions(QuizController controller)
    {
        controller.questions = new List<QuizController.Question>
        {
            Question(
                "Antara berikut, keadaan manakah paling sesuai untuk pertumbuhan mikroorganisma?",
                2,
                "Kehadiran cahaya yang terang",
                "Keadaan suhu yang sangat rendah",
                "Kehadiran nutrien dan kelembapan yang mencukupi",
                "Keadaan persekitaran yang sangat berasid"),
            Question(
                "Mengapakah kulapuk mudah tumbuh pada roti lembap yang dibiarkan di atas meja?",
                1,
                "Kerana roti tersebut disimpan di tempat yang gelap.",
                "Kerana roti membekalkan nutrien dan mempunyai kelembapan.",
                "Kerana suhu di atas meja adalah sangat rendah.",
                "Kerana udara di sekeliling roti adalah sangat kering."),
            Question(
                "Situasi manakah menunjukkan suhu mempengaruhi pertumbuhan mikroorganisma?",
                0,
                "Susu menjadi basi apabila dibiarkan di luar peti sejuk.",
                "Buah-buahan yang dikeringkan tahan lebih lama.",
                "Penjerukan buah mangga menggunakan cuka menghalang pertumbuhan bakteria.",
                "Roti yang lembap ditumbuhi tompokan hitam.")
        };
        EditorUtility.SetDirty(controller);
    }

    static QuizController.Question Question(string text, int correctIndex, params string[] choices)
    {
        return new QuizController.Question
        {
            question = text,
            correctIndex = correctIndex,
            choices = new List<string>(choices),
            choiceIcons = new List<Sprite>()
        };
    }

    static GameObject NewButton(string name, Transform parent, Vector2 position,
        Vector2 size, Color fill, Color border)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer),
            typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        SetRect(go.transform, position, size);

        Image image = go.GetComponent<Image>();
        image.sprite = LoadSprite(WhitePath);
        image.type = Image.Type.Sliced;
        image.color = fill;
        image.raycastTarget = true;
        OutlineGraphic(go, border, 4);
        DropShadow(go);

        Button button = go.GetComponent<Button>();
        button.targetGraphic = image;
        SetButtonColors(button, fill, Lighter(fill, .08f), Darker(fill, .09f), Color.gray);
        return go;
    }

    static Image Panel(string name, Transform parent, Vector2 position, Vector2 size,
        Color fill, Color border, float width)
    {
        Image image = NewImage(name, parent, WhitePath, fill, position, size, false);
        image.type = Image.Type.Sliced;
        OutlineGraphic(image.gameObject, border, width);
        DropShadow(image.gameObject);
        return image;
    }

    static Image NewImage(string name, Transform parent, string path, Color color,
        Vector2 position, Vector2 size, bool raycast)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        SetRect(go.transform, position, size);
        Image image = go.GetComponent<Image>();
        image.sprite = LoadSprite(path);
        image.color = color;
        image.raycastTarget = raycast;
        return image;
    }

    static TextMeshProUGUI NewText(string name, Transform parent, string value, float size,
        Color color, TextAlignmentOptions alignment, Vector2 position, Vector2 rectSize)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer),
            typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        SetRect(go.transform, position, rectSize);

        TextMeshProUGUI text = go.GetComponent<TextMeshProUGUI>();
        text.font = font;
        text.text = value;
        text.fontSize = size;
        text.color = color;
        text.alignment = alignment;
        text.enableWordWrapping = true;
        text.overflowMode = TextOverflowModes.Ellipsis;
        text.raycastTarget = false;
        return text;
    }

    static Sprite LoadSprite(string path)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (sprite == null) Debug.LogWarning("[Akt5Kuiz5] Sprite missing: " + path);
        return sprite;
    }

    static RectTransform NewRect(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go.GetComponent<RectTransform>();
    }

    static void BuildSpark(Transform parent, string name, Vector2 position, Color color)
    {
        RectTransform holder = NewRect(name, parent);
        SetRect(holder, position, new Vector2(38, 38));
        Image vertical = NewImage("Vertical", holder, WhitePath, color,
            Vector2.zero, new Vector2(9, 36), false);
        vertical.type = Image.Type.Sliced;
        Image horizontal = NewImage("Horizontal", holder, WhitePath, color,
            Vector2.zero, new Vector2(36, 9), false);
        horizontal.type = Image.Type.Sliced;
    }

    static void BuildForwardTriangle(Transform parent, Color color)
    {
        RectTransform holder = NewRect("ForwardTriangle", parent);
        SetRect(holder, new Vector2(3, 0), new Vector2(30, 34));
        Image left = NewImage("Upper", holder, WhitePath, color,
            new Vector2(0, 7), new Vector2(9, 25), false);
        left.type = Image.Type.Sliced;
        left.rectTransform.localRotation = Quaternion.Euler(0, 0, -45);
        Image right = NewImage("Lower", holder, WhitePath, color,
            new Vector2(0, -7), new Vector2(9, 25), false);
        right.type = Image.Type.Sliced;
        right.rectTransform.localRotation = Quaternion.Euler(0, 0, 45);
    }

    static void Delete(Transform parent, string name)
    {
        Transform found = parent.Find(name);
        if (found != null) UnityEngine.Object.DestroyImmediate(found.gameObject);
    }

    static void SetRect(Transform transform, Vector2 position, Vector2 size)
    {
        RectTransform rect = transform as RectTransform;
        if (rect == null) return;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;
    }

    static void Full(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = Vector2.zero;
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;
    }

    static void OutlineGraphic(GameObject go, Color color, float width)
    {
        Outline outline = go.GetComponent<Outline>() ?? go.AddComponent<Outline>();
        outline.effectColor = color;
        outline.effectDistance = new Vector2(width, -width);
        outline.useGraphicAlpha = true;
    }

    static void DropShadow(GameObject go)
    {
        Shadow shadow = go.GetComponents<Shadow>().FirstOrDefault(component => !(component is Outline));
        if (shadow == null) shadow = go.AddComponent<Shadow>();
        shadow.effectColor = new Color(.12f, .07f, 0, .27f);
        shadow.effectDistance = new Vector2(0, -7);
        shadow.useGraphicAlpha = true;
    }

    static void TextShadow(TextMeshProUGUI text, Color color, Vector2 distance)
    {
        Shadow shadow = text.GetComponents<Shadow>().FirstOrDefault(component => !(component is Outline));
        if (shadow == null) shadow = text.gameObject.AddComponent<Shadow>();
        shadow.effectColor = color;
        shadow.effectDistance = distance;
        shadow.useGraphicAlpha = true;
    }

    static void SetButtonColors(Button button, Color normal, Color highlighted,
        Color pressed, Color disabled)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = normal;
        colors.highlightedColor = highlighted;
        colors.selectedColor = highlighted;
        colors.pressedColor = pressed;
        colors.disabledColor = disabled;
        colors.colorMultiplier = 1;
        colors.fadeDuration = .12f;
        button.colors = colors;
        button.transition = Selectable.Transition.ColorTint;
    }

    static Color Lighter(Color color, float amount)
    {
        return new Color(Mathf.Lerp(color.r, 1, amount), Mathf.Lerp(color.g, 1, amount),
            Mathf.Lerp(color.b, 1, amount), color.a);
    }

    static Color Darker(Color color, float amount)
    {
        return new Color(color.r * (1 - amount), color.g * (1 - amount),
            color.b * (1 - amount), color.a);
    }
}
