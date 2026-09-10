using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class DefinisiReferenceLayoutBuilder
{
    static readonly Color Ink = new Color(0.02f, 0.29f, 0.08f, 1f);
    static readonly Color Cream = new Color(1f, 0.98f, 0.87f, 1f);
    static readonly Color Green = new Color(0.16f, 0.58f, 0.05f, 1f);

    [MenuItem("MicroFun/Build Definisi Reference Layout")]
    public static void Build()
    {
        Transform canvas = GameObject.Find("Canvas").transform;
        Transform definisi = canvas.Find("Definisi");
        Transform title = definisi.Find("Title");
        Transform top = definisi.Find("Top");
        Transform content = definisi.Find("Content");
        Transform bottom = definisi.Find("Bottom");
        Transform dialog = definisi.Find("DialogParent");
        Transform old = definisi.Find("DefinisiReference");

        if (old != null) Object.DestroyImmediate(old.gameObject);

        Transform home = bottom.Find("HomeButton");
        if (home == null) home = definisi.Find("HomeButton");
        Transform next = bottom.Find("NextButton");
        if (next == null) next = definisi.Find("NextButton");
        home.SetParent(definisi, false);
        next.SetParent(definisi, false);
        title.gameObject.SetActive(false);
        top.gameObject.SetActive(false);
        content.gameObject.SetActive(false);
        bottom.gameObject.SetActive(false);
        dialog.gameObject.SetActive(false);

        Transform root = new GameObject("DefinisiReference", typeof(RectTransform)).transform;
        root.SetParent(definisi, false);
        root.SetAsFirstSibling();
        Rect(root, Vector2.zero, new Vector2(1920, 1080));

        Image background = NewImage("WarmBackground", root, "Assets/Images/UI Flat/btn_white_flat.png", new Color(1f, 0.87f, 0.57f, 1f));
        Rect(background.transform, Vector2.zero, new Vector2(1920, 1080));
        background.raycastTarget = false;

        Image heading = NewImage("Heading", root, "Assets/Images/UI Flat/btn_white_flat.png", Cream);
        Rect(heading.transform, new Vector2(0, 455), new Vector2(490, 100));
        TextMeshProUGUI headingText = NewText("HeadingText", heading.transform, "DEFINISI", 62, Ink, TextAlignmentOptions.Center);
        Rect(headingText.transform, Vector2.zero, new Vector2(440, 85));

        BuildCard(root, "DefinitionCard", new Vector2(-455, 165), new Color(0.82f, 0.94f, 0.67f, 1f),
            "Assets/Images/UI Flat/icon_cute_microbe.png",
            "Mikroorganisma ialah\nhidupan seni yang\nsangat kecil.");
        BuildCard(root, "VisibilityCard", new Vector2(-455, -105), new Color(1f, 0.92f, 0.65f, 1f),
            "Assets/Images/UI Flat/icon_eye_magnify.png",
            "Kebanyakannya\ntidak dapat dilihat\ndengan mata kasar.");
        BuildCard(root, "MicroscopeCard", new Vector2(-455, -375), new Color(0.77f, 0.92f, 0.97f, 1f),
            "Assets/Images/UI Flat/icon_microscope_card.png",
            "Mikroorganisma boleh\ndiperhatikan\nmenggunakan\nmikroskop.");

        Deco("PetriDish", root, "Assets/Images/UI Flat/icon_petri_dish.png", new Vector2(615, 215), new Vector2(420, 330));
        Deco("Microscope", root, "Assets/Images/UI Flat/img_microscope_large.png", new Vector2(260, -260), new Vector2(520, 530));
        Bubble(root, new Vector2(790, -40), Green, 30);
        Bubble(root, new Vector2(855, -140), new Color(0.2f, 0.7f, 0.35f, 1f), 22);
        Bubble(root, new Vector2(-785, -30), Cream, 22);

        BuildAudioButton(root);
        BuildBackButton(root);
        BuildExistingNavigation(home, "Home", new Vector2(-760, -475), "Assets/Images/UI Flat/icon_home.png", new Color(1f, 0.67f, 0.04f, 1f));
        BuildExistingNavigation(next, "Next", new Vector2(760, -475), "Assets/Images/UI Flat/ic_arrow_thick.png", Green);

        EditorSceneManager.MarkSceneDirty(canvas.gameObject.scene);
        Debug.Log("Definisi reference layout built.");
    }

    static void BuildCard(Transform root, string name, Vector2 position, Color color, string iconPath, string copy)
    {
        Image card = NewImage(name, root, "Assets/Images/UI Flat/btn_white_flat.png", color);
        Rect(card.transform, position, new Vector2(800, 235));

        Image iconFrame = NewImage("IconFrame", card.transform, "Assets/Images/UI Flat/btn_white_flat.png", Cream);
        Rect(iconFrame.transform, new Vector2(-282, 0), new Vector2(190, 190));
        Image icon = NewImage("Illustration", iconFrame.transform, iconPath, Color.white);
        Rect(icon.transform, Vector2.zero, new Vector2(155, 155));

        TextMeshProUGUI text = NewText("Copy", card.transform, copy, 37, Ink, TextAlignmentOptions.MidlineLeft);
        Rect(text.transform, new Vector2(104, 0), new Vector2(500, 190));
    }

    static void BuildAudioButton(Transform root)
    {
        Image baseImage = NewImage("DengarAudioButton", root, "Assets/Images/UI Flat/btn_white_flat.png", Cream);
        Rect(baseImage.transform, new Vector2(-735, 470), new Vector2(365, 84));
        Button button = baseImage.gameObject.AddComponent<Button>();
        button.targetGraphic = baseImage;
        baseImage.raycastTarget = true;
        Feedback(baseImage.gameObject);

        Image circle = NewImage("SpeakerCircle", baseImage.transform, "Assets/Images/UI Flat/btn_green_flat.png", Green);
        Rect(circle.transform, new Vector2(-130, 0), new Vector2(88, 88));
        Image speaker = NewImage("SpeakerIcon", circle.transform, "Assets/Images/UI Flat/icon_speaker.png", Color.white);
        Rect(speaker.transform, Vector2.zero, new Vector2(48, 48));
        TextMeshProUGUI label = NewText("Label", baseImage.transform, "Dengar audio", 29, Ink, TextAlignmentOptions.MidlineLeft);
        Rect(label.transform, new Vector2(52, 0), new Vector2(240, 76));

        AudioSource source = baseImage.gameObject.AddComponent<AudioSource>();
        AudioNarrationButton narration = baseImage.gameObject.AddComponent<AudioNarrationButton>();
        SerializedObject serialized = new SerializedObject(narration);
        serialized.FindProperty("narrationClip").objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/akt1_intro_voice.mp3");
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    static void BuildBackButton(Transform root)
    {
        Image baseImage = NewImage("BackButton", root, "Assets/Images/UI Flat/btn_green_flat.png", new Color(0.1f, 0.72f, 0.62f, 1f));
        Rect(baseImage.transform, new Vector2(-610, -475), new Vector2(94, 94));
        Button button = baseImage.gameObject.AddComponent<Button>();
        button.targetGraphic = baseImage;
        baseImage.raycastTarget = true;
        baseImage.gameObject.AddComponent<DefinisiBackButton>();
        Feedback(baseImage.gameObject);

        Image arrow = NewImage("ArrowIcon", baseImage.transform, "Assets/Images/UI Flat/icon_arrow_left.png", Color.white);
        Rect(arrow.transform, Vector2.zero, new Vector2(52, 52));
    }

    static void BuildExistingNavigation(Transform buttonTransform, string name, Vector2 position, string iconPath, Color color)
    {
        buttonTransform.gameObject.SetActive(true);
        buttonTransform.name = name + "Button";
        ClearChildren(buttonTransform);
        Rect(buttonTransform, position, new Vector2(94, 94));

        Image image = EnsureImage(buttonTransform.gameObject);
        image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Images/UI Flat/btn_green_flat.png");
        image.type = Image.Type.Sliced;
        image.color = color;
        image.raycastTarget = true;

        Button button = buttonTransform.GetComponent<Button>();
        button.targetGraphic = image;
        Feedback(buttonTransform.gameObject);

        Image icon = NewImage("Icon", buttonTransform, iconPath, Color.white);
        Rect(icon.transform, Vector2.zero, new Vector2(52, 52));
    }

    static void ClearChildren(Transform parent)
    {
        for (int index = parent.childCount - 1; index >= 0; index--)
            Object.DestroyImmediate(parent.GetChild(index).gameObject);
    }

    static void Deco(string name, Transform root, string path, Vector2 position, Vector2 size)
    {
        Image image = NewImage(name, root, path, Color.white);
        Rect(image.transform, position, size);
        image.raycastTarget = false;
    }

    static void Bubble(Transform root, Vector2 position, Color color, float size)
    {
        Image image = NewImage("Bubble", root, "Assets/Images/UI Flat/btn_green_flat.png", color);
        Rect(image.transform, position, new Vector2(size, size));
        image.raycastTarget = false;
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