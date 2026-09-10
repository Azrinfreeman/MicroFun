using System;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Akt5KehadiranNutrienUIBuilder
{
    const string ScenePath = "Assets/Scenes/Aktivity/akt5/akt5.unity";
    const string BackupPath = "Assets/Scenes/Aktivity/akt5/Backups/akt5_PreKehadiranNutrienRedesign_20260903.unity";
    const string BackgroundPath = "Assets/Images/Akt5CenterUI/Background/akt5_factor_background_v2.png";
    const string WaterPath = "Assets/Images/Akt5KehadiranNutrienUI/distilled_water_glass_v1.png";
    const string MilkPath = "Assets/Images/Akt5KehadiranNutrienUI/fresh_milk_carton_glass_v1.png";
    const string YeastSugarPath = "Assets/Images/Akt5KehadiranNutrienUI/yeast_sugar_v1.png";
    const string StudentOnePath = "Assets/Images/2d/student1.png";
    const string StudentTwoPath = "Assets/Images/2d/student2.png";
    const string ScientistPath = "Assets/Images/scientist.png";
    const string GreenMicrobePath = "Assets/Images/UI Flat/char_green_microbe.png";
    const string PurpleMicrobePath = "Assets/Images/UI Flat/char_purple_microbe.png";
    const string WaterMascotPath = "Assets/Images/UI Flat/char_water_drop.png";
    const string NutritionPath = "Assets/Images/UI Flat/icon_fun_nutrition.png";
    const string GrowthPath = "Assets/Images/UI Flat/icon_bertumbuh.png";
    const string LightbulbPath = "Assets/Images/UI Flat/icon_lightbulb.png";
    const string ArrowPath = "Assets/Images/UI Flat/icon_arrow_left.png";
    const string WhitePath = "Assets/Images/UI Flat/btn_white_flat.png";
    const string CirclePath = "Assets/Images/UI Flat/ui_circle_white.png";
    const string FontPath = "Assets/Fonts/FredokaOne SDF.asset";

    static readonly Color Ink = new Color32(9, 70, 30, 255);
    static readonly Color Dark = new Color32(47, 35, 25, 255);
    static readonly Color Green = new Color32(80, 180, 20, 255);
    static readonly Color Blue = new Color32(37, 145, 226, 255);
    static readonly Color Purple = new Color32(137, 68, 210, 255);
    static readonly Color Pink = new Color32(238, 73, 135, 255);
    static readonly Color Orange = new Color32(245, 137, 31, 255);
    static readonly Color Cream = new Color32(255, 250, 229, 255);
    static TMP_FontAsset font;

    [MenuItem("Tools/MicroFun/Activity 5/Build Kehadiran Nutrien Theme")]
    public static void Build()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogError("[Akt5KehadiranNutrien] Exit Play Mode first.");
            return;
        }

        font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        if (font == null) throw new InvalidOperationException("Fredoka One font is missing.");

        BackupScene();
        ConfigureGeneratedSprites();
        Scene scene = SceneManager.GetActiveScene().path == ScenePath
            ? SceneManager.GetActiveScene()
            : EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

        GameObject canvasObject = GameObject.Find("Canvas");
        if (canvasObject == null) throw new InvalidOperationException("Canvas not found.");
        Transform canvas = canvasObject.transform;
        Transform page = canvas.Find("Kehadiran Nutrien");
        Transform faktor = canvas.Find("Faktor");
        if (page == null || faktor == null)
            throw new InvalidOperationException("Canvas/Kehadiran Nutrien or Canvas/Faktor not found.");

        ConfigureCanvas(canvasObject.GetComponent<Canvas>());
        BuildPage(page, faktor);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[Akt5KehadiranNutrien] Nutrient-presence lesson built successfully.");
    }

    static void BackupScene()
    {
        const string parent = "Assets/Scenes/Aktivity/akt5";
        const string folder = parent + "/Backups";
        if (!AssetDatabase.IsValidFolder(folder)) AssetDatabase.CreateFolder(parent, "Backups");
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(BackupPath) == null)
            AssetDatabase.CopyAsset(ScenePath, BackupPath);
    }

    static void ConfigureGeneratedSprites()
    {
        foreach (string path in new[] { WaterPath, MilkPath, YeastSugarPath })
        {
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) throw new InvalidOperationException("Cannot import generated sprite: " + path);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.maxTextureSize = 2048;
            importer.textureCompression = TextureImporterCompression.Compressed;
            importer.SaveAndReimport();
        }
    }

    static void ConfigureCanvas(Canvas canvas)
    {
        if (canvas == null) throw new InvalidOperationException("Canvas component not found.");
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>() ?? canvas.gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = .5f;
    }

    static void BuildPage(Transform page, Transform faktor)
    {
        Delete(page, "Akt5KehadiranNutrienThemeRoot");
        RectTransform root = NewRect("Akt5KehadiranNutrienThemeRoot", page);
        Full(root);

        foreach (Transform child in page)
            if (child != root) child.gameObject.SetActive(false);

        Image background = NewImage("Background", root, BackgroundPath, Color.white, Vector2.zero,
            new Vector2(1920, 1080), false);
        background.preserveAspect = false;
        Full(background.rectTransform);

        BuildDecorations(root);
        BuildHeader(root);
        BuildDialogueColumn(root);
        BuildComparison(root);
        BuildConceptSummary(root);
        BuildBottomCards(root);
        BuildBackButton(root, page, faktor);

        root.SetAsLastSibling();
        page.gameObject.SetActive(false);
        faktor.gameObject.SetActive(true);
    }

    static void BuildDecorations(Transform root)
    {
        AddDecoration("GreenMicrobe", root, GreenMicrobePath, new Vector2(-825, 355), new Vector2(150, 150));
        AddDecoration("PurpleMicrobe", root, PurpleMicrobePath, new Vector2(835, 365), new Vector2(145, 145));
        AddDecoration("WaterMascot", root, WaterMascotPath, new Vector2(770, -425), new Vector2(118, 150));
        Image nutrition = NewImage("NutritionDecor", root, NutritionPath, Color.white,
            new Vector2(-820, -420), new Vector2(118, 118), false);
        nutrition.preserveAspect = true;
    }

    static void AddDecoration(string name, Transform root, string path, Vector2 position, Vector2 size)
    {
        Image image = NewImage(name, root, path, Color.white, position, size, false);
        image.preserveAspect = true;
    }

    static void BuildHeader(Transform root)
    {
        Image badge = Panel("ActivityBadge", root, new Vector2(0, 500), new Vector2(340, 62),
            new Color32(218, 245, 164, 255), Green, 3);
        NewText("Label", badge.transform, "AKTIVITI 5", 38, Ink, TextAlignmentOptions.Center,
            Vector2.zero, new Vector2(315, 54));

        Image title = Panel("TitleBanner", root, new Vector2(80, 420), new Vector2(940, 92),
            Purple, Darker(Purple, .3f), 4);
        NewText("Label", title.transform, "KEHADIRAN NUTRIEN", 53, Color.white,
            TextAlignmentOptions.Center, Vector2.zero, new Vector2(900, 80));

        Image question = Panel("MainQuestion", root, new Vector2(80, 328), new Vector2(1040, 70),
            new Color(1, .96f, .84f, .97f), Orange, 3);
        NewText("Text", question.transform, "Mengapa mikroorganisma memerlukan nutrien?", 35, Dark,
            TextAlignmentOptions.Center, Vector2.zero, new Vector2(1000, 60));
    }

    static void BuildDialogueColumn(Transform root)
    {
        DialogueCard(root, "PupilCard", "Murid", "Mengapakah kita perlu memasukkan gula semasa eksperimen yis?",
            StudentOnePath, new Vector2(-700, 195), Blue);
        DialogueCard(root, "FriendCard", "Rakan", "Yis memerlukan nutrien untuk tumbuh dan menjadi aktif.",
            StudentTwoPath, new Vector2(-700, 18), Green);
        DialogueCard(root, "TeacherCard", "Guru", "Bandingkan pertumbuhan mikroorganisma dalam air suling dan susu segar.",
            ScientistPath, new Vector2(-700, -159), Orange);
    }

    static void DialogueCard(Transform root, string name, string role, string body, string portraitPath,
        Vector2 position, Color accent)
    {
        Image card = Panel(name, root, position, new Vector2(455, 150),
            new Color(1, .99f, .94f, .97f), accent, 3);
        Image portraitCircle = NewImage("PortraitCircle", card.transform, CirclePath, new Color(1, 1, 1, .96f),
            new Vector2(-196, 2), new Vector2(108, 108), false);
        portraitCircle.preserveAspect = true;
        OutlineGraphic(portraitCircle.gameObject, accent, 3);
        Image portrait = NewImage("Portrait", portraitCircle.transform, portraitPath, Color.white,
            new Vector2(0, -2), new Vector2(92, 105), false);
        portrait.preserveAspect = true;

        Image roleChip = Panel("Role", card.transform, new Vector2(-92, 52), new Vector2(145, 42),
            accent, Darker(accent, .28f), 2);
        NewText("Text", roleChip.transform, role, 22, Color.white, TextAlignmentOptions.Center,
            Vector2.zero, new Vector2(130, 34));
        NewText("Body", card.transform, body, 22, Dark, TextAlignmentOptions.MidlineLeft,
            new Vector2(57, -16), new Vector2(305, 94));
    }

    static void BuildComparison(Transform root)
    {
        Image panel = Panel("ComparisonPanel", root, new Vector2(85, 20), new Vector2(875, 500),
            new Color(1, .99f, .93f, .95f), Orange, 3);

        ComparisonColumn(panel.transform, "DistilledWaterCard", "AIR SULING", WaterPath,
            "air suling", new Vector2(-285, 0), Blue, new Vector2(240, 245));
        ComparisonColumn(panel.transform, "FreshMilkCard", "SUSU SEGAR", MilkPath,
            "susu segar", new Vector2(0, 0), Green, new Vector2(230, 220));

        Image vs = NewImage("VersusBadge", panel.transform, CirclePath, new Color(1, .98f, .9f, 1),
            new Vector2(-143, 10), new Vector2(70, 70), false);
        vs.preserveAspect = true;
        OutlineGraphic(vs.gameObject, Blue, 3);
        NewText("Text", vs.transform, "VS", 31, new Color32(42, 77, 165, 255),
            TextAlignmentOptions.Center, Vector2.zero, new Vector2(60, 54));

        BuildYeastInset(panel.transform);
    }

    static void ComparisonColumn(Transform parent, string name, string title, string imagePath,
        string caption, Vector2 position, Color accent, Vector2 imageSize)
    {
        Image card = Panel(name, parent, position, new Vector2(255, 430),
            new Color(1, 1, 1, .83f), accent, 2);
        Image titleChip = Panel("Title", card.transform, new Vector2(0, 185), new Vector2(225, 52),
            accent, Darker(accent, .25f), 2);
        NewText("Text", titleChip.transform, title, 25, Color.white, TextAlignmentOptions.Center,
            Vector2.zero, new Vector2(205, 44));
        Image art = NewImage("Illustration", card.transform, imagePath, Color.white,
            new Vector2(0, 22), imageSize, false);
        art.preserveAspect = true;
        NewText("Caption", card.transform, caption, 27, Dark, TextAlignmentOptions.Center,
            new Vector2(0, -176), new Vector2(220, 44));
    }

    static void BuildYeastInset(Transform parent)
    {
        Image inset = Panel("YeastSugarInset", parent, new Vector2(285, 0), new Vector2(250, 430),
            new Color(1, .98f, .9f, .95f), Orange, 2);
        Image title = Panel("Title", inset.transform, new Vector2(0, 185), new Vector2(220, 52),
            Orange, Darker(Orange, .27f), 2);
        NewText("Text", title.transform, "YIS + GULA", 25, Color.white, TextAlignmentOptions.Center,
            Vector2.zero, new Vector2(200, 44));
        Image art = NewImage("Illustration", inset.transform, YeastSugarPath, Color.white,
            new Vector2(0, 40), new Vector2(225, 185), false);
        art.preserveAspect = true;
        TextMeshProUGUI caption = NewText("Caption", inset.transform,
            "Sumber <color=#18823B>nutrien</color> menjadikan yis aktif dan tumbuh.",
            21, Dark, TextAlignmentOptions.Center, new Vector2(0, -132), new Vector2(220, 118));
        caption.richText = true;
    }

    static void BuildConceptSummary(Transform root)
    {
        ConceptChip(root, "NutrientChip", "Nutrien", NutritionPath, new Vector2(750, 150), Green);
        ConceptChip(root, "ActiveYeastChip", "Yis aktif", null, new Vector2(750, 10), Blue);
        ConceptChip(root, "GrowthChip", "Pertumbuhan\nmeningkat", GrowthPath, new Vector2(750, -130), Purple);
    }

    static void ConceptChip(Transform root, string name, string label, string iconPath, Vector2 position, Color accent)
    {
        Image chip = Panel(name, root, position, new Vector2(315, 112), accent, Darker(accent, .28f), 3);
        Image iconCircle = NewImage("IconCircle", chip.transform, CirclePath, new Color(1, 1, 1, .94f),
            new Vector2(-112, 0), new Vector2(88, 88), false);
        iconCircle.preserveAspect = true;
        OutlineGraphic(iconCircle.gameObject, Color.white, 2);
        if (!string.IsNullOrEmpty(iconPath))
        {
            Image icon = NewImage("Icon", iconCircle.transform, iconPath, Color.white,
                Vector2.zero, new Vector2(68, 68), false);
            icon.preserveAspect = true;
        }
        else
        {
            NewText("IconText", iconCircle.transform, "Y", 38, accent, TextAlignmentOptions.Center,
                Vector2.zero, new Vector2(68, 60));
        }

        NewText("Label", chip.transform, label, 27, Color.white, TextAlignmentOptions.Center,
            new Vector2(38, 0), new Vector2(205, 90));
    }

    static void BuildBottomCards(Transform root)
    {
        Image idea = Panel("MainIdeaCard", root, new Vector2(45, -356), new Vector2(1090, 82),
            new Color(1, .98f, .86f, .98f), Orange, 3);
        Image ideaChip = Panel("Label", idea.transform, new Vector2(-430, 0), new Vector2(210, 58),
            new Color32(46, 111, 204, 255), new Color32(24, 70, 150, 255), 2);
        Image bulb = NewImage("Bulb", ideaChip.transform, LightbulbPath, Color.white,
            new Vector2(-78, 0), new Vector2(48, 48), false);
        bulb.preserveAspect = true;
        NewText("Text", ideaChip.transform, "IDEA UTAMA", 23, Color.white, TextAlignmentOptions.Center,
            new Vector2(20, 0), new Vector2(150, 46));
        TextMeshProUGUI ideaText = NewText("Text", idea.transform,
            "Mikroorganisma memerlukan nutrien sebagai sumber makanan untuk tumbuh, membiak dan menjadi aktif.",
            23, Dark, TextAlignmentOptions.Center, new Vector2(120, 0), new Vector2(790, 66));
        ideaText.richText = true;

        Image trigger = Panel("TriggerQuestionCard", root, new Vector2(45, -456), new Vector2(1090, 88),
            new Color(1, .96f, .91f, .98f), Pink, 3);
        Image triggerChip = Panel("Label", trigger.transform, new Vector2(-420, 0), new Vector2(230, 64),
            Pink, Darker(Pink, .27f), 2);
        NewText("Mark", triggerChip.transform, "?", 42, Color.white, TextAlignmentOptions.Center,
            new Vector2(-86, 0), new Vector2(48, 52));
        NewText("Text", triggerChip.transform, "SOALAN\nPENCETUS", 20, Color.white,
            TextAlignmentOptions.Center, new Vector2(22, 0), new Vector2(158, 56));
        TextMeshProUGUI triggerText = NewText("Text", trigger.transform,
            "Medium manakah lebih sesuai untuk pertumbuhan mikroorganisma: <color=#257DD0>air suling</color> atau <color=#278B35>susu segar</color>? Mengapa?",
            22, Dark, TextAlignmentOptions.Center, new Vector2(125, 0), new Vector2(790, 72));
        triggerText.richText = true;
    }

    static void BuildBackButton(Transform root, Transform page, Transform faktor)
    {
        GameObject go = new GameObject("BackButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        go.transform.SetParent(root, false);
        SetRect(go.transform, new Vector2(-842, 495), new Vector2(205, 62));
        Image image = go.GetComponent<Image>();
        image.sprite = LoadSprite(WhitePath);
        image.type = Image.Type.Sliced;
        image.color = Cream;
        OutlineGraphic(go, Ink, 2);
        DropShadow(go);

        Button button = go.GetComponent<Button>();
        button.targetGraphic = image;
        SetButtonColors(button, Cream, Color.white, Darker(Cream, .08f), Color.gray);
        if (go.GetComponent<ButtonInteractionFeedback>() == null) go.AddComponent<ButtonInteractionFeedback>();
        UnityEventTools.AddBoolPersistentListener(button.onClick, page.gameObject.SetActive, false);
        UnityEventTools.AddBoolPersistentListener(button.onClick, faktor.gameObject.SetActive, true);

        Image circle = NewImage("Circle", go.transform, CirclePath, Green, new Vector2(-70, 0), new Vector2(54, 54), false);
        circle.preserveAspect = true;
        OutlineGraphic(circle.gameObject, Color.white, 2);
        Image arrow = NewImage("Arrow", circle.transform, ArrowPath, Color.white, Vector2.zero, new Vector2(29, 29), false);
        arrow.preserveAspect = true;
        NewText("Label", go.transform, "FAKTOR", 22, Ink, TextAlignmentOptions.Center,
            new Vector2(28, 0), new Vector2(118, 48));
    }

    static Image Panel(string name, Transform parent, Vector2 position, Vector2 size, Color fill, Color border, float width)
    {
        Image image = NewImage(name, parent, WhitePath, fill, position, size, false);
        image.type = Image.Type.Sliced;
        OutlineGraphic(image.gameObject, border, width);
        DropShadow(image.gameObject);
        return image;
    }

    static Image NewImage(string name, Transform parent, string path, Color color, Vector2 position, Vector2 size, bool raycast)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        SetRect(go.transform, position, size);
        Image image = go.GetComponent<Image>();
        image.sprite = LoadSprite(path);
        image.color = color;
        image.raycastTarget = raycast;
        return image;
    }

    static Sprite LoadSprite(string path)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (sprite == null) Debug.LogWarning("[Akt5KehadiranNutrien] Sprite missing: " + path);
        return sprite;
    }

    static TextMeshProUGUI NewText(string name, Transform parent, string value, float size, Color color,
        TextAlignmentOptions alignment, Vector2 position, Vector2 rectSize)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        SetRect(go.transform, position, rectSize);
        TextMeshProUGUI text = go.GetComponent<TextMeshProUGUI>();
        text.font = font;
        text.text = value;
        text.fontSize = size;
        text.color = color;
        text.alignment = alignment;
        text.textWrappingMode = TextWrappingModes.Normal;
        text.overflowMode = TextOverflowModes.Ellipsis;
        text.raycastTarget = false;
        return text;
    }

    static RectTransform NewRect(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go.GetComponent<RectTransform>();
    }

    static void Delete(Transform parent, string name)
    {
        Transform found = parent.Find(name);
        if (found != null) UnityEngine.Object.DestroyImmediate(found.gameObject);
    }

    static void SetRect(Transform transform, Vector2 position, Vector2 size)
    {
        RectTransform rect = transform as RectTransform;
        if (rect == null) return;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;
    }

    static void Full(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = Vector2.zero;
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;
    }

    static void OutlineGraphic(GameObject go, Color color, float width)
    {
        Outline outline = go.GetComponent<Outline>() ?? go.AddComponent<Outline>();
        outline.effectColor = color;
        outline.effectDistance = new Vector2(width, -width);
        outline.useGraphicAlpha = true;
    }

    static void DropShadow(GameObject go)
    {
        Shadow shadow = go.GetComponents<Shadow>().FirstOrDefault(component => !(component is Outline));
        if (shadow == null) shadow = go.AddComponent<Shadow>();
        shadow.effectColor = new Color(.15f, .08f, 0, .25f);
        shadow.effectDistance = new Vector2(0, -6);
        shadow.useGraphicAlpha = true;
    }

    static void SetButtonColors(Button button, Color normal, Color highlighted, Color pressed, Color disabled)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = normal;
        colors.highlightedColor = highlighted;
        colors.selectedColor = highlighted;
        colors.pressedColor = pressed;
        colors.disabledColor = disabled;
        colors.colorMultiplier = 1;
        colors.fadeDuration = .12f;
        button.colors = colors;
        button.transition = Selectable.Transition.ColorTint;
    }

    static Color Darker(Color color, float amount)
    {
        return new Color(color.r * (1 - amount), color.g * (1 - amount), color.b * (1 - amount), color.a);
    }
}
