#if UNITY_EDITOR
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class Akt4CenterMenuUIBuilder
{
    private const string ScenePath = "Assets/Scenes/Aktivity/akt4/akt4.unity";
    private const string RootName = "Akt4CenterRedesignRoot";
    private const string BackupPath = "Assets/Scenes/Aktivity/akt4/Backups/akt4_PreCenterMenuRedesign_20260904.unity";
    private const string BackgroundPath = "Assets/Images/Akt4CenterUI/Background/akt4_center_background.png";
    private const string WhitePanelPath = "Assets/Images/UI Flat/btn_white_flat.png";
    private const string CirclePath = "Assets/Images/UI Flat/ui_circle_white.png";
    private const string FontPath = "Assets/Fonts/FredokaOne SDF.asset";
    private const string BookPath = "Assets/Images/UI Flat/icon_book_open.png";
    private const string FactorPath = "Assets/Images/UI Flat/ic_settings.png";
    private const string MicroscopeCardPath = "Assets/Images/UI Flat/icon_microscope_card.png";
    private const string QuizPath = "Assets/Images/UI Flat/icon_quiz_checklist.png";
    private const string WaterDropPath = "Assets/Images/UI Flat/char_water_drop.png";
    private const string PurpleMicrobePath = "Assets/Images/UI Flat/char_purple_microbe.png";
    private const string MicroscopePath = "Assets/Images/UI Flat/img_microscope_large.png";
    private const string PetriPath = "Assets/Images/UI Flat/img_petri_bakteria.png";
    private const string HomePath = "Assets/Images/UI Flat/icon_home.png";

    private static TMP_FontAsset font;
    private static Sprite whitePanel;
    private static Sprite circle;

    [MenuItem("Tools/MicroFun/Rebuild Akt4 Center Menu")]
    public static void Rebuild()
    {
        if (EditorSceneManager.GetActiveScene().path != ScenePath)
        {
            Debug.LogError("Akt4CenterMenuUIBuilder: open " + ScenePath + " before rebuilding.");
            return;
        }

        EnsureBackground();
        font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        whitePanel = LoadSprite(WhitePanelPath);
        circle = LoadSprite(CirclePath);
        Sprite background = LoadSprite(BackgroundPath);
        Sprite book = LoadSprite(BookPath);
        Sprite factor = LoadSprite(FactorPath);
        Sprite microscopeCard = LoadSprite(MicroscopeCardPath);
        Sprite quiz = LoadSprite(QuizPath);
        Sprite waterDrop = LoadSprite(WaterDropPath);
        Sprite purpleMicrobe = LoadSprite(PurpleMicrobePath);
        Sprite microscope = LoadSprite(MicroscopePath);
        Sprite petri = LoadSprite(PetriPath);
        Sprite homeIcon = LoadSprite(HomePath);

        if (font == null || whitePanel == null || circle == null || background == null ||
            book == null || factor == null || microscopeCard == null || quiz == null ||
            waterDrop == null || purpleMicrobe == null || microscope == null ||
            petri == null || homeIcon == null)
        {
            Debug.LogError("Akt4CenterMenuUIBuilder: one or more required UI assets are missing.");
            return;
        }

        GameObject canvasObject = GameObject.Find("Canvas");
        Transform center = canvasObject != null ? canvasObject.transform.Find("Center") : null;
        Transform legacyContainer = center != null ? center.Find("GameObject") : null;
        if (center == null || legacyContainer == null)
        {
            Debug.LogError("Akt4CenterMenuUIBuilder: Canvas/Center/GameObject was not found.");
            return;
        }

        Button pengenalan = FindButton(legacyContainer, "Button (1)");
        Button faktor = FindButton(legacyContainer, "Button (2)");
        Button microScene = FindButton(legacyContainer, "Button (3)");
        Button kuiz = FindButton(legacyContainer, "Button (4)");
        Button home = FindButton(legacyContainer, "HomeButton");
        if (pengenalan == null || faktor == null || microScene == null || kuiz == null || home == null)
        {
            Debug.LogError("Akt4CenterMenuUIBuilder: one or more legacy navigation buttons are missing.");
            return;
        }

        CreateBackup();

        Transform existing = center.Find(RootName);
        if (existing != null)
            Object.DestroyImmediate(existing.gameObject);

        GameObject root = UI(RootName, center);
        Stretch(root.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero);
        root.transform.SetSiblingIndex(0);

        Image backgroundImage = root.AddComponent<Image>();
        backgroundImage.sprite = background;
        backgroundImage.color = Color.white;
        backgroundImage.preserveAspect = false;
        backgroundImage.raycastTarget = false;

        BuildDecorations(root.transform, waterDrop, purpleMicrobe, microscope, petri);
        BuildTitle(root.transform);

        GameObject stack = UI("MenuStack", root.transform);
        Stretch(stack.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero);

        BuildMenuButton(stack.transform, "PengenalanButton", "PENGENALAN",
            new Color32(180, 72, 218, 255), new Color32(102, 26, 150, 255),
            new Color32(226, 194, 244, 255), book, 235f, pengenalan);
        BuildMenuButton(stack.transform, "FaktorButton", "FAKTOR",
            new Color32(48, 132, 240, 255), new Color32(12, 77, 177, 255),
            new Color32(191, 226, 255, 255), factor, 85f, faktor);
        BuildMenuButton(stack.transform, "MicroSceneButton", "MICRO-SCENE",
            new Color32(108, 205, 12, 255), new Color32(39, 132, 0, 255),
            new Color32(222, 255, 175, 255), microscopeCard, -65f, microScene);
        BuildMenuButton(stack.transform, "KuizButton", "KUIZ",
            new Color32(255, 151, 10, 255), new Color32(211, 81, 0, 255),
            new Color32(255, 226, 168, 255), quiz, -215f, kuiz);

        BuildCurriculum(root.transform);
        BuildHome(root.transform, home, homeIcon);

        Transform legacyTitle = center.Find("Title");
        if (legacyTitle != null)
            legacyTitle.gameObject.SetActive(false);
        legacyContainer.gameObject.SetActive(false);

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
        Debug.Log("AKT4_CENTER_MENU_BUILD_SUCCESS: reference-style center menu built and legacy callbacks preserved.");
    }

    private static void BuildTitle(Transform parent)
    {
        GameObject plaque = Panel("TitlePlaque", parent, new Color32(220, 255, 185, 255));
        SetRect(plaque.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0f, -90f), new Vector2(940f, 142f));
        AddShadow(plaque, new Color32(52, 98, 15, 90), new Vector2(0f, -9f));
        AddOutline(plaque, Color.white, new Vector2(4f, 4f));

        GameObject inner = Panel("CreamInner", plaque.transform, new Color32(255, 252, 220, 255));
        SetRect(inner.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(916f, 120f));

        TMP_Text title = Text("TitleText", inner.transform, "AKTIVITI 4", 96f,
            new Color32(40, 116, 3, 255), TextAlignmentOptions.Center);
        Stretch(title.rectTransform, new Vector2(30f, 1f), new Vector2(-30f, -5f));
        title.fontStyle = FontStyles.Bold;
        title.outlineColor = Color.white;
        title.outlineWidth = 0.18f;
        AddShadow(title.gameObject, new Color32(39, 74, 0, 110), new Vector2(0f, -5f));

        TMP_Text topic = Text("TopicTitle", parent, "Faktor Pertumbuhan Mikroorganisma", 38f,
            new Color32(40, 116, 3, 255), TextAlignmentOptions.Center);
        SetRect(topic.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -204f), new Vector2(1100f, 58f));
        topic.fontStyle = FontStyles.Bold;
        topic.fontSharedMaterial = title.fontSharedMaterial;
        AddShadow(topic.gameObject, new Color32(39, 74, 0, 110), new Vector2(0f, -2f));

        AddLeaf(inner.transform, "LeafA", new Vector2(405f, -38f), new Vector2(40f, 20f), -24f, new Color32(52, 179, 32, 255));
        AddLeaf(inner.transform, "LeafB", new Vector2(431f, -26f), new Vector2(42f, 21f), 18f, new Color32(84, 205, 38, 255));
        AddLeaf(inner.transform, "LeafC", new Vector2(416f, -8f), new Vector2(35f, 18f), 58f, new Color32(36, 145, 25, 255));
    }

    private static void BuildMenuButton(
        Transform parent,
        string name,
        string caption,
        Color32 bodyColor,
        Color32 shadowColor,
        Color32 medallionColor,
        Sprite iconSprite,
        float y,
        Button sourceButton)
    {
        GameObject host = UI(name, parent);
        SetRect(host.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0f, y), new Vector2(980f, 122f));

        GameObject border = Panel("WhiteBorder", host.transform, Color.white);
        Stretch(border.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero);
        AddShadow(border, new Color32(shadowColor.r, shadowColor.g, shadowColor.b, 120), new Vector2(0f, -8f));
        AddOutline(border, new Color32(255, 255, 255, 235), new Vector2(3f, 3f));

        GameObject body = Panel("ColorBody", host.transform, bodyColor);
        Stretch(body.GetComponent<RectTransform>(), new Vector2(7f, 7f), new Vector2(-7f, -7f));
        Image bodyImage = body.GetComponent<Image>();
        bodyImage.raycastTarget = true;

        GameObject lowerTone = Panel("LowerTone", body.transform,
            new Color32(shadowColor.r, shadowColor.g, shadowColor.b, 70));
        RectTransform lowerRect = lowerTone.GetComponent<RectTransform>();
        lowerRect.anchorMin = new Vector2(0f, 0f);
        lowerRect.anchorMax = new Vector2(1f, 0.46f);
        lowerRect.offsetMin = new Vector2(4f, 4f);
        lowerRect.offsetMax = new Vector2(-4f, 0f);

        GameObject sheen = Panel("TopSheen", body.transform, new Color32(255, 255, 255, 38));
        RectTransform sheenRect = sheen.GetComponent<RectTransform>();
        sheenRect.anchorMin = new Vector2(0.02f, 0.56f);
        sheenRect.anchorMax = new Vector2(0.98f, 0.94f);
        sheenRect.offsetMin = Vector2.zero;
        sheenRect.offsetMax = Vector2.zero;

        GameObject medallion = Panel("IconMedallion", host.transform, Color.white);
        SetRect(medallion.GetComponent<RectTransform>(), new Vector2(0f, 0.5f), new Vector2(72f, 0f), new Vector2(170f, 118f));
        AddShadow(medallion, new Color32(shadowColor.r, shadowColor.g, shadowColor.b, 100), new Vector2(0f, -5f));

        GameObject medallionInner = Panel("MedallionInner", medallion.transform, medallionColor);
        SetRect(medallionInner.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(151f, 101f));

        Image icon = ImageObject("Icon", medallion.transform, iconSprite);
        SetRect(icon.rectTransform, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(98f, 94f));
        icon.preserveAspect = true;

        TMP_Text label = Text("Label", host.transform, caption, caption == "MICRO-SCENE" ? 55f : 59f,
            Color.white, TextAlignmentOptions.Center);
        SetRect(label.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(72f, 0f), new Vector2(760f, 94f));
        label.fontStyle = FontStyles.Bold;
        label.enableAutoSizing = true;
        label.fontSizeMin = 38f;
        label.fontSizeMax = caption == "MICRO-SCENE" ? 55f : 59f;
        label.outlineColor = new Color32(shadowColor.r, shadowColor.g, shadowColor.b, 235);
        label.outlineWidth = 0.16f;
        label.raycastTarget = false;
        AddShadow(label.gameObject, new Color32(shadowColor.r, shadowColor.g, shadowColor.b, 130), new Vector2(0f, -4f));

        Button button = host.AddComponent<Button>();
        button.targetGraphic = bodyImage;
        button.navigation = new Navigation { mode = Navigation.Mode.None };
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1f, 1f, 1f, 0.92f);
        colors.pressedColor = new Color(0.82f, 0.82f, 0.82f, 1f);
        colors.selectedColor = Color.white;
        colors.fadeDuration = 0.08f;
        button.colors = colors;

        CenterMenuButtonRelay relay = host.AddComponent<CenterMenuButtonRelay>();
        SerializedObject relayObject = new SerializedObject(relay);
        relayObject.FindProperty("sourceButton").objectReferenceValue = sourceButton;
        relayObject.ApplyModifiedPropertiesWithoutUndo();
        UnityEventTools.AddPersistentListener(button.onClick, relay.InvokeSource);
    }

    private static void BuildCurriculum(Transform parent)
    {
        GameObject card = Panel("CurriculumCard", parent, new Color32(255, 249, 221, 248));
        SetRect(card.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0f, -401f), new Vector2(500f, 132f));
        AddShadow(card, new Color32(100, 78, 18, 90), new Vector2(0f, -8f));
        AddOutline(card, Color.white, new Vector2(4f, 4f));

        TMP_Text text = Text("CurriculumText", card.transform, "SP: 3.1\nSK: 3.1.1, 3.1.3", 35f,
            new Color32(21, 92, 20, 255), TextAlignmentOptions.Center);
        Stretch(text.rectTransform, new Vector2(15f, 8f), new Vector2(-15f, -8f));
        text.fontStyle = FontStyles.Bold;
        text.lineSpacing = 8f;

        AddLeaf(card.transform, "LeafA", new Vector2(225f, -50f), new Vector2(38f, 19f), -30f, new Color32(45, 163, 28, 255));
        AddLeaf(card.transform, "LeafB", new Vector2(244f, -35f), new Vector2(39f, 19f), 28f, new Color32(92, 204, 42, 255));
    }

    private static void BuildHome(Transform parent, Button sourceButton, Sprite homeIcon)
    {
        GameObject host = UI("HomeButton", parent);
        SetRect(host.GetComponent<RectTransform>(), new Vector2(0f, 0f), new Vector2(66f, 66f), new Vector2(92f, 92f));

        GameObject panel = Panel("Panel", host.transform, new Color32(255, 247, 210, 250));
        Stretch(panel.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero);
        AddShadow(panel, new Color32(37, 92, 18, 95), new Vector2(0f, -6f));
        AddOutline(panel, Color.white, new Vector2(3f, 3f));
        Image panelImage = panel.GetComponent<Image>();
        panelImage.raycastTarget = true;

        Image icon = ImageObject("Icon", host.transform, homeIcon);
        SetRect(icon.rectTransform, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(58f, 58f));
        icon.preserveAspect = true;

        Button button = host.AddComponent<Button>();
        button.targetGraphic = panelImage;
        button.navigation = new Navigation { mode = Navigation.Mode.None };

        CenterMenuButtonRelay relay = host.AddComponent<CenterMenuButtonRelay>();
        SerializedObject relayObject = new SerializedObject(relay);
        relayObject.FindProperty("sourceButton").objectReferenceValue = sourceButton;
        relayObject.ApplyModifiedPropertiesWithoutUndo();
        UnityEventTools.AddPersistentListener(button.onClick, relay.InvokeSource);
    }

    private static void BuildDecorations(Transform parent, Sprite waterDrop, Sprite purpleMicrobe, Sprite microscope, Sprite petri)
    {
        GameObject decorations = UI("Decorations", parent);
        Stretch(decorations.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero);

        AddSprite(decorations.transform, "WaterDropHero", waterDrop, new Vector2(0f, 0f), new Vector2(205f, 155f), new Vector2(300f, 310f), 1f);
        AddSprite(decorations.transform, "PurpleMicrobeHero", purpleMicrobe, new Vector2(1f, 0f), new Vector2(-255f, 153f), new Vector2(235f, 215f), 1f);
        AddSprite(decorations.transform, "MicroscopeHero", microscope, new Vector2(1f, 0f), new Vector2(-150f, 158f), new Vector2(245f, 285f), 1f);
        AddSprite(decorations.transform, "PetriDishHero", petri, new Vector2(1f, 0f), new Vector2(-185f, 72f), new Vector2(280f, 125f), 1f);

        Bubble(decorations.transform, "BubbleL1", new Vector2(0f, 0.5f), new Vector2(105f, 250f), 44f, new Color32(88, 204, 255, 110));
        Bubble(decorations.transform, "BubbleL2", new Vector2(0f, 0.5f), new Vector2(285f, 365f), 27f, new Color32(55, 167, 244, 120));
        Bubble(decorations.transform, "BubbleT1", new Vector2(0.5f, 1f), new Vector2(-535f, -78f), 36f, new Color32(72, 185, 255, 115));
        Bubble(decorations.transform, "BubbleR1", new Vector2(1f, 0.5f), new Vector2(-125f, 345f), 38f, new Color32(255, 255, 255, 120));
        Bubble(decorations.transform, "BubbleR2", new Vector2(1f, 0.5f), new Vector2(-285f, 50f), 24f, new Color32(255, 255, 255, 95));

        Plus(decorations.transform, "PlusOrange", new Vector2(0f, 0.5f), new Vector2(360f, 260f), 64f, new Color32(255, 166, 0, 255), 12f);
        Plus(decorations.transform, "PlusPurple", new Vector2(1f, 0.5f), new Vector2(-305f, -30f), 61f, new Color32(126, 61, 239, 255), -9f);
        Plus(decorations.transform, "PlusGreen", new Vector2(1f, 0.5f), new Vector2(-160f, 225f), 58f, new Color32(91, 198, 28, 255), 9f);
    }

    private static void Bubble(Transform parent, string name, Vector2 anchor, Vector2 position, float size, Color32 color)
    {
        Image image = ImageObject(name, parent, circle);
        SetRect(image.rectTransform, anchor, position, new Vector2(size, size));
        image.color = color;
        AddOutline(image.gameObject, new Color32(255, 255, 255, 170), new Vector2(2f, 2f));
    }

    private static void Plus(Transform parent, string name, Vector2 anchor, Vector2 position, float size, Color32 color, float rotation)
    {
        TMP_Text plus = Text(name, parent, "+", size, color, TextAlignmentOptions.Center);
        SetRect(plus.rectTransform, anchor, position, new Vector2(size * 1.25f, size * 1.25f));
        plus.fontStyle = FontStyles.Bold;
        plus.outlineColor = Color.white;
        plus.outlineWidth = 0.22f;
        plus.rectTransform.localEulerAngles = new Vector3(0f, 0f, rotation);
    }

    private static void AddLeaf(Transform parent, string name, Vector2 position, Vector2 size, float rotation, Color32 color)
    {
        Image leaf = ImageObject(name, parent, circle);
        SetRect(leaf.rectTransform, new Vector2(0.5f, 0.5f), position, size);
        leaf.color = color;
        leaf.rectTransform.localEulerAngles = new Vector3(0f, 0f, rotation);
    }

    private static void AddSprite(Transform parent, string name, Sprite sprite, Vector2 anchor, Vector2 position, Vector2 size, float alpha)
    {
        Image image = ImageObject(name, parent, sprite);
        SetRect(image.rectTransform, anchor, position, size);
        image.preserveAspect = true;
        image.color = new Color(1f, 1f, 1f, alpha);
    }

    private static GameObject UI(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.layer = 5;
        go.transform.SetParent(parent, false);
        return go;
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

    private static Image ImageObject(string name, Transform parent, Sprite sprite)
    {
        GameObject go = UI(name, parent);
        Image image = go.AddComponent<Image>();
        image.sprite = sprite;
        image.color = Color.white;
        image.raycastTarget = false;
        return image;
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
        text.raycastTarget = false;
        text.enableWordWrapping = false;
        return text;
    }

    private static void SetRect(RectTransform rect, Vector2 anchor, Vector2 position, Vector2 size)
    {
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
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

    private static void AddShadow(GameObject target, Color color, Vector2 distance)
    {
        Shadow shadow = target.AddComponent<Shadow>();
        shadow.effectColor = color;
        shadow.effectDistance = distance;
        shadow.useGraphicAlpha = true;
    }

    private static void AddOutline(GameObject target, Color color, Vector2 distance)
    {
        Outline outline = target.AddComponent<Outline>();
        outline.effectColor = color;
        outline.effectDistance = distance;
        outline.useGraphicAlpha = true;
    }

    private static Button FindButton(Transform parent, string childName)
    {
        Transform child = parent.Find(childName);
        return child != null ? child.GetComponent<Button>() : null;
    }

    private static Sprite LoadSprite(string path)
    {
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    private static void CreateBackup()
    {
        const string parent = "Assets/Scenes/Aktivity/akt4";
        const string backups = parent + "/Backups";
        if (!AssetDatabase.IsValidFolder(backups))
            AssetDatabase.CreateFolder(parent, "Backups");

        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(BackupPath) == null)
        {
            if (!AssetDatabase.CopyAsset(ScenePath, BackupPath))
                Debug.LogWarning("Akt4CenterMenuUIBuilder: scene backup could not be created.");
            AssetDatabase.SaveAssets();
        }
    }

    private static void EnsureBackground()
    {
        string folderA = "Assets/Images/Akt4CenterUI";
        string folderB = folderA + "/Background";
        if (!AssetDatabase.IsValidFolder(folderA))
            AssetDatabase.CreateFolder("Assets/Images", "Akt4CenterUI");
        if (!AssetDatabase.IsValidFolder(folderB))
            AssetDatabase.CreateFolder(folderA, "Background");

        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), BackgroundPath);
        if (!File.Exists(fullPath))
        {
            const int width = 640;
            const int height = 360;
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            Color left = new Color32(218, 247, 145, 255);
            Color middle = new Color32(255, 248, 190, 255);
            Color right = new Color32(255, 197, 151, 255);

            for (int y = 0; y < height; y++)
            {
                float ny = y / (height - 1f);
                for (int x = 0; x < width; x++)
                {
                    float nx = x / (width - 1f);
                    Color horizontal = nx < 0.52f
                        ? Color.Lerp(left, middle, nx / 0.52f)
                        : Color.Lerp(middle, right, (nx - 0.52f) / 0.48f);
                    float centerGlow = Mathf.Clamp01(1f - Vector2.Distance(new Vector2(nx, ny), new Vector2(0.52f, 0.57f)) * 1.25f);
                    Color pixel = Color.Lerp(horizontal, new Color32(255, 251, 218, 255), centerGlow * 0.30f);
                    float edge = Mathf.Clamp01(Vector2.Distance(new Vector2(nx, ny), new Vector2(0.5f, 0.5f)) * 1.05f);
                    pixel = Color.Lerp(pixel, horizontal * 0.95f, edge * 0.12f);
                    pixel.a = 1f;
                    texture.SetPixel(x, y, pixel);
                }
            }

            texture.Apply(false, false);
            File.WriteAllBytes(fullPath, texture.EncodeToPNG());
            Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(BackgroundPath, ImportAssetOptions.ForceSynchronousImport);
        }

        TextureImporter importer = AssetImporter.GetAtPath(BackgroundPath) as TextureImporter;
        if (importer != null && (importer.textureType != TextureImporterType.Sprite || importer.mipmapEnabled))
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = false;
            importer.filterMode = FilterMode.Bilinear;
            importer.textureCompression = TextureImporterCompression.CompressedHQ;
            importer.SaveAndReimport();
        }
    }
}
#endif
