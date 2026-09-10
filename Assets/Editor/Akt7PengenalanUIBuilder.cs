#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Rebuilds only Canvas/Pengenalan in Activity 7. Existing Home and Next
/// buttons remain serialized callback sources and are invoked through relays.
/// </summary>
public static class Akt7PengenalanUIBuilder
{
    private const string ScenePath = "Assets/Scenes/Aktivity/akt7/akt7.unity";
    private const string RootName = "Akt7PengenalanRedesignRoot";
    private const string BackgroundPath = "Assets/Images/Akt7PengenalanUI/Background/akt7_pengenalan_reference_background.png";
    private const string WhitePanelPath = "Assets/Images/UI Flat/btn_white_flat.png";
    private const string HomeIconPath = "Assets/Images/UI Flat/icon_home.png";
    private const string PlayIconPath = "Assets/Images/UI Flat/icon_play.png";
    private const string FontPath = "Assets/Fonts/FredokaOne SDF.asset";

    private static TMP_FontAsset font;
    private static Sprite whitePanel;

    [MenuItem("Tools/MicroFun/Rebuild Akt7 Pengenalan UI")]
    public static void Rebuild()
    {
        if (EditorSceneManager.GetActiveScene().path != ScenePath)
        {
            Debug.LogError("Akt7PengenalanUIBuilder: open " + ScenePath + " before rebuilding.");
            return;
        }

        ConfigureSprite(BackgroundPath);
        font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        whitePanel = AssetDatabase.LoadAssetAtPath<Sprite>(WhitePanelPath);
        Sprite backgroundSprite = AssetDatabase.LoadAssetAtPath<Sprite>(BackgroundPath);
        Sprite homeSprite = AssetDatabase.LoadAssetAtPath<Sprite>(HomeIconPath);
        Sprite playSprite = AssetDatabase.LoadAssetAtPath<Sprite>(PlayIconPath);
        if (font == null || whitePanel == null || backgroundSprite == null)
        {
            Debug.LogError("Akt7PengenalanUIBuilder: a required font, panel, or background asset is missing.");
            return;
        }

        GameObject canvasObject = GameObject.Find("Canvas");
        Transform pengenalan = canvasObject != null ? canvasObject.transform.Find("Pengenalan") : null;
        if (pengenalan == null)
        {
            Debug.LogError("Akt7PengenalanUIBuilder: Canvas/Pengenalan was not found.");
            return;
        }

        Button sourceNext = FindButton(pengenalan, "Bottom/NextButton");
        Button sourceHome = FindButton(pengenalan, "Bottom/HomeButton");
        if (sourceNext == null || sourceHome == null)
        {
            Debug.LogError("Akt7PengenalanUIBuilder: legacy Home or Next button is missing.");
            return;
        }

        Transform existing = pengenalan.Find(RootName);
        if (existing != null)
            Object.DestroyImmediate(existing.gameObject);

        GameObject root = CreateUI(RootName, pengenalan);
        Stretch(root.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero);
        root.transform.SetSiblingIndex(0);

        Image background = root.AddComponent<Image>();
        background.sprite = backgroundSprite;
        background.color = Color.white;
        background.preserveAspect = false;
        background.raycastTarget = false;

        BuildHeader(root.transform);
        BuildQuestion(root.transform);
        BuildQuestionMark(root.transform);
        BuildNavigation(root.transform, sourceHome, sourceNext, homeSprite, playSprite);

        SetChildActive(pengenalan, "Top", false);
        SetChildActive(pengenalan, "Title", false);
        SetChildActive(pengenalan, "Content", false);
        SetChildActive(pengenalan, "Bottom", false);
        SetChildActive(pengenalan, "DialogParentRight Variant", false);
        SetChildActive(pengenalan, "DialogParentLeft Variant", false);
        SetChildActive(pengenalan, "DialogParentRight Variant (1)", false);
        SetChildActive(pengenalan, "DialogParentLeft Variant (1)", false);
        SetChildActive(pengenalan, "DialogParentRight Variant (2)", false);

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        if (scaler != null)
            scaler.matchWidthOrHeight = 0.5f;

        EditorUtility.SetDirty(pengenalan.gameObject);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Selection.activeGameObject = root;
        Debug.Log("Activity 7 Pengenalan UI rebuilt. Original Home and Next callbacks are preserved through relays.");
    }

    private static void BuildHeader(Transform parent)
    {
        GameObject badge = CreatePanel("ActivityBadge", parent, new Color32(188, 244, 239, 250));
        SetRect(badge.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -30f), new Vector2(500f, 90f));
        AddShadow(badge, new Color32(44, 76, 58, 70), new Vector2(0f, -6f));
        AddOutline(badge, Color.white, new Vector2(3f, 3f));
        TMP_Text badgeText = AddText("Text", badge.transform, "AKTIVITI 7", 50f, new Color32(10, 76, 24, 255), TextAlignmentOptions.Center);
        Stretch(badgeText.rectTransform, new Vector2(12f, 2f), new Vector2(-12f, -2f));
        badgeText.fontStyle = FontStyles.Bold;

        TMP_Text title = AddText("MainTitle", parent, "Pengenalan", 112f, new Color32(21, 118, 18, 255), TextAlignmentOptions.Center);
        SetRect(title.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -116f), new Vector2(1120f, 150f));
        title.fontStyle = FontStyles.Bold;
        title.outlineColor = Color.white;
        title.outlineWidth = 0.24f;
        AddShadow(title.gameObject, new Color32(45, 64, 21, 90), new Vector2(0f, -8f));

        GameObject subtitlePanel = CreatePanel("SubtitlePanel", parent, new Color32(255, 252, 230, 245));
        SetRect(subtitlePanel.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -282f), new Vector2(1110f, 78f));
        AddShadow(subtitlePanel, new Color32(69, 92, 34, 55), new Vector2(0f, -5f));
        AddOutline(subtitlePanel, new Color32(70, 157, 35, 255), new Vector2(3f, 3f));
        TMP_Text subtitle = AddText("Text", subtitlePanel.transform,
            "Mikroorganisma juga memberikan kesan keburukan.", 37f,
            new Color32(18, 75, 33, 255), TextAlignmentOptions.Center);
        Stretch(subtitle.rectTransform, new Vector2(24f, 4f), new Vector2(-24f, -4f));
        subtitle.fontStyle = FontStyles.Bold;
    }

    private static void BuildQuestion(Transform parent)
    {
        TMP_Text question = AddText("QuestionText", parent,
            "Bagaimanakah\nmikroorganisma\nmemberikan kesan\nburuk kepada kita?", 58f,
            new Color32(10, 70, 25, 255), TextAlignmentOptions.Center);
        SetRect(question.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(150f, -520f), new Vector2(700f, 340f));
        question.fontStyle = FontStyles.Bold;
        question.enableAutoSizing = true;
        question.fontSizeMin = 38f;
        question.fontSizeMax = 58f;
        question.lineSpacing = 3f;
    }

    private static void BuildQuestionMark(Transform parent)
    {
        TMP_Text mark = AddText("QuestionMark", parent, "?", 118f,
            new Color32(246, 76, 62, 255), TextAlignmentOptions.Center);
        SetRect(mark.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(165f, -236f), new Vector2(150f, 175f));
        mark.fontStyle = FontStyles.Bold;
        mark.outlineColor = new Color32(24, 26, 24, 255);
        mark.outlineWidth = 0.22f;
    }

    private static void BuildNavigation(Transform parent, Button sourceHome, Button sourceNext, Sprite homeSprite, Sprite playSprite)
    {
        BuildRelayButton(parent, "HomeButton", new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(24f, -24f),
            new Vector2(88f, 88f), new Color32(255, 250, 227, 245), new Color32(34, 105, 48, 255), homeSprite, sourceHome);

        BuildRelayButton(parent, "NextButton", new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-26f, 26f),
            new Vector2(92f, 92f), new Color32(255, 187, 24, 250), new Color32(199, 107, 0, 255), playSprite, sourceNext);
    }

    private static void BuildRelayButton(Transform parent, string name, Vector2 anchor, Vector2 pivot, Vector2 position,
        Vector2 size, Color32 baseColor, Color32 outlineColor, Sprite iconSprite, Button source)
    {
        GameObject go = CreatePanel(name, parent, baseColor);
        SetRect(go.GetComponent<RectTransform>(), anchor, pivot, position, size);
        Image image = go.GetComponent<Image>();
        image.raycastTarget = true;
        AddOutline(go, outlineColor, new Vector2(3f, 3f));
        AddShadow(go, new Color32(outlineColor.r, outlineColor.g, outlineColor.b, 95), new Vector2(0f, -6f));

        Button button = go.AddComponent<Button>();
        button.targetGraphic = image;
        button.navigation = new Navigation { mode = Navigation.Mode.None };
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.08f, 1.08f, 1.08f, 1f);
        colors.pressedColor = new Color(0.82f, 0.82f, 0.82f, 1f);
        colors.selectedColor = Color.white;
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.08f;
        button.colors = colors;

        if (iconSprite != null)
        {
            GameObject iconObject = CreateUI("Icon", go.transform);
            SetRect(iconObject.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, size * 0.63f);
            Image icon = iconObject.AddComponent<Image>();
            icon.sprite = iconSprite;
            icon.preserveAspect = true;
            icon.color = name == "HomeButton" ? new Color32(40, 150, 55, 255) : Color.white;
            icon.raycastTarget = false;
        }

        CenterMenuButtonRelay relay = go.AddComponent<CenterMenuButtonRelay>();
        SerializedObject relayObject = new SerializedObject(relay);
        relayObject.FindProperty("sourceButton").objectReferenceValue = source;
        relayObject.ApplyModifiedPropertiesWithoutUndo();
        UnityEventTools.AddPersistentListener(button.onClick, relay.InvokeSource);
    }

    private static TMP_Text AddText(string name, Transform parent, string value, float size, Color color, TextAlignmentOptions alignment)
    {
        GameObject go = CreateUI(name, parent);
        TextMeshProUGUI text = go.AddComponent<TextMeshProUGUI>();
        text.font = font;
        text.text = value;
        text.fontSize = size;
        text.color = color;
        text.alignment = alignment;text.overflowMode = TextOverflowModes.Overflow;
        text.raycastTarget = false;
        return text;
    }

    private static GameObject CreatePanel(string name, Transform parent, Color color)
    {
        GameObject go = CreateUI(name, parent);
        Image image = go.AddComponent<Image>();
        image.sprite = whitePanel;
        image.type = Image.Type.Sliced;
        image.color = color;
        image.raycastTarget = false;
        return go;
    }

    private static GameObject CreateUI(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.layer = parent.gameObject.layer;
        go.transform.SetParent(parent, false);
        return go;
    }

    private static Button FindButton(Transform parent, string path)
    {
        Transform child = parent.Find(path);
        return child != null ? child.GetComponent<Button>() : null;
    }

    private static void SetChildActive(Transform parent, string path, bool active)
    {
        Transform child = parent.Find(path);
        if (child != null)
            child.gameObject.SetActive(active);
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

    private static void ConfigureSprite(string path)
    {
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
            return;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = false;
        importer.mipmapEnabled = false;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.filterMode = FilterMode.Bilinear;
        importer.maxTextureSize = 2048;
        importer.SaveAndReimport();
    }
}
#endif
