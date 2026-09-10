using System;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Akt5KehadiranUdaraUIBuilder
{
    const string ScenePath = "Assets/Scenes/Aktivity/akt5/akt5.unity";
    const string BackgroundPath = "Assets/Images/Akt5CenterUI/Background/akt5_factor_background_v2.png";
    const string VacuumApplePath = "Assets/Images/2d/apple vacuum.jpg";
    const string NoVacuumApplePath = "Assets/Images/Akt5KehadiranUdaraUI/apple_slices_no_vacuum_v1.png";
    const string StudentOnePath = "Assets/Images/2d/student1.png";
    const string StudentTwoPath = "Assets/Images/2d/student2.png";
    const string AsparagusPath = "Assets/Images/2d/asparagus.png";
    const string WhitePath = "Assets/Images/UI Flat/btn_white_flat.png";
    const string CirclePath = "Assets/Images/UI Flat/ui_circle_white.png";
    const string BookPath = "Assets/Images/UI Flat/icon_book_open.png";
    const string FlaskPath = "Assets/Images/UI Flat/icon_flask.png";
    const string LightbulbPath = "Assets/Images/UI Flat/icon_lightbulb.png";
    const string ArrowPath = "Assets/Images/UI Flat/icon_arrow_left.png";
    const string TickPath = "Assets/Images/UI Flat 1/tick.png";
    const string FontPath = "Assets/Fonts/FredokaOne SDF.asset";

    static readonly Color Ink = new Color32(8, 70, 30, 255);
    static readonly Color Dark = new Color32(35, 48, 28, 255);
    static readonly Color DarkGreen = new Color32(18, 105, 14, 255);
    static readonly Color Green = new Color32(83, 184, 20, 255);
    static readonly Color Blue = new Color32(42, 145, 225, 255);
    static readonly Color Pink = new Color32(239, 75, 135, 255);
    static readonly Color Orange = new Color32(244, 119, 36, 255);
    static readonly Color Purple = new Color32(127, 64, 202, 255);
    static readonly Color Cream = new Color32(255, 250, 231, 255);
    static readonly Color LightGreen = new Color32(235, 250, 214, 255);
    static TMP_FontAsset font;

    [MenuItem("Tools/MicroFun/Activity 5/Build Kehadiran Udara Theme")]
    public static void Build()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogError("[Akt5KehadiranUdara] Exit Play Mode first.");
            return;
        }

        font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        if (font == null)
        {
            Debug.LogError("[Akt5KehadiranUdara] Fredoka One font missing.");
            return;
        }

        string originalScene = SceneManager.GetActiveScene().path;
        BackupScene();

        try
        {
            ConfigureGeneratedSprite();
            Scene scene = SceneManager.GetActiveScene().path == ScenePath
                ? SceneManager.GetActiveScene()
                : EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            GameObject canvasObject = GameObject.Find("Canvas");
            if (canvasObject == null) throw new InvalidOperationException("Canvas not found.");

            Transform canvas = canvasObject.transform;
            Transform page = canvas.Find("Kehadiran Udara");
            Transform faktor = canvas.Find("Faktor");
            if (page == null || faktor == null)
                throw new InvalidOperationException("Canvas/Kehadiran Udara or Canvas/Faktor not found.");

            ConfigureCanvas(canvasObject.GetComponent<Canvas>());
            BuildPage(page, faktor);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[Akt5KehadiranUdara] Reference-themed air-presence lesson built successfully.");
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            throw;
        }
        finally
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            if (!string.IsNullOrEmpty(originalScene) && originalScene != ScenePath)
                EditorSceneManager.OpenScene(originalScene, OpenSceneMode.Single);
        }
    }

    static void BackupScene()
    {
        const string parent = "Assets/Scenes/Aktivity/akt5";
        const string backupFolder = parent + "/Backups";
        if (!AssetDatabase.IsValidFolder(backupFolder)) AssetDatabase.CreateFolder(parent, "Backups");

        const string backupPath = backupFolder + "/akt5_PreKehadiranUdaraRedesign_20260902.unity";
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(backupPath) == null)
            AssetDatabase.CopyAsset(ScenePath, backupPath);
    }

    static void ConfigureGeneratedSprite()
    {
        AssetDatabase.ImportAsset(NoVacuumApplePath, ImportAssetOptions.ForceUpdate);
        TextureImporter importer = AssetImporter.GetAtPath(NoVacuumApplePath) as TextureImporter;
        if (importer == null) throw new InvalidOperationException("Generated no-vacuum apple asset could not be imported.");

        bool changed = importer.textureType != TextureImporterType.Sprite ||
                       importer.spriteImportMode != SpriteImportMode.Single ||
                       !importer.alphaIsTransparency || importer.mipmapEnabled;

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.maxTextureSize = 2048;
        importer.textureCompression = TextureImporterCompression.Compressed;
        if (changed) importer.SaveAndReimport();
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
        Delete(page, "Akt5KehadiranUdaraThemeRoot");
        RectTransform root = NewRect("Akt5KehadiranUdaraThemeRoot", page);
        Full(root);

        foreach (Transform child in page)
            if (child != root) child.gameObject.SetActive(false);

        Image background = NewImage("Background", root, BackgroundPath, Color.white, Vector2.zero, new Vector2(1920, 1080), false);
        background.preserveAspect = false;
        Full(background.rectTransform);

        BuildHeader(root);
        BuildDiscussion(root);
        BuildExperiment(root);
        BuildPrompt(root);
        BuildBackButton(root, page, faktor);

        root.SetAsLastSibling();
        page.gameObject.SetActive(false);
        faktor.gameObject.SetActive(true);
    }

    static void BuildHeader(Transform root)
    {
        Image badge = Panel("ActivityBadge", root, new Vector2(0, 498), new Vector2(350, 66),
            new Color32(217, 245, 167, 255), Green, 3);
        NewText("Label", badge.transform, "AKTIVITI 5", 40, Ink, TextAlignmentOptions.Center,
            Vector2.zero, new Vector2(324, 58));

        Image title = Panel("TitleBanner", root, new Vector2(250, 402), new Vector2(1090, 108),
            new Color32(66, 171, 10, 255), DarkGreen, 4);
        Image iconBadge = NewImage("BookBadge", title.transform, CirclePath, new Color32(65, 157, 7, 255),
            new Vector2(-478, 0), new Vector2(126, 126), false);
        iconBadge.preserveAspect = true;
        OutlineGraphic(iconBadge.gameObject, Color.white, 4);
        DropShadow(iconBadge.gameObject);
        Image book = NewImage("Book", iconBadge.transform, BookPath, Color.white, Vector2.zero, new Vector2(90, 90), false);
        book.preserveAspect = true;

        TextMeshProUGUI titleText = NewText("Label", title.transform, "KEHADIRAN UDARA", 56, Color.white,
            TextAlignmentOptions.Center, new Vector2(48, 0), new Vector2(900, 94));
        TextShadow(titleText, new Color(0, 0, 0, .32f), new Vector2(4, -5));
    }

    static void BuildDiscussion(Transform root)
    {
        Image question = SpeechPanel("QuestionCard", root, new Vector2(-655, 215), new Vector2(510, 190), Blue);
        Image questionIcon = NewImage("QuestionIcon", question.transform, CirclePath, Blue,
            new Vector2(-228, 73), new Vector2(66, 66), false);
        questionIcon.preserveAspect = true;
        OutlineGraphic(questionIcon.gameObject, Color.white, 2);
        NewText("Mark", questionIcon.transform, "?", 40, Color.white, TextAlignmentOptions.Center,
            Vector2.zero, new Vector2(52, 52));
        TextMeshProUGUI questionText = NewText("Text", question.transform,
            "Sayur asparagus ini dipek di dalam <color=#6B35C8>pek vakum</color>.\nAdakah sayur-sayuran yang dipek vakum tahan lebih lama?",
            24, Dark, TextAlignmentOptions.MidlineLeft, new Vector2(22, -4), new Vector2(430, 158));
        questionText.richText = true;
        Image asparagus = NewImage("Asparagus", question.transform, AsparagusPath, Color.white,
            new Vector2(210, 62), new Vector2(80, 80), false);
        asparagus.preserveAspect = true;

        Image studentOne = NewImage("StudentOne", root, StudentOnePath, Color.white,
            new Vector2(-905, 205), new Vector2(170, 245), false);
        studentOne.preserveAspect = true;

        Image answer = SpeechPanel("AnswerCard", root, new Vector2(-665, -2), new Vector2(480, 150), Green);
        Image check = NewImage("CheckIcon", answer.transform, CirclePath, Green,
            new Vector2(-216, 58), new Vector2(62, 62), false);
        check.preserveAspect = true;
        OutlineGraphic(check.gameObject, Color.white, 2);
        Image answerTick = NewImage("Tick", check.transform, TickPath, Color.white,
            Vector2.zero, new Vector2(38, 38), false);
        answerTick.preserveAspect = true;
        TextMeshProUGUI answerText = NewText("Text", answer.transform,
            "Ya! Sayur-sayuran yang dipek vakum boleh\n<color=#6B35C8>tahan lebih lama</color>.",
            24, Dark, TextAlignmentOptions.MidlineLeft, new Vector2(25, 0), new Vector2(390, 122));
        answerText.richText = true;

        Image studentTwo = NewImage("StudentTwo", root, StudentTwoPath, Color.white,
            new Vector2(-910, -47), new Vector2(155, 230), false);
        studentTwo.preserveAspect = true;

        Image explanation = SpeechPanel("ExplanationCard", root, new Vector2(-665, -192), new Vector2(480, 180), Green);
        TextMeshProUGUI explanationText = NewText("Text", explanation.transform,
            "Ini kerana <color=#167A35>mikroorganisma</color> tidak dapat tumbuh jika\n<color=#D52E2E>tiada udara</color> dalam pek vakum.",
            24, Dark, TextAlignmentOptions.MidlineLeft, new Vector2(8, 0), new Vector2(420, 148));
        explanationText.richText = true;
    }

    static Image SpeechPanel(string name, Transform parent, Vector2 position, Vector2 size, Color border)
    {
        return Panel(name, parent, position, size, new Color(1, .99f, .94f, .97f), border, 3);
    }

    static void BuildExperiment(Transform root)
    {
        Image experiment = Panel("ExperimentPanel", root, new Vector2(310, -32), new Vector2(1125, 704),
            new Color(1, .985f, .93f, .96f), Orange, 3);

        Image tab = Panel("ExperimentTab", experiment.transform, new Vector2(-280, 320), new Vector2(420, 72),
            Pink, new Color32(198, 43, 104, 255), 3);
        Image flaskCircle = NewImage("FlaskCircle", tab.transform, CirclePath, new Color32(246, 105, 158, 255),
            new Vector2(-174, 0), new Vector2(78, 78), false);
        flaskCircle.preserveAspect = true;
        OutlineGraphic(flaskCircle.gameObject, Color.white, 3);
        Image flask = NewImage("Flask", flaskCircle.transform, FlaskPath, Color.white, Vector2.zero, new Vector2(55, 55), false);
        flask.preserveAspect = true;
        NewText("Label", tab.transform, "Eksperimen Mudah", 31, Color.white, TextAlignmentOptions.Center,
            new Vector2(30, 0), new Vector2(330, 60));

        TextMeshProUGUI instruction = NewText("Instruction", experiment.transform,
            "Bandingkan hirisan epal yang disimpan <color=#3B4CC0>dengan</color> dan <color=#E63B25>tanpa</color> vakum.",
            25, Dark, TextAlignmentOptions.Center, new Vector2(0, 255), new Vector2(980, 58));
        instruction.richText = true;

        BuildComparisonCard(experiment.transform, true);
        BuildComparisonCard(experiment.transform, false);

        Image divider = NewImage("Divider", experiment.transform, WhitePath, new Color32(237, 141, 47, 180),
            new Vector2(0, -32), new Vector2(3, 470), false);
        divider.type = Image.Type.Sliced;
        TextMeshProUGUI versus = NewText("Versus", experiment.transform, "VS", 55, new Color32(25, 80, 174, 255),
            TextAlignmentOptions.Center, new Vector2(0, 8), new Vector2(100, 82));
        OutlineGraphic(versus.gameObject, Color.white, 3);
        TextShadow(versus, new Color(0, 0, 0, .25f), new Vector2(3, -4));
    }

    static void BuildComparisonCard(Transform parent, bool vacuum)
    {
        Vector2 cardPosition = vacuum ? new Vector2(-278, -35) : new Vector2(278, -35);
        Color accent = vacuum ? Purple : Orange;
        Image card = Panel(vacuum ? "VacuumCard" : "NoVacuumCard", parent, cardPosition, new Vector2(500, 500),
            new Color(1, 1, 1, .94f), accent, 3);

        Image label = Panel("Label", card.transform, new Vector2(0, 225), new Vector2(250, 58),
            vacuum ? new Color32(126, 69, 198, 255) : new Color32(242, 91, 28, 255), accent, 2);
        NewText("Text", label.transform, vacuum ? "Pek vakum" : "Tanpa vakum", 28, Color.white,
            TextAlignmentOptions.Center, Vector2.zero, new Vector2(225, 50));

        Image photoFrame = Panel("PhotoFrame", card.transform, new Vector2(0, 78), new Vector2(445, 230),
            new Color(1, 1, 1, .96f), new Color(1, 1, 1, .9f), 2);
        Image photo = NewImage("Photo", photoFrame.transform, vacuum ? VacuumApplePath : NoVacuumApplePath,
            Color.white, Vector2.zero, vacuum ? new Vector2(425, 208) : new Vector2(410, 215), false);
        photo.preserveAspect = true;

        string caption = vacuum
            ? "Hirisan epal dalam pek vakum kekal <color=#6B35C8>segar</color> lebih lama kerana <color=#D52E2E>tiada udara</color> untuk mikroorganisma tumbuh."
            : "Hirisan epal <color=#D52E2E>tanpa</color> vakum cepat menjadi perang dan rosak kerana mikroorganisma boleh tumbuh dengan adanya <color=#2456D4>udara</color>.";
        TextMeshProUGUI captionText = NewText("Caption", card.transform, caption, 21, Dark,
            TextAlignmentOptions.TopLeft, new Vector2(0, -127), new Vector2(430, 190));
        captionText.richText = true;

        Image resultBadge = NewImage("ResultBadge", card.transform, CirclePath,
            vacuum ? Green : new Color32(225, 47, 70, 255), new Vector2(205, -214), new Vector2(70, 70), false);
        resultBadge.preserveAspect = true;
        OutlineGraphic(resultBadge.gameObject, Color.white, 3);
        DropShadow(resultBadge.gameObject);
        if (vacuum)
        {
            Image tick = NewImage("Tick", resultBadge.transform, TickPath, Color.white,
                Vector2.zero, new Vector2(44, 44), false);
            tick.preserveAspect = true;
        }
        else
        {
            Image slashA = NewImage("CrossA", resultBadge.transform, WhitePath, Color.white,
                Vector2.zero, new Vector2(9, 39), false);
            slashA.rectTransform.localRotation = Quaternion.Euler(0, 0, 45);
            Image slashB = NewImage("CrossB", resultBadge.transform, WhitePath, Color.white,
                Vector2.zero, new Vector2(9, 39), false);
            slashB.rectTransform.localRotation = Quaternion.Euler(0, 0, -45);
        }
    }

    static void BuildPrompt(Transform root)
    {
        Image panel = Panel("ReflectionPrompt", root, new Vector2(155, -482), new Vector2(1160, 92),
            new Color(1, .98f, .86f, .98f), Orange, 3);
        Image iconCircle = NewImage("IconCircle", panel.transform, CirclePath, new Color32(255, 202, 43, 255),
            new Vector2(-525, 0), new Vector2(78, 78), false);
        iconCircle.preserveAspect = true;
        OutlineGraphic(iconCircle.gameObject, Color.white, 3);
        Image bulb = NewImage("Lightbulb", iconCircle.transform, LightbulbPath, Color.white,
            Vector2.zero, new Vector2(58, 58), false);
        bulb.preserveAspect = true;

        TextMeshProUGUI text = NewText("Text", panel.transform,
            "Bagaimanakah kehadiran <color=#315EEA>udara</color> mempengaruhi pertumbuhan <color=#167A35>mikroorganisma</color>?",
            28, Ink, TextAlignmentOptions.Center, new Vector2(35, 0), new Vector2(1000, 74));
        text.richText = true;
    }

    static void BuildBackButton(Transform root, Transform page, Transform faktor)
    {
        GameObject go = new GameObject("BackButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        go.transform.SetParent(root, false);
        SetRect(go.transform, new Vector2(-842, 492), new Vector2(205, 66));

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

        Image circle = NewImage("Circle", go.transform, CirclePath, Green, new Vector2(-70, 0), new Vector2(57, 57), false);
        circle.preserveAspect = true;
        OutlineGraphic(circle.gameObject, Color.white, 2);
        Image arrow = NewImage("Arrow", circle.transform, ArrowPath, Color.white, Vector2.zero, new Vector2(30, 30), false);
        arrow.preserveAspect = true;
        NewText("Label", go.transform, "FAKTOR", 22, Ink, TextAlignmentOptions.Center,
            new Vector2(28, 0), new Vector2(118, 52));
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
        if (sprite == null) Debug.LogWarning("[Akt5KehadiranUdara] Sprite missing: " + path);
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
        shadow.effectColor = new Color(.15f, .08f, 0, .27f);
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

    static Color Darker(Color color, float amount)
    {
        return new Color(color.r * (1 - amount), color.g * (1 - amount), color.b * (1 - amount), color.a);
    }
}
