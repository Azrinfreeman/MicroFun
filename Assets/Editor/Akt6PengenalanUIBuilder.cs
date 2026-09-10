#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Rebuilds Activity 6 Pengenalan from the approved 1920x1080 reference.
/// Legacy content and dialogue objects remain serialized. Only the original
/// Home and Continue buttons are relayed because the old lesson buttons do not
/// semantically match the new bread, yogurt, and wastewater examples.
/// </summary>
public static class Akt6PengenalanUIBuilder
{
    private const string ScenePath = "Assets/Scenes/Aktivity/akt6/akt6.unity";
    private const string RootName = "Akt6PengenalanReferenceRoot";
    private const string ReferencePath = "Assets/Images/Akt6PengenalanUI/Background/akt6_pengenalan_reference.png";
    private const string PlayIconPath = "Assets/Images/UI Flat/icon_play.png";
    private const string ButtonSpritePath = "Assets/Images/UI Flat/btn_green_flat.png";

    [MenuItem("Tools/MicroFun/Activity 6/Build Pengenalan Reference UI")]
    public static void Rebuild()
    {
        if (EditorSceneManager.GetActiveScene().path != ScenePath)
        {
            Debug.LogError("Akt6PengenalanUIBuilder: open " + ScenePath + " before rebuilding.");
            return;
        }

        ConfigureReferenceSprite();
        Sprite reference = AssetDatabase.LoadAssetAtPath<Sprite>(ReferencePath);
        Sprite playIcon = AssetDatabase.LoadAssetAtPath<Sprite>(PlayIconPath);
        Sprite buttonSprite = AssetDatabase.LoadAssetAtPath<Sprite>(ButtonSpritePath);
        if (reference == null || playIcon == null || buttonSprite == null)
        {
            Debug.LogError("Akt6PengenalanUIBuilder: reference background is missing at " + ReferencePath);
            return;
        }

        GameObject canvasObject = GameObject.Find("Canvas");
        Transform pengenalan = canvasObject != null ? canvasObject.transform.Find("Pengenalan") : null;
        Transform bottom = pengenalan != null ? pengenalan.Find("Bottom") : null;
        Button home = FindButton(bottom, "HomeButton");
        Button next = FindButton(bottom, "NextButton");
        if (canvasObject == null || pengenalan == null || bottom == null || home == null || next == null)
        {
            Debug.LogError("Akt6PengenalanUIBuilder: Pengenalan or its original Home/Next buttons were not found.");
            return;
        }

        Transform existing = pengenalan.Find(RootName);
        if (existing != null)
            Object.DestroyImmediate(existing.gameObject);

        GameObject root = CreateUI(RootName, pengenalan);
        Stretch(root.GetComponent<RectTransform>());
        root.transform.SetSiblingIndex(0);

        Image background = root.AddComponent<Image>();
        background.sprite = reference;
        background.preserveAspect = false;
        background.color = Color.white;
        background.raycastTarget = false;

        // Non-interactive semantic regions retained for clear hierarchy and future accessibility.
        CreateMarker(root.transform, "BreadExampleRegion",
            new Vector2(-525f, 20f), new Vector2(490f, 350f));
        CreateMarker(root.transform, "YogurtExampleRegion",
            new Vector2(0f, 20f), new Vector2(490f, 350f));
        CreateMarker(root.transform, "WastewaterExampleRegion",
            new Vector2(535f, 20f), new Vector2(510f, 350f));

        // The Activity 6 badge returns to the main Activity 6 menu.
        BuildRelayButton(root.transform, "HomeHotspot", home,
            new Vector2(-825f, 420f), new Vector2(245f, 225f),
            new Color32(109, 51, 192, 255), null, Vector2.zero, Vector2.zero);

        // The discussion prompt advances to Kegunaan. A small arrow makes this discoverable.
        BuildRelayButton(root.transform, "ContinueHotspot", next,
            new Vector2(-15f, -390f), new Vector2(950f, 185f),
            new Color32(99, 45, 166, 255), playIcon,
            new Vector2(425f, 0f), new Vector2(58f, 58f));

                BuildVisibleBackButton(root.transform, home, buttonSprite, playIcon);

        
Transform title = pengenalan.Find("Title");
        Transform content = pengenalan.Find("Content");
        if (title != null) title.gameObject.SetActive(false);
        if (content != null) content.gameObject.SetActive(false);
        bottom.gameObject.SetActive(false);

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        if (scaler != null)
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
        }

        EditorUtility.SetDirty(pengenalan.gameObject);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Selection.activeGameObject = root;
        Debug.Log("Activity 6 Pengenalan reference UI rebuilt. Home and Continue callbacks are preserved; legacy content remains serialized.");
    }

    private static void CreateMarker(Transform parent, string name, Vector2 position, Vector2 size)
    {
        GameObject marker = CreateUI(name, parent);
        SetRect(marker.GetComponent<RectTransform>(), position, size);
    }

    private static void BuildRelayButton(Transform parent, string name, Button source,
        Vector2 position, Vector2 size, Color32 accent, Sprite iconSprite,
        Vector2 iconPosition, Vector2 iconSize)
    {
        GameObject go = CreateUI(name, parent);
        RectTransform rect = go.GetComponent<RectTransform>();
        SetRect(rect, position, size);

        Image hitArea = go.AddComponent<Image>();
        hitArea.color = new Color(1f, 1f, 1f, 0.001f);
        hitArea.raycastTarget = true;

        Button button = go.AddComponent<Button>();
        button.targetGraphic = hitArea;
        button.transition = Selectable.Transition.ColorTint;
        button.navigation = new Navigation { mode = Navigation.Mode.None };

        ColorBlock colors = button.colors;
        colors.normalColor = new Color(1f, 1f, 1f, 0.001f);
        colors.highlightedColor = new Color(1f, 1f, 1f, 0.12f);
        colors.pressedColor = new Color(accent.r / 255f, accent.g / 255f, accent.b / 255f, 0.20f);
        colors.selectedColor = new Color(1f, 1f, 1f, 0.08f);
        colors.disabledColor = new Color(1f, 1f, 1f, 0.001f);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.08f;
        button.colors = colors;

        CenterMenuButtonRelay relay = go.AddComponent<CenterMenuButtonRelay>();
        SerializedObject serializedRelay = new SerializedObject(relay);
        serializedRelay.FindProperty("sourceButton").objectReferenceValue = source;
        serializedRelay.ApplyModifiedPropertiesWithoutUndo();
        UnityEventTools.AddPersistentListener(button.onClick, relay.InvokeSource);

        if (iconSprite != null && iconSize.sqrMagnitude > 0f)
        {
            GameObject iconObject = CreateUI("ContinueArrow", go.transform);
            SetRect(iconObject.GetComponent<RectTransform>(), iconPosition, iconSize);
            Image icon = iconObject.AddComponent<Image>();
            icon.sprite = iconSprite;
            icon.preserveAspect = true;
            icon.color = new Color32(105, 49, 176, 235);
            icon.raycastTarget = false;
            Shadow shadow = iconObject.AddComponent<Shadow>();
            shadow.effectColor = new Color32(255, 255, 255, 210);
            shadow.effectDistance = new Vector2(2f, -2f);
            shadow.useGraphicAlpha = true;
        }
    }

    private static Button FindButton(Transform parent, string name)
    {
        if (parent == null)
            return null;
        Transform child = parent.Find(name);
        return child != null ? child.GetComponent<Button>() : null;
    }

    private static GameObject CreateUI(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.layer = parent.gameObject.layer;
        go.transform.SetParent(parent, false);
        return go;
    }

    private static void SetRect(RectTransform rect, Vector2 position, Vector2 size)
    {
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;
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
        SetRect(go.GetComponent<RectTransform>(), new Vector2(900f, 470f), new Vector2(86f, 86f));

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
        SetRect(arrowObject.GetComponent<RectTransform>(), Vector2.zero, new Vector2(46f, 46f));
        Image arrow = arrowObject.AddComponent<Image>();
        arrow.sprite = arrowSprite;
        arrow.preserveAspect = true;
        arrow.color = Color.white;
        arrow.raycastTarget = false;
        arrow.rectTransform.localScale = new Vector3(-1f, 1f, 1f);

        CenterMenuButtonRelay relay = go.AddComponent<CenterMenuButtonRelay>();
        SerializedObject serializedRelay = new SerializedObject(relay);
        serializedRelay.FindProperty("sourceButton").objectReferenceValue = source;
        serializedRelay.ApplyModifiedPropertiesWithoutUndo();
        UnityEventTools.AddPersistentListener(button.onClick, relay.InvokeSource);
    }
}
#endif
