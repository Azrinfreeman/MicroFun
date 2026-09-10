#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Rebuilds the Activities menu presentation while preserving the original
/// scene Button components and their serialized SceneController callbacks.
/// </summary>
public static class ActivitiesUIBuilder
{
    private const string ScenePath = "Assets/Scenes/Activities.unity";
    private const string BackupPath = "Assets/Scenes/Activities_PreRedesign_20260829.unity";
    private const string ArtFolder = "Assets/Images/ActivitiesUI";
    private const string GeneratedFolder = ArtFolder + "/Generated";
    private const string FontPath = "Assets/Fonts/FredokaOne SDF.asset";

    private static TMP_FontAsset _font;
    private static Sprite _pillSprite;
    private static Sprite _circleSprite;

    private static readonly Color Navy = Hex("102D5A");
    private static readonly Color DeepGreen = Hex("075A20");
    private static readonly Color ConnectorGreen = Hex("51A928");

    private static readonly Dictionary<int, Color> ButtonColors = new Dictionary<int, Color>
    {
        { 1, Hex("A8DB45") },
        { 2, Hex("39C8C4") },
        { 3, Hex("FFB934") },
        { 4, Hex("A77BE7") },
        { 5, Hex("63B4F4") },
        { 6, Hex("F36F9D") },
        { 7, Hex("35BCE5") },
        { 8, Hex("28B918") }
    };

    private static readonly Dictionary<int, string> IconPaths = new Dictionary<int, string>
    {
        { 1, "Assets/Images/UI Flat/icon_clipboard_pencil.png" },
        { 2, "Assets/Images/UI Flat/icon_magnify_glass.png" },
        { 3, "Assets/Images/UI Flat/icon_microscope_card.png" },
        { 4, "Assets/Images/ActivitiesUI/icon_activity04_shield_question.png" },
        { 5, "Assets/Images/ActivitiesUI/icon_activity05_puzzle.png" },
        { 6, "Assets/Images/UI Flat/icon_lightbulb.png" },
        { 7, "Assets/Images/ActivitiesUI/icon_activity07_chart.png" }
    };

    [MenuItem("Tools/MicroFun/Rebuild Activities UI")]
    public static void Build()
    {
        var scene = EditorSceneManager.GetActiveScene();
        if (scene.path != ScenePath)
        {
            Debug.LogError("Open Assets/Scenes/Activities.unity before rebuilding the Activities UI.");
            return;
        }

        EnsureBackup();
        PrepareSpriteAssets();

        _font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        _pillSprite = AssetDatabase.LoadAssetAtPath<Sprite>(GeneratedFolder + "/ui_pill_white.png");
        _circleSprite = AssetDatabase.LoadAssetAtPath<Sprite>(GeneratedFolder + "/ui_circle_white.png");

        var backgroundSprite = AssetDatabase.LoadAssetAtPath<Sprite>(ArtFolder + "/activities_bg_decor_1920x1080.png");
        var portraitSprite = AssetDatabase.LoadAssetAtPath<Sprite>(ArtFolder + "/activities_center_students_1024.png");

        if (_font == null || _pillSprite == null || _circleSprite == null || backgroundSprite == null || portraitSprite == null)
        {
            Debug.LogError("Activities UI assets were not imported correctly. Reimport Assets/Images/ActivitiesUI and retry.");
            return;
        }

        var canvas = UnityEngine.Object.FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Activities scene has no Canvas.");
            return;
        }

        ConfigureCanvas(canvas);

        var activityButtons = FindActivityButtons(canvas);
        var backButton = FindBackButton(canvas, activityButtons.Values);
        if (activityButtons.Count != 8)
        {
            Debug.LogError("Expected 8 activity buttons but found " + activityButtons.Count + ". No destructive rebuild was performed.");
            return;
        }

        // Move preserved buttons out before removing a previous generated presentation.
        foreach (var button in activityButtons.Values)
            button.transform.SetParent(canvas.transform, true);
        if (backButton != null)
            backButton.transform.SetParent(canvas.transform, true);

        var previous = canvas.transform.Find("ActivitiesUI_Redesign");
        if (previous != null)
            UnityEngine.Object.DestroyImmediate(previous.gameObject);

        DisableLegacyPresentation(canvas);

        var designRoot = CreateRect("ActivitiesUI_Redesign", canvas.transform, Vector2.zero, new Vector2(1920f, 1080f));
        Stretch(designRoot);

        var background = CreateImage("Background", designRoot, backgroundSprite, Color.white, Vector2.zero, new Vector2(1920f, 1080f));
        Stretch(background.rectTransform);
        background.raycastTarget = false;

        var softVignette = CreateImage("SoftCenterGlow", designRoot, _circleSprite, new Color(1f, 1f, 0.9f, 0.12f), new Vector2(0f, -25f), new Vector2(1050f, 860f));
        softVignette.raycastTarget = false;

        BuildHeader(designRoot);

        var mapRoot = CreateRect("ActivityMap", designRoot, Vector2.zero, new Vector2(1920f, 1080f));
        Stretch(mapRoot);
        BuildConnectors(mapRoot);
        BuildPortrait(mapRoot, portraitSprite);
        BuildActivityButtons(mapRoot, activityButtons);
        BuildBackButton(designRoot, backButton);

        EditorUtility.SetDirty(canvas.gameObject);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeGameObject = designRoot.gameObject;
        Debug.Log("Activities UI redesign built successfully. Existing activity and StartScene callbacks were preserved.");
    }

    private static void EnsureBackup()
    {
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(BackupPath) == null)
        {
            if (!AssetDatabase.CopyAsset(ScenePath, BackupPath))
                Debug.LogWarning("Could not create Activities scene backup at " + BackupPath);
            else
                AssetDatabase.SaveAssets();
        }
    }

    private static void PrepareSpriteAssets()
    {
        if (!AssetDatabase.IsValidFolder(ArtFolder))
            AssetDatabase.CreateFolder("Assets/Images", "ActivitiesUI");
        if (!AssetDatabase.IsValidFolder(GeneratedFolder))
            AssetDatabase.CreateFolder(ArtFolder, "Generated");

        CreateShapeTexture(GeneratedFolder + "/ui_pill_white.png", 256, 96, 46f, false);
        CreateShapeTexture(GeneratedFolder + "/ui_circle_white.png", 256, 256, 128f, true);

        EnsureSpriteImporter(ArtFolder + "/activities_bg_decor_1920x1080.png", Vector4.zero, 2048);
        EnsureSpriteImporter(ArtFolder + "/activities_center_students_1024.png", Vector4.zero, 2048);
        EnsureSpriteImporter(GeneratedFolder + "/ui_pill_white.png", new Vector4(46f, 46f, 46f, 46f), 512);
        EnsureSpriteImporter(GeneratedFolder + "/ui_circle_white.png", Vector4.zero, 512);

        // Existing project icons retain their current import settings. Reimporting
        // source artwork here can disturb multi-sprite or platform overrides.
        foreach (var iconPath in IconPaths.Values.Where(path => path.StartsWith(ArtFolder, StringComparison.Ordinal)))
            EnsureSpriteImporter(iconPath, Vector4.zero, 1024);

        AssetDatabase.Refresh();
    }

    private static void CreateShapeTexture(string assetPath, int width, int height, float radius, bool circle)
    {
        var absolutePath = Path.Combine(Directory.GetCurrentDirectory(), assetPath.Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(absolutePath))
            return;

        var texture = new Texture2D(width, height, TextureFormat.RGBA32, false, true);
        var pixels = new Color32[width * height];
        var center = new Vector2((width - 1) * 0.5f, (height - 1) * 0.5f);
        var half = new Vector2(width * 0.5f, height * 0.5f);

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                float signedDistance;
                if (circle)
                {
                    signedDistance = Vector2.Distance(new Vector2(x, y), center) - radius + 1f;
                }
                else
                {
                    var p = new Vector2(Mathf.Abs(x - center.x), Mathf.Abs(y - center.y));
                    var q = p - (half - Vector2.one * radius);
                    signedDistance = Mathf.Min(Mathf.Max(q.x, q.y), 0f) + new Vector2(Mathf.Max(q.x, 0f), Mathf.Max(q.y, 0f)).magnitude - radius;
                }

                var alpha = (byte)Mathf.RoundToInt(Mathf.Clamp01(0.8f - signedDistance) * 255f);
                pixels[y * width + x] = new Color32(255, 255, 255, alpha);
            }
        }

        texture.SetPixels32(pixels);
        texture.Apply();
        File.WriteAllBytes(absolutePath, texture.EncodeToPNG());
        UnityEngine.Object.DestroyImmediate(texture);
        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport);
    }

    private static void EnsureSpriteImporter(string path, Vector4 border, int maxSize)
    {
        if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), path.Replace('/', Path.DirectorySeparatorChar))))
        {
            Debug.LogWarning("Activities UI sprite is missing: " + path);
            return;
        }

        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
            return;

        var changed = importer.textureType != TextureImporterType.Sprite ||
                      importer.spriteImportMode != SpriteImportMode.Single ||
                      importer.mipmapEnabled ||
                      importer.maxTextureSize != maxSize ||
                      importer.spriteBorder != border;

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.maxTextureSize = maxSize;
        importer.textureCompression = TextureImporterCompression.CompressedHQ;
        importer.spritePixelsPerUnit = 100f;
        importer.spriteBorder = border;
        if (changed)
            importer.SaveAndReimport();
    }

    private static void ConfigureCanvas(Canvas canvas)
    {
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 0;
        var scaler = canvas.GetComponent<CanvasScaler>();
        if (scaler == null)
            scaler = canvas.gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
    }

    private static Dictionary<int, Button> FindActivityButtons(Canvas canvas)
    {
        var result = new Dictionary<int, Button>();
        foreach (var button in canvas.GetComponentsInChildren<Button>(true))
        {
            var number = ParseActivityNumber(button);
            if (number >= 1 && number <= 8 && !result.ContainsKey(number))
                result[number] = button;
        }
        return result;
    }

    private static int ParseActivityNumber(Button button)
    {
        var match = Regex.Match(button.name, @"Activity0?([1-8])", RegexOptions.IgnoreCase);
        if (match.Success)
            return int.Parse(match.Groups[1].Value);

        var label = button.GetComponentInChildren<TMP_Text>(true);
        if (label == null)
            return -1;
        match = Regex.Match(label.text ?? string.Empty, @"([1-8])");
        return match.Success ? int.Parse(match.Groups[1].Value) : -1;
    }

    private static Button FindBackButton(Canvas canvas, IEnumerable<Button> activityButtons)
    {
        var activities = new HashSet<Button>(activityButtons);
        return canvas.GetComponentsInChildren<Button>(true).FirstOrDefault(button => !activities.Contains(button));
    }

    private static void DisableLegacyPresentation(Canvas canvas)
    {
        foreach (var raw in canvas.GetComponentsInChildren<RawImage>(true))
            raw.gameObject.SetActive(false);

        var legacyCenter = canvas.transform.Find("Image (1)");
        if (legacyCenter != null)
            legacyCenter.gameObject.SetActive(false);

        var videoPlayer = GameObject.Find("Video Player");
        if (videoPlayer != null)
            videoPlayer.SetActive(false);
    }

    private static void BuildHeader(RectTransform root)
    {
        var badgeRoot = CreateRect("ActivityBadge", root, new Vector2(0f, 477f), new Vector2(330f, 76f));
        CreateImage("Shadow", badgeRoot, _pillSprite, new Color(0.26f, 0.18f, 0.04f, 0.28f), new Vector2(0f, -8f), new Vector2(342f, 82f));
        CreateImage("WhiteFrame", badgeRoot, _pillSprite, new Color(1f, 0.99f, 0.9f, 1f), Vector2.zero, new Vector2(342f, 82f));
        CreateImage("GreenFill", badgeRoot, _pillSprite, Hex("4E9F22"), Vector2.zero, new Vector2(320f, 64f));
        CreateImage("TopGloss", badgeRoot, _pillSprite, new Color(1f, 1f, 1f, 0.16f), new Vector2(0f, 13f), new Vector2(292f, 27f));
        var badgeText = CreateText("BadgeText", badgeRoot, "AKTIVITI 8", 43f, Color.white, Vector2.zero, new Vector2(300f, 70f));
        badgeText.outlineColor = new Color32(24, 85, 22, 255);
        badgeText.outlineWidth = 0.16f;

        var titleShadow = CreateText("TitleShadow", root, "Jom Teroka Aktiviti 8!", 94f, new Color(0.12f, 0.27f, 0.05f, 0.30f), new Vector2(6f, 374f), new Vector2(1450f, 145f));
        titleShadow.fontStyle = FontStyles.Bold;
        var title = CreateText("MainTitle", root, "Jom Teroka Aktiviti 8!", 94f, Hex("168C22"), new Vector2(0f, 384f), new Vector2(1450f, 145f));
        title.fontStyle = FontStyles.Bold;
        title.outlineColor = new Color32(255, 252, 238, 255);
        title.outlineWidth = 0.25f;

        var subtitle = CreateText("Subtitle", root, "Aktiviti pengukuhan untuk menguji kefahaman tentang mikroorganisma.", 31f, Navy, new Vector2(0f, 302f), new Vector2(1200f, 58f));
        subtitle.fontStyle = FontStyles.Bold;

        CreateLeafFlourish(root, new Vector2(-662f, 303f), false);
        CreateLeafFlourish(root, new Vector2(662f, 303f), true);
    }

    private static void CreateLeafFlourish(RectTransform parent, Vector2 position, bool mirror)
    {
        var root = CreateRect(mirror ? "RightLeafFlourish" : "LeftLeafFlourish", parent, position, new Vector2(62f, 34f));
        var direction = mirror ? -1f : 1f;
        var leaf1 = CreateImage("LeafLarge", root, _pillSprite, Hex("2E9C1F"), new Vector2(12f * direction, 6f), new Vector2(30f, 11f));
        leaf1.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 22f * direction);
        var leaf2 = CreateImage("LeafSmall", root, _pillSprite, Hex("5CB722"), new Vector2(13f * direction, -7f), new Vector2(23f, 9f));
        leaf2.rectTransform.localRotation = Quaternion.Euler(0f, 0f, -17f * direction);
    }

    private static void BuildConnectors(RectTransform root)
    {
        var connectorRoot = CreateRect("Connectors", root, Vector2.zero, new Vector2(1920f, 1080f));
        Stretch(connectorRoot);

        var segments = new[]
        {
            new[] { new Vector2(-190f, 116f), new Vector2(-265f, 174f), new Vector2(-286f, 174f) },
            new[] { new Vector2(-222f, 20f), new Vector2(-267f, 36f) },
            new[] { new Vector2(-214f, -120f), new Vector2(-286f, -111f) },
            new[] { new Vector2(-164f, -220f), new Vector2(-237f, -260f), new Vector2(-268f, -260f) },
            new[] { new Vector2(190f, 116f), new Vector2(270f, 174f), new Vector2(286f, 174f) },
            new[] { new Vector2(222f, 20f), new Vector2(286f, 39f) },
            new[] { new Vector2(214f, -120f), new Vector2(294f, -103f) },
            new[] { new Vector2(164f, -220f), new Vector2(240f, -250f), new Vector2(282f, -250f) }
        };

        foreach (var points in segments)
        {
            for (var i = 0; i < points.Length - 1; i++)
                CreateDottedSegment(connectorRoot, points[i], points[i + 1], 7);
            CreateConnectorNode(connectorRoot, points[points.Length - 1]);
        }
    }

    private static void CreateDottedSegment(RectTransform parent, Vector2 from, Vector2 to, int count)
    {
        for (var i = 1; i <= count; i++)
        {
            var t = i / (count + 1f);
            var dot = CreateImage("Dot", parent, _circleSprite, ConnectorGreen, Vector2.Lerp(from, to, t), new Vector2(10f, 10f));
            dot.raycastTarget = false;
        }
    }

    private static void CreateConnectorNode(RectTransform parent, Vector2 position)
    {
        CreateImage("NodeWhite", parent, _circleSprite, Color.white, position, new Vector2(29f, 29f)).raycastTarget = false;
        CreateImage("NodeGreen", parent, _circleSprite, ConnectorGreen, position, new Vector2(19f, 19f)).raycastTarget = false;
    }

    private static void BuildPortrait(RectTransform root, Sprite portraitSprite)
    {
        var center = new Vector2(0f, -62f);
        CreateImage("PortraitShadow", root, _circleSprite, new Color(0.18f, 0.20f, 0.05f, 0.28f), center + new Vector2(0f, -12f), new Vector2(458f, 458f));
        CreateImage("PortraitOuterGreen", root, _circleSprite, Hex("65B833"), center, new Vector2(450f, 450f));
        CreateImage("PortraitWhiteRing", root, _circleSprite, new Color(1f, 0.99f, 0.91f, 1f), center, new Vector2(433f, 433f));
        CreateImage("PortraitInnerGreen", root, _circleSprite, Hex("8DD247"), center, new Vector2(415f, 415f));

        var maskImage = CreateImage("PortraitMask", root, _circleSprite, Color.white, center, new Vector2(401f, 401f));
        var mask = maskImage.gameObject.AddComponent<Mask>();
        mask.showMaskGraphic = true;

        var portrait = CreateImage("StudentsPortrait", maskImage.rectTransform, portraitSprite, Color.white, Vector2.zero, new Vector2(405f, 405f));
        portrait.preserveAspect = true;
        portrait.raycastTarget = false;

        var gloss = CreateImage("PortraitGloss", root, _circleSprite, new Color(1f, 1f, 1f, 0.10f), center + new Vector2(-52f, 83f), new Vector2(250f, 180f));
        gloss.raycastTarget = false;
    }

    private static void BuildActivityButtons(RectTransform root, Dictionary<int, Button> buttons)
    {
        var positions = new Dictionary<int, Vector2>
        {
            { 1, new Vector2(-445f, 176f) },
            { 8, new Vector2(-480f, 35f) },
            { 7, new Vector2(-450f, -111f) },
            { 6, new Vector2(-395f, -260f) },
            { 2, new Vector2(445f, 176f) },
            { 3, new Vector2(490f, 39f) },
            { 4, new Vector2(500f, -103f) },
            { 5, new Vector2(425f, -250f) }
        };

        for (var activity = 1; activity <= 8; activity++)
            ConfigureActivityButton(buttons[activity], root, activity, positions[activity]);
    }

    private static void ConfigureActivityButton(Button button, RectTransform parent, int activity, Vector2 position)
    {
        button.gameObject.SetActive(true);
        button.gameObject.name = "Activity" + activity.ToString("00");
        button.transform.SetParent(parent, false);

        var rect = button.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;
        var selected = activity == 8;
        var size = selected ? new Vector2(420f, 112f) : new Vector2(345f, 90f);
        rect.sizeDelta = size;

        for (var i = rect.childCount - 1; i >= 0; i--)
            UnityEngine.Object.DestroyImmediate(rect.GetChild(i).gameObject);

        var rootGraphic = button.GetComponent<Image>();
        if (rootGraphic == null)
            rootGraphic = button.gameObject.AddComponent<Image>();
        rootGraphic.color = new Color(1f, 1f, 1f, 0f);
        rootGraphic.raycastTarget = false;

        var palette = ButtonColors[activity];
        var shadow = CreateImage("Shadow", rect, _pillSprite, new Color(0.20f, 0.14f, 0.04f, selected ? 0.34f : 0.24f), new Vector2(0f, -8f), size + new Vector2(10f, 10f));
        shadow.raycastTarget = false;
        var frame = CreateImage("WhiteFrame", rect, _pillSprite, new Color(1f, 0.99f, 0.92f, 1f), Vector2.zero, size + new Vector2(10f, 10f));
        frame.raycastTarget = false;
        var border = CreateImage("ColorBorder", rect, _pillSprite, Darken(palette, 0.18f), Vector2.zero, size + new Vector2(1f, 1f));
        border.raycastTarget = false;
        var fill = CreateImage("Fill", rect, _pillSprite, palette, Vector2.zero, size - new Vector2(8f, 8f));
        fill.raycastTarget = true;
        var gloss = CreateImage("TopGloss", rect, _pillSprite, new Color(1f, 1f, 1f, 0.20f), new Vector2(10f, size.y * 0.18f), new Vector2(size.x - 44f, size.y * 0.31f));
        gloss.raycastTarget = false;

        var badgeSize = selected ? 100f : 78f;
        var badgeX = -size.x * 0.5f + (selected ? 57f : 46f);
        CreateImage("BadgeShadow", rect, _circleSprite, new Color(0.15f, 0.12f, 0.05f, 0.30f), new Vector2(badgeX, -5f), new Vector2(badgeSize + 4f, badgeSize + 4f)).raycastTarget = false;
        CreateImage("BadgeWhite", rect, _circleSprite, Color.white, new Vector2(badgeX, 1f), new Vector2(badgeSize + 4f, badgeSize + 4f)).raycastTarget = false;
        CreateImage("BadgeColor", rect, _circleSprite, Darken(palette, 0.09f), new Vector2(badgeX, 1f), new Vector2(badgeSize - 8f, badgeSize - 8f)).raycastTarget = false;

        if (selected)
        {
            CreateCheckmark(rect, new Vector2(badgeX, 1f));
            var selectedGlow = CreateImage("SelectedGlow", rect, _pillSprite, new Color(0.88f, 1f, 0.25f, 0.20f), Vector2.zero, size + new Vector2(26f, 26f));
            selectedGlow.transform.SetAsFirstSibling();
            selectedGlow.raycastTarget = false;
        }
        else
        {
            Sprite iconSprite;
            IconPaths.TryGetValue(activity, out var iconPath);
            iconSprite = string.IsNullOrEmpty(iconPath) ? null : AssetDatabase.LoadAssetAtPath<Sprite>(iconPath);
            if (iconSprite != null)
            {
                var icon = CreateImage("Icon", rect, iconSprite, Color.white, new Vector2(badgeX, 1f), new Vector2(52f, 52f));
                icon.preserveAspect = true;
                icon.raycastTarget = false;
            }
        }

        var labelOffset = selected ? 52f : 38f;
        var label = CreateText("Label", rect, "Aktiviti " + activity, selected ? 47f : 36f, selected ? Color.white : Navy, new Vector2(labelOffset, 0f), new Vector2(size.x - 100f, size.y));
        label.fontStyle = FontStyles.Bold;
        if (selected)
        {
            label.outlineColor = new Color32(13, 93, 24, 255);
            label.outlineWidth = 0.17f;
        }

        button.targetGraphic = fill;
        button.transition = Selectable.Transition.ColorTint;
        var colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.08f, 1.08f, 1.08f, 1f);
        colors.pressedColor = new Color(0.88f, 0.88f, 0.88f, 1f);
        colors.selectedColor = colors.highlightedColor;
        colors.disabledColor = new Color(0.55f, 0.55f, 0.55f, 0.70f);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.08f;
        button.colors = colors;

        if (button.GetComponent<ButtonInteractionFeedback>() == null)
            button.gameObject.AddComponent<ButtonInteractionFeedback>();
    }

    private static void CreateCheckmark(RectTransform parent, Vector2 center)
    {
        var shortStroke = CreateImage("CheckShort", parent, _pillSprite, Color.white, center + new Vector2(-13f, -2f), new Vector2(34f, 13f));
        shortStroke.rectTransform.localRotation = Quaternion.Euler(0f, 0f, -45f);
        shortStroke.raycastTarget = false;
        var longStroke = CreateImage("CheckLong", parent, _pillSprite, Color.white, center + new Vector2(12f, 8f), new Vector2(57f, 13f));
        longStroke.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 45f);
        longStroke.raycastTarget = false;
    }

    private static void BuildBackButton(RectTransform root, Button button)
    {
        if (button == null)
            return;

        button.gameObject.SetActive(true);
        button.gameObject.name = "BackButton";
        button.transform.SetParent(root, false);
        var rect = button.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(70f, -68f);
        rect.sizeDelta = new Vector2(76f, 76f);
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;
        for (var i = rect.childCount - 1; i >= 0; i--)
            UnityEngine.Object.DestroyImmediate(rect.GetChild(i).gameObject);

        var rootGraphic = button.GetComponent<Image>();
        if (rootGraphic == null)
            rootGraphic = button.gameObject.AddComponent<Image>();
        rootGraphic.color = new Color(1f, 1f, 1f, 0f);
        rootGraphic.raycastTarget = false;

        CreateImage("Shadow", rect, _circleSprite, new Color(0.1f, 0.12f, 0.03f, 0.28f), new Vector2(0f, -5f), new Vector2(79f, 79f)).raycastTarget = false;
        CreateImage("WhiteFrame", rect, _circleSprite, Color.white, Vector2.zero, new Vector2(78f, 78f)).raycastTarget = false;
        var fill = CreateImage("Fill", rect, _circleSprite, Hex("4AAE2A"), Vector2.zero, new Vector2(66f, 66f));
        fill.raycastTarget = true;

        var arrowSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Images/UI Flat/icon_arrow_left.png");
        if (arrowSprite != null)
        {
            var arrow = CreateImage("Arrow", rect, arrowSprite, Color.white, Vector2.zero, new Vector2(37f, 37f));
            arrow.preserveAspect = true;
            arrow.raycastTarget = false;
        }
        else
        {
            var arrow = CreateText("Arrow", rect, "<", 42f, Color.white, Vector2.zero, new Vector2(60f, 60f));
            arrow.fontStyle = FontStyles.Bold;
        }

        button.targetGraphic = fill;
        if (button.GetComponent<ButtonInteractionFeedback>() == null)
            button.gameObject.AddComponent<ButtonInteractionFeedback>();
    }

    private static RectTransform CreateRect(string name, Transform parent, Vector2 position, Vector2 size)
    {
        var gameObject = new GameObject(name, typeof(RectTransform));
        gameObject.layer = 5;
        var rect = gameObject.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        rect.localScale = Vector3.one;
        return rect;
    }

    private static Image CreateImage(string name, Transform parent, Sprite sprite, Color color, Vector2 position, Vector2 size)
    {
        var rect = CreateRect(name, parent, position, size);
        var image = rect.gameObject.AddComponent<Image>();
        image.sprite = sprite;
        image.color = color;
        image.type = sprite != null && sprite.border.sqrMagnitude > 0f ? Image.Type.Sliced : Image.Type.Simple;
        image.raycastTarget = false;
        return image;
    }

    private static TextMeshProUGUI CreateText(string name, Transform parent, string text, float size, Color color, Vector2 position, Vector2 bounds)
    {
        var rect = CreateRect(name, parent, position, bounds);
        var label = rect.gameObject.AddComponent<TextMeshProUGUI>();
        label.font = _font;
        label.text = text;
        label.fontSize = size;
        label.color = color;
        label.alignment = TextAlignmentOptions.Center;
        label.textWrappingMode = TextWrappingModes.NoWrap;
        label.overflowMode = TextOverflowModes.Overflow;
        label.raycastTarget = false;
        return label;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;
        rect.localScale = Vector3.one;
    }

    private static Color Darken(Color color, float amount)
    {
        return new Color(color.r * (1f - amount), color.g * (1f - amount), color.b * (1f - amount), color.a);
    }

    private static Color Hex(string value)
    {
        Color color;
        return ColorUtility.TryParseHtmlString("#" + value, out color) ? color : Color.white;
    }
}
#endif
