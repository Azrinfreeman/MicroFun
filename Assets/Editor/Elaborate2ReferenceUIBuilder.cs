using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Rebuilds Akt2's additive Elaborasi page as the three-card Alga, Protozoa,
/// and Virus exploration screen.  Run again from the Tools menu to refresh
/// only this scene's generated presentation layer.
/// </summary>
public static class Elaborate2ReferenceUIBuilder
{
    private const string ScenePath = "Assets/Scenes/Aktivity/akt1/elaborate2.unity";
    private static readonly Color Ink = new Color32(13, 39, 90, 255);
    private static readonly Color Cream = new Color32(255, 249, 224, 255);
    private static readonly Color Green = new Color32(69, 160, 16, 255);
    private static readonly Color Blue = new Color32(36, 120, 235, 255);

    [MenuItem("Tools/MicroFun/Rebuild Elaborate2 Reference UI")]
    public static void Build()
    {
        var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        var canvasObject = GameObject.Find("Canvas");
        if (canvasObject == null)
        {
            Debug.LogError("Elaborate2 needs a Canvas before it can be rebuilt.");
            return;
        }

        var canvas = canvasObject.GetComponent<Canvas>();
        var scaler = canvasObject.GetComponent<CanvasScaler>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        for (var i = canvasObject.transform.childCount - 1; i >= 0; i--)
            Object.DestroyImmediate(canvasObject.transform.GetChild(i).gameObject);

        var sceneController = canvasObject.GetComponent<SceneController>();
        var mainCamera = GameObject.Find("Main Camera");
        var eventSystem = GameObject.Find("EventSystem");
        ConfigureSceneController(sceneController, canvasObject, mainCamera, eventSystem);
        ConfigureArtworkImporters();

        var root = canvasObject.transform;
        CreateBackdrop(root);
        CreateTitle(root);
        CreateInstruction(root);

        var cardA = CreateCard(root, "AlgaCard", new Vector2(-635f, -105f), "Alga",
            "Assets/Generated/Elaborate2/elaborate2_alga_card.png", new Color32(191, 228, 109, 255));
        var cardB = CreateCard(root, "ProtozoaCard", new Vector2(0f, -105f), "Protozoa",
            "Assets/Generated/Elaborate2/elaborate2_protozoa_card.png", new Color32(129, 190, 248, 255));
        var cardC = CreateCard(root, "VirusCard", new Vector2(635f, -105f), "Virus",
            "Assets/Generated/Elaborate2/elaborate2_virus_card.png", new Color32(247, 133, 179, 255));

        UnityEventTools.AddStringPersistentListener(cardA.GetComponent<Button>().onClick, sceneController.AddScene, "chrorella");
        UnityEventTools.AddStringPersistentListener(cardB.GetComponent<Button>().onClick, sceneController.AddScene, "amoeba");
        UnityEventTools.AddStringPersistentListener(cardC.GetComponent<Button>().onClick, sceneController.AddScene, "influenza");
        CreateNavigation(root, sceneController);
        AddCardFeedback(cardA);
        AddCardFeedback(cardB);
        AddCardFeedback(cardC);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Elaborate2 reference UI rebuilt.");
    }

    private static void ConfigureSceneController(SceneController controller, GameObject canvas, GameObject camera, GameObject eventSystem)
    {
        var serialized = new SerializedObject(controller);
        serialized.FindProperty("oldCanvas").objectReferenceValue = canvas;
        serialized.FindProperty("oldCamera").objectReferenceValue = camera;
        serialized.FindProperty("oldEventSystem").objectReferenceValue = eventSystem;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void ConfigureArtworkImporters()
    {
        foreach (var path in new[]
                 {
                     "Assets/Generated/Elaborate2/elaborate2_alga_card.png",
                     "Assets/Generated/Elaborate2/elaborate2_protozoa_card.png",
                     "Assets/Generated/Elaborate2/elaborate2_virus_card.png"
                 })
        {
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null || importer.textureType == TextureImporterType.Sprite)
                continue;

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.filterMode = FilterMode.Bilinear;
            importer.SaveAndReimport();
        }
    }

    private static void CreateBackdrop(Transform root)
    {
        var left = CreateImage("BackdropLeft", root, Vector2.zero, new Vector2(1040f, 1080f), new Color32(207, 239, 174, 255));
        left.rectTransform.anchoredPosition = new Vector2(-440f, 0f);
        var right = CreateImage("BackdropRight", root, Vector2.zero, new Vector2(1040f, 1080f), new Color32(255, 207, 148, 255));
        right.rectTransform.anchoredPosition = new Vector2(440f, 0f);
        var glow = CreateImage("CenterGlow", root, Vector2.zero, new Vector2(1080f, 1080f), new Color32(255, 247, 188, 125));
        glow.rectTransform.anchoredPosition = new Vector2(0f, 0f);

        CreateBubble(root, new Vector2(-885f, 420f), 38f);
        CreateBubble(root, new Vector2(-805f, 320f), 30f);
        CreateBubble(root, new Vector2(-720f, 265f), 24f);
        CreateBubble(root, new Vector2(740f, 250f), 43f);
        CreateBubble(root, new Vector2(900f, 80f), 28f);
        CreateBubble(root, new Vector2(820f, -205f), 22f);

        CreateLeafCluster(root, new Vector2(-780f, -455f), false);
        CreateLeafCluster(root, new Vector2(805f, -455f), true);
    }

    private static void CreateTitle(Transform root)
    {
        var title = CreateText(root, "Title", "ELABORASI", new Vector2(0f, 420f), new Vector2(880f, 145f), 104f, Ink, FontStyles.Bold, TextAlignmentOptions.Center);
        title.outlineWidth = 0.06f;
        title.outlineColor = Color.white;
        title.enableWordWrapping = false;
        CreateSimpleLeaf(root, new Vector2(-620f, 414f), -35f);
        CreateSimpleLeaf(root, new Vector2(-560f, 420f), 20f);
        CreateSimpleLeaf(root, new Vector2(560f, 420f), -20f);
        CreateSimpleLeaf(root, new Vector2(620f, 414f), 35f);
    }

    private static void CreateInstruction(Transform root)
    {
        var panel = CreateRoundedPanel("Instruction", root, new Vector2(0f, 270f), new Vector2(980f, 118f), Cream, new Color32(245, 158, 26, 255), 3);
        CreateText(panel.transform, "Clipboard", "≡", new Vector2(-405f, 0f), new Vector2(115f, 80f), 72f, new Color32(241, 119, 44, 255), FontStyles.Bold, TextAlignmentOptions.Center);
        var copy = CreateText(panel.transform, "Copy", "Gerakkan kanta pembesar ke imej\nuntuk mengenal pasti mikroorganisma.", new Vector2(78f, 0f), new Vector2(725f, 92f), 32f, Color.black, FontStyles.Bold, TextAlignmentOptions.Left);
        copy.overflowMode = TextOverflowModes.Overflow;
        copy.lineSpacing = -5f;
    }

    private static GameObject CreateCard(Transform root, string name, Vector2 position, string label, string spritePath, Color labelColor)
    {
        var card = CreateRoundedPanel(name, root, position, new Vector2(585f, 555f), Color.white, Color.white, 0);
        var button = card.AddComponent<Button>();
        button.targetGraphic = card.GetComponent<Image>();
        var colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1f, 1f, 1f, 0.96f);
        colors.pressedColor = new Color(0.87f, 0.87f, 0.87f, 1f);
        button.colors = colors;

        var artMask = new GameObject("Artwork", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(RectMask2D));
        artMask.transform.SetParent(card.transform, false);
        var artMaskRect = artMask.GetComponent<RectTransform>();
        artMaskRect.anchorMin = new Vector2(0.04f, 0.09f);
        artMaskRect.anchorMax = new Vector2(0.96f, 0.96f);
        artMaskRect.offsetMin = Vector2.zero;
        artMaskRect.offsetMax = Vector2.zero;
        artMask.GetComponent<Image>().color = Color.white;

        var art = CreateImage("Illustration", artMask.transform, Vector2.zero, Vector2.zero, Color.white);
        art.rectTransform.anchorMin = Vector2.zero;
        art.rectTransform.anchorMax = Vector2.one;
        art.rectTransform.offsetMin = Vector2.zero;
        art.rectTransform.offsetMax = Vector2.zero;
        art.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
        art.type = Image.Type.Simple;
        art.preserveAspect = false;

        var labelPanel = CreateRoundedPanel("Label", card.transform, new Vector2(0f, -252f), new Vector2(390f, 84f), labelColor, Color.white, 1);
        var labelText = CreateText(labelPanel.transform, "Text", label, Vector2.zero, new Vector2(360f, 70f), 49f, new Color32(8, 26, 61, 255), FontStyles.Bold, TextAlignmentOptions.Center);
        labelText.enableWordWrapping = false;
        return card;
    }

    private static void AddCardFeedback(GameObject card)
    {
        var feedback = card.AddComponent<AnimatedButtonFeedback>();
        var serialized = new SerializedObject(feedback);
        var hover = serialized.FindProperty("hoverScale");
        var pressed = serialized.FindProperty("pressedScale");
        if (hover != null) hover.floatValue = 1.025f;
        if (pressed != null) pressed.floatValue = 0.96f;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void CreateNavigation(Transform root, SceneController sceneController)
    {
        var home = CreateButton(root, "HomeButton", new Vector2(-770f, -460f), new Vector2(350f, 100f), "Laman\nUtama", new Color32(250, 143, 16, 255), 36f);
        var previous = CreateButton(root, "PreviousButton", new Vector2(-290f, -460f), new Vector2(405f, 94f), "‹  Sebelumnya", Blue, 36f);
        var next = CreateButton(root, "NextButton", new Vector2(710f, -460f), new Vector2(405f, 94f), "Seterusnya  ›", Blue, 36f);
        var audio = CreateButton(root, "AudioButton", new Vector2(745f, 425f), new Vector2(300f, 112f), "Dengar\naudio", Green, 33f);

        UnityEventTools.AddStringPersistentListener(home.GetComponent<Button>().onClick, sceneController.ChangeScene, "Activities");
        var backController = previous.AddComponent<BackController>();
        var backSerialized = new SerializedObject(backController);
        backSerialized.FindProperty("fallbackSceneName").stringValue = "akt2";
        backSerialized.ApplyModifiedPropertiesWithoutUndo();
        UnityEventTools.AddStringPersistentListener(next.GetComponent<Button>().onClick, sceneController.AddScene, "kuiz2");

        var audioSource = audio.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        var audioControl = audio.AddComponent<AudioNarrationButton>();
        var audioSerialized = new SerializedObject(audioControl);
        audioSerialized.FindProperty("narrationClip").objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/akt1_intro_voice.mp3");
        audioSerialized.FindProperty("label").objectReferenceValue = audio.GetComponentInChildren<TextMeshProUGUI>();
        audioSerialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static GameObject CreateButton(Transform parent, string name, Vector2 position, Vector2 size, string label, Color color, float fontSize)
    {
        var button = CreateRoundedPanel(name, parent, position, size, color, Color.white, 2);
        var text = CreateText(button.transform, "Text", label, Vector2.zero, size - new Vector2(34f, 16f), fontSize, Color.white, FontStyles.Bold, TextAlignmentOptions.Center);
        text.lineSpacing = -10f;
        button.AddComponent<Button>();
        var feedback = button.AddComponent<AnimatedButtonFeedback>();
        var serialized = new SerializedObject(feedback);
        var hover = serialized.FindProperty("hoverScale");
        var pressed = serialized.FindProperty("pressedScale");
        if (hover != null) hover.floatValue = 1.04f;
        if (pressed != null) pressed.floatValue = 0.94f;
        serialized.ApplyModifiedPropertiesWithoutUndo();
        return button;
    }

    private static GameObject CreateRoundedPanel(string name, Transform parent, Vector2 position, Vector2 size, Color fill, Color border, int borderWidth)
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

    private static Image CreateImage(string name, Transform parent, Vector2 position, Vector2 size, Color color)
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
        image.color = color;
        return image;
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
        text.text = value;
        text.font = TMP_Settings.defaultFontAsset;
        text.fontSize = fontSize;
        text.color = color;
        text.fontStyle = style;
        text.alignment = alignment;
        text.enableAutoSizing = false;
        text.enableWordWrapping = true;
        text.overflowMode = TextOverflowModes.Ellipsis;
        return text;
    }

    private static void CreateBubble(Transform root, Vector2 position, float size)
    {
        var bubble = CreateRoundedPanel("Bubble", root, position, new Vector2(size, size), new Color(1f, 1f, 1f, 0.35f), new Color(1f, 1f, 1f, 0.7f), 1);
        bubble.GetComponent<Image>().sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        bubble.GetComponent<Image>().type = Image.Type.Simple;
    }

    private static void CreateSimpleLeaf(Transform root, Vector2 position, float rotation)
    {
        var leaf = CreateRoundedPanel("Leaf", root, position, new Vector2(58f, 96f), new Color32(98, 185, 24, 255), Color.white, 1);
        leaf.GetComponent<Image>().sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        leaf.GetComponent<Image>().type = Image.Type.Simple;
        leaf.transform.localRotation = Quaternion.Euler(0f, 0f, rotation);
    }

    private static void CreateLeafCluster(Transform root, Vector2 position, bool flip)
    {
        for (var i = 0; i < 6; i++)
        {
            var angle = (flip ? -1f : 1f) * (-30f + i * 13f);
            var offset = new Vector2((flip ? -1f : 1f) * i * 28f, i % 2 == 0 ? 4f : -12f);
            var leaf = CreateRoundedPanel("CornerLeaf", root, position + offset, new Vector2(62f, 116f), new Color32((byte)(59 + i * 8), (byte)(143 + i * 9), 37, 255), new Color32(27, 105, 37, 155), 1);
            leaf.GetComponent<Image>().sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
            leaf.GetComponent<Image>().type = Image.Type.Simple;
            leaf.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}
