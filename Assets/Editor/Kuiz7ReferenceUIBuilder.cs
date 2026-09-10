#if UNITY_EDITOR
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Rebuilds Kuiz 7 into the warm science-themed reference layout while
/// preserving the scene's QuizController, question data, result flow and BackController.
/// </summary>
public static class Kuiz7ReferenceUIBuilder
{
    private const string ScenePath = "Assets/Scenes/Aktivity/akt7/kuiz7.unity";
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
    private const string FontPath = "Assets/Fonts/FredokaOne SDF.asset";

    private static readonly Color Ink = new Color(0.02f, 0.29f, 0.08f, 1f);
    private static readonly Color Cream = new Color(1f, 0.98f, 0.87f, 1f);
    private static readonly Color PaleCream = new Color(1f, 0.99f, 0.93f, 0.97f);
    private static readonly Color Green = new Color(0.14f, 0.66f, 0.03f, 1f);
    private static readonly Color DeepGreen = new Color(0.02f, 0.38f, 0.04f, 1f);
    private static readonly Color Gold = new Color(1f, 0.64f, 0.02f, 1f);
    private static readonly Color SoftGold = new Color(0.93f, 0.73f, 0.31f, 0.9f);

    private static TMP_FontAsset font;
    private static Sprite whiteCard;
    private static Sprite greenButton;

    [MenuItem("Tools/MicroFun/Activity 7/Build Kuiz 7 Reference UI")]
    public static void Build()
    {
        if (EditorSceneManager.GetActiveScene().path != ScenePath)
        {
            Debug.LogError("Kuiz7ReferenceUIBuilder: open " + ScenePath + " before rebuilding.");
            return;
        }

        GameObject canvasObject = GameObject.Find("Canvas");
        Transform canvas = canvasObject != null ? canvasObject.transform : null;
        Transform content = canvas != null ? canvas.Find("Content") : null;
        QuizController quiz = content != null ? content.GetComponent<QuizController>() : null;
        Transform title = FindPreserved(canvas, "Title");
        Transform score = FindPreserved(canvas, "Score");
        Transform back = canvas != null ? canvas.Find("Back") : null;
        Transform winPanel = canvas != null ? canvas.Find("WinPanel") : null;

        if (canvas == null || content == null || quiz == null || title == null || score == null || back == null || winPanel == null)
        {
            Debug.LogError("Kuiz7ReferenceUIBuilder: required Kuiz 7 objects were not found.");
            return;
        }

        font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        whiteCard = AssetDatabase.LoadAssetAtPath<Sprite>(WhiteCardPath);
        greenButton = AssetDatabase.LoadAssetAtPath<Sprite>(GreenButtonPath);
        if (font == null || whiteCard == null || greenButton == null)
        {
            Debug.LogError("Kuiz7ReferenceUIBuilder: required font or UI sprites are missing.");
            return;
        }

        PreserveFunctionalObjects(canvas, content, quiz, title, score);
        DestroyChild(canvas, "Kuiz7Backdrop");
        DestroyChild(canvas, "Kuiz7Header");
        DestroyChild(content, "QuizCardRoot");

        Transform legacyBackground = canvas.Find("Image");
        if (legacyBackground != null) legacyBackground.gameObject.SetActive(false);
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

        RectTransform contentRect = content as RectTransform;
        contentRect.anchorMin = Vector2.zero;
        contentRect.anchorMax = Vector2.one;
        contentRect.offsetMin = Vector2.zero;
        contentRect.offsetMax = Vector2.zero;
        contentRect.localScale = Vector3.one;
        RemoveLayout(content.gameObject);

        BuildBackdrop(canvas);
        Transform header = BuildHeader(canvas, title, score, quiz);
        BuildQuizCards(content, quiz);
        BuildBackButton(back);

        header.SetAsLastSibling();
        content.SetAsLastSibling();
        back.SetAsLastSibling();
        winPanel.SetAsLastSibling();
        winPanel.gameObject.SetActive(false);

        EditorUtility.SetDirty(quiz);
        EditorUtility.SetDirty(canvasObject);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Selection.activeGameObject = content.gameObject;
        Debug.Log("Kuiz 7 reference UI built and saved. Existing question data and quiz flow were preserved.");
    }

    private static void PreserveFunctionalObjects(Transform canvas, Transform content, QuizController quiz, Transform title, Transform score)
    {
        title.SetParent(canvas, false);
        score.SetParent(canvas, false);
        if (quiz.questionText != null) quiz.questionText.SetParent(content, false);
        foreach (Transform answer in quiz.buttonAnswers)
            if (answer != null) answer.SetParent(content, false);
    }

    private static Transform FindPreserved(Transform canvas, string objectName)
    {
        if (canvas == null) return null;
        Transform direct = canvas.Find(objectName);
        if (direct != null) return direct;
        Transform header = canvas.Find("Kuiz7Header");
        return header != null ? header.Find(objectName) : null;
    }

    private static void BuildBackdrop(Transform canvas)
    {
        Transform root = NewRect("Kuiz7Backdrop", canvas);
        Stretch(root);
        root.SetAsFirstSibling();

        GameObject backgroundObject = new GameObject("WarmScienceBackground", typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
        backgroundObject.layer = canvas.gameObject.layer;
        backgroundObject.transform.SetParent(root, false);
        Stretch(backgroundObject.transform);
        RawImage raw = backgroundObject.GetComponent<RawImage>();
        raw.texture = AssetDatabase.LoadAssetAtPath<Texture2D>(BackgroundPath);
        raw.color = Color.white;
        raw.raycastTarget = false;

        Image wash = NewImage("WarmWash", root, whiteCard, new Color(1f, 0.92f, 0.58f, 0.27f));
        Stretch(wash.transform);

        Decor(root, "GreenMicrobe", GreenMicrobePath, new Vector2(-750f, 210f), new Vector2(225f, 225f), 0.72f);
        Decor(root, "PurpleMicrobe", PurpleMicrobePath, new Vector2(750f, 210f), new Vector2(220f, 220f), 0.72f);
        Decor(root, "WaterDrop", WaterDropPath, new Vector2(710f, -300f), new Vector2(130f, 175f), 0.9f);
        Decor(root, "Flask", FlaskPath, new Vector2(-850f, -365f), new Vector2(145f, 225f), 0.9f);
        Decor(root, "Microscope", MicroscopePath, new Vector2(-765f, -305f), new Vector2(255f, 280f), 0.82f);
        Decor(root, "Magnifier", MagnifierPath, new Vector2(820f, -365f), new Vector2(225f, 225f), 0.86f);
        Decor(root, "PetriDish", PetriPath, new Vector2(-675f, -425f), new Vector2(215f, 140f), 0.86f);

        Bubble(root, "Bubble01", new Vector2(-500f, 410f), 44f);
        Bubble(root, "Bubble02", new Vector2(500f, 425f), 32f);
        Bubble(root, "Bubble03", new Vector2(865f, 55f), 24f);
        Bubble(root, "Bubble04", new Vector2(-870f, 40f), 22f);
        Spark(root, "BlueSpark", new Vector2(-485f, 335f), new Color(0.03f, 0.61f, 1f, 1f));
        Spark(root, "PurpleSpark", new Vector2(445f, 330f), new Color(0.62f, 0.22f, 0.93f, 1f));
        Spark(root, "GreenSpark", new Vector2(375f, 270f), Green);
    }

    private static Transform BuildHeader(Transform canvas, Transform title, Transform score, QuizController quiz)
    {
        Transform root = NewRect("Kuiz7Header", canvas);
        Stretch(root);

        title.SetParent(root, false);
        SetRect(title, new Vector2(0f, 355f), new Vector2(610f, 130f));
        Image titleImage = title.GetComponent<Image>();
        if (titleImage != null) titleImage.enabled = false;
        TextMeshProUGUI titleText = title.GetComponentInChildren<TextMeshProUGUI>(true);
        StyleText(titleText, "KUIZ", 112f, DeepGreen, TextAlignmentOptions.Center);
        titleText.outlineWidth = 0.14f;
        titleText.outlineColor = new Color32(255, 249, 220, 255);
        ReplaceEffects(titleText.gameObject, new Color(0.18f, 0.16f, 0.05f, 0.28f), new Vector2(5f, -7f), false);

        score.SetParent(root, false);
        SetRect(score, new Vector2(0f, 485f), new Vector2(300f, 64f));
        Image scoreImage = EnsureImage(score.gameObject);
        StyleImage(scoreImage, whiteCard, new Color(1f, 0.97f, 0.83f, 0.98f));
        ReplaceEffects(score.gameObject, SoftGold, new Vector2(0f, -4f), true);
        TextMeshProUGUI progress = score.GetComponentInChildren<TextMeshProUGUI>(true);
        int total = quiz.questions != null ? quiz.questions.Count : 0;
        StyleText(progress, "Soalan 1/" + total, 34f, Ink, TextAlignmentOptions.Center);
        quiz.indicatorText = progress;

        Button audio = NewButton("AudioButton", root, whiteCard, Cream);
        SetRect(audio.transform, new Vector2(-765f, 480f), new Vector2(340f, 84f));
        AddOutline(audio.gameObject, new Color(0.10f, 0.50f, 0.10f, 1f), new Vector2(3f, 3f));
        AddShadow(audio.gameObject, new Color(0.12f, 0.25f, 0.06f, 0.28f), new Vector2(0f, -5f));
        Image audioCircle = NewImage("SpeakerCircle", audio.transform, greenButton, DeepGreen);
        SetRect(audioCircle.transform, new Vector2(-128f, 0f), new Vector2(92f, 92f));
        Image speaker = NewImage("SpeakerIcon", audioCircle.transform, LoadSprite(SpeakerPath), Color.white);
        SetRect(speaker.transform, Vector2.zero, new Vector2(50f, 50f));
        TextMeshProUGUI audioLabel = NewText("Label", audio.transform, "Dengar audio", 30f, Ink, TextAlignmentOptions.MidlineLeft);
        SetRect(audioLabel.transform, new Vector2(58f, 0f), new Vector2(220f, 76f));
        AudioSource narration = audio.gameObject.AddComponent<AudioSource>();
        narration.playOnAwake = false;

        Image curriculum = NewImage("CurriculumPill", root, whiteCard, new Color(1f, 0.97f, 0.83f, 0.98f));
        SetRect(curriculum.transform, new Vector2(665f, 485f), new Vector2(390f, 64f));
        AddOutline(curriculum.gameObject, SoftGold, new Vector2(2f, 2f));
        AddShadow(curriculum.gameObject, new Color(0.18f, 0.16f, 0.05f, 0.20f), new Vector2(0f, -4f));
        TextMeshProUGUI curriculumText = NewText("Label", curriculum.transform, "SP 3.1  |  SK 3.1.4", 32f, Ink, TextAlignmentOptions.Center);
        Stretch(curriculumText.transform);

        SetPrivateReference(quiz, "narrationButton", audio);
        SetPrivateReference(quiz, "narrationSource", narration);
        return root;
    }

    private static void BuildQuizCards(Transform content, QuizController quiz)
    {
        Transform oldPanel = content.Find("bg");
        Transform root = NewRect("QuizCardRoot", content);
        Stretch(root);

        Image questionCard = NewImage("QuestionCard", root, whiteCard, PaleCream);
        SetRect(questionCard.transform, new Vector2(0f, 165f), new Vector2(1100f, 150f));
        AddOutline(questionCard.gameObject, new Color(0.17f, 0.17f, 0.12f, 0.88f), new Vector2(4f, 4f));
        AddShadow(questionCard.gameObject, new Color(0.24f, 0.19f, 0.08f, 0.28f), new Vector2(0f, -8f));

        TextMeshProUGUI questionLabel = quiz.questionText != null ? quiz.questionText.GetComponent<TextMeshProUGUI>() : null;
        if (questionLabel == null)
            questionLabel = NewText("QuestionLabel", questionCard.transform, "Apakah definisi mikroorganisma?", 44f, Ink, TextAlignmentOptions.Center);
        else
            questionLabel.transform.SetParent(questionCard.transform, false);
        questionLabel.gameObject.name = "QuestionLabel";
        SetRect(questionLabel.transform, Vector2.zero, new Vector2(1010f, 118f));
        StyleText(questionLabel, questionLabel.text, 44f, Ink, TextAlignmentOptions.Center);
        questionLabel.enableAutoSizing = true;
        questionLabel.fontSizeMin = 29f;
        questionLabel.fontSizeMax = 44f;
        questionLabel.enableWordWrapping = true;
        quiz.questionText = questionLabel.transform;

        List<Image> iconSlots = new List<Image>();
        for (int i = 0; i < quiz.buttonAnswers.Count; i++)
        {
            Transform answer = quiz.buttonAnswers[i];
            if (answer == null) continue;
            RemoveLayout(answer.gameObject);
            answer.SetParent(root, false);
            if (i >= 3)
            {
                answer.gameObject.SetActive(false);
                continue;
            }

            answer.gameObject.name = "Answer" + (char)('A' + i);
            SetRect(answer, new Vector2(0f, -15f - (132f * i)), new Vector2(1100f, 108f));
            Image answerImage = EnsureImage(answer.gameObject);
            StyleImage(answerImage, whiteCard, Cream);
            RemoveEffects(answer.gameObject);
            AddOutline(answer.gameObject, new Color(0.17f, 0.17f, 0.12f, 0.88f), new Vector2(4f, 4f));
            AddShadow(answer.gameObject, new Color(0.24f, 0.19f, 0.08f, 0.28f), new Vector2(0f, -7f));

            Button button = answer.GetComponent<Button>();
            if (button == null) button = answer.gameObject.AddComponent<Button>();
            button.targetGraphic = answerImage;
            button.transition = Selectable.Transition.ColorTint;
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(0.94f, 1f, 0.86f, 1f);
            colors.pressedColor = new Color(0.80f, 0.94f, 0.66f, 1f);
            colors.selectedColor = new Color(0.88f, 0.98f, 0.72f, 1f);
            colors.disabledColor = new Color(0.88f, 0.90f, 0.82f, 1f);
            button.colors = colors;

            TextMeshProUGUI answerText = answer.GetComponentInChildren<TextMeshProUGUI>(true);
            if (answerText == null) answerText = NewText("AnswerText", answer, string.Empty, 37f, Ink, TextAlignmentOptions.MidlineLeft);
            answerText.gameObject.name = "AnswerText";
            SetRect(answerText.transform, new Vector2(35f, 0f), new Vector2(850f, 88f));
            StyleText(answerText, answerText.text, 37f, Ink, TextAlignmentOptions.MidlineLeft);
            answerText.enableAutoSizing = true;
            answerText.fontSizeMin = 25f;
            answerText.fontSizeMax = 37f;
            answerText.enableWordWrapping = true;

            DestroyChild(answer, "LetterBadge");
            Image badge = NewImage("LetterBadge", answer, greenButton, Green);
            SetRect(badge.transform, new Vector2(-485f, 0f), new Vector2(84f, 84f));
            AddOutline(badge.gameObject, DeepGreen, new Vector2(3f, 3f));
            TextMeshProUGUI letter = NewText("LetterLabel", badge.transform, ((char)('A' + i)).ToString(), 50f, Color.white, TextAlignmentOptions.Center);
            Stretch(letter.transform);
            letter.outlineWidth = 0.08f;
            letter.outlineColor = new Color32(9, 72, 14, 255);

            Image icon = NewImage("ChoiceIcon", answer, null, Color.white);
            SetRect(icon.transform, new Vector2(480f, 0f), new Vector2(96f, 82f));
            icon.preserveAspect = true;
            icon.raycastTarget = false;
            icon.enabled = false;
            icon.gameObject.SetActive(false);
            iconSlots.Add(icon);
        }

        Button action = NewButton("PilihJawapanButton", root, greenButton, Green);
        SetRect(action.transform, new Vector2(0f, -447f), new Vector2(730f, 96f));
        AddOutline(action.gameObject, DeepGreen, new Vector2(4f, 4f));
        AddShadow(action.gameObject, new Color(0f, 0.24f, 0.02f, 0.48f), new Vector2(0f, -9f));
        TextMeshProUGUI actionLabel = NewText("ActionLabel", action.transform, "PILIH JAWAPAN", 50f, Color.white, TextAlignmentOptions.Center);
        SetRect(actionLabel.transform, new Vector2(-18f, 2f), new Vector2(620f, 82f));
        actionLabel.outlineWidth = 0.10f;
        actionLabel.outlineColor = new Color32(10, 78, 12, 255);
        Image arrow = NewImage("Arrow", action.transform, LoadSprite(PlayPath), Gold);
        SetRect(arrow.transform, new Vector2(310f, 0f), new Vector2(52f, 52f));

        SerializedObject serializedQuiz = new SerializedObject(quiz);
        serializedQuiz.FindProperty("useSubmitConfirmation").boolValue = true;
        serializedQuiz.FindProperty("nextButton").objectReferenceValue = action;
        serializedQuiz.FindProperty("actionButtonLabel").objectReferenceValue = actionLabel;
        SerializedProperty icons = serializedQuiz.FindProperty("choiceIconImages");
        icons.arraySize = iconSlots.Count;
        for (int i = 0; i < iconSlots.Count; i++) icons.GetArrayElementAtIndex(i).objectReferenceValue = iconSlots[i];
        serializedQuiz.ApplyModifiedPropertiesWithoutUndo();

        if (oldPanel != null) Object.DestroyImmediate(oldPanel.gameObject);
    }

    private static void BuildBackButton(Transform back)
    {
        SetRect(back, new Vector2(-890f, -494f), new Vector2(66f, 66f));
        RemoveLayout(back.gameObject);
        Image image = EnsureImage(back.gameObject);
        StyleImage(image, greenButton, new Color(0.11f, 0.52f, 0.08f, 0.96f));
        RemoveEffects(back.gameObject);
        AddOutline(back.gameObject, DeepGreen, new Vector2(3f, 3f));
        AddShadow(back.gameObject, new Color(0f, 0.18f, 0f, 0.35f), new Vector2(0f, -5f));
        Button button = back.GetComponent<Button>();
        if (button != null) button.targetGraphic = image;
        TextMeshProUGUI label = back.GetComponentInChildren<TextMeshProUGUI>(true);
        if (label != null)
        {
            SetRect(label.transform, Vector2.zero, new Vector2(58f, 58f));
            StyleText(label, "‹", 52f, Color.white, TextAlignmentOptions.Center);
        }
    }

    private static Transform NewRect(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.layer = parent.gameObject.layer;
        go.transform.SetParent(parent, false);
        return go.transform;
    }

    private static Image NewImage(string name, Transform parent, Sprite sprite, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.layer = parent.gameObject.layer;
        go.transform.SetParent(parent, false);
        Image image = go.GetComponent<Image>();
        image.sprite = sprite;
        image.type = sprite != null ? Image.Type.Sliced : Image.Type.Simple;
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    private static Button NewButton(string name, Transform parent, Sprite sprite, Color color)
    {
        Image image = NewImage(name, parent, sprite, color);
        image.raycastTarget = true;
        Button button = image.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.navigation = new Navigation { mode = Navigation.Mode.None };
        return button;
    }

    private static TextMeshProUGUI NewText(string name, Transform parent, string value, float size, Color color, TextAlignmentOptions alignment)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        go.layer = parent.gameObject.layer;
        go.transform.SetParent(parent, false);
        TextMeshProUGUI text = go.GetComponent<TextMeshProUGUI>();
        StyleText(text, value, size, color, alignment);
        return text;
    }

    private static void StyleText(TextMeshProUGUI text, string value, float size, Color color, TextAlignmentOptions alignment)
    {
        if (text == null) return;
        text.font = font;
        text.text = value;
        text.fontSize = size;
        text.color = color;
        text.alignment = alignment;
        text.fontStyle = FontStyles.Bold;
        text.raycastTarget = false;
        text.overflowMode = TextOverflowModes.Overflow;
    }

    private static void StyleImage(Image image, Sprite sprite, Color color)
    {
        if (image == null) return;
        image.enabled = true;
        image.sprite = sprite;
        image.type = sprite != null ? Image.Type.Sliced : Image.Type.Simple;
        image.color = color;
    }

    private static Image EnsureImage(GameObject go)
    {
        Image image = go.GetComponent<Image>();
        return image != null ? image : go.AddComponent<Image>();
    }

    private static void Decor(Transform parent, string name, string path, Vector2 position, Vector2 size, float alpha)
    {
        Sprite sprite = LoadSprite(path);
        if (sprite == null) return;
        Image image = NewImage(name, parent, sprite, new Color(1f, 1f, 1f, alpha));
        image.type = Image.Type.Simple;
        image.preserveAspect = true;
        SetRect(image.transform, position, size);
    }

    private static void Bubble(Transform parent, string name, Vector2 position, float size)
    {
        Image bubble = NewImage(name, parent, whiteCard, new Color(1f, 1f, 1f, 0.20f));
        SetRect(bubble.transform, position, Vector2.one * size);
        AddOutline(bubble.gameObject, new Color(0.72f, 0.92f, 1f, 0.55f), new Vector2(2f, 2f));
    }

    private static void Spark(Transform parent, string name, Vector2 position, Color color)
    {
        TextMeshProUGUI spark = NewText(name, parent, "+", 48f, color, TextAlignmentOptions.Center);
        SetRect(spark.transform, position, new Vector2(56f, 56f));
    }

    private static Sprite LoadSprite(string path)
    {
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    private static void SetRect(Transform transform, Vector2 position, Vector2 size)
    {
        RectTransform rect = transform as RectTransform;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;
    }

    private static void Stretch(Transform transform)
    {
        RectTransform rect = transform as RectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;
    }

    private static void AddShadow(GameObject go, Color color, Vector2 distance)
    {
        Shadow shadow = go.AddComponent<Shadow>();
        shadow.effectColor = color;
        shadow.effectDistance = distance;
        shadow.useGraphicAlpha = true;
    }

    private static void AddOutline(GameObject go, Color color, Vector2 distance)
    {
        Outline outline = go.AddComponent<Outline>();
        outline.effectColor = color;
        outline.effectDistance = distance;
        outline.useGraphicAlpha = true;
    }

    private static void ReplaceEffects(GameObject go, Color color, Vector2 distance, bool outline)
    {
        RemoveEffects(go);
        if (outline) AddOutline(go, color, new Vector2(2f, 2f));
        AddShadow(go, color, distance);
    }

    private static void RemoveEffects(GameObject go)
    {
        foreach (Shadow effect in go.GetComponents<Shadow>()) Object.DestroyImmediate(effect);
    }

    private static void RemoveLayout(GameObject go)
    {
        foreach (LayoutGroup layout in go.GetComponents<LayoutGroup>()) Object.DestroyImmediate(layout);
        foreach (ContentSizeFitter fitter in go.GetComponents<ContentSizeFitter>()) Object.DestroyImmediate(fitter);
        foreach (LayoutElement element in go.GetComponents<LayoutElement>()) Object.DestroyImmediate(element);
    }

    private static void DestroyChild(Transform parent, string name)
    {
        if (parent == null) return;
        Transform child = parent.Find(name);
        if (child != null) Object.DestroyImmediate(child.gameObject);
    }

    private static void SetPrivateReference(Object target, string propertyName, Object value)
    {
        SerializedObject serializedObject = new SerializedObject(target);
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        if (property != null)
        {
            property.objectReferenceValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
#endif
