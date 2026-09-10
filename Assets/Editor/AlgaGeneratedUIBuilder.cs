using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class AlgaGeneratedUIBuilder
{
    const string BackdropPath = "Assets/Images/Generated/Alga/AlgaBackdrop-v1.png";
    static readonly Color Navy = new Color32(8, 31, 82, 255);
    static readonly Color Cream = new Color32(255, 249, 225, 255);
    static readonly Color DarkGreen = new Color32(9, 86, 39, 255);

    [MenuItem("MicroFun/Build Alga Reference UI")]
    public static void Build()
    {
        var panel = GameObject.Find("Canvas/Alga/1");
        if (panel == null) throw new Exception("Canvas/Alga/1 was not found.");

        var legacyDialog = panel.transform.Find("DialogParent");

        var existing = panel.transform.Find("AlgaGeneratedReference");
        if (existing != null) UnityEngine.Object.DestroyImmediate(existing.gameObject);
        foreach (Transform child in panel.transform) child.gameObject.SetActive(false);

        ConfigureBackdrop();
        var root = New("AlgaGeneratedReference", panel.transform);
        Fill(root);

        var backdrop = New("Backdrop", root);
        var backdropImage = backdrop.gameObject.AddComponent<Image>();
        backdropImage.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(BackdropPath);
        backdropImage.preserveAspect = false;
        backdropImage.raycastTarget = false;
        Fill(backdrop);

        MakeTitle(root);
        MakeAudio(root, legacyDialog == null ? null : legacyDialog.gameObject);

        MakeFactCard(root, 235, new Color32(212, 243, 184, 255), new Color32(70, 172, 22, 255), "Assets/Images/UI Flat/icon_fun_habitat.png", "Mempunyai sifat tumbuhan.", 40);
        MakeFactCard(root, 80, new Color32(186, 229, 250, 255), new Color32(31, 151, 229, 255), "Assets/Images/UI Flat/icon_petri_dish.png", "Mengandungi klorofil.", 40);
        MakeFactCard(root, -75, new Color32(255, 226, 155, 255), new Color32(249, 156, 0, 255), "Assets/Images/UI Flat/icon_fun_nutrition.png", "Menghasilkan makanan sendiri\nmelalui fotosintesis.", 36);
        MakeFactCard(root, -230, new Color32(179, 237, 235, 255), new Color32(15, 146, 214, 255), "Assets/Images/droplet-removebg-preview.png", "Biasanya hidup di air.", 40);

        MakeExamples(root);
        MakeNavigation(root, panel);

        EditorUtility.SetDirty(panel);
        Debug.Log("Alga generated UI built.");
    }

    static Button FindButton(Transform parent, string path)
    {
        var t = parent.Find(path);
        return t == null ? null : t.GetComponent<Button>();
    }

    static void ConfigureBackdrop()
    {
        var importer = AssetImporter.GetAtPath(BackdropPath) as TextureImporter;
        if (importer != null && importer.textureType != TextureImporterType.Sprite)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.mipmapEnabled = false;
            importer.SaveAndReimport();
        }
    }

    static RectTransform New(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        var rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        return rt;
    }

    static void Fill(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    static void Place(RectTransform rt, Vector2 position, Vector2 size)
    {
        rt.anchorMin = rt.anchorMax = new Vector2(.5f, .5f);
        rt.pivot = new Vector2(.5f, .5f);
        rt.anchoredPosition = position;
        rt.sizeDelta = size;
    }

    static Sprite DefaultSprite()
    {
        return AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
    }

    static Image MakePanel(Transform parent, string name, Vector2 position, Vector2 size, Color color)
    {
        var rt = New(name, parent);
        Place(rt, position, size);
        var image = rt.gameObject.AddComponent<Image>();
        image.sprite = DefaultSprite();
        image.type = Image.Type.Sliced;
        image.color = color;
        return image;
    }

    static TextMeshProUGUI MakeText(Transform parent, string name, string content, Vector2 position, Vector2 size, float fontSize, TextAlignmentOptions alignment, Color color)
    {
        var rt = New(name, parent);
        Place(rt, position, size);
        var text = rt.gameObject.AddComponent<TextMeshProUGUI>();
        text.font = TMP_Settings.defaultFontAsset;
        text.text = content;
        text.fontSize = fontSize;
        text.color = color;
        text.fontStyle = FontStyles.Bold;
        text.alignment = alignment;
        text.enableWordWrapping = true;
        text.raycastTarget = false;
        text.margin = new Vector4(8, 4, 8, 4);
        return text;
    }

    static void MakeTitle(Transform root)
    {
        var title = MakeText(root, "Title", "ALGA", new Vector2(0, 420), new Vector2(720, 145), 114, TextAlignmentOptions.Center, Navy);
        var outline = title.gameObject.AddComponent<Outline>();
        outline.effectColor = Color.white;
        outline.effectDistance = new Vector2(5, -5);
        var shadow = title.gameObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(0, 0, 0, .22f);
        shadow.effectDistance = new Vector2(0, -8);
        // Background foliage provides the title accents without relying on unsupported glyphs.
    }

    static void MakeAudio(Transform root, GameObject legacyDialog)
    {
        var panel = MakePanel(root, "AudioButton", new Vector2(-742, 441), new Vector2(320, 70), Cream);
        var button = panel.gameObject.AddComponent<Button>();
        button.targetGraphic = panel;
        ConfigureRoute(panel.gameObject, 0);
        panel.gameObject.AddComponent<AnimatedButtonFeedback>();

        var disc = MakePanel(panel.transform, "SpeakerDisc", new Vector2(-126, 0), new Vector2(82, 82), new Color32(22, 110, 33, 255));
        var iconRt = New("SpeakerIcon", disc.transform);
        Place(iconRt, Vector2.zero, new Vector2(50, 50));
        var icon = iconRt.gameObject.AddComponent<Image>();
        icon.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Images/UI Flat/icon_speaker.png");
        icon.preserveAspect = true;
        icon.color = Color.white;
        icon.raycastTarget = false;
        MakeText(panel.transform, "AudioLabel", "Dengar audio", new Vector2(42, 0), new Vector2(210, 52), 29, TextAlignmentOptions.Center, DarkGreen);
    }

    static void MakeFactCard(Transform root, float y, Color fill, Color iconColor, string iconPath, string fact, float fontSize)
    {
        var outer = MakePanel(root, "FactCard", new Vector2(80, y), new Vector2(890, 136), new Color(1, 1, 1, .96f));
        var outerShadow = outer.gameObject.AddComponent<Shadow>();
        outerShadow.effectColor = new Color(0, 0, 0, .20f);
        outerShadow.effectDistance = new Vector2(0, -7);

        var inner = MakePanel(outer.transform, "Fill", Vector2.zero, new Vector2(872, 118), fill);
        inner.raycastTarget = false;

        var iconOuter = MakePanel(outer.transform, "IconOuter", new Vector2(-390, 0), new Vector2(132, 132), Color.white);
        iconOuter.raycastTarget = false;
        var iconDisc = MakePanel(iconOuter.transform, "IconDisc", Vector2.zero, new Vector2(110, 110), iconColor);
        iconDisc.raycastTarget = false;
        var iconRt = New("Icon", iconDisc.transform);
        Place(iconRt, Vector2.zero, new Vector2(76, 76));
        var glyph = iconRt.gameObject.AddComponent<Image>();
        glyph.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(iconPath);
        glyph.preserveAspect = true;
        glyph.raycastTarget = false;
        glyph.color = Color.white;

        var label = MakeText(outer.transform, "FactText", fact, new Vector2(65, 0), new Vector2(650, 108), fontSize, TextAlignmentOptions.MidlineLeft, Navy);
        label.lineSpacing = -8;
    }

    static void MakeExamples(Transform root)
    {
        var outer = MakePanel(root, "ExamplesPanel", new Vector2(75, -385), new Vector2(850, 114), new Color(1, 1, 1, .96f));
        var shadow = outer.gameObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(0, 0, 0, .20f);
        shadow.effectDistance = new Vector2(0, -6);
        var inner = MakePanel(outer.transform, "Fill", Vector2.zero, new Vector2(830, 94), new Color32(255, 245, 202, 255));
        inner.raycastTarget = false;
        var flaskRt = New("Flask", outer.transform);
        Place(flaskRt, new Vector2(-348, 0), new Vector2(76, 76));
        var flask = flaskRt.gameObject.AddComponent<Image>();
        flask.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Images/UI Flat/icon_flask.png");
        flask.preserveAspect = true;
        flask.raycastTarget = false;
        var label = MakeText(outer.transform, "ExampleText", "Contoh: <i>Chlorella, Chlamydomonas\ndan Volvox.</i>", new Vector2(80, 0), new Vector2(610, 92), 30, TextAlignmentOptions.MidlineLeft, Navy);
        label.lineSpacing = -10;
    }

    static void MakeNavigation(Transform root, GameObject algaPanel)
    {
        MakeNav(root, "UtamaButton", "Assets/Images/Generated/Akt2/Nav_Home.png", new Vector2(-810, -455), () =>
        {
            var alga = algaPanel.transform.parent == null ? algaPanel : algaPanel.transform.parent.gameObject;
            alga.SetActive(false);
            var center = GameObject.Find("Canvas/Center");
            if (center != null) center.SetActive(true);
        });

        MakeNav(root, "KembaliButton", "Assets/Images/Generated/Akt2/Nav_Back.png", new Vector2(0, -455), () =>
        {
            algaPanel.SetActive(false);
            var center = GameObject.Find("Canvas/Center");
            if (center != null) center.SetActive(true);
        });
    }

    static void ConfigureRoute(GameObject target, int action)
    {
        var route = target.AddComponent<AlgaGeneratedButtonRoute>();
        route.action = action;
    }

    static void MakeNav(Transform root, string name, string spritePath, Vector2 position, System.Action onClick)
    {
        var rt = New(name, root);
        Place(rt, position, new Vector2(128, 128));
        var image = rt.gameObject.AddComponent<Image>();
        image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
        image.preserveAspect = true;
        var button = rt.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        ConfigureRoute(rt.gameObject, name == "UtamaButton" ? 1 : 2);
        rt.gameObject.AddComponent<AnimatedButtonFeedback>();
    }
}
