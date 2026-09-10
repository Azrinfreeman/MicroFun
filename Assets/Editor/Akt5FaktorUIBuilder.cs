using System;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Akt5FaktorUIBuilder
{
    const string ScenePath = "Assets/Scenes/Aktivity/akt5/akt5.unity";
    const string BgPath = "Assets/Images/Akt5CenterUI/Background/akt5_factor_background_v2.png";
    const string White = "Assets/Images/UI Flat/btn_white_flat.png";
    const string Circle = "Assets/Images/UI Flat/ui_circle_white.png";
    const string Arrow = "Assets/Images/UI Flat/icon_arrow_left.png";
    const string Air = "Assets/Images/air-removebg-preview.png";
    const string Nutrition = "Assets/Images/UI Flat/icon_fun_nutrition.png";
    const string FontPath = "Assets/Fonts/FredokaOne SDF.asset";

    static readonly Color Ink = new Color32(8, 70, 30, 255);
    static readonly Color Dark = new Color32(27, 43, 24, 255);
    static readonly Color Green = new Color32(91, 191, 20, 255);
    static readonly Color Red = new Color32(239, 54, 44, 255);
    static readonly Color Blue = new Color32(32, 139, 230, 255);
    static readonly Color Orange = new Color32(248, 147, 24, 255);
    static readonly Color Purple = new Color32(139, 64, 211, 255);
    static readonly Color Cream = new Color32(255, 249, 226, 255);
    static TMP_FontAsset font;

    [MenuItem("Tools/MicroFun/Activity 5/Build Faktor Theme")]
    public static void Build()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogError("[Akt5Faktor] Exit Play Mode first.");
            return;
        }

        font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        if (font == null)
        {
            Debug.LogError("[Akt5Faktor] Fredoka One font missing.");
            return;
        }

        string original = SceneManager.GetActiveScene().path;
        Backup();
        try
        {
            Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            GameObject canvasObject = GameObject.Find("Canvas");
            if (canvasObject == null) throw new InvalidOperationException("Canvas not found.");
            Transform canvas = canvasObject.transform;
            Transform page = canvas.Find("Faktor");
            Transform center = canvas.Find("Center");
            if (page == null || center == null) throw new InvalidOperationException("Canvas/Faktor or Canvas/Center not found.");

            ConfigureCanvas(canvas.GetComponent<Canvas>());
            BuildPage(canvas, page, center);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[Akt5Faktor] Four-factor menu built; Kehadiran Air omitted.");
        }
        finally
        {
            if (!string.IsNullOrEmpty(original) && original != ScenePath)
                EditorSceneManager.OpenScene(original, OpenSceneMode.Single);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    static void Backup()
    {
        const string parent = "Assets/Scenes/Aktivity/akt5";
        const string dir = parent + "/Backups";
        if (!AssetDatabase.IsValidFolder(dir)) AssetDatabase.CreateFolder(parent, "Backups");
        string destination = dir + "/akt5_PreFaktorRedesign_20260902.unity";
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(destination) == null)
            AssetDatabase.CopyAsset(ScenePath, destination);
    }

    static void ConfigureCanvas(Canvas canvas)
    {
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>() ?? canvas.gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = .5f;
    }

    static void BuildPage(Transform canvas, Transform page, Transform center)
    {
        Delete(page, "Akt5FaktorThemeRoot");
        RectTransform root = NewRect("Akt5FaktorThemeRoot", page);
        Full(root);
        root.SetAsLastSibling();

        foreach (Transform child in page)
            if (child != root) child.gameObject.SetActive(false);

        Image background = NewImage("Background", root, BgPath, Color.white, Vector2.zero, new Vector2(1920, 1080), false);
        background.preserveAspect = false;
        Full(background.rectTransform);

        BuildHeader(root);

        Transform suhu = Require(canvas, "Suhu Yang Sesuai");
        Transform udara = Require(canvas, "Kehadiran Udara");
        Transform nutrien = Require(canvas, "Kehadiran Nutrien");
        Transform keasidan = Require(canvas, "Keasidan Yang Sesuai");
        Transform air = canvas.Find("Kehadiran Air");
        if (air != null) air.gameObject.SetActive(false);

        FactorButton(root, page, suhu, "01", "SUHU", new Vector2(-340, 70), Red, null, "°C");
        FactorButton(root, page, udara, "02", "KEHADIRAN\nUDARA", new Vector2(340, 70), Blue, Air, "");
        FactorButton(root, page, nutrien, "03", "KEHADIRAN\nNUTRIEN", new Vector2(-340, -135), Orange, Nutrition, "");
        FactorButton(root, page, keasidan, "04", "KEASIDAN YANG\nSESUAI", new Vector2(340, -135), Purple, null, "pH");

        Image standard = Panel("StandardPanel", root, new Vector2(0, -405), new Vector2(490, 82), new Color(1, .97f, .84f, .97f), Orange, 3);
        NewText("Text", standard.transform, "SP: 3.1   |   SK: 3.1.4", 31, Ink, TextAlignmentOptions.Center, Vector2.zero, new Vector2(450, 68));

        BuildBackButton(root, page, center);
    }

    static void BuildHeader(Transform root)
    {
        Image badge = Panel("ActivityBadge", root, new Vector2(0, 485), new Vector2(360, 72), new Color32(214, 244, 165, 255), Green, 3);
        NewText("Label", badge.transform, "AKTIVITI 5", 43, Ink, TextAlignmentOptions.Center, Vector2.zero, new Vector2(330, 62));

        TextMeshProUGUI title = NewText("Title", root,
            "Faktor Mempengaruhi\nPertumbuhan Mikroorganisma", 68, Ink,
            TextAlignmentOptions.Center, new Vector2(0, 360), new Vector2(1250, 170));
        OutlineText(title, Color.white, 4);
        TextShadow(title, new Color(0, 0, 0, .28f), new Vector2(4, -6));

        Image instruction = Panel("Instruction", root, new Vector2(0, 235), new Vector2(1050, 96), new Color(1, .98f, .88f, .97f), Orange, 3);
        NewText("Text", instruction.transform,
            "Terokai faktor yang mempengaruhi pertumbuhan mikroorganisma.\nPilih faktor untuk mula meneroka!",
            31, Dark, TextAlignmentOptions.Center, Vector2.zero, new Vector2(990, 82));
    }

    static void FactorButton(Transform root, Transform page, Transform target, string number, string label,
        Vector2 position, Color color, string iconPath, string iconText)
    {
        GameObject go = new GameObject(label.Replace("\n", "") + "Button", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        go.transform.SetParent(root, false);
        SetRect(go.transform, position, new Vector2(590, 162));

        Image image = go.GetComponent<Image>();
        image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(White);
        image.type = Image.Type.Sliced;
        image.color = color;
        OutlineGraphic(go, Darker(color, .35f), 4);
        DropShadow(go);

        Button button = go.GetComponent<Button>();
        button.targetGraphic = image;
        SetButtonColors(button, color, Lighter(color, .13f), Darker(color, .13f), Color.gray);
        if (go.GetComponent<ButtonInteractionFeedback>() == null) go.AddComponent<ButtonInteractionFeedback>();
        UnityEventTools.AddBoolPersistentListener(button.onClick, page.gameObject.SetActive, false);
        UnityEventTools.AddBoolPersistentListener(button.onClick, target.gameObject.SetActive, true);

        Image numberCircle = NewImage("Number", go.transform, Circle, Lighter(color, .08f), new Vector2(-234, 0), new Vector2(145, 145), false);
        numberCircle.preserveAspect = true;
        OutlineGraphic(numberCircle.gameObject, Color.white, 4);
        NewText("Value", numberCircle.transform, number, 54, Color.white, TextAlignmentOptions.Center, Vector2.zero, new Vector2(120, 110));

        Image iconCircle = NewImage("IconCircle", go.transform, Circle, new Color(1, 1, 1, .92f), new Vector2(-112, 0), new Vector2(112, 112), false);
        iconCircle.preserveAspect = true;
        OutlineGraphic(iconCircle.gameObject, Color.white, 2);
        if (!string.IsNullOrEmpty(iconPath))
        {
            Image icon = NewImage("Icon", iconCircle.transform, iconPath, Color.white, Vector2.zero, new Vector2(88, 88), false);
            icon.preserveAspect = true;
        }
        if (!string.IsNullOrEmpty(iconText))
            NewText("IconText", iconCircle.transform, iconText, 40, color, TextAlignmentOptions.Center, Vector2.zero, new Vector2(88, 80));

        float labelSize = label.StartsWith("KEASIDAN", StringComparison.Ordinal) ? 31f : 36f;
        TextMeshProUGUI text = NewText("Label", go.transform, label, labelSize, Color.white,
            TextAlignmentOptions.Center, new Vector2(85, 0), new Vector2(240, 128));
        TextShadow(text, new Color(0, 0, 0, .3f), new Vector2(3, -4));

        Image arrowCircle = NewImage("ArrowCircle", go.transform, Circle, Lighter(color, .25f), new Vector2(250, 0), new Vector2(68, 68), false);
        arrowCircle.preserveAspect = true;
        OutlineGraphic(arrowCircle.gameObject, Color.white, 2);
        Image arrow = NewImage("Arrow", arrowCircle.transform, Arrow, Color.white, Vector2.zero, new Vector2(36, 36), false);
        arrow.preserveAspect = true;
        arrow.rectTransform.localRotation = Quaternion.Euler(0, 0, 180);
    }

    static void BuildBackButton(Transform root, Transform page, Transform center)
    {
        GameObject go = new GameObject("BackButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        go.transform.SetParent(root, false);
        SetRect(go.transform, new Vector2(-835, 490), new Vector2(215, 70));
        Image image = go.GetComponent<Image>();
        image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(White);
        image.type = Image.Type.Sliced;
        image.color = Cream;
        OutlineGraphic(go, Ink, 2);
        DropShadow(go);
        Button button = go.GetComponent<Button>();
        button.targetGraphic = image;
        SetButtonColors(button, Cream, Color.white, Darker(Cream, .08f), Color.gray);
        if (go.GetComponent<ButtonInteractionFeedback>() == null) go.AddComponent<ButtonInteractionFeedback>();
        UnityEventTools.AddBoolPersistentListener(button.onClick, page.gameObject.SetActive, false);
        UnityEventTools.AddBoolPersistentListener(button.onClick, center.gameObject.SetActive, true);

        Image circle = NewImage("Circle", go.transform, Circle, Green, new Vector2(-74, 0), new Vector2(60, 60), false);
        circle.preserveAspect = true;
        OutlineGraphic(circle.gameObject, Color.white, 2);
        Image arrow = NewImage("Arrow", circle.transform, Arrow, Color.white, Vector2.zero, new Vector2(32, 32), false);
        arrow.preserveAspect = true;
        NewText("Label", go.transform, "AKTIVITI", 22, Ink, TextAlignmentOptions.Center, new Vector2(26, 0), new Vector2(125, 54));
    }

    static Transform Require(Transform parent, string name)
    {
        Transform result = parent.Find(name);
        if (result == null) throw new InvalidOperationException("Canvas/" + name + " not found.");
        result.gameObject.SetActive(false);
        return result;
    }

    static void DecorativeImage(Transform parent, string name, string path, Vector2 position, Vector2 size)
    {
        Image image = NewImage(name, parent, path, Color.white, position, size, false);
        image.preserveAspect = true;
    }

    static Image Panel(string name, Transform parent, Vector2 position, Vector2 size, Color fill, Color border, float width)
    {
        Image image = NewImage(name, parent, White, fill, position, size, false);
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
        image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        image.color = color;
        image.raycastTarget = raycast;
        return image;
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
        text.enableWordWrapping = true;
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
    }

    static void OutlineGraphic(GameObject go, Color color, float width)
    {
        Outline outline = go.GetComponent<Outline>() ?? go.AddComponent<Outline>();
        outline.effectColor = color;
        outline.effectDistance = new Vector2(width, -width);
        outline.useGraphicAlpha = true;
    }

    static void OutlineText(TextMeshProUGUI text, Color color, float width) => OutlineGraphic(text.gameObject, color, width);

    static void DropShadow(GameObject go)
    {
        Shadow shadow = go.GetComponents<Shadow>().FirstOrDefault(component => !(component is Outline));
        if (shadow == null) shadow = go.AddComponent<Shadow>();
        shadow.effectColor = new Color(.15f, .08f, 0, .28f);
        shadow.effectDistance = new Vector2(0, -7);
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

    static Color Lighter(Color color, float amount) => new Color(
        Mathf.Lerp(color.r, 1, amount), Mathf.Lerp(color.g, 1, amount), Mathf.Lerp(color.b, 1, amount), color.a);

    static Color Darker(Color color, float amount) => new Color(
        color.r * (1 - amount), color.g * (1 - amount), color.b * (1 - amount), color.a);
}
