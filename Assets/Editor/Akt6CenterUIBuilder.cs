#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Rebuilds the Activity 6 Center page with the approved clean reference
/// background and exactly three Unity-native menu buttons. Existing navigation
/// remains serialized on the retained legacy buttons and is invoked by relays.
/// </summary>
public static class Akt6CenterUIBuilder
{
    private const string ScenePath = "Assets/Scenes/Aktivity/akt6/akt6.unity";
    private const string RootName = "Akt6CenterReferenceRoot";
    private const string BackgroundPath = "Assets/Images/Akt6CenterUI/Background/akt6_center_reference_clean.png";
    private const string FontPath = "Assets/Fonts/FredokaOne SDF.asset";
    private const string ButtonSpritePath = "Assets/Images/UI Flat/btn_green_flat.png";
    private const string WhitePanelPath = "Assets/Images/UI Flat/btn_white_flat.png";
    private const string BookIconPath = "Assets/Images/UI Flat/icon_book_open.png";
    private const string KegunaanIconPath = "Assets/Images/UI Flat/char_green_microbe.png";
    private const string QuizIconPath = "Assets/Images/UI Flat/icon_quiz_checklist.png";
    private const string PlayIconPath = "Assets/Images/UI Flat/icon_play.png";

    private static TMP_FontAsset font;
    private static Sprite buttonSprite;
    private static Sprite whitePanel;
    private static Sprite playIcon;

    [MenuItem("Tools/MicroFun/Activity 6/Build Center Reference UI")]
    public static void Rebuild()
    {
        if (EditorSceneManager.GetActiveScene().path != ScenePath)
        {
            Debug.LogError("Akt6CenterUIBuilder: open " + ScenePath + " before rebuilding.");
            return;
        }

        ConfigureBackgroundSprite();
        font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        buttonSprite = AssetDatabase.LoadAssetAtPath<Sprite>(ButtonSpritePath);
        whitePanel = AssetDatabase.LoadAssetAtPath<Sprite>(WhitePanelPath);
        playIcon = AssetDatabase.LoadAssetAtPath<Sprite>(PlayIconPath);
        Sprite backgroundSprite = AssetDatabase.LoadAssetAtPath<Sprite>(BackgroundPath);
        if (font == null || buttonSprite == null || whitePanel == null || playIcon == null || backgroundSprite == null)
        {
            Debug.LogError("Akt6CenterUIBuilder: one or more required font, panel, icon, or background assets are missing.");
            return;
        }

        GameObject canvasObject = GameObject.Find("Canvas");
        Transform center = canvasObject != null ? canvasObject.transform.Find("Center") : null;
        Transform legacyMenu = center != null ? center.Find("GameObject") : null;
        if (canvasObject == null || center == null || legacyMenu == null)
        {
            Debug.LogError("Akt6CenterUIBuilder: Canvas/Center/GameObject was not found.");
            return;
        }

        Button pengenalan = FindButton(legacyMenu, "Button (1)");
        Button kegunaan = FindButton(legacyMenu, "Button (2)");
        Button kuiz = FindButton(legacyMenu, "Button (4)");
        Button home = FindButton(legacyMenu, "HomeButton");
        if (pengenalan == null || kegunaan == null || kuiz == null || home == null)
        {
            Debug.LogError("Akt6CenterUIBuilder: one or more required source buttons are missing.");
            return;
        }

        Transform existing = center.Find(RootName);
        if (existing != null)
            Object.DestroyImmediate(existing.gameObject);

        GameObject root = CreateUI(RootName, center);
        Stretch(root.GetComponent<RectTransform>());
        root.transform.SetSiblingIndex(0);

        Image background = root.AddComponent<Image>();
        background.sprite = backgroundSprite;
        background.preserveAspect = false;
        background.color = Color.white;
        background.raycastTarget = false;

        BuildHomeHotspot(root.transform, home);
        BuildVisibleBackButton(root.transform, home);
        BuildMenuButton(root.transform, "PengenalanButton", "Pengenalan",
            new Color32(25, 165, 229, 255), new Color32(5, 91, 160, 255),
            AssetDatabase.LoadAssetAtPath<Sprite>(BookIconPath), new Vector2(0f, -105f), pengenalan);
        BuildMenuButton(root.transform, "KegunaanButton", "Kegunaan",
            new Color32(105, 196, 23, 255), new Color32(35, 126, 10, 255),
            AssetDatabase.LoadAssetAtPath<Sprite>(KegunaanIconPath), new Vector2(0f, -240f), kegunaan);
        BuildMenuButton(root.transform, "KuizButton", "Kuiz",
            new Color32(139, 73, 218, 255), new Color32(78, 32, 148, 255),
            AssetDatabase.LoadAssetAtPath<Sprite>(QuizIconPath), new Vector2(0f, -375f), kuiz);

        Transform legacyTitle = center.Find("Title");
        if (legacyTitle != null)
            legacyTitle.gameObject.SetActive(false);
        legacyMenu.gameObject.SetActive(false);

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        if (scaler != null)
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
        }

        EditorUtility.SetDirty(center.gameObject);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Selection.activeGameObject = root;
        Debug.Log("Activity 6 Center UI rebuilt with Pengenalan, Kegunaan, and Kuiz only. Peranan is omitted and original callbacks are preserved.");
    }

    private static void BuildHomeHotspot(Transform parent, Button source)
    {
        GameObject go = CreateUI("ActivityBadgeHomeHotspot", parent);
        SetRect(go.GetComponent<RectTransform>(), new Vector2(0f, 475f), new Vector2(470f, 105f));

        Image hitArea = go.AddComponent<Image>();
        hitArea.color = new Color(1f, 1f, 1f, 0.001f);
        hitArea.raycastTarget = true;

        Button button = go.AddComponent<Button>();
        button.targetGraphic = hitArea;
        button.navigation = new Navigation { mode = Navigation.Mode.None };
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(1f, 1f, 1f, 0.001f);
        colors.highlightedColor = new Color(1f, 1f, 1f, 0.12f);
        colors.pressedColor = new Color(0.32f, 0.62f, 0.12f, 0.20f);
        colors.selectedColor = new Color(1f, 1f, 1f, 0.08f);
        colors.disabledColor = new Color(1f, 1f, 1f, 0.001f);
        colors.fadeDuration = 0.08f;
        button.colors = colors;

        AttachRelay(go, button, source);
    }

    private static void BuildMenuButton(Transform parent, string name, string labelValue,
        Color32 mainColor, Color32 darkColor, Sprite iconSprite, Vector2 position, Button source)
    {
        GameObject go = CreateUI(name, parent);
        SetRect(go.GetComponent<RectTransform>(), position, new Vector2(670f, 112f));

        Image panel = go.AddComponent<Image>();
        panel.sprite = buttonSprite;
        panel.type = Image.Type.Sliced;
        panel.color = mainColor;
        panel.raycastTarget = true;

        Shadow shadow = go.AddComponent<Shadow>();
        shadow.effectColor = new Color(darkColor.r / 255f, darkColor.g / 255f, darkColor.b / 255f, 0.72f);
        shadow.effectDistance = new Vector2(0f, -9f);
        shadow.useGraphicAlpha = true;

        Outline whiteOutline = go.AddComponent<Outline>();
        whiteOutline.effectColor = new Color32(255, 255, 255, 235);
        whiteOutline.effectDistance = new Vector2(4f, 4f);
        whiteOutline.useGraphicAlpha = true;

        Button button = go.AddComponent<Button>();
        button.targetGraphic = panel;
        button.navigation = new Navigation { mode = Navigation.Mode.None };
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.08f, 1.08f, 1.08f, 1f);
        colors.pressedColor = new Color(0.86f, 0.86f, 0.86f, 1f);
        colors.selectedColor = new Color(1.04f, 1.04f, 1.04f, 1f);
        colors.disabledColor = new Color(0.65f, 0.65f, 0.65f, 0.55f);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.08f;
        button.colors = colors;

        GameObject sheenObject = CreateUI("Sheen", go.transform);
        RectTransform sheenRect = sheenObject.GetComponent<RectTransform>();
        sheenRect.anchorMin = new Vector2(0f, 0.50f);
        sheenRect.anchorMax = new Vector2(1f, 1f);
        sheenRect.offsetMin = new Vector2(12f, -2f);
        sheenRect.offsetMax = new Vector2(-12f, -7f);
        Image sheen = sheenObject.AddComponent<Image>();
        sheen.sprite = whitePanel;
        sheen.type = Image.Type.Sliced;
        sheen.color = new Color(1f, 1f, 1f, 0.18f);
        sheen.raycastTarget = false;

        GameObject iconMedallion = CreateUI("IconMedallion", go.transform);
        SetRect(iconMedallion.GetComponent<RectTransform>(), new Vector2(-277f, 0f), new Vector2(104f, 104f));
        Image medallion = iconMedallion.AddComponent<Image>();
        medallion.sprite = whitePanel;
        medallion.type = Image.Type.Sliced;
        medallion.color = new Color32(255, 255, 255, 245);
        medallion.raycastTarget = false;
        Outline medallionOutline = iconMedallion.AddComponent<Outline>();
        medallionOutline.effectColor = darkColor;
        medallionOutline.effectDistance = new Vector2(3f, 3f);
        medallionOutline.useGraphicAlpha = true;

        if (iconSprite != null)
        {
            GameObject iconObject = CreateUI("Icon", iconMedallion.transform);
            SetRect(iconObject.GetComponent<RectTransform>(), Vector2.zero, new Vector2(74f, 74f));
            Image icon = iconObject.AddComponent<Image>();
            icon.sprite = iconSprite;
            icon.preserveAspect = true;
            icon.color = Color.white;
            icon.raycastTarget = false;
        }

        TextMeshProUGUI label = AddText("Label", go.transform, labelValue, 52f, Color.white);
        SetRect(label.rectTransform, new Vector2(10f, 0f), new Vector2(450f, 90f));
        label.fontStyle = FontStyles.Bold;
        label.outlineColor = new Color32(darkColor.r, darkColor.g, darkColor.b, 210);
        label.outlineWidth = 0.12f;
        label.enableAutoSizing = true;
        label.fontSizeMin = 38f;
        label.fontSizeMax = 52f;

        GameObject arrowMedallion = CreateUI("ArrowMedallion", go.transform);
        SetRect(arrowMedallion.GetComponent<RectTransform>(), new Vector2(284f, 0f), new Vector2(72f, 72f));
        Image arrowCircle = arrowMedallion.AddComponent<Image>();
        arrowCircle.sprite = whitePanel;
        arrowCircle.type = Image.Type.Sliced;
        arrowCircle.color = new Color32(255, 255, 255, 245);
        arrowCircle.raycastTarget = false;
        Outline arrowOutline = arrowMedallion.AddComponent<Outline>();
        arrowOutline.effectColor = darkColor;
        arrowOutline.effectDistance = new Vector2(2f, 2f);
        arrowOutline.useGraphicAlpha = true;

        GameObject arrowObject = CreateUI("Arrow", arrowMedallion.transform);
        SetRect(arrowObject.GetComponent<RectTransform>(), new Vector2(3f, 0f), new Vector2(38f, 38f));
        Image arrow = arrowObject.AddComponent<Image>();
        arrow.sprite = playIcon;
        arrow.preserveAspect = true;
        arrow.color = mainColor;
        arrow.raycastTarget = false;

        AttachRelay(go, button, source);
    }

    private static TextMeshProUGUI AddText(string name, Transform parent, string value, float size, Color color)
    {
        GameObject go = CreateUI(name, parent);
        TextMeshProUGUI text = go.AddComponent<TextMeshProUGUI>();
        text.font = font;
        text.text = value;
        text.fontSize = size;
        text.color = color;
        text.alignment = TextAlignmentOptions.Center;
        text.raycastTarget = false;
        text.overflowMode = TextOverflowModes.Overflow;
        return text;
    }

    private static void AttachRelay(GameObject host, Button generatedButton, Button source)
    {
        CenterMenuButtonRelay relay = host.AddComponent<CenterMenuButtonRelay>();
        SerializedObject serializedRelay = new SerializedObject(relay);
        serializedRelay.FindProperty("sourceButton").objectReferenceValue = source;
        serializedRelay.ApplyModifiedPropertiesWithoutUndo();
        UnityEventTools.AddPersistentListener(generatedButton.onClick, relay.InvokeSource);
    }

    private static Button FindButton(Transform parent, string name)
    {
        Transform child = parent.Find(name);
        return child != null ? child.GetComponent<Button>() : null;
    }

    private static GameObject CreateUI(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.layer = parent.gameObject.layer;
        go.transform.SetParent(parent, false);
        return go;
    }

    private static void SetRect(RectTransform rect, Vector2 position, Vector2 size)
    {
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;
    }

    private static void ConfigureBackgroundSprite()
    {
        AssetDatabase.ImportAsset(BackgroundPath, ImportAssetOptions.ForceSynchronousImport);
        TextureImporter importer = AssetImporter.GetAtPath(BackgroundPath) as TextureImporter;
        if (importer == null)
            return;

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = false;
        importer.mipmapEnabled = false;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.filterMode = FilterMode.Bilinear;
        importer.maxTextureSize = 2048;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.SaveAndReimport();
    }


private static void BuildVisibleBackButton(Transform parent, Button source)
    {
        GameObject go = CreateUI("BackButton", parent);
        SetRect(go.GetComponent<RectTransform>(), new Vector2(-900f, 470f), new Vector2(86f, 86f));

        Image panel = go.AddComponent<Image>();
        panel.sprite = buttonSprite;
        panel.type = Image.Type.Sliced;
        panel.color = new Color32(28, 158, 225, 255);
        panel.raycastTarget = true;

        Shadow shadow = go.AddComponent<Shadow>();
        shadow.effectColor = new Color32(9, 62, 105, 145);
        shadow.effectDistance = new Vector2(0f, -6f);
        shadow.useGraphicAlpha = true;

        Outline outline = go.AddComponent<Outline>();
        outline.effectColor = Color.white;
        outline.effectDistance = new Vector2(3f, 3f);
        outline.useGraphicAlpha = true;

        Button button = go.AddComponent<Button>();
        button.targetGraphic = panel;
        button.navigation = new Navigation { mode = Navigation.Mode.None };

        GameObject arrowObject = CreateUI("BackArrow", go.transform);
        SetRect(arrowObject.GetComponent<RectTransform>(), Vector2.zero, new Vector2(46f, 46f));
        Image arrow = arrowObject.AddComponent<Image>();
        arrow.sprite = playIcon;
        arrow.preserveAspect = true;
        arrow.color = Color.white;
        arrow.raycastTarget = false;
        arrow.rectTransform.localScale = new Vector3(-1f, 1f, 1f);

        AttachRelay(go, button, source);
    }
}
#endif
