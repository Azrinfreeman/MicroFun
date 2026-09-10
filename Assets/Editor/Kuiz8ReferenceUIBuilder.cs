using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class Kuiz8ReferenceUIBuilder
{
    private const string ScenePath = "Assets/Scenes/Aktivity/akt8/kuiz8.unity";
    private const string WhiteCardPath = "Assets/Images/UI Flat/btn_white_flat.png";
    private const string GreenButtonPath = "Assets/Images/UI Flat/btn_green_flat.png";
    private const string BackgroundPath = "Assets/Images/Akt8CenterMenuUI/Background/akt8_center_background.png";
    private const string GreenMicrobePath = "Assets/Images/UI Flat/char_green_microbe.png";
    private const string PurpleMicrobePath = "Assets/Images/UI Flat/char_purple_microbe.png";
    private const string WaterDropPath = "Assets/Images/UI Flat/char_water_drop.png";
    private const string FlaskPath = "Assets/Images/UI Flat/icon_flask.png";
    private const string MicroscopePath = "Assets/Images/UI Flat/img_microscope_large.png";
    private const string MagnifierPath = "Assets/Images/UI Flat/icon_magnify_glass.png";
    private const string PetriPath = "Assets/Images/UI Flat/img_petri_bakteria.png";
    private const string SpeakerPath = "Assets/Images/UI Flat/icon_speaker.png";
    private const string PlayPath = "Assets/Images/UI Flat/icon_play.png";
    private const string LightPath = "Assets/Images/UI Flat/icon_lightbulb.png";
    private const string ChoicePetriPath = "Assets/Images/UI Flat/icon_petri_dish.png";
    private const string FoodPath = "Assets/Images/UI Flat/img_fun_bread.png";
    private const string FontPath = "Assets/Fonts/FredokaOne SDF.asset";

    private static readonly Color Ink = new Color(0.02f, 0.29f, 0.08f, 1f);
    private static readonly Color Cream = new Color(1f, 0.98f, 0.87f, 1f);
    private static readonly Color PaleCream = new Color(1f, 0.99f, 0.92f, 0.97f);
    private static readonly Color Green = new Color(0.19f, 0.67f, 0.04f, 1f);
    private static readonly Color DeepGreen = new Color(0.02f, 0.40f, 0.04f, 1f);
    private static readonly Color Gold = new Color(1f, 0.69f, 0.05f, 1f);
    private static readonly Color SoftGold = new Color(0.96f, 0.78f, 0.36f, 1f);

    [MenuItem("MicroFun/Activity 8/Build Kuiz 8 Reference UI")]
    public static void Build()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().path != ScenePath)
        {
            Debug.LogError("Open Assets/Scenes/Aktivity/akt8/kuiz8.unity before building the Kuiz 8 UI.");
            return;
        }

        GameObject canvasObject = GameObject.Find("Canvas");
        if (canvasObject == null)
        {
            Debug.LogError("Kuiz 8 Canvas was not found.");
            return;
        }

        Transform canvas = canvasObject.transform;
        Transform content = canvas.Find("Content");
        QuizController quiz = content == null ? null : content.GetComponent<QuizController>();
        Transform title = FindPreserved(canvas, "Title");
        Transform score = FindPreserved(canvas, "Score");
        Transform back = canvas.Find("Back");
        Transform winPanel = canvas.Find("WinPanel");

        if (content == null || quiz == null || title == null || score == null || winPanel == null)
        {
            Debug.LogError("Kuiz 8 is missing one or more required legacy UI objects.");
            return;
        }

        PreserveBeforeCleanup(canvas, content, quiz, title, score, winPanel);
        DestroyChild(canvas, "Kuiz8Backdrop");
        DestroyChild(canvas, "Kuiz8Header");
        DestroyChild(content, "QuizCardRoot");
        DestroyChild(winPanel, "Kuiz8Result");

        Transform oldBackground = canvas.Find("Image");
        if (oldBackground != null) oldBackground.gameObject.SetActive(false);
        GameObject videoPlayer = GameObject.Find("Video Player");
        if (videoPlayer != null) videoPlayer.SetActive(false);

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        if (scaler != null)
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
        }

        RemoveLegacyLayout(content.gameObject);
        BuildBackdrop(canvas);
        Transform header = BuildHeader(canvas, title, score, quiz);
        BuildQuizCards(content, quiz);
        BuildBackButton(back);
        BuildResultPanel(winPanel, quiz);

        header.SetAsLastSibling();
        content.SetAsLastSibling();
        if (back != null) back.SetAsLastSibling();
        winPanel.SetAsLastSibling();
        winPanel.gameObject.SetActive(false);

        ConfigureReferenceQuestion(quiz);
        EditorUtility.SetDirty(quiz);
        EditorSceneManager.MarkSceneDirty(canvasObject.scene);
        EditorSceneManager.SaveScene(canvasObject.scene);
        Debug.Log("Kuiz 8 reference UI built and saved.");
    }

    private static void PreserveBeforeCleanup(
        Transform canvas,
        Transform content,
        QuizController quiz,
        Transform title,
        Transform score,
        Transform winPanel)
    {
        title.SetParent(canvas, false);
        score.SetParent(canvas, false);

        if (quiz.questionText != null)
            quiz.questionText.SetParent(content, false);

        foreach (Transform answer in quiz.buttonAnswers)
        {
            if (answer != null) answer.SetParent(content, false);
        }

        ScoreInit result = winPanel.GetComponent<ScoreInit>();
        if (result != null && result.exitButton != null)
            result.exitButton.transform.SetParent(winPanel, false);
        if (result != null && result.titleText != null)
            result.titleText.transform.SetParent(winPanel, false);
    }

    private static Transform FindPreserved(Transform canvas, string name)
    {
        Transform direct = canvas.Find(name);
        if (direct != null) return direct;
        Transform header = canvas.Find("Kuiz8Header");
        return header == null ? null : header.Find(name);
    }

    private static void BuildBackdrop(Transform canvas)
    {
        Transform root = NewRect("Kuiz8Backdrop", canvas);
        Stretch(root);
        root.SetAsFirstSibling();

        GameObject backgroundObject = new GameObject(
            "WarmScienceBackground",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(RawImage));
        backgroundObject.transform.SetParent(root, false);
        Stretch(backgroundObject.transform);
        RawImage raw = backgroundObject.GetComponent<RawImage>();
        raw.texture = AssetDatabase.LoadAssetAtPath<Texture2D>(BackgroundPath);
        raw.color = Color.white;
        raw.raycastTarget = false;

        Image wash = NewImage("WarmWash", root, WhiteCardPath, new Color(1f, 0.91f, 0.59f, 0.30f));
        Stretch(wash.transform);

        Decor("GreenMicrobe", root, GreenMicrobePath, new Vector2(-765f, 250f), new Vector2(225f, 225f));
        Decor("PurpleMicrobe", root, PurpleMicrobePath, new Vector2(760f, 245f), new Vector2(210f, 210f));
        Decor("WaterDrop", root, WaterDropPath, new Vector2(725f, -275f), new Vector2(135f, 180f));
        Decor("Flask", root, FlaskPath, new Vector2(-835f, -365f), new Vector2(150f, 235f));
        Decor("Microscope", root, MicroscopePath, new Vector2(-785f, -300f), new Vector2(245f, 270f));
        Decor("Magnifier", root, MagnifierPath, new Vector2(805f, -365f), new Vector2(235f, 235f));
        Decor("PetriDish", root, PetriPath, new Vector2(-690f, -420f), new Vector2(220f, 145f));

        Star(root, "BlueStar", new Vector2(-500f, 370f), new Color(0.02f, 0.62f, 1f, 1f));
        Star(root, "GoldStar", new Vector2(-430f, 315f), Gold);
        Star(root, "PurpleStar", new Vector2(430f, 370f), new Color(0.62f, 0.23f, 0.92f, 1f));
        Star(root, "GreenStar", new Vector2(365f, 310f), Green);
    }

    private static Transform BuildHeader(Transform canvas, Transform title, Transform score, QuizController quiz)
    {
        Transform root = NewRect("Kuiz8Header", canvas);
        Stretch(root);

        title.SetParent(root, false);
        SetRect(title, new Vector2(0f, 350f), new Vector2(610f, 140f));
        Image titleImage = title.GetComponent<Image>();
        if (titleImage != null) titleImage.enabled = false;
        TextMeshProUGUI titleText = title.GetComponentInChildren<TextMeshProUGUI>(true);
        StyleText(titleText, "KUIZ", 118f, DeepGreen, TextAlignmentOptions.Center);
        titleText.outlineWidth = 0.16f;
        titleText.outlineColor = new Color32(255, 248, 218, 255);
        AddTextShadow(titleText.gameObject, new Color(0.18f, 0.17f, 0.05f, 0.32f), new Vector2(5f, -7f));

        score.SetParent(root, false);
        SetRect(score, new Vector2(0f, 482f), new Vector2(300f, 64f));
        Image scoreImage = EnsureImage(score.gameObject);
        StyleImage(scoreImage, WhiteCardPath, new Color(1f, 0.97f, 0.83f, 0.98f));
        AddCardEffects(score.gameObject, SoftGold, new Vector2(0f, -4f));
        TextMeshProUGUI progress = score.GetComponentInChildren<TextMeshProUGUI>(true);
        StyleText(progress, "Soalan 1/10", 35f, Ink, TextAlignmentOptions.Center);
        quiz.indicatorText = progress;

        Button audio = NewButton("AudioButton", root, WhiteCardPath, Cream);
        SetRect(audio.transform, new Vector2(-765f, 480f), new Vector2(340f, 84f));
        AddCardEffects(audio.gameObject, new Color(0.10f, 0.52f, 0.10f, 1f), new Vector2(0f, -5f));
        Image audioCircle = NewImage("SpeakerCircle", audio.transform, GreenButtonPath, DeepGreen);
        SetRect(audioCircle.transform, new Vector2(-128f, 0f), new Vector2(92f, 92f));
        Image speaker = NewImage("SpeakerIcon", audioCircle.transform, SpeakerPath, Color.white);
        SetRect(speaker.transform, Vector2.zero, new Vector2(50f, 50f));
        TextMeshProUGUI audioLabel = NewText("Label", audio.transform, "Dengar audio", 30f, Ink, TextAlignmentOptions.MidlineLeft);
        SetRect(audioLabel.transform, new Vector2(58f, 0f), new Vector2(220f, 76f));
        AudioSource narrationSource = audio.gameObject.GetComponent<AudioSource>();
        if (narrationSource == null) narrationSource = audio.gameObject.AddComponent<AudioSource>();
        narrationSource.playOnAwake = false;

        Image curriculum = NewImage("CurriculumPill", root, WhiteCardPath, new Color(1f, 0.97f, 0.83f, 0.98f));
        SetRect(curriculum.transform, new Vector2(665f, 482f), new Vector2(390f, 64f));
        AddCardEffects(curriculum.gameObject, SoftGold, new Vector2(0f, -4f));
        TextMeshProUGUI curriculumText = NewText(
            "Label",
            curriculum.transform,
            "SP 3.1  |  SK 3.1.4",
            32f,
            Ink,
            TextAlignmentOptions.Center);
        Stretch(curriculumText.transform);

        SetPrivateReference(quiz, "narrationButton", audio);
        SetPrivateReference(quiz, "narrationSource", narrationSource);
        return root;
    }

    private static void BuildQuizCards(Transform content, QuizController quiz)
    {
        Transform oldPanel = content.Find("bg");

        Transform root = NewRect("QuizCardRoot", content);
        Stretch(root);
        root.SetAsLastSibling();

        Image questionCard = NewImage("QuestionCard", root, WhiteCardPath, PaleCream);
        SetRect(questionCard.transform, new Vector2(0f, 188f), new Vector2(1100f, 155f));
        AddCardEffects(questionCard.gameObject, new Color(0.86f, 0.71f, 0.40f, 0.70f), new Vector2(0f, -7f));

        TextMeshProUGUI questionLabel = quiz.questionText == null
            ? null
            : quiz.questionText.GetComponent<TextMeshProUGUI>();
        if (questionLabel == null)
            questionLabel = NewText("QuestionLabel", questionCard.transform, "Apakah definisi mikroorganisma?", 45f, Ink, TextAlignmentOptions.Center);
        else
            questionLabel.transform.SetParent(questionCard.transform, false);

        questionLabel.gameObject.name = "QuestionLabel";
        SetRect(questionLabel.transform, Vector2.zero, new Vector2(1000f, 125f));
        StyleText(questionLabel, questionLabel.text, 45f, Ink, TextAlignmentOptions.Center);
        questionLabel.enableAutoSizing = true;
        questionLabel.fontSizeMin = 31f;
        questionLabel.fontSizeMax = 45f;
        quiz.questionText = questionLabel.transform;

        Transform answerRoot = NewRect("AnswerCards", root);
        Stretch(answerRoot);
        List<Image> iconSlots = new List<Image>();

        for (int i = 0; i < quiz.buttonAnswers.Count; i++)
        {
            Transform answer = quiz.buttonAnswers[i];
            if (answer == null) continue;

            if (i >= 3)
            {
                answer.SetParent(root, false);
                answer.gameObject.SetActive(false);
                continue;
            }

            answer.SetParent(answerRoot, false);
            answer.gameObject.name = "Answer" + (char)('A' + i);
            SetRect(answer, new Vector2(0f, 10f - (150f * i)), new Vector2(1120f, 126f));

            RemoveLegacyLayout(answer.gameObject);
            Image answerImage = EnsureImage(answer.gameObject);
            StyleImage(answerImage, WhiteCardPath, Cream);
            AddCardEffects(answer.gameObject, new Color(0.83f, 0.68f, 0.35f, 0.70f), new Vector2(0f, -6f));

            Button button = answer.GetComponent<Button>();
            if (button == null) button = answer.gameObject.AddComponent<Button>();
            button.targetGraphic = answerImage;
            button.transition = Selectable.Transition.ColorTint;
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(0.95f, 1f, 0.88f, 1f);
            colors.pressedColor = new Color(0.82f, 0.94f, 0.70f, 1f);
            colors.selectedColor = new Color(0.90f, 0.98f, 0.75f, 1f);
            colors.disabledColor = new Color(0.90f, 0.92f, 0.84f, 1f);
            button.colors = colors;
            if (answer.GetComponent<ButtonInteractionFeedback>() == null)
                answer.gameObject.AddComponent<ButtonInteractionFeedback>();

            TextMeshProUGUI answerText = answer.GetComponentInChildren<TextMeshProUGUI>(true);
            if (answerText == null)
                answerText = NewText("AnswerText", answer, string.Empty, 39f, Ink, TextAlignmentOptions.MidlineLeft);
            answerText.gameObject.name = "AnswerText";
            SetRect(answerText.transform, new Vector2(-15f, 0f), new Vector2(790f, 106f));
            StyleText(answerText, answerText.text, 39f, Ink, TextAlignmentOptions.MidlineLeft);
            answerText.enableAutoSizing = true;
            answerText.fontSizeMin = 27f;
            answerText.fontSizeMax = 39f;

            DestroyChild(answer, "LetterBadge");
            Image badge = NewImage("LetterBadge", answer, GreenButtonPath, Green);
            SetRect(badge.transform, new Vector2(-485f, 0f), new Vector2(94f, 94f));
            AddCardEffects(badge.gameObject, DeepGreen, new Vector2(0f, -3f));
            TextMeshProUGUI letter = NewText(
                "LetterLabel",
                badge.transform,
                ((char)('A' + i)).ToString(),
                55f,
                Color.white,
                TextAlignmentOptions.Center);
            Stretch(letter.transform);
            letter.outlineWidth = 0.08f;
            letter.outlineColor = new Color32(12, 90, 15, 255);

            Image icon = NewImage("ChoiceIcon", answer, WhiteCardPath, Color.white);
            SetRect(icon.transform, new Vector2(480f, 0f), new Vector2(112f, 98f));
            icon.preserveAspect = true;
            icon.raycastTarget = false;
            icon.enabled = false;
            icon.gameObject.SetActive(false);
            iconSlots.Add(icon);
        }

        Button action = NewButton("PilihJawapanButton", root, GreenButtonPath, Green);
        SetRect(action.transform, new Vector2(0f, -455f), new Vector2(735f, 100f));
        AddCardEffects(action.gameObject, new Color(0.00f, 0.28f, 0.03f, 1f), new Vector2(0f, -9f));
        TextMeshProUGUI actionLabel = NewText(
            "ActionLabel",
            action.transform,
            "PILIH JAWAPAN",
            52f,
            Color.white,
            TextAlignmentOptions.Center);
        SetRect(actionLabel.transform, new Vector2(-20f, 2f), new Vector2(620f, 88f));
        actionLabel.outlineWidth = 0.10f;
        actionLabel.outlineColor = new Color32(11, 86, 11, 255);
        Image arrow = NewImage("Arrow", action.transform, PlayPath, Gold);
        SetRect(arrow.transform, new Vector2(310f, 1f), new Vector2(56f, 56f));
        arrow.preserveAspect = true;
        if (action.GetComponent<ButtonInteractionFeedback>() == null)
            action.gameObject.AddComponent<ButtonInteractionFeedback>();

        SerializedObject serializedQuiz = new SerializedObject(quiz);
        serializedQuiz.FindProperty("useSubmitConfirmation").boolValue = true;
        serializedQuiz.FindProperty("nextButton").objectReferenceValue = action;
        serializedQuiz.FindProperty("actionButtonLabel").objectReferenceValue = actionLabel;
        SerializedProperty icons = serializedQuiz.FindProperty("choiceIconImages");
        icons.arraySize = iconSlots.Count;
        for (int i = 0; i < iconSlots.Count; i++)
            icons.GetArrayElementAtIndex(i).objectReferenceValue = iconSlots[i];
        serializedQuiz.ApplyModifiedPropertiesWithoutUndo();

        if (oldPanel != null) Object.DestroyImmediate(oldPanel.gameObject);
    }

    private static void BuildBackButton(Transform back)
    {
        if (back == null) return;

        back.SetParent(GameObject.Find("Canvas").transform, false);
        SetRect(back, new Vector2(-890f, -494f), new Vector2(66f, 66f));
        RemoveLegacyLayout(back.gameObject);
        Image image = EnsureImage(back.gameObject);
        StyleImage(image, GreenButtonPath, new Color(0.11f, 0.52f, 0.08f, 0.94f));
        Button button = back.GetComponent<Button>();
        if (button != null) button.targetGraphic = image;
        TextMeshProUGUI label = back.GetComponentInChildren<TextMeshProUGUI>(true);
        if (label != null)
        {
            SetRect(label.transform, Vector2.zero, new Vector2(58f, 58f));
            StyleText(label, "‹", 53f, Color.white, TextAlignmentOptions.Center);
        }
    }

    private static void BuildResultPanel(Transform winPanel, QuizController quiz)
    {
        ScoreInit result = winPanel.GetComponent<ScoreInit>();
        Button exitButton = result == null ? null : result.exitButton;
        TextMeshProUGUI resultTitle = result == null ? null : result.titleText;

        foreach (Transform child in new List<Transform>(GetChildren(winPanel)))
        {
            if (exitButton != null && child == exitButton.transform) continue;
            if (resultTitle != null && child == resultTitle.transform) continue;
            Object.DestroyImmediate(child.gameObject);
        }

        Transform root = NewRect("Kuiz8Result", winPanel);
        Stretch(root);
        Image veil = NewImage("Veil", root, WhiteCardPath, new Color(0.03f, 0.20f, 0.05f, 0.58f));
        Stretch(veil.transform);

        Image card = NewImage("ResultCard", root, WhiteCardPath, new Color(1f, 0.98f, 0.86f, 1f));
        SetRect(card.transform, Vector2.zero, new Vector2(720f, 470f));
        AddCardEffects(card.gameObject, DeepGreen, new Vector2(0f, -12f));

        TextMeshProUGUI heading = NewText("Heading", card.transform, "TAHNIAH!", 69f, DeepGreen, TextAlignmentOptions.Center);
        SetRect(heading.transform, new Vector2(0f, 145f), new Vector2(620f, 90f));

        if (resultTitle == null)
            resultTitle = NewText("ScorePercent", card.transform, "0 %", 100f, Green, TextAlignmentOptions.Center);
        else
            resultTitle.transform.SetParent(card.transform, false);
        resultTitle.gameObject.name = "ScorePercent";
        SetRect(resultTitle.transform, new Vector2(0f, 30f), new Vector2(520f, 120f));
        StyleText(resultTitle, resultTitle.text, 100f, Green, TextAlignmentOptions.Center);

        TextMeshProUGUI message = NewText(
            "Message",
            card.transform,
            "Syabas! Anda telah menamatkan Kuiz 8.",
            31f,
            Ink,
            TextAlignmentOptions.Center);
        SetRect(message.transform, new Vector2(0f, -65f), new Vector2(590f, 70f));

        if (exitButton == null)
        {
            exitButton = NewButton("ExitBtn", card.transform, GreenButtonPath, Green);
        }
        else
        {
            exitButton.transform.SetParent(card.transform, false);
            Image exitImage = EnsureImage(exitButton.gameObject);
            StyleImage(exitImage, GreenButtonPath, Green);
            exitButton.targetGraphic = exitImage;
        }

        SetRect(exitButton.transform, new Vector2(0f, -160f), new Vector2(380f, 82f));
        AddCardEffects(exitButton.gameObject, DeepGreen, new Vector2(0f, -7f));
        TextMeshProUGUI exitLabel = exitButton.GetComponentInChildren<TextMeshProUGUI>(true);
        if (exitLabel == null)
            exitLabel = NewText("Label", exitButton.transform, "KEMBALI", 39f, Color.white, TextAlignmentOptions.Center);
        else
            StyleText(exitLabel, "KEMBALI", 39f, Color.white, TextAlignmentOptions.Center);
        Stretch(exitLabel.transform);

        if (result != null)
        {
            result.titleText = resultTitle;
            result.exitButton = exitButton;
            EditorUtility.SetDirty(result);
        }

        quiz.WinPanel = winPanel;
    }

    private static void ConfigureReferenceQuestion(QuizController quiz)
    {
        if (quiz.questions == null || quiz.questions.Count < 5) return;

        QuizController.Question questionFive = quiz.questions[4];
        questionFive.question = "Faktor manakah mempengaruhi\npertumbuhan mikroorganisma?";
        questionFive.correctIndex = 1;
        questionFive.choiceIcons = new List<Sprite>
        {
            AssetDatabase.LoadAssetAtPath<Sprite>(LightPath),
            AssetDatabase.LoadAssetAtPath<Sprite>(ChoicePetriPath),
            AssetDatabase.LoadAssetAtPath<Sprite>(FoodPath)
        };
    }

    private static void SetPrivateReference(Object target, string fieldName, Object value)
    {
        SerializedObject serialized = new SerializedObject(target);
        SerializedProperty property = serialized.FindProperty(fieldName);
        if (property != null)
        {
            property.objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }
    }

    private static void Decor(string name, Transform parent, string path, Vector2 position, Vector2 size)
    {
        Image image = NewImage(name, parent, path, Color.white);
        SetRect(image.transform, position, size);
        image.preserveAspect = true;
        image.raycastTarget = false;
    }

    private static void Star(Transform parent, string name, Vector2 position, Color color)
    {
        TextMeshProUGUI star = NewText(name, parent, "+", 48f, color, TextAlignmentOptions.Center);
        SetRect(star.transform, position, new Vector2(60f, 60f));
    }

    private static Transform NewRect(string name, Transform parent)
    {
        GameObject gameObject = new GameObject(name, typeof(RectTransform));
        gameObject.layer = LayerMask.NameToLayer("UI");
        gameObject.transform.SetParent(parent, false);
        return gameObject.transform;
    }

    private static Image NewImage(string name, Transform parent, string spritePath, Color color)
    {
        GameObject gameObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        gameObject.layer = LayerMask.NameToLayer("UI");
        gameObject.transform.SetParent(parent, false);
        Image image = gameObject.GetComponent<Image>();
        StyleImage(image, spritePath, color);
        image.raycastTarget = false;
        return image;
    }

    private static Button NewButton(string name, Transform parent, string spritePath, Color color)
    {
        Image image = NewImage(name, parent, spritePath, color);
        image.raycastTarget = true;
        Button button = image.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        return button;
    }

    private static TextMeshProUGUI NewText(
        string name,
        Transform parent,
        string value,
        float size,
        Color color,
        TextAlignmentOptions alignment)
    {
        GameObject gameObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        gameObject.layer = LayerMask.NameToLayer("UI");
        gameObject.transform.SetParent(parent, false);
        TextMeshProUGUI text = gameObject.GetComponent<TextMeshProUGUI>();
        StyleText(text, value, size, color, alignment);
        return text;
    }

    private static void StyleImage(Image image, string spritePath, Color color)
    {
        image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
        image.type = Image.Type.Sliced;
        image.color = color;
    }

    private static void StyleText(
        TextMeshProUGUI text,
        string value,
        float size,
        Color color,
        TextAlignmentOptions alignment)
    {
        if (text == null) return;
        TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        if (font != null) text.font = font;
        text.text = value;
        text.fontSize = size;
        text.color = color;
        text.alignment = alignment;
        text.enableWordWrapping = true;
        text.raycastTarget = false;
        text.fontStyle = FontStyles.Normal;
    }

    private static Image EnsureImage(GameObject gameObject)
    {
        Image image = gameObject.GetComponent<Image>();
        if (image == null) image = gameObject.AddComponent<Image>();
        return image;
    }

    private static void AddCardEffects(GameObject gameObject, Color outlineColor, Vector2 shadowDistance)
    {
        Outline outline = gameObject.GetComponent<Outline>();
        if (outline == null) outline = gameObject.AddComponent<Outline>();
        outline.effectColor = outlineColor;
        outline.effectDistance = new Vector2(2f, -2f);
        outline.useGraphicAlpha = true;

        Shadow shadow = gameObject.GetComponent<Shadow>();
        if (shadow == null || shadow == outline)
        {
            Shadow[] shadows = gameObject.GetComponents<Shadow>();
            shadow = shadows.Length > 1 ? shadows[1] : gameObject.AddComponent<Shadow>();
        }
        shadow.effectColor = new Color(0.20f, 0.16f, 0.05f, 0.28f);
        shadow.effectDistance = shadowDistance;
        shadow.useGraphicAlpha = true;
    }

    private static void AddTextShadow(GameObject gameObject, Color color, Vector2 distance)
    {
        Shadow shadow = gameObject.GetComponent<Shadow>();
        if (shadow == null) shadow = gameObject.AddComponent<Shadow>();
        shadow.effectColor = color;
        shadow.effectDistance = distance;
        shadow.useGraphicAlpha = true;
    }

    private static void SetRect(Transform target, Vector2 position, Vector2 size)
    {
        RectTransform rect = target as RectTransform;
        if (rect == null) return;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;
    }

    private static void Stretch(Transform target)
    {
        RectTransform rect = target as RectTransform;
        if (rect == null) return;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;
    }

    private static void RemoveLegacyLayout(GameObject gameObject)
    {
        HorizontalOrVerticalLayoutGroup[] groups = gameObject.GetComponents<HorizontalOrVerticalLayoutGroup>();
        foreach (HorizontalOrVerticalLayoutGroup group in groups)
            Object.DestroyImmediate(group);

        ContentSizeFitter fitter = gameObject.GetComponent<ContentSizeFitter>();
        if (fitter != null) Object.DestroyImmediate(fitter);

        LayoutElement layout = gameObject.GetComponent<LayoutElement>();
        if (layout != null) Object.DestroyImmediate(layout);
    }

    private static void DestroyChild(Transform parent, string name)
    {
        if (parent == null) return;
        Transform child = parent.Find(name);
        if (child != null) Object.DestroyImmediate(child.gameObject);
    }

    private static List<Transform> GetChildren(Transform parent)
    {
        List<Transform> children = new List<Transform>();
        for (int i = 0; i < parent.childCount; i++)
            children.Add(parent.GetChild(i));
        return children;
    }
}
