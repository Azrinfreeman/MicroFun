#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Rebuilds only Canvas/Center in the Aktiviti 7 scene. The original buttons
/// remain serialized and are invoked through CenterMenuButtonRelay, preserving
/// every existing scene/panel callback.
/// </summary>
public static class Akt7CenterMenuUIBuilder
{
    private const string ScenePath = "Assets/Scenes/Aktivity/akt7/akt7.unity";
    private const string RootName = "Akt7CenterRedesignRoot";
    private const string BackgroundPath = "Assets/Images/Akt8CenterMenuUI/Background/akt8_center_background.png";
    private const string WhitePanelPath = "Assets/Images/UI Flat/btn_white_flat.png";
    private const string FontPath = "Assets/Fonts/FredokaOne SDF.asset";
    private const string BookPath = "Assets/Images/UI Flat/icon_book_open.png";
    private const string BadMicrobePath = "Assets/Images/UI Flat/char_purple_microbe.png";
    private const string VideoPath = "Assets/Images/UI Flat/ic_video_flat.png";
    private const string QuizPath = "Assets/Images/UI Flat/icon_quiz_checklist.png";
    private const string GreenMicrobePath = "Assets/Images/UI Flat/char_green_microbe.png";
    private const string WaterDropPath = "Assets/Images/UI Flat/char_water_drop.png";
    private const string MicroscopePath = "Assets/Images/UI Flat/img_microscope_large.png";
    private const string PetriPath = "Assets/Images/UI Flat/img_petri_bakteria.png";
    private const string MagnifierPath = "Assets/Images/UI Flat/icon_magnify_glass.png";
    private const string PlayPath = "Assets/Images/UI Flat/icon_play.png";
    private const string HomePath = "Assets/Images/UI Flat/icon_home.png";

    private static TMP_FontAsset font;
    private static Sprite whitePanel;

    [MenuItem("Tools/MicroFun/Rebuild Akt7 Center Menu")]
    public static void Rebuild()
    {
        if (EditorSceneManager.GetActiveScene().path != ScenePath)
        {
            Debug.LogError("Akt7CenterMenuUIBuilder: open " + ScenePath + " before rebuilding.");
            return;
        }

        font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        whitePanel = LoadSprite(WhitePanelPath);
        Sprite background = LoadSprite(BackgroundPath);
        if (font == null || whitePanel == null || background == null)
        {
            Debug.LogError("Akt7CenterMenuUIBuilder: required font, panel, or background asset is missing.");
            return;
        }

        GameObject canvasObject = GameObject.Find("Canvas");
        Transform center = canvasObject != null ? canvasObject.transform.Find("Center") : null;
        Transform legacyContainer = center != null ? center.Find("GameObject") : null;
        if (center == null || legacyContainer == null)
        {
            Debug.LogError("Akt7CenterMenuUIBuilder: Canvas/Center/GameObject was not found.");
            return;
        }

        Button pengenalan = FindButton(legacyContainer, "Button (1)");
        Button keburukan = FindButton(legacyContainer, "Button (2)");
        Button microScene = FindButton(legacyContainer, "Button (3)");
        Button kuiz = FindButton(legacyContainer, "Button (4)");
        Button home = FindButton(legacyContainer, "HomeButton");
        if (pengenalan == null || keburukan == null || microScene == null || kuiz == null || home == null)
        {
            Debug.LogError("Akt7CenterMenuUIBuilder: one or more original navigation buttons are missing.");
            return;
        }

        Transform oldRoot = center.Find(RootName);
        if (oldRoot != null)
            Object.DestroyImmediate(oldRoot.gameObject);

        GameObject root = UI(RootName, center);
        Stretch(root.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero);
        root.transform.SetSiblingIndex(0);

        Image bg = root.AddComponent<Image>();
        bg.sprite = background;
        bg.color = Color.white;
        bg.preserveAspect = false;
        bg.raycastTarget = false;

        BuildHeader(root.transform);BuildMenuButton(root.transform, "PengenalanButton", "PENGENALAN",
            new Color32(103, 194, 17, 255), new Color32(40, 130, 10, 255),
            new Color32(226, 255, 190, 255), LoadSprite(BookPath), -306f, pengenalan);
        BuildMenuButton(root.transform, "KeburukanButton", "KEBURUKAN",
            new Color32(255, 153, 12, 255), new Color32(205, 88, 0, 255),
            new Color32(255, 226, 163, 255), LoadSprite(BadMicrobePath), -444f, keburukan);
        BuildMenuButton(root.transform, "MicroSceneButton", "MICRO-SCENE",
            new Color32(39, 160, 229, 255), new Color32(11, 91, 165, 255),
            new Color32(190, 237, 255, 255), LoadSprite(VideoPath), -582f, microScene);
        BuildMenuButton(root.transform, "KuizButton", "KUIZ",
            new Color32(242, 79, 132, 255), new Color32(177, 30, 82, 255),
            new Color32(255, 199, 220, 255), LoadSprite(QuizPath), -720f, kuiz);

        BuildCurriculumCard(root.transform);
        BuildHome(root.transform, home);

        Transform legacyTitle = center.Find("Title");
        if (legacyTitle != null)
            legacyTitle.gameObject.SetActive(false);
        legacyContainer.gameObject.SetActive(false);

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        if (scaler != null)
            scaler.matchWidthOrHeight = 0.5f;

        EditorUtility.SetDirty(center.gameObject);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Selection.activeGameObject = root;
        Debug.Log("Aktiviti 7 Center menu rebuilt. All five original navigation callbacks remain preserved through relays.");
    }

    private static void BuildHeader(Transform parent)
    {
        GameObject badge = Panel("ActivityBadge", parent, new Color32(255, 247, 207, 250));
        SetRect(badge.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -23f), new Vector2(410f, 80f));
        AddShadow(badge, new Color32(95, 72, 20, 65), new Vector2(0f, -6f));
        AddOutline(badge, Color.white, new Vector2(3f, 3f));
        TMP_Text badgeText = Text("Text", badge.transform, "AKTIVITI 7", 45f, new Color32(12, 91, 30, 255), TextAlignmentOptions.Center);
        Stretch(badgeText.rectTransform, new Vector2(12f, 3f), new Vector2(-12f, -3f));
        badgeText.fontStyle = FontStyles.Bold;

        TMP_Text title = Text("MainTitle", parent, "Mikroorganisma & Keburukan", 80f,
            new Color32(24, 124, 22, 255), TextAlignmentOptions.Center);
        SetRect(title.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -103f), new Vector2(1510f, 112f));
        title.fontStyle = FontStyles.Bold;
        title.outlineColor = Color.white;
        title.outlineWidth = 0.22f;
        AddShadow(title.gameObject, new Color32(50, 72, 20, 85), new Vector2(0f, -7f));

        TMP_Text subtitle = Text("Subtitle", parent,
            "Kenali mikroorganisma dan kesan keburukannya dalam kehidupan seharian!", 31f,
            new Color32(17, 75, 35, 255), TextAlignmentOptions.Center);
        SetRect(subtitle.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -220f), new Vector2(1250f, 48f));
        subtitle.fontStyle = FontStyles.Bold;
    }

    private static void BuildDecorations(Transform parent)
    {
        AddDecoration(parent, "GreenMicrobe", LoadSprite(GreenMicrobePath), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(290f, -280f), new Vector2(260f, 260f));
        AddDecoration(parent, "PurpleMicrobe", LoadSprite(BadMicrobePath), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-245f, -300f), new Vector2(250f, 250f));
        AddDecoration(parent, "Microscope", LoadSprite(MicroscopePath), new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(27f, 20f), new Vector2(350f, 470f));
        AddDecoration(parent, "PetriDish", LoadSprite(PetriPath), new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(235f, 18f), new Vector2(285f, 185f));
        AddDecoration(parent, "WaterDrop", LoadSprite(WaterDropPath), new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-285f, 72f), new Vector2(210f, 250f));
        AddDecoration(parent, "Magnifier", LoadSprite(MagnifierPath), new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-18f, 12f), new Vector2(325f, 360f));

        AddBubble(parent, "BubbleA", new Vector2(176f, -58f), 95f, new Color32(255, 255, 255, 95));
        AddBubble(parent, "BubbleB", new Vector2(-167f, -78f), 42f, new Color32(188, 239, 255, 115), true);
        AddBubble(parent, "BubbleC", new Vector2(465f, -165f), 30f, new Color32(124, 219, 118, 130));
        AddBubble(parent, "BubbleD", new Vector2(-437f, -207f), 48f, new Color32(255, 255, 255, 105), true);
        AddBubble(parent, "BubbleE", new Vector2(400f, 95f), 38f, new Color32(255, 255, 255, 110));
    }

    private static void BuildMenuButton(Transform parent, string objectName, string caption,
        Color32 baseColor, Color32 borderColor, Color32 wellColor, Sprite iconSprite, float topY, Button source)
    {
        GameObject go = UI(objectName, parent);
        SetRect(go.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(20f, topY), new Vector2(820f, 112f));
        Image image = go.AddComponent<Image>();
        image.sprite = whitePanel;
        image.type = Image.Type.Sliced;
        image.color = baseColor;
        image.raycastTarget = true;
        AddShadow(go, new Color32(borderColor.r, borderColor.g, borderColor.b, 150), new Vector2(0f, -9f));
        AddOutline(go, Color.white, new Vector2(3f, 3f));

        Button button = go.AddComponent<Button>();
        button.targetGraphic = image;
        button.navigation = new Navigation { mode = Navigation.Mode.None };
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.08f, 1.08f, 1.08f, 1f);
        colors.pressedColor = new Color(0.84f, 0.84f, 0.84f, 1f);
        colors.selectedColor = Color.white;
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.08f;
        button.colors = colors;

        GameObject shine = Panel("TopShine", go.transform, new Color(1f, 1f, 1f, 0.16f));
        RectTransform shineRect = shine.GetComponent<RectTransform>();
        shineRect.anchorMin = new Vector2(0f, 0.54f);
        shineRect.anchorMax = Vector2.one;
        shineRect.offsetMin = new Vector2(9f, 0f);
        shineRect.offsetMax = new Vector2(-9f, -7f);

        GameObject well = Panel("IconWell", go.transform, wellColor);
        SetRect(well.GetComponent<RectTransform>(), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(58f, 0f), new Vector2(126f, 126f));
        AddOutline(well, borderColor, new Vector2(3f, 3f));
        AddShadow(well, new Color32(borderColor.r, borderColor.g, borderColor.b, 95), new Vector2(0f, -5f));
        AddImage("Icon", well.transform, iconSprite, Vector2.one * 91f);

        TMP_Text label = Text("Label", go.transform, caption, 51f, Color.white, TextAlignmentOptions.Center);
        label.rectTransform.anchorMin = Vector2.zero;
        label.rectTransform.anchorMax = Vector2.one;
        label.rectTransform.offsetMin = new Vector2(145f, 8f);
        label.rectTransform.offsetMax = new Vector2(-102f, -8f);
        label.fontStyle = FontStyles.Bold;
        label.outlineColor = new Color32(borderColor.r, borderColor.g, borderColor.b, 190);
        label.outlineWidth = 0.12f;

        GameObject playWell = Panel("PlayWell", go.transform, new Color32(255, 183, 23, 255));
        SetRect(playWell.GetComponent<RectTransform>(), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-54f, 0f), new Vector2(78f, 78f));
        AddOutline(playWell, new Color32(208, 112, 0, 255), new Vector2(2f, 2f));
        AddImage("PlayIcon", playWell.transform, LoadSprite(PlayPath), new Vector2(48f, 48f));

        CenterMenuButtonRelay relay = go.AddComponent<CenterMenuButtonRelay>();
        SerializedObject relayObject = new SerializedObject(relay);
        relayObject.FindProperty("sourceButton").objectReferenceValue = source;
        relayObject.ApplyModifiedPropertiesWithoutUndo();
        UnityEventTools.AddPersistentListener(button.onClick, relay.InvokeSource);
    }

    private static void BuildCurriculumCard(Transform parent)
    {
        GameObject card = Panel("CurriculumCard", parent, new Color32(255, 248, 220, 248));
        SetRect(card.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(20f, 19f), new Vector2(430f, 122f));
        AddShadow(card, new Color32(95, 70, 25, 65), new Vector2(0f, -6f));
        AddOutline(card, Color.white, new Vector2(3f, 3f));
        TMP_Text text = Text("Text", card.transform, "SP: 3.1\nSK: 3.1.1, 3.1.5", 31f,
            new Color32(18, 72, 35, 255), TextAlignmentOptions.Center);
        Stretch(text.rectTransform, new Vector2(12f, 4f), new Vector2(-12f, -4f));
        text.fontStyle = FontStyles.Bold;
        text.lineSpacing = 2f;
    }

    private static void BuildHome(Transform parent, Button source)
    {
        GameObject go = Panel("HomeButton", parent, new Color32(255, 250, 225, 245));
        SetRect(go.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(27f, -27f), new Vector2(88f, 88f));
        AddShadow(go, new Color32(42, 72, 27, 80), new Vector2(0f, -6f));
        AddOutline(go, new Color32(27, 105, 40, 255), new Vector2(3f, 3f));
        Image image = go.GetComponent<Image>();
        image.raycastTarget = true;
        Button button = go.AddComponent<Button>();
        button.targetGraphic = image;
        button.navigation = new Navigation { mode = Navigation.Mode.None };
        AddImage("Icon", go.transform, LoadSprite(HomePath), new Vector2(58f, 58f));
        CenterMenuButtonRelay relay = go.AddComponent<CenterMenuButtonRelay>();
        SerializedObject relayObject = new SerializedObject(relay);
        relayObject.FindProperty("sourceButton").objectReferenceValue = source;
        relayObject.ApplyModifiedPropertiesWithoutUndo();
        UnityEventTools.AddPersistentListener(button.onClick, relay.InvokeSource);
    }

    private static void AddDecoration(Transform parent, string name, Sprite sprite, Vector2 anchor, Vector2 pivot, Vector2 position, Vector2 size)
    {
        if (sprite == null) return;
        GameObject go = UI(name, parent);
        SetRect(go.GetComponent<RectTransform>(), anchor, pivot, position, size);
        Image image = go.AddComponent<Image>();
        image.sprite = sprite;
        image.preserveAspect = true;
        image.raycastTarget = false;
    }

    private static void AddBubble(Transform parent, string name, Vector2 position, float size, Color color, bool right = false)
    {
        GameObject bubble = Panel(name, parent, color);
        Vector2 anchor = right ? new Vector2(1f, 0.5f) : new Vector2(0f, 0.5f);
        Vector2 pivot = anchor;
        SetRect(bubble.GetComponent<RectTransform>(), anchor, pivot, position, Vector2.one * size);
        AddOutline(bubble, new Color(1f, 1f, 1f, 0.55f), new Vector2(2f, 2f));
    }

    private static GameObject AddImage(string name, Transform parent, Sprite sprite, Vector2 size)
    {
        GameObject go = UI(name, parent);
        SetRect(go.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, size);
        Image image = go.AddComponent<Image>();
        image.sprite = sprite;
        image.preserveAspect = true;
        image.raycastTarget = false;
        return go;
    }

    private static TMP_Text Text(string name, Transform parent, string value, float size, Color color, TextAlignmentOptions alignment)
    {
        GameObject go = UI(name, parent);
        TextMeshProUGUI text = go.AddComponent<TextMeshProUGUI>();
        text.font = font;
        text.text = value;
        text.fontSize = size;
        text.color = color;
        text.alignment = alignment;
        text.enableWordWrapping = false;
        text.overflowMode = TextOverflowModes.Overflow;
        text.raycastTarget = false;
        return text;
    }

    private static GameObject Panel(string name, Transform parent, Color color)
    {
        GameObject go = UI(name, parent);
        Image image = go.AddComponent<Image>();
        image.sprite = whitePanel;
        image.type = Image.Type.Sliced;
        image.color = color;
        image.raycastTarget = false;
        return go;
    }

    private static GameObject UI(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.layer = parent.gameObject.layer;
        go.transform.SetParent(parent, false);
        return go;
    }

    private static Sprite LoadSprite(string path)
    {
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    private static Button FindButton(Transform parent, string name)
    {
        Transform child = parent.Find(name);
        return child != null ? child.GetComponent<Button>() : null;
    }

    private static void SetRect(RectTransform rect, Vector2 anchor, Vector2 pivot, Vector2 position, Vector2 size)
    {
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = pivot;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        rect.localScale = Vector3.one;
    }

    private static void Stretch(RectTransform rect, Vector2 minOffset, Vector2 maxOffset)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMin = minOffset;
        rect.offsetMax = maxOffset;
        rect.localScale = Vector3.one;
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
}
#endif
