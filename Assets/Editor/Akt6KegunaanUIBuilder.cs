#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Rebuilds Canvas/Kegunaan to match the supplied Activity 6 reference.
/// The original buttons remain serialized in the scene and are invoked through
/// CenterMenuButtonRelay, preserving all existing explanation-panel callbacks.
/// </summary>
public static class Akt6KegunaanUIBuilder
{
    private const string ScenePath = "Assets/Scenes/Aktivity/akt6/akt6.unity";
    private const string RootName = "Akt6KegunaanReferenceRoot";
    private const string ReferencePath = "Assets/Images/Akt6KegunaanUI/Background/akt6_kegunaan_reference.png";
    private const string ButtonSpritePath = "Assets/Images/UI Flat/btn_green_flat.png";
    private const string PlayIconPath = "Assets/Images/UI Flat/icon_play.png";

[MenuItem("Tools/MicroFun/Activity 6/Build Kegunaan Reference UI")]
    public static void Rebuild()
    {
        if (EditorSceneManager.GetActiveScene().path != ScenePath)
        {
            Debug.LogError("Akt6KegunaanUIBuilder: open " + ScenePath + " before rebuilding.");
            return;
        }

        ConfigureReferenceSprite();
        Sprite reference = AssetDatabase.LoadAssetAtPath<Sprite>(ReferencePath);
        Sprite buttonSprite = AssetDatabase.LoadAssetAtPath<Sprite>(ButtonSpritePath);
        Sprite playIcon = AssetDatabase.LoadAssetAtPath<Sprite>(PlayIconPath);
        if (reference == null || buttonSprite == null || playIcon == null)
        {
            Debug.LogError("Akt6KegunaanUIBuilder: reference background or back-button sprites are missing.");
            return;
        }

        GameObject canvasObject = GameObject.Find("Canvas");
        Transform kegunaan = canvasObject != null ? canvasObject.transform.Find("Kegunaan") : null;
        Transform legacyCenter = kegunaan != null ? kegunaan.Find("Center") : null;
        Transform legacyBottom = kegunaan != null ? kegunaan.Find("Bottom") : null;
        if (canvasObject == null || kegunaan == null || legacyCenter == null || legacyBottom == null)
        {
            Debug.LogError("Akt6KegunaanUIBuilder: Canvas/Kegunaan Center or Bottom was not found.");
            return;
        }

        Button food = FindButton(legacyCenter, "Button (2)");
        Button fertilizer = FindButton(legacyCenter, "Button (5)");
        Button wastewater = FindButton(legacyCenter, "Button (6)");
        Button medicine = FindButton(legacyCenter, "Button (3)");
        Button home = FindButton(legacyBottom, "HomeButton");
        if (food == null || fertilizer == null || wastewater == null || medicine == null || home == null)
        {
            Debug.LogError("Akt6KegunaanUIBuilder: one or more original Kegunaan or Back buttons are missing.");
            return;
        }

        Transform existing = kegunaan.Find(RootName);
        if (existing != null)
            Object.DestroyImmediate(existing.gameObject);

        GameObject root = CreateUI(RootName, kegunaan);
        Stretch(root.GetComponent<RectTransform>());
        root.transform.SetSiblingIndex(0);

        Image background = root.AddComponent<Image>();
        background.sprite = reference;
        background.preserveAspect = false;
        background.color = Color.white;
        background.raycastTarget = false;

        BuildRelayButton(root.transform, "PembuatanMakananButton", food,
            new Vector2(-532f, 143f), new Vector2(668f, 252f), new Color32(22, 155, 226, 255));
        BuildRelayButton(root.transform, "PenghasilanUbatButton", medicine,
            new Vector2(506f, 143f), new Vector2(646f, 252f), new Color32(255, 165, 7, 255));
        BuildRelayButton(root.transform, "PenghasilanBajaButton", fertilizer,
            new Vector2(-532f, -180f), new Vector2(668f, 260f), new Color32(242, 67, 127, 255));
        BuildRelayButton(root.transform, "PerawatanKumbahanButton", wastewater,
            new Vector2(506f, -180f), new Vector2(646f, 260f), new Color32(9, 171, 166, 255));
        BuildVisibleBackButton(root.transform, home, buttonSprite, playIcon);

        legacyCenter.gameObject.SetActive(false);
        legacyBottom.gameObject.SetActive(false);

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        if (scaler != null)
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
        }

        EditorUtility.SetDirty(kegunaan.gameObject);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Selection.activeGameObject = root;
        Debug.Log("Activity 6 Kegunaan reference UI rebuilt with four content callbacks and a visible Back button.");
    }

    private static void BuildRelayButton(Transform parent, string name, Button source,
        Vector2 position, Vector2 size, Color32 accent)
    {
        GameObject go = CreateUI(name, parent);
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        rect.localScale = Vector3.one;

        Image hitArea = go.AddComponent<Image>();
        hitArea.color = new Color(1f, 1f, 1f, 0.001f);
        hitArea.raycastTarget = true;

        Button button = go.AddComponent<Button>();
        button.targetGraphic = hitArea;
        button.transition = Selectable.Transition.ColorTint;
        button.navigation = new Navigation { mode = Navigation.Mode.None };

        ColorBlock colors = button.colors;
        colors.normalColor = new Color(1f, 1f, 1f, 0.001f);
        colors.highlightedColor = new Color(1f, 1f, 1f, 0.16f);
        colors.pressedColor = new Color(accent.r / 255f, accent.g / 255f, accent.b / 255f, 0.22f);
        colors.selectedColor = new Color(1f, 1f, 1f, 0.10f);
        colors.disabledColor = new Color(1f, 1f, 1f, 0.001f);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.08f;
        button.colors = colors;

        CenterMenuButtonRelay relay = go.AddComponent<CenterMenuButtonRelay>();
        SerializedObject serializedRelay = new SerializedObject(relay);
        serializedRelay.FindProperty("sourceButton").objectReferenceValue = source;
        serializedRelay.ApplyModifiedPropertiesWithoutUndo();
        UnityEventTools.AddPersistentListener(button.onClick, relay.InvokeSource);
    }

    private static Button FindButton(Transform parent, string path)
    {
        Transform child = parent.Find(path);
        return child != null ? child.GetComponent<Button>() : null;
    }

    private static GameObject CreateUI(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.layer = parent.gameObject.layer;
        go.transform.SetParent(parent, false);
        return go;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;
    }

    private static void ConfigureReferenceSprite()
    {
        AssetDatabase.ImportAsset(ReferencePath, ImportAssetOptions.ForceSynchronousImport);
        TextureImporter importer = AssetImporter.GetAtPath(ReferencePath) as TextureImporter;
        if (importer == null)
            return;

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = false;
        importer.mipmapEnabled = false;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.filterMode = FilterMode.Bilinear;
        importer.maxTextureSize = 2048;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.SaveAndReimport();
    }


private static void BuildVisibleBackButton(Transform parent, Button source, Sprite buttonSprite, Sprite arrowSprite)
    {
        GameObject go = CreateUI("BackButton", parent);
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(-900f, 470f);
        rect.sizeDelta = new Vector2(86f, 86f);
        rect.localScale = Vector3.one;

        Image panel = go.AddComponent<Image>();
        panel.sprite = buttonSprite;
        panel.type = Image.Type.Sliced;
        panel.color = new Color32(28, 158, 225, 255);
        panel.raycastTarget = true;

        Shadow shadow = go.AddComponent<Shadow>();
        shadow.effectColor = new Color32(9, 62, 105, 145);
        shadow.effectDistance = new Vector2(0f, -6f);
        shadow.useGraphicAlpha = true;

        Outline outline = go.AddComponent<Outline>();
        outline.effectColor = Color.white;
        outline.effectDistance = new Vector2(3f, 3f);
        outline.useGraphicAlpha = true;

        Button button = go.AddComponent<Button>();
        button.targetGraphic = panel;
        button.navigation = new Navigation { mode = Navigation.Mode.None };

        GameObject arrowObject = CreateUI("BackArrow", go.transform);
        RectTransform arrowRect = arrowObject.GetComponent<RectTransform>();
        arrowRect.anchorMin = new Vector2(0.5f, 0.5f);
        arrowRect.anchorMax = new Vector2(0.5f, 0.5f);
        arrowRect.pivot = new Vector2(0.5f, 0.5f);
        arrowRect.anchoredPosition = Vector2.zero;
        arrowRect.sizeDelta = new Vector2(46f, 46f);
        arrowRect.localScale = new Vector3(-1f, 1f, 1f);
        Image arrow = arrowObject.AddComponent<Image>();
        arrow.sprite = arrowSprite;
        arrow.preserveAspect = true;
        arrow.color = Color.white;
        arrow.raycastTarget = false;

        CenterMenuButtonRelay relay = go.AddComponent<CenterMenuButtonRelay>();
        SerializedObject serializedRelay = new SerializedObject(relay);
        serializedRelay.FindProperty("sourceButton").objectReferenceValue = source;
        serializedRelay.ApplyModifiedPropertiesWithoutUndo();
        UnityEventTools.AddPersistentListener(button.onClick, relay.InvokeSource);
    }
}
#endif
