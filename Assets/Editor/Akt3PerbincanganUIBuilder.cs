#if UNITY_EDITOR
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class Akt3PerbincanganUIBuilder
{
    const string ScenePath = "Assets/Scenes/Aktivity/akt3/akt3.unity";
    const string BackgroundPath = "Assets/Generated/Akt3/perbincangan_background.png";

    [MenuItem("MicroFun/Build Activity 3 Perbincangan")]
    public static void Build()
    {
        var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        var canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) { Debug.LogError("Akt3 canvas was not found."); return; }

        var page = canvas.transform.Find("Penerangan");
        if (page == null) { Debug.LogError("Penerangan page was not found."); return; }
        page.gameObject.SetActive(true);

        var back = page.Find("Back")?.gameObject;
        var dialogButton = page.Find("Top/DialogButton")?.gameObject;
        var dialog = page.Find("DialogParent")?.gameObject;
        var routes = new[]
        {
            page.Find("Button")?.gameObject,
            page.Find("Button (1)")?.gameObject,
            page.Find("Button (2)")?.gameObject,
            page.Find("Button (4)")?.gameObject
        }.Where(x => x != null).ToArray();

        var parking = new GameObject("__PerbincanganParking").transform;
        parking.SetParent(canvas.transform, false);
        foreach (var keep in routes.Concat(new[] { back, dialogButton, dialog }).Where(x => x != null))
            keep.transform.SetParent(parking, false);

        for (int i = 0; i < page.childCount; i++) { page.GetChild(i).gameObject.SetActive(false); }
        parking.gameObject.SetActive(false);

        var root = page.GetComponent<RectTransform>();
        Stretch(root);

        var bg = new GameObject("Perbincangan Background", typeof(RectTransform), typeof(RawImage));
        bg.transform.SetParent(page, false);
        Stretch(bg.GetComponent<RectTransform>());
        bg.GetComponent<RawImage>().texture = AssetDatabase.LoadAssetAtPath<Texture2D>(BackgroundPath);
        bg.GetComponent<RawImage>().color = Color.white;

        var badge = Panel("Activity Badge", page, new Vector2(0, 432), new Vector2(292, 78), new Color(1f, .96f, .76f), new Color(.99f, .99f, .99f), 4);
        Text("AKTIVITI 3", badge.transform, Vector2.zero, new Vector2(278, 66), 40, new Color(.02f, .35f, .10f), FontStyles.Bold, TextAlignmentOptions.Center);

        var title = Text("Perbincangan", page, new Vector2(0, 315), new Vector2(1100, 155), 106, new Color(.02f, .40f, .07f), FontStyles.Bold, TextAlignmentOptions.Center);
        var titleOutline = title.gameObject.AddComponent<Outline>();
        titleOutline.effectColor = Color.white;
        titleOutline.effectDistance = new Vector2(4, -4);

        var bubble = Panel("Discussion Speech Bubble", page, new Vector2(320, -53), new Vector2(910, 545), Color.white, new Color(.29f, .71f, .06f), 7);
        bubble.GetComponent<Shadow>().effectColor = new Color(0, 0, 0, .14f);
        bubble.GetComponent<Shadow>().effectDistance = new Vector2(0, -10);
        var tail = Panel("Bubble Tail", bubble.transform, new Vector2(-365, -215), new Vector2(116, 108), Color.white, new Color(.29f, .71f, .06f), 7);
        tail.transform.localEulerAngles = new Vector3(0, 0, 45);
        tail.transform.SetSiblingIndex(0);

        var prompt = "Selepas menonton video\nproses hidup mikroorganisma,\nboleh awak <color=#159447>bincangkan</color>\ndan <color=#159447>terangkan</color> apakah\nproses yang terlibat?";
        var promptText = Text(prompt, bubble.transform, new Vector2(35, 55), new Vector2(770, 380), 47, new Color(.02f, .06f, .18f), FontStyles.Bold, TextAlignmentOptions.Center);
        promptText.enableWordWrapping = false;
        promptText.richText = true;

        var guide = Text("Pilih contoh bakteria untuk berbincang", bubble.transform, new Vector2(35, -158), new Vector2(700, 36), 22, new Color(.02f, .35f, .10f), FontStyles.Bold, TextAlignmentOptions.Center);

        if (back != null)
        {
            back.transform.SetParent(page, false);
            back.SetActive(true);
            StyleButton(back, new Vector2(-740, -468), new Vector2(278, 74), "‹  Kembali", new Color(.05f, .65f, .67f), Color.white, 30);
        }
        if (dialogButton != null)
        {
            dialogButton.transform.SetParent(page, false);
            dialogButton.SetActive(true);
            StyleButton(dialogButton, new Vector2(-420, -468), new Vector2(210, 64), "Soalan panduan", new Color(.14f, .55f, .28f), Color.white, 21);
        }

        string[] labels = { "Spirillum", "Streptococcus", "E. coli", "Salmonella" };
        Color[] chipColors =
        {
            new Color(.53f,.35f,.82f), new Color(.97f,.67f,.12f),
            new Color(.22f,.63f,.83f), new Color(.90f,.33f,.33f)
        };
        for (int i = 0; i < routes.Length; i++)
        {
            routes[i].transform.SetParent(bubble.transform, false);
            routes[i].SetActive(true);
            StyleButton(routes[i], new Vector2(-250 + i * 190, -220), new Vector2(172, 55), labels[i], chipColors[i], Color.white, 18);
        }

        if (dialog != null)
        {
            dialog.transform.SetParent(page, false);
            dialog.SetActive(false);
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Selection.activeGameObject = page.gameObject;
        Debug.Log("Built Activity 3 Perbincangan reference UI.");
    }

    static GameObject Panel(string name, Transform parent, Vector2 position, Vector2 size, Color fill, Color border, int borderSize)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Outline), typeof(Shadow));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(.5f, .5f);
        rt.pivot = new Vector2(.5f, .5f);
        rt.anchoredPosition = position;
        rt.sizeDelta = size;
        var image = go.GetComponent<Image>();
        image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        image.type = Image.Type.Sliced;
        image.color = fill;
        var outline = go.GetComponent<Outline>();
        outline.effectColor = border;
        outline.effectDistance = new Vector2(borderSize, -borderSize);
        var shadow = go.GetComponent<Shadow>();
        shadow.effectColor = new Color(0, 0, 0, .09f);
        shadow.effectDistance = new Vector2(0, -5);
        return go;
    }

    static TextMeshProUGUI Text(string value, Transform parent, Vector2 position, Vector2 size, float fontSize, Color color, FontStyles style, TextAlignmentOptions alignment)
    {
        var go = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(.5f, .5f);
        rt.pivot = new Vector2(.5f, .5f);
        rt.anchoredPosition = position;
        rt.sizeDelta = size;
        var text = go.GetComponent<TextMeshProUGUI>();
        text.font = TMP_Settings.defaultFontAsset;
        text.text = value;
        text.fontSize = fontSize;
        text.color = color;
        text.fontStyle = style;
        text.alignment = alignment;
        text.enableAutoSizing = false;
        text.overflowMode = TextOverflowModes.Overflow;
        return text;
    }

    static void StyleButton(GameObject go, Vector2 position, Vector2 size, string label, Color fill, Color labelColor, int labelSize)
    {
        for (int i = 0; i < go.transform.childCount; i++) { go.transform.GetChild(i).gameObject.SetActive(false); }
        var rt = go.GetComponent<RectTransform>() ?? go.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(.5f, .5f);
        rt.pivot = new Vector2(.5f, .5f);
        rt.anchoredPosition = position;
        rt.sizeDelta = size;

        var image = go.GetComponent<Image>() ?? go.AddComponent<Image>();
        image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        image.type = Image.Type.Sliced;
        image.color = fill;
        foreach (var outline in go.GetComponents<Outline>()) { outline.enabled = false; }
        var outlineNew = go.AddComponent<Outline>();
        outlineNew.effectColor = Color.white;
        outlineNew.effectDistance = new Vector2(3, -3);
        foreach (var shadow in go.GetComponents<Shadow>()) { shadow.enabled = false; }
        var shadowNew = go.AddComponent<Shadow>();
        shadowNew.effectColor = new Color(0, 0, 0, .22f);
        shadowNew.effectDistance = new Vector2(0, -5);

        var button = go.GetComponent<Button>() ?? go.AddComponent<Button>();
        button.targetGraphic = image;
        var colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1f, 1f, 1f, .94f);
        colors.pressedColor = new Color(.82f, .82f, .82f, 1f);
        button.colors = colors;
        if (go.GetComponent<AnimatedButtonFeedback>() == null) go.AddComponent<AnimatedButtonFeedback>();

        var text = Text(label, go.transform, Vector2.zero, size - new Vector2(16, 8), labelSize, labelColor, FontStyles.Bold, TextAlignmentOptions.Center);
        text.enableWordWrapping = false;
    }

    static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        rt.pivot = new Vector2(.5f, .5f);
    }
}
#endif