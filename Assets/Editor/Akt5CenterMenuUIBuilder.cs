using System;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Akt5CenterMenuUIBuilder
{
    const string ScenePath="Assets/Scenes/Aktivity/akt5/akt5.unity";
    const string BgPath="Assets/Images/Akt5CenterUI/Background/akt5_factor_background_v2.png";
    const string HeroPath="Assets/Images/Akt5CenterUI/Hero/akt5_students_science_hero.png";
    const string White="Assets/Images/UI Flat/btn_white_flat.png";
    const string GreenFlat="Assets/Images/UI Flat/btn_green_flat.png";
    const string Circle="Assets/Images/UI Flat/ui_circle_white.png";
    const string ArrowLeft="Assets/Images/UI Flat/icon_arrow_left.png";
    const string FontPath="Assets/Fonts/FredokaOne SDF.asset";

    static readonly Color Ink=new Color32(8,74,31,255);
    static readonly Color Cream=new Color32(255,249,223,255);
    static readonly Color Green=new Color32(74,181,18,255);
    static readonly Color DarkGreen=new Color32(17,111,15,255);
    static readonly Color Purple=new Color32(153,75,220,255);
    static readonly Color Blue=new Color32(37,145,231,255);
    static readonly Color Teal=new Color32(18,184,183,255);
    static readonly Color Orange=new Color32(250,156,13,255);
    static readonly Color Pink=new Color32(240,72,126,255);
    static TMP_FontAsset font;

    [MenuItem("Tools/MicroFun/Activity 5/Build Center Reference Theme")]
    public static void Build()
    {
        if(EditorApplication.isPlayingOrWillChangePlaymode){Debug.LogError("[Akt5Center] Exit Play Mode first.");return;}
        font=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        if(font==null){Debug.LogError("[Akt5Center] Fredoka One font missing.");return;}

        string original=SceneManager.GetActiveScene().path;
        Backup();
        try
        {
            Scene scene=EditorSceneManager.OpenScene(ScenePath,OpenSceneMode.Single);
            Transform canvas=GameObject.Find("Canvas").transform;
            Transform center=canvas.Find("Center");
            Transform group=center==null?null:center.Find("GameObject");
            if(center==null||group==null)throw new InvalidOperationException("Expected Canvas/Center/GameObject hierarchy not found.");

            ConfigureCanvas(canvas.GetComponent<Canvas>());
            BuildCenter(center,group);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[Akt5Center] Reference-style Activity 5 center built.");
        }
        finally
        {
            if(!string.IsNullOrEmpty(original)&&original!=ScenePath)EditorSceneManager.OpenScene(original,OpenSceneMode.Single);
            AssetDatabase.SaveAssets();AssetDatabase.Refresh();
        }
    }

static void Backup()
    {
        const string parent="Assets/Scenes/Aktivity/akt5";
        const string dir=parent+"/Backups";
        if(!AssetDatabase.IsValidFolder(dir))AssetDatabase.CreateFolder(parent,"Backups");
        string dst=dir+"/akt5_PreFactorMenuCorrection_20260902.unity";
        if(AssetDatabase.LoadAssetAtPath<SceneAsset>(dst)==null)AssetDatabase.CopyAsset(ScenePath,dst);
    }

    static void ConfigureCanvas(Canvas canvas)
    {
        canvas.renderMode=RenderMode.ScreenSpaceOverlay;
        CanvasScaler s=canvas.GetComponent<CanvasScaler>()??canvas.gameObject.AddComponent<CanvasScaler>();
        s.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;
        s.referenceResolution=new Vector2(1920,1080);
        s.screenMatchMode=CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        s.matchWidthOrHeight=.5f;
    }

static void BuildCenter(Transform center,Transform group)
    {
        R(center,Vector2.zero,new Vector2(1920,1080));
        Full((RectTransform)center);

        Del(center,"Akt5CenterThemeRoot");
        RectTransform root=NR("Akt5CenterThemeRoot",center);Full(root);root.SetAsFirstSibling();

        Image bg=NI("Background",root,BgPath,Color.white,Vector2.zero,new Vector2(1920,1080),false);
        bg.type=Image.Type.Simple;bg.preserveAspect=false;Full(bg.rectTransform);

        BuildHeader(root);

        Transform oldTitle=center.Find("Title");if(oldTitle!=null)oldTitle.gameObject.SetActive(false);
        NoLayout(group);Full((RectTransform)group);

        Transform broken=group.Find("Button");if(broken!=null)broken.gameObject.SetActive(false);
        Transform[] buttons={group.Find("Button (1)"),group.Find("Button (2)"),group.Find("Button (3)"),group.Find("Button (4)")};
        string[] labels={"PENGENALAN","FAKTOR","MICRO-SCENE","KUIZ"};
        string[] icons={"Assets/Images/UI Flat/icon_book_open.png","Assets/Images/factors.png","Assets/Images/UI Flat/icon_microscope_card.png","Assets/Images/UI Flat/icon_quiz_checklist.png"};
        Color[] colors={Green,Orange,Blue,Pink};
        Vector2[] pos={new Vector2(0,100),new Vector2(0,-35),new Vector2(0,-170),new Vector2(0,-305)};

        for(int i=0;i<buttons.Length;i++)
        {
            if(buttons[i]==null)throw new InvalidOperationException("Activity 5 center button "+i+" missing.");
            buttons[i].gameObject.SetActive(true);
            StyleMenuButton(buttons[i].GetComponent<Button>(),i+1,labels[i],icons[i],colors[i],pos[i]);
        }

        Transform home=group.Find("HomeButton");
        if(home!=null)StyleHome(home.GetComponent<Button>());
    }

static void BuildHeader(Transform root)
    {
        Image activityBadge=NI("ActivityBadge",root,White,new Color32(210,242,151,255),new Vector2(0,472),new Vector2(420,82),false);
        activityBadge.type=Image.Type.Sliced;OL(activityBadge.gameObject,Color.white,4);SH(activityBadge.gameObject);
        TextMeshProUGUI activity=NT("Label",activityBadge.transform,"AKTIVITI 5",48,Ink,TextAlignmentOptions.Center,Vector2.zero,new Vector2(390,72));
        TS(activity,new Color(1,1,1,.75f),new Vector2(2,-2));

        TextMeshProUGUI title=NT("MainTitle",root,"FAKTOR PERTUMBUHAN MIKROORGANISMA",78,DarkGreen,TextAlignmentOptions.Center,new Vector2(0,365),new Vector2(1700,135));
        OL(title.gameObject,Color.white,5);TS(title,new Color(.15f,.1f,0,.34f),new Vector2(6,-8));

        TextMeshProUGUI subtitle=NT("SubtitleLabel",root,"Terokai bagaimana suhu, nutrien dan keasidan mempengaruhi pembiakan mikroorganisma!",29,Ink,TextAlignmentOptions.Center,new Vector2(0,280),new Vector2(1450,60));
        TS(subtitle,new Color(1,1,1,.7f),new Vector2(2,-2));

        Image curriculum=NI("CurriculumBadge",root,White,new Color(1,.97f,.84f,.97f),new Vector2(0,-453),new Vector2(390,92),false);
        curriculum.type=Image.Type.Sliced;OL(curriculum.gameObject,Orange,3);SH(curriculum.gameObject);
        NT("Label",curriculum.transform,"SP: 3.1\nSK: 3.1.4",29,Ink,TextAlignmentOptions.Center,Vector2.zero,new Vector2(350,82));
    }

    static void BuildHero(Transform root)
    {
        Image outer=NI("HeroOuterRing",root,Circle,Color.white,new Vector2(0,-105),new Vector2(680,680),false);outer.preserveAspect=true;SH(outer.gameObject);
        Image ring1=NI("HeroGreenRing",outer.transform,Circle,Green,Vector2.zero,new Vector2(650,650),false);ring1.preserveAspect=true;
        Image ring2=NI("HeroCreamRing",outer.transform,Circle,Cream,Vector2.zero,new Vector2(618,618),false);ring2.preserveAspect=true;
        Image hero=NI("StudentsHero",outer.transform,HeroPath,Color.white,new Vector2(0,-10),new Vector2(575,575),false);hero.preserveAspect=true;

        Color[] nodeColors={Purple,Blue,Teal,Orange};
        Vector2[] nodes={new Vector2(-305,135),new Vector2(-315,-145),new Vector2(305,135),new Vector2(315,-145)};
        for(int i=0;i<nodes.Length;i++)
        {
            Image node=NI("RouteNode"+(i+1),outer.transform,Circle,nodeColors[i],nodes[i],new Vector2(62,62),false);
            node.preserveAspect=true;OL(node.gameObject,Color.white,3);SH(node.gameObject);
        }
    }

    static void BuildConnectors(Transform root)
    {
        Connector(root,"LeftTop",new Vector2(-350,30),new Vector2(180,13),Purple);
        Connector(root,"LeftBottom",new Vector2(-350,-205),new Vector2(170,13),Blue);
        Connector(root,"RightTop",new Vector2(350,30),new Vector2(180,13),Teal);
        Connector(root,"RightBottom",new Vector2(350,-205),new Vector2(170,13),Orange);
    }

    static void Connector(Transform root,string name,Vector2 pos,Vector2 size,Color color)
    {
        Image line=NI(name,root,White,color,pos,size,false);line.type=Image.Type.Sliced;
    }

static void StyleMenuButton(Button button,int number,string label,string iconPath,Color color,Vector2 pos)
    {
        Transform t=button.transform;R(t,pos,new Vector2(820,118));
        Image im=button.GetComponent<Image>()??button.gameObject.AddComponent<Image>();
        im.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(White);im.type=Image.Type.Sliced;im.color=Color.white;button.targetGraphic=im;
        BC(button,color,Lighter(color,.14f),Darker(color,.12f),Color.gray);OL(button.gameObject,Color.white,4);SH(button.gameObject);FB(button.gameObject);

        foreach(Transform ch in t)if(ch.name=="Image")ch.gameObject.SetActive(false);
        while(t.Find("Akt5NumberBadge")!=null)UnityEngine.Object.DestroyImmediate(t.Find("Akt5NumberBadge").gameObject);
        while(t.Find("Akt5Icon")!=null)UnityEngine.Object.DestroyImmediate(t.Find("Akt5Icon").gameObject);
        while(t.Find("Akt5Arrow")!=null)UnityEngine.Object.DestroyImmediate(t.Find("Akt5Arrow").gameObject);
        while(t.Find("Akt5Gloss")!=null)UnityEngine.Object.DestroyImmediate(t.Find("Akt5Gloss").gameObject);

        Image iconBadge=NI("Akt5NumberBadge",t,Circle,Darker(color,.06f),new Vector2(-340,0),new Vector2(112,112),false);
        iconBadge.preserveAspect=true;OL(iconBadge.gameObject,Color.white,4);SH(iconBadge.gameObject);
        Image icon=NI("Akt5Icon",iconBadge.transform,iconPath,Color.white,Vector2.zero,new Vector2(88,88),false);
        icon.preserveAspect=true;

        TextMeshProUGUI text=button.GetComponentInChildren<TextMeshProUGUI>(true);
        text.text=label;R(text.transform,new Vector2(0,0),new Vector2(560,104));ST(text,48,Color.white,TextAlignmentOptions.Center);
        OL(text.gameObject,new Color(0,0,0,.18f),2);TS(text,new Color(0,0,0,.30f),new Vector2(3,-4));

        Image arrowBadge=NI("Akt5Arrow",t,Circle,new Color32(255,185,22,255),new Vector2(340,0),new Vector2(82,82),false);
        arrowBadge.preserveAspect=true;OL(arrowBadge.gameObject,Color.white,3);SH(arrowBadge.gameObject);
        Image arrow=NI("Arrow",arrowBadge.transform,ArrowLeft,Color.white,Vector2.zero,new Vector2(40,40),false);
        arrow.preserveAspect=true;arrow.rectTransform.localRotation=Quaternion.Euler(0,0,180);

        Image gloss=NI("Akt5Gloss",t,White,new Color(1,1,1,.14f),new Vector2(0,42),new Vector2(735,12),false);
        gloss.type=Image.Type.Sliced;
    }

static void StyleHome(Button button)
    {
        Transform t=button.transform;R(t,new Vector2(-835,485),new Vector2(220,68));
        while(t.Find("BackCircle")!=null)UnityEngine.Object.DestroyImmediate(t.Find("BackCircle").gameObject);
        while(t.Find("Label")!=null)UnityEngine.Object.DestroyImmediate(t.Find("Label").gameObject);
        Image im=button.GetComponent<Image>()??button.gameObject.AddComponent<Image>();
        im.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(White);im.type=Image.Type.Sliced;im.color=Color.white;button.targetGraphic=im;
        BC(button,Cream,new Color(1,.94f,.65f),new Color(.9f,.98f,.8f),Color.gray);OL(button.gameObject,Ink,2);SH(button.gameObject);FB(button.gameObject);
        foreach(Transform ch in t)ch.gameObject.SetActive(false);
        Image circle=NI("BackCircle",t,Circle,Green,new Vector2(-76,0),new Vector2(60,60),false);circle.preserveAspect=true;OL(circle.gameObject,DarkGreen,2);
        Image arrow=NI("Arrow",circle.transform,ArrowLeft,Color.white,Vector2.zero,new Vector2(32,32),false);arrow.preserveAspect=true;
        NT("Label",t,"AKTIVITI",23,Ink,TextAlignmentOptions.MidlineLeft,new Vector2(34,0),new Vector2(122,56));
    }

    static Image NI(string n,Transform p,string path,Color c,Vector2 pos,Vector2 size,bool ray)
    {
        GameObject go=new GameObject(n,typeof(RectTransform),typeof(CanvasRenderer),typeof(Image));go.transform.SetParent(p,false);R(go.transform,pos,size);
        Image i=go.GetComponent<Image>();i.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(path);i.color=c;i.raycastTarget=ray;return i;
    }

    static TextMeshProUGUI NT(string n,Transform p,string value,float fs,Color c,TextAlignmentOptions a,Vector2 pos,Vector2 size)
    {
        GameObject go=new GameObject(n,typeof(RectTransform),typeof(CanvasRenderer),typeof(TextMeshProUGUI));go.transform.SetParent(p,false);R(go.transform,pos,size);
        TextMeshProUGUI t=go.GetComponent<TextMeshProUGUI>();t.text=value;ST(t,fs,c,a);return t;
    }

    static RectTransform NR(string n,Transform p){GameObject go=new GameObject(n,typeof(RectTransform));go.transform.SetParent(p,false);return go.GetComponent<RectTransform>();}
    static void ST(TextMeshProUGUI t,float fs,Color c,TextAlignmentOptions a){if(t==null)return;t.font=font;t.fontSize=fs;t.color=c;t.alignment=a;t.enableWordWrapping=true;t.overflowMode=TextOverflowModes.Ellipsis;t.raycastTarget=false;}
    static void TS(TextMeshProUGUI t,Color c,Vector2 d){Shadow s=t.GetComponents<Shadow>().FirstOrDefault(x=>!(x is Outline));if(s==null)s=t.gameObject.AddComponent<Shadow>();s.effectColor=c;s.effectDistance=d;s.useGraphicAlpha=true;}
    static void OL(GameObject go,Color c,float w){Outline o=go.GetComponent<Outline>()??go.AddComponent<Outline>();o.effectColor=c;o.effectDistance=new Vector2(w,-w);o.useGraphicAlpha=true;}
    static void SH(GameObject go){Shadow s=go.GetComponents<Shadow>().FirstOrDefault(x=>!(x is Outline));if(s==null)s=go.AddComponent<Shadow>();s.effectColor=new Color(.16f,.1f,.02f,.28f);s.effectDistance=new Vector2(0,-8);s.useGraphicAlpha=true;}
    static void FB(GameObject go){if(go.GetComponent<ButtonInteractionFeedback>()==null)go.AddComponent<ButtonInteractionFeedback>();}
    static void BC(Button b,Color n,Color h,Color p,Color d){ColorBlock x=b.colors;x.normalColor=n;x.highlightedColor=h;x.selectedColor=h;x.pressedColor=p;x.disabledColor=d;x.colorMultiplier=1;x.fadeDuration=.12f;b.colors=x;b.transition=Selectable.Transition.ColorTint;}
    static Color Lighter(Color c,float a){return new Color(Mathf.Lerp(c.r,1,a),Mathf.Lerp(c.g,1,a),Mathf.Lerp(c.b,1,a),c.a);}
    static Color Darker(Color c,float a){return new Color(c.r*(1-a),c.g*(1-a),c.b*(1-a),c.a);}
    static void NoLayout(Transform t){LayoutGroup g=t.GetComponent<LayoutGroup>();if(g!=null)UnityEngine.Object.DestroyImmediate(g);ContentSizeFitter f=t.GetComponent<ContentSizeFitter>();if(f!=null)UnityEngine.Object.DestroyImmediate(f);}
    static void Del(Transform p,string n){Transform x=p.Find(n);if(x!=null)UnityEngine.Object.DestroyImmediate(x.gameObject);}
    static void R(Transform t,Vector2 pos,Vector2 size){if(t==null)return;RectTransform r=t as RectTransform;if(r==null)return;r.anchorMin=r.anchorMax=r.pivot=new Vector2(.5f,.5f);r.anchoredPosition=pos;r.sizeDelta=size;r.localScale=Vector3.one;r.localRotation=Quaternion.identity;}
    static void Full(RectTransform r){r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.pivot=new Vector2(.5f,.5f);r.anchoredPosition=Vector2.zero;r.sizeDelta=Vector2.zero;r.localScale=Vector3.one;}
}
