#if UNITY_EDITOR
using System;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class MicroScene4StyleBuilder
{
    const string ScenePath="Assets/Scenes/Aktivity/akt4/Microscene4.unity";
    const string Art="Assets/Images/MicroScene4UI/";
    static Sprite round;
    static TMP_FontAsset heading,body;
    static Color navy=C("#06345B"),blue=C("#087BC1"),orange=C("#CE4C08"),green=C("#18733B");
    static Color C(string hex){Color c;ColorUtility.TryParseHtmlString(hex,out c);return c;}
    static RectTransform R(Transform t,float x,float y,float w,float h)
    {
        var r=(RectTransform)t;r.anchorMin=r.anchorMax=r.pivot=new Vector2(.5f,.5f);
        r.anchoredPosition=new Vector2(x,y);r.sizeDelta=new Vector2(w,h);r.localScale=Vector3.one;r.localRotation=Quaternion.identity;
        return r;
    }
    static Transform Need(Transform root,string path)
    {
        var t=root.Find(path);if(!t)throw new Exception("Missing UI: "+path);return t;
    }
    static Image Panel(Transform t,Color color,bool hit=false)
    {
        var i=t.GetComponent<Image>();if(!i)i=t.gameObject.AddComponent<Image>();
        i.enabled=true;i.sprite=round;i.type=Image.Type.Sliced;i.pixelsPerUnitMultiplier=1;i.color=color;i.raycastTarget=hit;i.preserveAspect=false;
        return i;
    }
    static void Edge(Graphic g,Color color,float amount)
    {
        var o=g.GetComponent<Outline>();if(!o)o=g.gameObject.AddComponent<Outline>();
        o.effectColor=color;o.effectDistance=new Vector2(amount,-amount);o.useGraphicAlpha=true;
    }
    static RectTransform Node(Transform parent,string name,float x,float y,float w,float h)
    {
        var t=parent.Find(name);
        if(!t){var go=new GameObject(name,typeof(RectTransform));go.transform.SetParent(parent,false);t=go.transform;}
        return R(t,x,y,w,h);
    }
    static TMP_Text Font(Transform t,float size,Color color,bool title=false,string value=null)
    {
        var text=t.GetComponent<TMP_Text>();if(!text)text=t.gameObject.AddComponent<TextMeshProUGUI>();
        text.font=title?heading:body;text.fontSize=size;text.enableAutoSizing=false;text.fontStyle=title?FontStyles.Normal:FontStyles.Bold;
        text.color=color;text.alignment=TextAlignmentOptions.Center;text.margin=Vector4.zero;
        text.overflowMode=TextOverflowModes.Overflow;text.raycastTarget=false;
        if(value!=null)text.text=value;return text;
    }
    static TMP_Text Label(Transform p,string name,string value,float x,float y,float w,float h,float size,Color color,bool title=false)
    {return Font(Node(p,name,x,y,w,h),size,color,title,value);}
    static void Icon(Transform p,string name,string asset,float x,float y,float w,float h)
    {
        var t=Node(p,name,x,y,w,h);var i=t.GetComponent<Image>();if(!i)i=t.gameObject.AddComponent<Image>();
        i.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(asset);if(!i.sprite)throw new Exception("Missing sprite "+asset);
        i.color=Color.white;i.preserveAspect=true;i.raycastTarget=false;i.type=Image.Type.Simple;
    }
    static void ButtonStyle(Transform t,Color color,float x,float y,float w,float h,float size,string caption=null)
    {
        R(t,x,y,w,h);var i=Panel(t,color,true);Edge(i,Color.white,2);
        var b=t.GetComponent<Button>();b.targetGraphic=i;b.transition=Selectable.Transition.ColorTint;
        var colors=b.colors;colors.normalColor=Color.white;colors.highlightedColor=new Color(1,.96f,.78f);
        colors.pressedColor=new Color(.78f,.85f,.9f);colors.selectedColor=Color.white;
        b.colors=colors;
        var label=t.GetComponentInChildren<TMP_Text>(true);R(label.transform,0,0,w-12,h-4);Font(label.transform,size,Color.white,true,caption);
    }
    static Sprite DishSprite(string name,int side)
    {
        var existing=AssetDatabase.LoadAssetAtPath<Sprite>(Art+name+".asset");if(existing)return existing;
        var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(Art+"petri_atlas_v1.png");
        float sx=texture.width/1536f,sy=texture.height/1024f;
        var sprite=Sprite.Create(texture,new Rect(side*768*sx,180*sy,768*sx,650*sy),new Vector2(.5f,.5f),100,0,SpriteMeshType.FullRect);
        sprite.name=name;AssetDatabase.CreateAsset(sprite,Art+name+".asset");return sprite;
    }
    static void Dish(Transform root,string name,string title,string subtitle,float x,Color accent,Sprite dish)
    {
        var t=Need(root,name);R(t,x,20,560,520);var frame=Panel(t,sideColor(name));Edge(frame,accent,4);
        var art=Node(t,"Style_DishArt",0,-15,550,400);var img=art.GetComponent<Image>();if(!img)img=art.gameObject.AddComponent<Image>();
        img.sprite=dish;img.color=Color.white;img.type=Image.Type.Simple;img.raycastTarget=false;art.SetAsFirstSibling();
        var band=Need(t,"LabelImage");R(band,0,214,520,64);Panel(band,accent);
        var label=band.GetComponentInChildren<TMP_Text>(true);R(label.transform,0,0,500,62);Font(label.transform,36,Color.white,true,title);
        Label(t,"Style_Condition",subtitle,0,158,516,42,27,accent);
        var container=Need(t,name=="PetriDishA"?"MicrobeContainer_A":"MicrobeContainer_B");
        R(container,0,-15,420,240);container.SetAsLastSibling();
        var result=Need(t,name=="PetriDishA"?"ResultText_A":"ResultText_B");
        var resultBg=Node(t,"Style_ResultCard",0,-211,510,64);Panel(resultBg,new Color(1,1,1,.86f));
        R(result,0,-211,500,58);Font(result,27,navy);result.SetAsLastSibling();
    }
    static Color sideColor(string name){return name=="PetriDishA"?C("#E6F5FF"):C("#FFF0D2");}
    static void Row(Transform root,string path,float y,Color color,string[] buttons)
    {
        var t=Need(root,path);R(t,0,y,610,76);Panel(t,new Color(1,1,1,.9f));
        var icon=Need(t,"Icon");R(icon,-267,0,48,48);icon.GetComponent<Image>().raycastTarget=false;
        var label=Need(t,"Text");R(label,-151,0,178,64);Font(label,24,navy);
        float width=buttons.Length==3?87:135;
        for(int n=0;n<buttons.Length;n++)ButtonStyle(Need(t,buttons[n]),color,28+n*(width+10),0,width,54,22);
    }
    public static string Apply()
    {
        if(EditorApplication.isPlaying)throw new Exception("Stop Play Mode first.");
        var scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if(scene.path!=ScenePath)throw new Exception("Wrong scene.");
        var canvas=scene.GetRootGameObjects().First(x=>x.name=="Canvas").transform;
        // Validate functional objects before any scene changes.
        foreach(var path in new[]{"Back","HeaderTitle","ExperimentArea/PetriDishA/MicrobeContainer_A","ExperimentArea/PetriDishB/MicrobeContainer_B","FactorPanel/1/WaterButton","FactorPanel/1/UdaraButton","FactorPanel/1/MasaButton","FactorPanel/2/PlayTime_Button","ObservationPanel/Questions/DropSlot_1","ObservationPanel/Questions/DropSlot_2","ProgressPanel/ProgressSlider"})
            Need(canvas,path);
        var sim=UnityEngine.Object.FindFirstObjectByType<SimulationManager>();
        if(!sim||!sim.microbePrefab||!sim.feedbackText)throw new Exception("Missing simulation references.");
        round=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Images/Akt4KehadiranAirUI/air_rounded.asset");
        heading=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/FredokaOne SDF.asset");
        body=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");
        if(!round||!heading||!body)throw new Exception("Missing style assets.");
        AssetDatabase.ImportAsset(Art+"petri_atlas_v1.png",ImportAssetOptions.ForceUpdate);
        var importer=(TextureImporter)AssetImporter.GetAtPath(Art+"petri_atlas_v1.png");
        importer.textureType=TextureImporterType.Sprite;importer.npotScale=TextureImporterNPOTScale.None;importer.maxTextureSize=2048;importer.mipmapEnabled=false;importer.SaveAndReimport();
        var dishA=DishSprite("DishA_v1",0);var dishB=DishSprite("DishB_v1",1);
        if(!EditorSceneManager.SaveScene(scene,"Assets/Scenes/Aktivity/akt4/Microscene4_before_apply_"+DateTime.Now.ToString("yyyyMMdd_HHmmss")+".unity",true))throw new Exception("Backup failed.");
        Undo.RegisterFullObjectHierarchyUndo(canvas.gameObject,"MicroScene4 illustrated style");
        var background=Need(canvas,"Image").GetComponent<RawImage>();
        background.texture=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Images/Akt4KehadiranAirUI/air_warm_gradient.asset").texture;
        background.color=Color.white;background.raycastTarget=false;
        var bg=(RectTransform)background.transform;bg.anchorMin=Vector2.zero;bg.anchorMax=Vector2.one;bg.offsetMin=bg.offsetMax=Vector2.zero;
        var decor=Node(canvas,"Style_Decoration",0,0,1920,1080);decor.SetSiblingIndex(1);
        var badge=Node(decor,"ActivityBadge",0,489,330,55);Panel(badge,navy);Edge(badge.GetComponent<Image>(),Color.white,2);
        Label(badge,"Caption","AKTIVITI 4",0,0,320,50,38,Color.white,true);
        Icon(decor,"GreenMicrobe","Assets/Images/UI Flat/char_green_microbe.png",-754,385,170,180);
        Icon(decor,"BlueMicrobe","Assets/Images/UI Flat/char_blue_microbe.png",-531,431,94,94);
        Icon(decor,"PurpleMicrobe","Assets/Images/UI Flat/char_purple_microbe.png",740,420,172,172);
        Icon(decor,"WaterDrop","Assets/Images/UI Flat/char_water_drop.png",537,402,99,123);
        for(int j=0;j<8;j++)Icon(decor,"Bubble"+j,"Assets/Images/Akt4KehadiranUdaraUI/bubble.asset",(j<4?-1:1)*(830+(j%2)*61),325+(j%4)*52,24+(j%3)*10,24+(j%3)*10);
        var title=Need(canvas,"HeaderTitle");R(title,0,414,1110,108);
        var tx=Font(title,124,Color.white,true,"MICRO-SCENE");
        var titleMaterial=AssetDatabase.LoadAssetAtPath<Material>(Art+"TitleOutline.mat");
        if(!titleMaterial){titleMaterial=new Material(heading.material);AssetDatabase.CreateAsset(titleMaterial,Art+"TitleOutline.mat");}
        titleMaterial.SetColor(ShaderUtilities.ID_OutlineColor,navy);
        titleMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth,.25f);
        titleMaterial.EnableKeyword("OUTLINE_ON");EditorUtility.SetDirty(titleMaterial);
        tx.fontSharedMaterial=titleMaterial;
        var oldOutline=tx.GetComponent<Outline>();if(oldOutline)oldOutline.enabled=false;
        var sub=Node(decor,"Subtitle",0,342,730,48);Panel(sub,C("#00623C"));
        Label(sub,"Caption","Simulasi Eksperimen Maya",0,0,710,46,31,Color.white,true);
        Label(decor,"Prompt","Apa yang membantu mikroorganisma membiak?",0,294,1080,40,29,C("#124B2B"));
        ButtonStyle(Need(canvas,"Back"),navy,-824,489,200,54,26,"KEMBALI");
        var area=Need(canvas,"ExperimentArea");R(area,0,0,1920,1080);
        Dish(area,"PetriDishA","Persekitaran A","Ada air + udara cukup", -640,blue,dishA);
        Dish(area,"PetriDishB","Persekitaran B","Mengikut pilihan faktor",640,orange,dishB);
        var factors=Need(canvas,"FactorPanel");R(factors,0,20,650,520);Panel(factors,new Color(1,1,1,.6f));
        var one=Need(factors,"1");R(one,0,0,650,520);
        R(Need(one,"Text (TMP)"),0,218,620,44);Font(Need(one,"Text (TMP)"),29,navy,true,"1. PILIH FAKTOR B");
        Row(one,"WaterButton",145,blue,new[]{"Water_Ada_Button","Water_Tiada_Button"});
        Row(one,"UdaraButton",60,C("#298B79"),new[]{"Air_Cukup_Button","Air_Kurang_Button"});
        Row(one,"MasaButton",-25,C("#7153B4"),new[]{"Time_1_Button","Time_2_Button","Time_3_Button"});
        var two=Need(factors,"2");R(two,0,-125,610,86);
        R(Need(two,"Text (TMP)"),0,46,590,28);Font(Need(two,"Text (TMP)"),19,green,true,"2. MULAKAN SIMULASI");
        var play=Need(two,"PlayTime_Button");ButtonStyle(play,green,0,-5,540,68,32,"MAINKAN MASA");
        R(play.GetComponentInChildren<TMP_Text>().transform,27,0,430,64);
        Icon(play,"Style_PlayIcon","Assets/Images/UI Flat/icon_play.png",-215,0,46,46);
        var three=Need(factors,"3");R(three,0,-218,620,42);
        R(Need(three,"Text (TMP)"),0,0,620,40);Font(Need(three,"Text (TMP)"),23,green,true,"3. CERAP DAN BANDINGKAN");
        var progress=Need(canvas,"ProgressPanel");R(progress,0,-256,650,28);
        var slider=Need(progress,"ProgressSlider");R(slider,-46,0,510,20);
        var track=Need(slider,"Background");R(track,0,0,510,18);Panel(track,C("#D5E3CF"));
        var fillArea=(RectTransform)Need(slider,"Fill Area");fillArea.anchorMin=Vector2.zero;fillArea.anchorMax=Vector2.one;fillArea.offsetMin=fillArea.offsetMax=Vector2.zero;
        var fill=Need(fillArea,"Fill").GetComponent<Image>();fill.sprite=round;fill.type=Image.Type.Sliced;fill.color=green;fill.raycastTarget=false;
        R(Need(progress,"ProgressText"),270,0,130,30);Font(Need(progress,"ProgressText"),23,navy);
        var obs=Need(canvas,"ObservationPanel");R(obs,0,-395,1820,230);Edge(Panel(obs,C("#FFFBE9")),C("#DEAB39"),3);
        var obsh=Need(obs,"Image");R(obsh,0,84,1745,45);Panel(obsh,green);
        var obslabel=obsh.GetComponentInChildren<TMP_Text>();R(obslabel.transform,0,0,1700,44);Font(obslabel.transform,27,Color.white,true,"RAKAMAN PEMERHATIAN  -  Seret jawapan ke ruang kosong");
        var questions=Need(obs,"Questions");R(questions,-235,-3,1260,120);
        var q1=Need(questions,"Question1");R(q1,-185,27,760,58);Font(q1,27,navy).alignment=TextAlignmentOptions.MidlineLeft;
        var q2=Need(questions,"Text (TMP) (1)");R(q2,-185,-38,760,60);Font(q2,27,navy).alignment=TextAlignmentOptions.MidlineLeft;
        // Preserve question wording and answer IDs.
        for(int k=0;k<2;k++){
            var slot=Need(questions,"DropSlot_"+(k+1));R(slot,440,27-k*65,300,57);
            Edge(Panel(slot,C("#E6F1E1"),true),green,2);
        }
        var answers=Need(obs,"Answers");R(answers,645,-4,430,125);
        string[] names={"Button","Button (1)","Button (2)","Button (3)"};
        Color[] colors={blue,C("#A96116"),C("#7651AD"),green};
        for(int n=0;n<4;n++)ButtonStyle(Need(answers,names[n]),colors[n],n%2==0?-109:109,n<2?28:-35,204,53,21);
        var feedback=Need(canvas,"FeedbackText");R(feedback,0,-490,1740,31);Font(feedback,24,navy);
        // Layout groups would otherwise override these explicitly authored positions.
        foreach(var group in canvas.GetComponentsInChildren<LayoutGroup>(true))group.enabled=false;
        foreach(var fitter in canvas.GetComponentsInChildren<ContentSizeFitter>(true))fitter.enabled=false;
        Canvas.ForceUpdateCanvases();
        EditorSceneManager.MarkSceneDirty(scene);AssetDatabase.SaveAssets();
        if(!EditorSceneManager.SaveScene(scene))throw new Exception("Scene save failed.");
        return "Styled and saved MicroScene4. Gameplay components, callbacks and prefab unchanged.";
    }
}
#endif
