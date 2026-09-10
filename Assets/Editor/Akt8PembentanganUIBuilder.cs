#if UNITY_EDITOR
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class Akt8PembentanganUIBuilder
{
    private const string ScenePath = "Assets/Scenes/Aktivity/akt8/akt8.unity";
    private const string PlatePath = "Assets/Images/Akt8PembentanganUI/Background/pembentangan_presentation_plate.png";
    private const string FontPath = "Assets/Fonts/FredokaOne SDF.asset";
    private const string PageName = "Pembentangan";
    private const string PreservedName = "PreservedPembentanganContent";
    private const string RootName = "PembentanganRedesignRoot";

    private static TMP_FontAsset font;
    private static Sprite roundedSprite;

    [MenuItem("MicroFun/Build Activity 8 Pembentangan")]
    public static void Build()
    {
        var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        Canvas canvas = null;
        foreach (var candidate in Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (candidate.transform.Find("Center") != null)
            {
                canvas = candidate;
                break;
            }
        }

        if (canvas == null)
        {
            Debug.LogError("Aktiviti 8 main Canvas was not found.");
            return;
        }

        var page = canvas.transform.Find(PageName);
        var center = canvas.transform.Find("Center");
        if (center == null)
        {
            Debug.LogError("Expected Canvas/Center object was not found.");
            return;
        }

        if (page == null)
        {
            page = CreateUIObject(PageName, canvas.transform).transform;
            Stretch(page.GetComponent<RectTransform>());
        }

        font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath) ?? TMP_Settings.defaultFontAsset;
        roundedSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        ConfigurePlateImporter();
        var plate = AssetDatabase.LoadAssetAtPath<Texture2D>(PlatePath);
        if (plate == null)
        {
            Debug.LogError("Pembentangan presentation plate is missing at " + PlatePath);
            return;
        }

        var pageRect = page.GetComponent<RectTransform>();
        Stretch(pageRect);
        page.gameObject.SetActive(true);
        center.gameObject.SetActive(false);

        var preserved = PreserveExistingChildren(page);
        preserved.gameObject.SetActive(false);

        var oldRoot = page.Find(RootName);
        if (oldRoot != null)
        {
            Object.DestroyImmediate(oldRoot.gameObject);
        }

        var root = CreateUIObject(RootName, page);
        Stretch(root.GetComponent<RectTransform>());
        var controller = root.AddComponent<PembentanganSectionController>();

        CreateBackground(root, plate);
        CreateHeader(root.transform);
        CreateAudioButton(root.transform);
        CreateHomeButton(root.transform, page.gameObject, center.gameObject);
        CreateTaskCard(root.transform);
        CreateWhiteboardOverlay(root.transform);
        CreateGroupBadge(root.transform);

        var prompt = CreateTopicPrompt(root.transform);
        controller.Configure(prompt.panel, prompt.text);
        CreateBottomNavigation(root.transform, controller);

        WireCenterMenuButton(center, page.gameObject);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Selection.activeGameObject = root;
        Debug.Log("Built Activity 8 Pembentangan reference UI and preserved the previous hierarchy.");
    }

    private static Transform PreserveExistingChildren(Transform page)
    {
        var preserved = page.Find(PreservedName);
        if (preserved != null)
        {
            return preserved;
        }

        var existing = new List<Transform>();
        for (var i = 0; i < page.childCount; i++)
        {
            var child = page.GetChild(i);
            if (child.name != RootName)
            {
                existing.Add(child);
            }
        }

        var preservedObject = CreateUIObject(PreservedName, page);
        Stretch(preservedObject.GetComponent<RectTransform>());
        foreach (var child in existing)
        {
            child.SetParent(preservedObject.transform, true);
        }

        return preservedObject.transform;
    }

    private static void CreateBackground(GameObject root, Texture2D plate)
    {
        var background = CreateUIObject("Background", root.transform, typeof(RawImage));
        Stretch(background.GetComponent<RectTransform>());
        var rawImage = background.GetComponent<RawImage>();
        rawImage.texture = plate;
        rawImage.color = Color.white;
        rawImage.raycastTarget = false;

        var softWash = Panel("HeaderSoftWash", root.transform, new Vector2(0, 390), new Vector2(1500, 300),
            new Color(1f, .97f, .82f, .22f), Color.clear, 0, false);
        softWash.GetComponent<Image>().raycastTarget = false;
    }

    private static void CreateHeader(Transform root)
    {
        var badge = Panel("ActivityBadge", root, new Vector2(0, 493), new Vector2(270, 68),
            new Color(1f, .96f, .76f, .98f), new Color(1f, 1f, 1f, .98f), 3, true);
        Text("Label", "AKTIVITI 8", badge.transform, Vector2.zero, new Vector2(250, 56), 36,
            new Color(.02f, .33f, .09f), FontStyles.Bold, TextAlignmentOptions.Center);

        var title = Text("Title", "Pembentangan", root, new Vector2(0, 385), new Vector2(1100, 150), 104,
            new Color(.02f, .39f, .06f), FontStyles.Bold, TextAlignmentOptions.Center);
        AddOutlineAndShadow(title.gameObject, new Color(1f, 1f, 1f, .98f), new Vector2(5, -5),
            new Color(.25f, .18f, .03f, .28f), new Vector2(0, -9));

        var subtitlePanel = Panel("SubtitlePanel", root, new Vector2(0, 292), new Vector2(1030, 54),
            new Color(1f, .97f, .82f, .72f), Color.clear, 0, false);
        Text("Subtitle", "Bentangkan hasil perbincangan tentang mikroorganisma kumpulan anda.", subtitlePanel.transform,
            Vector2.zero, new Vector2(1000, 48), 29, new Color(.02f, .30f, .11f), FontStyles.Bold,
            TextAlignmentOptions.Center);

        var logo = CreateUIObject("MicroSquadLogo", root);
        SetRect(logo.GetComponent<RectTransform>(), new Vector2(770, 430), new Vector2(300, 120));
        var micro = Text("Micro", "MICRO", logo.transform, new Vector2(0, 24), new Vector2(280, 64), 47,
            new Color(.08f, .48f, .11f), FontStyles.Bold, TextAlignmentOptions.Center);
        AddOutlineAndShadow(micro.gameObject, Color.white, new Vector2(2, -2), new Color(0, 0, 0, .18f), new Vector2(0, -4));
        var squad = Text("Squad", "SQUAD", logo.transform, new Vector2(0, -25), new Vector2(280, 62), 45,
            new Color(1f, .52f, .02f), FontStyles.Bold, TextAlignmentOptions.Center);
        AddOutlineAndShadow(squad.gameObject, Color.white, new Vector2(2, -2), new Color(0, 0, 0, .18f), new Vector2(0, -4));
    }

    private static void CreateAudioButton(Transform root)
    {
        var buttonObject = Panel("AudioNarrationButton", root, new Vector2(-730, 474), new Vector2(300, 78),
            new Color(1f, .99f, .91f, .97f), new Color(.04f, .45f, .12f), 3, true);
        var button = buttonObject.AddComponent<Button>();
        button.targetGraphic = buttonObject.GetComponent<Image>();
        buttonObject.AddComponent<AudioSource>();
        var narration = buttonObject.AddComponent<AudioNarrationButton>();
        AddButtonFeedback(buttonObject);

        var icon = Circle("SpeakerBadge", buttonObject.transform, new Vector2(-116, 0), new Vector2(76, 76),
            new Color(.03f, .53f, .10f), Color.white, 3);
        Text("Speaker", "))", icon.transform, Vector2.zero, new Vector2(60, 54), 32, Color.white,
            FontStyles.Bold, TextAlignmentOptions.Center);
        var label = Text("Label", "Dengar audio", buttonObject.transform, new Vector2(35, 0), new Vector2(205, 58), 30,
            new Color(.02f, .31f, .09f), FontStyles.Bold, TextAlignmentOptions.Center);
        var serializedNarration = new SerializedObject(narration);
        serializedNarration.FindProperty("label").objectReferenceValue = label;
        serializedNarration.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void CreateHomeButton(Transform root, GameObject page, GameObject center)
    {
        var go = Circle("HomeButton", root, new Vector2(-920, 474), new Vector2(66, 66),
            new Color(.04f, .55f, .13f), Color.white, 3);
        var button = go.AddComponent<Button>();
        button.targetGraphic = go.GetComponent<Image>();
        AddButtonFeedback(go);
        Text("Icon", "<", go.transform, new Vector2(-2, 2), new Vector2(52, 52), 42, Color.white,
            FontStyles.Bold, TextAlignmentOptions.Center);
        UnityEventTools.AddBoolPersistentListener(button.onClick, page.SetActive, false);
        UnityEventTools.AddBoolPersistentListener(button.onClick, center.SetActive, true);
    }

    private static void CreateTaskCard(Transform root)
    {
        var card = Panel("TaskCard", root, new Vector2(-730, -15), new Vector2(365, 430),
            new Color(1f, .99f, .92f, .96f), new Color(1f, 1f, 1f, .94f), 3, true);
        var clip = Circle("ClipboardBadge", card.transform, new Vector2(0, 192), new Vector2(100, 100),
            new Color(.24f, .67f, .14f), Color.white, 4);
        var paper = Panel("Paper", clip.transform, Vector2.zero, new Vector2(54, 64), Color.white,
            new Color(.10f, .33f, .10f), 2, false);
        for (var i = 0; i < 3; i++)
        {
            ImageBar("Line" + i, paper.transform, new Vector2(6, 15 - i * 15), new Vector2(30, 4),
                new Color(.12f, .45f, .16f));
            Circle("Dot" + i, paper.transform, new Vector2(-19, 15 - i * 15), new Vector2(6, 6),
                new Color(.10f, .45f, .15f), Color.clear, 0);
        }

        Text("Heading", "Tugasan", card.transform, new Vector2(0, 115), new Vector2(320, 65), 48,
            new Color(.03f, .34f, .10f), FontStyles.Bold, TextAlignmentOptions.Center);
        CreateDottedDivider(card.transform, new Vector2(0, 72), 270, new Color(.35f, .65f, .19f));
        var copy = Text("TaskText", "Terangkan ciri,\ncontoh dan\nkepentingan\nmikroorganisma\nyang dipilih.", card.transform,
            new Vector2(0, -56), new Vector2(310, 255), 34, new Color(.02f, .22f, .12f),
            FontStyles.Bold, TextAlignmentOptions.Center);
        copy.enableWordWrapping = true;
        copy.lineSpacing = 3;
    }

    private static void CreateWhiteboardOverlay(Transform root)
    {
        var overlay = CreateUIObject("WhiteboardOverlay", root);
        SetRect(overlay.GetComponent<RectTransform>(), new Vector2(0, 0), new Vector2(620, 450));

        Text("BoardTitle", "MIKROORGANISMA", overlay.transform, new Vector2(0, 217), new Vector2(480, 42), 27,
            new Color(.03f, .37f, .15f), FontStyles.Bold, TextAlignmentOptions.Center);

        CreateBoardColumn(overlay.transform, -174, "CIRI", new Color(.28f, .65f, .17f), "o");
        CreateBoardColumn(overlay.transform, 0, "CONTOH", new Color(.12f, .48f, .80f), "••");
        CreateBoardColumn(overlay.transform, 174, "KEPENTINGAN", new Color(.96f, .50f, .05f), "+");
    }

    private static void CreateBoardColumn(Transform parent, float x, string heading, Color color, string icon)
    {
        Text(heading + "Heading", heading, parent, new Vector2(x, 168), new Vector2(164, 34), 20,
            color, FontStyles.Bold, TextAlignmentOptions.Center);
        ImageBar(heading + "Divider", parent, new Vector2(x, 146), new Vector2(132, 3), new Color(color.r, color.g, color.b, .55f));
        var iconCircle = Circle(heading + "Icon", parent, new Vector2(x, 91), new Vector2(74, 74),
            new Color(color.r, color.g, color.b, .16f), new Color(color.r, color.g, color.b, .72f), 2);
        Text("Mark", icon, iconCircle.transform, Vector2.zero, new Vector2(62, 54), heading == "KEPENTINGAN" ? 42 : 24,
            color, FontStyles.Bold, TextAlignmentOptions.Center);
        for (var i = 0; i < 3; i++)
        {
            Circle("Bullet" + i, parent, new Vector2(x - 55, 28 - i * 22), new Vector2(8, 8), color, Color.clear, 0);
            ImageBar("AnswerLine" + i, parent, new Vector2(x + 10, 28 - i * 22), new Vector2(105, 6),
                new Color(.60f, .52f, .36f, .72f));
        }
    }

    private static void CreateGroupBadge(Transform root)
    {
        var badge = Panel("GroupActivityBadge", root, new Vector2(735, 34), new Vector2(330, 155),
            new Color(1f, .98f, .88f, .95f), Color.white, 3, true);
        var icon = Circle("GroupIcon", badge.transform, new Vector2(-112, 0), new Vector2(98, 98),
            new Color(1f, .73f, .12f, .28f), new Color(1f, .73f, .12f, .75f), 2);
        Circle("HeadA", icon.transform, new Vector2(-21, 16), new Vector2(22, 22), new Color(.07f, .49f, .12f), Color.clear, 0);
        Circle("HeadB", icon.transform, new Vector2(0, 23), new Vector2(25, 25), new Color(.07f, .49f, .12f), Color.clear, 0);
        Circle("HeadC", icon.transform, new Vector2(22, 16), new Vector2(22, 22), new Color(.07f, .49f, .12f), Color.clear, 0);
        Panel("BodyA", icon.transform, new Vector2(-22, -14), new Vector2(24, 31), new Color(.07f, .49f, .12f), Color.clear, 0, false);
        Panel("BodyB", icon.transform, new Vector2(0, -11), new Vector2(28, 38), new Color(.07f, .49f, .12f), Color.clear, 0, false);
        Panel("BodyC", icon.transform, new Vector2(22, -14), new Vector2(24, 31), new Color(.07f, .49f, .12f), Color.clear, 0, false);
        var text = Text("Label", "Aktiviti\npembentangan\nberkumpulan", badge.transform, new Vector2(58, 0),
            new Vector2(190, 125), 26, new Color(.03f, .33f, .10f), FontStyles.Bold, TextAlignmentOptions.Center);
        text.lineSpacing = 2;
    }

    private static (GameObject panel, TextMeshProUGUI text) CreateTopicPrompt(Transform root)
    {
        var panel = Panel("TopicPromptPanel", root, new Vector2(0, -350), new Vector2(860, 70),
            new Color(1f, .98f, .85f, .97f), Color.white, 3, true);
        var label = Text("Prompt", string.Empty, panel.transform, new Vector2(-20, 0), new Vector2(760, 54), 26,
            new Color(.02f, .30f, .11f), FontStyles.Bold, TextAlignmentOptions.Center);
        var close = Circle("CloseButton", panel.transform, new Vector2(392, 0), new Vector2(46, 46),
            new Color(.96f, .34f, .21f), Color.white, 2);
        var closeButton = close.AddComponent<Button>();
        closeButton.targetGraphic = close.GetComponent<Image>();
        Text("X", "X", close.transform, Vector2.zero, new Vector2(38, 38), 24, Color.white,
            FontStyles.Bold, TextAlignmentOptions.Center);
        panel.SetActive(false);
        return (panel, label);
    }

private static void CreateBottomNavigation(Transform root, PembentanganSectionController controller)
    {
        var ciri = CreateNavigationButton("CiriButton", root, new Vector2(-415, -460), new Vector2(350, 88),
            "Ciri", new Color(.25f, .70f, .17f), 0);
        var contoh = CreateNavigationButton("ContohButton", root, new Vector2(0, -460), new Vector2(350, 88),
            "Contoh", new Color(.19f, .57f, .86f), 1);
        var kepentingan = CreateNavigationButton("KepentinganButton", root, new Vector2(415, -460), new Vector2(390, 88),
            "Kepentingan", new Color(1f, .58f, .08f), 2);
        var quiz = CreateNavigationButton("QuizButton", root, new Vector2(785, -460), new Vector2(250, 88),
            "Kuiz", new Color(.58f, .32f, .82f), 2);

        var quizIcon = quiz.transform.Find("Icon");
        if (quizIcon != null)
        {
            for (var i = quizIcon.childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(quizIcon.GetChild(i).gameObject);
            }

            Text("QuestionMark", "?", quizIcon, Vector2.zero, new Vector2(58, 58), 44, new Color(.45f, .20f, .72f),
                FontStyles.Bold, TextAlignmentOptions.Center);
        }

        UnityEventTools.AddPersistentListener(ciri.onClick, controller.ShowCiri);
        UnityEventTools.AddPersistentListener(contoh.onClick, controller.ShowContoh);
        UnityEventTools.AddPersistentListener(kepentingan.onClick, controller.ShowKepentingan);

        SceneController sceneController = null;
        foreach (var candidate in Object.FindObjectsByType<SceneController>(FindObjectsInactive.Include))
        {
            if (candidate.gameObject.scene == root.gameObject.scene)
            {
                sceneController = candidate;
                break;
            }
        }

        if (sceneController != null)
        {
            UnityEventTools.AddStringPersistentListener(quiz.onClick, sceneController.ChangeScene, "kuiz8");
        }
        else
        {
            Debug.LogError("SceneController was not found; QuizButton could not be linked to kuiz8.");
        }

        var promptPanel = root.Find("TopicPromptPanel");
        if (promptPanel != null)
        {
            var closeButton = promptPanel.Find("CloseButton")?.GetComponent<Button>();
            if (closeButton != null)
            {
                UnityEventTools.AddPersistentListener(closeButton.onClick, controller.HidePrompt);
            }
        }
    }

    private static Button CreateNavigationButton(string name, Transform parent, Vector2 position, Vector2 size,
        string label, Color fill, int iconKind)
    {
        var go = Panel(name, parent, position, size, fill, Color.white, 4, true);
        var button = go.AddComponent<Button>();
        button.targetGraphic = go.GetComponent<Image>();
        AddButtonFeedback(go);

        var icon = Circle("Icon", go.transform, new Vector2(-size.x * .35f, 0), new Vector2(72, 72),
            new Color(1f, 1f, 1f, .88f), Color.white, 2);
        if (iconKind == 0)
        {
            var lens = Circle("Lens", icon.transform, new Vector2(-5, 6), new Vector2(34, 34),
                new Color(.58f, .91f, 1f, .85f), new Color(.03f, .43f, .34f), 2);
            ImageBar("Handle", icon.transform, new Vector2(14, -15), new Vector2(10, 29), new Color(.94f, .48f, .05f), -42);
        }
        else if (iconKind == 1)
        {
            var dish = Circle("Dish", icon.transform, Vector2.zero, new Vector2(49, 49),
                new Color(.66f, .90f, 1f, .86f), new Color(.14f, .48f, .75f), 2);
            Circle("Microbe1", dish.transform, new Vector2(-11, 7), new Vector2(9, 9), new Color(.20f, .64f, .88f), Color.clear, 0);
            Circle("Microbe2", dish.transform, new Vector2(9, -8), new Vector2(12, 12), new Color(.08f, .46f, .78f), Color.clear, 0);
            Circle("Microbe3", dish.transform, new Vector2(13, 11), new Vector2(7, 7), new Color(.36f, .75f, .90f), Color.clear, 0);
        }
        else
        {
            var shield = Panel("Shield", icon.transform, Vector2.zero, new Vector2(46, 54),
                new Color(.27f, .69f, .18f), new Color(.06f, .38f, .12f), 2, false);
            Text("Plus", "+", shield.transform, Vector2.zero, new Vector2(40, 45), 37, Color.white,
                FontStyles.Bold, TextAlignmentOptions.Center);
        }

        var text = Text("Label", label, go.transform, new Vector2(size.x * .10f, 0),
            new Vector2(size.x * .68f, 62), label == "Kepentingan" ? 32 : 39, Color.white,
            FontStyles.Bold, TextAlignmentOptions.Center);
        AddOutlineAndShadow(text.gameObject, new Color(.03f, .20f, .10f, .35f), new Vector2(1, -1),
            new Color(0, 0, 0, .24f), new Vector2(0, -3));
        return button;
    }

private static void WireCenterMenuButton(Transform center, GameObject page)
    {
        var menuButtonTransform = center.Find("CenterMenuRedesignRoot/MenuStack/PembentanganButton");
        var menuButton = menuButtonTransform != null ? menuButtonTransform.GetComponent<Button>() : null;
        if (menuButton == null)
        {
            Debug.LogWarning("Center-menu PembentanganButton was not found; page UI was built without changing menu navigation.");
        }
        else if (menuButton.onClick.GetPersistentEventCount() == 0)
        {
            UnityEventTools.AddBoolPersistentListener(menuButton.onClick, center.gameObject.SetActive, false);
            UnityEventTools.AddBoolPersistentListener(menuButton.onClick, page.SetActive, true);
        }

        var microThinkerTransform = center.Find("CenterMenuRedesignRoot/MenuStack/MicroThinkerButton");
        var microThinkerButton = microThinkerTransform != null ? microThinkerTransform.GetComponent<Button>() : null;
        if (microThinkerButton != null)
        {
            for (var i = microThinkerButton.onClick.GetPersistentEventCount() - 1; i >= 0; i--)
            {
                UnityEventTools.RemovePersistentListener(microThinkerButton.onClick, i);
            }
        }
    }

    private static void ConfigurePlateImporter()
    {
        var importer = AssetImporter.GetAtPath(PlatePath) as TextureImporter;
        if (importer == null)
        {
            return;
        }

        var changed = importer.textureType != TextureImporterType.Default || importer.mipmapEnabled || importer.maxTextureSize < 2048;
        importer.textureType = TextureImporterType.Default;
        importer.mipmapEnabled = false;
        importer.sRGBTexture = true;
        importer.alphaIsTransparency = true;
        importer.maxTextureSize = 2048;
        importer.textureCompression = TextureImporterCompression.CompressedHQ;
        if (changed)
        {
            importer.SaveAndReimport();
        }
    }

    private static GameObject Panel(string name, Transform parent, Vector2 position, Vector2 size, Color fill,
        Color border, int borderSize, bool shadow)
    {
        var go = CreateUIObject(name, parent, typeof(Image));
        SetRect(go.GetComponent<RectTransform>(), position, size);
        var image = go.GetComponent<Image>();
        image.sprite = roundedSprite;
        image.type = Image.Type.Sliced;
        image.color = fill;

        if (borderSize > 0 && border.a > 0)
        {
            var outline = go.AddComponent<Outline>();
            outline.effectColor = border;
            outline.effectDistance = new Vector2(borderSize, -borderSize);
        }

        if (shadow)
        {
            var dropShadow = go.AddComponent<Shadow>();
            dropShadow.effectColor = new Color(0, 0, 0, .20f);
            dropShadow.effectDistance = new Vector2(0, -7);
        }

        return go;
    }

    private static GameObject Circle(string name, Transform parent, Vector2 position, Vector2 size, Color fill,
        Color border, int borderSize)
    {
        return Panel(name, parent, position, size, fill, border, borderSize, false);
    }

    private static TextMeshProUGUI Text(string name, string value, Transform parent, Vector2 position, Vector2 size,
        float fontSize, Color color, FontStyles style, TextAlignmentOptions alignment)
    {
        var go = CreateUIObject(name, parent, typeof(TextMeshProUGUI));
        SetRect(go.GetComponent<RectTransform>(), position, size);
        var text = go.GetComponent<TextMeshProUGUI>();
        text.font = font;
        text.text = value;
        text.fontSize = fontSize;
        text.color = color;
        text.fontStyle = style;
        text.alignment = alignment;
        text.enableWordWrapping = false;
        text.overflowMode = TextOverflowModes.Overflow;
        text.raycastTarget = false;
        return text;
    }

    private static GameObject ImageBar(string name, Transform parent, Vector2 position, Vector2 size, Color color,
        float rotation = 0)
    {
        var go = CreateUIObject(name, parent, typeof(Image));
        SetRect(go.GetComponent<RectTransform>(), position, size);
        go.GetComponent<Image>().color = color;
        go.GetComponent<Image>().raycastTarget = false;
        go.transform.localEulerAngles = new Vector3(0, 0, rotation);
        return go;
    }

    private static void CreateDottedDivider(Transform parent, Vector2 center, float width, Color color)
    {
        const int count = 18;
        for (var i = 0; i < count; i++)
        {
            var x = center.x - width * .5f + i * (width / (count - 1));
            Circle("DividerDot" + i, parent, new Vector2(x, center.y), new Vector2(6, 6), color, Color.clear, 0);
        }
    }

    private static void AddOutlineAndShadow(GameObject go, Color outlineColor, Vector2 outlineDistance,
        Color shadowColor, Vector2 shadowDistance)
    {
        var outline = go.AddComponent<Outline>();
        outline.effectColor = outlineColor;
        outline.effectDistance = outlineDistance;
        var shadow = go.AddComponent<Shadow>();
        shadow.effectColor = shadowColor;
        shadow.effectDistance = shadowDistance;
    }

    private static void AddButtonFeedback(GameObject go)
    {
        if (go.GetComponent<AnimatedButtonFeedback>() == null)
        {
            go.AddComponent<AnimatedButtonFeedback>();
        }

        var button = go.GetComponent<Button>();
        if (button == null)
        {
            return;
        }

        var colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1f, 1f, 1f, .94f);
        colors.pressedColor = new Color(.82f, .82f, .82f, 1f);
        colors.selectedColor = Color.white;
        button.colors = colors;
    }

    private static GameObject CreateUIObject(string name, Transform parent, params System.Type[] extraComponents)
    {
        var types = new List<System.Type> { typeof(RectTransform) };
        if (extraComponents != null)
        {
            types.AddRange(extraComponents);
        }

        var go = new GameObject(name, types.ToArray());
        go.layer = LayerMask.NameToLayer("UI");
        go.transform.SetParent(parent, false);
        return go;
    }

    private static void SetRect(RectTransform rect, Vector2 position, Vector2 size)
    {
        rect.anchorMin = rect.anchorMax = new Vector2(.5f, .5f);
        rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        rect.localScale = Vector3.one;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.pivot = new Vector2(.5f, .5f);
        rect.localScale = Vector3.one;
    }
}
#endif
