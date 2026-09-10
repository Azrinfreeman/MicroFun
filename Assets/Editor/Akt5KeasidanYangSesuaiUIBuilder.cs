using System;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Akt5KeasidanYangSesuaiUIBuilder
{
    const string ScenePath = "Assets/Scenes/Aktivity/akt5/akt5.unity";
    const string BackupPath = "Assets/Scenes/Aktivity/akt5/Backups/akt5_PreKeasidanYangSesuaiRedesign_20260903.unity";
    const string BackgroundPath = "Assets/Images/Akt5CenterUI/Background/akt5_factor_background_v2.png";
    const string PickledMangoPath = "Assets/Images/Akt5KeasidanUI/pickled_mango_jar_v1.png";
    const string VinegarApplePath = "Assets/Images/Akt5KeasidanUI/apple_in_vinegar_v1.png";
    const string PlainWaterApplePath = "Assets/Images/Akt5KeasidanUI/apple_in_plain_water_browned_v1.png";
    const string StudentOnePath = "Assets/Images/2d/student1.png";
    const string StudentTwoPath = "Assets/Images/2d/student2.png";
    const string ScientistPath = "Assets/Images/scientist.png";
    const string GreenMicrobePath = "Assets/Images/UI Flat/char_green_microbe.png";
    const string PurpleMicrobePath = "Assets/Images/UI Flat/char_purple_microbe.png";
    const string WaterMascotPath = "Assets/Images/UI Flat/char_water_drop.png";
    const string MicroscopePath = "Assets/Images/UI Flat/img_microscope_cartoon.png";
    const string GrowthPath = "Assets/Images/UI Flat/icon_bertumbuh.png";
    const string LightbulbPath = "Assets/Images/UI Flat/icon_lightbulb.png";
    const string ArrowPath = "Assets/Images/UI Flat/icon_arrow_left.png";
    const string WhitePath = "Assets/Images/UI Flat/btn_white_flat.png";
    const string CirclePath = "Assets/Images/UI Flat/ui_circle_white.png";
    const string FontPath = "Assets/Fonts/FredokaOne SDF.asset";

    static readonly Color Ink = new Color32(10, 69, 29, 255);
    static readonly Color Dark = new Color32(52, 34, 23, 255);
    static readonly Color Green = new Color32(76, 177, 31, 255);
    static readonly Color Blue = new Color32(40, 137, 220, 255);
    static readonly Color Purple = new Color32(137, 69, 211, 255);
    static readonly Color Pink = new Color32(237, 77, 129, 255);
    static readonly Color Orange = new Color32(247, 130, 35, 255);
    static readonly Color Red = new Color32(225, 57, 54, 255);
    static readonly Color Cream = new Color32(255, 250, 229, 255);
    static TMP_FontAsset font;

    [MenuItem("Tools/MicroFun/Activity 5/Build Keasidan Yang Sesuai Theme")]
    public static void Build()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogError("[Akt5Keasidan] Exit Play Mode first.");
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
        Transform page = canvas.Find("Keasidan Yang Sesuai");
        Transform faktor = canvas.Find("Faktor");
        if (page == null || faktor == null)
            throw new InvalidOperationException("Canvas/Keasidan Yang Sesuai or Canvas/Faktor not found.");

        ConfigureCanvas(canvasObject.GetComponent<Canvas>());
        BuildPage(page, faktor);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[Akt5Keasidan] Acidity lesson built successfully.");
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
        foreach (string path in new[] { PickledMangoPath, VinegarApplePath, PlainWaterApplePath })
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
        Delete(page, "Akt5KeasidanYangSesuaiThemeRoot");
        RectTransform root = NewRect("Akt5KeasidanYangSesuaiThemeRoot", page);
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
        BuildConceptSequence(root);
        BuildBottomCards(root);
        BuildBackButton(root, page, faktor);

        root.SetAsLastSibling();
        page.gameObject.SetActive(false);
        faktor.gameObject.SetActive(true);
    }

    static void BuildDecorations(Transform root)
    {
        AddDecoration("GreenMicrobe", root, GreenMicrobePath, new Vector2(-842, 338), new Vector2(132, 132));
        AddDecoration("PurpleMicrobe", root, PurpleMicrobePath, new Vector2(850, 385), new Vector2(130, 130));
        AddDecoration("WaterMascot", root, WaterMascotPath, new Vector2(840, -422), new Vector2(105, 135));
        AddDecoration("Microscope", root, MicroscopePath, new Vector2(-855, -405), new Vector2(125, 160));
    }

    static void AddDecoration(string name, Transform root, string path, Vector2 position, Vector2 size)
    {
        Image image = NewImage(name, root, path, Color.white, position, size, false);
        image.preserveAspect = true;
    }

    static void BuildHeader(Transform root)
    {
        Image badge = Panel("ActivityBadge", root, new Vector2(0, 500), new Vector2(345, 62),
            new Color32(218, 245, 164, 255), Green, 3);
        NewText("Label", badge.transform, "AKTIVITI 5", 38, Ink, TextAlignmentOptions.Center,
            Vector2.zero, new Vector2(320, 54));

        Image title = Panel("TitleBanner", root, new Vector2(90, 420), new Vector2(1040, 92),
            Purple, Darker(Purple, .3f), 4);
        NewText("Label", title.transform, "KEASIDAN YANG SESUAI", 53, Color.white,
            TextAlignmentOptions.Center, Vector2.zero, new Vector2(995, 80));

        Image question = Panel("MainQuestion", root, new Vector2(80, 326), new Vector2(1340, 72),
            new Color(1, .96f, .84f, .97f), Orange, 3);
        NewText("Text", question.transform,
            "Bagaimanakah keasidan mempengaruhi pertumbuhan mikroorganisma?", 31, Dark,
            TextAlignmentOptions.Center, Vector2.zero, new Vector2(1290, 61));
    }

    static void BuildDialogueColumn(Transform root)
    {
        DialogueCard(root, "PupilCard", "Murid",
            "Bagaimanakah mangga jeruk kekal elok untuk jangka masa yang lama?",
            StudentOnePath, new Vector2(-704, 174), Blue);
        DialogueCard(root, "FriendCard", "Rakan",
            "Keasidan air jeruk tidak sesuai untuk pertumbuhan mikroorganisma.",
            StudentTwoPath, new Vector2(-704, 18), Green);
        DialogueCard(root, "TeacherCard", "Guru",
            "Mari bandingkan hirisan buah dalam larutan cuka dan air biasa.",
            ScientistPath, new Vector2(-704, -138), Orange);
    }

    static void DialogueCard(Transform root, string name, string role, string body, string portraitPath,
        Vector2 position, Color accent)
    {
        Image card = Panel(name, root, position, new Vector2(440, 136),
            new Color(1, .99f, .94f, .97f), accent, 3);
        Image portraitCircle = NewImage("PortraitCircle", card.transform, CirclePath,
            new Color(1, 1, 1, .97f), new Vector2(-191, 0), new Vector2(100, 100), false);
        portraitCircle.preserveAspect = true;
        OutlineGraphic(portraitCircle.gameObject, accent, 3);
        Image portrait = NewImage("Portrait", portraitCircle.transform, portraitPath, Color.white,
            new Vector2(0, -2), new Vector2(84, 96), false);
        portrait.preserveAspect = true;

        Image roleChip = Panel("Role", card.transform, new Vector2(-83, 46), new Vector2(142, 39),
            accent, Darker(accent, .28f), 2);
        NewText("Text", roleChip.transform, role, 21, Color.white, TextAlignmentOptions.Center,
            Vector2.zero, new Vector2(130, 32));
        NewText("Body", card.transform, body, 20, Dark, TextAlignmentOptions.MidlineLeft,
            new Vector2(55, -13), new Vector2(296, 85));
    }

    static void BuildComparison(Transform root)
    {
        Image panel = Panel("AcidityComparison", root, new Vector2(235, 47), new Vector2(1035, 470),
            new Color(1, .99f, .92f, .94f), Orange, 3);

        ComparisonCard(panel.transform, "PickledMangoCard", "MANGGA JERUK", PickledMangoPath,
            new Vector2(-344, 0), Green, new Vector2(265, 300), false);
        ComparisonCard(panel.transform, "VinegarAppleCard", "HIRISAN BUAH\nDALAM CUKA", VinegarApplePath,
            new Vector2(0, 0), Blue, new Vector2(270, 286), true);
        ComparisonCard(panel.transform, "PlainWaterAppleCard", "HIRISAN BUAH\nDALAM AIR BIASA", PlainWaterApplePath,
            new Vector2(344, 0), Purple, new Vector2(270, 286), false);
    }

    static void ComparisonCard(Transform parent, string name, string titleText, string artPath,
        Vector2 position, Color accent, Vector2 artSize, bool positive)
    {
        Image card = Panel(name, parent, position, new Vector2(315, 420),
            new Color(1, 1, 1, .84f), accent, 2);
        Image title = Panel("Title", card.transform, new Vector2(0, 180), new Vector2(285, 62),
            accent, Darker(accent, .26f), 2);
        NewText("Text", title.transform, titleText, titleText.Contains("\n") ? 21 : 24,
            Color.white, TextAlignmentOptions.Center, Vector2.zero, new Vector2(270, 54));

        Image art = NewImage("Illustration", card.transform, artPath, Color.white,
            new Vector2(0, -8), artSize, false);
        art.preserveAspect = true;

        if (name == "PickledMangoCard")
        {
            Image note = Panel("Note", card.transform, new Vector2(0, -174), new Vector2(270, 45),
                new Color32(225, 247, 190, 255), Green, 2);
            NewText("Text", note.transform, "Makanan berjeruk", 21, Ink,
                TextAlignmentOptions.Center, Vector2.zero, new Vector2(250, 36));
            return;
        }

        BuildResultMark(card.transform, positive, new Vector2(116, -154));
        NewText("Result", card.transform, positive ? "Lambat rosak" : "Lebih cepat rosak", 20,
            positive ? Ink : new Color32(130, 35, 42, 255), TextAlignmentOptions.Center,
            new Vector2(-20, -174), new Vector2(225, 40));
    }

    static void BuildResultMark(Transform parent, bool positive, Vector2 position)
    {
        Color accent = positive ? Green : Red;
        Image circle = NewImage(positive ? "Tick" : "Cross", parent, CirclePath, accent,
            position, new Vector2(60, 60), false);
        circle.preserveAspect = true;
        OutlineGraphic(circle.gameObject, Color.white, 3);

        if (positive)
        {
            SolidBar("Short", circle.transform, new Vector2(-9, -2), new Vector2(22, 8), Color.white, -43);
            SolidBar("Long", circle.transform, new Vector2(8, 4), new Vector2(34, 8), Color.white, 47);
        }
        else
        {
            SolidBar("SlashA", circle.transform, Vector2.zero, new Vector2(37, 8), Color.white, 45);
            SolidBar("SlashB", circle.transform, Vector2.zero, new Vector2(37, 8), Color.white, -45);
        }
    }

    static void BuildConceptSequence(Transform root)
    {
        ConceptChip(root, "HighAcidityChip", "Keasidan tinggi\n(pH rendah)", "pH",
            null, new Vector2(-120, -232), Red);
        ConceptChip(root, "ReducedGrowthChip", "Pertumbuhan\nberkurang", string.Empty,
            GrowthPath, new Vector2(245, -232), Blue);
        ConceptChip(root, "LongerKeepingChip", "Makanan tahan\nlebih lama", "clock",
            null, new Vector2(610, -232), Green);
    }

    static void ConceptChip(Transform root, string name, string label, string iconKind,
        string iconPath, Vector2 position, Color accent)
    {
        Image chip = Panel(name, root, position, new Vector2(330, 90), accent, Darker(accent, .28f), 3);
        Image iconCircle = NewImage("IconCircle", chip.transform, CirclePath, new Color(1, 1, 1, .95f),
            new Vector2(-122, 0), new Vector2(72, 72), false);
        iconCircle.preserveAspect = true;
        OutlineGraphic(iconCircle.gameObject, Color.white, 2);

        if (!string.IsNullOrEmpty(iconPath))
        {
            Image icon = NewImage("Icon", iconCircle.transform, iconPath, Color.white,
                Vector2.zero, new Vector2(56, 56), false);
            icon.preserveAspect = true;
        }
        else if (iconKind == "clock")
        {
            OutlineGraphic(iconCircle.gameObject, accent, 2);
            SolidBar("HourHand", iconCircle.transform, new Vector2(0, 8), new Vector2(6, 23), accent, 0);
            SolidBar("MinuteHand", iconCircle.transform, new Vector2(9, -1), new Vector2(22, 6), accent, 15);
            Image dot = NewImage("Pivot", iconCircle.transform, CirclePath, accent,
                Vector2.zero, new Vector2(12, 12), false);
            dot.preserveAspect = true;
        }
        else
        {
            NewText("IconText", iconCircle.transform, iconKind, 25, accent,
                TextAlignmentOptions.Center, Vector2.zero, new Vector2(62, 52));
        }

        NewText("Label", chip.transform, label, 24, Color.white, TextAlignmentOptions.Center,
            new Vector2(36, 0), new Vector2(225, 75));
    }

    static void BuildBottomCards(Transform root)
    {
        Image idea = Panel("MainIdeaCard", root, new Vector2(140, -353), new Vector2(1320, 92),
            new Color(1, .98f, .86f, .98f), Orange, 3);
        Image ideaChip = Panel("Label", idea.transform, new Vector2(-520, 0), new Vector2(230, 65),
            Purple, Darker(Purple, .27f), 2);
        Image bulb = NewImage("Bulb", ideaChip.transform, LightbulbPath, Color.white,
            new Vector2(-88, 0), new Vector2(50, 50), false);
        bulb.preserveAspect = true;
        NewText("Text", ideaChip.transform, "IDEA UTAMA", 22, Color.white,
            TextAlignmentOptions.Center, new Vector2(20, 0), new Vector2(165, 48));
        TextMeshProUGUI ideaText = NewText("Text", idea.transform,
            "Keadaan yang terlalu <color=#7C34C8>berasid</color> boleh menghalang pertumbuhan mikroorganisma. Oleh itu, makanan berjeruk boleh tahan lebih lama.",
            22, Dark, TextAlignmentOptions.Center, new Vector2(130, 0), new Vector2(990, 72));
        ideaText.richText = true;

        Image trigger = Panel("TriggerQuestionCard", root, new Vector2(140, -463), new Vector2(1320, 86),
            new Color(1, .97f, .91f, .98f), Blue, 3);
        Image triggerChip = Panel("Label", trigger.transform, new Vector2(-500, 0), new Vector2(270, 62),
            new Color32(45, 103, 195, 255), new Color32(24, 67, 143, 255), 2);
        NewText("Mark", triggerChip.transform, "?", 40, Color.white, TextAlignmentOptions.Center,
            new Vector2(-103, 0), new Vector2(52, 50));
        NewText("Text", triggerChip.transform, "SOALAN PENCETUS", 19, Color.white,
            TextAlignmentOptions.Center, new Vector2(18, 0), new Vector2(195, 48));
        NewText("Text", trigger.transform,
            "Mengapakah hirisan buah dalam cuka lebih lambat rosak?", 25, Dark,
            TextAlignmentOptions.Center, new Vector2(160, 0), new Vector2(900, 66));
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

    static void SolidBar(string name, Transform parent, Vector2 position, Vector2 size, Color color, float angle)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        SetRect(go.transform, position, size);
        go.transform.localEulerAngles = new Vector3(0, 0, angle);
        Image image = go.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
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
        if (sprite == null) Debug.LogWarning("[Akt5Keasidan] Sprite missing: " + path);
        return sprite;
    }

    static TextMeshProUGUI NewText(string name, Transform parent, string value, float size,
        Color color, TextAlignmentOptions alignment, Vector2 position, Vector2 rectSize)
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
