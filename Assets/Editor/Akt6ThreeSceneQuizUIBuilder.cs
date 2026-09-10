using System;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Akt6ThreeSceneQuizUIBuilder
{
    const string F = "Assets/Scenes/Aktivity/akt6/";
    const string BG = "Assets/Images/Akt6QuizUI/Background/akt6_three_scene_quiz_background.png";
    const string W = "Assets/Images/UI Flat/btn_white_flat.png";
    const string G = "Assets/Images/UI Flat/btn_green_flat.png";
    const string FONT = "Assets/Fonts/FredokaOne SDF.asset";

    static readonly Color Ink = new Color32(11,79,36,255);
    static readonly Color Cream = new Color32(255,249,223,255);
    static readonly Color Green = new Color32(85,183,25,255);
    static readonly Color DarkGreen = new Color32(22,112,13,255);
    static readonly Color Blue = new Color32(40,169,232,255);
    static readonly Color Orange = new Color32(245,163,19,255);
    static readonly Color Purple = new Color32(152,88,213,255);
    static readonly Color Pink = new Color32(238,92,145,255);
    static readonly Color Teal = new Color32(32,184,167,255);
    static readonly Color Red = new Color32(232,76,76,255);
    static readonly Color Correct = new Color32(68,199,103,255);
    static TMP_FontAsset font;

    [MenuItem("Tools/MicroFun/Activity 6/Build Three-Scene Quiz Theme")]
    public static void Build()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) { Debug.LogError("Exit Play Mode first."); return; }
        font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FONT);
        if (font == null) { Debug.LogError("Fredoka One font not found."); return; }

        string original = SceneManager.GetActiveScene().path;
        string[] names = { "SceneV1", "SceneV2", "SceneV3" };
        Backup(names);
        try
        {
            foreach (string n in names)
            {
                Scene s = EditorSceneManager.OpenScene(F + n + ".unity", OpenSceneMode.Single);
                Canvas c = UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None).FirstOrDefault();
                if (c == null) throw new InvalidOperationException("Canvas missing in " + n);
                SetupCanvas(c);
                if (n == "SceneV1") V1(c); else if (n == "SceneV2") V2(c); else V3(c);
                FadeLast(c.transform);
                EditorSceneManager.MarkSceneDirty(s);
                EditorSceneManager.SaveScene(s);
                Debug.Log("[Akt6QuizTheme] Built " + n);
            }
        }
        finally
        {
            if (!string.IsNullOrEmpty(original)) EditorSceneManager.OpenScene(original, OpenSceneMode.Single);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        Debug.Log("[Akt6QuizTheme] All three scenes complete.");
    }

    static void Backup(string[] names)
    {
        string dir = F + "Backups";
        if (!AssetDatabase.IsValidFolder(dir)) AssetDatabase.CreateFolder(F.TrimEnd('/'), "Backups");
        foreach (string n in names)
        {
            string dst = dir + "/" + n + "_PreQuizTheme_20260901.unity";
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(dst) == null) AssetDatabase.CopyAsset(F+n+".unity", dst);
        }
    }

    static void SetupCanvas(Canvas c)
    {
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler s = c.GetComponent<CanvasScaler>() ?? c.gameObject.AddComponent<CanvasScaler>();
        s.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        s.referenceResolution = new Vector2(1920,1080);
        s.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        s.matchWidthOrHeight = .5f;
        Del(c.transform,"Akt6QuizThemeRoot"); Del(c.transform,"Akt6QuizChrome");

        RectTransform root = NR("Akt6QuizThemeRoot",c.transform); Full(root); root.SetAsFirstSibling();
        Image bg = NI("Background",root,BG,Color.white,Vector2.zero,new Vector2(1920,1080),false);
        bg.type=Image.Type.Simple; bg.preserveAspect=false; Full(bg.rectTransform);
        RectTransform chrome = NR("Akt6QuizChrome",c.transform); Full(chrome); chrome.SetAsLastSibling();
    }

    static void Header(Canvas c,string step,string title,string sub)
    {
        Transform r=c.transform.Find("Akt6QuizChrome");
        Pill("Activity",r,new Vector2(0,492),new Vector2(330,70),DarkGreen,Color.white,"AKTIVITI 6",36);
        Pill("Step",r,new Vector2(-560,492),new Vector2(260,62),Cream,Ink,step,28);
        Pill("Curriculum",r,new Vector2(650,492),new Vector2(390,62),Cream,Ink,"SP 3.1  |  SK 3.1.4",25);
        TextMeshProUGUI h=NT("PageTitle",r,title,68,DarkGreen,TextAlignmentOptions.Center,new Vector2(0,405),new Vector2(1000,100));
        TS(h,new Color(0.1f,0.1f,0.02f,.28f),new Vector2(5,-6));
        Pill("Subtitle",r,new Vector2(0,331),new Vector2(1080,58),new Color(1,.985f,.91f,.96f),Ink,sub,27);
        Back(r);
    }

    static void Back(Transform r)
    {
        Image im=NI("BackButton",r,W,Color.white,new Vector2(-810,490),new Vector2(190,68),true); im.type=Image.Type.Sliced;
        OL(im.gameObject,Ink,3); SH(im.gameObject);
        Button b=im.gameObject.AddComponent<Button>(); b.targetGraphic=im; BC(b,Cream,new Color(1,.95f,.69f),new Color(.88f,.98f,.77f),Color.gray);
        Image cir=NI("ArrowCircle",im.transform,G,Green,new Vector2(-61,0),new Vector2(62,62),false); cir.type=Image.Type.Sliced; OL(cir.gameObject,DarkGreen,2);
        Image arrow=NI("Arrow",cir.transform,"Assets/Images/UI Flat/icon_arrow_left.png",Color.white,Vector2.zero,new Vector2(36,36),false);arrow.preserveAspect=true;
        NT("Label",im.transform,"Kembali",24,Ink,TextAlignmentOptions.MidlineLeft,new Vector2(34,0),new Vector2(105,58));
        SceneLoader l=UnityEngine.Object.FindObjectsByType<SceneLoader>(FindObjectsInactive.Include,FindObjectsSortMode.None).FirstOrDefault();
        if(l!=null) UnityEventTools.AddStringPersistentListener(b.onClick,l.LoadScene,"akt6");
        FB(b.gameObject);
    }

static void V1(Canvas c)
    {
        Header(c,"Bahagian 1 / 3","KUIZ","Pilih jawapan yang paling tepat.");
        Transform legacyVideo = c.transform.Find("Image");
        if (legacyVideo != null && legacyVideo.GetComponent<RawImage>() != null)
            legacyVideo.gameObject.SetActive(false);

        Transform p=c.transform.Find("QuizPanel"); if(p==null) throw new InvalidOperationException("QuizPanel missing");
        R(p,new Vector2(0,-82),new Vector2(1360,660)); Panel(p.gameObject,new Color(1,.985f,.91f,.96f),Ink,4);
        Transform q=p.Find("QuestionText"); R(q,new Vector2(-90,215),new Vector2(880,120)); ST(q.GetComponent<TextMeshProUGUI>(),38,Ink,TextAlignmentOptions.Center);
        Transform qi=p.Find("QuestionImage"); if(qi!=null){R(qi,new Vector2(-500,85),new Vector2(250,180)); Image x=qi.GetComponent<Image>(); x.preserveAspect=true; OL(x.gameObject,Blue,3);}
        Transform ap=p.Find("AnswerButtonsPanel"); NoLayout(ap); R(ap,new Vector2(0,-78),new Vector2(1160,390));
        QuizManager m=UnityEngine.Object.FindObjectsByType<QuizManager>(FindObjectsInactive.Include,FindObjectsSortMode.None).FirstOrDefault();
        if(m==null) throw new InvalidOperationException("QuizManager missing");
        Color[] ac={Green,Blue,Orange,Purple};
        for(int i=0;i<m.answerButtons.Length;i++){Button b=m.answerButtons[i]; if(b==null)continue; R(b.transform,new Vector2(i%2==0?-285:285,i/2==0?88:-84),new Vector2(535,142)); Answer(b,i,ac[i],29);}
        Transform score=p.Find("ScoreText"); R(score,new Vector2(500,250),new Vector2(275,58)); ST(score.GetComponent<TextMeshProUGUI>(),24,Ink,TextAlignmentOptions.Center); TextPill(score.gameObject,Cream,DarkGreen);
        Transform feed=p.Find("FeedbackText"); R(feed,new Vector2(-145,-280),new Vector2(760,72)); ST(feed.GetComponent<TextMeshProUGUI>(),27,DarkGreen,TextAlignmentOptions.Center); Del(p,"FeedbackTextBackdrop");
        Transform next=p.Find("NextButton"); R(next,new Vector2(500,-282),new Vector2(300,74)); Action(next.GetComponent<Button>(),Green,"SETERUSNYA  ›",27);
        m.defaultColor=Cream; m.correctColor=Correct; m.wrongColor=Red; EditorUtility.SetDirty(m);
    }

    static void V2(Canvas c)
    {
        Header(c,"Bahagian 2 / 3","CABARAN SITUASI","Gunakan pengetahuan anda dalam situasi sebenar.");
        Transform p=c.transform.Find("ScenarioPanel"); if(p==null) throw new InvalidOperationException("ScenarioPanel missing");
        R(p,new Vector2(0,-85),new Vector2(1420,670)); Panel(p.gameObject,new Color(1,.985f,.91f,.96f),Ink,4);
        Transform si=p.Find("ScenarioImage"); R(si,new Vector2(-470,90),new Vector2(390,270)); Image sim=si.GetComponent<Image>(); sim.preserveAspect=true; OL(sim.gameObject,Teal,4); SH(sim.gameObject);
        Transform d=p.Find("ScenarioDescription"); R(d,new Vector2(-470,-108),new Vector2(430,126)); ST(d.GetComponent<TextMeshProUGUI>(),23,Ink,TextAlignmentOptions.Center); TextPill(d.gameObject,Cream,Teal);
        Transform q=p.Find("QuestionText"); R(q,new Vector2(245,220),new Vector2(780,100)); ST(q.GetComponent<TextMeshProUGUI>(),36,Ink,TextAlignmentOptions.Center);
        Transform ap=p.Find("AnswerButtonsPanel"); NoLayout(ap); R(ap,new Vector2(245,-65),new Vector2(825,415)); Image api=ap.GetComponent<Image>(); if(api!=null){api.color=Color.clear;api.raycastTarget=false;}
        ScenarioManager[] all=UnityEngine.Object.FindObjectsByType<ScenarioManager>(FindObjectsInactive.Include,FindObjectsSortMode.None);
        ScenarioManager m=all.FirstOrDefault(x=>x.gameObject.name=="ScenarioManager")??all.FirstOrDefault(); if(m==null) throw new InvalidOperationException("ScenarioManager missing");
        foreach(ScenarioManager x in all) if(x!=m) UnityEngine.Object.DestroyImmediate(x);
        SceneLoader loader=m.GetComponent<SceneLoader>(); if(loader!=null)m.sceneLoader=loader;
        Color[] ac={Green,Blue,Orange,Purple};
        for(int i=0;i<m.answerButtons.Length;i++){Button b=m.answerButtons[i];if(b==null)continue;R(b.transform,new Vector2(i%2==0?-195:195,i/2==0?92:-88),new Vector2(370,145));Answer(b,i,ac[i],25);}
        Transform ex=p.Find("ExplanationPanel"); R(ex,new Vector2(0,-20),new Vector2(1050,390)); Panel(ex.gameObject,new Color(1,.98f,.86f,.99f),DarkGreen,5); ex.SetAsLastSibling();
        Transform et=ex.Find("ExplanationText"); R(et,new Vector2(0,55),new Vector2(870,200)); ST(et.GetComponent<TextMeshProUGUI>(),29,Ink,TextAlignmentOptions.Center);
        Transform nx=ex.Find("NextButton"); R(nx,new Vector2(0,-125),new Vector2(390,82)); Action(nx.GetComponent<Button>(),Green,"TERUSKAN  ›",29);
        m.defaultColor=Cream;m.correctColor=Correct;m.wrongColor=Red;EditorUtility.SetDirty(m);
    }

static void V3(Canvas c)
    {
        Header(c,"Bahagian 3 / 3","REFLEKSI","Nilai kefahaman anda sebelum tamat.");
        Transform p=c.transform.Find("ReflectionPanel"); if(p==null) throw new InvalidOperationException("ReflectionPanel missing");
        R(p,new Vector2(0,-72),new Vector2(1320,650)); Panel(p.gameObject,new Color(1,.985f,.91f,.96f),Ink,4);
        ReflectionManager m=UnityEngine.Object.FindObjectsByType<ReflectionManager>(FindObjectsInactive.Include,FindObjectsSortMode.None).FirstOrDefault();
        if(m==null)throw new InvalidOperationException("ReflectionManager missing");
        for(int s=0;s<m.statementPanels.Length;s++)
        {
            GameObject sp=m.statementPanels[s];if(sp==null)continue;NoLayout(sp.transform);R(sp.transform,Vector2.zero,new Vector2(1120,450));Panel(sp,new Color(1,1,1,.74f),s==0?Blue:Purple,3);
            if(s<m.statementTexts.Length&&m.statementTexts[s]!=null){R(m.statementTexts[s].transform,new Vector2(0,105),new Vector2(930,150));ST(m.statementTexts[s],37,Ink,TextAlignmentOptions.Center);}
            Transform row=sp.transform.Find("EmojiButtonsRow"); if(row!=null)R(row,new Vector2(0,-105),new Vector2(1040,170));
            for(int e=0;e<3;e++){int i=s*3+e;if(i>=m.emojiButtons.Length||m.emojiButtons[i]==null)continue;Button b=m.emojiButtons[i];R(b.transform,new Vector2(-330+e*330,0),new Vector2(285,145));Action(b,e==0?Green:e==1?Orange:Pink,e==0?"Faham":e==1?"Tidak pasti":"Belum faham",25);}
        }
        Transform sub=p.Find("SubmitButton");R(sub,new Vector2(0,-250),new Vector2(380,78));Action(sub.GetComponent<Button>(),Green,"HANTAR REFLEKSI  ›",27);
        Transform warn=c.transform.Find("SubmitWarningText");if(warn!=null){R(warn,new Vector2(0,-425),new Vector2(850,54));ST(warn.GetComponent<TextMeshProUGUI>(),25,Red,TextAlignmentOptions.Center);}
        Transform sum=c.transform.Find("SummaryPanel");if(sum!=null){R(sum,new Vector2(0,-60),new Vector2(1160,650));Panel(sum.gameObject,new Color(1,.985f,.9f,.99f),DarkGreen,5);
            Transform fs=sum.Find("FinalScoreText");R(fs,new Vector2(0,190),new Vector2(850,110));ST(fs.GetComponent<TextMeshProUGUI>(),45,DarkGreen,TextAlignmentOptions.Center);
            Transform rs=sum.Find("ReflectionSummaryText");R(rs,new Vector2(0,25),new Vector2(920,230));ST(rs.GetComponent<TextMeshProUGUI>(),27,Ink,TextAlignmentOptions.Center);
            Transform re=sum.Find("RestartButton");R(re,new Vector2(-220,-205),new Vector2(370,82));Action(re.GetComponent<Button>(),Blue,"CUBA LAGI",29);
            Transform exit=sum.Find("KeluarButton");R(exit,new Vector2(220,-205),new Vector2(370,82));Action(exit.GetComponent<Button>(),Green,"TAMAT  ›",29);}
        m.defaultEmojiColor=Cream;m.selectedEmojiColor=new Color32(255,217,90,255);EditorUtility.SetDirty(m);
    }

    static void Answer(Button b,int i,Color accent,float fs)
    {
        Image im=b.GetComponent<Image>()??b.gameObject.AddComponent<Image>();im.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(W);im.type=Image.Type.Sliced;im.color=Color.white;b.targetGraphic=im;
        BC(b,Cream,new Color(1,.96f,.72f),new Color(.91f,.98f,.84f),Color.gray);OL(b.gameObject,accent,4);SH(b.gameObject);FB(b.gameObject);
        TextMeshProUGUI t=b.GetComponentInChildren<TextMeshProUGUI>(true);if(t!=null){R(t.transform,new Vector2(38,0),new Vector2(((RectTransform)b.transform).sizeDelta.x-125,112));ST(t,fs,Ink,TextAlignmentOptions.MidlineLeft);}
        Del(b.transform,"LetterBadge");Image badge=NI("LetterBadge",b.transform,G,accent,new Vector2(-((RectTransform)b.transform).sizeDelta.x*.5f+53,0),new Vector2(82,82),false);badge.type=Image.Type.Sliced;OL(badge.gameObject,Ink,2);
        NT("Letter",badge.transform,((char)('A'+i)).ToString(),42,Color.white,TextAlignmentOptions.Center,Vector2.zero,new Vector2(76,76));
    }

    static void Action(Button b,Color col,string label,float fs)
    {
        if(b==null)return;Image im=b.GetComponent<Image>()??b.gameObject.AddComponent<Image>();im.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(G);im.type=Image.Type.Sliced;im.color=Color.white;b.targetGraphic=im;
        BC(b,col,Lighter(col,.14f),Darker(col,.12f),Color.gray);OL(b.gameObject,Ink,3);SH(b.gameObject);FB(b.gameObject);
        TextMeshProUGUI t=b.GetComponentInChildren<TextMeshProUGUI>(true);if(t==null)t=NT("Label",b.transform,label,fs,Color.white,TextAlignmentOptions.Center,Vector2.zero,((RectTransform)b.transform).sizeDelta-new Vector2(24,16));
        ST(t,fs,Color.white,TextAlignmentOptions.Center);t.text=label;
    }

    static void Panel(GameObject go,Color c,Color edge,float w){Image i=go.GetComponent<Image>()??go.AddComponent<Image>();i.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(W);i.type=Image.Type.Sliced;i.color=c;i.raycastTarget=false;OL(go,edge,w);SH(go);}
static void TextPill(GameObject go, Color c, Color edge)
    {
        RectTransform textRect = go.transform as RectTransform;
        if (textRect == null || go.transform.parent == null) return;
        string backdropName = go.name + "Backdrop";
        Del(go.transform.parent, backdropName);
        Image image = NI(backdropName, go.transform.parent, W, c, textRect.anchoredPosition, textRect.sizeDelta, false);
        image.type = Image.Type.Sliced;
        image.transform.SetSiblingIndex(go.transform.GetSiblingIndex());
        OL(image.gameObject, edge, 2);
    }
    static Image Pill(string n,Transform p,Vector2 pos,Vector2 size,Color bg,Color tc,string txt,float fs){Image i=NI(n,p,W,bg,pos,size,false);i.type=Image.Type.Sliced;OL(i.gameObject,Ink,2);SH(i.gameObject);NT("Label",i.transform,txt,fs,tc,TextAlignmentOptions.Center,Vector2.zero,size-new Vector2(26,10));return i;}
    static Image NI(string n,Transform p,string path,Color c,Vector2 pos,Vector2 size,bool ray){GameObject go=new GameObject(n,typeof(RectTransform),typeof(CanvasRenderer),typeof(Image));go.transform.SetParent(p,false);R(go.transform,pos,size);Image i=go.GetComponent<Image>();i.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(path);i.color=c;i.raycastTarget=ray;return i;}
    static TextMeshProUGUI NT(string n,Transform p,string v,float fs,Color c,TextAlignmentOptions a,Vector2 pos,Vector2 size){GameObject go=new GameObject(n,typeof(RectTransform),typeof(CanvasRenderer),typeof(TextMeshProUGUI));go.transform.SetParent(p,false);R(go.transform,pos,size);TextMeshProUGUI t=go.GetComponent<TextMeshProUGUI>();t.text=v;ST(t,fs,c,a);return t;}
    static RectTransform NR(string n,Transform p){GameObject go=new GameObject(n,typeof(RectTransform));go.transform.SetParent(p,false);return go.GetComponent<RectTransform>();}
    static void ST(TextMeshProUGUI t,float fs,Color c,TextAlignmentOptions a){if(t==null)return;t.font=font;t.fontSize=fs;t.color=c;t.alignment=a;t.enableWordWrapping=true;t.overflowMode=TextOverflowModes.Ellipsis;t.raycastTarget=false;t.fontStyle=FontStyles.Normal;}
    static void TS(TextMeshProUGUI t,Color c,Vector2 d){Shadow s=t.GetComponent<Shadow>()??t.gameObject.AddComponent<Shadow>();s.effectColor=c;s.effectDistance=d;s.useGraphicAlpha=true;}
    static void OL(GameObject go,Color c,float w){Outline o=go.GetComponent<Outline>()??go.AddComponent<Outline>();o.effectColor=c;o.effectDistance=new Vector2(w,-w);o.useGraphicAlpha=true;}
    static void SH(GameObject go){Shadow s=go.GetComponents<Shadow>().FirstOrDefault(x=>!(x is Outline));if(s==null)s=go.AddComponent<Shadow>();s.effectColor=new Color(.18f,.12f,.03f,.25f);s.effectDistance=new Vector2(0,-7);s.useGraphicAlpha=true;}
    static void FB(GameObject go){if(go.GetComponent<ButtonInteractionFeedback>()==null)go.AddComponent<ButtonInteractionFeedback>();}
    static void BC(Button b,Color n,Color h,Color p,Color d){ColorBlock x=b.colors;x.normalColor=n;x.highlightedColor=h;x.selectedColor=h;x.pressedColor=p;x.disabledColor=d;x.colorMultiplier=1;x.fadeDuration=.12f;b.colors=x;b.transition=Selectable.Transition.ColorTint;}
    static void NoLayout(Transform t){if(t==null)return;LayoutGroup g=t.GetComponent<LayoutGroup>();if(g!=null)UnityEngine.Object.DestroyImmediate(g);ContentSizeFitter f=t.GetComponent<ContentSizeFitter>();if(f!=null)UnityEngine.Object.DestroyImmediate(f);}
    static void FadeLast(Transform c){Transform f=c.Find("FadePanel");if(f==null){Transform q=c.Find("QuizPanel");if(q!=null)f=q.Find("FadePanel");}if(f!=null)f.SetAsLastSibling();}
    static void Del(Transform p,string n){Transform x=p.Find(n);if(x!=null)UnityEngine.Object.DestroyImmediate(x.gameObject);}
    static void R(Transform t,Vector2 pos,Vector2 size){if(t==null)return;RectTransform r=t as RectTransform;if(r==null)return;r.anchorMin=r.anchorMax=r.pivot=new Vector2(.5f,.5f);r.anchoredPosition=pos;r.sizeDelta=size;r.localScale=Vector3.one;r.localRotation=Quaternion.identity;}
    static void Full(RectTransform r){r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.pivot=new Vector2(.5f,.5f);r.anchoredPosition=Vector2.zero;r.sizeDelta=Vector2.zero;r.localScale=Vector3.one;}
    static Color Lighter(Color c,float a){return new Color(Mathf.Lerp(c.r,1,a),Mathf.Lerp(c.g,1,a),Mathf.Lerp(c.b,1,a),c.a);}
    static Color Darker(Color c,float a){return new Color(c.r*(1-a),c.g*(1-a),c.b*(1-a),c.a);}
}
