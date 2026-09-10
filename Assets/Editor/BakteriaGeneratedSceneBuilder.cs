using System;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class BakteriaGeneratedSceneBuilder
{
    const string ArtPath = "Assets/Images/Generated/BakteriaSceneBase.png";
    static readonly Color Ink = new Color32(7, 79, 29, 255);
    static readonly Color Cream = new Color32(255, 249, 220, 255);

    [MenuItem("MicroFun/Build Generated Bakteria Scene")]
    public static void Build()
    {
        var section = GameObject.Find("Canvas/Bakteria");
        if (section == null) throw new Exception("Canvas/Bakteria was not found.");

        AudioClip narration = FindNarrationClip(section);
        string nextTarget = FindNextTarget(section);
        var old = section.transform.Find("BakteriaGeneratedReference");
        if (old != null) UnityEngine.Object.DestroyImmediate(old.gameObject);
        foreach (Transform child in section.transform) child.gameObject.SetActive(false);

        ConfigureGeneratedSprite();
        var root = New("BakteriaGeneratedReference", section.transform);
        Fill(root);
        root.SetAsFirstSibling();

        var background = New("GeneratedArtwork", root);
        var image = background.gameObject.AddComponent<Image>();
        image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath);
        image.preserveAspect = false;
        image.raycastTarget = false;
        Fill(background);

        MakeTitle(root, "BAKTERIA");
        MakeAudio(root, narration);

        string[] text =
        {
            "Wujud dalam bentuk\nsfera, rod dan lingkaran.",
            "Boleh ditemui di udara, air,\ntanah dan pada hidupan lain.",
            "Sesetengah bakteria bermanfaat,\nsesetengah lagi menyebabkan\npenyakit.",
            "Contoh: <i>Spirillum</i>, <i>Streptococcus</i>,\n<i>Escherichia coli</i> dan <i>Salmonella</i>."
        };
        float[] y = { 330f, 125f, -86f, -294f };
        for (int i = 0; i < 4; i++)
        {
            MakeLessonText(root, "LessonText" + (i + 1), text[i], y[i], i == 2 ? 31 : 33);
            float[] badgeY = { 354f, 133f, -86f, -315f };
            MakeBadgeNumber(root, (i + 1).ToString(), badgeY[i]);
        }
        MakeExampleLabels(root);
        MakeNavigation(root, string.IsNullOrEmpty(nextTarget) ? "kuiz1" : nextTarget);

        EditorUtility.SetDirty(section);
        Debug.Log("Bakteria generated reference layout built.");
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
        var field = typeof(AudioNarrationButton).GetField("narrationClip", BindingFlags.Instance | BindingFlags.NonPublic);
        return existing == null || field == null ? null : field.GetValue(existing) as AudioClip;
    }

    static string FindNextTarget(GameObject root)
    {
        var existing = root.GetComponentInChildren<NextController>(true);
        if (existing == null) return null;
        foreach (var field in typeof(NextController).GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            if (field.FieldType == typeof(string)) return field.GetValue(existing) as string;
        return null;
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
        var panel = rt.gameObject.AddComponent<Image>();
        panel.sprite = DefaultSprite();
        panel.type = Image.Type.Sliced;
        panel.color = color;
        return panel;
    }

    static TextMeshProUGUI MakeText(Transform parent, string name, string content, Vector2 position, Vector2 size, float fontSize, TextAlignmentOptions align)
    {
        var rt = New(name, parent);
        Place(rt, position, size);
        var text = rt.gameObject.AddComponent<TextMeshProUGUI>();
        text.font = TMP_Settings.defaultFontAsset;
        text.text = content;
        text.fontSize = fontSize;
        text.color = Ink;
        text.fontStyle = FontStyles.Bold;
        text.alignment = align;
        text.enableWordWrapping = true;
        text.richText = true;
        text.raycastTarget = false;
        text.margin = new Vector4(8, 5, 8, 5);
        return text;
    }

    static void MakeTitle(Transform root, string title)
    {
        var panel = MakePanel(root, "TitlePill", new Vector2(0, 468), new Vector2(500, 75), new Color32(252, 248, 200, 245));
        var shadow = panel.gameObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(0.25f, .18f, .05f, .32f);
        shadow.effectDistance = new Vector2(0, -5);
        MakeText(panel.transform, "TitleText", title, Vector2.zero, new Vector2(470, 66), 51, TextAlignmentOptions.Center);
    }

    static void MakeAudio(Transform root, AudioClip narration)
    {
        var panel = MakePanel(root, "AudioButton", new Vector2(-742, 441), new Vector2(320, 70), Cream);
        panel.gameObject.AddComponent<Button>();
        panel.gameObject.AddComponent<ButtonInteractionFeedback>();
        var narrationButton = panel.gameObject.AddComponent<AudioNarrationButton>();
        var field = typeof(AudioNarrationButton).GetField("narrationClip", BindingFlags.Instance | BindingFlags.NonPublic);
        if (field != null) field.SetValue(narrationButton, narration);

        var disc = MakePanel(panel.transform, "SpeakerDisc", new Vector2(-126, 0), new Vector2(82, 82), new Color32(22, 110, 33, 255));
        var iconRt = New("SpeakerIcon", disc.transform);
        Place(iconRt, Vector2.zero, new Vector2(50, 50));
        var icon = iconRt.gameObject.AddComponent<Image>();
        icon.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Images/UI Flat/icon_speaker.png");
        icon.preserveAspect = true;
        icon.color = Color.white;
        icon.raycastTarget = false;
        MakeText(panel.transform, "AudioLabel", "Dengar audio", new Vector2(42, 0), new Vector2(210, 52), 29, TextAlignmentOptions.Center);
    }

    static void MakeLessonText(Transform root, string name, string content, float y, float size)
    {
        var tmp = MakeText(root, name, content, new Vector2(-115, y), new Vector2(750, 150), size, TextAlignmentOptions.Left);
        tmp.verticalAlignment = VerticalAlignmentOptions.Middle;
        var outline = tmp.gameObject.AddComponent<Outline>();
        outline.effectColor = new Color(1,1,1,.20f);
        outline.effectDistance = new Vector2(1,-1);
    }

    static void MakeBadgeNumber(Transform root, string number, float y)
    {
        var t = MakeText(root, "Badge" + number, number, new Vector2(-562, y), new Vector2(90, 90), 52, TextAlignmentOptions.Center);
        t.color = Color.white;
        t.verticalAlignment = VerticalAlignmentOptions.Middle;
    }

    static void MakeExampleLabels(Transform root)
    {
        string[] labels = { "Spirillum", "Streptococcus", "Escherichia coli", "Salmonella" };
        float[] xs = { 150f, 340f, 530f, 720f };
        for (int i = 0; i < labels.Length; i++)
        {
            var t = MakeText(root, "Example" + i, labels[i], new Vector2(xs[i], -375), new Vector2(175, 34), 17, TextAlignmentOptions.Center);
            t.fontStyle = FontStyles.Bold;
        }
    }

    static void MakeNavigation(Transform root, string target)
    {
        var home = MakeButton(root, "HomeButton", new Vector2(-810, -455), new Vector2(115, 92), "⌂", 53, new Color32(255,180,0,255));
        ConfigureReturn(home.gameObject.AddComponent<ElaborateReturnButton>(), "ActivityHome");

        var back = MakeButton(root, "KembaliButton", new Vector2(-198, -455), new Vector2(310, 82), "‹   KEMBALI", 36, new Color32(20,179,177,255));
        ConfigureReturn(back.gameObject.AddComponent<ElaborateReturnButton>(), "SubMenu");

        var next = MakeButton(root, "SeterusnyaButton", new Vector2(390, -455), new Vector2(360, 82), "SETERUSNYA   ›", 35, new Color32(46,161,31,255));
        var controller = next.gameObject.AddComponent<NextController>();
        foreach (var field in typeof(NextController).GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            if (field.FieldType == typeof(string)) { field.SetValue(controller, target); break; }
    }

    static Image MakeButton(Transform parent, string name, Vector2 position, Vector2 size, string label, float fontSize, Color color)
    {
        var panel = MakePanel(parent, name, position, size, color);
        panel.gameObject.AddComponent<Button>();
        panel.gameObject.AddComponent<ButtonInteractionFeedback>();
        var shadow = panel.gameObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(0,0,0,.32f);
        shadow.effectDistance = new Vector2(0,-5);
        var text = MakeText(panel.transform, "Label", label, Vector2.zero, size - new Vector2(20,8), fontSize, TextAlignmentOptions.Center);
        text.color = Color.white;
        return panel;
    }

    static void ConfigureReturn(ElaborateReturnButton button, string destination)
    {
        foreach (var field in typeof(ElaborateReturnButton).GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            if (field.FieldType.IsEnum && field.FieldType.Name.Contains("ReturnDestination"))
            {
                field.SetValue(button, Enum.Parse(field.FieldType, destination));
                return;
            }
    }
}