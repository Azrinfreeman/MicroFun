#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class MicroSquadUIBuilder
{
    private const string BackgroundPath = "Assets/Images/MicroSquadUI/Background/microsquad_background.png";
    private const string WheelPath = "Assets/Images/MicroSquadUI/Wheel/microsquad_wheel.png";
    private const string RoundedSpritePath = "Assets/Images/UI Flat/btn_white_flat.png";
    private const string FontPath = "Assets/Fonts/FredokaOne SDF.asset";

    [MenuItem("Tools/MicroFun/Build Micro Squad UI")]
    public static void Build()
    {
        GameObject microsquad = GameObject.Find("Canvas/Microsquad");
        if (microsquad == null)
        {
            Debug.LogError("Micro Squad UI Builder: Canvas/Microsquad was not found in the active scene.");
            return;
        }

        ConfigureSprite(BackgroundPath, 2048);
        ConfigureSprite(WheelPath, 2048);

        Sprite backgroundSprite = AssetDatabase.LoadAssetAtPath<Sprite>(BackgroundPath);
        Sprite wheelSprite = AssetDatabase.LoadAssetAtPath<Sprite>(WheelPath);
        Sprite roundedSprite = AssetDatabase.LoadAssetAtPath<Sprite>(RoundedSpritePath);
        TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);

        if (backgroundSprite == null || wheelSprite == null || roundedSprite == null || font == null)
        {
            Debug.LogError("Micro Squad UI Builder: one or more required assets could not be loaded.");
            return;
        }

        Transform oldRoot = microsquad.transform.Find("RedesignRoot");
        if (oldRoot != null)
            Object.DestroyImmediate(oldRoot.gameObject);

        SetDirectChildActive(microsquad.transform, "Center", false);
        SetDirectChildActive(microsquad.transform, "Button", false);

        RectTransform root = CreateRect("RedesignRoot", microsquad.transform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        root.SetAsFirstSibling();

        Image background = CreateImage("Background", root, backgroundSprite, Color.white, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        background.preserveAspect = false;

        BuildHeader(root, roundedSprite, font);
        BuildInstructions(root, roundedSprite, font);
        BuildWheel(root, wheelSprite, roundedSprite, font);
        RestyleNavigation(microsquad.transform, roundedSprite);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        Selection.activeGameObject = root.gameObject;
        Debug.Log("Micro Squad UI Builder: redesign created and scene saved.");
    }

    private static void BuildHeader(RectTransform root, Sprite roundedSprite, TMP_FontAsset font)
    {
        Image badge = CreatePanel("ActivityBadge", root, roundedSprite, new Color32(255, 249, 218, 248),
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -26f), new Vector2(280f, 68f));
        AddOutline(badge.gameObject, new Color32(255, 255, 255, 230), 3f);
        AddShadow(badge.gameObject, new Color32(88, 67, 25, 80), new Vector2(0f, -7f));
        CreateText("Label", badge.rectTransform, "AKTIVITI 8", font, 38f, new Color32(24, 105, 35, 255), TextAlignmentOptions.Center,
            Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        TextMeshProUGUI title = CreateText("Title", root, "Micro Squad", font, 104f, new Color32(32, 142, 18, 255), TextAlignmentOptions.Center,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -93f), new Vector2(980f, 136f));
        title.enableAutoSizing = true;
        title.fontSizeMin = 72f;
        title.fontSizeMax = 104f;
        title.outlineColor = new Color32(255, 255, 255, 255);
        title.outlineWidth = 0.22f;
        AddShadow(title.gameObject, new Color32(36, 72, 10, 95), new Vector2(0f, -8f));

        TextMeshProUGUI subtitle = CreateText("Subtitle", root,
            "Putar roda untuk memilih mikroorganisma kumpulan anda.", font, 31f, new Color32(21, 87, 41, 255), TextAlignmentOptions.Center,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -223f), new Vector2(1050f, 48f));
        subtitle.enableAutoSizing = true;
        subtitle.fontSizeMin = 24f;
        subtitle.fontSizeMax = 31f;
    }

    private static void BuildInstructions(RectTransform root, Sprite roundedSprite, TMP_FontAsset font)
    {
        Image card = CreatePanel("InstructionsCard", root, roundedSprite, new Color32(255, 252, 237, 235),
            new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(175f, -18f), new Vector2(660f, 500f));
        AddOutline(card.gameObject, new Color32(255, 255, 255, 245), 4f);
        AddShadow(card.gameObject, new Color32(88, 68, 29, 75), new Vector2(0f, -8f));

        Image cardHeader = CreatePanel("Header", card.rectTransform, roundedSprite, new Color32(255, 249, 216, 255),
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -34f), new Vector2(330f, 74f));
        AddOutline(cardHeader.gameObject, new Color32(102, 174, 43, 255), 3f);
        CreateText("Icon", cardHeader.rectTransform, "!", font, 40f, new Color32(20, 121, 58, 255), TextAlignmentOptions.Center,
            new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(0f, 0.5f), new Vector2(32f, 0f), new Vector2(70f, 70f));
        CreateText("Text", cardHeader.rectTransform, "Arahan", font, 41f, new Color32(22, 92, 42, 255), TextAlignmentOptions.Center,
            new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0.5f, 0.5f), new Vector2(28f, 0f), new Vector2(-70f, -8f));

        CreateStep(card.rectTransform, roundedSprite, font, "1", "Klik butang <color=#138A2E>PUTAR</color>\nuntuk memilih satu\nmikroorganisma.", new Vector2(0f, -142f));

        Image divider = CreateImage("Divider", card.rectTransform, null, new Color32(48, 146, 61, 220),
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -292f), new Vector2(520f, 4f));
        divider.type = Image.Type.Simple;

        CreateStep(card.rectTransform, roundedSprite, font, "2", "Setiap kumpulan akan\nmendapat <color=#138A2E>1 mikroorganisma</color>.", new Vector2(0f, -376f));

        Image collaboration = CreatePanel("CollaborationBadge", root, roundedSprite, new Color32(255, 249, 217, 245),
            new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(330f, 52f), new Vector2(430f, 78f));
        AddOutline(collaboration.gameObject, new Color32(255, 255, 255, 235), 3f);
        AddShadow(collaboration.gameObject, new Color32(80, 70, 25, 65), new Vector2(0f, -6f));
        CreateText("People", collaboration.rectTransform, "● ● ●", font, 29f, new Color32(50, 160, 50, 255), TextAlignmentOptions.Center,
            new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(0f, 0.5f), new Vector2(26f, 0f), new Vector2(120f, 60f));
        CreateText("Text", collaboration.rectTransform, "Aktiviti kolaboratif", font, 29f, new Color32(25, 91, 42, 255), TextAlignmentOptions.Center,
            new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0.5f, 0.5f), new Vector2(42f, 0f), new Vector2(-100f, -10f));
    }

    private static void BuildWheel(RectTransform root, Sprite wheelSprite, Sprite roundedSprite, TMP_FontAsset font)
    {
        RectTransform rotator = CreateRect("WheelRotator", root, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), new Vector2(525f, -6f), new Vector2(650f, 650f));
        Image wheel = rotator.gameObject.AddComponent<Image>();
        wheel.sprite = wheelSprite;
        wheel.preserveAspect = true;
        wheel.raycastTarget = false;
        AddShadow(rotator.gameObject, new Color32(78, 48, 12, 95), new Vector2(0f, -12f));

        RectTransform[] labels =
        {
            AddWheelLabel(rotator, font, "Bakteria", new Vector2(0f, 220f)),
            AddWheelLabel(rotator, font, "Fungi", new Vector2(205f, 66f)),
            AddWheelLabel(rotator, font, "Alga", new Vector2(128f, -205f)),
            AddWheelLabel(rotator, font, "Protozoa", new Vector2(-128f, -205f)),
            AddWheelLabel(rotator, font, "Virus", new Vector2(-205f, 66f))
        };


        SpinControl spin = rotator.gameObject.AddComponent<SpinControl>();
        spin.spinDuration = 4.2f;
        spin.minTurns = 3;
        spin.maxTurns = 5;
        spin.uprightLabels = labels;

        TextMeshProUGUI pointer = CreateText("Pointer", root, "▼", font, 104f, new Color32(244, 48, 24, 255), TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(525f, 343f), new Vector2(130f, 130f));
        pointer.outlineColor = new Color32(152, 28, 10, 255);
        pointer.outlineWidth = 0.18f;
        AddShadow(pointer.gameObject, new Color32(98, 39, 7, 100), new Vector2(0f, -8f));

        Image pointerDot = CreatePanel("PointerDot", root, roundedSprite, new Color32(255, 203, 31, 255),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(525f, 397f), new Vector2(42f, 42f));
        AddOutline(pointerDot.gameObject, new Color32(209, 109, 0, 255), 3f);

        Image buttonImage = CreatePanel("SpinButton", root, roundedSprite, new Color32(237, 45, 25, 255),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(525f, -420f), new Vector2(390f, 112f));
        AddOutline(buttonImage.gameObject, new Color32(255, 236, 209, 255), 5f);
        AddShadow(buttonImage.gameObject, new Color32(88, 39, 13, 150), new Vector2(0f, -12f));
        buttonImage.raycastTarget = true;
        Button button = buttonImage.gameObject.AddComponent<Button>();
        button.targetGraphic = buttonImage;
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color32(255, 232, 178, 255);
        colors.pressedColor = new Color32(215, 215, 215, 255);
        colors.disabledColor = new Color32(150, 150, 150, 190);
        colors.fadeDuration = 0.08f;
        button.colors = colors;
        TextMeshProUGUI buttonText = CreateText("Text", buttonImage.rectTransform, "PUTAR", font, 58f, Color.white, TextAlignmentOptions.Center,
            Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(-16f, -10f));
        buttonText.outlineColor = new Color32(133, 24, 12, 255);
        buttonText.outlineWidth = 0.16f;
        AddShadow(buttonText.gameObject, new Color32(88, 15, 7, 150), new Vector2(0f, -5f));

        TextMeshProUGUI result = CreateText("ResultLabel", root, "Tekan PUTAR untuk memilih", font, 24f, new Color32(31, 93, 42, 255), TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(525f, -493f), new Vector2(470f, 42f));
        result.enableAutoSizing = true;
        result.fontSizeMin = 18f;
        result.fontSizeMax = 24f;

        spin.spinButton = button;
        spin.resultLabel = result;
        button.onClick.AddListener(spin.Spin);
    }

    private static void CreateStep(RectTransform parent, Sprite roundedSprite, TMP_FontAsset font, string number, string body, Vector2 position)
    {
        Image icon = CreatePanel("Step" + number + "Icon", parent, roundedSprite, new Color32(74, 181, 48, 255),
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), position + new Vector2(-232f, 0f), new Vector2(92f, 92f));
        AddOutline(icon.gameObject, new Color32(255, 255, 255, 245), 4f);
        AddShadow(icon.gameObject, new Color32(51, 83, 25, 90), new Vector2(0f, -5f));
        CreateText("Number", icon.rectTransform, number, font, 48f, Color.white, TextAlignmentOptions.Center,
            Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(-8f, -8f));

        TextMeshProUGUI stepText = CreateText("Step" + number + "Text", parent, body, font, 30f, new Color32(24, 77, 39, 255), TextAlignmentOptions.Left,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), position + new Vector2(77f, 0f), new Vector2(465f, 118f));
        stepText.enableAutoSizing = true;
        stepText.fontSizeMin = 24f;
        stepText.fontSizeMax = 30f;
    }

    private static RectTransform AddWheelLabel(RectTransform parent, TMP_FontAsset font, string label, Vector2 position)
    {
        TextMeshProUGUI text = CreateText(label, parent, label, font, 31f, new Color32(23, 77, 31, 255), TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), position, new Vector2(190f, 48f));
        text.outlineColor = new Color32(255, 255, 255, 255);
        text.outlineWidth = 0.2f;
        AddShadow(text.gameObject, new Color32(16, 52, 20, 95), new Vector2(0f, -3f));
        return text.rectTransform;
    }

    private static void RestyleNavigation(Transform microsquad, Sprite roundedSprite)
    {
        Transform bottom = microsquad.Find("Bottom");
        if (bottom == null)
            return;

        Transform home = bottom.Find("HomeButton");
        if (home != null && home.TryGetComponent(out RectTransform homeRect))
        {
            home.SetParent(microsquad, false);
            home.SetAsLastSibling();
            homeRect.anchorMin = new Vector2(0f, 1f);
            homeRect.anchorMax = new Vector2(0f, 1f);
            homeRect.pivot = new Vector2(0f, 1f);
            homeRect.anchoredPosition = new Vector2(28f, -25f);
            homeRect.sizeDelta = new Vector2(90f, 90f);
            if (home.TryGetComponent(out Image image))
            {
                image.sprite = roundedSprite;
                image.type = Image.Type.Sliced;
                image.color = new Color32(255, 250, 224, 245);
            }
        }

        Transform next = bottom.Find("NextButton");
        if (next != null)
            next.gameObject.SetActive(false);
    }

    private static void ConfigureSprite(string path, int maxSize)
    {
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
            return;

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.filterMode = FilterMode.Bilinear;
        importer.textureCompression = TextureImporterCompression.CompressedHQ;
        importer.maxTextureSize = maxSize;
        importer.SaveAndReimport();
    }

    private static RectTransform CreateRect(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = sizeDelta;
        rect.localScale = Vector3.one;
        return rect;
    }

    private static Image CreatePanel(string name, Transform parent, Sprite sprite, Color color, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        Image image = CreateImage(name, parent, sprite, color, anchorMin, anchorMax, pivot, anchoredPosition, sizeDelta);
        image.type = Image.Type.Sliced;
        image.raycastTarget = false;
        return image;
    }

    private static Image CreateImage(string name, Transform parent, Sprite sprite, Color color, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        RectTransform rect = CreateRect(name, parent, anchorMin, anchorMax, pivot, anchoredPosition, sizeDelta);
        Image image = rect.gameObject.AddComponent<Image>();
        image.sprite = sprite;
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    private static TextMeshProUGUI CreateText(string name, Transform parent, string value, TMP_FontAsset font, float size, Color color,
        TextAlignmentOptions alignment, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        RectTransform rect = CreateRect(name, parent, anchorMin, anchorMax, pivot, anchoredPosition, sizeDelta);
        TextMeshProUGUI text = rect.gameObject.AddComponent<TextMeshProUGUI>();
        text.text = value;
        text.font = font;
        text.fontSize = size;
        text.fontStyle = FontStyles.Bold;
        text.color = color;
        text.alignment = alignment;
        text.enableWordWrapping = true;
        text.overflowMode = TextOverflowModes.Ellipsis;
        text.raycastTarget = false;
        return text;
    }

    private static void AddShadow(GameObject target, Color color, Vector2 distance)
    {
        Shadow shadow = target.AddComponent<Shadow>();
        shadow.effectColor = color;
        shadow.effectDistance = distance;
        shadow.useGraphicAlpha = true;
    }

    private static void AddOutline(GameObject target, Color color, float distance)
    {
        Outline outline = target.AddComponent<Outline>();
        outline.effectColor = color;
        outline.effectDistance = new Vector2(distance, -distance);
        outline.useGraphicAlpha = true;
    }

    private static void SetDirectChildActive(Transform parent, string childName, bool active)
    {
        Transform child = parent.Find(childName);
        if (child != null)
            child.gameObject.SetActive(active);
    }
}
#endif
