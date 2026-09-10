using System;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Akt5PengenalanUIBuilder
{
    const string ScenePath="Assets/Scenes/Aktivity/akt5/akt5.unity";
    const string BgPath="Assets/Images/Akt5CenterUI/Background/akt5_factor_background_v2.png";
    const string PeoplePath="Assets/Images/Akt5PengenalanUI/akt5_pengenalan_students_soup_v2.png";
    const string White="Assets/Images/UI Flat/btn_white_flat.png";
    const string Circle="Assets/Images/UI Flat/ui_circle_white.png";
    const string Book="Assets/Images/UI Flat/icon_book_open.png";
    const string Arrow="Assets/Images/UI Flat/icon_arrow_left.png";
    const string Nutrition="Assets/Images/UI Flat/icon_fun_nutrition.png";
    const string Factors="Assets/Images/factors.png";
    const string Lightbulb="Assets/Images/UI Flat/icon_lightbulb.png";
    const string FontPath="Assets/Fonts/FredokaOne SDF.asset";

    static readonly Color Ink=new Color32(8,70,30,255);
    static readonly Color DarkGreen=new Color32(18,105,14,255);
    static readonly Color Green=new Color32(87,188,16,255);
    static readonly Color Blue=new Color32(43,148,224,255);
    static readonly Color Pink=new Color32(239,80,139,255);
    static readonly Color Orange=new Color32(248,139,24,255);
    static readonly Color Purple=new Color32(151,72,207,255);
    static readonly Color Cream=new Color32(255,250,229,255);
    static TMP_FontAsset font;

    [MenuItem("Tools/MicroFun/Activity 5/Build Pengenalan Discussion Theme")]
    public static void Build()
    {
        if(EditorApplication.isPlayingOrWillChangePlaymode){Debug.LogError("[Akt5Pengenalan] Exit Play Mode first.");return;}
        font=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        if(font==null){Debug.LogError("[Akt5Pengenalan] Fredoka One font missing.");return;}

        string original=SceneManager.GetActiveScene().path;
        Backup();
        try
        {
            Scene scene=EditorSceneManager.OpenScene(ScenePath,OpenSceneMode.Single);
            Transform canvas=GameObject.Find("Canvas").transform;
            Transform page=canvas.Find("Pengenalan");
            if(page==null)throw new InvalidOperationException("Canvas/Pengenalan not found.");

            ConfigureCanvas(canvas.GetComponent<Canvas>());
            BuildPage(page);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[Akt5Pengenalan] Discussion-style Pengenalan page built.");
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
        string dst=dir+"/akt5_PrePengenalanRedesign_20260902.unity";
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

    static void BuildPage(Transform page)
    {
        Del(page,"Akt5PengenalanThemeRoot");
        RectTransform root=NR("Akt5PengenalanThemeRoot",page);Full(root);root.SetAsFirstSibling();

        foreach(Transform child in page)
            if(child!=root && child.name!="Bottom")child.gameObject.SetActive(false);

        Image bg=NI("Background",root,BgPath,Color.white,Vector2.zero,new Vector2(1920,1080),false);
        bg.preserveAspect=false;Full(bg.rectTransform);

        Image people=NI("AliAinaSoup",root,PeoplePath,Color.white,new Vector2(0,-66),new Vector2(1920,1080),false);
        people.preserveAspect=true;

        BuildHeader(root);
        BuildDialogue(root);
        BuildFactors(root);
        BuildPrompt(root);

        Transform bottom=page.Find("Bottom");
        if(bottom==null)throw new InvalidOperationException("Pengenalan/Bottom navigation not found.");
        bottom.gameObject.SetActive(true);Full((RectTransform)bottom);
        Button next=bottom.Find("NextButton").GetComponent<Button>();
        Button home=bottom.Find("HomeButton").GetComponent<Button>();
        StyleNavigation(home,false);
        StyleNavigation(next,true);
        bottom.SetAsLastSibling();
    }

    static void BuildHeader(Transform root)
    {
        Image badge=Panel("ActivityBadge",root,new Vector2(0,485),new Vector2(360,70),new Color32(214,244,165,255),Green,3);
        NT("Label",badge.transform,"AKTIVITI 5",42,Ink,TextAlignmentOptions.Center,Vector2.zero,new Vector2(330,62));

        Image title=Panel("TitleBar",root,new Vector2(0,402),new Vector2(770,98),Green,DarkGreen,3);
        Image iconBadge=NI("BookBadge",title.transform,Circle,new Color32(76,169,13,255),new Vector2(-327,0),new Vector2(112,112),false);
        iconBadge.preserveAspect=true;OL(iconBadge.gameObject,Color.white,4);SH(iconBadge.gameObject);
        Image book=NI("Book",iconBadge.transform,Book,Color.white,Vector2.zero,new Vector2(84,84),false);book.preserveAspect=true;
        TextMeshProUGUI titleText=NT("Label",title.transform,"PENGENALAN",53,Color.white,TextAlignmentOptions.Center,new Vector2(38,0),new Vector2(610,86));
        TS(titleText,new Color(0,0,0,.28f),new Vector2(3,-4));

        Image q=Panel("MainQuestion",root,new Vector2(0,292),new Vector2(950,142),new Color(1,.97f,.84f,.96f),Orange,3);
        TextMeshProUGUI qt=NT("Label",q.transform,"Apakah yang menyebabkan\nsup ini cepat basi?",48,Ink,TextAlignmentOptions.Center,Vector2.zero,new Vector2(900,128));
        OL(qt.gameObject,Color.white,3);TS(qt,new Color(.2f,.1f,0,.25f),new Vector2(4,-5));
    }

    static void BuildDialogue(Transform root)
    {
        Speech(root,"AliTop",new Vector2(-445,92),new Vector2(520,150),Blue,
            "<color=#1671BD>Ali:</color>\nEh, semalam sup ini masih elok,\ntapi hari ini sudah <color=#D62828>berbau masam</color>. Kenapa ya?");
        Speech(root,"AinaTop",new Vector2(445,92),new Vector2(520,150),Pink,
            "<color=#D93478>Aina:</color>\nMungkin sebab sup ini dibiarkan di dapur semalaman.\n<color=#D62828>Suhu</color> dapur panas berbanding peti sejuk.");
        Speech(root,"AliBottom",new Vector2(-445,-64),new Vector2(520,140),Blue,
            "<color=#1671BD>Ali:</color>\nTapi sup ini ada banyak <color=#287C36>isi ayam dan sayur</color>.\nAdakah itu memberi kesan?");
        Speech(root,"AinaBottom",new Vector2(445,-64),new Vector2(520,140),Pink,
            "<color=#D93478>Aina:</color>\nHmm... rasa sup ini juga sudah masam.\nMungkinkah <color=#6D2CCB>keasidan</color> berubah?");
    }

    static void Speech(Transform root,string name,Vector2 pos,Vector2 size,Color border,string value)
    {
        Image bubble=Panel(name,root,pos,size,new Color(1,.99f,.94f,.97f),border,3);
        TextMeshProUGUI text=NT("Text",bubble.transform,value,24,Ink,TextAlignmentOptions.TopLeft,new Vector2(8,-2),new Vector2(size.x-42,size.y-18));
        text.richText=true;text.fontStyle=FontStyles.Normal;
        Image avatar=NI("Avatar",bubble.transform,Circle,border,new Vector2(-size.x*.5f+18,size.y*.5f-18),new Vector2(52,52),false);
        avatar.preserveAspect=true;OL(avatar.gameObject,Color.white,2);
        NT("Person",avatar.transform,"●",24,Color.white,TextAlignmentOptions.Center,Vector2.zero,new Vector2(42,42));
    }

    static void BuildFactors(Transform root)
    {
        FactorChip(root,"Suhu",new Vector2(-270,-295),new Color32(247,96,64,255),Factors,"°C");
        FactorChip(root,"Nutrien",new Vector2(0,-295),new Color32(153,192,44,255),Nutrition,"");
        FactorChip(root,"Keasidan",new Vector2(270,-295),Purple,null,"pH");
    }

    static void FactorChip(Transform root,string label,Vector2 pos,Color color,string iconPath,string iconText)
    {
        Image chip=Panel(label+"Chip",root,pos,new Vector2(245,70),new Color(1,.98f,.88f,.97f),color,3);
        Image circle=NI("IconCircle",chip.transform,Circle,Lighter(color,.12f),new Vector2(-86,0),new Vector2(62,62),false);
        circle.preserveAspect=true;OL(circle.gameObject,Color.white,2);
        if(!string.IsNullOrEmpty(iconPath))
        {
            Image icon=NI("Icon",circle.transform,iconPath,Color.white,Vector2.zero,new Vector2(46,46),false);
            icon.preserveAspect=true;
        }
        if(!string.IsNullOrEmpty(iconText))NT("IconText",circle.transform,iconText,20,Color.white,TextAlignmentOptions.Center,Vector2.zero,new Vector2(48,48));
        NT("Label",chip.transform,label,27,color,TextAlignmentOptions.Center,new Vector2(25,0),new Vector2(160,58));
    }

    static void BuildPrompt(Transform root)
    {
        Image panel=Panel("TriggerQuestion",root,new Vector2(0,-433),new Vector2(1160,100),new Color(1,.98f,.85f,.97f),Orange,3);
        Image iconCircle=NI("IconCircle",panel.transform,Circle,new Color32(255,205,52,255),new Vector2(-520,0),new Vector2(76,76),false);
        iconCircle.preserveAspect=true;OL(iconCircle.gameObject,Color.white,2);
        Image bulb=NI("Lightbulb",iconCircle.transform,Lightbulb,Color.white,Vector2.zero,new Vector2(58,58),false);bulb.preserveAspect=true;
        TextMeshProUGUI text=NT("Text",panel.transform,
            "<color=#0A5D2A>Soalan pencetus:</color>\nFaktor manakah paling mempengaruhi <color=#197C35>pembiakan mikroorganisma</color> dalam sup ini?",
            25,Ink,TextAlignmentOptions.MidlineLeft,new Vector2(45,0),new Vector2(1000,86));
        text.richText=true;
    }

    static void StyleNavigation(Button button,bool next)
    {
        Transform t=button.transform;
        R(t,next?new Vector2(830,-474):new Vector2(-840,486),next?new Vector2(190,66):new Vector2(210,66));
        while(t.Find("PengenalanNavCircle")!=null)UnityEngine.Object.DestroyImmediate(t.Find("PengenalanNavCircle").gameObject);
        while(t.Find("PengenalanNavLabel")!=null)UnityEngine.Object.DestroyImmediate(t.Find("PengenalanNavLabel").gameObject);
        foreach(Transform child in t)child.gameObject.SetActive(false);

        Image image=button.GetComponent<Image>()??button.gameObject.AddComponent<Image>();
        image.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(White);image.type=Image.Type.Sliced;image.color=Color.white;button.targetGraphic=image;
        Color normal=next?new Color32(255,181,28,255):Cream;
        BC(button,normal,Lighter(normal,.12f),Darker(normal,.1f),Color.gray);OL(button.gameObject,next?Orange:Ink,2);SH(button.gameObject);FB(button.gameObject);

        Image circle=NI("PengenalanNavCircle",t,Circle,next?Orange:Green,new Vector2(next?65:-72,0),new Vector2(58,58),false);
        circle.preserveAspect=true;OL(circle.gameObject,Color.white,2);
        Image arrow=NI("Arrow",circle.transform,Arrow,Color.white,Vector2.zero,new Vector2(30,30),false);arrow.preserveAspect=true;
        if(next)arrow.rectTransform.localRotation=Quaternion.Euler(0,0,180);
        NT("PengenalanNavLabel",t,next?"FAKTOR":"AKTIVITI",22,next?Color.white:Ink,next?TextAlignmentOptions.MidlineLeft:TextAlignmentOptions.MidlineLeft,new Vector2(next?-26:30,0),new Vector2(116,54));
    }

    static Image Panel(string name,Transform parent,Vector2 pos,Vector2 size,Color fill,Color border,float width)
    {
        Image image=NI(name,parent,White,fill,pos,size,false);
        image.type=Image.Type.Sliced;OL(image.gameObject,border,width);SH(image.gameObject);return image;
    }

    static Image NI(string name,Transform parent,string path,Color color,Vector2 pos,Vector2 size,bool raycast)
    {
        GameObject go=new GameObject(name,typeof(RectTransform),typeof(CanvasRenderer),typeof(Image));go.transform.SetParent(parent,false);R(go.transform,pos,size);
        Image image=go.GetComponent<Image>();image.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(path);image.color=color;image.raycastTarget=raycast;return image;
    }

    static TextMeshProUGUI NT(string name,Transform parent,string value,float size,Color color,TextAlignmentOptions alignment,Vector2 pos,Vector2 rectSize)
    {
        GameObject go=new GameObject(name,typeof(RectTransform),typeof(CanvasRenderer),typeof(TextMeshProUGUI));go.transform.SetParent(parent,false);R(go.transform,pos,rectSize);
        TextMeshProUGUI text=go.GetComponent<TextMeshProUGUI>();text.font=font;text.text=value;text.fontSize=size;text.color=color;text.alignment=alignment;text.enableWordWrapping=true;text.overflowMode=TextOverflowModes.Ellipsis;text.raycastTarget=false;return text;
    }

    static RectTransform NR(string name,Transform parent){GameObject go=new GameObject(name,typeof(RectTransform));go.transform.SetParent(parent,false);return go.GetComponent<RectTransform>();}
    static void Del(Transform parent,string name){Transform found=parent.Find(name);if(found!=null)UnityEngine.Object.DestroyImmediate(found.gameObject);}
    static void R(Transform t,Vector2 pos,Vector2 size){RectTransform r=t as RectTransform;if(r==null)return;r.anchorMin=r.anchorMax=r.pivot=new Vector2(.5f,.5f);r.anchoredPosition=pos;r.sizeDelta=size;r.localScale=Vector3.one;r.localRotation=Quaternion.identity;}
    static void Full(RectTransform r){r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.pivot=new Vector2(.5f,.5f);r.anchoredPosition=Vector2.zero;r.sizeDelta=Vector2.zero;r.localScale=Vector3.one;}
    static void OL(GameObject go,Color color,float width){Outline o=go.GetComponent<Outline>()??go.AddComponent<Outline>();o.effectColor=color;o.effectDistance=new Vector2(width,-width);o.useGraphicAlpha=true;}
    static void SH(GameObject go){Shadow s=go.GetComponents<Shadow>().FirstOrDefault(x=>!(x is Outline));if(s==null)s=go.AddComponent<Shadow>();s.effectColor=new Color(.15f,.08f,0,.28f);s.effectDistance=new Vector2(0,-7);s.useGraphicAlpha=true;}
    static void TS(TextMeshProUGUI text,Color color,Vector2 distance){Shadow s=text.GetComponents<Shadow>().FirstOrDefault(x=>!(x is Outline));if(s==null)s=text.gameObject.AddComponent<Shadow>();s.effectColor=color;s.effectDistance=distance;s.useGraphicAlpha=true;}
    static void FB(GameObject go){if(go.GetComponent<ButtonInteractionFeedback>()==null)go.AddComponent<ButtonInteractionFeedback>();}
    static void BC(Button button,Color normal,Color highlighted,Color pressed,Color disabled){ColorBlock c=button.colors;c.normalColor=normal;c.highlightedColor=highlighted;c.selectedColor=highlighted;c.pressedColor=pressed;c.disabledColor=disabled;c.colorMultiplier=1;c.fadeDuration=.12f;button.colors=c;button.transition=Selectable.Transition.ColorTint;}
    static Color Lighter(Color c,float amount){return new Color(Mathf.Lerp(c.r,1,amount),Mathf.Lerp(c.g,1,amount),Mathf.Lerp(c.b,1,amount),c.a);}
    static Color Darker(Color c,float amount){return new Color(c.r*(1-amount),c.g*(1-amount),c.b*(1-amount),c.a);}
}
