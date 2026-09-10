#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class Elaborate2MagnifierSetup
{
    [MenuItem("MicroFun/Setup Elaborate2 Magnifier")]
    public static void Build()
    {
        const string scenePath = "Assets/Scenes/Aktivity/akt1/elaborate2.unity";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        var canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) { Debug.LogError("Canvas not found."); return; }

        if (canvas.transform.Find("InteractiveMagnifier") != null)
        {
            Debug.Log("Interactive magnifier already exists.");
            return;
        }

        var alga = canvas.transform.Find("AlgaCard") as RectTransform;
        var protozoa = canvas.transform.Find("ProtozoaCard") as RectTransform;
        var virus = canvas.transform.Find("VirusCard") as RectTransform;
        if (alga == null || protozoa == null || virus == null)
        {
            Debug.LogError("Expected organism cards were not found.");
            return;
        }

        var magnifier = CreateImage("InteractiveMagnifier", canvas.transform, new Vector2(0, 285), new Vector2(230, 230), Color.white, true);
        magnifier.transform.SetAsLastSibling();
        var group = magnifier.AddComponent<CanvasGroup>();
        group.blocksRaycasts = true;
        group.interactable = true;

        var outer = magnifier.GetComponent<Image>();
        outer.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        outer.color = new Color(.08f, .24f, .37f, 1f);
        var outline = magnifier.AddComponent<Outline>();
        outline.effectColor = Color.white;
        outline.effectDistance = new Vector2(5, -5);
        var shadow = magnifier.AddComponent<Shadow>();
        shadow.effectColor = new Color(0, 0, 0, .25f);
        shadow.effectDistance = new Vector2(7, -8);

        var lens = CreateImage("Lens", magnifier.transform, new Vector2(-15, 15), new Vector2(158, 158), new Color(.72f, .94f, 1f, .28f), false);
        lens.GetComponent<Image>().sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        var lensOutline = lens.AddComponent<Outline>();
        lensOutline.effectColor = new Color(.8f, 1f, 1f, .85f);
        lensOutline.effectDistance = new Vector2(3, -3);

        var handle = CreateImage("Handle", magnifier.transform, new Vector2(70, -70), new Vector2(112, 43), new Color(.05f, .20f, .34f), false);
        handle.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0, -45);
        handle.GetComponent<Image>().sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        handle.GetComponent<Image>().type = Image.Type.Sliced;
        var handleOutline = handle.AddComponent<Outline>();
        handleOutline.effectColor = Color.white;
        handleOutline.effectDistance = new Vector2(3, -3);

        var pill = CreateImage("IdentificationPill", magnifier.transform, new Vector2(0, 135), new Vector2(250, 58), new Color(.22f, .67f, .24f), false);
        pill.GetComponent<Image>().sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        pill.GetComponent<Image>().type = Image.Type.Sliced;
        var pillOutline = pill.AddComponent<Outline>();
        pillOutline.effectColor = Color.white;
        pillOutline.effectDistance = new Vector2(3, -3);

        var result = new GameObject("ResultText", typeof(RectTransform), typeof(TextMeshProUGUI));
        result.transform.SetParent(pill.transform, false);
        var resultRect = result.GetComponent<RectTransform>();
        resultRect.anchorMin = Vector2.zero;
        resultRect.anchorMax = Vector2.one;
        resultRect.offsetMin = Vector2.zero;
        resultRect.offsetMax = Vector2.zero;
        var resultText = result.GetComponent<TextMeshProUGUI>();
        resultText.font = TMP_Settings.defaultFontAsset;
        resultText.fontSize = 30;
        resultText.fontStyle = FontStyles.Bold;
        resultText.alignment = TextAlignmentOptions.Center;
        resultText.color = Color.white;
        resultText.raycastTarget = false;

        var controller = magnifier.AddComponent<MagnifierDragIdentifier>();
        var serialized = new SerializedObject(controller);
        serialized.FindProperty("draggableRect").objectReferenceValue = magnifier.GetComponent<RectTransform>();
        serialized.FindProperty("dragBounds").objectReferenceValue = canvas.transform as RectTransform;
        serialized.FindProperty("rootCanvas").objectReferenceValue = canvas;
        serialized.FindProperty("resultLabel").objectReferenceValue = resultText;
        serialized.FindProperty("resultPill").objectReferenceValue = pill.GetComponent<Image>();
        serialized.FindProperty("lensGlow").objectReferenceValue = lens.GetComponent<Image>();
        var targets = serialized.FindProperty("targets");
        targets.arraySize = 3;
        SetTarget(targets.GetArrayElementAtIndex(0), alga, "Alga", new Color(.49f, .76f, .20f));
        SetTarget(targets.GetArrayElementAtIndex(1), protozoa, "Protozoa", new Color(.28f, .61f, .91f));
        SetTarget(targets.GetArrayElementAtIndex(2), virus, "Virus", new Color(.92f, .38f, .60f));
        serialized.ApplyModifiedPropertiesWithoutUndo();

        pill.SetActive(false);
        result.SetActive(false);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Selection.activeGameObject = magnifier;
        Debug.Log("Interactive magnifier added to Elaborate2.");
    }

    static void SetTarget(SerializedProperty target, RectTransform zone, string name, Color accent)
    {
        target.FindPropertyRelative("zone").objectReferenceValue = zone;
        target.FindPropertyRelative("microorganismName").stringValue = name;
        target.FindPropertyRelative("accent").colorValue = accent;
    }

    static GameObject CreateImage(string name, Transform parent, Vector2 position, Vector2 size, Color color, bool raycastTarget)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(.5f, .5f);
        rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        var image = go.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = raycastTarget;
        return go;
    }
}
#endif