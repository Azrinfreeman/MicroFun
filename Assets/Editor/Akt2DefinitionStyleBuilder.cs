#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class Akt2DefinitionStyleBuilder
{
    static Sprite rounded;
    static TMP_FontAsset head,body;
    static Color green=ColorHex("#104A25");
    static Color ColorHex(string s){Color c;ColorUtility.TryParseHtmlString(s,out c);return c;}
    static RectTransform Rect(Transform t,float x,float y,float w,float h){
        var r=(RectTransform)t;r.anchorMin=r.anchorMax=r.pivot=new Vector2(.5f,.5f);
        r.anchoredPosition=new Vector2(x,y);r.sizeDelta=new Vector2(w,h);r.localScale=Vector3.one;r.localRotation=Quaternion.identity;return r;
    }
    static Transform Need(Transform p,string path){var t=p.Find(path);if(!t)throw new Exception("Missing "+path);return t;}
    static RectTransform Node(Transform p,string n,float x,float y,float w,float h){
        var t=p.Find(n);if(!t){t=new GameObject(n,typeof(RectTransform)).transform;t.SetParent(p,false);}return Rect(t,x,y,w,h);
    }
    static Image Panel(Transform t,Color color,bool raycast=false){
        var i=t.GetComponent<Image>();if(!i)i=t.gameObject.AddComponent<Image>();
        i.sprite=rounded;i.type=Image.Type.Sliced;i.pixelsPerUnitMultiplier=.5f;i.color=color;i.raycastTarget=raycast;i.preserveAspect=false;return i;
    }
    static void Border(Graphic g,Color color,float width){
        var o=g.GetComponent<Outline>();if(!o)o=g.gameObject.AddComponent<Outline>();o.effectColor=color;o.effectDistance=new Vector2(width,-width);
    }
    static TMP_Text Text(Transform t,string value,float size,bool title=false){
        var tx=t.GetComponent<TMP_Text>();if(!tx)tx=t.gameObject.AddComponent<TextMeshProUGUI>();
        tx.font=title?head:body;tx.fontStyle=title?FontStyles.Normal:FontStyles.Bold;tx.fontSize=size;tx.enableAutoSizing=false;
        tx.color=green;tx.text=value;tx.alignment=TextAlignmentOptions.Center;tx.margin=Vector4.zero;tx.raycastTarget=false;tx.overflowMode=TextOverflowModes.Overflow;return tx;
    }
    static void Icon(Transform p,string n,string file,float x,float y,float w,float h){
        var r=Node(p,n,x,y,w,h);var i=r.GetComponent<Image>();if(!i)i=r.gameObject.AddComponent<Image>();
        i.sprite=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Images/UI Flat/"+file);if(!i.sprite)throw new Exception("Missing "+file);
        i.type=Image.Type.Simple;i.color=Color.white;i.raycastTarget=false;i.preserveAspect=true;
    }
    static void Button(Transform t,float x,float y,float w,float h,Color color){
        Rect(t,x,y,w,h);var i=Panel(t,color,true);Border(i,Color.white,3);
        var b=t.GetComponent<Button>();b.targetGraphic=i;b.transition=Selectable.Transition.ColorTint;
        var c=b.colors;c.normalColor=c.selectedColor=Color.white;c.highlightedColor=new Color(1,.95f,.74f);c.pressedColor=new Color(.8f,.87f,.8f);b.colors=c;
    }
    public static string Apply(){
        var scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if(EditorApplication.isPlaying||scene.path!="Assets/Scenes/Aktivity/akt2/akt2.unity")throw new Exception("AKT2 Edit Mode required.");
        var d=GameObject.Find("Canvas").transform.Find("Definisi");if(!d)throw new Exception("Definition missing");
        foreach(var path in new[]{"Title/Text","Top/DialogButton","Bottom/HomeButton","Bottom/NextButton","DialogParent/textDialog","DialogParent/ExitButton"})Need(d,path);
        rounded=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Images/Akt4KehadiranAirUI/air_rounded.asset");
        head=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/FredokaOne SDF.asset");
        body=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");
        if(!rounded||!head||!body)throw new Exception("Missing style resources");
        if(!EditorSceneManager.SaveScene(scene,"Assets/Scenes/Aktivity/akt2/akt2_before_definition_apply_"+DateTime.Now.ToString("yyyyMMdd_HHmmss")+".unity",true))throw new Exception("Backup failed");
        Undo.RegisterFullObjectHierarchyUndo(d.gameObject,"Definition reference style");
        Rect(d,0,0,1920,1080);
        var bg=Node(d,"Style_Background",0,0,1920,1080);var image=Panel(bg,Color.white);
        image.sprite=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Images/Akt4KehadiranAirUI/air_warm_gradient.asset");image.type=Image.Type.Simple;bg.SetAsFirstSibling();
        var fit=d.GetComponent<Akt2DefinitionFit>();if(!fit)fit=d.gameObject.AddComponent<Akt2DefinitionFit>();fit.background=bg;
        var content=Need(d,"Content");Rect(content,0,0,1920,1080);
        // Existing lesson art is superseded, but the objects remain recoverable.
        foreach(var g in new[]{Need(content,"RawImage").GetComponent<Graphic>(),Need(content,"Image").GetComponent<Graphic>()})g.enabled=false;
        var owned=Node(content,"Style_Definition",0,0,1920,1080);
        string[] copy={"Mikroorganisma ialah hidupan seni yang sangat kecil.","Kebanyakannya tidak dapat dilihat dengan mata kasar.","Mikroorganisma boleh diperhatikan menggunakan mikroskop."};
        string[] icons={"char_green_microbe.png","icon_eye_magnify.png","img_microscope_large.png"};
        string[] colours={"#E3F0BB","#FFF0BD","#D9EEF1"};
        for(int k=0;k<3;k++){
            var card=Node(owned,"Card"+(k+1),-465,240-k*240,875,213);Border(Panel(card,ColorHex(colours[k])),Color.white,4);
            var plate=Node(card,"IconFrame",-322,0,168,174);Border(Panel(plate,ColorHex("#FFFCEB")),Color.white,2);
            Icon(plate,"Icon",icons[k],0,0,150,155);
            var text=Text(Node(card,"Definition",97,0,578,184),copy[k],42);
            text.alignment=TextAlignmentOptions.MidlineLeft;
        }
        Icon(owned,"MagnifiedMicrobes","img_microbe_circle.png",605,183,576,576);
        Icon(owned,"Microscope","img_microscope_large.png",344,-133,480,637);
        Text(Node(owned,"IllustrationCaption",660,-185,450,44),"Ilustrasi mikroorganisma",24);
        for(int k=0;k<7;k++){
            var bubble=Node(owned,"Bubble"+k,(k<3?-1:1)*(914-(k%2)*12),330-(k%4)*176,22+k%3*12,22+k%3*12);
            var bi=Panel(bubble,Color.white);bi.sprite=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Images/Akt4KehadiranUdaraUI/bubble.asset");bi.type=Image.Type.Simple;bi.preserveAspect=true;
        }
        var title=Need(d,"Title");Rect(title,0,448,590,112);Border(Panel(title,ColorHex("#FFF9DB")),Color.white,4);
        Rect(Need(title,"Text"),0,0,558,108);Text(Need(title,"Text"),"DEFINISI",72,true);
        var top=Need(d,"Top");Rect(top,0,0,1920,1080);
        var read=Need(top,"DialogButton");Button(read,-671,452,447,81,ColorHex("#F1F7DA"));
        Icon(read,"Image","icon_book_open.png",-174,0,66,64);
        Text(Node(read,"Style_Label",42,0,338,72),"Baca penerangan",30,true);
        var bottom=Need(d,"Bottom");Rect(bottom,0,0,1920,1080);
        var home=Need(bottom,"HomeButton");Button(home,-810,-462,154,100,ColorHex("#F7AF18"));Icon(home,"Image","icon_home.png",0,0,78,78);
        var next=Need(bottom,"NextButton");Button(next,810,-462,154,100,ColorHex("#4B9B19"));Icon(next,"Image","icon_arrow_left.png",0,0,71,71);
        next.Find("Image").localRotation=Quaternion.Euler(0,0,180);
        var dialog=Need(d,"DialogParent");Rect(dialog,0,0,1920,1080);dialog.SetAsLastSibling();
        fit.modalShield=(RectTransform)dialog;
        // Modal shield blocks click-through, not the dialogue's own controls.
        var shield=Panel(dialog,new Color(0,.12f,.07f,.55f),true);shield.sprite=null;shield.type=Image.Type.Simple;
        var cardBg=Need(dialog,"bg");Rect(cardBg,0,-30,1420,500);Border(Panel(cardBg,ColorHex("#FFFAE0"),true),Color.white,4);
        Need(dialog,"Person").GetComponent<Image>().enabled=false;
        Text(Node(dialog,"Style_Heading",0,150,1220,84),"PENERANGAN",45,true);
        var textDialog=Need(dialog,"textDialog");Rect(textDialog,0,-15,1210,220);Text(textDialog,copy[0]+" "+copy[1],42);
        var internalNext=textDialog.GetChild(0);Button(internalNext,0,-195,328,76,ColorHex("#26743B"));
        var nextLabel=internalNext.GetComponentInChildren<TMP_Text>(true);nextLabel.gameObject.SetActive(true);Rect(nextLabel.transform,0,0,308,72);Text(nextLabel.transform,"SETERUSNYA",29,true).color=Color.white;
        var exit=Need(dialog,"ExitButton");Button(exit,0,-218,290,76,ColorHex("#26743B"));
        var exitLabel=exit.GetComponentInChildren<TMP_Text>(true);Rect(exitLabel.transform,0,0,270,72);Text(exitLabel.transform,"TUTUP",29,true).color=Color.white;
        var controller=dialog.GetComponent<DialogController>();
        controller.dialogList=new List<string>{copy[0]+" "+copy[1],copy[2]};
        Canvas.ForceUpdateCanvases();EditorSceneManager.MarkSceneDirty(scene);
        if(!EditorSceneManager.SaveScene(scene))throw new Exception("Save failed");
        return "Definition styled and saved; current dialogue visibility and original callbacks preserved.";
    }
}
#endif
