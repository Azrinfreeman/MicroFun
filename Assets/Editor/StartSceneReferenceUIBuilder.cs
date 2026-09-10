#if UNITY_EDITOR
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Repeatable visual rebuild; reuses the original controls and their serialized events.</summary>
public static class StartSceneReferenceUIBuilder
{
    const string ScenePath = "Assets/Scenes/StartScene.unity";
    const string ArtFolder = "Assets/Generated/StartScene/";
    static readonly Color Ink = new Color(.025f, .23f, .055f);
    static readonly Color Green = new Color(.08f, .48f, .12f);
    static TMP_FontAsset displayFont;
    static Material inputMaterial;
    static Sprite panelSprite, buttonSprite, circleSprite;

    [MenuItem("MicroFun/Build StartScene Reference UI")]
    public static void Build()
    {
        if (EditorApplication.isPlaying) throw new System.InvalidOperationException("Exit Play Mode before rebuilding StartScene.");
        var scene = EditorSceneManager.GetActiveScene();
        if (scene.path != ScenePath) scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        var canvas = scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Canvas>(true)).First();
        var fields = canvas.GetComponentsInChildren<TMP_InputField>(true);
        var name = fields.First(f => f.name == "NamaInput");
        var klass = fields.First(f => f.name == "KelasInpuit");
        var start = canvas.GetComponentsInChildren<Button>(true).First(b => Enumerable.Range(0, b.onClick.GetPersistentEventCount()).Any(i => b.onClick.GetPersistentMethodName(i) == "ChangeScene"));
        var audio = canvas.GetComponentsInChildren<Button>(true).FirstOrDefault(b => b.name == "AudioButton");
        // Keep original object identities, field content, validators and callback wiring.
        foreach (var t in new Transform[] { name.transform, klass.transform, start.transform }) t.SetParent(canvas.transform, false);
        if (audio != null) audio.transform.SetParent(canvas.transform, false);
        var old = canvas.transform.Find("StartSceneReferenceUI");
        if (old != null) Object.DestroyImmediate(old.gameObject);
        var legacy = canvas.transform.Find("LegacyStartSceneUI");
        if (legacy == null)
        {
            legacy = Rect("LegacyStartSceneUI", canvas.transform, Vector2.zero, Vector2.zero);
            foreach (var t in canvas.transform.Cast<Transform>().ToArray())
                if (t != legacy && t != name.transform && t != klass.transform && t != start.transform && (audio == null || t != audio.transform)) t.SetParent(legacy, false);
        }
        legacy.gameObject.SetActive(false);
        var animator = canvas.GetComponent<Animator>();
        if (animator != null) animator.enabled = false;
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvas.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        displayFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/FredokaOne SDF.asset");
        inputMaterial = AssetDatabase.LoadAssetAtPath<Material>(ArtFolder + "reference_input_text.mat");
        if (inputMaterial == null)
        {
            inputMaterial = new Material(TMP_Settings.defaultFontAsset.material) { name = "StartScene Input Text" };
            inputMaterial.SetFloat("_OutlineWidth", 0); inputMaterial.SetColor("_FaceColor", Color.white);
            inputMaterial.DisableKeyword("OUTLINE_ON"); inputMaterial.DisableKeyword("UNDERLAY_ON");
            AssetDatabase.CreateAsset(inputMaterial, ArtFolder + "reference_input_text.mat");
        }
        panelSprite = Surface("reference_panel", new Color(1, 1, 1), new Color(.97f, .975f, .965f), 24, false);
        buttonSprite = Surface("reference_button", new Color(.62f, .84f, .10f), new Color(.09f, .57f, .10f), 40, true);
        circleSprite = Surface("reference_circle", new Color(.27f, .70f, .09f), new Color(.015f, .40f, .11f), 64, false, 128, 128);
        var root = Rect("StartSceneReferenceUI", canvas.transform, Vector2.zero, Vector2.zero);
        Stretch(root);
        var artPath = ArtFolder + "start_scene_reference_v2.png";
        var importer = (TextureImporter)AssetImporter.GetAtPath(artPath);
        if (importer.maxTextureSize != 4096 || importer.textureCompression != TextureImporterCompression.Uncompressed || importer.mipmapEnabled)
        {
            importer.maxTextureSize = 4096; importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled = false; importer.wrapMode = TextureWrapMode.Clamp; importer.SaveAndReimport();
        }
        var wash = root.gameObject.AddComponent<RawImage>();
        wash.texture = AssetDatabase.LoadAssetAtPath<Texture2D>(artPath); wash.uvRect = new Rect(0, .999f, 1, .001f); wash.raycastTarget = false;
        // Fixed-aspect composition stays undistorted; Expand gives safe margins on other aspect ratios.
        var content = Rect("ReferenceComposition", root, Vector2.zero, new Vector2(1920, 1080));
        var background = Rect("IllustratedBackground", content, new Vector2(0, -22), new Vector2(1920, 1036));
        var art = background.gameObject.AddComponent<RawImage>();
        art.texture = AssetDatabase.LoadAssetAtPath<Texture2D>(ArtFolder + "start_scene_reference_v2.png");
        if (art.texture == null) throw new System.InvalidOperationException("Reference artwork is missing.");
        art.raycastTarget = false;
        Label("Welcome", "Selamat Datang ke", content, new Vector2(0, 487), new Vector2(800, 72), 52, Ink, true);

        if (audio == null) audio = Rect("AudioButton", content, Vector2.zero, Vector2.zero).gameObject.AddComponent<Button>();
        audio.transform.SetParent(content, false);
        ClearDecorations(audio.gameObject);
        Place((RectTransform)audio.transform, new Vector2(-737, 476), new Vector2(370, 90));
        var audioImage = audio.GetComponent<Image>() ?? audio.gameObject.AddComponent<Image>();
        audioImage.sprite = panelSprite; audioImage.type = Image.Type.Sliced; audioImage.pixelsPerUnitMultiplier = 1; audioImage.color = Color.white;
        audio.targetGraphic = audioImage;
        Border(audio.gameObject, Green, 1.5f); Shadow(audio.gameObject, .13f, 4);
        var speaker = Panel("SpeakerBadge", audio.transform, new Vector2(-135, 0), new Vector2(78, 78), circleSprite, Color.white);
        Border(speaker.gameObject, Color.white, 2);
        Icon("Speaker", speaker, StartSceneIcon.Symbol.Speaker, Vector2.zero, new Vector2(48, 48), Color.white);
        Label("AudioLabel", "Dengar audio", audio.transform, new Vector2(31, 0), new Vector2(231, 52), 29, Ink, true);
        Feedback(audio);

        var instruction = Panel("Instruction", content, new Vector2(0, -66), new Vector2(970, 70), panelSprite, new Color(1, 1, 1, .84f));
        Border(instruction.gameObject, new Color(1, 1, 1, .9f), 1); Shadow(instruction.gameObject, .09f, 4);
        Icon("Clipboard", instruction, StartSceneIcon.Symbol.Clipboard, new Vector2(-422, 0), new Vector2(48, 56), Green);
        Label("Instructions", "Masukkan nama murid dan pilih kelas untuk memulakan.", instruction, new Vector2(32, 0), new Vector2(820, 62), 28, Ink, true);

        Field(name, content, new Vector2(35, -203), "Masukkan nama murid");
        Field(klass, content, new Vector2(35, -341), "Masukkan kelas");
        FieldLabel(content, "NAMA MURID", -137, StartSceneIcon.Symbol.Person);
        FieldLabel(content, "KELAS", -275, StartSceneIcon.Symbol.Cap);
        Icon("PersonIcon", name.transform, StartSceneIcon.Symbol.Person, new Vector2(357, 0), new Vector2(40, 44), Green);
        Icon("ClassIcon", klass.transform, StartSceneIcon.Symbol.Cap, new Vector2(357, 0), new Vector2(42, 42), Green);

        start.transform.SetParent(content, false); start.gameObject.SetActive(true);
        ClearDecorations(start.gameObject);
        Place((RectTransform)start.transform, new Vector2(0, -461), new Vector2(510, 124));
        var startImage = start.GetComponent<Image>(); startImage.sprite = buttonSprite; startImage.type = Image.Type.Sliced; startImage.pixelsPerUnitMultiplier = 1; startImage.color = Color.white;
        start.targetGraphic = startImage;
        Border(start.gameObject, new Color(.14f, .49f, .04f), 2); Shadow(start.gameObject, .23f, 8);
        var mula = Label("MulaLabel", "MULA", start.transform, new Vector2(-24, 4), new Vector2(370, 100), 77, Color.white, true);
        Shadow(mula.gameObject, .27f, 4);
        Icon("PlayArrow", start.transform, StartSceneIcon.Symbol.Play, new Vector2(182, 1), new Vector2(47, 53), new Color(1, .83f, .035f));
        Feedback(start);
        NavigationOrder(audio, name, klass, start);
        Canvas.ForceUpdateCanvases();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Selection.activeGameObject = root.gameObject;
        Debug.Log("StartScene reference UI rebuilt: original inputs and MULA/audio events preserved; 1920x1080 safe composition.");
    }

    static void Field(TMP_InputField field, Transform parent, Vector2 pos, string placeholder)
    {
        field.transform.SetParent(parent, false); field.gameObject.SetActive(true);
        Place((RectTransform)field.transform, pos, new Vector2(805, 82));
        foreach (var g in field.GetComponentsInChildren<StartSceneIcon>(true)) Object.DestroyImmediate(g.gameObject);
        foreach (Transform child in field.transform.Cast<Transform>().ToArray())
            if (child.name == "DropdownChevron") Object.DestroyImmediate(child.gameObject);
        var image = field.GetComponent<Image>(); image.sprite = panelSprite; image.type = Image.Type.Sliced; image.pixelsPerUnitMultiplier = 1; image.color = Color.white;
        Border(field.gameObject, Green, 1.5f); Shadow(field.gameObject, .12f, 4);
        field.targetGraphic = image;
        var viewport = field.textViewport;
        Stretch(viewport); viewport.offsetMin = new Vector2(31, 9); viewport.offsetMax = new Vector2(-85, -9);
        StyleFieldText(field.textComponent, 35, Ink);
        var ph = field.placeholder as TMP_Text;
        if (ph != null) { StyleFieldText(ph, 31, new Color(.22f, .38f, .22f, .68f)); ph.text = placeholder; }
        var colors = field.colors; colors.normalColor = Color.white; colors.highlightedColor = new Color(.96f, 1, .92f);
        colors.selectedColor = new Color(.94f, 1, .89f); colors.pressedColor = colors.selectedColor; colors.fadeDuration = .12f; field.colors = colors;
    }
    static void StyleFieldText(TMP_Text text, float size, Color color)
    {
        foreach (var fitter in text.GetComponents<ContentSizeFitter>()) Object.DestroyImmediate(fitter);
        Stretch(text.rectTransform); text.margin = Vector4.zero;
        text.font = TMP_Settings.defaultFontAsset; text.fontSharedMaterial = inputMaterial; text.fontSize = size; text.enableAutoSizing = false;
        text.fontStyle = FontStyles.Bold; text.color = color; text.alignment = TextAlignmentOptions.MidlineLeft;
        text.textWrappingMode = TextWrappingModes.NoWrap; text.overflowMode = TextOverflowModes.Masking;
        text.raycastTarget = false;
    }
    static void FieldLabel(Transform parent, string text, float y, StartSceneIcon.Symbol symbol)
    {
        var badge = Panel(text + " Badge", parent, new Vector2(-425, y), new Vector2(58, 58), circleSprite, Color.white);
        Border(badge.gameObject, Color.white, 2);
        Icon(text + " Icon", badge, symbol, Vector2.zero, new Vector2(35, 35), Color.white);
        var label = Label(text, text, parent, new Vector2(-169, y), new Vector2(390, 53), 33, Ink, true);
        label.alignment = TextAlignmentOptions.MidlineLeft;
    }
    static void ClearDecorations(GameObject go)
    {
        // Only decorative children are replaced. Existing callbacks and audio components stay attached.
        foreach (Transform child in go.transform.Cast<Transform>().ToArray()) Object.DestroyImmediate(child.gameObject);
        foreach (var effect in go.GetComponents<BaseMeshEffect>()) Object.DestroyImmediate(effect);
    }
    static void NavigationOrder(params Selectable[] controls)
    {
        for (int i = 0; i < controls.Length; i++) controls[i].navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnUp = controls[(i + controls.Length - 1) % controls.Length], selectOnDown = controls[(i + 1) % controls.Length] };
    }
    static void Feedback(Button button)
    {
        if (button.GetComponent<AnimatedButtonFeedback>() == null) button.gameObject.AddComponent<AnimatedButtonFeedback>();
        var colors = button.colors; colors.normalColor = Color.white; colors.highlightedColor = new Color(1, 1, .88f);
        colors.selectedColor = new Color(1, 1, .84f); colors.pressedColor = new Color(.85f, .94f, .75f); colors.fadeDuration = .12f; button.colors = colors;
    }
    static RectTransform Rect(string name, Transform parent, Vector2 pos, Vector2 size)
    {
        var rt = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>(); rt.SetParent(parent, false); Place(rt, pos, size); return rt;
    }
    static void Place(RectTransform rt, Vector2 pos, Vector2 size)
    {
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f); rt.localScale = Vector3.one; rt.localRotation = Quaternion.identity; rt.anchoredPosition3D = new Vector3(pos.x, pos.y, 0); rt.sizeDelta = size;
    }
    static void Stretch(RectTransform rt) { rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.offsetMin = rt.offsetMax = Vector2.zero; rt.localScale = Vector3.one; }
    static RectTransform Panel(string name, Transform parent, Vector2 pos, Vector2 size, Sprite sprite, Color color)
    {
        var rt = Rect(name, parent, pos, size); var image = rt.gameObject.AddComponent<Image>(); image.sprite = sprite; image.type = Image.Type.Sliced; image.color = color; image.raycastTarget = false; return rt;
    }
    static TMP_Text Label(string name, string value, Transform parent, Vector2 pos, Vector2 size, float fontSize, Color color, bool display)
    {
        var rt = Rect(name, parent, pos, size); var t = rt.gameObject.AddComponent<TextMeshProUGUI>();
        t.font = display ? displayFont : TMP_Settings.defaultFontAsset; t.text = value; t.fontSize = fontSize; t.color = color;
        t.alignment = TextAlignmentOptions.Center; t.textWrappingMode = TextWrappingModes.NoWrap; t.raycastTarget = false; return t;
    }
    static void Icon(string name, Transform parent, StartSceneIcon.Symbol symbol, Vector2 pos, Vector2 size, Color color)
    {
        var icon = Rect(name, parent, pos, size).gameObject.AddComponent<StartSceneIcon>(); icon.symbol = symbol; icon.color = color; icon.raycastTarget = false;
    }
    static void Border(GameObject go, Color color, float width) { var e = go.GetComponent<Outline>() ?? go.AddComponent<Outline>(); e.effectColor = color; e.effectDistance = new Vector2(width, -width); }
    static void Shadow(GameObject go, float alpha, float depth) { var e = go.GetComponents<Shadow>().FirstOrDefault(s => !(s is Outline)) ?? go.AddComponent<Shadow>(); e.effectColor = new Color(.08f, .19f, .035f, alpha); e.effectDistance = new Vector2(0, -depth); }

    // Small native UI textures, generated once with antialiased edges and sliced corners.
    static Sprite Surface(string name, Color top, Color bottom, int radius, bool bevel, int width = 256, int height = 128)
    {
        var path = ArtFolder + name + ".asset";
        var existing = AssetDatabase.LoadAssetAtPath<Sprite>(path); if (existing != null) return existing;
        var tex = new Texture2D(width, height, TextureFormat.RGBA32, false) { name = name + " Texture", filterMode = FilterMode.Bilinear, wrapMode = TextureWrapMode.Clamp };
        var pixels = new Color[width * height];
        for (int y = 0; y < height; y++) for (int x = 0; x < width; x++)
        {
            float qx = Mathf.Abs(x + .5f - width / 2f) - (width / 2f - radius - 1);
            float qy = Mathf.Abs(y + .5f - height / 2f) - (height / 2f - radius - 1);
            float d = new Vector2(Mathf.Max(qx, 0), Mathf.Max(qy, 0)).magnitude + Mathf.Min(Mathf.Max(qx, qy), 0) - radius;
            var c = Color.Lerp(bottom, top, (float)y / (height - 1));
            if (bevel && d > -7) c = Color.Lerp(c, y > height / 2 ? new Color(.85f, .97f, .40f) : new Color(.04f, .39f, .08f), .60f);
            c.a = Mathf.Clamp01(.5f - d); pixels[y * width + x] = c;
        }
        tex.SetPixels(pixels); tex.Apply();
        var sprite = Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(.5f, .5f), 100, 0, SpriteMeshType.FullRect, new Vector4(radius, radius, radius, radius));
        sprite.name = name; AssetDatabase.CreateAsset(sprite, path); AssetDatabase.AddObjectToAsset(tex, sprite); return sprite;
    }
}
#endif
