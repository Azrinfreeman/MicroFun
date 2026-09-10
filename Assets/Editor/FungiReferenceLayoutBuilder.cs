using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class FungiReferenceLayoutBuilder
{
    static readonly Color Ink = new Color(0.02f, 0.29f, 0.08f, 1f);
    static readonly Color Cream = new Color(1f, 0.98f, 0.87f, 1f);
    static readonly Color Green = new Color(0.28f, 0.67f, 0.08f, 1f);

    [MenuItem("MicroFun/Build Fungi Reference Layout")]
    public static void Build()
    {
        Transform canvas = GameObject.Find("Canvas").transform;
        Transform fungi = canvas.Find("Fungi");
        Transform top = fungi.Find("Top");
        Transform title = fungi.Find("Title");
        Transform back = fungi.Find("Back");
        Transform speciesA = fungi.Find("Button");
        Transform speciesB = fungi.Find("Button (1)");
        Transform speciesC = fungi.Find("Button (2)");
        Transform dialog = fungi.Find("DialogParent");

        Transform oldRoot = fungi.Find("FungiReference");
        if (oldRoot != null) Object.DestroyImmediate(oldRoot.gameObject);

        back.SetParent(fungi, false);
        top.gameObject.SetActive(false);
        title.gameObject.SetActive(false);
        speciesA.gameObject.SetActive(false);
        speciesB.gameObject.SetActive(false);
        speciesC.gameObject.SetActive(false);
        dialog.gameObject.SetActive(false);

        Transform root = new GameObject("FungiReference", typeof(RectTransform)).transform;
        root.SetParent(fungi, false);
        root.SetAsFirstSibling();
        Rect(root, Vector2.zero, new Vector2(1920, 1080));

        Image background = NewImage("WarmBackground", root, "Assets/Images/UI Flat/btn_white_flat.png", new Color(1f, 0.87f, 0.57f, 1f));
        Rect(background.transform, Vector2.zero, new Vector2(1920, 1080));

        Image heading = NewImage("Heading", root, "Assets/Images/UI Flat/btn_white_flat.png", new Color(0.87f, 0.94f, 0.57f, 1f));
        Rect(heading.transform, new Vector2(0, 458), new Vector2(480, 100));
        TextMeshProUGUI headingText = NewText("HeadingText", heading.transform, "FUNGI", 62, Ink, TextAlignmentOptions.Center);
        Rect(headingText.transform, Vector2.zero, new Vector2(420, 86));

        BuildInfoRow(root, "EnvironmentRow", new Vector2(0, 225), Green,
            "Assets/Images/UI Flat/icon_cute_microbe.png",
            "Boleh ditemui di udara, tanah\ndan pada hidupan lain.",
            "Assets/Images/UI Flat/img_fun_bread.png", new Vector2(470, 0), new Vector2(180, 125));

        BuildInfoRow(root, "FoodRow", new Vector2(0, 55), new Color(1f, 0.70f, 0.08f, 1f),
            "Assets/Images/UI Flat/icon_petri_dish.png",
            "Mendapat makanan daripada\nbahan organik.",
            "Assets/Images/UI Flat/icon_petri_dish.png", new Vector2(475, 0), new Vector2(165, 125));

        BuildInfoRow(root, "SporeRow", new Vector2(0, -115), new Color(0.63f, 0.35f, 0.85f, 1f),
            "Assets/Images/UI Flat/icon_petri_dish.png",
            "Membiak melalui spora.",
            "Assets/Images/UI Flat/icon_petri_dish.png", new Vector2(475, 0), new Vector2(165, 125));

        BuildInfoRow(root, "ExamplesRow", new Vector2(0, -285), new Color(0.05f, 0.65f, 0.82f, 1f),
            "Assets/Images/UI Flat/icon_petri_dish.png",
            "Contoh: <i>Rhizopus</i>, <i>Penicillium</i>\ndan yis.",
            "Assets/Images/UI Flat/icon_petri_dish.png", new Vector2(475, 0), new Vector2(165, 125));

        BuildAudioButton(root);
        BuildExistingBack(back);
        BuildHomeButton(root);
        BuildNextButton(root);

        Plus(root, new Vector2(-730, 260), Green);
        Plus(root, new Vector2(-745, 50), new Color(1f, 0.67f, 0.04f, 1f));
        Plus(root, new Vector2(-750, -160), new Color(0.63f, 0.35f, 0.85f, 1f));
        Bubble(root, new Vector2(800, 105), Green, 26);
        Bubble(root, new Vector2(815, -90), Color.white, 22);
        Bubble(root, new Vector2(-820, -210), new Color(0.05f, 0.65f, 0.82f, 1f), 20);

        EditorSceneManager.MarkSceneDirty(canvas.gameObject.scene);
        Debug.Log("Fungi reference layout built.");
    }

    static void BuildInfoRow(Transform root, string name, Vector2 position, Color accent, string iconPath, string copy, string artPath, Vector2 artPosition, Vector2 artSize)
    {
        Image row = NewImage(name, root, "Assets/Images/UI Flat/btn_white_flat.png", Cream);
        Rect(row.transform, position, new Vector2(1380, 150));

        Image iconCircle = NewImage("IconCircle", row.transform, "Assets/Images/UI Flat/btn_green_flat.png", accent);
        Rect(iconCircle.transform, new Vector2(-565, 0), new Vector2(142, 142));
        Image icon = NewImage("Icon", iconCircle.transform, iconPath, Color.white);
        Rect(icon.transform, Vector2.zero, new Vector2(105, 105));

        TextMeshProUGUI text = NewText("Copy", row.transform, copy, 35, Ink, TextAlignmentOptions.MidlineLeft);
        Rect(text.transform, new Vector2(-165, 0), new Vector2(690, 130));

        Image art = NewImage("Art", row.transform, artPath, Color.white);
        Rect(art.transform, artPosition, artSize);
    }

    static void BuildAudioButton(Transform root)
    {
        Image baseImage = NewImage("DengarAudioButton", root, "Assets/Images/UI Flat/btn_white_flat.png", Cream);
        Rect(baseImage.transform, new Vector2(-745, 465), new Vector2(365, 84));
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

    static void BuildExistingBack(Transform back)
    {
        back.gameObject.SetActive(true);
        ClearChildren(back);
        Rect(back, new Vector2(-310, -470), new Vector2(360, 86));

        Image image = EnsureImage(back.gameObject);
        image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Images/UI Flat/btn_green_flat.png");
        image.type = Image.Type.Sliced;
        image.color = new Color(0.10f, 0.72f, 0.62f, 1f);
        image.raycastTarget = true;
        back.GetComponent<Button>().targetGraphic = image;
        Feedback(back.gameObject);

        Image arrow = NewImage("Arrow", back, "Assets/Images/UI Flat/icon_arrow_left.png", Color.white);
        Rect(arrow.transform, new Vector2(-125, 0), new Vector2(50, 50));
        TextMeshProUGUI label = NewText("Label", back, "KEMBALI", 35, Color.white, TextAlignmentOptions.Center);
        Rect(label.transform, new Vector2(38, 0), new Vector2(240, 70));
    }

    static void BuildHomeButton(Transform root)
    {
        Image baseImage = NewImage("HomeButton", root, "Assets/Images/UI Flat/btn_green_flat.png", new Color(1f, 0.67f, 0.04f, 1f));
        Rect(baseImage.transform, new Vector2(-770, -470), new Vector2(96, 96));
        Button button = baseImage.gameObject.AddComponent<Button>();
        button.targetGraphic = baseImage;
        baseImage.raycastTarget = true;
        Feedback(baseImage.gameObject);

        ElaborateReturnButton route = baseImage.gameObject.AddComponent<ElaborateReturnButton>();
        SerializedObject serialized = new SerializedObject(route);
        serialized.FindProperty("destination").enumValueIndex = 1;
        serialized.ApplyModifiedPropertiesWithoutUndo();

        Image icon = NewImage("HomeIcon", baseImage.transform, "Assets/Images/UI Flat/icon_home.png", Color.white);
        Rect(icon.transform, Vector2.zero, new Vector2(52, 52));
    }

    static void BuildNextButton(Transform root)
    {
        Image baseImage = NewImage("SeterusnyaButton", root, "Assets/Images/UI Flat/btn_green_flat.png", new Color(0.12f, 0.62f, 0.06f, 1f));
        Rect(baseImage.transform, new Vector2(380, -470), new Vector2(470, 86));
        Button button = baseImage.gameObject.AddComponent<Button>();
        button.targetGraphic = baseImage;
        baseImage.raycastTarget = true;
        Feedback(baseImage.gameObject);

        NextController route = baseImage.gameObject.AddComponent<NextController>();
        SerializedObject serialized = new SerializedObject(route);
        serialized.FindProperty("targetSceneName").stringValue = "kuiz1";
        serialized.ApplyModifiedPropertiesWithoutUndo();

        TextMeshProUGUI label = NewText("Label", baseImage.transform, "SETERUSNYA", 35, Color.white, TextAlignmentOptions.Center);
        Rect(label.transform, new Vector2(-20, 0), new Vector2(340, 70));
        Image arrow = NewImage("Arrow", baseImage.transform, "Assets/Images/UI Flat/ic_arrow_thick.png", new Color(1f, 0.86f, 0.10f, 1f));
        Rect(arrow.transform, new Vector2(178, 0), new Vector2(52, 52));
    }

    static void Plus(Transform root, Vector2 position, Color color)
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