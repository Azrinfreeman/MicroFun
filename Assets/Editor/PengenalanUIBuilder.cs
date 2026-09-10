#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Rebuilds the Aktiviti 8 Pengenalan presentation while preserving the scene's
/// existing navigation and dialog controllers.
/// </summary>
public static class PengenalanUIBuilder
{
    private const string BackgroundPath = "Assets/Images/PengenalanUI/Background/pengenalan_background.png";
    private const string PuzzlePath = "Assets/Images/PengenalanUI/Puzzle/pengenalan_puzzle.png";
    private const string WhitePanelPath = "Assets/Images/UI Flat/btn_white_flat.png";
    private const string SpeakerPath = "Assets/Images/UI Flat/icon_speaker.png";
    private const string FontPath = "Assets/Fonts/FredokaOne SDF.asset";
    private const string NarrationPath = "Assets/Images/PengenalanUI/Audio/pengenalan_narration.wav";

    private static TMP_FontAsset _font;
    private static Sprite _whitePanel;

    [MenuItem("Tools/MicroFun/Rebuild Pengenalan UI")]
    public static void Rebuild()
    {
        ConfigureTexture(BackgroundPath, true);
        ConfigureTexture(PuzzlePath, false);

        _font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        _whitePanel = AssetDatabase.LoadAssetAtPath<Sprite>(WhitePanelPath);
        Sprite backgroundSprite = AssetDatabase.LoadAssetAtPath<Sprite>(BackgroundPath);
        Sprite speakerSprite = AssetDatabase.LoadAssetAtPath<Sprite>(SpeakerPath);
        Texture2D puzzleTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(PuzzlePath);
        AudioClip narration = AssetDatabase.LoadAssetAtPath<AudioClip>(NarrationPath);

        GameObject canvasObject = GameObject.Find("Canvas");
        if (canvasObject == null)
        {
            Debug.LogError("PengenalanUIBuilder: Canvas was not found in the active scene.");
            return;
        }

        Transform pengenalan = canvasObject.transform.Find("Pengenalan");
        if (pengenalan == null)
        {
            Debug.LogError("PengenalanUIBuilder: Canvas/Pengenalan was not found in the active scene.");
            return;
        }

        Transform existing = pengenalan.Find("PengenalanRedesignRoot");
        if (existing != null)
        {
            Object.DestroyImmediate(existing.gameObject);
        }

        SetChildActive(pengenalan, "Title", false);
        SetChildActive(pengenalan, "Content", false);

        GameObject root = CreateUI("PengenalanRedesignRoot", pengenalan);
        RectTransform rootRect = root.GetComponent<RectTransform>();
        Stretch(rootRect, Vector2.zero, Vector2.zero);
        root.transform.SetSiblingIndex(0);

        Image background = root.AddComponent<Image>();
        background.sprite = backgroundSprite;
        background.color = Color.white;
        background.preserveAspect = false;
        background.raycastTarget = false;

        BuildAudioButton(root.transform, speakerSprite, narration);
        BuildHeader(root.transform);
        BuildInstructionCard(root.transform);
        BuildHandsOnBadge(root.transform);

        GameObject boardFrame = CreateUI("PuzzleBoard", root.transform);
        RectTransform boardRect = boardFrame.GetComponent<RectTransform>();
        SetRect(boardRect, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-118f, -304f), new Vector2(800f, 596f));
        Image boardFrameImage = boardFrame.AddComponent<Image>();
        boardFrameImage.sprite = _whitePanel;
        boardFrameImage.type = Image.Type.Sliced;
        boardFrameImage.color = new Color32(17, 133, 210, 255);
        boardFrameImage.raycastTarget = false;
        AddShadow(boardFrame, new Color32(31, 68, 93, 105), new Vector2(0f, -9f));
        AddOutline(boardFrame, new Color32(3, 70, 123, 255), new Vector2(4f, 4f));

        GameObject art = CreateUI("PuzzleArtwork", boardFrame.transform);
        RectTransform artRect = art.GetComponent<RectTransform>();
        Stretch(artRect, new Vector2(16f, 16f), new Vector2(-16f, -16f));
        RawImage artImage = art.AddComponent<RawImage>();
        artImage.texture = puzzleTexture;
        artImage.color = Color.white;
        artImage.raycastTarget = false;

        GameObject slot = CreateUI("MissingBottomRightSlot", art.transform);
        RectTransform slotRect = slot.GetComponent<RectTransform>();
        slotRect.anchorMin = new Vector2(2f / 3f, 0f);
        slotRect.anchorMax = new Vector2(1f, 1f / 3f);
        slotRect.pivot = new Vector2(0.5f, 0.5f);
        slotRect.offsetMin = new Vector2(2f, 2f);
        slotRect.offsetMax = new Vector2(-2f, -2f);
        Image slotCover = slot.AddComponent<Image>();
        slotCover.sprite = _whitePanel;
        slotCover.type = Image.Type.Sliced;
        slotCover.color = new Color32(151, 75, 226, 255);
        slotCover.raycastTarget = true;
        AddOutline(slot, new Color32(76, 31, 142, 255), new Vector2(3f, 3f));

        TMP_Text slotHint = AddText("SlotHint", slot.transform, "?", 84f, Color.white, TextAlignmentOptions.Center);
        Stretch(slotHint.rectTransform, Vector2.zero, Vector2.zero);
        slotHint.raycastTarget = false;
        slotHint.alpha = 0.55f;

        PuzzleDropZone dropZone = slot.AddComponent<PuzzleDropZone>();

        TMP_Text status = AddText("PuzzleStatus", root.transform, "Seret kepingan ungu ke ruang kosong.", 27f,
            new Color32(23, 91, 47, 255), TextAlignmentOptions.Center);
        SetRect(status.rectTransform, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-118f, -264f), new Vector2(800f, 42f));
        status.fontStyle = FontStyles.Bold;
        status.raycastTarget = false;

        GameObject loosePiece = CreateUI("LoosePuzzlePiece", root.transform);
        RectTransform looseRect = loosePiece.GetComponent<RectTransform>();
        SetRect(looseRect, new Vector2(1f, 1f), new Vector2(1f, 0.5f), new Vector2(-34f, -858f), new Vector2(258f, 192f));
        RawImage looseImage = loosePiece.AddComponent<RawImage>();
        looseImage.texture = puzzleTexture;
        looseImage.uvRect = new Rect(2f / 3f, 0f, 1f / 3f, 1f / 3f);
        looseImage.color = Color.white;
        looseImage.raycastTarget = true;
        AddShadow(loosePiece, new Color32(50, 29, 73, 120), new Vector2(0f, -9f));
        AddOutline(loosePiece, Color.white, new Vector2(4f, 4f));
        loosePiece.AddComponent<CanvasGroup>();
        DraggablePuzzlePiece draggable = loosePiece.AddComponent<DraggablePuzzlePiece>();

        PengenalanPuzzleController controller = root.AddComponent<PengenalanPuzzleController>();
        Button nextButton = FindButton(pengenalan, "Bottom/Next");

        SerializedObject dropSerialized = new SerializedObject(dropZone);
        dropSerialized.FindProperty("expectedPieceId").stringValue = "piece-09";
        dropSerialized.FindProperty("snapTarget").objectReferenceValue = slotRect;
        dropSerialized.FindProperty("controller").objectReferenceValue = controller;
        dropSerialized.ApplyModifiedPropertiesWithoutUndo();

        SerializedObject pieceSerialized = new SerializedObject(draggable);
        pieceSerialized.FindProperty("pieceId").stringValue = "piece-09";
        pieceSerialized.FindProperty("dragLayer").objectReferenceValue = rootRect;
        pieceSerialized.ApplyModifiedPropertiesWithoutUndo();

        SerializedObject controllerSerialized = new SerializedObject(controller);
        controllerSerialized.FindProperty("loosePiece").objectReferenceValue = draggable;
        controllerSerialized.FindProperty("targetSlot").objectReferenceValue = dropZone;
        controllerSerialized.FindProperty("missingPieceCover").objectReferenceValue = slotCover;
        controllerSerialized.FindProperty("puzzleBoard").objectReferenceValue = boardRect;
        controllerSerialized.FindProperty("statusLabel").objectReferenceValue = status;
        controllerSerialized.FindProperty("nextButton").objectReferenceValue = nextButton;
        controllerSerialized.FindProperty("lockNextUntilComplete").boolValue = false;
        controllerSerialized.ApplyModifiedPropertiesWithoutUndo();

        EnsureEventSystem();
        EditorUtility.SetDirty(pengenalan.gameObject);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Selection.activeGameObject = root;

        Debug.Log("Pengenalan UI rebuilt. Navigation/dialogs preserved; drag the loose lower-right puzzle piece into the purple slot.");
    }

    private static void BuildAudioButton(Transform parent, Sprite speakerSprite, AudioClip narration)
    {
        GameObject buttonObject = CreateUI("AudioNarrationButton", parent);
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        SetRect(rect, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(36f, -28f), new Vector2(330f, 82f));

        Image image = buttonObject.AddComponent<Image>();
        image.sprite = _whitePanel;
        image.type = Image.Type.Sliced;
        image.color = new Color32(255, 253, 234, 255);
        image.raycastTarget = true;
        AddShadow(buttonObject, new Color32(34, 78, 30, 80), new Vector2(0f, -6f));
        AddOutline(buttonObject, new Color32(39, 147, 50, 255), new Vector2(3f, 3f));

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.92f, 1f, 0.90f, 1f);
        colors.pressedColor = new Color(0.78f, 0.95f, 0.75f, 1f);
        colors.disabledColor = Color.white;
        button.colors = colors;

        // The speaker asset already includes its circular button treatment.
        // Draw it once instead of nesting it inside a second green disc.
        GameObject iconObject = CreateUI("SpeakerIcon", buttonObject.transform);
        RectTransform iconRect = iconObject.GetComponent<RectTransform>();
        SetRect(iconRect, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(43f, 0f), new Vector2(76f, 76f));
        Image icon = iconObject.AddComponent<Image>();
        icon.sprite = speakerSprite;
        icon.color = Color.white;
        icon.preserveAspect = true;
        icon.raycastTarget = false;

        TextMeshProUGUI label = (TextMeshProUGUI)AddText("Label", buttonObject.transform, "Dengar audio", 27f,
            new Color32(18, 91, 38, 255), TextAlignmentOptions.MidlineLeft);
        label.fontStyle = FontStyles.Bold;
        label.rectTransform.anchorMin = new Vector2(0f, 0f);
        label.rectTransform.anchorMax = new Vector2(1f, 1f);
        label.rectTransform.offsetMin = new Vector2(116f, 6f);
        label.rectTransform.offsetMax = new Vector2(-18f, -6f);
        label.raycastTarget = false;

        AudioSource source = buttonObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = false;
        AudioNarrationButton narrationButton = buttonObject.AddComponent<AudioNarrationButton>();
        SerializedObject serialized = new SerializedObject(narrationButton);
        serialized.FindProperty("narrationClip").objectReferenceValue = narration;
        serialized.FindProperty("label").objectReferenceValue = label;
        serialized.ApplyModifiedPropertiesWithoutUndo();

        // Keep the reference visual, but avoid a misleading clickable control until a clip is supplied.
        button.interactable = narration != null;
    }

    private static void BuildHeader(Transform parent)
    {
        GameObject badge = CreatePanel("ActivityBadge", parent, new Color32(255, 250, 218, 255));
        SetRect(badge.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -28f), new Vector2(282f, 66f));
        AddShadow(badge, new Color32(80, 71, 32, 65), new Vector2(0f, -5f));
        AddOutline(badge, Color.white, new Vector2(3f, 3f));
        TMP_Text badgeText = AddText("Text", badge.transform, "AKTIVITI 8", 37f, new Color32(18, 104, 36, 255), TextAlignmentOptions.Center);
        Stretch(badgeText.rectTransform, Vector2.zero, Vector2.zero);
        badgeText.fontStyle = FontStyles.Bold;

        TMP_Text title = AddText("Title", parent, "Pengenalan", 102f, new Color32(20, 129, 21, 255), TextAlignmentOptions.Center);
        SetRect(title.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -94f), new Vector2(990f, 130f));
        title.fontStyle = FontStyles.Bold;
        title.outlineColor = new Color32(255, 255, 255, 255);
        title.outlineWidth = 0.24f;
        AddShadow(title.gameObject, new Color32(39, 61, 19, 85), new Vector2(0f, -8f));

        TMP_Text subtitle = AddText("Subtitle", parent, "Susun puzzle untuk mengenal pasti mikroorganisma.", 34f,
            new Color32(16, 83, 38, 255), TextAlignmentOptions.Center);
        SetRect(subtitle.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -222f), new Vector2(1120f, 48f));
        subtitle.fontStyle = FontStyles.Bold;
        AddShadow(subtitle.gameObject, new Color32(255, 255, 255, 150), new Vector2(0f, -2f));
    }

    private static void BuildInstructionCard(Transform parent)
    {
        GameObject card = CreatePanel("InstructionCard", parent, new Color32(255, 253, 239, 247));
        SetRect(card.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(166f, -350f), new Vector2(510f, 440f));
        AddShadow(card, new Color32(63, 70, 39, 76), new Vector2(0f, -8f));
        AddOutline(card, new Color32(255, 255, 255, 230), new Vector2(4f, 4f));

        GameObject header = CreatePanel("Header", card.transform, new Color32(255, 250, 217, 255));
        SetRect(header.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -38f), new Vector2(310f, 74f));
        AddOutline(header, new Color32(88, 163, 65, 255), new Vector2(2f, 2f));

        GameObject mark = CreatePanel("InstructionIcon", header.transform, new Color32(38, 155, 49, 255));
        SetRect(mark.GetComponent<RectTransform>(), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(38f, 0f), new Vector2(56f, 56f));
        TMP_Text markText = AddText("Mark", mark.transform, "!", 40f, Color.white, TextAlignmentOptions.Center);
        Stretch(markText.rectTransform, Vector2.zero, Vector2.zero);
        markText.fontStyle = FontStyles.Bold;

        TMP_Text headerText = AddText("Text", header.transform, "Arahan", 39f, new Color32(20, 94, 42, 255), TextAlignmentOptions.Center);
        headerText.rectTransform.anchorMin = Vector2.zero;
        headerText.rectTransform.anchorMax = Vector2.one;
        headerText.rectTransform.offsetMin = new Vector2(78f, 0f);
        headerText.rectTransform.offsetMax = new Vector2(-14f, 0f);
        headerText.fontStyle = FontStyles.Bold;

        TMP_Text instructions = AddText("Instructions", card.transform,
            "Lengkapkan puzzle\nuntuk melihat gambar\nmikroorganisma.", 37f,
            new Color32(22, 84, 42, 255), TextAlignmentOptions.Center);
        SetRect(instructions.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -25f), new Vector2(440f, 210f));
        instructions.fontStyle = FontStyles.Bold;
        instructions.lineSpacing = 8f;

        GameObject divider = CreateUI("Divider", card.transform);
        RectTransform dividerRect = divider.GetComponent<RectTransform>();
        SetRect(dividerRect, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 46f), new Vector2(360f, 3f));
        Image dividerImage = divider.AddComponent<Image>();
        dividerImage.color = new Color32(93, 159, 74, 180);
        dividerImage.raycastTarget = false;
    }

    private static void BuildHandsOnBadge(Transform parent)
    {
        GameObject badge = CreatePanel("HandsOnBadge", parent, new Color32(255, 251, 224, 250));
        SetRect(badge.GetComponent<RectTransform>(), new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(238f, 55f), new Vector2(390f, 90f));
        AddShadow(badge, new Color32(58, 80, 38, 75), new Vector2(0f, -6f));
        AddOutline(badge, Color.white, new Vector2(3f, 3f));

        GameObject icon = CreatePanel("Icon", badge.transform, new Color32(108, 191, 50, 255));
        SetRect(icon.GetComponent<RectTransform>(), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(45f, 0f), new Vector2(66f, 66f));
        BuildCheckmark(icon.transform);

        TMP_Text label = AddText("Label", badge.transform, "Aktiviti\nhands-on digital", 27f,
            new Color32(21, 93, 44, 255), TextAlignmentOptions.MidlineLeft);
        label.rectTransform.anchorMin = Vector2.zero;
        label.rectTransform.anchorMax = Vector2.one;
        label.rectTransform.offsetMin = new Vector2(118f, 6f);
        label.rectTransform.offsetMax = new Vector2(-14f, -6f);
        label.fontStyle = FontStyles.Bold;
        label.lineSpacing = 2f;
    }

    private static TMP_Text AddText(string name, Transform parent, string value, float size, Color color, TextAlignmentOptions alignment)
    {
        GameObject go = CreateUI(name, parent);
        TextMeshProUGUI text = go.AddComponent<TextMeshProUGUI>();
        text.font = _font;
        text.text = value;
        text.fontSize = size;
        text.color = color;
        text.alignment = alignment;
        text.enableWordWrapping = false;
        text.raycastTarget = false;
        text.overflowMode = TextOverflowModes.Overflow;
        return text;
    }

    private static void BuildCheckmark(Transform parent)
    {
        GameObject shortBar = CreateUI("CheckShort", parent);
        RectTransform shortRect = shortBar.GetComponent<RectTransform>();
        SetRect(shortRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-8f, -4f), new Vector2(9f, 24f));
        shortRect.localRotation = Quaternion.Euler(0f, 0f, 45f);
        Image shortImage = shortBar.AddComponent<Image>();
        shortImage.color = Color.white;
        shortImage.raycastTarget = false;

        GameObject longBar = CreateUI("CheckLong", parent);
        RectTransform longRect = longBar.GetComponent<RectTransform>();
        SetRect(longRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(8f, 2f), new Vector2(9f, 37f));
        longRect.localRotation = Quaternion.Euler(0f, 0f, -45f);
        Image longImage = longBar.AddComponent<Image>();
        longImage.color = Color.white;
        longImage.raycastTarget = false;
    }


    private static GameObject CreatePanel(string name, Transform parent, Color color)
    {
        GameObject go = CreateUI(name, parent);
        Image image = go.AddComponent<Image>();
        image.sprite = _whitePanel;
        image.type = Image.Type.Sliced;
        image.color = color;
        image.raycastTarget = false;
        return go;
    }

    private static GameObject CreateUI(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.layer = parent.gameObject.layer;
        go.transform.SetParent(parent, false);
        return go;
    }

    private static void SetRect(RectTransform rect, Vector2 anchor, Vector2 pivot, Vector2 position, Vector2 size)
    {
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = pivot;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        rect.localScale = Vector3.one;
    }

    private static void Stretch(RectTransform rect, Vector2 minOffset, Vector2 maxOffset)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMin = minOffset;
        rect.offsetMax = maxOffset;
        rect.localScale = Vector3.one;
    }

    private static void AddShadow(GameObject go, Color color, Vector2 distance)
    {
        Shadow shadow = go.AddComponent<Shadow>();
        shadow.effectColor = color;
        shadow.effectDistance = distance;
        shadow.useGraphicAlpha = true;
    }

    private static void AddOutline(GameObject go, Color color, Vector2 distance)
    {
        Outline outline = go.AddComponent<Outline>();
        outline.effectColor = color;
        outline.effectDistance = distance;
        outline.useGraphicAlpha = true;
    }

    private static void ConfigureTexture(string path, bool sprite)
    {
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
        {
            return;
        }

        importer.textureType = sprite ? TextureImporterType.Sprite : TextureImporterType.Default;
        importer.spriteImportMode = sprite ? SpriteImportMode.Single : SpriteImportMode.None;
        importer.mipmapEnabled = false;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.filterMode = FilterMode.Bilinear;
        importer.maxTextureSize = 2048;
        importer.SaveAndReimport();
    }

    private static void SetChildActive(Transform parent, string path, bool active)
    {
        Transform child = parent.Find(path);
        if (child != null)
        {
            child.gameObject.SetActive(active);
        }
    }

    private static Button FindButton(Transform root, string path)
    {
        Transform target = root.Find(path);
        return target != null ? target.GetComponent<Button>() : null;
    }

    private static void EnsureEventSystem()
    {
        if (Object.FindFirstObjectByType<EventSystem>() != null)
        {
            return;
        }

        GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        Undo.RegisterCreatedObjectUndo(eventSystem, "Create EventSystem");
    }
}
#endif
