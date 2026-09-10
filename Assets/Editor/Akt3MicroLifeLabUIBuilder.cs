using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Rebuilds the Micro-Life Lab comparison page around the existing live video
/// RawImages. The video objects and their textures are deliberately preserved.
/// </summary>
public static class Akt3MicroLifeLabUIBuilder
{
    private const string ScenePath = "Assets/Scenes/Aktivity/akt3/akt3.unity";
    private const string BackgroundPath = "Assets/Generated/Akt3/micro_life_lab_background.png";
    private static readonly Color Ink = new Color32(4, 67, 35, 255);
    private static readonly Color Blue = new Color32(32, 133, 227, 255);
    private static readonly Color Green = new Color32(57, 159, 22, 255);
    private static readonly Color Orange = new Color32(244, 130, 9, 255);
    private static readonly Color Cream = new Color32(255, 251, 230, 255);

    [MenuItem("Tools/MicroFun/Rebuild Akt3 Micro-Life Lab UI")]
    public static void Build()
    {
        var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        var canvasObject = GameObject.Find("Canvas");
        var page = GameObject.Find("Canvas/MicroLab/1");
        if (canvasObject == null || page == null)
        {
            Debug.LogError("Akt3 MicroLab comparison page was not found.");
            return;
        }

        var scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        var rawImages = page.GetComponentsInChildren<RawImage>(true).Take(2).ToArray();
        var home = page.transform.Find("Bottom/HomeButton")?.gameObject;
        var next = page.transform.Find("Bottom/NextButton")?.gameObject;
        if (rawImages.Length != 2 || home == null || next == null)
        {
            Debug.LogError("Akt3 MicroLab video panels or navigation buttons are missing.");
            return;
        }

        foreach (var raw in rawImages)
            raw.transform.SetParent(canvasObject.transform, false);
        home.transform.SetParent(canvasObject.transform, false);
        next.transform.SetParent(canvasObject.transform, false);

        for (var index = page.transform.childCount - 1; index >= 0; index--)
            Object.DestroyImmediate(page.transform.GetChild(index).gameObject);

        var pageRect = page.GetComponent<RectTransform>();
        pageRect.anchorMin = Vector2.zero;
        pageRect.anchorMax = Vector2.one;
        pageRect.offsetMin = Vector2.zero;
        pageRect.offsetMax = Vector2.zero;

        CreateBackdrop(page.transform);
        CreateHeading(page.transform);
        CreateComparisonFrame(page.transform);

        PlaceVideo(rawImages[0], page.transform, new Vector2(-360f, -78f), new Vector2(650f, 370f));
        PlaceVideo(rawImages[1], page.transform, new Vector2(360f, -78f), new Vector2(650f, 370f));
        CreatePanelLabel(page.transform, new Vector2(-360f, 144f), "DENGAN OKSIGEN", Green);
        CreatePanelLabel(page.transform, new Vector2(360f, 144f), "TANPA OKSIGEN", Orange);
        CreateVsBadge(page.transform);
        CreateCallout(page.transform, new Vector2(-360f, -338f), new Vector2(600f, 94f), "✓", "Banyak mikroorganisma aktif\ndan membiak dengan baik.", Green);
        CreateCallout(page.transform, new Vector2(360f, -338f), new Vector2(600f, 94f), "×", "Mikroorganisma kurang aktif\natau tidak membiak.", Orange);

        home.transform.SetParent(page.transform, false);
        next.transform.SetParent(page.transform, false);
        StyleNavigationButton(home, new Vector2(-750f, -464f), new Vector2(330f, 96f), "Laman\nUtama", Orange);
        StyleNavigationButton(next, new Vector2(750f, -464f), new Vector2(370f, 96f), "Seterusnya  ›", Blue);

        page.transform.Find("Subtitle")?.SetAsLastSibling();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Akt3 Micro-Life Lab UI rebuilt while preserving video content.");
    }

    private static void CreateBackdrop(Transform parent)
    {
        var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(BackgroundPath);
        var background = new GameObject("GeneratedBackground", typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
        background.transform.SetParent(parent, false);
        var rect = background.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        var image = background.GetComponent<RawImage>();
        image.texture = texture;
        image.raycastTarget = false;
    }

    private static void CreateHeading(Transform parent)
    {
        var badge = Panel("AktivitiBadge", parent, new Vector2(0f, 460f), new Vector2(330f, 78f), Cream, new Color32(255, 235, 180, 255), 2);
        CreateText(badge.transform, "Text", "AKTIVITI 3", Vector2.zero, new Vector2(290f, 60f), 39f, Ink, FontStyles.Bold, TextAlignmentOptions.Center);

        var title = CreateText(parent, "MainTitle", "Micro-Life Lab", new Vector2(0f, 342f), new Vector2(1120f, 115f), 96f, Ink, FontStyles.Bold, TextAlignmentOptions.Center);
        title.outlineWidth = 0.05f;
        title.outlineColor = Color.white;
        title.enableWordWrapping = false;

        var subtitle = CreateText(parent, "Subtitle", "Bandingkan keadaan mikroorganisma dalam dua keadaan berbeza.", new Vector2(0f, 245f), new Vector2(1180f, 52f), 28f, Ink, FontStyles.Bold, TextAlignmentOptions.Center);
        subtitle.overflowMode = TextOverflowModes.Overflow;
        subtitle.lineSpacing = -8f;
    }

    private static void CreateComparisonFrame(Transform parent)
    {
        var outer = Panel("ComparisonFrame", parent, new Vector2(0f, -145f), new Vector2(1510f, 550f), new Color32(242, 252, 255, 235), new Color32(87, 191, 242, 255), 5);
        var left = Panel("OxygenPanel", outer.transform, new Vector2(-365f, 2f), new Vector2(690f, 495f), new Color32(232, 248, 194, 230), new Color32(105, 183, 36, 230), 2);
        left.GetComponent<RectTransform>().SetAsFirstSibling();
        var right = Panel("NoOxygenPanel", outer.transform, new Vector2(365f, 2f), new Vector2(690f, 495f), new Color32(255, 232, 166, 230), new Color32(243, 145, 31, 230), 2);
        right.GetComponent<RectTransform>().SetAsFirstSibling();
    }

    private static void PlaceVideo(RawImage raw, Transform parent, Vector2 position, Vector2 size)
    {
        raw.transform.SetParent(parent, false);
        raw.raycastTarget = false;
        var rect = raw.rectTransform;
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        raw.color = Color.white;
        var outline = raw.GetComponent<Outline>() ?? raw.gameObject.AddComponent<Outline>();
        outline.effectColor = Color.white;
        outline.effectDistance = new Vector2(5f, -5f);
    }

    private static void CreatePanelLabel(Transform parent, Vector2 position, string value, Color fill)
    {
        var label = Panel("Label_" + value, parent, position, new Vector2(420f, 72f), fill, Color.white, 2);
        CreateText(label.transform, "Text", value, Vector2.zero, new Vector2(390f, 58f), 35f, Color.white, FontStyles.Bold, TextAlignmentOptions.Center).enableWordWrapping = false;
    }

    private static void CreateVsBadge(Transform parent)
    {
        var badge = Panel("VsBadge", parent, new Vector2(0f, -78f), new Vector2(130f, 130f), new Color32(201, 245, 255, 255), Color.white, 4);
        badge.GetComponent<Image>().sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        badge.GetComponent<Image>().type = Image.Type.Simple;
        CreateText(badge.transform, "Text", "VS", Vector2.zero, new Vector2(105f, 90f), 55f, Ink, FontStyles.Bold, TextAlignmentOptions.Center);
    }

    private static void CreateCallout(Transform parent, Vector2 position, Vector2 size, string symbol, string value, Color accent)
    {
        var callout = Panel("Callout", parent, position, size, Cream, accent, 2);
        var circle = Panel("Icon", callout.transform, new Vector2(-240f, 0f), new Vector2(58f, 58f), accent, Color.white, 1);
        circle.GetComponent<Image>().sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        circle.GetComponent<Image>().type = Image.Type.Simple;
        CreateText(circle.transform, "Symbol", symbol, Vector2.zero, new Vector2(52f, 52f), 43f, Color.white, FontStyles.Bold, TextAlignmentOptions.Center);
        var text = CreateText(callout.transform, "Text", value, new Vector2(55f, 0f), new Vector2(430f, 74f), 26f, Ink, FontStyles.Bold, TextAlignmentOptions.Left);
        text.lineSpacing = -8f;
    }

    private static void StyleNavigationButton(GameObject buttonObject, Vector2 position, Vector2 size, string label, Color fill)
    {
        for (var i = buttonObject.transform.childCount - 1; i >= 0; i--)
            Object.DestroyImmediate(buttonObject.transform.GetChild(i).gameObject);

        var rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        var image = buttonObject.GetComponent<Image>();
        image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        image.type = Image.Type.Sliced;
        image.color = fill;
        var outline = buttonObject.GetComponent<Outline>() ?? buttonObject.AddComponent<Outline>();
        outline.effectColor = Color.white;
        outline.effectDistance = new Vector2(3f, -3f);
        var button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        var colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1f, 1f, 1f, 0.95f);
        colors.pressedColor = new Color(0.84f, 0.84f, 0.84f, 1f);
        button.colors = colors;
        if (buttonObject.GetComponent<AnimatedButtonFeedback>() == null)
            buttonObject.AddComponent<AnimatedButtonFeedback>();
        var text = CreateText(buttonObject.transform, "Text", label, Vector2.zero, size - new Vector2(28f, 14f), 32f, Color.white, FontStyles.Bold, TextAlignmentOptions.Center);
        text.lineSpacing = -8f;
    }

    private static GameObject Panel(string name, Transform parent, Vector2 position, Vector2 size, Color fill, Color border, int borderWidth)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        var image = go.GetComponent<Image>();
        image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        image.type = Image.Type.Sliced;
        image.color = fill;
        if (borderWidth > 0)
        {
            var outline = go.AddComponent<Outline>();
            outline.effectColor = border;
            outline.effectDistance = new Vector2(borderWidth, -borderWidth);
        }
        return go;
    }

    private static TextMeshProUGUI CreateText(Transform parent, string name, string value, Vector2 position, Vector2 size, float fontSize, Color color, FontStyles style, TextAlignmentOptions alignment)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        var text = go.GetComponent<TextMeshProUGUI>();
        text.font = TMP_Settings.defaultFontAsset;
        text.text = value;
        text.fontSize = fontSize;
        text.color = color;
        text.fontStyle = style;
        text.alignment = alignment;
        text.enableWordWrapping = true;
        text.overflowMode = TextOverflowModes.Ellipsis;
        return text;
    }
}
