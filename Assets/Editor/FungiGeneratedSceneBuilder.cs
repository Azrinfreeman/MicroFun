using System;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class FungiGeneratedSceneBuilder
{
    const string ArtPath = "Assets/Images/Generated/FungiSceneBase.png";
    static readonly Color Ink = new Color32(7, 79, 29, 255);
    static readonly Color Cream = new Color32(255, 249, 220, 255);

    [MenuItem("MicroFun/Build Generated Fungi Scene")]
    public static void Build()
    {
        var fungi = GameObject.Find("Canvas/Fungi");
        if (fungi == null) throw new Exception("Canvas/Fungi was not found.");

        AudioClip narration = FindNarrationClip(fungi);
        var old = fungi.transform.Find("FungiReference");
        if (old != null) UnityEngine.Object.DestroyImmediate(old.gameObject);
        var prior = fungi.transform.Find("FungiGeneratedReference");
        if (prior != null) UnityEngine.Object.DestroyImmediate(prior.gameObject);

        foreach (Transform child in fungi.transform)
            child.gameObject.SetActive(false);

        ConfigureGeneratedSprite();
        var root = New("FungiGeneratedReference", fungi.transform);
        Fill(root);
        root.SetAsFirstSibling();

        var background = New("GeneratedArtwork", root);
        var bgImage = background.gameObject.AddComponent<Image>();
        bgImage.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath);
        bgImage.preserveAspect = false;
        bgImage.raycastTarget = false;
        Fill(background);

        MakeTitle(root, "FUNGI");
        MakeAudio(root, narration);

        string[] lines =
        {
            "Boleh ditemui di udara, tanah\ndan pada hidupan lain.",
            "Mendapat makanan daripada\nbahan organik.",
            "Membiak melalui spora.",
            "Contoh: <i>Rhizopus</i>, <i>Penicillium</i>\ndan yis."
        };
        float[] ys = { 283f, 80f, -127f, -334f };
        for (int i = 0; i < lines.Length; i++)
            MakeLessonText(root, "LessonText" + (i + 1), lines[i], ys[i]);

        MakeNavigation(root);
        EditorUtility.SetDirty(fungi);
        Debug.Log("Fungi generated reference layout built.");
    }

    static void ConfigureGeneratedSprite()
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

    static AudioClip FindNarrationClip(GameObject root)
    {
        var existing = root.GetComponentInChildren<AudioNarrationButton>(true);
        if (existing == null) return null;
        var field = typeof(AudioNarrationButton).GetField("narrationClip", BindingFlags.Instance | BindingFlags.NonPublic);
        return field == null ? null : field.GetValue(existing) as AudioClip;
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

    static TextMeshProUGUI MakeText(Transform parent, string name, string content, Vector2 position, Vector2 size, float fontSize, TextAlignmentOptions alignment)
    {
        var rt = New(name, parent);
        Place(rt, position, size);
        var tmp = rt.gameObject.AddComponent<TextMeshProUGUI>();
        tmp.font = TMP_Settings.defaultFontAsset;
        tmp.text = content;
        tmp.fontSize = fontSize;
        tmp.color = Ink;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = alignment;
        tmp.enableWordWrapping = true;
        tmp.richText = true;
        tmp.raycastTarget = false;
        tmp.margin = new Vector4(8, 5, 8, 5);
        return tmp;
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

    static void MakeTitle(Transform parent, string title)
    {
        var panel = MakePanel(parent, "TitlePill", new Vector2(0, 445), new Vector2(390, 92), new Color32(252, 248, 200, 245));
        var shadow = panel.gameObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(0.25f, 0.18f, 0.05f, .32f);
        shadow.effectDistance = new Vector2(0, -6);
        MakeText(panel.transform, "TitleText", title, Vector2.zero, new Vector2(350, 80), 55, TextAlignmentOptions.Center);
    }

    static void MakeAudio(Transform parent, AudioClip narration)
    {
        var panel = MakePanel(parent, "AudioButton", new Vector2(-742, 441), new Vector2(320, 70), Cream);
        var button = panel.gameObject.AddComponent<Button>();
        var feedback = panel.gameObject.AddComponent<ButtonInteractionFeedback>();
        var source = panel.gameObject.AddComponent<AudioNarrationButton>();
        var field = typeof(AudioNarrationButton).GetField("narrationClip", BindingFlags.Instance | BindingFlags.NonPublic);
        if (field != null) field.SetValue(source, narration);

        var disc = MakePanel(panel.transform, "SpeakerDisc", new Vector2(-126, 0), new Vector2(82, 82), new Color32(22, 110, 33, 255));
        var discShadow = disc.gameObject.AddComponent<Shadow>();
        discShadow.effectColor = new Color(0, 0, 0, .35f);
        discShadow.effectDistance = new Vector2(0, -4);
        var iconRt = New("SpeakerIcon", disc.transform);
        Place(iconRt, Vector2.zero, new Vector2(50, 50));
        var icon = iconRt.gameObject.AddComponent<Image>();
        icon.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Images/UI Flat/icon_speaker.png");
        icon.preserveAspect = true;
        icon.color = Color.white;
        icon.raycastTarget = false;
        MakeText(panel.transform, "AudioLabel", "Dengar audio", new Vector2(42, 0), new Vector2(210, 52), 29, TextAlignmentOptions.Center);
    }

    static void MakeLessonText(Transform parent, string name, string text, float y)
    {
        var tmp = MakeText(parent, name, text, new Vector2(-34, y), new Vector2(780, 145), 35, TextAlignmentOptions.Left);
        tmp.verticalAlignment = VerticalAlignmentOptions.Middle;
        var outline = tmp.gameObject.AddComponent<Outline>();
        outline.effectColor = new Color(1, 1, 1, .20f);
        outline.effectDistance = new Vector2(1, -1);
    }

    static void MakeNavigation(Transform parent)
    {
        var home = MakeButton(parent, "HomeButton", new Vector2(-810, -455), new Vector2(115, 92), "⌂", 53, new Color32(255, 180, 0, 255));
        ConfigureReturn(home.gameObject.AddComponent<ElaborateReturnButton>(), "ActivityHome");

        var back = MakeButton(parent, "KembaliButton", new Vector2(-198, -455), new Vector2(310, 82), "‹   KEMBALI", 36, new Color32(20, 179, 177, 255));
        ConfigureReturn(back.gameObject.AddComponent<ElaborateReturnButton>(), "SubMenu");

        var next = MakeButton(parent, "SeterusnyaButton", new Vector2(390, -455), new Vector2(360, 82), "SETERUSNYA   ›", 35, new Color32(46, 161, 31, 255));
        var nextController = next.gameObject.AddComponent<NextController>();
        foreach (var field in typeof(NextController).GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            if (field.FieldType == typeof(string)) { field.SetValue(nextController, "kuiz1"); break; }
    }

    static Image MakeButton(Transform parent, string name, Vector2 position, Vector2 size, string label, float fontSize, Color color)
    {
        var panel = MakePanel(parent, name, position, size, color);
        var button = panel.gameObject.AddComponent<Button>();
        var feedback = panel.gameObject.AddComponent<ButtonInteractionFeedback>();
        var shadow = panel.gameObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(0, 0, 0, .32f);
        shadow.effectDistance = new Vector2(0, -5);
        var text = MakeText(panel.transform, "Label", label, Vector2.zero, size - new Vector2(20, 8), fontSize, TextAlignmentOptions.Center);
        text.color = Color.white;
        return panel;
    }

    static void ConfigureReturn(ElaborateReturnButton button, string destination)
    {
        foreach (var field in typeof(ElaborateReturnButton).GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
        {
            if (field.FieldType.IsEnum && field.FieldType.Name.Contains("ReturnDestination"))
            {
                field.SetValue(button, Enum.Parse(field.FieldType, destination));
                return;
            }
        }
    }
}