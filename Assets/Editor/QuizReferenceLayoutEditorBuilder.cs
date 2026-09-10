using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class QuizReferenceLayoutEditorBuilder
{
    private static readonly Color Ink = new Color(0.02f, 0.29f, 0.08f, 1f);
    private static readonly Color Cream = new Color(1f, 0.98f, 0.87f, 1f);
    private static readonly Color Panel = new Color(1f, 0.97f, 0.72f, 0.96f);
    private static readonly Color Green = new Color(0.16f, 0.58f, 0.05f, 1f);
    private static readonly Color Blue = new Color(0.04f, 0.58f, 0.88f, 1f);
    private static readonly Color Orange = new Color(1f, 0.55f, 0.02f, 1f);

    [MenuItem("MicroFun/Build Kuiz 1 Reference Layout")]
    public static void Build()
    {
        GameObject canvasObject = GameObject.Find("Canvas");
        if (canvasObject == null) { Debug.LogError("Canvas not found."); return; }

        Transform canvas = canvasObject.transform;
        QuizController quiz = canvas.GetComponent<QuizController>();
        Transform content = canvas.Find("Content");
        Transform panel = content == null ? null : content.Find("bg");
        Transform question = content == null ? null : content.Find("Soalan");
        Transform options = content == null ? null : content.Find("OptionsButtons");
        Transform title = canvas.Find("Title");
        Transform score = canvas.Find("Score");
        Transform back = canvas.Find("Back");

        if (quiz == null || content == null || panel == null || question == null || options == null || title == null || score == null || back == null)
        {
            Debug.LogError("Expected kuiz1 UI objects were not found.");
            return;
        }

        Transform old = canvas.Find("QuizDecor");
        if (old != null) Object.DestroyImmediate(old.gameObject);

        RemoveLayouts(content.gameObject);
        RemoveLayouts(panel.gameObject);
        RemoveLayouts(options.gameObject);
        Transform group1 = options.Find("1"); if (group1 != null) RemoveLayouts(group1.gameObject);
        Transform group2 = options.Find("2"); if (group2 != null) RemoveLayouts(group2.gameObject);

        Rect(content, new Vector2(0, -22), new Vector2(1500, 680));
        Rect(panel, Vector2.zero, new Vector2(1500, 680));
        Style(EnsureImage(panel.gameObject), "Assets/Images/UI Flat/btn_white_flat.png", Panel);

        Rect(title, new Vector2(0, 468), new Vector2(410, 92));
        Style(EnsureImage(title.gameObject), "Assets/Images/UI Flat/btn_white_flat.png", Cream);
        TextStyle(title.GetComponentInChildren<TextMeshProUGUI>(true), "KUIZ", 58, Ink, TextAlignmentOptions.Center);

        Rect(score, new Vector2(0, 256), new Vector2(450, 74));
        Style(EnsureImage(score.gameObject), "Assets/Images/UI Flat/btn_green_flat.png", Green);
        TextStyle(score.GetComponentInChildren<TextMeshProUGUI>(true), "Soalan 1 daripada 3", 34, Color.white, TextAlignmentOptions.Center);

        Rect(question, new Vector2(0, 165), new Vector2(1260, 118));
        Image questionImage = question.GetComponent<Image>(); if (questionImage != null) questionImage.enabled = false;
        TextStyle(quiz.questionText.GetComponent<TextMeshProUGUI>(), "Apakah definisi mikroorganisma?", 48, Ink, TextAlignmentOptions.Center);

        Rect(options, new Vector2(0, -75), new Vector2(1110, 430));
        for (int i = 0; i < quiz.buttonAnswers.Count; i++)
        {
            Transform answer = quiz.buttonAnswers[i];
            if (answer == null) continue;
            if (i >= 3) { answer.gameObject.SetActive(false); continue; }

            answer.SetParent(options, false);
            answer.gameObject.SetActive(true);
            Rect(answer, new Vector2(0, 95 - 145 * i), new Vector2(1080, 122));
            Image image = EnsureImage(answer.gameObject);
            Style(image, "Assets/Images/UI Flat/btn_white_flat.png", Cream);
            Button button = answer.GetComponent<Button>(); if (button == null) button = answer.gameObject.AddComponent<Button>(); button.targetGraphic = image;
            TextMeshProUGUI answerText = answer.GetComponentInChildren<TextMeshProUGUI>(true);
            answerText.gameObject.name = "AnswerText";
            Rect(answerText.transform, new Vector2(52, 0), new Vector2(820, 105));
            TextStyle(answerText, answerText.text, 32, Ink, TextAlignmentOptions.MidlineLeft);
            MakeBadge(answer, i);
            Feedback(answer.gameObject);
        }

        BuildBack(back);
        Transform decor = BuildDecor(canvas);
        Button next = BuildNext(decor);
        BuildAudio(decor);

        SerializedObject serial = new SerializedObject(quiz);
        SerializedProperty nextProperty = serial.FindProperty("nextButton");
        if (nextProperty != null) nextProperty.objectReferenceValue = next;
        serial.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.MarkSceneDirty(canvasObject.scene);
        Debug.Log("Kuiz 1 reference layout built.");
    }

    private static Transform BuildDecor(Transform canvas)
    {
        GameObject rootObject = new GameObject("QuizDecor", typeof(RectTransform));
        Transform root = rootObject.transform;
        root.SetParent(canvas, false);
        root.SetAsFirstSibling();
        Rect(root, Vector2.zero, new Vector2(1920, 1080));

        Image bg = NewImage("WarmBackground", root, "Assets/Images/UI Flat/btn_white_flat.png", new Color(1f, 0.85f, 0.51f, 1f));
        Rect(bg.transform, Vector2.zero, new Vector2(1920, 1080));
        Deco("GreenMicrobe", root, "Assets/Images/UI Flat/char_green_microbe.png", new Vector2(-650, 300), new Vector2(210,210));
        Deco("PurpleMicrobe", root, "Assets/Images/UI Flat/char_purple_microbe.png", new Vector2(600, 300), new Vector2(200,200));
        Deco("WaterDrop", root, "Assets/Images/UI Flat/char_water_drop.png", new Vector2(790, 230), new Vector2(135,185));
        Deco("Flask", root, "Assets/Images/UI Flat/icon_flask.png", new Vector2(-825, -330), new Vector2(160,245));
        Deco("PetriDish", root, "Assets/Images/UI Flat/icon_petri_dish.png", new Vector2(280, -410), new Vector2(235,165));
        Deco("MoldyBread", root, "Assets/Images/UI Flat/img_fun_bread.png", new Vector2(770, -330), new Vector2(210,220));
        Deco("Magnifier", root, "Assets/Images/UI Flat/icon_magnify_glass.png", new Vector2(-630, -265), new Vector2(135,135));
        Plus(root, new Vector2(-410,320), Green); Plus(root, new Vector2(-500,390), Orange); Plus(root, new Vector2(430,330), Orange);
        return root;
    }

    private static void BuildBack(Transform back)
    {
        Rect(back, new Vector2(-720,-472), new Vector2(350,82));
        Image baseImage = EnsureImage(back.gameObject);
        Style(baseImage, "Assets/Images/UI Flat/btn_white_flat.png", Cream);
        Button button = back.GetComponent<Button>(); if (button != null) button.targetGraphic = baseImage;
        TextMeshProUGUI label = back.GetComponentInChildren<TextMeshProUGUI>(true);
        Rect(label.transform, new Vector2(54,0), new Vector2(230,78)); TextStyle(label, "Laman Utama", 29, Ink, TextAlignmentOptions.MidlineLeft);
        Image circle = NewImage("HomeCircle", back, "Assets/Images/UI Flat/btn_green_flat.png", Green); Rect(circle.transform, new Vector2(-126,0), new Vector2(86,86));
        Image icon = NewImage("HomeIcon", circle.transform, "Assets/Images/UI Flat/icon_home.png", Color.white); Rect(icon.transform, Vector2.zero, new Vector2(44,44));
        Feedback(back.gameObject);
    }

    private static Button BuildNext(Transform root)
    {
        Image baseImage = NewImage("SeterusnyaButton", root, "Assets/Images/UI Flat/btn_white_flat.png", Cream);
        Rect(baseImage.transform, new Vector2(720,-472), new Vector2(350,82));
        Button button = baseImage.gameObject.AddComponent<Button>(); button.targetGraphic = baseImage;
        Feedback(baseImage.gameObject);
        TextMeshProUGUI label = NewText("Label", baseImage.transform, "Seterusnya", 29, Ink, TextAlignmentOptions.MidlineRight); Rect(label.transform,new Vector2(-45,0),new Vector2(225,78));
        Image circle = NewImage("ArrowCircle", baseImage.transform, "Assets/Images/UI Flat/btn_green_flat.png", Green); Rect(circle.transform,new Vector2(126,0),new Vector2(86,86));
        Image arrow = NewImage("ArrowIcon", circle.transform, "Assets/Images/UI Flat/ic_arrow_thick.png", Color.white); Rect(arrow.transform,Vector2.zero,new Vector2(46,46));
        return button;
    }

    private static void BuildAudio(Transform root)
    {
        Image baseImage = NewImage("DengarAudioButton", root, "Assets/Images/UI Flat/btn_white_flat.png", Cream);
        Rect(baseImage.transform, new Vector2(-720,468), new Vector2(355,82));
        Button button = baseImage.gameObject.AddComponent<Button>(); button.targetGraphic = baseImage;
        Feedback(baseImage.gameObject);
        Image circle = NewImage("SpeakerCircle", baseImage.transform, "Assets/Images/UI Flat/btn_green_flat.png", Green); Rect(circle.transform,new Vector2(-126,0),new Vector2(86,86));
        Image speaker = NewImage("SpeakerIcon", circle.transform, "Assets/Images/UI Flat/icon_speaker.png", Color.white); Rect(speaker.transform,Vector2.zero,new Vector2(47,47));
        TextMeshProUGUI label = NewText("Label", baseImage.transform, "Dengar audio", 29, Ink, TextAlignmentOptions.MidlineLeft); Rect(label.transform,new Vector2(52,0),new Vector2(235,78));
        AudioSource source = baseImage.GetComponent<AudioSource>(); if (source == null) source = baseImage.gameObject.AddComponent<AudioSource>();
        AudioNarrationButton narration = baseImage.GetComponent<AudioNarrationButton>(); if (narration == null) narration = baseImage.gameObject.AddComponent<AudioNarrationButton>();
        SerializedObject serial = new SerializedObject(narration);
        SerializedProperty property = serial.FindProperty("narrationClip");
        if(property != null) property.objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/akt1_intro_voice.mp3");
        serial.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void MakeBadge(Transform answer, int index)
    {
        Transform old = answer.Find("LetterBadge"); if(old != null) Object.DestroyImmediate(old.gameObject);
        Color color = index==0 ? Green : (index==1 ? Blue : Orange);
        Image badge = NewImage("LetterBadge", answer, "Assets/Images/UI Flat/btn_green_flat.png", color); Rect(badge.transform,new Vector2(-458,0),new Vector2(86,86));
        TextMeshProUGUI letter = NewText("LetterLabel", badge.transform, ((char)('A'+index)).ToString(), 48, Color.white, TextAlignmentOptions.Center); Rect(letter.transform,Vector2.zero,new Vector2(80,80));
    }

    private static void Deco(string name, Transform root, string path, Vector2 position, Vector2 size) { Image image = NewImage(name,root,path,Color.white); Rect(image.transform,position,size); image.raycastTarget=false; }
    private static void Plus(Transform root, Vector2 position, Color color) { TextMeshProUGUI plus=NewText("Plus",root,"+",44,color,TextAlignmentOptions.Center); Rect(plus.transform,position,new Vector2(55,55)); }
    private static Image NewImage(string name, Transform parent, string path, Color color) { GameObject go=new GameObject(name,typeof(RectTransform),typeof(CanvasRenderer),typeof(Image)); go.transform.SetParent(parent,false); Image image=go.GetComponent<Image>(); Style(image,path,color); return image; }
    private static TextMeshProUGUI NewText(string name, Transform parent, string content, float size, Color color, TextAlignmentOptions alignment) { GameObject go=new GameObject(name,typeof(RectTransform),typeof(CanvasRenderer),typeof(TextMeshProUGUI)); go.transform.SetParent(parent,false); TextMeshProUGUI text=go.GetComponent<TextMeshProUGUI>(); text.font=TMP_Settings.defaultFontAsset; TextStyle(text,content,size,color,alignment); return text; }
    private static void Style(Image image, string path, Color color) { image.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(path); image.type=Image.Type.Sliced; image.color=color; image.raycastTarget=false; }
    private static Image EnsureImage(GameObject go) { Image image=go.GetComponent<Image>(); if(image==null) image=go.AddComponent<Image>(); return image; }
    private static void TextStyle(TextMeshProUGUI text, string content, float size, Color color, TextAlignmentOptions alignment) { if(text==null)return; text.text=content; text.fontSize=size; text.color=color; text.alignment=alignment; text.enableWordWrapping=true; text.raycastTarget=false; }
    private static void Rect(Transform transform, Vector2 position, Vector2 size) { RectTransform rect=transform.GetComponent<RectTransform>(); if(rect==null)return; rect.anchorMin=new Vector2(.5f,.5f); rect.anchorMax=new Vector2(.5f,.5f); rect.pivot=new Vector2(.5f,.5f); rect.anchoredPosition=position; rect.sizeDelta=size; rect.localScale=Vector3.one; }
    private static void RemoveLayouts(GameObject go) { if(go==null)return; LayoutGroup group=go.GetComponent<LayoutGroup>(); if(group!=null)Object.DestroyImmediate(group); ContentSizeFitter fitter=go.GetComponent<ContentSizeFitter>(); if(fitter!=null)Object.DestroyImmediate(fitter); }
    private static void Feedback(GameObject go) { if(go.GetComponent<ButtonInteractionFeedback>()==null)go.AddComponent<ButtonInteractionFeedback>(); }
}