#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Builds the reference-led Aktiviti 8 Center landing menu while retaining the
/// original navigation buttons and their serialized callbacks.
/// </summary>
public static class Akt8CenterMenuUIBuilder
{
    private const string BackgroundPath = "Assets/Images/Akt8CenterMenuUI/Background/akt8_center_background.png";
    private const string PengenalanIconPath = "Assets/Images/Akt8CenterMenuUI/Icons/icon_pengenalan.png";
    private const string MicroSquadIconPath = "Assets/Images/Akt8CenterMenuUI/Icons/icon_micro_squad.png";
    private const string PembentanganIconPath = "Assets/Images/Akt8CenterMenuUI/Icons/icon_pembentangan.png";
    private const string MicroThinkerIconPath = "Assets/Images/Akt8CenterMenuUI/Icons/icon_micro_thinker.png";
    private const string WhitePanelPath = "Assets/Images/UI Flat/btn_white_flat.png";
    private const string PlayIconPath = "Assets/Images/UI Flat/icon_play.png";
    private const string HomeIconPath = "Assets/Images/UI Flat/icon_home.png";
    private const string FontPath = "Assets/Fonts/FredokaOne SDF.asset";

    private static TMP_FontAsset _font;
    private static Sprite _whitePanel;

    [MenuItem("Tools/MicroFun/Rebuild Akt8 Center Menu")]
    public static void Rebuild()
    {
        ConfigureSprite(BackgroundPath, false);
        ConfigureSprite(PengenalanIconPath, true);
        ConfigureSprite(MicroSquadIconPath, true);
        ConfigureSprite(PembentanganIconPath, true);
        ConfigureSprite(MicroThinkerIconPath, true);

        _font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        _whitePanel = AssetDatabase.LoadAssetAtPath<Sprite>(WhitePanelPath);
        Sprite backgroundSprite = AssetDatabase.LoadAssetAtPath<Sprite>(BackgroundPath);
        Sprite pengenalanIcon = AssetDatabase.LoadAssetAtPath<Sprite>(PengenalanIconPath);
        Sprite microSquadIcon = AssetDatabase.LoadAssetAtPath<Sprite>(MicroSquadIconPath);
        Sprite pembentanganIcon = AssetDatabase.LoadAssetAtPath<Sprite>(PembentanganIconPath);
        Sprite microThinkerIcon = AssetDatabase.LoadAssetAtPath<Sprite>(MicroThinkerIconPath);
        Sprite playIcon = AssetDatabase.LoadAssetAtPath<Sprite>(PlayIconPath);
        Sprite homeIcon = AssetDatabase.LoadAssetAtPath<Sprite>(HomeIconPath);

        GameObject canvasObject = GameObject.Find("Canvas");
        Transform center = canvasObject != null ? canvasObject.transform.Find("Center") : null;
        if (center == null)
        {
            Debug.LogError("Akt8CenterMenuUIBuilder: Canvas/Center was not found.");
            return;
        }

        Transform legacyTitle = center.Find("Title");
        Transform legacyContainer = center.Find("GameObject");
        if (legacyContainer == null)
        {
            Debug.LogError("Akt8CenterMenuUIBuilder: legacy Center/GameObject container was not found.");
            return;
        }

        Button legacyPengenalan = FindButton(legacyContainer, "Button (1)");
        Button legacyMicroSquad = FindButton(legacyContainer, "Button (2)");
        Button legacyPembentangan = FindButton(legacyContainer, "Button (3)");
        Button legacyHome = FindButton(legacyContainer, "HomeButton");

        Transform existing = center.Find("CenterMenuRedesignRoot");
        if (existing != null)
        {
            Object.DestroyImmediate(existing.gameObject);
        }

        GameObject root = CreateUI("CenterMenuRedesignRoot", center);
        RectTransform rootRect = root.GetComponent<RectTransform>();
        Stretch(rootRect, Vector2.zero, Vector2.zero);
        root.transform.SetSiblingIndex(0);

        Image background = root.AddComponent<Image>();
        background.sprite = backgroundSprite;
        background.color = Color.white;
        background.preserveAspect = false;
        background.raycastTarget = false;

        BuildHeader(root.transform);

        GameObject menuStack = CreateUI("MenuStack", root.transform);
        Stretch(menuStack.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero);

        BuildMenuButton(menuStack.transform, "PengenalanButton", "PENGENALAN",
            new Color32(103, 195, 20, 255), new Color32(36, 129, 12, 255),
            new Color32(218, 255, 174, 255), pengenalanIcon, playIcon, -310f, legacyPengenalan);

        BuildMenuButton(menuStack.transform, "MicroSquadButton", "MICRO SQUAD",
            new Color32(255, 157, 19, 255), new Color32(211, 91, 0, 255),
            new Color32(255, 222, 144, 255), microSquadIcon, playIcon, -450f, legacyMicroSquad);

        // The former no-callback Micro Thinker entry becomes Pembentangan.
        BuildMenuButton(menuStack.transform, "PembentanganButton", "PEMBENTANGAN",
            new Color32(31, 158, 231, 255), new Color32(11, 88, 168, 255),
            new Color32(184, 233, 255, 255), pembentanganIcon, playIcon, -590f, null);

        // Intentionally has no callback until its destination is supplied.
        BuildMenuButton(menuStack.transform, "MicroThinkerButton", "MICRO THINKER",
            new Color32(241, 76, 132, 255), new Color32(179, 31, 86, 255),
            new Color32(255, 193, 217, 255), microThinkerIcon, playIcon, -730f, null);

        BuildCurriculumBadge(root.transform);
        BuildHomeButton(root.transform, homeIcon, legacyHome);

        if (legacyTitle != null)
        {
            legacyTitle.gameObject.SetActive(false);
        }

        legacyContainer.gameObject.SetActive(false);

        // Preserve the user's clarified semantic change in the retained hierarchy.
        if (legacyPembentangan != null)
        {
            TMP_Text legacyLabel = legacyPembentangan.GetComponentInChildren<TMP_Text>(true);
            if (legacyLabel != null)
            {
                legacyLabel.text = "PEMBENTANGAN";
            }
        }

        EditorUtility.SetDirty(center.gameObject);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Selection.activeGameObject = root;

        Debug.Log("Aktiviti 8 Center menu rebuilt. Pengenalan, Micro Squad, and Home callbacks are preserved. Pembentangan and Micro Thinker intentionally have no callbacks.");
    }

    private static void BuildHeader(Transform parent)
    {
        GameObject badge = CreatePanel("ActivityBadge", parent, new Color32(255, 248, 205, 255));
        SetRect(badge.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -18f), new Vector2(420f, 78f));
        AddShadow(badge, new Color32(97, 73, 25, 75), new Vector2(0f, -6f));
        AddOutline(badge, Color.white, new Vector2(3f, 3f));
        TMP_Text badgeText = AddText("Text", badge.transform, "AKTIVITI 8", 46f, new Color32(16, 96, 32, 255), TextAlignmentOptions.Center);
        Stretch(badgeText.rectTransform, Vector2.zero, Vector2.zero);
        badgeText.fontStyle = FontStyles.Bold;

        TMP_Text title = AddText("Title", parent, "Micro Squad & Micro Thinker", 82f,
            new Color32(17, 118, 25, 255), TextAlignmentOptions.Center);
        SetRect(title.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -94f), new Vector2(1480f, 112f));
        title.fontStyle = FontStyles.Bold;
        title.outlineColor = Color.white;
        title.outlineWidth = 0.22f;
        AddShadow(title.gameObject, new Color32(43, 70, 18, 95), new Vector2(0f, -7f));

        TMP_Text subtitle = AddText("Subtitle", parent,
            "Mari bekerjasama, berbentang dan berfikir secara kreatif!", 31f,
            new Color32(19, 76, 39, 255), TextAlignmentOptions.Center);
        SetRect(subtitle.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -214f), new Vector2(1120f, 48f));
        subtitle.fontStyle = FontStyles.Bold;
    }

    private static Button BuildMenuButton(Transform parent, string objectName, string labelText,
        Color32 baseColor, Color32 borderColor, Color32 iconWellColor,
        Sprite iconSprite, Sprite playSprite, float topY, Button sourceButton)
    {
        GameObject buttonObject = CreateUI(objectName, parent);
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        SetRect(rect, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, topY), new Vector2(840f, 118f));

        Image image = buttonObject.AddComponent<Image>();
        image.sprite = _whitePanel;
        image.type = Image.Type.Sliced;
        image.color = baseColor;
        image.raycastTarget = true;
        AddShadow(buttonObject, new Color32(borderColor.r, borderColor.g, borderColor.b, 160), new Vector2(0f, -10f));
        AddOutline(buttonObject, Color.white, new Vector2(3f, 3f));

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.transition = Selectable.Transition.ColorTint;
        button.navigation = new Navigation { mode = Navigation.Mode.None };
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.08f, 1.08f, 1.08f, 1f);
        colors.pressedColor = new Color(0.86f, 0.86f, 0.86f, 1f);
        colors.selectedColor = Color.white;
        colors.disabledColor = new Color(0.75f, 0.75f, 0.75f, 0.75f);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.08f;
        button.colors = colors;

        GameObject highlight = CreateUI("TopHighlight", buttonObject.transform);
        RectTransform highlightRect = highlight.GetComponent<RectTransform>();
        highlightRect.anchorMin = new Vector2(0f, 0.52f);
        highlightRect.anchorMax = new Vector2(1f, 1f);
        highlightRect.offsetMin = new Vector2(8f, 0f);
        highlightRect.offsetMax = new Vector2(-8f, -7f);
        Image highlightImage = highlight.AddComponent<Image>();
        highlightImage.sprite = _whitePanel;
        highlightImage.type = Image.Type.Sliced;
        highlightImage.color = new Color(1f, 1f, 1f, 0.17f);
        highlightImage.raycastTarget = false;

        GameObject iconWell = CreatePanel("IconWell", buttonObject.transform, iconWellColor);
        SetRect(iconWell.GetComponent<RectTransform>(), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(62f, 0f), new Vector2(132f, 132f));
        AddOutline(iconWell, borderColor, new Vector2(3f, 3f));
        AddShadow(iconWell, new Color32(borderColor.r, borderColor.g, borderColor.b, 100), new Vector2(0f, -5f));

        GameObject iconObject = CreateUI("Icon", iconWell.transform);
        RectTransform iconRect = iconObject.GetComponent<RectTransform>();
        SetRect(iconRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(96f, 96f));
        Image iconImage = iconObject.AddComponent<Image>();
        iconImage.sprite = iconSprite;
        iconImage.preserveAspect = true;
        iconImage.color = Color.white;
        iconImage.raycastTarget = false;

        TMP_Text label = AddText("Label", buttonObject.transform, labelText, 52f, Color.white, TextAlignmentOptions.Center);
        label.rectTransform.anchorMin = Vector2.zero;
        label.rectTransform.anchorMax = Vector2.one;
        label.rectTransform.offsetMin = new Vector2(148f, 8f);
        label.rectTransform.offsetMax = new Vector2(-112f, -8f);
        label.fontStyle = FontStyles.Bold;
        label.outlineColor = new Color32(borderColor.r, borderColor.g, borderColor.b, 180);
        label.outlineWidth = 0.12f;
        label.raycastTarget = false;

        GameObject playObject = CreateUI("PlayIcon", buttonObject.transform);
        RectTransform playRect = playObject.GetComponent<RectTransform>();
        SetRect(playRect, new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-58f, 0f), new Vector2(82f, 82f));
        Image playImage = playObject.AddComponent<Image>();
        playImage.sprite = playSprite;
        playImage.preserveAspect = true;
        playImage.color = Color.white;
        playImage.raycastTarget = false;

        if (sourceButton != null)
        {
            CenterMenuButtonRelay relay = buttonObject.AddComponent<CenterMenuButtonRelay>();
            SerializedObject serialized = new SerializedObject(relay);
            serialized.FindProperty("sourceButton").objectReferenceValue = sourceButton;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            UnityEventTools.AddPersistentListener(button.onClick, relay.InvokeSource);
        }

        return button;
    }

    private static void BuildCurriculumBadge(Transform parent)
    {
        GameObject badge = CreatePanel("CurriculumBadge", parent, new Color32(255, 248, 220, 250));
        SetRect(badge.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 24f), new Vector2(430f, 128f));
        AddShadow(badge, new Color32(95, 71, 32, 70), new Vector2(0f, -6f));
        AddOutline(badge, Color.white, new Vector2(3f, 3f));

        TMP_Text text = AddText("Text", badge.transform, "SP: 3.1\nSK: 3.1.1, 3.1.5", 31f,
            new Color32(19, 83, 42, 255), TextAlignmentOptions.Center);
        Stretch(text.rectTransform, new Vector2(18f, 8f), new Vector2(-18f, -8f));
        text.fontStyle = FontStyles.Bold;
        text.lineSpacing = 2f;
    }

    private static void BuildHomeButton(Transform parent, Sprite homeSprite, Button sourceButton)
    {
        GameObject homeObject = CreateUI("HomeButton", parent);
        RectTransform rect = homeObject.GetComponent<RectTransform>();
        SetRect(rect, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(28f, -28f), new Vector2(86f, 86f));

        Image image = homeObject.AddComponent<Image>();
        image.sprite = _whitePanel;
        image.type = Image.Type.Sliced;
        image.color = new Color32(255, 250, 225, 245);
        image.raycastTarget = true;
        AddShadow(homeObject, new Color32(43, 73, 29, 90), new Vector2(0f, -6f));
        AddOutline(homeObject, new Color32(31, 111, 42, 255), new Vector2(3f, 3f));

        Button button = homeObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.navigation = new Navigation { mode = Navigation.Mode.None };

        GameObject iconObject = CreateUI("Icon", homeObject.transform);
        RectTransform iconRect = iconObject.GetComponent<RectTransform>();
        SetRect(iconRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(56f, 56f));
        Image icon = iconObject.AddComponent<Image>();
        icon.sprite = homeSprite;
        icon.preserveAspect = true;
        icon.color = new Color32(46, 157, 63, 255);
        icon.raycastTarget = false;

        if (sourceButton != null)
        {
            CenterMenuButtonRelay relay = homeObject.AddComponent<CenterMenuButtonRelay>();
            SerializedObject serialized = new SerializedObject(relay);
            serialized.FindProperty("sourceButton").objectReferenceValue = sourceButton;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            UnityEventTools.AddPersistentListener(button.onClick, relay.InvokeSource);
        }
    }

    private static TMP_Text AddText(string name, Transform parent, string value, float size, Color color, TextAlignmentOptions alignment)
    {
        GameObject go = CreateUI(name, parent);
        TextMeshProUGUI text = go.AddComponent<TextMeshProUGUI>();
        text.font = _font;
        text.text = value;
        text.fontSize = size;
        text.color = color;
        text.alignment = alignment;
        text.enableWordWrapping = false;
        text.overflowMode = TextOverflowModes.Overflow;
        text.raycastTarget = false;
        return text;
    }

    private static GameObject CreatePanel(string name, Transform parent, Color color)
    {
        GameObject go = CreateUI(name, parent);
        Image image = go.AddComponent<Image>();
        image.sprite = _whitePanel;
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

    private static Button FindButton(Transform parent, string name)
    {
        Transform child = parent.Find(name);
        return child != null ? child.GetComponent<Button>() : null;
    }

    private static void ConfigureSprite(string path, bool alpha)
    {
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
        {
            return;
        }

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = alpha;
        importer.mipmapEnabled = false;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.filterMode = FilterMode.Bilinear;
        importer.maxTextureSize = 2048;
        importer.SaveAndReimport();
    }
}
#endif
