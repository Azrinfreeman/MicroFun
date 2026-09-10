using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Akt8MicroThinkerUIBuilder
{
    private const string ScenePath = "Assets/Scenes/Aktivity/akt8/akt8.unity";
    private const string BackupPath = "Assets/Scenes/Aktivity/akt8/akt8_PreMicroThinkerRedesign_20260830.unity";
    private const string BackgroundPath = "Assets/Images/Akt8MicroThinkerUI/Background/akt8_microthinker_background.png";
    private const string MicroscopePath = "Assets/Images/Akt8MicroThinkerUI/Icons/icon_jenis_microscope.png";
    private const string PetriPath = "Assets/Images/Akt8MicroThinkerUI/Icons/icon_contoh_petri.png";
    private const string ShieldPath = "Assets/Images/Akt8MicroThinkerUI/Icons/icon_kepentingan_shield.png";
    private const string MagnifierPath = "Assets/Images/Akt8MicroThinkerUI/Icons/icon_ciri_magnifier.png";

    private static readonly Color Ink = Hex("#173B18");
    private static readonly Color Cream = Hex("#FFFDF2");
    private static readonly Color Green = Hex("#74C52D");
    private static readonly Color DeepGreen = Hex("#17690E");
    private static readonly Color Orange = Hex("#F3AE25");
    private static readonly Color Pink = Hex("#F174A8");
    private static readonly Color Blue = Hex("#4DB9DF");
    private static readonly Color Purple = Hex("#A96ACE");
    private static readonly Color SoftShadow = new Color(0.19f, 0.16f, 0.08f, 0.24f);

    private static TMP_FontAsset font;
    private static Sprite uiSprite;

    [MenuItem("Tools/MicroFun/Rebuild Akt8 Micro Thinker Mind Map")]
    public static void Rebuild()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.path != ScenePath)
        {
            Debug.LogError("Open Assets/Scenes/Aktivity/akt8/akt8.unity before rebuilding the Micro Thinker UI.");
            return;
        }

        EnsureBackup();
        PrepareAssets();
        font = TMP_Settings.defaultFontAsset;
        if (font == null)
            font = FindFirstAsset<TMP_FontAsset>("t:TMP_FontAsset");
        uiSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");

        GameObject canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            Debug.LogError("Canvas was not found.");
            return;
        }

        Transform microThinker = canvas.transform.Find("MicroThinker");
        if (microThinker == null)
        {
            Debug.LogError("Canvas/MicroThinker was not found.");
            return;
        }

        Undo.RegisterFullObjectHierarchyUndo(microThinker.gameObject, "Rebuild Akt8 Micro Thinker UI");
        PreserveLegacyContent(microThinker);

        Transform existing = microThinker.Find("MicroThinkerMindMapRoot");
        if (existing != null)
            Object.DestroyImmediate(existing.gameObject);

        RectTransform root = CreateStretch("MicroThinkerMindMapRoot", microThinker);
        root.SetAsFirstSibling();
        BuildBackground(root);
        BuildHeader(root);
        BuildInstructions(root);
        BuildMindMap(root);
        BuildRightMessage(root);
        BuildActivityBadge(root);

        Transform home = microThinker.Find("HomeButton");
        if (home != null)
        {
            home.gameObject.SetActive(true);
            home.SetAsLastSibling();
        }

        SetLayerRecursively(root.gameObject, LayerMask.NameToLayer("UI"));
        EditorUtility.SetDirty(microThinker.gameObject);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Aktiviti 8 Micro Thinker mind-map UI rebuilt. Home navigation preserved; center-menu Micro Thinker callback left unchanged.");
    }

    private static void BuildBackground(RectTransform root)
    {
        Image image = CreateImage("Background", root, Vector2.zero, new Vector2(1920f, 1080f), LoadSprite(BackgroundPath), Color.white);
        Stretch(image.rectTransform);
        image.preserveAspect = false;
        image.raycastTarget = false;
    }

    private static void BuildHeader(RectTransform root)
    {
        RectTransform badge = CreatePanel("ActivityBadge", root, new Vector2(0f, 488f), new Vector2(370f, 76f), Hex("#EFF7D2"), DeepGreen, 3f, 0.18f);
        CreateDashedBorder(badge, DeepGreen, 12, 3f);
        CreateText("Label", badge, Vector2.zero, new Vector2(330f, 62f), "AKTIVITI 8", 44f, DeepGreen, FontStyles.Bold);

        TextMeshProUGUI title = CreateText("Title", root, new Vector2(0f, 384f), new Vector2(1120f, 170f), "Micro Thinker", 100f, Hex("#2D8F10"), FontStyles.Bold);
        title.outlineWidth = 0.11f;
        title.outlineColor = Color.white;
        AddShadow(title.gameObject, new Color(0.17f, 0.17f, 0.04f, 0.38f), new Vector2(7f, -8f));

        RectTransform subtitle = CreatePanel("SubtitleCapsule", root, new Vector2(0f, 274f), new Vector2(1110f, 66f), new Color(1f, 0.99f, 0.91f, 0.95f), DeepGreen, 2f, 0.12f);
        CreateDashedBorder(subtitle, Hex("#57952C"), 25, 2f);
        CreateText("Text", subtitle, Vector2.zero, new Vector2(1060f, 54f),
            "Lengkapkan peta minda berdasarkan pembelajaran tentang mikroorganisma.",
            25f, Ink, FontStyles.Bold);
    }

    private static void BuildInstructions(RectTransform root)
    {
        RectTransform card = CreatePanel("InstructionsCard", root, new Vector2(-770f, -95f), new Vector2(300f, 445f), new Color(1f, 0.99f, 0.92f, 0.97f), Hex("#91B94A"), 3f, 0.2f);


        RectTransform heading = CreatePanel("Heading", card, new Vector2(0f, 175f), new Vector2(190f, 58f), Hex("#65AD2A"), DeepGreen, 2f, 0.16f);
        CreateDashedBorder(heading, Cream, 9, 2f);
        CreateText("Text", heading, Vector2.zero, new Vector2(174f, 48f), "Arahan:", 32f, Color.white, FontStyles.Bold);

        CreateInstructionStep(card, 1, "Baca semula\nmaklumat dalam\nmodul.", 84f);
        CreateInstructionStep(card, 2, "Lengkapkan\npeta minda.", -28f);
        CreateInstructionStep(card, 3, "Kongsi jawapan\nbersama rakan.", -132f);

    }

    private static void CreateInstructionStep(RectTransform card, int number, string copy, float y)
    {
        RectTransform circle = CreatePanel("Step" + number, card, new Vector2(-108f, y), new Vector2(45f, 45f), Hex("#67B52E"), DeepGreen, 2f, 0.12f);
        CreateText("Number", circle, Vector2.zero, new Vector2(42f, 42f), number.ToString(), 25f, Color.white, FontStyles.Bold);
        TextMeshProUGUI text = CreateText("Text", card, new Vector2(31f, y), new Vector2(205f, 92f), copy, 22f, Ink, FontStyles.Bold);
        text.alignment = TextAlignmentOptions.MidlineLeft;
        text.enableAutoSizing = true;
        text.fontSizeMin = 17f;
        text.fontSizeMax = 22f;
    }

    private static void BuildMindMap(RectTransform root)
    {
        RectTransform board = CreatePanel("MindMapBoard", root, new Vector2(70f, -112f), new Vector2(1120f, 535f), new Color(1f, 0.995f, 0.96f, 0.97f), Hex("#E9E2CF"), 3f, 0.22f);
        RectTransform connectors = CreateStretch("ConnectorLayer", board);

        RectTransform central = CreatePanel("CentralNode", board, Vector2.zero, new Vector2(320f, 92f), Hex("#B7E979"), Hex("#4E9827"), 3f, 0.2f);
        CreateDashedBorder(central, Hex("#4B8D27"), 12, 2f);
        CreateText("Text", central, Vector2.zero, new Vector2(295f, 76f), "Mikroorganisma", 30f, Ink, FontStyles.Bold);

        CreateBranch(board, connectors, "Jenis", new Vector2(-250f, 105f), true,
            new[] { 172f, 105f, 38f }, Orange, LoadSprite(MicroscopePath));
        CreateBranch(board, connectors, "Contoh", new Vector2(250f, 105f), false,
            new[] { 172f, 105f, 38f }, Pink, LoadSprite(PetriPath));
        CreateBranch(board, connectors, "Kepentingan", new Vector2(-250f, -122f), true,
            new[] { -48f, -122f, -196f }, Blue, LoadSprite(ShieldPath));
        CreateBranch(board, connectors, "Ciri", new Vector2(250f, -122f), false,
            new[] { -48f, -122f, -196f }, Purple, LoadSprite(MagnifierPath));
    }

    private static void CreateBranch(RectTransform board, RectTransform connectors, string label, Vector2 labelPosition, bool left,
        float[] answerYs, Color branchColor, Sprite iconSprite)
    {
        float answerX = left ? -475f : 475f;
        float labelHalfWidth = label == "Kepentingan" ? 98f : 82f;
        float labelWidth = labelHalfWidth * 2f;

        Vector2 labelToCenterStart = labelPosition + new Vector2(left ? labelHalfWidth : -labelHalfWidth, 0f);
        Vector2 labelToCenterEnd = new Vector2(left ? -160f : 160f, labelPosition.y > 0f ? 18f : -18f);
        AddConnector(connectors, label + "_CenterConnector", labelToCenterStart, labelToCenterEnd, branchColor, 6f, 80f);

        for (int index = 0; index < answerYs.Length; index++)
        {
            Vector2 answerPosition = new Vector2(answerX, answerYs[index]);
            Vector2 start = labelPosition + new Vector2(left ? -labelHalfWidth : labelHalfWidth, 0f);
            Vector2 end = answerPosition + new Vector2(left ? 82f : -82f, 0f);
            AddConnector(connectors, label + "_AnswerConnector_" + (index + 1), start, end, branchColor, 5f, 68f);
        }

        Image icon = CreateImage(label + "Icon", board,
            labelPosition + new Vector2(0f, labelPosition.y > 0f ? 82f : 78f),
            new Vector2(100f, 100f), iconSprite, Color.white);
        icon.preserveAspect = true;
        icon.raycastTarget = false;

        RectTransform pill = CreatePanel(label + "Node", board, labelPosition, new Vector2(labelWidth, 62f),
            Color.Lerp(branchColor, Color.white, 0.27f), Color.Lerp(branchColor, Color.black, 0.18f), 2f, 0.15f);
        CreateDashedBorder(pill, Color.Lerp(branchColor, Color.black, 0.12f), label == "Kepentingan" ? 8 : 7, 2f);
        CreateText("Text", pill, Vector2.zero, new Vector2(labelWidth - 16f, 52f), label,
            label == "Kepentingan" ? 23f : 28f, Ink, FontStyles.Bold);

        RectTransform inputs = CreateRect(label + "AnswerFields", board, Vector2.zero, Vector2.zero);
        for (int index = 0; index < answerYs.Length; index++)
            CreateAnswerField(label + "Answer" + (index + 1), inputs, new Vector2(answerX, answerYs[index]), branchColor);
    }

    private static void AddConnector(RectTransform parent, string name, Vector2 start, Vector2 end, Color color, float thickness, float bend)
    {
        RectTransform rect = CreateStretch(name, parent);
        MindMapConnectorGraphic graphic = rect.gameObject.AddComponent<MindMapConnectorGraphic>();
        graphic.Configure(start, end, color, thickness, bend);
    }

    private static void CreateAnswerField(string name, RectTransform parent, Vector2 position, Color branchColor)
    {
        RectTransform rect = CreateRect(name, parent, position, new Vector2(164f, 60f));
        Image background = rect.gameObject.AddComponent<Image>();
        background.sprite = uiSprite;
        background.type = Image.Type.Sliced;
        background.color = new Color(1f, 0.995f, 0.97f, 0.98f);
        Outline outline = rect.gameObject.AddComponent<Outline>();
        outline.effectColor = branchColor;
        outline.effectDistance = new Vector2(2.5f, -2.5f);
        Shadow shadow = rect.gameObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(0f, 0f, 0f, 0.12f);
        shadow.effectDistance = new Vector2(3f, -4f);

        TMP_InputField input = rect.gameObject.AddComponent<TMP_InputField>();
        input.targetGraphic = background;
        input.lineType = TMP_InputField.LineType.SingleLine;
        input.contentType = TMP_InputField.ContentType.Standard;
        input.characterLimit = 32;

        RectTransform viewport = CreateStretch("Text Area", rect);
        viewport.offsetMin = new Vector2(12f, 7f);
        viewport.offsetMax = new Vector2(-12f, -7f);
        viewport.gameObject.AddComponent<RectMask2D>();

        TextMeshProUGUI placeholder = CreateText("Placeholder", viewport, Vector2.zero, Vector2.zero, string.Empty, 20f,
            new Color(Ink.r, Ink.g, Ink.b, 0.35f), FontStyles.Italic);
        Stretch(placeholder.rectTransform);
        placeholder.alignment = TextAlignmentOptions.Center;

        TextMeshProUGUI value = CreateText("Text", viewport, Vector2.zero, Vector2.zero, string.Empty, 22f, Ink, FontStyles.Bold);
        Stretch(value.rectTransform);
        value.alignment = TextAlignmentOptions.Center;
        value.enableAutoSizing = true;
        value.fontSizeMin = 14f;
        value.fontSizeMax = 22f;

        input.textViewport = viewport;
        input.textComponent = value;
        input.placeholder = placeholder;
        input.caretColor = Color.Lerp(branchColor, Color.black, 0.2f);
        input.selectionColor = new Color(branchColor.r, branchColor.g, branchColor.b, 0.35f);

        ColorBlock colors = input.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = Color.Lerp(Color.white, branchColor, 0.12f);
        colors.pressedColor = Color.Lerp(Color.white, branchColor, 0.22f);
        colors.selectedColor = Color.Lerp(Color.white, branchColor, 0.16f);
        colors.disabledColor = new Color(0.8f, 0.8f, 0.8f, 0.65f);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.12f;
        input.colors = colors;
    }

    private static void BuildRightMessage(RectTransform root)
    {
        TextMeshProUGUI message = CreateText("ExerciseYourMind", root, new Vector2(785f, -228f), new Vector2(260f, 170f),
            "<color=#3C9DDA>Exercise</color>\n<color=#58A832>Your</color> <color=#F08A3A>Mind</color>",
            43f, Color.white, FontStyles.Bold);
        message.richText = true;
        message.outlineWidth = 0.25f;
        message.outlineColor = Color.white;
        AddShadow(message.gameObject, new Color(0.24f, 0.14f, 0.05f, 0.35f), new Vector2(4f, -5f));
    }

    private static void BuildActivityBadge(RectTransform root)
    {
        RectTransform badge = CreatePanel("IndividualActivityBadge", root, new Vector2(0f, -492f), new Vector2(410f, 72f),
            new Color(1f, 0.99f, 0.9f, 0.98f), Hex("#6CA533"), 2f, 0.18f);
        CreateDashedBorder(badge, Hex("#6CA533"), 15, 2f);

        RectTransform iconCircle = CreatePanel("Icon", badge, new Vector2(-150f, 0f), new Vector2(55f, 55f), DeepGreen, DeepGreen, 1f, 0.08f);
        Image head = CreateImage("Head", iconCircle, new Vector2(0f, 11f), new Vector2(17f, 17f), uiSprite, Color.white);
        head.type = Image.Type.Sliced;
        Image body = CreateImage("Body", iconCircle, new Vector2(0f, -10f), new Vector2(31f, 22f), uiSprite, Color.white);
        body.type = Image.Type.Sliced;
        CreateText("Text", badge, new Vector2(38f, 0f), new Vector2(290f, 56f), "Aktiviti individu", 30f, Ink, FontStyles.Bold);
    }

    private static void PreserveLegacyContent(Transform microThinker)
    {
        Transform legacy = microThinker.Find("LegacyMicroThinkerContent");
        if (legacy == null)
        {
            RectTransform legacyRect = CreateStretch("LegacyMicroThinkerContent", microThinker);
            legacy = legacyRect;
        }

        List<Transform> toMove = new List<Transform>();
        foreach (Transform child in microThinker)
        {
            if (child == legacy || child.name == "HomeButton" || child.name == "MicroThinkerMindMapRoot")
                continue;
            toMove.Add(child);
        }

        foreach (Transform child in toMove)
            child.SetParent(legacy, false);

        legacy.gameObject.SetActive(false);
    }

    private static void PrepareAssets()
    {
        ConfigureSprite(BackgroundPath, false);
        ConfigureSprite(MicroscopePath, true);
        ConfigureSprite(PetriPath, true);
        ConfigureSprite(ShieldPath, true);
        ConfigureSprite(MagnifierPath, true);
    }

    private static void ConfigureSprite(string path, bool alpha)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
            return;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = alpha;
        importer.mipmapEnabled = false;
        importer.filterMode = FilterMode.Bilinear;
        importer.textureCompression = TextureImporterCompression.CompressedHQ;
        importer.maxTextureSize = path == BackgroundPath ? 4096 : 2048;
        importer.SaveAndReimport();
    }

    private static void EnsureBackup()
    {
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(BackupPath) == null)
        {
            AssetDatabase.CopyAsset(ScenePath, BackupPath);
            AssetDatabase.SaveAssets();
        }
    }

    private static RectTransform CreateRect(string name, Transform parent, Vector2 position, Vector2 size)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        return rect;
    }

    private static RectTransform CreateStretch(string name, Transform parent)
    {
        RectTransform rect = CreateRect(name, parent, Vector2.zero, Vector2.zero);
        Stretch(rect);
        return rect;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;
    }

    private static Image CreateImage(string name, Transform parent, Vector2 position, Vector2 size, Sprite sprite, Color color)
    {
        RectTransform rect = CreateRect(name, parent, position, size);
        Image image = rect.gameObject.AddComponent<Image>();
        image.sprite = sprite;
        image.color = color;
        return image;
    }

    private static RectTransform CreatePanel(string name, Transform parent, Vector2 position, Vector2 size,
        Color fill, Color outlineColor, float outlineWidth, float shadowStrength)
    {
        Image image = CreateImage(name, parent, position, size, uiSprite, fill);
        image.type = Image.Type.Sliced;

        Shadow shadow = image.gameObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(SoftShadow.r, SoftShadow.g, SoftShadow.b, shadowStrength);
        shadow.effectDistance = new Vector2(5f, -6f);

        Outline outline = image.gameObject.AddComponent<Outline>();
        outline.effectColor = outlineColor;
        outline.effectDistance = new Vector2(outlineWidth, -outlineWidth);
        return image.rectTransform;
    }

    private static TextMeshProUGUI CreateText(string name, Transform parent, Vector2 position, Vector2 size,
        string value, float fontSize, Color color, FontStyles style)
    {
        RectTransform rect = CreateRect(name, parent, position, size);
        TextMeshProUGUI text = rect.gameObject.AddComponent<TextMeshProUGUI>();
        text.text = value;
        text.font = font;
        text.fontSize = fontSize;
        text.color = color;
        text.fontStyle = style;
        text.alignment = TextAlignmentOptions.Center;
        text.enableWordWrapping = true;
        text.overflowMode = TextOverflowModes.Ellipsis;
        text.raycastTarget = false;
        return text;
    }

    private static void CreateDashedBorder(RectTransform parent, Color color, int horizontalCount, float thickness)
    {
        Rect rect = parent.rect;
        float width = parent.sizeDelta.x;
        float height = parent.sizeDelta.y;
        float dashWidth = Mathf.Max(10f, (width - 28f) / (horizontalCount * 1.75f));
        float gap = (width - 28f - dashWidth * horizontalCount) / Mathf.Max(1, horizontalCount - 1);
        float startX = -width * 0.5f + 14f + dashWidth * 0.5f;

        RectTransform border = CreateRect("DashedBorder", parent, Vector2.zero, parent.sizeDelta);
        border.SetAsLastSibling();

        for (int i = 0; i < horizontalCount; i++)
        {
            float x = startX + i * (dashWidth + gap);
            CreateSolidDash(border, new Vector2(x, height * 0.5f - 7f), new Vector2(dashWidth, thickness), color);
            CreateSolidDash(border, new Vector2(x, -height * 0.5f + 7f), new Vector2(dashWidth, thickness), color);
        }

        int verticalCount = Mathf.Max(2, Mathf.RoundToInt(height / 26f));
        float verticalGap = (height - 30f) / Mathf.Max(1, verticalCount - 1);
        for (int i = 0; i < verticalCount; i++)
        {
            float y = -height * 0.5f + 15f + i * verticalGap;
            CreateSolidDash(border, new Vector2(-width * 0.5f + 7f, y), new Vector2(thickness, 10f), color);
            CreateSolidDash(border, new Vector2(width * 0.5f - 7f, y), new Vector2(thickness, 10f), color);
        }
    }

    private static void CreateSolidDash(Transform parent, Vector2 position, Vector2 size, Color color)
    {
        Image dash = CreateImage("Dash", parent, position, size, null, color);
        dash.raycastTarget = false;
    }

    private static void AddShadow(GameObject target, Color color, Vector2 distance)
    {
        Shadow shadow = target.AddComponent<Shadow>();
        shadow.effectColor = color;
        shadow.effectDistance = distance;
    }

    private static Sprite LoadSprite(string path)
    {
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    private static T FindFirstAsset<T>(string filter) where T : Object
    {
        string[] guids = AssetDatabase.FindAssets(filter);
        if (guids.Length == 0)
            return null;
        return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids[0]));
    }

    private static void SetLayerRecursively(GameObject target, int layer)
    {
        if (layer < 0)
            layer = 5;
        target.layer = layer;
        foreach (Transform child in target.transform)
            SetLayerRecursively(child.gameObject, layer);
    }

    private static Color Hex(string value)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(value, out color))
            return color;
        return Color.white;
    }
}
