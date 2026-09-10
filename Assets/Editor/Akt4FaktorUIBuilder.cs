#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using System.Linq;
using UnityEngine.Events;
using UnityEditor.Events;

public class Akt4FaktorUIBuilder
{
    static TMP_FontAsset _font;
    const string White = "Assets/Images/UI Flat/btn_white_flat.png";

    [MenuItem("Tools/MicroFun/Activity 4/Build Faktor UI")]
    public static void BuildUI()
    {
        var scene = EditorSceneManager.GetActiveScene();
        if (scene.name != "akt4") { Debug.LogError("Please open akt4"); return; }
        
        var roots = scene.GetRootGameObjects();
        var canvas = roots.FirstOrDefault(r => r.name == "Canvas")?.transform;
        if (canvas == null) return;
        
        var faktor = canvas.Find("Faktor");
        if (faktor == null) return;

        // Hide legacy UI
        var center = faktor.Find("Center");
        var bottom = faktor.Find("Bottom");
        if (center != null) center.gameObject.SetActive(false);
        if (bottom != null) bottom.gameObject.SetActive(false);

        // Remove old redesign if exists
        var oldRedesign = faktor.Find("Faktor_Redesign");
        if (oldRedesign != null) GameObject.DestroyImmediate(oldRedesign.gameObject);

        // Load fonts & config images
        _font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/FredokaOne SDF.asset");
        ConfigureSprite("Assets/Images/Akt4FaktorUI/faktor_bg.png");
        ConfigureSprite("Assets/Images/Akt4FaktorUI/faktor_microbe_green.png");
        ConfigureSprite("Assets/Images/Akt4FaktorUI/faktor_water_drop.png");

        RectTransform root = MakeRoot("Faktor_Redesign", faktor);

        // 1. Background
        NI("Background", root, "Assets/Images/Akt4FaktorUI/faktor_bg.png", Color.white, Vector2.zero, new Vector2(1920f, 1080f));

        // 2. Titles
        // "AKTIVITI 4" Pill
        var actPill = Panel("AktivitiPill", root, new Color32(230, 255, 180, 255), new Color32(110, 190, 40, 255), new Vector2(0, 460f), new Vector2(400f, 60f));
        NT("AktivitiTxt", actPill.transform, "AKTIVITI 4", 35f, new Color32(40, 100, 20, 255), TextAlignmentOptions.Center, Vector2.zero, new Vector2(400f, 60f));

        // Main Title
        var title = NT("MainTitle", root, "Faktor Mempengaruhi\nPertumbuhan Mikroorganisma", 75f, new Color32(20, 80, 20, 255), TextAlignmentOptions.Center, new Vector2(0, 360f), new Vector2(1400f, 180f));
        title.lineSpacing = -10f;
        TS(title, Color.white, new Vector2(4f, -4f)); // White drop shadow/outline

        // Subtitle box
        var subPill = Panel("SubtitlePill", root, new Color32(255, 245, 220, 255), new Color32(220, 190, 120, 255), new Vector2(0, 220f), new Vector2(1200f, 80f));
        NT("SubtitleTxt", subPill.transform, "Terokai faktor yang mempengaruhi pertumbuhan mikroorganisma.\nPilih faktor untuk mula meneroka!", 26f, new Color32(80, 60, 40, 255), TextAlignmentOptions.Center, Vector2.zero, new Vector2(1100f, 80f));

        // 3. Decorations
        var microbe = NI("MicrobeGreen", root, "Assets/Images/Akt4FaktorUI/faktor_microbe_green.png", Color.white, new Vector2(-750f, 300f), new Vector2(350f, 350f));
        microbe.preserveAspect = true;
        
        var waterDrop = NI("WaterDrop", root, "Assets/Images/Akt4FaktorUI/faktor_water_drop.png", Color.white, new Vector2(-750f, -300f), new Vector2(350f, 350f));
        waterDrop.preserveAspect = true;

        // 4. Factor Buttons
        // We link them to the 5 source buttons in `Center`.
        Button[] sources = new Button[5];
        if (center != null)
        {
            sources[0] = FindBtn(center, "Button");
            sources[1] = FindBtn(center, "Button (1)");
            sources[2] = FindBtn(center, "Button (2)");
            sources[3] = FindBtn(center, "Button (3)");
            sources[4] = FindBtn(center, "Button (4)");
        }

        float cardW = 550f;
        float cardH = 120f;
        
        // Suhu (Red)
        BuildFactorButton("Btn_Suhu", root, new Vector2(-320f, 60f), new Vector2(cardW, cardH), new Color32(230, 40, 40, 255), "1", "SUHU", sources[0]);
        // Kehadiran Air (Blue)
        BuildFactorButton("Btn_Air", root, new Vector2(320f, 60f), new Vector2(cardW, cardH), new Color32(40, 130, 230, 255), "2", "KEHADIRAN AIR", sources[1]);
        // Kehadiran Udara (Green)
        BuildFactorButton("Btn_Udara", root, new Vector2(-320f, -80f), new Vector2(cardW, cardH), new Color32(60, 180, 50, 255), "3", "KEHADIRAN UDARA", sources[2]);
        // Kehadiran Nutrient (Orange)
        BuildFactorButton("Btn_Nutrient", root, new Vector2(320f, -80f), new Vector2(cardW, cardH), new Color32(240, 140, 20, 255), "4", "KEHADIRAN NUTRIENT", sources[3]);
        // Keasidan Yang Sesuai (Purple)
        BuildFactorButton("Btn_Keasidan", root, new Vector2(0f, -220f), new Vector2(cardW, cardH), new Color32(140, 60, 220, 255), "5", "KEASIDAN YANG SESUAI", sources[4]);

        // 5. Navigation Buttons
        Button homeSrc = null;
        Button nextSrc = null;
        if (bottom != null)
        {
            homeSrc = FindBtn(bottom, "HomeButton");
            nextSrc = FindBtn(bottom, "NextButton");
        }

        BuildNavButton("HomeButton_Redesign", root, homeSrc, new Vector2(-750f, -440f), new Vector2(250f, 70f), new Color32(230, 140, 30, 255), "KEMBALI", true);
        BuildNavButton("NextButton_Redesign", root, nextSrc, new Vector2(750f, -440f), new Vector2(250f, 70f), new Color32(80, 180, 30, 255), "SETERUSNYA", false);

        EditorSceneManager.MarkSceneDirty(scene);
    }

    static void BuildFactorButton(string name, RectTransform parent, Vector2 pos, Vector2 size, Color32 color, string number, string label, Button source)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        go.layer = parent.gameObject.layer;
        R(go.transform, pos, size);

        Image img = go.AddComponent<Image>();
        img.sprite = SP(White);
        img.type = Image.Type.Sliced;
        img.color = color;
        OL(go, Color.white, 4f); // White inner outline effect
        SH(go);

        Button btn = go.AddComponent<Button>();
        btn.targetGraphic = img;
        BC(btn, color, Lighter(color, 0.15f), Darker(color, 0.15f), Color.gray);
        btn.navigation = new Navigation { mode = Navigation.Mode.None };

        // Number circle
        float circleSize = size.y - 20f;
        var circle = NI("NumCircle", go.transform, "Assets/Images/UI Flat/ui_circle_white.png", Color.white, new Vector2(-size.x/2f + circleSize/2f + 10f, 0f), new Vector2(circleSize, circleSize));
        OL(circle.gameObject, Darker(color, 0.2f), 2f);
        NT("NumTxt", circle.transform, number, 50f, color, TextAlignmentOptions.Center, Vector2.zero, new Vector2(circleSize, circleSize));

        // Label
        var lbl = NT("LabelTxt", go.transform, label, 35f, Color.white, TextAlignmentOptions.Center, new Vector2(circleSize/2f + 10f, 0f), new Vector2(size.x - circleSize - 40f, size.y));
        lbl.fontStyle = FontStyles.Bold;
        TS(lbl, new Color(0,0,0,0.3f), new Vector2(2f, -2f));

        if (source != null)
        {
            CenterMenuButtonRelay relay = go.AddComponent<CenterMenuButtonRelay>();
            var so = new SerializedObject(relay);
            so.FindProperty("sourceButton").objectReferenceValue = source;
            so.ApplyModifiedPropertiesWithoutUndo();
            UnityEventTools.AddPersistentListener(btn.onClick, relay.InvokeSource);
        }
    }

    static void BuildNavButton(string name, RectTransform parent, Button source, Vector2 pos, Vector2 size, Color32 color, string label, bool arrowLeft)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        go.layer = parent.gameObject.layer;
        R(go.transform, pos, size);

        Image img = go.AddComponent<Image>();
        img.sprite = SP(White);
        img.type = Image.Type.Sliced;
        img.color = color;
        OL(go, Color.white, 3f);
        SH(go);

        Button btn = go.AddComponent<Button>();
        btn.targetGraphic = img;
        BC(btn, color, Lighter(color, 0.12f), Darker(color, 0.12f), Color.gray);
        btn.navigation = new Navigation { mode = Navigation.Mode.None };

        var lbl = NT("Label", go.transform, label, 28f, Color.white, TextAlignmentOptions.Center, Vector2.zero, size);
        lbl.fontStyle = FontStyles.Bold;
        TS(lbl, new Color(0,0,0,0.3f), new Vector2(2f, -2f));

        if (source != null)
        {
            CenterMenuButtonRelay relay = go.AddComponent<CenterMenuButtonRelay>();
            var so = new SerializedObject(relay);
            so.FindProperty("sourceButton").objectReferenceValue = source;
            so.ApplyModifiedPropertiesWithoutUndo();
            UnityEventTools.AddPersistentListener(btn.onClick, relay.InvokeSource);
        }
    }

    // --- Helpers ---
    static RectTransform MakeRoot(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.layer = parent.gameObject.layer;
        go.transform.SetParent(parent, false);
        RectTransform r = go.GetComponent<RectTransform>();
        r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one;
        r.pivot = new Vector2(0.5f, 0.5f);
        r.offsetMin = r.offsetMax = Vector2.zero;
        r.localScale = Vector3.one;
        r.SetAsFirstSibling();
        return r;
    }

    static Image Panel(string name, Transform parent, Color fill, Color border, Vector2 pos, Vector2 size)
    {
        Image img = NI(name, parent, White, fill, pos, size);
        img.type = Image.Type.Sliced;
        OL(img.gameObject, border, 3f);
        return img;
    }

    static Image NI(string name, Transform parent, string path, Color color, Vector2 pos, Vector2 size)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.layer = parent.gameObject.layer;
        go.transform.SetParent(parent, false);
        R(go.transform, pos, size);
        Image img = go.GetComponent<Image>();
        img.sprite = SP(path);
        img.color = color;
        img.raycastTarget = false;
        return img;
    }

    static TextMeshProUGUI NT(string name, Transform parent, string value, float size, Color color, TextAlignmentOptions align, Vector2 pos, Vector2 rectSize)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        go.layer = parent.gameObject.layer;
        go.transform.SetParent(parent, false);
        R(go.transform, pos, rectSize);
        var t = go.GetComponent<TextMeshProUGUI>();
        t.font = _font;
        t.text = value;
        t.fontSize = size;
        t.color = color;
        t.alignment = align;
        t.enableWordWrapping = false;
        t.overflowMode = TextOverflowModes.Overflow;
        t.raycastTarget = false;
        t.richText = true;
        return t;
    }

    static void R(Transform t, Vector2 pos, Vector2 size)
    {
        RectTransform r = t as RectTransform ?? t.GetComponent<RectTransform>();
        if (r == null) return;
        r.anchorMin = r.anchorMax = r.pivot = new Vector2(0.5f, 0.5f);
        r.anchoredPosition = pos;
        r.sizeDelta = size;
        r.localScale = Vector3.one;
        r.localRotation = Quaternion.identity;
    }

    static void OL(GameObject go, Color color, float w)
    {
        Outline o = go.GetComponent<Outline>() ?? go.AddComponent<Outline>();
        o.effectColor = color;
        o.effectDistance = new Vector2(w, -w);
        o.useGraphicAlpha = true;
    }

    static void SH(GameObject go)
    {
        Shadow s = go.GetComponents<Shadow>().FirstOrDefault(x => !(x is Outline));
        if (s == null) s = go.AddComponent<Shadow>();
        s.effectColor = new Color(0.1f, 0.05f, 0f, 0.28f);
        s.effectDistance = new Vector2(0f, -7f);
        s.useGraphicAlpha = true;
    }

    static void TS(TextMeshProUGUI txt, Color color, Vector2 dist)
    {
        Shadow s = txt.GetComponents<Shadow>().FirstOrDefault(x => !(x is Outline));
        if (s == null) s = txt.gameObject.AddComponent<Shadow>();
        s.effectColor = color;
        s.effectDistance = dist;
        s.useGraphicAlpha = true;
    }

    static void BC(Button b, Color normal, Color hi, Color press, Color dis)
    {
        ColorBlock c = b.colors;
        c.normalColor = normal;
        c.highlightedColor = hi;
        c.selectedColor = hi;
        c.pressedColor = press;
        c.disabledColor = dis;
        c.colorMultiplier = 1f;
        c.fadeDuration = 0.12f;
        b.colors = c;
        b.transition = Selectable.Transition.ColorTint;
    }

    static Sprite SP(string path) => AssetDatabase.LoadAssetAtPath<Sprite>(path);

    static Button FindBtn(Transform parent, string name)
    {
        Transform t = parent.Find(name);
        return t != null ? t.GetComponent<Button>() : null;
    }

    static void ConfigureSprite(string path)
    {
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
        var imp = AssetImporter.GetAtPath(path) as TextureImporter;
        if (imp == null) return;
        imp.textureType = TextureImporterType.Sprite;
        imp.spriteImportMode = SpriteImportMode.Single;
        imp.alphaIsTransparency = true;
        imp.mipmapEnabled = false;
        imp.wrapMode = TextureWrapMode.Clamp;
        imp.filterMode = FilterMode.Bilinear;
        imp.maxTextureSize = 2048;
        imp.SaveAndReimport();
    }

    static Color Lighter(Color c, float t) => new Color(Mathf.Lerp(c.r, 1f, t), Mathf.Lerp(c.g, 1f, t), Mathf.Lerp(c.b, 1f, t), c.a);
    static Color Darker(Color c, float t) => new Color(c.r * (1f - t), c.g * (1f - t), c.b * (1f - t), c.a);
}
#endif
