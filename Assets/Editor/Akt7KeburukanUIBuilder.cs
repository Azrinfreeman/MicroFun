#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Builds the four-card Activity 7 Keburukan screen while retaining every
/// original card and Home callback as serialized source controls.
/// </summary>
public static class Akt7KeburukanUIBuilder
{
    private const string ScenePath = "Assets/Scenes/Aktivity/akt7/akt7.unity";
    private const string RootName = "Akt7KeburukanRedesignRoot";
    private const string BackgroundPath = "Assets/Images/Akt7KeburukanUI/Background/akt7_keburukan_four_cards.png";
    private const string WhitePanelPath = "Assets/Images/UI Flat/btn_white_flat.png";
    private const string HomeIconPath = "Assets/Images/UI Flat/icon_home.png";
    private const string FontPath = "Assets/Fonts/FredokaOne SDF.asset";

    private static TMP_FontAsset font;
    private static Sprite whitePanel;

    [MenuItem("Tools/MicroFun/Rebuild Akt7 Keburukan UI")]
    public static void Rebuild()
    {
        if (EditorSceneManager.GetActiveScene().path != ScenePath)
        {
            Debug.LogError("Akt7KeburukanUIBuilder: open " + ScenePath + " before rebuilding.");
            return;
        }

        ConfigureSprite(BackgroundPath);
        font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        whitePanel = AssetDatabase.LoadAssetAtPath<Sprite>(WhitePanelPath);
        Sprite backgroundSprite = AssetDatabase.LoadAssetAtPath<Sprite>(BackgroundPath);
        Sprite homeSprite = AssetDatabase.LoadAssetAtPath<Sprite>(HomeIconPath);
        if (font == null || whitePanel == null || backgroundSprite == null)
        {
            Debug.LogError("Akt7KeburukanUIBuilder: required font, panel, or background asset is missing.");
            return;
        }

        GameObject canvasObject = GameObject.Find("Canvas");
        Transform keburukan = canvasObject != null ? canvasObject.transform.Find("Keburukan") : null;
        Transform legacyCenter = keburukan != null ? keburukan.Find("Center") : null;
        if (keburukan == null || legacyCenter == null)
        {
            Debug.LogError("Akt7KeburukanUIBuilder: Canvas/Keburukan/Center was not found.");
            return;
        }

        Button food = FindButton(legacyCenter, "Button (2)");
        Button tooth = FindButton(legacyCenter, "Button (5)");
        Button poisoning = FindButton(legacyCenter, "Button (6)");
        Button plant = FindButton(legacyCenter, "Button (3)");
        Button home = FindButton(keburukan, "Bottom/HomeButton");
        if (food == null || tooth == null || poisoning == null || plant == null || home == null)
        {
            Debug.LogError("Akt7KeburukanUIBuilder: one or more required source buttons are missing.");
            return;
        }

        Transform existing = keburukan.Find(RootName);
        if (existing != null)
            Object.DestroyImmediate(existing.gameObject);

        GameObject root = CreateUI(RootName, keburukan);
        Stretch(root.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero);
        root.transform.SetSiblingIndex(0);

        Image background = root.AddComponent<Image>();
        background.sprite = backgroundSprite;
        background.color = Color.white;
        background.preserveAspect = false;
        background.raycastTarget = false;

        BuildHeader(root.transform);
        BuildCard(root.transform, "Card01SpoiledFood", "1", "Merosakkan\nmakanan", -650f,
            new Color32(255, 147, 7, 255), food, 37f);
        BuildCard(root.transform, "Card02ToothDecay", "2", "Mereputkan\ngigi", -220f,
            new Color32(242, 52, 73, 255), tooth, 39f);
        BuildCard(root.transform, "Card03FoodPoisoning", "3", "Menyebabkan\nkeracunan makanan", 210f,
            new Color32(111, 47, 202, 255), poisoning, 31f);
        BuildCard(root.transform, "Card04PlantDamage", "4", "Merosakkan\ntanaman", 620f,
            new Color32(35, 154, 107, 255), plant, 37f);
        BuildHome(root.transform, home, homeSprite);

        legacyCenter.gameObject.SetActive(false);
        Transform bottom = keburukan.Find("Bottom");
        if (bottom != null)
            bottom.gameObject.SetActive(false);

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        if (scaler != null)
            scaler.matchWidthOrHeight = 0.5f;

        EditorUtility.SetDirty(keburukan.gameObject);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Selection.activeGameObject = root;
        Debug.Log("Activity 7 Keburukan UI rebuilt with four illustrated cards. Original card and Home callbacks are preserved through relays.");
    }

    private static void BuildHeader(Transform parent)
    {
        GameObject badge = CreatePanel("ActivityBadge", parent, new Color32(255, 176, 8, 250));
        SetRect(badge.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -26f), new Vector2(450f, 102f));
        AddOutline(badge, Color.white, new Vector2(4f, 4f));
        AddShadow(badge, new Color32(118, 71, 9, 85), new Vector2(0f, -8f));
        TMP_Text badgeText = AddText("Text", badge.transform, "AKTIVITI 7", 54f, new Color32(9, 80, 26, 255), TextAlignmentOptions.Center);
        Stretch(badgeText.rectTransform, new Vector2(12f, 2f), new Vector2(-12f, -2f));
        badgeText.fontStyle = FontStyles.Bold;
        badgeText.outlineColor = Color.white;
        badgeText.outlineWidth = 0.18f;

        TMP_Text title = AddText("MainTitle", parent, "Keburukan Mikroorganisma", 82f,
            new Color32(18, 111, 20, 255), TextAlignmentOptions.Center);
        SetRect(title.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -139f), new Vector2(1510f, 115f));
        title.fontStyle = FontStyles.Bold;
        title.outlineColor = Color.white;
        title.outlineWidth = 0.22f;
        AddShadow(title.gameObject, new Color32(44, 62, 18, 90), new Vector2(0f, -7f));

        GameObject pill = CreatePanel("InstructionPill", parent, new Color32(255, 252, 231, 247));
        SetRect(pill.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -270f), new Vector2(1150f, 76f));
        AddOutline(pill, new Color32(72, 154, 36, 255), new Vector2(3f, 3f));
        AddShadow(pill, new Color32(66, 81, 32, 55), new Vector2(0f, -5f));
        TMP_Text instruction = AddText("Text", pill.transform,
            "Pilih kad untuk meneroka kesan keburukan mikroorganisma.", 32f,
            new Color32(16, 69, 31, 255), TextAlignmentOptions.Center);
        Stretch(instruction.rectTransform, new Vector2(25f, 3f), new Vector2(-25f, -3f));
        instruction.fontStyle = FontStyles.Bold;
    }

    private static void BuildCard(Transform parent, string objectName, string number, string label,
        float x, Color32 accent, Button source, float labelSize)
    {
        GameObject card = CreateUI(objectName, parent);
        SetRect(card.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(x, -372f), new Vector2(398f, 568f));

        Image hitArea = card.AddComponent<Image>();
        hitArea.color = Color.clear;
        hitArea.raycastTarget = true;

        Button button = card.AddComponent<Button>();
        button.targetGraphic = hitArea;
        button.navigation = new Navigation { mode = Navigation.Mode.None };
        ColorBlock colors = button.colors;
        colors.normalColor = Color.clear;
        colors.highlightedColor = new Color(1f, 1f, 1f, 0.12f);
        colors.pressedColor = new Color(accent.r / 255f, accent.g / 255f, accent.b / 255f, 0.18f);
        colors.selectedColor = new Color(1f, 1f, 1f, 0.10f);
        colors.disabledColor = Color.clear;
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.08f;
        button.colors = colors;

        TMP_Text numberText = AddText("Number", card.transform, number, 54f, Color.white, TextAlignmentOptions.Center);
        SetRect(numberText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 0.5f), new Vector2(0f, -16f), new Vector2(84f, 84f));
        numberText.fontStyle = FontStyles.Bold;
        numberText.outlineColor = new Color32(81, 54, 40, 190);
        numberText.outlineWidth = 0.16f;

        TMP_Text labelText = AddText("Label", card.transform, label, labelSize, Color.white, TextAlignmentOptions.Center);
        SetRect(labelText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 0.5f), new Vector2(0f, -451f), new Vector2(350f, 140f));
        labelText.fontStyle = FontStyles.Bold;
        labelText.enableAutoSizing = true;
        labelText.fontSizeMin = 25f;
        labelText.fontSizeMax = labelSize;
        labelText.lineSpacing = 2f;
        labelText.outlineColor = new Color32(83, 47, 52, 155);
        labelText.outlineWidth = 0.11f;

        CenterMenuButtonRelay relay = card.AddComponent<CenterMenuButtonRelay>();
        SerializedObject relayObject = new SerializedObject(relay);
        relayObject.FindProperty("sourceButton").objectReferenceValue = source;
        relayObject.ApplyModifiedPropertiesWithoutUndo();
        UnityEventTools.AddPersistentListener(button.onClick, relay.InvokeSource);
    }

    private static void BuildHome(Transform parent, Button source, Sprite iconSprite)
    {
        GameObject go = CreatePanel("HomeButton", parent, new Color32(255, 250, 225, 245));
        SetRect(go.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(24f, -24f), new Vector2(88f, 88f));
        Image image = go.GetComponent<Image>();
        image.raycastTarget = true;
        AddOutline(go, new Color32(31, 105, 42, 255), new Vector2(3f, 3f));
        AddShadow(go, new Color32(42, 73, 29, 80), new Vector2(0f, -6f));

        Button button = go.AddComponent<Button>();
        button.targetGraphic = image;
        button.navigation = new Navigation { mode = Navigation.Mode.None };

        if (iconSprite != null)
        {
            GameObject iconObject = CreateUI("Icon", go.transform);
            SetRect(iconObject.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(58f, 58f));
            Image icon = iconObject.AddComponent<Image>();
            icon.sprite = iconSprite;
            icon.preserveAspect = true;
            icon.color = new Color32(40, 150, 55, 255);
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
        text.alignment = alignment;
        text.overflowMode = TextOverflowModes.Overflow;
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
