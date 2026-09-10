using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class Akt2GeneratedMenuBuilder
{
    const string ArtPath = "Assets/Images/Generated/Akt2/Backdrop.png";
    static readonly Color Navy = new Color32(11, 31, 81, 255);
    static readonly Color Ink = new Color32(7, 28, 72, 255);
    static readonly Color Cream = new Color32(255, 249, 220, 255);

    [MenuItem("MicroFun/Build Generated AKT2 Menu")]
    public static void Build()
    {
        var center = GameObject.Find("Canvas/Center");
        if (center == null) throw new Exception("Canvas/Center was not found.");

        var legacyMenu = center.transform.Find("GameObject");
        if (legacyMenu == null) throw new Exception("AKT2 legacy menu buttons were not found.");
        var oldButtons = legacyMenu.GetComponentsInChildren<Button>(true);
        if (oldButtons.Length < 6) throw new Exception("AKT2 needs six legacy menu buttons.");

        var existing = center.transform.Find("Akt2GeneratedReference");
        if (existing != null) UnityEngine.Object.DestroyImmediate(existing.gameObject);

        foreach (Transform child in center.transform) child.gameObject.SetActive(false);
        var oldLayout = center.GetComponent<VerticalLayoutGroup>();
        if (oldLayout != null) UnityEngine.Object.DestroyImmediate(oldLayout);
        Fill(center.GetComponent<RectTransform>());

        var legacyBg = GameObject.Find("Canvas/Image");
        if (legacyBg != null) legacyBg.SetActive(false);

        ConfigureSprite();
        var root = New("Akt2GeneratedReference", center.transform);
        Fill(root);

        var artwork = New("GeneratedArtwork", root);
        var artImage = artwork.gameObject.AddComponent<Image>();
        artImage.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath);
        artImage.preserveAspect = false;
        artImage.raycastTarget = false;
        Fill(artwork);

        MakeTitle(root);
        MakeAudio(root);

        string[] labels = { "Pengenalan", "Definisi", "Jenis", "Elaborasi", "Kuiz" };
        string[] glyphs = { "▣", "⌕", "●", "✦", "☷" };
        Color[] colors =
        {
            new Color32(111, 210, 190, 255),
            new Color32(95, 181, 245, 255),
            new Color32(255, 204, 81, 255),
            new Color32(243, 128, 166, 255),
            new Color32(178, 133, 235, 255)
        };
        float[] y = { 325f, 205f, 85f, -35f, -155f };

        for (int i = 0; i < labels.Length; i++)
        {
            var b = MakeMenuButton(root, labels[i], glyphs[i], y[i], colors[i]);
            ConfigureRoute(b.gameObject, i, true);
        }

        MakeCurriculumPanel(root);
        var home = MakeHitArea(root, "UtamaButton", new Vector2(-810, -455), new Vector2(128, 128));
        ConfigureRoute(home.gameObject, 5, false);

        var back = MakeHitArea(root, "KembaliButton", new Vector2(0, -455), new Vector2(128, 128));
        ConfigureRoute(back.gameObject, 5, false);

        var next = MakeHitArea(root, "SeterusnyaButton", new Vector2(810, -455), new Vector2(128, 128));
        ConfigureRoute(next.gameObject, 0, true);

        EditorUtility.SetDirty(center);
        Debug.Log("AKT2 generated menu built.");
    }

    static void ConfigureSprite()
    {
        var importer = AssetImporter.GetAtPath(ArtPath) as TextureImporter;
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
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
    }

    static void Place(RectTransform rt, Vector2 position, Vector2 size)
    {
        rt.anchorMin = rt.anchorMax = new Vector2(.5f, .5f);
        rt.pivot = new Vector2(.5f, .5f);
        rt.anchoredPosition = position; rt.sizeDelta = size;
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
        text.enableWordWrapping = false;
        text.raycastTarget = false;
        text.margin = new Vector4(8, 4, 8, 4);
        return text;
    }

static void MakeTitle(Transform root)
    {
        var title = MakeText(root, "Title", "AKTIVITI 2", new Vector2(0, 500), new Vector2(840, 80), 56, TextAlignmentOptions.Center, Navy);
        var titleOutline = title.gameObject.AddComponent<Outline>();
        titleOutline.effectColor = Color.white;
        titleOutline.effectDistance = new Vector2(3, -3);

        var subtitle = MakePanel(root, "SubtitlePill", new Vector2(0, 430), new Vector2(670, 62), new Color32(67, 157, 14, 255));
        var shadow = subtitle.gameObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(0,0,0,.28f);
        shadow.effectDistance = new Vector2(0,-4);
        MakeText(subtitle.transform, "SubtitleText", "Alga, Protozoa dan Virus", Vector2.zero, new Vector2(640, 52), 31, TextAlignmentOptions.Center, Color.white);
    }

    static void MakeAudio(Transform root)
    {
        var panel = MakePanel(root, "AudioButton", new Vector2(-742, 441), new Vector2(320, 70), Cream);
        panel.gameObject.AddComponent<Button>();
        panel.gameObject.AddComponent<ButtonInteractionFeedback>();
        var disc = MakePanel(panel.transform, "SpeakerDisc", new Vector2(-126, 0), new Vector2(82, 82), new Color32(22, 110, 33, 255));
        var iconRt = New("SpeakerIcon", disc.transform);
        Place(iconRt, Vector2.zero, new Vector2(50, 50));
        var icon = iconRt.gameObject.AddComponent<Image>();
        icon.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Images/UI Flat/icon_speaker.png");
        icon.preserveAspect = true;
        icon.color = Color.white;
        icon.raycastTarget = false;
        MakeText(panel.transform, "AudioLabel", "Dengar audio", new Vector2(42, 0), new Vector2(210, 52), 29, TextAlignmentOptions.Center, new Color32(7,79,29,255));
    }

static Button MakeMenuButton(Transform root, string label, string glyph, float y, Color color)
    {
        var rt = New(label + "Button", root);
        Place(rt, new Vector2(0, y), new Vector2(660, 110));
        var image = rt.gameObject.AddComponent<Image>();
        image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Images/Generated/Akt2/Menu_" + label + ".png");
        image.preserveAspect = true;
        image.raycastTarget = true;
        var button = rt.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        rt.gameObject.AddComponent<AnimatedButtonFeedback>();

        var labelText = MakeText(rt, "Label", label, new Vector2(62, 0), new Vector2(485, 76), 40, TextAlignmentOptions.Center, Ink);
        var labelOutline = labelText.gameObject.AddComponent<Outline>();
        labelOutline.effectColor = new Color(1,1,1,.38f);
        labelOutline.effectDistance = new Vector2(1,-1);
        return button;
    }

static void MakeCurriculumPanel(Transform root)
    {
        var rt = New("CurriculumPanel", root);
        Place(rt, new Vector2(0, -350), new Vector2(650, 108));
        var image = rt.gameObject.AddComponent<Image>();
        image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Images/Generated/Akt2/CurriculumPanel.png");
        image.preserveAspect = true;
        image.raycastTarget = false;
        MakeText(rt, "CurriculumText", "SK: 3.1 Mikroorganisma\nSP: 3.1.1, 3.1.2", new Vector2(35, 0), new Vector2(560, 72), 25, TextAlignmentOptions.Left, Ink);
    }

    static Button MakeButton(Transform parent, string name, Vector2 position, Vector2 size, string label, float fontSize, Color color)
    {
        var panel = MakePanel(parent, name, position, size, color);
        var button = panel.gameObject.AddComponent<Button>();
        panel.gameObject.AddComponent<AnimatedButtonFeedback>();
        var shadow = panel.gameObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(0,0,0,.32f);
        shadow.effectDistance = new Vector2(0,-5);
        MakeText(panel.transform, "Label", label, Vector2.zero, size - new Vector2(20,8), fontSize, TextAlignmentOptions.Center, Color.white);
        return button;
    }


static void ConfigureRoute(GameObject target, int index, bool hideCenter)
    {
        var route = target.AddComponent<Akt2MenuButtonRoute>();
        var serialized = new SerializedObject(route);
        serialized.FindProperty("legacyButtonIndex").intValue = index;
        serialized.FindProperty("hideCenterBeforeInvoke").boolValue = hideCenter;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }


static Button MakeHitArea(Transform parent, string name, Vector2 position, Vector2 size)
    {
        string assetName = name == "UtamaButton" ? "Home" : name == "KembaliButton" ? "Back" : "Next";
        var rt = New(name, parent);
        Place(rt, position, size);
        var image = rt.gameObject.AddComponent<Image>();
        image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Images/Generated/Akt2/Nav_" + assetName + ".png");
        image.preserveAspect = true;
        image.raycastTarget = true;
        var button = rt.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        rt.gameObject.AddComponent<AnimatedButtonFeedback>();
        return button;
    }
}