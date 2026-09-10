using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class JenisReferenceLayoutBuilder
{
    static readonly Color Ink = new Color(0.02f, 0.29f, 0.08f, 1f);
    static readonly Color Cream = new Color(1f, 0.98f, 0.87f, 1f);
    static readonly Color Green = new Color(0.30f, 0.72f, 0.08f, 1f);
    static readonly Color Orange = new Color(1f, 0.67f, 0.04f, 1f);

    [MenuItem("MicroFun/Build Jenis Reference Layout")]
    public static void Build()
    {
        Transform canvas = GameObject.Find("Canvas").transform;
        Transform jenis = canvas.Find("Jenis");
        Transform center = jenis.Find("Center");
        Transform bottom = jenis.Find("Bottom");

        Transform fungi = center.Find("Button");
        if (fungi == null) fungi = jenis.Find("FungiCard");
        Transform bakteria = center.Find("Button (1)");
        if (bakteria == null) bakteria = jenis.Find("BakteriaCard");
        Transform home = bottom.Find("HomeButton");
        if (home == null) home = jenis.Find("HomeButton");
        Transform next = bottom.Find("NextButton");
        if (next == null) next = jenis.Find("NextButton");

        fungi.SetParent(jenis, false);
        bakteria.SetParent(jenis, false);
        home.SetParent(jenis, false);
        next.SetParent(jenis, false);

        Transform oldRoot = jenis.Find("JenisReference");
        if (oldRoot != null) Object.DestroyImmediate(oldRoot.gameObject);

        center.gameObject.SetActive(false);
        bottom.gameObject.SetActive(false);

        Transform root = new GameObject("JenisReference", typeof(RectTransform)).transform;
        root.SetParent(jenis, false);
        root.SetAsFirstSibling();
        Rect(root, Vector2.zero, new Vector2(1920, 1080));

        Image background = NewImage("WarmBackground", root, "Assets/Images/UI Flat/btn_white_flat.png", new Color(1f, 0.87f, 0.57f, 1f));
        Rect(background.transform, Vector2.zero, new Vector2(1920, 1080));

        Image heading = NewImage("Heading", root, "Assets/Images/UI Flat/btn_white_flat.png", Cream);
        Rect(heading.transform, new Vector2(0, 445), new Vector2(480, 105));
        TextMeshProUGUI headingText = NewText("HeadingText", heading.transform, "JENIS", 64, Ink, TextAlignmentOptions.Center);
        Rect(headingText.transform, Vector2.zero, new Vector2(420, 90));

        TextMeshProUGUI instruction = NewText("Instruction", root,
            "Pilih kumpulan mikroorganisma yang ingin diterokai.", 35, Ink, TextAlignmentOptions.Center);
        Rect(instruction.transform, new Vector2(0, 335), new Vector2(1050, 72));

        BuildChoiceCard(fungi, "FungiCard", new Vector2(-345, -35), new Color(0.61f, 0.86f, 0.20f, 1f),
            "Assets/Images/UI Flat/img_petri_fungi.png", "FUNGI");
        BuildChoiceCard(bakteria, "BakteriaCard", new Vector2(345, -35), new Color(1f, 0.74f, 0.08f, 1f),
            "Assets/Images/UI Flat/img_petri_bakteria.png", "BAKTERIA");

        BuildAudioButton(root);
        BuildBackButton(root);
        BuildExistingNavigation(home, "HomeButton", new Vector2(-780, -470),
            "Assets/Images/UI Flat/icon_home.png", Orange);
        BuildExistingNavigation(next, "NextButton", new Vector2(780, -470),
            "Assets/Images/UI Flat/ic_arrow_thick.png", new Color(0.12f, 0.62f, 0.06f, 1f));

        AddPlus(root, new Vector2(-730, 215), Green);
        AddPlus(root, new Vector2(725, 190), Orange);
        Bubble(root, new Vector2(-820, -10), Color.white, 28);
        Bubble(root, new Vector2(815, 5), Green, 25);
        Bubble(root, new Vector2(-720, -250), new Color(0.05f, 0.65f, 0.75f, 1f), 20);

        EditorSceneManager.MarkSceneDirty(canvas.gameObject.scene);
        Debug.Log("Jenis reference layout built.");
    }

    static void BuildChoiceCard(Transform card, string name, Vector2 position, Color color, string imagePath, string labelText)
    {
        card.gameObject.SetActive(true);
        card.name = name;
        ClearChildren(card);
        Rect(card, position, new Vector2(600, 620));

        Image cardImage = EnsureImage(card.gameObject);
        cardImage.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Images/UI Flat/btn_white_flat.png");
        cardImage.type = Image.Type.Sliced;
        cardImage.color = color;
        cardImage.raycastTarget = true;

        Button button = card.GetComponent<Button>();
        button.targetGraphic = cardImage;
        Feedback(card.gameObject);

        Image petri = NewImage("PetriDish", card, imagePath, Color.white);
        Rect(petri.transform, new Vector2(0, 70), new Vector2(500, 420));

        Image labelPanel = NewImage("LabelPanel", card, "Assets/Images/UI Flat/btn_white_flat.png", Cream);
        Rect(labelPanel.transform, new Vector2(0, -235), new Vector2(520, 120));
        TextMeshProUGUI label = NewText("Label", labelPanel.transform, labelText, 58, Ink, TextAlignmentOptions.Center);
        Rect(label.transform, Vector2.zero, new Vector2(480, 100));
    }

    static void BuildAudioButton(Transform root)
    {
        Image baseImage = NewImage("DengarAudioButton", root, "Assets/Images/UI Flat/btn_white_flat.png", Cream);
        Rect(baseImage.transform, new Vector2(-745, 460), new Vector2(365, 84));
        Button button = baseImage.gameObject.AddComponent<Button>();
        button.targetGraphic = baseImage;
        baseImage.raycastTarget = true;
        Feedback(baseImage.gameObject);

        Image circle = NewImage("SpeakerCircle", baseImage.transform, "Assets/Images/UI Flat/btn_green_flat.png", new Color(0.12f, 0.62f, 0.06f, 1f));
        Rect(circle.transform, new Vector2(-130, 0), new Vector2(88, 88));
        Image speaker = NewImage("SpeakerIcon", circle.transform, "Assets/Images/UI Flat/icon_speaker.png", Color.white);
        Rect(speaker.transform, Vector2.zero, new Vector2(48, 48));
        TextMeshProUGUI label = NewText("Label", baseImage.transform, "Dengar audio", 29, Ink, TextAlignmentOptions.MidlineLeft);
        Rect(label.transform, new Vector2(52, 0), new Vector2(240, 76));

        baseImage.gameObject.AddComponent<AudioSource>();
        AudioNarrationButton narration = baseImage.gameObject.AddComponent<AudioNarrationButton>();
        SerializedObject serialized = new SerializedObject(narration);
        serialized.FindProperty("narrationClip").objectReferenceValue =
            AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/akt1_intro_voice.mp3");
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    static void BuildBackButton(Transform root)
    {
        Image baseImage = NewImage("BackButton", root, "Assets/Images/UI Flat/btn_green_flat.png", new Color(0.10f, 0.72f, 0.62f, 1f));
        Rect(baseImage.transform, new Vector2(-625, -470), new Vector2(96, 96));
        Button button = baseImage.gameObject.AddComponent<Button>();
        button.targetGraphic = baseImage;
        baseImage.raycastTarget = true;
        baseImage.gameObject.AddComponent<JenisBackButton>();
        Feedback(baseImage.gameObject);

        Image arrow = NewImage("ArrowIcon", baseImage.transform, "Assets/Images/UI Flat/icon_arrow_left.png", Color.white);
        Rect(arrow.transform, Vector2.zero, new Vector2(54, 54));
    }

    static void BuildExistingNavigation(Transform control, string name, Vector2 position, string iconPath, Color color)
    {
        control.gameObject.SetActive(true);
        control.name = name;
        ClearChildren(control);
        Rect(control, position, new Vector2(96, 96));

        Image image = EnsureImage(control.gameObject);
        image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Images/UI Flat/btn_green_flat.png");
        image.type = Image.Type.Sliced;
        image.color = color;
        image.raycastTarget = true;

        Button button = control.GetComponent<Button>();
        button.targetGraphic = image;
        Feedback(control.gameObject);

        Image icon = NewImage("Icon", control, iconPath, Color.white);
        Rect(icon.transform, Vector2.zero, new Vector2(54, 54));
    }

    static void AddPlus(Transform root, Vector2 position, Color color)
    {
        TextMeshProUGUI plus = NewText("Plus", root, "+", 48, color, TextAlignmentOptions.Center);
        Rect(plus.transform, position, new Vector2(60, 60));
    }

    static void Bubble(Transform root, Vector2 position, Color color, float size)
    {
        Image bubble = NewImage("Bubble", root, "Assets/Images/UI Flat/btn_green_flat.png", color);
        Rect(bubble.transform, position, new Vector2(size, size));
    }

    static void ClearChildren(Transform parent)
    {
        for (int index = parent.childCount - 1; index >= 0; index--)
            Object.DestroyImmediate(parent.GetChild(index).gameObject);
    }

    static Image NewImage(string name, Transform parent, string path, Color color)
    {
        GameObject gameObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        gameObject.transform.SetParent(parent, false);
        Image image = gameObject.GetComponent<Image>();
        image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        image.type = Image.Type.Sliced;
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    static TextMeshProUGUI NewText(string name, Transform parent, string content, float size, Color color, TextAlignmentOptions alignment)
    {
        GameObject gameObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        gameObject.transform.SetParent(parent, false);
        TextMeshProUGUI text = gameObject.GetComponent<TextMeshProUGUI>();
        text.font = TMP_Settings.defaultFontAsset;
        text.text = content;
        text.fontSize = size;
        text.color = color;
        text.alignment = alignment;
        text.enableWordWrapping = true;
        text.raycastTarget = false;
        return text;
    }

    static Image EnsureImage(GameObject gameObject)
    {
        Image image = gameObject.GetComponent<Image>();
        if (image == null) image = gameObject.AddComponent<Image>();
        return image;
    }

    static void Rect(Transform transform, Vector2 position, Vector2 size)
    {
        RectTransform rect = transform.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        rect.localScale = Vector3.one;
    }

    static void Feedback(GameObject gameObject)
    {
        if (gameObject.GetComponent<ButtonInteractionFeedback>() == null)
            gameObject.AddComponent<ButtonInteractionFeedback>();
    }
}