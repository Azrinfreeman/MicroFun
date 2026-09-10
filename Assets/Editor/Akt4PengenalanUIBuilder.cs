#if UNITY_EDITOR
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Builds the AKT4 Pengenalan two-card Q&A page matching the reference image.
/// Run from Tools > MicroFun > Activity 4 > Build Pengenalan UI
/// The builder is idempotent: re-running destroys and rebuilds Akt4PengenalanRoot.
/// All legacy Pengenalan objects remain serialized; Bottom nav callbacks are relayed.
/// </summary>
public static class Akt4PengenalanUIBuilder
{
    // ── Paths ────────────────────────────────────────────────────────────────
    const string ScenePath   = "Assets/Scenes/Aktivity/akt4/akt4.unity";
    const string BackupDir   = "Assets/Scenes/Aktivity/akt4/Backups";
    const string BackupPath  = "Assets/Scenes/Aktivity/akt4/Backups/akt4_PrePengenalanRedesign_20260905.unity";
    const string RootName    = "Akt4PengenalanRoot";

    const string BgPath      = "Assets/Images/Akt4CenterUI/Background/akt4_center_background.png";
    const string White       = "Assets/Images/UI Flat/btn_white_flat.png";
    const string Circle      = "Assets/Images/UI Flat/ui_circle_white.png";
    const string Book        = "Assets/Images/UI Flat/icon_book_open.png";
    const string Lightbulb   = "Assets/Images/UI Flat/icon_lightbulb.png";
    const string MilkBoy     = "Assets/Images/2d/milk_boy.png";
    const string PurpleMicro = "Assets/Images/UI Flat/char_purple_microbe.png";
    const string GreenMicro  = "Assets/Images/UI Flat/char_green_microbe.png";
    const string WaterDrop   = "Assets/Images/UI Flat/char_water_drop.png";
    const string Flask       = "Assets/Images/UI Flat/icon_flask.png";
    const string HomeIcon    = "Assets/Images/UI Flat/icon_home.png";
    const string ArrowIcon   = "Assets/Images/UI Flat/icon_arrow_left.png";
    const string FontPath    = "Assets/Fonts/FredokaOne SDF.asset";

    // Generated transparent PNG assets
    const string TeacherPath = "Assets/Images/Akt4PengenalanUI/akt4_teacher_char.png";
    const string YeastPath   = "Assets/Images/Akt4PengenalanUI/akt4_yeast_package.png";
    const string TargetPath  = "Assets/Images/Akt4PengenalanUI/akt4_target_icon.png";

    // ── Colours ──────────────────────────────────────────────────────────────
    static readonly Color Cream      = new Color32(255, 252, 235, 255);
    static readonly Color CreamBadge = new Color32(255, 250, 218, 255);
    static readonly Color Green      = new Color32(87,  188,  16, 255);
    static readonly Color DarkGreen  = new Color32(18,  105,  14, 255);
    static readonly Color InkGreen   = new Color32(8,    70,  30, 255);
    static readonly Color Pink       = new Color32(245, 130, 170, 255);
    static readonly Color PinkBadge  = new Color32(255, 120, 170, 255);
    static readonly Color LightBlue  = new Color32(232, 245, 255, 255);
    static readonly Color Blue       = new Color32(60,  150, 230, 255);
    static readonly Color DarkBlue   = new Color32(20,   60, 140, 255);
    static readonly Color Orange     = new Color32(240, 145,  30, 255);
    static readonly Color DarkBrown  = new Color32(60,   30,  10, 255);

    static TMP_FontAsset _font;

    // ── Entry point ──────────────────────────────────────────────────────────
    [MenuItem("Tools/MicroFun/Activity 4/Build Pengenalan UI")]
    public static void Build()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogError("[Akt4Pengenalan] Exit Play Mode before building."); return;
        }

        _font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        if (_font == null) { Debug.LogError("[Akt4Pengenalan] FredokaOne font not found at " + FontPath); return; }

        // Ensure generated images are imported as sprites
        ConfigureSprite(BgPath);
        ConfigureSprite(TeacherPath);
        ConfigureSprite(YeastPath);
        ConfigureSprite(TargetPath);

        // Backup once
        if (!AssetDatabase.IsValidFolder(BackupDir))
            AssetDatabase.CreateFolder("Assets/Scenes/Aktivity/akt4", "Backups");
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(BackupPath) == null)
            AssetDatabase.CopyAsset(ScenePath, BackupPath);

        var scene = EditorSceneManager.GetActiveScene();
        GameObject canvasGO = GameObject.Find("Canvas");
        if (canvasGO == null) { Debug.LogError("[Akt4Pengenalan] Canvas not found."); return; }

        Transform canvas = canvasGO.transform;
        Transform page   = canvas.Find("Pengenalan");
        if (page == null) { Debug.LogError("[Akt4Pengenalan] Canvas/Pengenalan not found."); return; }

        // Locate existing nav buttons BEFORE hiding anything
        Transform bottom   = page.Find("Bottom");
        Button homeBtn     = bottom != null ? FindBtn(bottom, "HomeButton") : null;
        Button nextBtn     = bottom != null ? FindBtn(bottom, "NextButton") : null;

        // Destroy old root (idempotent)
        var old = page.Find(RootName);
        if (old != null) Object.DestroyImmediate(old.gameObject);

        // Hide legacy visual groups; keep Bottom + DialogParent alive
        SetActive(page, "Title",   false);
        SetActive(page, "Content", false);
        SetActive(page, "Top",     false);
        if (bottom != null) bottom.gameObject.SetActive(false); // hidden; relayed

        // Hide DialogParent at start — it is re-activated by the existing dialog trigger
        var dp = page.Find("DialogParent");
        if (dp != null)
        {
            dp.gameObject.SetActive(false);
            EditorUtility.SetDirty(dp.gameObject);
        }

        // Build new root
        RectTransform root = MakeRoot(RootName, page);

        BuildBackground(root);
        BuildDecorations(root);
        BuildHeader(root);
        BuildQuestionCard(root);
        BuildAnswerCard(root);
        BuildMainIdeaBar(root);
        BuildNavButtons(root, homeBtn, nextBtn);

        // Configure canvas scaler
        CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
        if (scaler != null)
        {
            scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode     = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight  = 0.5f;
        }

        EditorUtility.SetDirty(page.gameObject);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveOpenScenes();
        Selection.activeGameObject = root.gameObject;
        Debug.Log("[Akt4Pengenalan] Build complete. Scene saved.");
    }

    // ── Background ───────────────────────────────────────────────────────────
    static void BuildBackground(RectTransform root)
    {
        var img = root.gameObject.AddComponent<Image>();
        img.sprite         = SP(BgPath);
        img.color          = Color.white;
        img.preserveAspect = false;
        img.raycastTarget  = false;
    }

    // ── Decorations ──────────────────────────────────────────────────────────
    static void BuildDecorations(RectTransform root)
    {
        // Purple microbe — top right
        NI("PurpleMicrobe", root, PurpleMicro, Color.white, new Vector2(820f, 410f), new Vector2(190f, 190f));
        // Green microbe — between question card and answer card at bottom
        NI("GreenMicrobe",  root, GreenMicro,  Color.white, new Vector2(10f, -315f),  new Vector2(95f, 95f));
        // Water drop — lower right near bottom corner of answer card
        NI("WaterDrop",     root, WaterDrop,   Color.white, new Vector2(620f, -330f), new Vector2(90f, 90f));
        // Flask — lower left corner
        NI("Flask",         root, Flask,       Color.white, new Vector2(-870f, -380f), new Vector2(85f, 115f));
    }

    // ── Header ───────────────────────────────────────────────────────────────
    static void BuildHeader(RectTransform root)
    {
        // "AKTIVITI 4" badge
        Image badge = Panel("ActivityBadge", root, CreamBadge, Green, new Vector2(0f, 480f), new Vector2(340f, 68f));
        SH(badge.gameObject);
        NT("Text", badge.transform, "AKTIVITI 4", 38f, InkGreen, TextAlignmentOptions.Center, Vector2.zero, new Vector2(310f, 58f))
            .fontStyle = FontStyles.Bold;

        // Green title bar
        Image bar = Panel("TitleBar", root, Green, DarkGreen, new Vector2(0f, 400f), new Vector2(860f, 105f));
        SH(bar.gameObject);

        // Book medallion on left of title bar
        Image bookCircle = NI("BookCircle", bar.transform, Circle, new Color32(55, 155, 10, 255),
            new Vector2(-358f, 0f), new Vector2(118f, 118f));
        bookCircle.preserveAspect = true;
        OL(bookCircle.gameObject, Color.white, 4f);
        SH(bookCircle.gameObject);
        var bookImg = NI("BookIcon", bookCircle.transform, Book, Color.white, Vector2.zero, new Vector2(88f, 88f));
        bookImg.preserveAspect = true;

        // "PENGENALAN" text
        var titleTxt = NT("Label", bar.transform, "PENGENALAN", 58f, Color.white,
            TextAlignmentOptions.Center, new Vector2(42f, 0f), new Vector2(650f, 95f));
        titleTxt.fontStyle = FontStyles.Bold;
        TS(titleTxt, new Color(0f, 0f, 0f, 0.28f), new Vector2(3f, -4f));
    }

    // ── Question Card ────────────────────────────────────────────────────────
    static void BuildQuestionCard(RectTransform root)
    {
        Image card = Panel("QuestionCard", root, Cream, Pink, new Vector2(-448f, -50f), new Vector2(836f, 550f));
        OL(card.gameObject, Pink, 4f);
        SH(card.gameObject);

        // Pink question badge overlapping top edge
        Image qBadge = NI("QuestionBadge", card.transform, Circle, PinkBadge,
            new Vector2(0f, 308f), new Vector2(102f, 102f));
        qBadge.preserveAspect = true;
        OL(qBadge.gameObject, Color.white, 4f);
        SH(qBadge.gameObject);
        var qMark = NT("Q", qBadge.transform, "?", 68f, Color.white, TextAlignmentOptions.Center,
            Vector2.zero, new Vector2(90f, 90f));
        qMark.fontStyle = FontStyles.Bold;

        // Question text
        var qText = NT("QuestionText", card.transform,
            "Mengapa yis dalam\nkotak ini tidak bertumbuh?",
            44f, DarkBrown, TextAlignmentOptions.Center,
            new Vector2(0f, 130f), new Vector2(720f, 200f));
        qText.fontStyle      = FontStyles.Bold;
        qText.enableWordWrapping = true;

        // Student character — lower left
        var student = NI("StudentChar", card.transform, MilkBoy, Color.white,
            new Vector2(-260f, -135f), new Vector2(240f, 380f));
        student.preserveAspect = true;

        // Yeast package — lower center-right
        var yeast = NI("YeastPackage", card.transform, YeastPath, Color.white,
            new Vector2(140f, -135f), new Vector2(290f, 310f));
        yeast.preserveAspect = true;
    }

    // ── Answer Card ──────────────────────────────────────────────────────────
    static void BuildAnswerCard(RectTransform root)
    {
        Image card = Panel("AnswerCard", root, LightBlue, Blue, new Vector2(450f, -50f), new Vector2(856f, 550f));
        OL(card.gameObject, Blue, 4f);
        SH(card.gameObject);

        // "JAWAPAN" header tab overlapping top
        Image jawapanTab = Panel("JawapanHeader", card.transform, Blue, DarkBlue,
            new Vector2(-130f, 308f), new Vector2(360f, 70f));
        SH(jawapanTab.gameObject);

        // Lightbulb circle on left of tab
        Image bulbCircle = NI("LightbulbCircle", jawapanTab.transform, Circle,
            new Color32(255, 200, 30, 255), new Vector2(-138f, 0f), new Vector2(72f, 72f));
        bulbCircle.preserveAspect = true;
        OL(bulbCircle.gameObject, Color.white, 3f);
        var bulbImg = NI("Bulb", bulbCircle.transform, Lightbulb, Color.white, Vector2.zero, new Vector2(52f, 52f));
        bulbImg.preserveAspect = true;

        var jawapanTxt = NT("Label", jawapanTab.transform, "JAWAPAN", 46f, Color.white,
            TextAlignmentOptions.Center, new Vector2(22f, 0f), new Vector2(280f, 60f));
        jawapanTxt.fontStyle = FontStyles.Bold;
        TS(jawapanTxt, new Color(0f, 0.1f, 0.3f, 0.3f), new Vector2(2f, -3f));

        // Answer 1
        BuildAnswerRow(card.transform, "AnswerOne",
            "Yis dalam kotak ini tidak\nbertumbuh kerana masih\n<color=#E0335A>tidak aktif</color>.",
            new Vector2(-30f, 130f), new Vector2(680f, 170f));

        // Divider
        GameObject div = new GameObject("Divider", typeof(RectTransform));
        div.transform.SetParent(card.transform, false);
        R(div.transform, new Vector2(-30f, -20f), new Vector2(680f, 2f));
        var divImg = div.AddComponent<Image>();
        divImg.color         = new Color32(160, 200, 240, 180);
        divImg.raycastTarget = false;

        // Answer 2
        BuildAnswerRow(card.transform, "AnswerTwo",
            "Mikroorganisma akan\n<color=#28A655>bertumbuh</color> jika semua\n<color=#9932CC>keperluan</color> untuk\npertumbuhannya mencukupi.",
            new Vector2(-30f, -155f), new Vector2(680f, 220f));

        // Educator / teacher character — right side, standing prominently beside card
        var teacher = NI("EducatorChar", card.transform, TeacherPath, Color.white,
            new Vector2(390f, -30f), new Vector2(320f, 560f));
        teacher.preserveAspect = true;
    }

    static void BuildAnswerRow(Transform parent, string name, string text, Vector2 pos, Vector2 size)
    {
        GameObject row = new GameObject(name, typeof(RectTransform));
        row.transform.SetParent(parent, false);
        R(row.transform, pos, size);

        // Blue filled circle with white "V" shape (two rectangles) as checkmark
        Image check = NI("CheckCircle", row.transform, Circle, Blue,
            new Vector2(-(size.x * 0.5f - 34f), size.y * 0.5f - 34f), new Vector2(52f, 52f));
        check.preserveAspect = true;
        OL(check.gameObject, DarkBlue, 2f);
        // Short bar of checkmark
        var shortBar = new GameObject("Short", typeof(RectTransform)); shortBar.transform.SetParent(check.transform, false);
        R(shortBar.transform, new Vector2(-6f, -3f), new Vector2(7f, 18f));
        shortBar.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0f, 0f, 45f);
        var si = shortBar.AddComponent<Image>(); si.color = Color.white; si.raycastTarget = false;
        // Long bar of checkmark
        var longBar = new GameObject("Long", typeof(RectTransform)); longBar.transform.SetParent(check.transform, false);
        R(longBar.transform, new Vector2(7f, 1f), new Vector2(7f, 28f));
        longBar.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0f, 0f, -45f);
        var li = longBar.AddComponent<Image>(); li.color = Color.white; li.raycastTarget = false;

        // Answer text
        var txt = NT("Text", row.transform, text, 26f, DarkBlue, TextAlignmentOptions.TopLeft,
            new Vector2(32f, 0f), new Vector2(size.x - 80f, size.y));
        txt.richText            = true;
        txt.enableWordWrapping  = true;
        txt.fontStyle           = FontStyles.Bold;
    }

    // ── Main Idea Bar ────────────────────────────────────────────────────────
    static void BuildMainIdeaBar(RectTransform root)
    {
        Image bar = Panel("MainIdeaBar", root, Cream, Orange, new Vector2(0f, -420f), new Vector2(1400f, 105f));
        SH(bar.gameObject);

        // Target icon badge
        Image tgtCircle = NI("TargetCircle", bar.transform, Circle, new Color32(255, 200, 30, 255),
            new Vector2(-625f, 0f), new Vector2(76f, 76f));
        tgtCircle.preserveAspect = true;
        OL(tgtCircle.gameObject, Color.white, 3f);
        SH(tgtCircle.gameObject);
        var tgtImg = NI("TargetIcon", tgtCircle.transform, TargetPath, Color.white,
            Vector2.zero, new Vector2(58f, 58f));
        tgtImg.preserveAspect = true;

        // Unified idea text (no overlapping labels)
        var txt = NT("IdeaCombinedText", bar.transform,
            "<color=#14821E>Idea utama:</color>  <color=#143C8C>Mikroorganisma memerlukan keadaan yang sesuai untuk bertumbuh.</color>",
            28f, DarkBlue, TextAlignmentOptions.MidlineLeft,
            new Vector2(40f, 0f), new Vector2(1200f, 85f));
        txt.fontStyle          = FontStyles.Bold;
        txt.richText           = true;
        txt.enableWordWrapping = true;
    }

    // ── Navigation relay buttons ─────────────────────────────────────────────
    static void BuildNavButtons(RectTransform root, Button homeSource, Button nextSource)
    {
        // Home button — top-left corner
        BuildRelay("HomeButton", root, HomeIcon, homeSource,
            new Vector2(-880f, 455f), new Vector2(78f, 78f), new Color32(45, 140, 220, 255));

        // Next button — bottom-right, inset from edge
        BuildRelay("NextButton", root, ArrowIcon, nextSource,
            new Vector2(745f, -445f), new Vector2(245f, 68f), new Color32(87, 188, 16, 255),
            "SETERUSNYA", flipArrow: true, arrowOnRight: true);
    }

    static void BuildRelay(string name, RectTransform parent, string iconPath, Button source,
        Vector2 pos, Vector2 size, Color32 color, string label = null, bool flipArrow = false, bool arrowOnRight = false)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        go.layer = parent.gameObject.layer;
        R(go.transform, pos, size);

        Image img  = go.AddComponent<Image>();
        img.sprite = SP(White);
        img.type   = Image.Type.Sliced;
        img.color  = color;
        OL(go, Color.white, 3f);
        SH(go);

        Button btn = go.AddComponent<Button>();
        btn.targetGraphic = img;
        BC(btn, color, Lighter(color, 0.12f), Darker(color, 0.12f), Color.gray);
        btn.navigation = new Navigation { mode = Navigation.Mode.None };

        // Icon
        Vector2 iconPos = Vector2.zero;
        if (!string.IsNullOrEmpty(label))
            iconPos = arrowOnRight ? new Vector2(size.x * 0.5f - 34f, 0f) : new Vector2(-size.x * 0.5f + 34f, 0f);

        var iconGO = NI("Icon", go.transform, iconPath, Color.white, iconPos, new Vector2(44f, 44f));
        iconGO.preserveAspect = true;
        if (flipArrow) iconGO.rectTransform.localScale = new Vector3(-1f, 1f, 1f);

        if (!string.IsNullOrEmpty(label))
        {
            var lbl = NT("Label", go.transform, label, 25f, Color.white,
                TextAlignmentOptions.Center,
                arrowOnRight ? new Vector2(-22f, 0f) : new Vector2(22f, 0f),
                new Vector2(size.x - 76f, size.y));
            lbl.fontStyle = FontStyles.Bold;
        }

        if (source != null)
        {
            CenterMenuButtonRelay relay = go.AddComponent<CenterMenuButtonRelay>();
            var so = new SerializedObject(relay);
            so.FindProperty("sourceButton").objectReferenceValue = source;
            so.ApplyModifiedPropertiesWithoutUndo();
            UnityEventTools.AddPersistentListener(btn.onClick, relay.InvokeSource);
        }
    }

    // ── Helpers ──────────────────────────────────────────────────────────────
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
        Image img  = go.GetComponent<Image>();
        img.sprite = SP(path);
        img.color  = color;
        img.raycastTarget = false;
        return img;
    }

    static TextMeshProUGUI NT(string name, Transform parent, string value, float size,
        Color color, TextAlignmentOptions align, Vector2 pos, Vector2 rectSize)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        go.layer = parent.gameObject.layer;
        go.transform.SetParent(parent, false);
        R(go.transform, pos, rectSize);
        var t = go.GetComponent<TextMeshProUGUI>();
        t.font                = _font;
        t.text                = value;
        t.fontSize            = size;
        t.color               = color;
        t.alignment           = align;
        t.enableWordWrapping  = false;
        t.overflowMode        = TextOverflowModes.Overflow;
        t.raycastTarget       = false;
        t.richText            = true;
        return t;
    }

    static void R(Transform t, Vector2 pos, Vector2 size)
    {
        RectTransform r = t as RectTransform ?? t.GetComponent<RectTransform>();
        if (r == null) return;
        r.anchorMin = r.anchorMax = r.pivot = new Vector2(0.5f, 0.5f);
        r.anchoredPosition = pos;
        r.sizeDelta        = size;
        r.localScale       = Vector3.one;
        r.localRotation    = Quaternion.identity;
    }

    static void OL(GameObject go, Color color, float w)
    {
        Outline o = go.GetComponent<Outline>() ?? go.AddComponent<Outline>();
        o.effectColor    = color;
        o.effectDistance = new Vector2(w, -w);
        o.useGraphicAlpha = true;
    }

    static void SH(GameObject go)
    {
        Shadow s = go.GetComponents<Shadow>().FirstOrDefault(x => !(x is Outline));
        if (s == null) s = go.AddComponent<Shadow>();
        s.effectColor    = new Color(0.1f, 0.05f, 0f, 0.28f);
        s.effectDistance = new Vector2(0f, -7f);
        s.useGraphicAlpha = true;
    }

    static void TS(TextMeshProUGUI txt, Color color, Vector2 dist)
    {
        Shadow s = txt.GetComponents<Shadow>().FirstOrDefault(x => !(x is Outline));
        if (s == null) s = txt.gameObject.AddComponent<Shadow>();
        s.effectColor    = color;
        s.effectDistance = dist;
        s.useGraphicAlpha = true;
    }

    static void BC(Button b, Color normal, Color hi, Color press, Color dis)
    {
        ColorBlock c = b.colors;
        c.normalColor      = normal;
        c.highlightedColor = hi;
        c.selectedColor    = hi;
        c.pressedColor     = press;
        c.disabledColor    = dis;
        c.colorMultiplier  = 1f;
        c.fadeDuration     = 0.12f;
        b.colors = c;
        b.transition = Selectable.Transition.ColorTint;
    }

    static Sprite SP(string path)
        => AssetDatabase.LoadAssetAtPath<Sprite>(path);

    static Button FindBtn(Transform parent, string name)
    {
        Transform t = parent.Find(name);
        return t != null ? t.GetComponent<Button>() : null;
    }

    static void SetActive(Transform parent, string childName, bool active)
    {
        Transform t = parent.Find(childName);
        if (t != null) t.gameObject.SetActive(active);
    }

    static void ConfigureSprite(string path)
    {
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
        var imp = AssetImporter.GetAtPath(path) as TextureImporter;
        if (imp == null) return;
        imp.textureType      = TextureImporterType.Sprite;
        imp.spriteImportMode = SpriteImportMode.Single;
        imp.alphaIsTransparency = true;
        imp.mipmapEnabled    = false;
        imp.wrapMode         = TextureWrapMode.Clamp;
        imp.filterMode       = FilterMode.Bilinear;
        imp.maxTextureSize   = 2048;
        imp.SaveAndReimport();
    }

    static Color Lighter(Color c, float t) =>
        new Color(Mathf.Lerp(c.r, 1f, t), Mathf.Lerp(c.g, 1f, t), Mathf.Lerp(c.b, 1f, t), c.a);
    static Color Darker(Color c, float t) =>
        new Color(c.r * (1f - t), c.g * (1f - t), c.b * (1f - t), c.a);
}
#endif
