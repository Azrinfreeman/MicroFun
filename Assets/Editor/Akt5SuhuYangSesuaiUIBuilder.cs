using System;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Akt5SuhuYangSesuaiUIBuilder
{
    const string ScenePath = "Assets/Scenes/Aktivity/akt5/akt5.unity";
    const string BackupPath = "Assets/Scenes/Aktivity/akt5/Backups/akt5_PreSuhuYangSesuaiRedesign_20260904.unity";
    const string BackgroundPath = "Assets/Images/Akt5CenterUI/Background/akt5_factor_background_v2.png";
    const string ComparisonPath = "Assets/Images/Akt5SuhuUI/akt5_suhu_soup_temperature_comparison_v1.png";
    const string StudentOnePath = "Assets/Images/2d/student1.png";
    const string StudentTwoPath = "Assets/Images/2d/student2.png";
    const string ScientistPath = "Assets/Images/scientist.png";
    const string GreenMicrobePath = "Assets/Images/UI Flat/char_green_microbe.png";
    const string PurpleMicrobePath = "Assets/Images/UI Flat/char_purple_microbe.png";
    const string WaterMascotPath = "Assets/Images/UI Flat/char_water_drop.png";
    const string MicroscopePath = "Assets/Images/UI Flat/img_microscope_cartoon.png";
    const string LightbulbPath = "Assets/Images/UI Flat/icon_lightbulb.png";
    const string FlaskPath = "Assets/Images/UI Flat/icon_flask.png";
    const string TickPath = "Assets/Images/UI Flat 1/tick.png";
    const string ArrowPath = "Assets/Images/UI Flat/icon_arrow_left.png";
    const string WhitePath = "Assets/Images/UI Flat/btn_white_flat.png";
    const string CirclePath = "Assets/Images/UI Flat/ui_circle_white.png";
    const string FontPath = "Assets/Fonts/FredokaOne SDF.asset";

    static readonly Color Ink = new Color32(8, 68, 30, 255);
    static readonly Color Dark = new Color32(49, 35, 24, 255);
    static readonly Color Green = new Color32(71, 174, 29, 255);
    static readonly Color Blue = new Color32(38, 143, 224, 255);
    static readonly Color CoolBlue = new Color32(55, 158, 232, 255);
    static readonly Color WarmOrange = new Color32(245, 126, 33, 255);
    static readonly Color Red = new Color32(232, 67, 52, 255);
    static readonly Color Pink = new Color32(238, 77, 132, 255);
    static readonly Color Purple = new Color32(138, 67, 210, 255);
    static readonly Color Cream = new Color32(255, 249, 226, 255);
    static TMP_FontAsset font;

    [MenuItem("Tools/MicroFun/Activity 5/Build Suhu Yang Sesuai Theme")]
    public static void Build()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogError("[Akt5Suhu] Exit Play Mode first.");
            return;
        }

        font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        if (font == null) throw new InvalidOperationException("Fredoka One font is missing.");

        BackupScene();
        ConfigureComparisonSprite();

        Scene scene = SceneManager.GetActiveScene().path == ScenePath
            ? SceneManager.GetActiveScene()
            : EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

        GameObject canvasObject = GameObject.Find("Canvas");
        if (canvasObject == null) throw new InvalidOperationException("Canvas not found.");

        Transform canvas = canvasObject.transform;
        Transform page = canvas.Find("Suhu Yang Sesuai");
        Transform faktor = canvas.Find("Faktor");
        if (page == null || faktor == null)
            throw new InvalidOperationException("Canvas/Suhu Yang Sesuai or Canvas/Faktor not found.");

        ConfigureCanvas(canvasObject.GetComponent<Canvas>());
        BuildPage(page, faktor);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[Akt5Suhu] Temperature lesson built successfully.");
    }

    static void BackupScene()
    {
        const string parent = "Assets/Scenes/Aktivity/akt5";
        const string folder = parent + "/Backups";
        if (!AssetDatabase.IsValidFolder(folder)) AssetDatabase.CreateFolder(parent, "Backups");
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(BackupPath) == null)
            AssetDatabase.CopyAsset(ScenePath, BackupPath);
    }

    static void ConfigureComparisonSprite()
    {
        AssetDatabase.ImportAsset(ComparisonPath, ImportAssetOptions.ForceUpdate);
        TextureImporter importer = AssetImporter.GetAtPath(ComparisonPath) as TextureImporter;
        if (importer == null) throw new InvalidOperationException("Temperature comparison image is missing.");
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.maxTextureSize = 2048;
        importer.textureCompression = TextureImporterCompression.CompressedHQ;
        importer.SaveAndReimport();
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
        Delete(page, "Akt5SuhuYangSesuaiThemeRoot");
        RectTransform root = NewRect("Akt5SuhuYangSesuaiThemeRoot", page);
        Full(root);

        foreach (Transform child in page)
            if (child != root) child.gameObject.SetActive(false);

        Image background = NewImage("Background", root, BackgroundPath, Color.white,
            Vector2.zero, new Vector2(1920, 1080), false);
        background.preserveAspect = false;
        Full(background.rectTransform);

        BuildDecorations(root);
        BuildHeader(root);
        BuildDialogueColumn(root);
        BuildComparison(root);
        BuildBottomLearning(root);
        BuildBackButton(root, page, faktor);

        root.SetAsLastSibling();
        page.gameObject.SetActive(false);
        faktor.gameObject.SetActive(true);
    }

static void BuildDecorations(Transform root)
    {
        AddDecoration("GreenMicrobe", root, GreenMicrobePath, new Vector2(-855, 360), new Vector2(130, 130));
        AddDecoration("PurpleMicrobe", root, PurpleMicrobePath, new Vector2(850, 390), new Vector2(125, 125));
        AddDecoration("WaterMascot", root, WaterMascotPath, new Vector2(845, -420), new Vector2(105, 135));
        AddDecoration("Microscope", root, MicroscopePath, new Vector2(-860, -415), new Vector2(125, 160));
        BuildSpark(root, "CoolSpark", new Vector2(-530, 478), Blue);
        BuildSpark(root, "WarmSpark", new Vector2(540, 478), WarmOrange);
    }

    static void AddDecoration(string name, Transform root, string path, Vector2 position, Vector2 size)
    {
        Image image = NewImage(name, root, path, Color.white, position, size, false);
        image.preserveAspect = true;
    }

    static void BuildHeader(Transform root)
    {
        Image badge = Panel("ActivityBadge", root, new Vector2(0, 505), new Vector2(345, 62),
            new Color32(218, 245, 164, 255), Green, 3);
        NewText("Label", badge.transform, "AKTIVITI 5", 38, Ink, TextAlignmentOptions.Center,
            Vector2.zero, new Vector2(320, 54));

        Image title = Panel("TitleBanner", root, new Vector2(85, 425), new Vector2(1050, 94),
            Red, Darker(Red, .3f), 4);
        BuildThermometerIcon(title.transform, new Vector2(-458, 0), new Vector2(72, 72));
        TextMeshProUGUI titleText = NewText("Label", title.transform, "SUHU YANG SESUAI", 54,
            Color.white, TextAlignmentOptions.Center, new Vector2(45, 0), new Vector2(880, 80));
        TextShadow(titleText, new Color(0, 0, 0, .27f), new Vector2(3, -4));

        Image question = Panel("MainQuestion", root, new Vector2(85, 330), new Vector2(1340, 72),
            new Color(1, .97f, .86f, .98f), WarmOrange, 3);
        NewText("Text", question.transform,
            "Bagaimanakah suhu mempengaruhi pertumbuhan mikroorganisma?", 31, Dark,
            TextAlignmentOptions.Center, Vector2.zero, new Vector2(1290, 60));
    }

    static void BuildDialogueColumn(Transform root)
    {
        DialogueCard(root, "PupilCard", "Murid",
            "Mengapakah sup yang dibiarkan pada suhu bilik lebih cepat basi?",
            StudentOnePath, new Vector2(-706, 174), Blue);
        DialogueCard(root, "FriendCard", "Rakan",
            "Pada suhu yang sesuai, banyak mikroorganisma membiak dengan lebih cepat.",
            StudentTwoPath, new Vector2(-706, 18), Green);
        DialogueCard(root, "TeacherCard", "Guru",
            "Bandingkan sup di dalam peti sejuk dengan sup pada suhu bilik.",
            ScientistPath, new Vector2(-706, -138), WarmOrange);
    }

    static void DialogueCard(Transform root, string name, string role, string body,
        string portraitPath, Vector2 position, Color accent)
    {
        Image card = Panel(name, root, position, new Vector2(442, 136),
            new Color(1, .99f, .94f, .97f), accent, 3);

        Image portraitCircle = NewImage("PortraitCircle", card.transform, CirclePath,
            new Color(1, 1, 1, .97f), new Vector2(-192, 0), new Vector2(100, 100), false);
        portraitCircle.preserveAspect = true;
        OutlineGraphic(portraitCircle.gameObject, accent, 3);

        Image portrait = NewImage("Portrait", portraitCircle.transform, portraitPath, Color.white,
            new Vector2(0, -2), new Vector2(84, 96), false);
        portrait.preserveAspect = true;

        Image roleChip = Panel("Role", card.transform, new Vector2(-82, 46), new Vector2(142, 39),
            accent, Darker(accent, .28f), 2);
        NewText("Text", roleChip.transform, role, 21, Color.white, TextAlignmentOptions.Center,
            Vector2.zero, new Vector2(130, 32));

        NewText("Body", card.transform, body, 19, Dark, TextAlignmentOptions.MidlineLeft,
            new Vector2(56, -13), new Vector2(298, 86));
    }

    static void BuildComparison(Transform root)
    {
        Image panel = Panel("TemperatureComparison", root, new Vector2(238, 55), new Vector2(1018, 485),
            new Color(1, .99f, .93f, .95f), WarmOrange, 3);

        Image tab = Panel("ExperimentTab", panel.transform, new Vector2(-310, 226), new Vector2(345, 62),
            Pink, Darker(Pink, .25f), 2);
        Image flask = NewImage("Flask", tab.transform, FlaskPath, Color.white,
            new Vector2(-132, 0), new Vector2(45, 45), false);
        flask.preserveAspect = true;
        NewText("Text", tab.transform, "Eksperimen Mudah", 27, Color.white,
            TextAlignmentOptions.Center, new Vector2(20, 0), new Vector2(270, 48));

        NewText("Instruction", panel.transform,
            "Bandingkan sup pada suhu rendah dengan sup pada suhu bilik untuk tempoh yang sama.",
            20, Dark, TextAlignmentOptions.Center, new Vector2(0, 184), new Vector2(930, 46));

        TemperatureLabel(panel.transform, "ColdLabel", "DALAM PETI SEJUK  ≤4°C",
            new Vector2(-265, 137), CoolBlue);
        TemperatureLabel(panel.transform, "RoomLabel", "SUHU BILIK",
            new Vector2(265, 137), WarmOrange);

        Image art = NewImage("MatchedSoupComparison", panel.transform, ComparisonPath, Color.white,
            new Vector2(0, 7), new Vector2(875, 315), false);
        art.preserveAspect = true;

        Image divider = NewImage("Divider", panel.transform, WhitePath, new Color(1, 1, 1, .9f),
            new Vector2(0, -2), new Vector2(4, 276), false);
        divider.type = Image.Type.Sliced;

        Image versus = NewImage("VersusBadge", panel.transform, CirclePath, Color.white,
            new Vector2(0, 0), new Vector2(66, 66), false);
        versus.preserveAspect = true;
        OutlineGraphic(versus.gameObject, Blue, 3);
        NewText("Text", versus.transform, "VS", 25, Ink, TextAlignmentOptions.Center,
            Vector2.zero, new Vector2(56, 46));

        CaptionCard(panel.transform, "ColdCaption",
            "Suhu rendah memperlahankan pertumbuhan mikroorganisma.",
            new Vector2(-255, -157), CoolBlue, true);
        CaptionCard(panel.transform, "RoomCaption",
            "Pada suhu sesuai, mikroorganisma boleh membiak lebih cepat.",
            new Vector2(255, -157), WarmOrange, false);

        Image safety = Panel("SafetyNote", panel.transform, new Vector2(0, -226), new Vector2(650, 43),
            new Color32(255, 247, 211, 250), Red, 2);
        NewText("Text", safety.transform, "Pemerhatian sahaja — jangan rasa sampel eksperimen.",
            17, new Color32(133, 39, 32, 255), TextAlignmentOptions.Center,
            Vector2.zero, new Vector2(625, 34));
    }

static void TemperatureLabel(Transform parent, string name, string value, Vector2 position, Color accent)
    {
        Image label = Panel(name, parent, position, new Vector2(390, 56),
            accent, Darker(accent, .25f), 2);
        if (name == "ColdLabel")
            BuildSnowflake(label.transform, new Vector2(-160, 0), 34, Color.white);
        else
            BuildThermometerIcon(label.transform, new Vector2(-160, 0), new Vector2(40, 43));

        NewText("Text", label.transform, value, 23, Color.white, TextAlignmentOptions.Center,
            new Vector2(18, 0), new Vector2(330, 45));
    }

static void CaptionCard(Transform parent, string name, string value, Vector2 position,
        Color accent, bool positive)
    {
        Image card = Panel(name, parent, position, new Vector2(455, 76),
            new Color(1, 1, 1, .94f), accent, 2);

        Image mark = NewImage("Mark", card.transform, CirclePath,
            positive ? Green : Red, new Vector2(-190, 0), new Vector2(52, 52), false);
        mark.preserveAspect = true;
        OutlineGraphic(mark.gameObject, Color.white, 2);
        if (positive)
        {
            Image tick = NewImage("Tick", mark.transform, TickPath, Color.white,
                Vector2.zero, new Vector2(33, 33), false);
            tick.preserveAspect = true;
        }
        else
        {
            NewText("Symbol", mark.transform, "!", 31, Color.white,
                TextAlignmentOptions.Center, Vector2.zero, new Vector2(43, 40));
        }

        NewText("Text", card.transform, value, 17, Dark, TextAlignmentOptions.Center,
            new Vector2(27, 0), new Vector2(375, 62));
    }

    static void BuildBottomLearning(Transform root)
    {
        Image sequence = Panel("ConceptSummary", root, new Vector2(140, -250), new Vector2(1320, 68),
            new Color(1, .98f, .88f, .98f), Green, 2);
        NewText("ColdFlow", sequence.transform,
            "<color=#278FE0>Suhu rendah</color>  →  Pertumbuhan perlahan  →  Makanan tahan lebih lama",
            21, Ink, TextAlignmentOptions.Center, new Vector2(-318, 0), new Vector2(610, 54));
        NewText("WarmFlow", sequence.transform,
            "<color=#E84334>Suhu sesuai</color>  →  Pembiakan lebih cepat  →  Makanan cepat rosak",
            21, Ink, TextAlignmentOptions.Center, new Vector2(318, 0), new Vector2(610, 54));

        Image idea = Panel("MainIdeaCard", root, new Vector2(140, -355), new Vector2(1320, 90),
            new Color(1, .98f, .86f, .98f), WarmOrange, 3);
        Image ideaChip = Panel("Label", idea.transform, new Vector2(-520, 0), new Vector2(232, 64),
            Purple, Darker(Purple, .27f), 2);
        Image bulb = NewImage("Bulb", ideaChip.transform, LightbulbPath, Color.white,
            new Vector2(-88, 0), new Vector2(50, 50), false);
        bulb.preserveAspect = true;
        NewText("Text", ideaChip.transform, "IDEA UTAMA", 22, Color.white,
            TextAlignmentOptions.Center, new Vector2(20, 0), new Vector2(165, 48));
        TextMeshProUGUI ideaText = NewText("Text", idea.transform,
            "Suhu mempengaruhi kadar pertumbuhan mikroorganisma. Suhu rendah biasanya memperlahankan pertumbuhan, manakala suhu yang sesuai membolehkan banyak mikroorganisma membiak dengan lebih cepat.",
            19, Dark, TextAlignmentOptions.Center, new Vector2(130, 0), new Vector2(990, 74));
        ideaText.richText = true;

        Image trigger = Panel("TriggerQuestionCard", root, new Vector2(140, -465), new Vector2(1320, 84),
            new Color(1, .97f, .91f, .98f), Blue, 3);
        Image triggerChip = Panel("Label", trigger.transform, new Vector2(-500, 0), new Vector2(270, 61),
            new Color32(45, 103, 195, 255), new Color32(24, 67, 143, 255), 2);
        NewText("Mark", triggerChip.transform, "?", 39, Color.white, TextAlignmentOptions.Center,
            new Vector2(-103, 0), new Vector2(52, 49));
        NewText("Text", triggerChip.transform, "SOALAN PENCETUS", 19, Color.white,
            TextAlignmentOptions.Center, new Vector2(18, 0), new Vector2(195, 48));
        NewText("Text", trigger.transform,
            "Mengapakah sup yang disimpan di dalam peti sejuk lebih lambat basi?",
            24, Dark, TextAlignmentOptions.Center, new Vector2(160, 0), new Vector2(900, 64));
    }

    static void BuildThermometerIcon(Transform parent, Vector2 position, Vector2 size)
    {
        RectTransform holder = NewRect("ThermometerIcon", parent);
        SetRect(holder, position, size);

        Image stem = NewImage("Stem", holder, WhitePath, Color.white,
            new Vector2(0, 6), new Vector2(size.x * .34f, size.y * .66f), false);
        stem.type = Image.Type.Sliced;
        OutlineGraphic(stem.gameObject, new Color32(170, 48, 43, 255), 2);

        Image mercury = NewImage("Mercury", holder, WhitePath, Red,
            new Vector2(0, 1), new Vector2(size.x * .12f, size.y * .48f), false);
        mercury.type = Image.Type.Sliced;

        Image bulb = NewImage("Bulb", holder, CirclePath, Red,
            new Vector2(0, -size.y * .27f), new Vector2(size.x * .38f, size.x * .38f), false);
        bulb.preserveAspect = true;
        OutlineGraphic(bulb.gameObject, Color.white, 2);
    }

    static void BuildBackButton(Transform root, Transform page, Transform faktor)
    {
        GameObject go = new GameObject("BackButton", typeof(RectTransform), typeof(CanvasRenderer),
            typeof(Image), typeof(Button));
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
        if (go.GetComponent<ButtonInteractionFeedback>() == null)
            go.AddComponent<ButtonInteractionFeedback>();
        UnityEventTools.AddBoolPersistentListener(button.onClick, page.gameObject.SetActive, false);
        UnityEventTools.AddBoolPersistentListener(button.onClick, faktor.gameObject.SetActive, true);

        Image circle = NewImage("Circle", go.transform, CirclePath, Green,
            new Vector2(-70, 0), new Vector2(54, 54), false);
        circle.preserveAspect = true;
        OutlineGraphic(circle.gameObject, Color.white, 2);
        Image arrow = NewImage("Arrow", circle.transform, ArrowPath, Color.white,
            Vector2.zero, new Vector2(29, 29), false);
        arrow.preserveAspect = true;
        NewText("Label", go.transform, "FAKTOR", 22, Ink, TextAlignmentOptions.Center,
            new Vector2(28, 0), new Vector2(118, 48));
    }

    static Image Panel(string name, Transform parent, Vector2 position, Vector2 size,
        Color fill, Color border, float width)
    {
        Image image = NewImage(name, parent, WhitePath, fill, position, size, false);
        image.type = Image.Type.Sliced;
        OutlineGraphic(image.gameObject, border, width);
        DropShadow(image.gameObject);
        return image;
    }

    static Image NewImage(string name, Transform parent, string path, Color color,
        Vector2 position, Vector2 size, bool raycast)
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
        if (sprite == null) Debug.LogWarning("[Akt5Suhu] Sprite missing: " + path);
        return sprite;
    }

    static TextMeshProUGUI NewText(string name, Transform parent, string value, float size,
        Color color, TextAlignmentOptions alignment, Vector2 position, Vector2 rectSize)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer),
            typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        SetRect(go.transform, position, rectSize);

        TextMeshProUGUI text = go.GetComponent<TextMeshProUGUI>();
        text.font = font;
        text.text = value;
        text.fontSize = size;
        text.color = color;
        text.alignment = alignment;
        text.enableWordWrapping = true;
        text.overflowMode = TextOverflowModes.Ellipsis;
        text.raycastTarget = false;
        text.richText = true;
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

    static void TextShadow(TextMeshProUGUI text, Color color, Vector2 distance)
    {
        Shadow shadow = text.GetComponents<Shadow>().FirstOrDefault(component => !(component is Outline));
        if (shadow == null) shadow = text.gameObject.AddComponent<Shadow>();
        shadow.effectColor = color;
        shadow.effectDistance = distance;
        shadow.useGraphicAlpha = true;
    }

    static void SetButtonColors(Button button, Color normal, Color highlighted,
        Color pressed, Color disabled)
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
        return new Color(color.r * (1 - amount), color.g * (1 - amount),
            color.b * (1 - amount), color.a);
    }


static void BuildSpark(Transform parent, string name, Vector2 position, Color color)
    {
        RectTransform holder = NewRect(name, parent);
        SetRect(holder, position, new Vector2(48, 48));
        Image vertical = NewImage("Vertical", holder, WhitePath, color,
            Vector2.zero, new Vector2(10, 44), false);
        vertical.type = Image.Type.Sliced;
        Image horizontal = NewImage("Horizontal", holder, WhitePath, color,
            Vector2.zero, new Vector2(44, 10), false);
        horizontal.type = Image.Type.Sliced;
    }

    static void BuildSnowflake(Transform parent, Vector2 position, float size, Color color)
    {
        RectTransform holder = NewRect("Snowflake", parent);
        SetRect(holder, position, new Vector2(size, size));
        for (int i = 0; i < 3; i++)
        {
            Image bar = NewImage("Ray" + i, holder, WhitePath, color,
                Vector2.zero, new Vector2(5, size), false);
            bar.type = Image.Type.Sliced;
            bar.rectTransform.localRotation = Quaternion.Euler(0, 0, i * 60f);
        }
        Image center = NewImage("Center", holder, CirclePath, color,
            Vector2.zero, new Vector2(9, 9), false);
        center.preserveAspect = true;
    }
}
