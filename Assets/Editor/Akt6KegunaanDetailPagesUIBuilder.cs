#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Builds a shared Activity 6 visual treatment for the four Kegunaan detail pages.
/// Legacy DialogController objects and serialized UnityEvents remain the functional layer.
/// </summary>
public static class Akt6KegunaanDetailPagesUIBuilder
{
    private const string ScenePath = "Assets/Scenes/Aktivity/akt6/akt6.unity";
    private const string GeneratedRootName = "Akt6KegunaanDetailReferenceRoot";
    private const string BackgroundPath = "Assets/Images/Akt6KegunaanDetailsUI/Shared/akt6_detail_shared_background.png";
    private const string PanelSpritePath = "Assets/Images/UI Flat/btn_white_flat.png";
    private const string FontPath = "Assets/Fonts/FredokaOne SDF.asset";
    private const string ArrowSpritePath = "Assets/Images/UI Flat/icon_play.png";
    private static TMP_FontAsset font;

    private sealed class PageConfig
    {
        public string section;
        public string title;
        public string subtitle;
        public string heroPath;
        public Color32 accent;
        public string backPath;
        public string[] dialogNames;

        public PageConfig(string section, string title, string subtitle, string heroPath,
            Color32 accent, string backPath, string[] dialogNames)
        {
            this.section = section;
            this.title = title;
            this.subtitle = subtitle;
            this.heroPath = heroPath;
            this.accent = accent;
            this.backPath = backPath;
            this.dialogNames = dialogNames;
        }
    }

    [MenuItem("Tools/MicroFun/Activity 6/Build Four Kegunaan Detail Pages")]
    public static void Rebuild()
    {
        if (EditorSceneManager.GetActiveScene().path != ScenePath)
        {
            Debug.LogError("Open " + ScenePath + " before rebuilding the Activity 6 detail pages.");
            return;
        }

        ConfigureSprite(BackgroundPath, false);
        Sprite background = AssetDatabase.LoadAssetAtPath<Sprite>(BackgroundPath);
        Sprite panelSprite = AssetDatabase.LoadAssetAtPath<Sprite>(PanelSpritePath);
        Sprite arrowSprite = AssetDatabase.LoadAssetAtPath<Sprite>(ArrowSpritePath);
        font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        if (background == null || panelSprite == null || arrowSprite == null || font == null)
        {
            Debug.LogError("Activity 6 detail-page background or shared UI sprites are missing.");
            return;
        }

        PageConfig[] pages =
        {
            new PageConfig(
                "Pembuatan Makanan",
                "Pembuatan Makanan",
                "Mikroorganisma membantu menghasilkan makanan melalui proses yang berguna.",
                "Assets/Images/2d/pembuatan makanan.jpg",
                new Color32(22, 155, 226, 255),
                "Bottom/HomeButton",
                new [] { "DialogParentRight Variant", "DialogParentLeft Variant", "DialogParentRight Variant (1)", "DialogParentLeft Variant (1)" }),
            new PageConfig(
                "Penghasilan Baja Organik",
                "Penghasilan Baja Organik",
                "Mikroorganisma menguraikan bahan organik untuk menyuburkan tanah.",
                "Assets/Images/2d/baa organik.png",
                new Color32(242, 67, 127, 255),
                "Bottom/HomeButton",
                new [] { "DialogParentRight Variant", "DialogParentLeft Variant", "DialogParentRight Variant (1)", "DialogParentLeft Variant (1)" }),
            new PageConfig(
                "Perawatan Sisa Kumbahan",
                "Perawatan Sisa Kumbahan",
                "Mikroorganisma membantu membersihkan air dan menguraikan bahan buangan.",
                "Assets/Images/2d/kumbahan.png",
                new Color32(9, 171, 166, 255),
                "Bottom/HomeButton",
                new [] { "DialogParentLeft Variant", "DialogParentRight Variant", "DialogParentRight Variant (1)" }),
            new PageConfig(
                "Penghasilan Ubat Ubatan",
                "Penghasilan Ubat-ubatan",
                "Mikroorganisma digunakan dalam penghasilan ubat untuk menjaga kesihatan.",
                "Assets/Images/2d/pill.jpg",
                new Color32(255, 157, 0, 255),
                "Back",
                new [] { "DialogParentRight", "DialogParentLeft", "DialogParentRight (1)", "DialogParentLeft (1)" })
        };

        Transform canvas = GameObject.Find("Canvas") != null ? GameObject.Find("Canvas").transform : null;
        if (canvas == null)
        {
            Debug.LogError("Canvas was not found.");
            return;
        }

        foreach (PageConfig page in pages)
        {
            Transform section = canvas.Find(page.section);
            if (section == null)
            {
                Debug.LogError("Missing section Canvas/" + page.section);
                return;
            }

            Button backSource = FindChild(section, page.backPath) != null
                ? FindChild(section, page.backPath).GetComponent<Button>()
                : null;
            Sprite hero = AssetDatabase.LoadAssetAtPath<Sprite>(page.heroPath);
            if (backSource == null || hero == null)
            {
                Debug.LogError("Missing Back callback or hero sprite for " + page.section);
                return;
            }

            BuildPage(section, page, background, panelSprite, arrowSprite, hero);
        }

        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        if (scaler != null)
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();
        Debug.Log("Activity 6: rebuilt all four Kegunaan detail pages with shared theme and preserved legacy interactions.");
    }

    private static void BuildPage(Transform section, PageConfig page, Sprite background,
        Sprite panelSprite, Sprite arrowSprite, Sprite hero)
    {
        Transform oldRoot = section.Find(GeneratedRootName);
        if (oldRoot != null)
            Object.DestroyImmediate(oldRoot.gameObject);

        GameObject root = CreateUI(GeneratedRootName, section);
        Stretch(root.GetComponent<RectTransform>());
        root.transform.SetSiblingIndex(0);

        Image bg = root.AddComponent<Image>();
        bg.sprite = background;
        bg.preserveAspect = false;
        bg.raycastTarget = false;

        BuildActivityBadge(root.transform, panelSprite);
        CreateText(root.transform, "PageTitle", page.title, new Vector2(0f, 355f), new Vector2(1500f, 135f),
            82f, new Color32(21, 93, 35, 255), FontStyles.Bold);
        TMP_Text subtitle = CreateText(root.transform, "Subtitle", page.subtitle, new Vector2(0f, 265f),
            new Vector2(1420f, 70f), 32f, new Color32(21, 78, 39, 255), FontStyles.Bold);
        subtitle.enableAutoSizing = true;
        subtitle.fontSizeMin = 24f;
        subtitle.fontSizeMax = 34f;

        GameObject heroPanel = CreatePanel(root.transform, "HeroPanel", new Vector2(-495f, -90f),
            new Vector2(660f, 590f), new Color32(255, 252, 238, 245), panelSprite, page.accent);
        GameObject heroObject = CreateUI("TopicIllustration", heroPanel.transform);
        RectTransform heroRect = heroObject.GetComponent<RectTransform>();
        heroRect.anchorMin = heroRect.anchorMax = new Vector2(0.5f, 0.5f);
        heroRect.anchoredPosition = new Vector2(0f, 25f);
        heroRect.sizeDelta = new Vector2(580f, 440f);
        Image heroImage = heroObject.AddComponent<Image>();
        heroImage.sprite = hero;
        heroImage.preserveAspect = true;
        heroImage.raycastTarget = false;

        GameObject topicPill = CreatePanel(heroPanel.transform, "TopicPill", new Vector2(0f, -245f),
            new Vector2(470f, 82f), page.accent, panelSprite, Color.white);
        TMP_Text topicText = CreateText(topicPill.transform, "Label", "KEGUNAAN MIKROORGANISMA",
            Vector2.zero, new Vector2(430f, 60f), 25f, Color.white, FontStyles.Bold);
        topicText.enableAutoSizing = true;
        topicText.fontSizeMin = 18f;
        topicText.fontSizeMax = 27f;

        CreatePanel(root.transform, "LessonPanel", new Vector2(385f, -95f),
            new Vector2(850f, 610f), new Color32(255, 252, 238, 247), panelSprite, page.accent);
        GameObject lessonPill = CreatePanel(root.transform, "LessonTitlePill", new Vector2(385f, 160f),
            new Vector2(390f, 78f), page.accent, panelSprite, Color.white);
        CreateText(lessonPill.transform, "Label", "PENERANGAN", Vector2.zero,
            new Vector2(350f, 56f), 28f, Color.white, FontStyles.Bold);



        BuildBackButton(root.transform, section, section.parent.Find("Kegunaan"), page.dialogNames, panelSprite, arrowSprite);

        HideLegacy(section, "Top");
        HideLegacy(section, "Center");
        HideLegacy(section, "Bottom");
        if (page.section == "Penghasilan Ubat Ubatan")
        {
            HideLegacy(section, "Back");
            HideLegacy(section, "Title");
            HideLegacy(section, "Content");
            HideLegacy(section, "Button");
            HideLegacy(section, "Button (1)");
            HideLegacy(section, "Button (2)");
            HideLegacy(section, "Button (4)");
        }

        Transform firstDialog = section.Find(page.dialogNames[0]);
        Transform kegunaanTarget = section.parent.Find("Kegunaan");
        for (int i = 0; i < page.dialogNames.Length; i++)
        {
            Transform dialog = section.Find(page.dialogNames[i]);
            Transform nextDialog = i + 1 < page.dialogNames.Length ? section.Find(page.dialogNames[i + 1]) : null;
            if (dialog != null)
                StyleDialog(dialog, nextDialog, firstDialog, section.gameObject, kegunaanTarget.gameObject,
                    i + 1, page.dialogNames.Length, page.accent, panelSprite);
        }

        EditorUtility.SetDirty(section.gameObject);
    }

    private static void BuildActivityBadge(Transform parent, Sprite panelSprite)
    {
        GameObject badge = CreatePanel(parent, "ActivityBadge", new Vector2(0f, 475f),
            new Vector2(390f, 82f), new Color32(64, 153, 20, 255), panelSprite, Color.white);
        CreateText(badge.transform, "Label", "AKTIVITI 6", Vector2.zero,
            new Vector2(350f, 62f), 38f, Color.white, FontStyles.Bold);
    }

    private static void StyleDialog(Transform dialog, Transform nextDialog, Transform firstDialog,
        GameObject currentPage, GameObject targetPage, int step, int total, Color32 accent, Sprite panelSprite)
    {
        RectTransform dialogRect = dialog as RectTransform;
        if (dialogRect != null) Stretch(dialogRect);

        Transform bg = dialog.Find("bg");
        if (bg != null)
        {
            Image image = bg.GetComponent<Image>();
            if (image != null) image.enabled = false;
        }

        Transform person = dialog.Find("Person");
        if (person != null)
        {
            Image image = person.GetComponent<Image>();
            if (image != null) image.enabled = false;
        }

        DialogController controller = dialog.GetComponent<DialogController>();
        string lesson = "";
        if (controller != null)
        {
            SerializedObject serializedController = new SerializedObject(controller);
            SerializedProperty list = serializedController.FindProperty("dialogList");
            if (list != null && list.isArray && list.arraySize > 0)
                lesson = list.GetArrayElementAtIndex(0).stringValue;
        }

        Transform textDialog = dialog.Find("textDialog");
        if (textDialog != null)
        {
            CanvasGroup textGroup = textDialog.GetComponent<CanvasGroup>();
            if (textGroup == null) textGroup = textDialog.gameObject.AddComponent<CanvasGroup>();
            textGroup.alpha = 0f;
            textGroup.interactable = false;
            textGroup.blocksRaycasts = false;
        }

        Transform exit = dialog.Find("ExitButton");
        if (exit != null)
        {
            CanvasGroup exitGroup = exit.GetComponent<CanvasGroup>();
            if (exitGroup == null) exitGroup = exit.gameObject.AddComponent<CanvasGroup>();
            exitGroup.alpha = 0f;
            exitGroup.interactable = false;
            exitGroup.blocksRaycasts = false;
        }

        Transform oldCopy = dialog.Find("Akt6LessonCopy");
        if (oldCopy != null) Object.DestroyImmediate(oldCopy.gameObject);
        TMP_Text copy = CreateText(dialog, "Akt6LessonCopy", lesson,
            new Vector2(385f, -80f), new Vector2(690f, 300f), 40f,
            new Color32(20, 79, 40, 255), FontStyles.Bold);
        copy.enableAutoSizing = true;
        copy.fontSizeMin = 27f;
        copy.fontSizeMax = 42f;
        copy.margin = new Vector4(24f, 18f, 24f, 18f);

        Transform oldContinue = dialog.Find("Akt6ContinueButton");
        if (oldContinue != null) Object.DestroyImmediate(oldContinue.gameObject);

        GameObject continueObject = CreateUI("Akt6ContinueButton", dialog);
        RectTransform continueRect = continueObject.GetComponent<RectTransform>();
        continueRect.anchorMin = continueRect.anchorMax = new Vector2(0.5f, 0.5f);
        continueRect.pivot = new Vector2(0.5f, 0.5f);
        continueRect.anchoredPosition = new Vector2(385f, -305f);
        continueRect.sizeDelta = new Vector2(350f, 82f);

        Image continueImage = continueObject.AddComponent<Image>();
        continueImage.sprite = panelSprite;
        continueImage.type = Image.Type.Sliced;
        continueImage.color = accent;

        Shadow continueShadow = continueObject.AddComponent<Shadow>();
        continueShadow.effectColor = new Color32(32, 65, 34, 120);
        continueShadow.effectDistance = new Vector2(0f, -6f);

        Outline continueOutline = continueObject.AddComponent<Outline>();
        continueOutline.effectColor = Color.white;
        continueOutline.effectDistance = new Vector2(3f, 3f);

        Button continueButton = continueObject.AddComponent<Button>();
        continueButton.targetGraphic = continueImage;
        continueButton.navigation = new Navigation { mode = Navigation.Mode.None };

        string continueLabel = step < total ? "SETERUSNYA" : "SELESAI";
        CreateText(continueObject.transform, "Label", continueLabel,
            Vector2.zero, new Vector2(310f, 60f), 28f, Color.white, FontStyles.Bold);

        if (nextDialog != null)
        {
            UnityEventTools.AddBoolPersistentListener(continueButton.onClick, dialog.gameObject.SetActive, false);
            UnityEventTools.AddBoolPersistentListener(continueButton.onClick, nextDialog.gameObject.SetActive, true);
        }
        else
        {
            UnityEventTools.AddBoolPersistentListener(continueButton.onClick, dialog.gameObject.SetActive, false);
            UnityEventTools.AddBoolPersistentListener(continueButton.onClick, firstDialog.gameObject.SetActive, true);
            UnityEventTools.AddBoolPersistentListener(continueButton.onClick, currentPage.SetActive, false);
            UnityEventTools.AddBoolPersistentListener(continueButton.onClick, targetPage.SetActive, true);
        }

        Transform oldStep = dialog.Find("Akt6StepBadge");
        if (oldStep != null) Object.DestroyImmediate(oldStep.gameObject);
        GameObject stepBadge = CreatePanel(dialog, "Akt6StepBadge", new Vector2(385f, 92f),
            new Vector2(250f, 58f), new Color32(255, 255, 255, 245), panelSprite, accent);
        CreateText(stepBadge.transform, "Label", "LANGKAH " + step + " / " + total, Vector2.zero,
            new Vector2(220f, 44f), 22f, new Color32(27, 91, 43, 255), FontStyles.Bold);
    }

    private static void StyleButton(Transform target, Color32 accent, Sprite panelSprite, string label, float size)
    {
        Image image = target.GetComponent<Image>();
        if (image != null)
        {
            image.enabled = true;
            image.sprite = panelSprite;
            image.type = Image.Type.Sliced;
            image.color = accent;
            image.raycastTarget = true;
        }

        Button button = target.GetComponent<Button>();
        if (button != null)
        {
            button.targetGraphic = image;
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1f, 1f, 1f, 0.88f);
            colors.pressedColor = new Color(0.82f, 0.82f, 0.82f, 1f);
            colors.selectedColor = Color.white;
            button.colors = colors;
            button.navigation = new Navigation { mode = Navigation.Mode.None };
        }

        TMP_Text text = target.GetComponentInChildren<TMP_Text>(true);
        if (text != null)
        {
            text.text = label;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Center;
            text.fontStyle = FontStyles.Bold;
            text.fontSize = size;
            text.enableAutoSizing = true;
            text.fontSizeMin = Mathf.Max(18f, size - 10f);
            text.fontSizeMax = size;
            text.outlineColor = new Color32(20, 72, 30, 180);
            text.outlineWidth = 0.18f;
            Stretch(text.rectTransform);
        }
    }

    private static void BuildBackButton(Transform parent, Transform currentPage, Transform targetPage,
        string[] dialogNames, Sprite panelSprite, Sprite arrowSprite)
    {
        GameObject go = CreateUI("BackButton", parent);
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(-900f, 470f);
        rect.sizeDelta = new Vector2(86f, 86f);

        Image panel = go.AddComponent<Image>();
        panel.sprite = panelSprite;
        panel.type = Image.Type.Sliced;
        panel.color = new Color32(28, 158, 225, 255);

        Shadow shadow = go.AddComponent<Shadow>();
        shadow.effectColor = new Color32(9, 62, 105, 145);
        shadow.effectDistance = new Vector2(0f, -6f);

        Outline outline = go.AddComponent<Outline>();
        outline.effectColor = Color.white;
        outline.effectDistance = new Vector2(3f, 3f);

        Button button = go.AddComponent<Button>();
        button.targetGraphic = panel;
        button.navigation = new Navigation { mode = Navigation.Mode.None };

        GameObject arrowObject = CreateUI("BackArrow", go.transform);
        RectTransform arrowRect = arrowObject.GetComponent<RectTransform>();
        arrowRect.anchorMin = arrowRect.anchorMax = new Vector2(0.5f, 0.5f);
        arrowRect.pivot = new Vector2(0.5f, 0.5f);
        arrowRect.anchoredPosition = Vector2.zero;
        arrowRect.sizeDelta = new Vector2(46f, 46f);
        arrowRect.localScale = new Vector3(-1f, 1f, 1f);

        Image arrow = arrowObject.AddComponent<Image>();
        arrow.sprite = arrowSprite;
        arrow.preserveAspect = true;
        arrow.color = Color.white;
        arrow.raycastTarget = false;

        for (int i = 0; i < dialogNames.Length; i++)
        {
            Transform dialog = currentPage.Find(dialogNames[i]);
            if (dialog != null)
                UnityEventTools.AddBoolPersistentListener(button.onClick, dialog.gameObject.SetActive, i == 0);
        }
        UnityEventTools.AddBoolPersistentListener(button.onClick, currentPage.gameObject.SetActive, false);
        UnityEventTools.AddBoolPersistentListener(button.onClick, targetPage.gameObject.SetActive, true);
    }

    private static void AttachRelay(GameObject owner, Button visibleButton, Button source)
    {
        CenterMenuButtonRelay relay = owner.AddComponent<CenterMenuButtonRelay>();
        SerializedObject serializedRelay = new SerializedObject(relay);
        serializedRelay.FindProperty("sourceButton").objectReferenceValue = source;
        serializedRelay.ApplyModifiedPropertiesWithoutUndo();
        UnityEventTools.AddPersistentListener(visibleButton.onClick, relay.InvokeSource);
    }

    private static void HideLegacy(Transform section, string path)
    {
        Transform target = section.Find(path);
        if (target == null) return;
        CanvasGroup group = target.GetComponent<CanvasGroup>();
        if (group == null) group = target.gameObject.AddComponent<CanvasGroup>();
        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;
    }

    private static Transform FindChild(Transform root, string path)
    {
        return root != null ? root.Find(path) : null;
    }

    private static GameObject CreatePanel(Transform parent, string name, Vector2 position,
        Vector2 size, Color color, Sprite sprite, Color outlineColor)
    {
        GameObject go = CreateUI(name, parent);
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Image image = go.AddComponent<Image>();
        image.sprite = sprite;
        image.type = Image.Type.Sliced;
        image.color = color;
        image.raycastTarget = false;

        Shadow shadow = go.AddComponent<Shadow>();
        shadow.effectColor = new Color32(62, 73, 41, 80);
        shadow.effectDistance = new Vector2(0f, -7f);

        Outline outline = go.AddComponent<Outline>();
        outline.effectColor = outlineColor;
        outline.effectDistance = new Vector2(3f, 3f);
        return go;
    }

    private static TMP_Text CreateText(Transform parent, string name, string value,
        Vector2 position, Vector2 size, float fontSize, Color color, FontStyles style)
    {
        GameObject go = CreateUI(name, parent);
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        TextMeshProUGUI text = go.AddComponent<TextMeshProUGUI>();
        text.text = value;
        text.font = font != null ? font : TMP_Settings.defaultFontAsset;
        text.fontSize = fontSize;
        text.color = color;
        text.fontStyle = style;
        text.alignment = TextAlignmentOptions.Center;
        text.enableWordWrapping = true;
        text.raycastTarget = false;
        text.outlineColor = new Color32(255, 255, 255, 210);
        text.outlineWidth = 0.12f;
        return text;
    }

    private static GameObject CreateUI(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.layer = parent.gameObject.layer;
        go.transform.SetParent(parent, false);
        return go;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;
    }

    private static void ConfigureSprite(string path, bool alpha)
    {
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) return;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = alpha;
        importer.mipmapEnabled = false;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.filterMode = FilterMode.Bilinear;
        importer.maxTextureSize = 2048;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.SaveAndReimport();
    }
}
#endif
