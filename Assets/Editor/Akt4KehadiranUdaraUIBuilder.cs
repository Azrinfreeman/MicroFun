#if UNITY_EDITOR
using System;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class Akt4KehadiranUdaraUIBuilder
{
    const string ScenePath = "Assets/Scenes/Aktivity/akt4/akt4.unity";
    const string Dir = "Assets/Images/Akt4KehadiranUdaraUI/";
    const string Shared = "Assets/Images/Akt4KehadiranAirUI/";
    const string Icons = "Assets/Images/UI Flat/";
    const string RootName = "Akt4KehadiranUdaraReferenceRoot";
    static TMP_FontAsset heading, body;
    static Sprite rounded, banner;

    [MenuItem("Tools/MicroFun/Activity 4/Build Kehadiran Udara UI")]
    public static void Build()
    {
        var scene = EditorSceneManager.GetActiveScene();
        if (scene.path != ScenePath || EditorApplication.isPlayingOrWillChangePlaymode)
            throw new InvalidOperationException("Open AKT4 in Edit Mode. Other activities are not modified.");
        var canvas = scene.GetRootGameObjects().First(x => x.name == "Canvas").transform;
        var page = canvas.Find("Kehadiran Udara");
        if (page == null) throw new InvalidOperationException("Kehadiran Udara missing.");
        var source = page.Find("Back").GetComponent<Button>();
        if (source.onClick.GetPersistentEventCount() != 2 ||
            source.onClick.GetPersistentTarget(0) != page.gameObject ||
            source.onClick.GetPersistentTarget(1) != canvas.Find("Faktor").gameObject)
            throw new InvalidOperationException("Check Back callback targets.");
        heading = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/FredokaOne SDF.asset");
        body = TMP_Settings.defaultFontAsset;
        rounded = AssetDatabase.LoadAssetAtPath<Sprite>(Shared + "air_rounded.asset");
        if (heading == null || body == null || rounded == null ||
            AssetDatabase.LoadAssetAtPath<Texture2D>(Dir + "udara_food_atlas_v1.png") == null)
            throw new InvalidOperationException("Required assets missing.");
        string backup = "Assets/Scenes/Aktivity/akt4/akt4_before_udara_" + DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + ".unity";
        if (!EditorSceneManager.SaveScene(scene, backup, true)) throw new InvalidOperationException("Backup failed; no page changes applied.");
        PrepareAssets();
        var root = R(RootName + "_Building", page, 0,0,1920,1080);
        root.gameObject.SetActive(false);
        root.gameObject.AddComponent<KehadiranAirLayout>();
        try
        {
            A("Background",root,Shared+"air_warm_gradient.asset",0,0,1920,1080,false);
            var header=R("Header",root,0,0,1920,1080);
            P("ActivityBadge",header,0,481,318,61,"EAF7BD");
            T("Activity",header,"AKTIVITI 4",38,0,481,295,55,"185A0C",true,true);
            var title=P("TitleBanner",header,0,367,1310,141,"FFFFFF");
            title.sprite=banner;
            T("Title",header,"KEHADIRAN UDARA",76,36,367,1140,118,"FFFFFF",true,true);
            Circle("BookBadge",header,-623,367,148,"4B9B04");
            A("Book",header,Icons+"icon_book_open.png",-623,367,118,116);
            var deco=R("Decorations",root,0,0,1920,1080);
            A("PurpleMicrobe",deco,Icons+"char_purple_microbe.png",824,377,168,168);
            A("GreenMicrobe",deco,Icons+"char_green_microbe.png",-822,-389,205,205);
            A("WaterDrop",deco,Icons+"char_water_drop.png",846,-400,172,205);
            Star(deco,-391,478,48,"FFCA27"); Star(deco,399,481,47,"13AAFF");
            foreach(var v in new[]{new Vector2(-876,394),new Vector2(-782,470),new Vector2(-685,483),new Vector2(924,204),new Vector2(758,469),new Vector2(-902,-200)})
                Bubble(deco,v.x,v.y,42);
            Leaf(deco,-700,304,-30); Leaf(deco,693,312,30);

            // Reuse the verified portrait sprites without changing their source assets.
            Portrait(root,"Questioner",Shared+"BoyFramed.asset",-819,35,243,434);
            Portrait(root,"Responder",Shared+"GirlFramed.asset",827,15,228,415);

            var discussion=R("Discussion",root,0,0,1920,1080);
            Speech(discussion,"Question",-477,47,391,357,"149DDD","?",
                "Sayur asparagus ini dipek di dalam <color=#087BC2><b>pek vakum</b></color>.\n\nAdakah sayur-sayuran yang dipek di dalam pek vakum <b>tahan lebih lama?</b>",31);
            Speech(discussion,"Hypothesis",536,25,350,419,"47AB38","!",
                "<b>Saya berpendapat</b> bahawa sayur-sayuran akan tahan lebih lama kerana tiada <color=#087BC2><b>udara</b></color> di dalam pek vakum ini.\n\n<color=#7737B8><b>Mikroorganisma</b></color> tidak dapat tumbuh jika tiada udara.",30);
            T("HypothesisQualifier",discussion,"HIPOTESIS MURID • PERLU DIUJI",17,536,-207,348,35,"33722B",true,true);
            P("AsparagusPaper",discussion,46,13,573,470,"FFFCF3");
            A("VacuumAsparagus",discussion,Dir+"Asparagus.asset",46,7,450,382);
            Tab(discussion,"VacuumTab","PEK VAKUM",46,240,388,"177BC4");
            T("AsparagusCaption",discussion,"Sayur asparagus",26,46,-207,468,43,"195023",true,true);
            Footer(discussion,
                "Adakah sayur-sayuran di dalam <color=#087BC2><b>pek vakum</b></color> tahan lebih lama?",
                "Mari kita uji hipotesis ini melalui <color=#D34218><b>eksperimen</b></color>.");

            var investigation=R("Investigation",root,0,0,1920,1080);
            Speech(investigation,"ExperimentQuestion",-477,47,391,357,"149DDD","?",
                "Saya bersetuju dengan <color=#087BC2><b>hipotesis</b></color> kamu.\n\nMari kita jalankan eksperimen dengan menggunakan <b>hirisan epal</b> ini.",32);
            Speech(investigation,"ExperimentMethod",536,25,350,419,"47AB38","!",
                "Beberapa hirisan epal akan dipek di dalam <color=#087BC2><b>pek vakum</b></color>, manakala beberapa hirisan lagi <color=#D56313><b>tidak dipek</b></color> di dalam pek vakum.",32);
            P("ApplePaper",investigation,46,13,573,470,"FFFCF3");
            Tab(investigation,"ExperimentTab","HIRISAN EPAL",46,240,418,"177BC4");
            A("VacuumApple",investigation,Dir+"VacuumApple.asset",-94,18,242,330);
            A("OpenApple",investigation,Dir+"OpenApple.asset",184,18,242,330);
            T("VacuumCaption",investigation,"DIPEK VAKUM",24,-94,-176,255,65,"147AA9",true,true);
            T("OpenCaption",investigation,"TANPA PEK VAKUM",23,184,-176,255,65,"C15A15",true,true);
            for(int i=0;i<12;i++) P("CompareDash"+i,investigation,46,177-i*29,3,15,"D6C595",false);
            Footer(investigation,
                "Bagaimanakah kehadiran <color=#087BC2><b>udara</b></color> mempengaruhi pertumbuhan <color=#287C23><b>mikroorganisma</b></color>?",
                "Cuba buktikannya melalui <color=#D34218><b>eksperimen</b></color>.");

            var flow=root.gameObject.AddComponent<Akt4UdaraPresentation>();
            var label=T("PageLabel",header,"1 / 2  •  BINCANG",23,790,482,268,56,"32612A",true,true);
            var next=Btn("NextButton",root,"TERUSKAN",519,-471,244,65,"268D26");
            var prev=Btn("PreviousButton",root,"SEBELUM",247,-471,244,65,"176BAD");
            var back=Btn("BackButton",root,"KEMBALI",-812,484,239,68,"FFAD24");
            var arrow=A("Arrow",back.transform,Icons+"icon_arrow_left.png",-89,0,31,31);
            back.GetComponentInChildren<TMP_Text>().rectTransform.anchoredPosition=new Vector2(18,0);
            var relay=back.gameObject.AddComponent<CenterMenuButtonRelay>();
            var so=new SerializedObject(relay);
            so.FindProperty("sourceButton").objectReferenceValue=source;
            so.ApplyModifiedPropertiesWithoutUndo();
            UnityEventTools.AddPersistentListener(back.onClick,relay.InvokeSource);
            UnityEventTools.AddPersistentListener(next.onClick,flow.Next);
            UnityEventTools.AddPersistentListener(prev.onClick,flow.Previous);
            so=new SerializedObject(flow);
            so.FindProperty("discussion").objectReferenceValue=discussion.gameObject;
            so.FindProperty("investigation").objectReferenceValue=investigation.gameObject;
            so.FindProperty("pageLabel").objectReferenceValue=label;
            so.FindProperty("nextLabel").objectReferenceValue=next.GetComponentInChildren<TMP_Text>();
            so.FindProperty("previousButton").objectReferenceValue=prev;
            so.ApplyModifiedPropertiesWithoutUndo();
            flow.Show(0);
            if(root.GetComponentsInChildren<TMP_Text>(true).Any(t=>t.font==null) ||
                root.GetComponentsInChildren<Image>(true).Any(i=>i.sprite==null))
                throw new InvalidOperationException("Generated reference check failed.");

            var old=page.Find(RootName);
            if(old!=null) UnityEngine.Object.DestroyImmediate(old.gameObject);
            foreach(string name in new[]{"Top","Back","Title","Content","DialogParentRight","DialogParentLeft","DialogParentRight (1)","DialogParentLeft (1)"})
            {
                var legacy=page.Find(name);
                if(legacy!=null) legacy.gameObject.SetActive(false);
            }
            root.name=RootName; root.SetAsLastSibling(); root.gameObject.SetActive(true);
            EditorSceneManager.MarkSceneDirty(scene);
            if(!EditorSceneManager.SaveScene(scene)) throw new InvalidOperationException("Save failed.");
            Debug.Log("[Akt4Udara] Saved redesign; backup: "+backup);
        }
        catch { if(root!=null && root.name.EndsWith("_Building")) UnityEngine.Object.DestroyImmediate(root.gameObject); throw; }
    }

    static void Footer(Transform parent,string question,string prompt)
    {
        var f=P("InquiryFooter",parent,0,-383,1450,255,"FFFCED");
        // Lightweight dashed strokes mimic the reference without baking text into the artwork.
        for(int i=0;i<45;i++) {
            P("TopDash"+i,f.transform,-676+i*30,113,18,3,"EEA123",false);
            P("BottomDash"+i,f.transform,-676+i*30,-113,18,3,"EEA123",false);
        }
        Circle("BulbBadge",f.transform,-651,50,136,"FFDE74");
        A("Lightbulb",f.transform,Icons+"icon_lightbulb.png",-651,50,102,110);
        T("InquiryQuestion",f.transform,question,35,-48,49,1062,100,"24351D");
        P("Divider",f.transform,-40,-13,1060,2,"ECB746",false);
        T("ExperimentPrompt",f.transform,prompt,30,-68,-31,1010,44,"24351D");
        A("Magnifier",f.transform,Icons+"icon_magnify_glass.png",630,29,160,160);
    }
    static void Speech(Transform p,string name,float x,float y,float w,float h,string color,string symbol,string copy,float size)
    {
        var outer=P(name,p,x,y,w,h,color);
        P("Paper",outer.transform,0,0,w-8,h-8,"FFFCF3",false);
        // A rotated square, covered by the paper edge, forms the speech pointer.
        var tail=P("Tail",outer.transform,name.Contains("Question")?-w/2:w/2,0,24,24,color,false);
        tail.rectTransform.localRotation=Quaternion.Euler(0,0,45); tail.transform.SetAsFirstSibling();
        Circle("Badge",outer.transform,-w/2+8,h/2-8,78,color);
        T("BadgeText",outer.transform,symbol,52,-w/2+8,h/2-8,68,68,"FFFFFF",true,true);
        T("Copy",outer.transform,copy,size,7,-8,w-57,h-49,"182716");
    }
    static void Portrait(Transform p,string name,string path,float x,float y,float w,float h)
    {
        P(name+"Frame",p,x,y,w,h,"FFFCF3");
        A(name,p,path,x,y,w-10,h-14);
    }
    static void Tab(Transform p,string name,string copy,float x,float y,float w,string color)
    {
        P(name,p,x,y,w,72,color);
        T(name+"Label",p,copy,33,x,y,w-22,65,"FFFFFF",true,true);
    }
    static Button Btn(string name,Transform p,string copy,float x,float y,float w,float h,string color)
    {
        var im=P(name,p,x,y,w,h,color); im.raycastTarget=true;
        var b=im.gameObject.AddComponent<Button>(); b.targetGraphic=im;
        var c=b.colors; c.highlightedColor=new Color(1,.96f,.81f);c.pressedColor=new Color(.76f,.84f,.72f);b.colors=c;
        T("Label",im.transform,copy,27,0,0,w-26,h-10,"FFFFFF",true,true);
        return b;
    }
    static RectTransform R(string name,Transform p,float x,float y,float w,float h)
    {
        var o=new GameObject(name,typeof(RectTransform));o.layer=p.gameObject.layer;
        var r=(RectTransform)o.transform;r.SetParent(p,false);
        r.anchorMin=r.anchorMax=r.pivot=Vector2.one*.5f;r.anchoredPosition=new Vector2(x,y);r.sizeDelta=new Vector2(w,h);
        return r;
    }
    static Image A(string name,Transform p,string path,float x,float y,float w,float h,bool aspect=true)
    {
        var i=R(name,p,x,y,w,h).gameObject.AddComponent<Image>();
        i.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if(i.sprite==null)throw new InvalidOperationException("Missing sprite "+path);
        i.preserveAspect=aspect;i.raycastTarget=false;return i;
    }
    static Image P(string name,Transform p,float x,float y,float w,float h,string color,bool edge=true)
    {
        var i=R(name,p,x,y,w,h).gameObject.AddComponent<Image>();i.sprite=rounded;i.type=Image.Type.Sliced;i.color=C(color);i.raycastTarget=false;
        if(edge) {
            var sh=i.gameObject.AddComponent<Shadow>();sh.effectColor=new Color(.17f,.25f,.06f,.22f);sh.effectDistance=new Vector2(0,-5);
            var ol=i.gameObject.AddComponent<Outline>();ol.effectColor=Color.white;ol.effectDistance=new Vector2(2,-2);
        } return i;
    }
    static TMP_Text T(string name,Transform p,string value,float size,float x,float y,float w,float h,string color,bool centre=false,bool title=false)
    {
        var t=R(name,p,x,y,w,h).gameObject.AddComponent<TextMeshProUGUI>();t.font=title?heading:body;
        t.text=value;t.fontSize=size;t.color=C(color);t.richText=true;t.raycastTarget=false;
        t.textWrappingMode=TextWrappingModes.Normal;t.overflowMode=TextOverflowModes.Overflow;
        t.alignment=centre?TextAlignmentOptions.Center:TextAlignmentOptions.MidlineLeft;
        if(name=="Title"){t.outlineColor=C("1B500D");t.outlineWidth=.12f;}
        return t;
    }
    static void Circle(string name,Transform p,float x,float y,float size,string color)
    {
        var im=A(name,p,Icons+"ui_circle_white.png",x,y,size,size);im.color=C(color);
        var ol=im.gameObject.AddComponent<Outline>();ol.effectColor=Color.white;ol.effectDistance=new Vector2(3,-3);
    }
    static void Bubble(Transform p,float x,float y,float size)
    {
        var im=A("Bubble",p,Dir+"bubble.asset",x,y,size,size); im.color=Color.white;
    }
    static void Star(Transform p,float x,float y,float size,string color)
    {
        var im=A("Star",p,Dir+"star.asset",x,y,size,size);im.color=C(color);
        var o=im.gameObject.AddComponent<Outline>();o.effectColor=Color.white;o.effectDistance=new Vector2(3,-3);
    }
    static void Leaf(Transform p,float x,float y,float angle)
    {
        var im=A("Leaf",p,Icons+"ui_circle_white.png",x,y,67,28,false);im.color=C("43A331");im.rectTransform.localRotation=Quaternion.Euler(0,0,angle);
    }
    static Color C(string hex){Color c;ColorUtility.TryParseHtmlString("#"+hex,out c);return c;}

    static void PrepareAssets()
    {
        var imp=(TextureImporter)AssetImporter.GetAtPath(Dir+"udara_food_atlas_v1.png");
        if(imp.npotScale!=TextureImporterNPOTScale.None || imp.mipmapEnabled || imp.textureCompression!=TextureImporterCompression.Uncompressed) {
            imp.npotScale=TextureImporterNPOTScale.None;imp.mipmapEnabled=false;imp.maxTextureSize=2048;
            imp.textureCompression=TextureImporterCompression.Uncompressed;imp.wrapMode=TextureWrapMode.Clamp;imp.SaveAndReimport();
        }
        var tex=AssetDatabase.LoadAssetAtPath<Texture2D>(Dir+"udara_food_atlas_v1.png");
        string[] names={"Asparagus","VacuumApple","OpenApple"};
        var rects=new[]{new Rect(35,55,496,908),new Rect(543,140,455,700),new Rect(1016,168,494,538)};
        for(int i=0;i<names.Length;i++)if(AssetDatabase.LoadAssetAtPath<Sprite>(Dir+names[i]+".asset")==null) {
            var r=rects[i];r.x*=tex.width/1536f;r.width*=tex.width/1536f;r.y*=tex.height/1024f;r.height*=tex.height/1024f;
            AssetDatabase.CreateAsset(Sprite.Create(tex,r,Vector2.one*.5f,100,0,SpriteMeshType.FullRect),Dir+names[i]+".asset");
        }
        banner=AssetDatabase.LoadAssetAtPath<Sprite>(Dir+"banner.asset");
        if(banner==null) {
            var texture=new Texture2D(128,128,TextureFormat.RGBA32,false);
            for(int y=0;y<128;y++)for(int x=0;x<128;x++){
                float dx=Mathf.Max(Mathf.Abs(x-63.5f)-27,0),dy=Mathf.Max(Mathf.Abs(y-63.5f)-27,0);
                float alpha=Mathf.Clamp01(36-Mathf.Sqrt(dx*dx+dy*dy));
                var c=Color.Lerp(C("145700"),C("77B900"),y/127f);
                if(y>112)c=Color.Lerp(c,Color.white,.24f);
                c.a=alpha;texture.SetPixel(x,y,c);
            }
            texture.Apply();AssetDatabase.CreateAsset(texture,Dir+"banner_texture.asset");
            banner=Sprite.Create(texture,new Rect(0,0,128,128),Vector2.one*.5f,100,0,SpriteMeshType.FullRect,new Vector4(45,45,45,45));
            AssetDatabase.CreateAsset(banner,Dir+"banner.asset");
        }
        foreach(string name in new[]{"bubble","star"}) if(AssetDatabase.LoadAssetAtPath<Sprite>(Dir+name+".asset")==null) {
            var texture=new Texture2D(96,96,TextureFormat.RGBA32,false);
            for(int y=0;y<96;y++)for(int x=0;x<96;x++){
                float dx=x-47.5f,dy=y-47.5f,r=Mathf.Sqrt(dx*dx+dy*dy);Color c=Color.white;
                if(name=="bubble") {
                    c=Color.Lerp(new Color(.72f,.93f,.78f,.13f),Color.white,Mathf.Clamp01((r-38)/5));
                    c.a=r>45?0:Mathf.Lerp(.13f,.95f,Mathf.Clamp01((r-39)/4));
                    if(Vector2.Distance(new Vector2(dx,dy),new Vector2(-16,24))<7)c=Color.white;
                } else {
                    float a=Mathf.Atan2(dy,dx)-Mathf.PI/2;
                    float k=Mathf.Repeat(a/(Mathf.PI/5),2);
                    float outer=Mathf.Lerp(42,20,k<1?k:2-k);
                    c.a=Mathf.Clamp01(outer-r);
                }
                texture.SetPixel(x,y,c);
            }
            texture.Apply();AssetDatabase.CreateAsset(texture,Dir+name+"_texture.asset");
            AssetDatabase.CreateAsset(Sprite.Create(texture,new Rect(0,0,96,96),Vector2.one*.5f),Dir+name+".asset");
        }
        AssetDatabase.SaveAssets();
    }
}
#endif
