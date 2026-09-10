using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

[ExecuteAlways]
public class QuizReferenceLayoutBuilder : MonoBehaviour
{
    private const string WhiteButtonPath = "Assets/Images/UI Flat/btn_white_flat.png";
    private const string GreenButtonPath = "Assets/Images/UI Flat/btn_green_flat.png";
    private const string SpeakerPath = "Assets/Images/UI Flat/icon_speaker.png";
    private const string HomePath = "Assets/Images/UI Flat/icon_home.png";
    private const string ArrowPath = "Assets/Images/UI Flat/ic_arrow_thick.png";
    private const string GreenMicrobePath = "Assets/Images/UI Flat/char_green_microbe.png";
    private const string PurpleMicrobePath = "Assets/Images/UI Flat/char_purple_microbe.png";
    private const string WaterDropPath = "Assets/Images/UI Flat/char_water_drop.png";
    private const string FlaskPath = "Assets/Images/UI Flat/icon_flask.png";
    private const string PetriPath = "Assets/Images/UI Flat/icon_petri_dish.png";
    private const string BreadPath = "Assets/Images/UI Flat/img_fun_bread.png";
    private const string MagnifyPath = "Assets/Images/UI Flat/icon_magnify_glass.png";

    private static readonly Color Ink = new Color(0.02f, 0.29f, 0.08f, 1f);
    private static readonly Color Cream = new Color(1f, 0.98f, 0.87f, 1f);
    private static readonly Color Panel = new Color(1f, 0.97f, 0.72f, 0.96f);
    private static readonly Color Green = new Color(0.16f, 0.58f, 0.05f, 1f);
    private static readonly Color Blue = new Color(0.04f, 0.58f, 0.88f, 1f);
    private static readonly Color Orange = new Color(1f, 0.55f, 0.02f, 1f);

    [ContextMenu("Build reference quiz layout")]
    public void BuildReferenceLayout()
    {
#if UNITY_EDITOR
        Transform canvas = transform;
        QuizController quiz = canvas.GetComponent<QuizController>();
        if (quiz == null) quiz = canvas.GetComponentInChildren<QuizController>(true);

        Transform content = canvas.Find("Content");
        Transform panel = content == null ? null : content.Find("bg");
        Transform question = content == null ? null : content.Find("Soalan");
        Transform options = content == null ? null : content.Find("OptionsButtons");
        Transform title = canvas.Find("Title");
        Transform score = canvas.Find("Score");
        Transform back = canvas.Find("Back");

        if (quiz == null || content == null || panel == null || question == null || options == null || title == null || score == null || back == null)
        {
            Debug.LogError("QuizReferenceLayoutBuilder: expected kuiz1 UI objects were not found.");
            return;
        }

        ClearBuilderRoot(canvas);
        RemoveLayoutComponents(content.gameObject);
        RemoveLayoutComponents(panel.gameObject);
        RemoveLayoutComponents(options.gameObject);
        RemoveLayoutComponents(options.Find("1") == null ? null : options.Find("1").gameObject);
        RemoveLayoutComponents(options.Find("2") == null ? null : options.Find("2").gameObject);

        RectTransform contentRect = content.GetComponent<RectTransform>();
        SetRect(contentRect, new Vector2(0, -22), new Vector2(1500, 680));
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        SetRect(panelRect, Vector2.zero, new Vector2(1500, 680));
        Image panelImage = EnsureImage(panel.gameObject);
        StyleImage(panelImage, WhiteButtonPath, Panel);

        SetRect(title.GetComponent<RectTransform>(), new Vector2(0, 468), new Vector2(410, 92));
        StyleImage(EnsureImage(title.gameObject), WhiteButtonPath, Cream);
        StyleText(title.GetComponentInChildren<TextMeshProUGUI>(true), "KUIZ", 58, Ink, TextAlignmentOptions.Center);

        SetRect(score.GetComponent<RectTransform>(), new Vector2(0, 256), new Vector2(450, 74));
        StyleImage(EnsureImage(score.gameObject), GreenButtonPath, Green);
        StyleText(score.GetComponentInChildren<TextMeshProUGUI>(true), "Soalan 1 daripada 3", 34, Color.white, TextAlignmentOptions.Center);

        RectTransform questionRect = question.GetComponent<RectTransform>();
        SetRect(questionRect, new Vector2(0, 165), new Vector2(1260, 118));
        Image questionImage = question.GetComponent<Image>();
        if (questionImage != null) questionImage.enabled = false;
        StyleText(quiz.questionText == null ? question.GetComponentInChildren<TextMeshProUGUI>(true) : quiz.questionText.GetComponent<TextMeshProUGUI>(),
            "Apakah definisi mikroorganisma?", 48, Ink, TextAlignmentOptions.Center);

        SetRect(options.GetComponent<RectTransform>(), new Vector2(0, -75), new Vector2(1110, 430));
        for (int i = 0; i < quiz.buttonAnswers.Count; i++)
        {
            Transform answer = quiz.buttonAnswers[i];
            if (answer == null) continue;

            if (i >= 3)
            {
                answer.gameObject.SetActive(false);
                continue;
            }

            answer.SetParent(options, false);
            answer.gameObject.SetActive(true);
            SetRect(answer.GetComponent<RectTransform>(), new Vector2(0, 95 - (i * 145)), new Vector2(1080, 122));

            Image answerImage = EnsureImage(answer.gameObject);
            StyleImage(answerImage, WhiteButtonPath, Cream);
            Button answerButton = answer.GetComponent<Button>();
            if (answerButton == null) answerButton = answer.gameObject.AddComponent<Button>();
            answerButton.targetGraphic = answerImage;

            TextMeshProUGUI answerText = answer.GetComponentInChildren<TextMeshProUGUI>(true);
            if (answerText != null)
            {
                answerText.gameObject.name = "AnswerText";
                SetRect(answerText.rectTransform, new Vector2(52, 0), new Vector2(820, 105));
                StyleText(answerText, answerText.text, 32, Ink, TextAlignmentOptions.MidlineLeft);
            }

            CreateLetterBadge(answer, i);
            AddFeedback(answer.gameObject);
        }

        BuildBackButton(back);
        BuildDecorations(canvas);
        Button nextButton = BuildNextButton(canvas);
        BuildAudioButton(canvas);

        SerializedObject quizData = new SerializedObject(quiz);
        SerializedProperty nextProperty = quizData.FindProperty("nextButton");
        if (nextProperty != null) nextProperty.objectReferenceValue = nextButton;
        quizData.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.MarkSceneDirty(gameObject.scene);
        Debug.Log("QuizReferenceLayoutBuilder: reference layout built.");
#endif
    }

#if UNITY_EDITOR
    private void ClearBuilderRoot(Transform canvas)
    {
        Transform oldRoot = canvas.Find("QuizDecor");
        if (oldRoot != null) DestroyImmediate(oldRoot.gameObject);
    }

    private void RemoveLayoutComponents(GameObject target)
    {
        if (target == null) return;
        LayoutGroup group = target.GetComponent<LayoutGroup>();
        if (group != null) DestroyImmediate(group);
        ContentSizeFitter fitter = target.GetComponent<ContentSizeFitter>();
        if (fitter != null) DestroyImmediate(fitter);
    }

    private void BuildBackButton(Transform back)
    {
        SetRect(back.GetComponent<RectTransform>(), new Vector2(-720, -472), new Vector2(350, 82));
        Image baseImage = EnsureImage(back.gameObject);
        StyleImage(baseImage, WhiteButtonPath, Cream);
        Button button = back.GetComponent<Button>();
        if (button != null) button.targetGraphic = baseImage;

        TextMeshProUGUI label = back.GetComponentInChildren<TextMeshProUGUI>(true);
        if (label != null)
        {
            SetRect(label.rectTransform, new Vector2(54, 0), new Vector2(230, 78));
            StyleText(label, "Laman Utama", 29, Ink, TextAlignmentOptions.MidlineLeft);
        }

        Transform icon = back.Find("HomeCircle");
        if (icon == null) icon = CreateImage("HomeCircle", back, GreenButtonPath, Green).transform;
        SetRect(icon.GetComponent<RectTransform>(), new Vector2(-126, 0), new Vector2(86, 86));
        Transform home = icon.Find("HomeIcon");
        if (home == null) home = CreateImage("HomeIcon", icon, HomePath, Color.white).transform;
        SetRect(home.GetComponent<RectTransform>(), Vector2.zero, new Vector2(44, 44));
        AddFeedback(back.gameObject);
    }

    private Button BuildNextButton(Transform canvas)
    {
        Transform root = canvas.Find("QuizDecor");
        GameObject next = CreateImage("SeterusnyaButton", root, WhiteButtonPath, Cream).gameObject;
        SetRect(next.GetComponent<RectTransform>(), new Vector2(720, -472), new Vector2(350, 82));
        Button nextButton = next.AddComponent<Button>();
        nextButton.targetGraphic = next.GetComponent<Image>();
        AddFeedback(next);

        TextMeshProUGUI label = CreateText("Label", next.transform, "Seterusnya", 29, Ink, TextAlignmentOptions.MidlineRight);
        SetRect(label.rectTransform, new Vector2(-45, 0), new Vector2(225, 78));

        Transform circle = CreateImage("ArrowCircle", next.transform, GreenButtonPath, Green).transform;
        SetRect(circle.GetComponent<RectTransform>(), new Vector2(126, 0), new Vector2(86, 86));
        Transform arrow = CreateImage("ArrowIcon", circle, ArrowPath, Color.white).transform;
        SetRect(arrow.GetComponent<RectTransform>(), Vector2.zero, new Vector2(46, 46));
        return nextButton;
    }

    private void BuildAudioButton(Transform canvas)
    {
        Transform root = canvas.Find("QuizDecor");
        GameObject audio = CreateImage("DengarAudioButton", root, WhiteButtonPath, Cream).gameObject;
        SetRect(audio.GetComponent<RectTransform>(), new Vector2(-720, 468), new Vector2(355, 82));
        Button audioButton = audio.AddComponent<Button>();
        audioButton.targetGraphic = audio.GetComponent<Image>();
        AddFeedback(audio);

        Transform circle = CreateImage("SpeakerCircle", audio.transform, GreenButtonPath, Green).transform;
        SetRect(circle.GetComponent<RectTransform>(), new Vector2(-126, 0), new Vector2(86, 86));
        Transform speaker = CreateImage("SpeakerIcon", circle, SpeakerPath, Color.white).transform;
        SetRect(speaker.GetComponent<RectTransform>(), Vector2.zero, new Vector2(47, 47));

        TextMeshProUGUI label = CreateText("Label", audio.transform, "Dengar audio", 29, Ink, TextAlignmentOptions.MidlineLeft);
        SetRect(label.rectTransform, new Vector2(52, 0), new Vector2(235, 78));

        AudioSource audioSource = audio.GetComponent<AudioSource>();
        if (audioSource == null) audioSource = audio.AddComponent<AudioSource>();
        AudioNarrationButton narration = audio.GetComponent<AudioNarrationButton>();
        if (narration == null) narration = audio.AddComponent<AudioNarrationButton>();

        SerializedObject narrationData = new SerializedObject(narration);
        SerializedProperty clipProperty = narrationData.FindProperty("narrationClip");
        if (clipProperty != null)
            clipProperty.objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/akt1_intro_voice.mp3");
        narrationData.ApplyModifiedPropertiesWithoutUndo();
    }

    private void BuildDecorations(Transform canvas)
    {
        Transform root = new GameObject("QuizDecor", typeof(RectTransform)).transform;
        root.SetParent(canvas, false);
        root.SetAsFirstSibling();
        SetRect(root.GetComponent<RectTransform>(), Vector2.zero, new Vector2(1920, 1080));

        Image backdrop = CreateImage("WarmBackground", root, WhiteButtonPath, new Color(1f, 0.88f, 0.55f, 1f));
        SetRect(backdrop.rectTransform, Vector2.zero, new Vector2(1920, 1080));

        AddDecoration("GreenMicrobe", root, GreenMicrobePath, new Vector2(-650, 300), new Vector2(210, 210), Color.white);
        AddDecoration("PurpleMicrobe", root, PurpleMicrobePath, new Vector2(600, 300), new Vector2(200, 200), Color.white);
        AddDecoration("WaterDrop", root, WaterDropPath, new Vector2(790, 232), new Vector2(135, 185), Color.white);
        AddDecoration("Flask", root, FlaskPath, new Vector2(-825, -330), new Vector2(160, 245), Color.white);
        AddDecoration("PetriDish", root, PetriPath, new Vector2(280, -410), new Vector2(235, 165), Color.white);
        AddDecoration("MoldyBread", root, BreadPath, new Vector2(770, -330), new Vector2(210, 220), Color.white);
        AddDecoration("Magnifier", root, MagnifyPath, new Vector2(-630, -265), new Vector2(135, 135), Color.white);

        AddPlus(root, new Vector2(-410, 320), Green);
        AddPlus(root, new Vector2(-500, 390), Orange);
        AddPlus(root, new Vector2(430, 330), Orange);
        AddPlus(root, new Vector2(-770, 0), Blue);
        AddPlus(root, new Vector2(820, -15), Green);
    }

    private void AddPlus(Transform root, Vector2 position, Color color)
    {
        TextMeshProUGUI plus = CreateText("Plus", root, "+", 44, color, TextAlignmentOptions.Center);
        SetRect(plus.rectTransform, position, new Vector2(55, 55));
    }

    private void AddDecoration(string name, Transform parent, string spritePath, Vector2 position, Vector2 size, Color color)
    {
        Image image = CreateImage(name, parent, spritePath, color);
        SetRect(image.rectTransform, position, size);
        image.raycastTarget = false;
    }

    private void CreateLetterBadge(Transform answer, int index)
    {
        Transform oldBadge = answer.Find("LetterBadge");
        if (oldBadge != null) DestroyImmediate(oldBadge.gameObject);

        Color color = index == 0 ? Green : (index == 1 ? Blue : Orange);
        Transform badge = CreateImage("LetterBadge", answer, GreenButtonPath, color).transform;
        SetRect(badge.GetComponent<RectTransform>(), new Vector2(-458, 0), new Vector2(86, 86));

        TextMeshProUGUI letter = CreateText("LetterLabel", badge, ((char)('A' + index)).ToString(), 48, Color.white, TextAlignmentOptions.Center);
        SetRect(letter.rectTransform, Vector2.zero, new Vector2(80, 80));
    }

    private Image CreateImage(string name, Transform parent, string spritePath, Color color)
    {
        GameObject gameObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        gameObject.transform.SetParent(parent, false);
        Image image = gameObject.GetComponent<Image>();
        StyleImage(image, spritePath, color);
        return image;
    }

    private TextMeshProUGUI CreateText(string name, Transform parent, string text, float size, Color color, TextAlignmentOptions alignment)
    {
        GameObject gameObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        gameObject.transform.SetParent(parent, false);
        TextMeshProUGUI label = gameObject.GetComponent<TextMeshProUGUI>();
        label.font = TMP_Settings.defaultFontAsset;
        StyleText(label, text, size, color, alignment);
        return label;
    }

    private void StyleImage(Image image, string spritePath, Color color)
    {
        image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
        image.type = Image.Type.Sliced;
        image.color = color;
        image.raycastTarget = false;
    }

    private Image EnsureImage(GameObject gameObject)
    {
        Image image = gameObject.GetComponent<Image>();
        if (image == null) image = gameObject.AddComponent<Image>();
        return image;
    }

    private void StyleText(TextMeshProUGUI label, string text, float fontSize, Color color, TextAlignmentOptions alignment)
    {
        if (label == null) return;
        label.text = text;
        label.fontSize = fontSize;
        label.color = color;
        label.alignment = alignment;
        label.enableWordWrapping = true;
        label.raycastTarget = false;
    }

    private void SetRect(RectTransform rect, Vector2 position, Vector2 size)
    {
        if (rect == null) return;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        rect.localScale = Vector3.one;
    }

    private void AddFeedback(GameObject gameObject)
    {
        if (gameObject.GetComponent<ButtonInteractionFeedback>() == null)
            gameObject.AddComponent<ButtonInteractionFeedback>();
    }
#endif
}