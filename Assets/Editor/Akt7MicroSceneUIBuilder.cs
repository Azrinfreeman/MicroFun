#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Applies the established Activity 7 theme to Micro Scene 7 while retaining
/// the existing hotspot objects, hover Animators, modal callbacks and page toggles.
/// </summary>
public static class Akt7MicroSceneUIBuilder
{
    private const string ScenePath = "Assets/Scenes/Aktivity/akt7/microscene7.unity";
    private const string RootName = "Akt7MicroSceneThemeRoot";
    private const string BackgroundPath = "Assets/Images/Akt8CenterMenuUI/Background/akt8_center_background.png";
    private const string WhitePanelPath = "Assets/Images/UI Flat/btn_white_flat.png";
    private const string GreenButtonPath = "Assets/Images/UI Flat/btn_green_flat.png";
    private const string GreenMicrobePath = "Assets/Images/UI Flat/char_green_microbe.png";
    private const string PurpleMicrobePath = "Assets/Images/UI Flat/char_purple_microbe.png";
    private const string WaterDropPath = "Assets/Images/UI Flat/char_water_drop.png";
    private const string MicroscopePath = "Assets/Images/UI Flat/img_microscope_large.png";
    private const string PetriPath = "Assets/Images/UI Flat/img_petri_bakteria.png";
    private const string MagnifierPath = "Assets/Images/UI Flat/icon_magnify_glass.png";
    private const string PlayPath = "Assets/Images/UI Flat/icon_play.png";
    private const string FontPath = "Assets/Fonts/FredokaOne SDF.asset";

    private static readonly Color Ink = new Color(0.02f, 0.28f, 0.08f, 1f);
    private static readonly Color Cream = new Color(1f, 0.98f, 0.88f, 0.98f);
    private static readonly Color Green = new Color(0.18f, 0.66f, 0.04f, 1f);
    private static readonly Color DeepGreen = new Color(0.02f, 0.37f, 0.04f, 1f);
    private static readonly Color Gold = new Color(1f, 0.66f, 0.03f, 1f);
    private static readonly Color SoftGold = new Color(0.94f, 0.72f, 0.28f, 0.95f);

    private static TMP_FontAsset font;
    private static Sprite whitePanel;
    private static Sprite greenButton;

    [MenuItem("Tools/MicroFun/Activity 7/Build Micro Scene 7 Theme")]
    public static void Build()
    {
        if (EditorSceneManager.GetActiveScene().path != ScenePath)
        {
            Debug.LogError("Akt7MicroSceneUIBuilder: open " + ScenePath + " before rebuilding.");
            return;
        }

        GameObject canvasObject = GameObject.Find("Canvas");
        Transform canvas = canvasObject != null ? canvasObject.transform : null;
        Transform pageOne = canvas != null ? canvas.Find("Content") : null;
        Transform pageTwo = canvas != null ? canvas.Find("Content (1)") : null;
        Transform next = canvas != null ? canvas.Find("next") : null;
        Transform previous = canvas != null ? canvas.Find("back") : null;
        Transform sceneBack = canvas != null ? canvas.Find("Back") : null;
        Transform title = canvas != null ? canvas.Find("Title") : null;
        Transform gigiModal = canvas != null ? canvas.Find("GigiReput") : null;
        Transform rotiModal = canvas != null ? canvas.Find("RotiReput") : null;

        if (canvas == null || pageOne == null || pageTwo == null || next == null || previous == null ||
            sceneBack == null || gigiModal == null || rotiModal == null)
        {
            Debug.LogError("Akt7MicroSceneUIBuilder: one or more required Micro Scene objects are missing.");
            return;
        }

        font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        whitePanel = AssetDatabase.LoadAssetAtPath<Sprite>(WhitePanelPath);
        greenButton = AssetDatabase.LoadAssetAtPath<Sprite>(GreenButtonPath);
        Texture2D backgroundTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(BackgroundPath);
        if (font == null || whitePanel == null || greenButton == null || backgroundTexture == null)
        {
            Debug.LogError("Akt7MicroSceneUIBuilder: required theme assets are missing.");
            return;
        }

        Transform existing = canvas.Find(RootName);
        if (existing != null) Object.DestroyImmediate(existing.gameObject);

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        if (scaler != null)
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
        }

        
        ConfigureCursorTexture();
DisableLegacyBackground(canvas);
        GameObject root = BuildThemeRoot(canvas, backgroundTexture);
        TextMeshProUGUI pageIndicator = BuildHeader(root.transform);
        BuildPageOne(pageOne);
        BuildPageTwo(pageTwo);
        StyleNavigation(next, previous, pageOne, pageTwo, pageIndicator, root.transform);
        StyleSceneBack(sceneBack);
        StyleModal(gigiModal, "GIGI REPUT");
        StyleModal(rotiModal, "PEREPUTAN ROTI");

        if (title != null) title.gameObject.SetActive(false);
        pageOne.gameObject.SetActive(true);
        pageTwo.gameObject.SetActive(false);
        next.gameObject.SetActive(true);
        previous.gameObject.SetActive(false);
        gigiModal.gameObject.SetActive(false);
        rotiModal.gameObject.SetActive(false);

        root.transform.SetAsFirstSibling();
        pageOne.SetAsLastSibling();
        pageTwo.SetAsLastSibling();
        next.SetAsLastSibling();
        previous.SetAsLastSibling();
        sceneBack.SetAsLastSibling();
        gigiModal.SetAsLastSibling();
        rotiModal.SetAsLastSibling();

        EditorUtility.SetDirty(canvasObject);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Selection.activeGameObject = root;
        Debug.Log("Activity 7 Micro Scene theme built and saved. Existing hotspots, animations, modals and page callbacks were preserved.");
    }

    private static GameObject BuildThemeRoot(Transform canvas, Texture2D backgroundTexture)
    {
        GameObject root = UI(RootName, canvas);
        Stretch(root.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero);

        GameObject backdrop = new GameObject("WarmScienceBackground", typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
        backdrop.layer = canvas.gameObject.layer;
        backdrop.transform.SetParent(root.transform, false);
        Stretch(backdrop.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero);
        RawImage raw = backdrop.GetComponent<RawImage>();
        raw.texture = backgroundTexture;
        raw.color = Color.white;
        raw.raycastTarget = false;

        GameObject wash = Panel("WarmWash", root.transform, new Color(1f, 0.93f, 0.64f, 0.27f));
        Stretch(wash.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero);

        AddDecoration(root.transform, "GreenMicrobe", GreenMicrobePath, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(36f, -165f), new Vector2(205f, 205f), 0.72f);
        AddDecoration(root.transform, "PurpleMicrobe", PurpleMicrobePath, Vector2.one, Vector2.one, new Vector2(-35f, -160f), new Vector2(205f, 205f), 0.72f);
        AddDecoration(root.transform, "Microscope", MicroscopePath, Vector2.zero, Vector2.zero, new Vector2(18f, 15f), new Vector2(230f, 260f), 0.62f);
        AddDecoration(root.transform, "PetriDish", PetriPath, Vector2.zero, Vector2.zero, new Vector2(200f, 18f), new Vector2(205f, 130f), 0.68f);
        AddDecoration(root.transform, "WaterDrop", WaterDropPath, new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-185f, 22f), new Vector2(120f, 165f), 0.72f);
        AddDecoration(root.transform, "Magnifier", MagnifierPath, new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-12f, 5f), new Vector2(200f, 200f), 0.66f);

        Bubble(root.transform, "BubbleA", new Vector2(-565f, 430f), 42f);
        Bubble(root.transform, "BubbleB", new Vector2(550f, 430f), 28f);
        Bubble(root.transform, "BubbleC", new Vector2(-850f, 30f), 24f);
        Bubble(root.transform, "BubbleD", new Vector2(850f, 35f), 32f);
        Spark(root.transform, "BlueSpark", new Vector2(-500f, 330f), new Color(0.02f, 0.59f, 1f, 1f));
        Spark(root.transform, "GoldSpark", new Vector2(-420f, 275f), Gold);
        Spark(root.transform, "PurpleSpark", new Vector2(470f, 330f), new Color(0.62f, 0.22f, 0.92f, 1f));

        return root;
    }

    private static TextMeshProUGUI BuildHeader(Transform parent)
    {
        GameObject badge = Panel("ActivityBadge", parent, new Color(1f, 0.96f, 0.76f, 0.98f));
        SetRect(badge.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -26f), new Vector2(400f, 72f));
        AddOutline(badge, SoftGold, new Vector2(3f, 3f));
        AddShadow(badge, new Color(0.25f, 0.18f, 0.04f, 0.25f), new Vector2(0f, -5f));
        TextMeshProUGUI badgeText = Text("Text", badge.transform, "AKTIVITI 7", 42f, Ink, TextAlignmentOptions.Center);
        Stretch(badgeText.rectTransform, new Vector2(12f, 2f), new Vector2(-12f, -2f));

        TextMeshProUGUI title = Text("Title", parent, "MICRO-SCENE", 88f, DeepGreen, TextAlignmentOptions.Center);
        SetRect(title.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -105f), new Vector2(1060f, 108f));
        title.outlineColor = new Color32(255, 250, 226, 255);
        title.outlineWidth = 0.18f;
        AddShadow(title.gameObject, new Color(0.12f, 0.16f, 0.04f, 0.30f), new Vector2(5f, -7f));

        GameObject subtitlePill = Panel("SubtitlePill", parent, new Color(1f, 0.99f, 0.91f, 0.93f));
        SetRect(subtitlePill.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -207f), new Vector2(1040f, 58f));
        AddOutline(subtitlePill, new Color(0.30f, 0.62f, 0.14f, 0.92f), new Vector2(2f, 2f));
        TextMeshProUGUI subtitle = Text("Text", subtitlePill.transform, "Terokai situasi dan kenali mikroorganisma yang terlibat.", 28f, Ink, TextAlignmentOptions.Center);
        Stretch(subtitle.rectTransform, new Vector2(20f, 2f), new Vector2(-20f, -2f));

        GameObject pagePill = Panel("PageIndicator", parent, new Color(1f, 0.97f, 0.82f, 0.98f));
        SetRect(pagePill.GetComponent<RectTransform>(), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-48f, -45f), new Vector2(250f, 60f));
        AddOutline(pagePill, SoftGold, new Vector2(2f, 2f));
        AddShadow(pagePill, new Color(0.25f, 0.18f, 0.04f, 0.18f), new Vector2(0f, -4f));
        TextMeshProUGUI indicator = Text("Text", pagePill.transform, "Halaman 1/2", 29f, Ink, TextAlignmentOptions.Center);
        Stretch(indicator.rectTransform, new Vector2(10f, 1f), new Vector2(-10f, -1f));
        return indicator;
    }

    private static void BuildPageOne(Transform page)
    {
        RemoveLayout(page.gameObject);
        SetRect(page as RectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -92f), new Vector2(1700f, 700f));

        Transform rowOne = page.Find("1");
        Transform rowTwo = page.Find("2");
        
        rowOne.gameObject.SetActive(true);
        rowTwo.gameObject.SetActive(true);
if (rowOne == null || rowTwo == null) return;
        RemoveLayout(rowOne.gameObject);
        RemoveLayout(rowTwo.gameObject);
        SetRect(rowOne as RectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 165f), new Vector2(1660f, 325f));
        SetRect(rowTwo as RectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -165f), new Vector2(1660f, 325f));

        StyleCard(rowOne.Find("Bread"), "Roti Berkulat", -555f, new Color32(255, 157, 21, 255));
        StyleCard(rowOne.Find("Dirt"), "Tanah", 0f, new Color32(45, 157, 102, 255));
        StyleCard(rowOne.Find("RawChicken"), "Ayam Mentah", 555f, new Color32(237, 86, 111, 255));
        StyleCard(rowTwo.Find("Dough"), "Doh", -555f, new Color32(250, 176, 28, 255));
        StyleCard(rowTwo.Find("Water"), "Air", 0f, new Color32(27, 149, 220, 255));
        StyleCard(rowTwo.Find("Streptococcus"), "Streptococcus", 555f, new Color32(126, 72, 205, 255));
    }

    private static void BuildPageTwo(Transform page)
    {
        RemoveLayout(page.gameObject);
        SetRect(page as RectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -92f), new Vector2(1700f, 700f));
        Transform row = page.Find("1");
        
        row.gameObject.SetActive(true);
if (row == null) return;
        RemoveLayout(row.gameObject);
        SetRect(row as RectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(1660f, 650f));
        Transform card = row.Find("Bread");
        
        card.gameObject.SetActive(true);
        Transform reveal = card.Find("Content");
        if (reveal != null) reveal.gameObject.SetActive(false);
if (card == null) return;

        RemoveLayout(card.gameObject);
        RectTransform rect = card as RectTransform;
        SetRect(rect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -10f), rect.sizeDelta);
        rect.localScale = Vector3.one * 0.88f;
        AddCardEffects(card.gameObject, new Color32(68, 152, 199, 255));
        AddCardLabel(card, "Spirillum", new Color32(68, 152, 199, 255));

        GameObject instruction = Panel("FeaturedInstruction", page, new Color(1f, 0.98f, 0.86f, 0.96f));
        SetRect(instruction.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 22f), new Vector2(620f, 62f));
        AddOutline(instruction, new Color(0.26f, 0.62f, 0.76f, 0.95f), new Vector2(2f, 2f));
        TextMeshProUGUI text = Text("Text", instruction.transform, "Gerakkan penuding pada imej untuk melihat maklumat.", 25f, Ink, TextAlignmentOptions.Center);
        Stretch(text.rectTransform, new Vector2(16f, 2f), new Vector2(-16f, -2f));
    }

private static void StyleCard(Transform card, string label, float x, Color32 accent)
    {
        if (card == null) return;
        card.gameObject.SetActive(true);
        RemoveLayout(card.gameObject);
        RectTransform rect = card as RectTransform;
        SetRect(rect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(x, 0f), rect.sizeDelta);
        rect.localScale = Vector3.one * 0.82f;
        Transform reveal = card.Find("Content");
        if (reveal != null) reveal.gameObject.SetActive(false);
        AddCardEffects(card.gameObject, accent);
        AddCardLabel(card, label, accent);
    }

    private static void AddCardEffects(GameObject card, Color32 accent)
    {
        RemoveEffects(card);
        Image image = card.GetComponent<Image>();
        if (image != null)
        {
            image.color = Color.white;
            image.raycastTarget = false;
        }
        AddOutline(card, accent, new Vector2(7f, 7f));
        AddShadow(card, new Color32(62, 63, 30, 90), new Vector2(0f, -10f));
    }

    private static void AddCardLabel(Transform card, string value, Color32 accent)
    {
        Transform old = card.Find("ThemeLabel");
        if (old != null) Object.DestroyImmediate(old.gameObject);
        GameObject label = Panel("ThemeLabel", card, new Color32(255, 250, 225, 247));
        SetRect(label.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 12f), new Vector2(365f, 66f));
        AddOutline(label, accent, new Vector2(3f, 3f));
        AddShadow(label, new Color32(61, 57, 29, 70), new Vector2(0f, -5f));
        TextMeshProUGUI text = Text("Text", label.transform, value, 30f, Ink, TextAlignmentOptions.Center);
        Stretch(text.rectTransform, new Vector2(12f, 2f), new Vector2(-12f, -2f));
        text.enableAutoSizing = true;
        text.fontSizeMin = 21f;
        text.fontSizeMax = 30f;
        label.transform.SetAsLastSibling();
    }

    private static void StyleNavigation(Transform nextTransform, Transform previousTransform, Transform pageOne,
        Transform pageTwo, TextMeshProUGUI indicator, Transform themeRoot)
    {
        Button next = nextTransform.GetComponent<Button>();
        Button previous = previousTransform.GetComponent<Button>();
        if (next == null || previous == null) return;

        StyleNavigationButton(next, "SETERUSNYA", true);
        StyleNavigationButton(previous, "SEBELUMNYA", false);
        SetRect(nextTransform as RectTransform, new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-48f, 35f), new Vector2(260f, 72f));
        SetRect(previousTransform as RectTransform, Vector2.zero, Vector2.zero, new Vector2(48f, 35f), new Vector2(260f, 72f));

        Akt7MicroScenePresenter presenter = themeRoot.gameObject.AddComponent<Akt7MicroScenePresenter>();
        SerializedObject serialized = new SerializedObject(presenter);
        serialized.FindProperty("firstPage").objectReferenceValue = pageOne.gameObject;
        serialized.FindProperty("secondPage").objectReferenceValue = pageTwo.gameObject;
        serialized.FindProperty("pageIndicator").objectReferenceValue = indicator;
        serialized.ApplyModifiedPropertiesWithoutUndo();

        RemovePersistentRefreshListeners(next, presenter);
        RemovePersistentRefreshListeners(previous, presenter);
        UnityEventTools.AddPersistentListener(next.onClick, presenter.RefreshPageIndicator);
        UnityEventTools.AddPersistentListener(previous.onClick, presenter.RefreshPageIndicator);
    }

    private static void RemovePersistentRefreshListeners(Button button, Akt7MicroScenePresenter presenter)
    {
        for (int i = button.onClick.GetPersistentEventCount() - 1; i >= 0; i--)
        {
            if (button.onClick.GetPersistentMethodName(i) == nameof(Akt7MicroScenePresenter.RefreshPageIndicator))
                UnityEventTools.RemovePersistentListener(button.onClick, i);
        }
    }

    private static void StyleNavigationButton(Button button, string labelValue, bool forward)
    {
        RemoveLayout(button.gameObject);
        Image image = button.GetComponent<Image>();
        image.sprite = forward ? greenButton : whitePanel;
        image.type = Image.Type.Sliced;
        image.color = forward ? Green : Cream;
        image.raycastTarget = true;
        button.targetGraphic = image;
        button.navigation = new Navigation { mode = Navigation.Mode.None };
        RemoveEffects(button.gameObject);
        AddOutline(button.gameObject, forward ? DeepGreen : new Color(0.20f, 0.52f, 0.12f, 1f), new Vector2(3f, 3f));
        AddShadow(button.gameObject, new Color(0.05f, 0.20f, 0.02f, 0.30f), new Vector2(0f, -6f));

        TextMeshProUGUI label = button.GetComponentInChildren<TextMeshProUGUI>(true);
        if (label != null)
        {
            label.font = font;
            label.text = labelValue;
            label.fontSize = 28f;
            label.fontStyle = FontStyles.Bold;
            label.color = forward ? Color.white : Ink;
            label.alignment = TextAlignmentOptions.Center;
            label.raycastTarget = false;
        }

        Transform oldArrow = button.transform.Find("ThemeArrow");
        if (oldArrow != null) Object.DestroyImmediate(oldArrow.gameObject);
        Image arrow = AddImage("ThemeArrow", button.transform, LoadSprite(PlayPath), forward ? Gold : Green);
        SetRect(arrow.rectTransform, new Vector2(forward ? 1f : 0f, 0.5f), new Vector2(forward ? 1f : 0f, 0.5f), new Vector2(forward ? -20f : 20f, 0f), new Vector2(38f, 38f));
        arrow.rectTransform.localScale = new Vector3(forward ? 1f : -1f, 1f, 1f);
    }

    private static void StyleSceneBack(Transform back)
    {
        RemoveLayout(back.gameObject);
        SetRect(back as RectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(28f, -28f), new Vector2(76f, 76f));
        Image image = back.GetComponent<Image>();
        if (image != null)
        {
            image.sprite = greenButton;
            image.type = Image.Type.Sliced;
            image.color = Green;
        }
        RemoveEffects(back.gameObject);
        AddOutline(back.gameObject, DeepGreen, new Vector2(3f, 3f));
        AddShadow(back.gameObject, new Color(0f, 0.18f, 0f, 0.35f), new Vector2(0f, -5f));
        TextMeshProUGUI label = back.GetComponentInChildren<TextMeshProUGUI>(true);
        if (label != null)
        {
            label.font = font;
            label.text = "‹";
            label.fontSize = 58f;
            label.color = Color.white;
            label.alignment = TextAlignmentOptions.Center;
            label.raycastTarget = false;
        }
    }

    private static void StyleModal(Transform modal, string titleValue)
    {
        RectTransform modalRect = modal as RectTransform;
        Stretch(modalRect, Vector2.zero, Vector2.zero);
        Transform bg = modal.Find("bg");
        Transform content = modal.Find("Content");
        if (bg == null || content == null) return;

        Image bgImage = bg.GetComponent<Image>();
        if (bgImage != null)
        {
            bgImage.sprite = whitePanel;
            bgImage.type = Image.Type.Sliced;
            bgImage.color = new Color(0.02f, 0.10f, 0.03f, 0.62f);
            bgImage.raycastTarget = true;
        }
        Stretch(bg as RectTransform, Vector2.zero, Vector2.zero);

        SetRect(content as RectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(1280f, 720f));
        Image panel = content.GetComponent<Image>();
        if (panel != null)
        {
            panel.sprite = whitePanel;
            panel.type = Image.Type.Sliced;
            panel.color = new Color(1f, 0.98f, 0.88f, 1f);
        }
        RemoveEffects(content.gameObject);
        AddOutline(content.gameObject, new Color(0.30f, 0.62f, 0.14f, 1f), new Vector2(4f, 4f));
        AddShadow(content.gameObject, new Color(0f, 0.12f, 0f, 0.42f), new Vector2(0f, -12f));

        TextMeshProUGUI[] texts = content.GetComponentsInChildren<TextMeshProUGUI>(true);
        TextMeshProUGUI title = null;
        TextMeshProUGUI body = null;
        foreach (TextMeshProUGUI text in texts)
        {
            if (text.transform.parent == content && title == null) title = text;
            else if (text.transform.parent == content && body == null) body = text;
        }
        if (title != null)
        {
            title.font = font;
            title.text = titleValue;
            title.fontSize = 50f;
            title.fontStyle = FontStyles.Bold;
            title.color = DeepGreen;
            title.alignment = TextAlignmentOptions.Center;
            SetRect(title.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -35f), new Vector2(900f, 82f));
        }
        if (body != null)
        {
            body.font = font;
            body.fontSize = 30f;
            body.fontStyle = FontStyles.Normal;
            body.color = Ink;
            body.alignment = TextAlignmentOptions.TopLeft;
            body.enableWordWrapping = true;
            body.overflowMode = TextOverflowModes.Overflow;
            body.lineSpacing = 5f;
            body.text = body.text.Replace("<color=yellow>", "<color=#278B36>");
            Stretch(body.rectTransform, new Vector2(85f, 92f), new Vector2(-85f, -75f));
        }

        Button close = content.GetComponentInChildren<Button>(true);
        if (close != null)
        {
            SetRect(close.transform as RectTransform, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-24f, -24f), new Vector2(70f, 70f));
            Image closeImage = close.GetComponent<Image>();
            closeImage.sprite = greenButton;
            closeImage.type = Image.Type.Sliced;
            closeImage.color = Green;
            close.targetGraphic = closeImage;
            RemoveEffects(close.gameObject);
            AddOutline(close.gameObject, DeepGreen, new Vector2(3f, 3f));
            TextMeshProUGUI closeLabel = close.GetComponentInChildren<TextMeshProUGUI>(true);
            if (closeLabel != null)
            {
                closeLabel.font = font;
                closeLabel.text = "×";
                closeLabel.fontSize = 48f;
                closeLabel.color = Color.white;
                closeLabel.alignment = TextAlignmentOptions.Center;
                
                Stretch(closeLabel.rectTransform, Vector2.zero, Vector2.zero);
closeLabel.raycastTarget = false;
            }
        }
    }

    private static void DisableLegacyBackground(Transform canvas)
    {
        foreach (RawImage image in canvas.GetComponentsInChildren<RawImage>(true))
        {
            if (image.transform.parent == canvas) image.gameObject.SetActive(false);
        }
        foreach (UnityEngine.Video.VideoPlayer player in Object.FindObjectsByType<UnityEngine.Video.VideoPlayer>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (player.gameObject.scene == canvas.gameObject.scene) player.gameObject.SetActive(false);
        }
    }

    private static GameObject UI(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.layer = parent.gameObject.layer;
        go.transform.SetParent(parent, false);
        return go;
    }

    private static GameObject Panel(string name, Transform parent, Color color)
    {
        GameObject go = UI(name, parent);
        Image image = go.AddComponent<Image>();
        image.sprite = whitePanel;
        image.type = Image.Type.Sliced;
        image.color = color;
        image.raycastTarget = false;
        return go;
    }

    private static Image AddImage(string name, Transform parent, Sprite sprite, Color color)
    {
        GameObject go = UI(name, parent);
        Image image = go.AddComponent<Image>();
        image.sprite = sprite;
        image.preserveAspect = true;
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    private static TextMeshProUGUI Text(string name, Transform parent, string value, float size, Color color, TextAlignmentOptions alignment)
    {
        GameObject go = UI(name, parent);
        TextMeshProUGUI text = go.AddComponent<TextMeshProUGUI>();
        text.font = font;
        text.text = value;
        text.fontSize = size;
        text.fontStyle = FontStyles.Bold;
        text.color = color;
        text.alignment = alignment;
        text.raycastTarget = false;
        text.overflowMode = TextOverflowModes.Overflow;
        return text;
    }

    private static void AddDecoration(Transform parent, string name, string path, Vector2 anchor, Vector2 pivot,
        Vector2 position, Vector2 size, float alpha)
    {
        Sprite sprite = LoadSprite(path);
        if (sprite == null) return;
        Image image = AddImage(name, parent, sprite, new Color(1f, 1f, 1f, alpha));
        SetRect(image.rectTransform, anchor, pivot, position, size);
    }

    private static void Bubble(Transform parent, string name, Vector2 position, float size)
    {
        GameObject bubble = Panel(name, parent, new Color(1f, 1f, 1f, 0.20f));
        SetRect(bubble.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), position, Vector2.one * size);
        AddOutline(bubble, new Color(0.70f, 0.90f, 1f, 0.55f), new Vector2(2f, 2f));
    }

    private static void Spark(Transform parent, string name, Vector2 position, Color color)
    {
        TextMeshProUGUI text = Text(name, parent, "+", 46f, color, TextAlignmentOptions.Center);
        SetRect(text.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), position, new Vector2(50f, 50f));
    }

    private static Sprite LoadSprite(string path)
    {
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    private static void SetRect(RectTransform rect, Vector2 anchor, Vector2 pivot, Vector2 position, Vector2 size)
    {
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = pivot;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;
    }

    private static void Stretch(RectTransform rect, Vector2 minOffset, Vector2 maxOffset)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMin = minOffset;
        rect.offsetMax = maxOffset;
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;
    }

    private static void AddShadow(GameObject go, Color color, Vector2 distance)
    {
        Shadow shadow = go.AddComponent<Shadow>();
        shadow.effectColor = color;
        shadow.effectDistance = distance;
        shadow.useGraphicAlpha = true;
    }

    private static void AddOutline(GameObject go, Color color, Vector2 distance)
    {
        Outline outline = go.AddComponent<Outline>();
        outline.effectColor = color;
        outline.effectDistance = distance;
        outline.useGraphicAlpha = true;
    }

    private static void RemoveEffects(GameObject go)
    {
        foreach (Shadow effect in go.GetComponents<Shadow>()) Object.DestroyImmediate(effect);
    }

    private static void RemoveLayout(GameObject go)
    {
        foreach (LayoutGroup layout in go.GetComponents<LayoutGroup>()) Object.DestroyImmediate(layout);
        foreach (ContentSizeFitter fitter in go.GetComponents<ContentSizeFitter>()) Object.DestroyImmediate(fitter);
        foreach (LayoutElement element in go.GetComponents<LayoutElement>()) Object.DestroyImmediate(element);
    }


private static void ConfigureCursorTexture()
    {
        const string cursorPath = "Assets/Images/2d/bread/magnifiying_glass.png";
        TextureImporter importer = AssetImporter.GetAtPath(cursorPath) as TextureImporter;
        if (importer == null) return;
        importer.textureType = TextureImporterType.Default;
        importer.isReadable = true;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.npotScale = TextureImporterNPOTScale.None;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.SaveAndReimport();
    }
}
#endif
