#if UNITY_EDITOR
using System;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class Quiz4StyleBuilder
{
    const string ScenePath="Assets/Scenes/Aktivity/akt4/quiz4.unity";
    static Sprite rounded;
    static TMP_FontAsset heading,body;
    static Color navy=Hex("#06345B"),blue=Hex("#087BC1"),green=Hex("#18733B"),orange=Hex("#CF650C");
    static Color Hex(string h){Color c;ColorUtility.TryParseHtmlString(h,out c);return c;}
    static RectTransform Rect(Transform t,float x,float y,float w,float h)
    {
        var r=(RectTransform)t;r.anchorMin=r.anchorMax=r.pivot=new Vector2(.5f,.5f);
        r.anchoredPosition=new Vector2(x,y);r.sizeDelta=new Vector2(w,h);r.localScale=Vector3.one;r.localRotation=Quaternion.identity;return r;
    }
    static Transform Find(Transform p,string path){var t=p.Find(path);if(!t)throw new Exception("Missing "+path);return t;}
    static RectTransform Node(Transform p,string name,float x,float y,float w,float h)
    {
        var t=p.Find(name);if(!t){t=new GameObject(name,typeof(RectTransform)).transform;t.SetParent(p,false);}
        return Rect(t,x,y,w,h);
    }
    static Image Panel(Transform t,Color color,bool hit=false)
    {
        var image=t.GetComponent<Image>();if(!image)image=t.gameObject.AddComponent<Image>();
        image.sprite=rounded;image.type=Image.Type.Sliced;image.color=color;image.pixelsPerUnitMultiplier=.6f;
        image.raycastTarget=hit;image.preserveAspect=false;return image;
    }
    static void Edge(Graphic g,Color color,float width=3)
    {
        var edge=g.GetComponent<Outline>();if(!edge)edge=g.gameObject.AddComponent<Outline>();
        edge.effectColor=color;edge.effectDistance=new Vector2(width,-width);
    }
    static TMP_Text Text(Transform t,float size,Color color,bool title=false,string value=null)
    {
        var text=t.GetComponent<TMP_Text>();if(!text)text=t.gameObject.AddComponent<TextMeshProUGUI>();
        text.font=title?heading:body;text.fontSize=size;text.enableAutoSizing=false;text.color=color;
        text.fontStyle=title?FontStyles.Normal:FontStyles.Bold;text.alignment=TextAlignmentOptions.Center;
        text.margin=Vector4.zero;text.overflowMode=TextOverflowModes.Overflow;text.raycastTarget=false;
        if(value!=null)text.text=value;return text;
    }
    static void Icon(Transform p,string name,string sprite,float x,float y,float w,float h)
    {
        var t=Node(p,name,x,y,w,h);var i=t.GetComponent<Image>();if(!i)i=t.gameObject.AddComponent<Image>();
        i.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(sprite);if(!i.sprite)throw new Exception("Missing art "+sprite);
        i.color=Color.white;i.type=Image.Type.Simple;i.preserveAspect=true;i.raycastTarget=false;
    }
    static void Button(Transform t,Color color,float x,float y,float w,float h,float size,bool title=true,string caption=null)
    {
        Rect(t,x,y,w,h);var image=Panel(t,color,true);Edge(image,Color.white,3);
        var b=t.GetComponent<Button>();b.targetGraphic=image;b.transition=Selectable.Transition.ColorTint;
        var colors=b.colors;colors.normalColor=colors.selectedColor=Color.white;
        colors.highlightedColor=new Color(1,.94f,.73f);colors.pressedColor=new Color(.75f,.83f,.88f);
        colors.disabledColor=new Color(.55f,.62f,.67f);b.colors=colors;
        var label=t.GetComponentInChildren<TMP_Text>(true);Rect(label.transform,0,0,w-48,h-12);Text(label.transform,size,Color.white,title,caption);
    }
    static void Header(Transform p,Transform existing=null)
    {
        var t=existing?existing:Node(p,"Style_Title",0,451,1380,148);
        Rect(t,0,451,1380,148);var txt=Text(t,108,Color.white,true,"MICRO-FUN QUIZ");
        txt.fontSharedMaterial=AssetDatabase.LoadAssetAtPath<Material>("Assets/Images/MicroScene4UI/TitleOutline.mat");
    }
    static void ResetGroup(Transform t)
    {
        Rect(t,0,0,1640,700);
        foreach(Transform child in t)if(!child.GetComponent<Button>()&&!child.GetComponent<TMP_Text>())Rect(child,0,0,1640,700);
    }
    static void Face(Transform button,int mood)
    {
        var face=Node(button,"Style_Emotion",0,41,106,106);
        Icon(button,"Style_Emotion","Assets/Images/UI Flat/ui_circle_white.png",0,41,106,106);
        face.GetComponent<Image>().color=Hex("#FFE489");
        foreach(int side in new[]{-1,1}){
            Icon(face,"Eye"+side,"Assets/Images/UI Flat/ui_circle_white.png",side*20,12,10,13);
            face.Find("Eye"+side).GetComponent<Image>().color=navy;
        }
        Vector2[] points=mood==0?new[]{new Vector2(-24,-10),new Vector2(-12,-22),new Vector2(0,-26),new Vector2(12,-22),new Vector2(24,-10)}
            :mood==1?new[]{new Vector2(-23,-17),new Vector2(-11,-17),new Vector2(0,-17),new Vector2(11,-17),new Vector2(23,-17)}
            :new[]{new Vector2(-24,-26),new Vector2(-12,-15),new Vector2(0,-11),new Vector2(12,-15),new Vector2(24,-26)};
        for(int k=0;k<4;k++){
            var mid=(points[k]+points[k+1])/2;var delta=points[k+1]-points[k];
            var segment=Node(face,"Mouth"+k,mid.x,mid.y,delta.magnitude+3,5);Panel(segment,navy);
            segment.localRotation=Quaternion.Euler(0,0,Mathf.Atan2(delta.y,delta.x)*Mathf.Rad2Deg);
        }
    }
    public static string Apply()
    {
        var scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if(EditorApplication.isPlaying||scene.path!=ScenePath)throw new Exception("quiz4 Edit Mode required.");
        var q=scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<QuizManager4>(true)).Single();
        if(typeof(QuizManager4).GetFields().Any(f=>typeof(UnityEngine.Object).IsAssignableFrom(f.FieldType)&&f.GetValue(q)==null))
            throw new Exception("Missing manager references.");
        rounded=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Images/Akt4KehadiranAirUI/air_rounded.asset");
        heading=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/FredokaOne SDF.asset");
        body=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");
        if(!rounded||!heading||!body)throw new Exception("Missing shared style resources.");
        var canvas=q.startPanel.transform.parent;
        foreach(var path in new[]{"StartPanel/TitleText","StartPanel/StartButton","DragDropPanel/DragSlot","ReflectionPanel/DragFeedbackText","ResultPanel/RestartButton","ResultPanel/RestartButton (1)"})Find(canvas,path);
        if(!EditorSceneManager.SaveScene(scene,"Assets/Scenes/Aktivity/akt4/quiz4_before_apply_"+DateTime.Now.ToString("yyyyMMdd_HHmmss")+".unity",true))throw new Exception("Backup failed.");
        Undo.RegisterFullObjectHierarchyUndo(canvas.gameObject,"Quiz4 illustrated UI");
        foreach(var layout in canvas.GetComponentsInChildren<LayoutGroup>(true))layout.enabled=false;
        foreach(var fitter in canvas.GetComponentsInChildren<ContentSizeFitter>(true))fitter.enabled=false;
        var bg=Find(canvas,"Background").GetComponent<Image>();
        bg.sprite=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Images/Akt4KehadiranAirUI/air_warm_gradient.asset");
        bg.color=Color.white;bg.type=Image.Type.Simple;bg.preserveAspect=false;bg.raycastTarget=false;
        var bgr=(RectTransform)bg.transform;bgr.anchorMin=Vector2.zero;bgr.anchorMax=Vector2.one;bgr.offsetMin=bgr.offsetMax=Vector2.zero;
        var decor=Node(canvas,"Style_Decoration",0,0,1920,1080);decor.SetSiblingIndex(1);
        var badge=Node(decor,"ActivityBadge",0,486,330,58);Edge(Panel(badge,navy),Color.white,2);
        Text(Node(badge,"Caption",0,0,315,54),38,Color.white,true,"AKTIVITI 4");
        Icon(decor,"MicrobeLeft","Assets/Images/UI Flat/char_green_microbe.png",-821,411,146,156);
        Icon(decor,"MicrobeRight","Assets/Images/UI Flat/char_purple_microbe.png",824,414,151,158);
        Icon(decor,"WaterDrop","Assets/Images/UI Flat/char_water_drop.png",-866,-443,106,137);
        Icon(decor,"BlueMicrobe","Assets/Images/UI Flat/char_blue_microbe.png",864,-445,117,117);
        for(int k=0;k<8;k++)Icon(decor,"Bubble"+k,"Assets/Images/Akt4KehadiranUdaraUI/bubble.asset",(k<4?-1:1)*(888+(k%2)*31),290-(k%4)*194,24+k%3*12,24+k%3*12);
        var panels=new[]{q.startPanel,q.quizPanel,q.dragDropPanel,q.reflectionPanel,q.resultPanel};
        var accents=new[]{green,blue,orange,green,green};
        for(int k=0;k<panels.Length;k++){
            Rect(panels[k].transform,0,-65,1640,700);Edge(Panel(panels[k].transform,Hex("#FFFBEA")),accents[k],4);
            Header(panels[k].transform,k==0?Find(q.startPanel.transform,"TitleText"):null);
        }
        var start=q.startPanel.transform;
        Icon(start,"Style_QuizHero","Assets/Images/UI Flat/icon_quiz_checklist.png",0,76,330,330);
        Icon(start,"Style_BlueFriend","Assets/Images/UI Flat/char_blue_microbe.png",-451,40,216,216);
        Icon(start,"Style_GreenFriend","Assets/Images/UI Flat/char_green_microbe.png",451,40,216,236);
        Button(Find(start,"StartButton"),green,0,-219,490,102,46,true);
        var quiz=q.quizPanel.transform;
        var frame=Node(quiz,"Style_ImageFrame",-466,0,596,519);Edge(Panel(frame,Hex("#E6F5FF")),blue,3);frame.SetAsFirstSibling();
        Rect(q.questionImage.transform,-466,0,534,457);q.questionImage.preserveAspect=true;q.questionImage.color=Color.white;q.questionImage.raycastTarget=false;
        Rect(q.questionText.transform,322,220,870,161);Text(q.questionText.transform,40,navy);
        ResetGroup(Find(quiz,"Buttons"));
        var answers=new[]{q.answerButton1,q.answerButton2,q.answerButton3};
        for(int k=0;k<3;k++)Button(answers[k].transform,blue,322,67-k*127,866,105,33,false);
        Rect(q.feedbackText.transform,0,-303,1500,60);Text(q.feedbackText.transform,33,navy);
        var drag=q.dragDropPanel.transform;
        Rect(Find(drag,"DragQuestionText"),0,231,1480,109);Text(Find(drag,"DragQuestionText"),45,navy,true);
        var slot=Find(drag,"DragSlot");Rect(slot,0,37,455,140);Edge(Panel(slot,Hex("#DBEFFB"),true),blue,4);
        var targetText=slot.GetComponentInChildren<TMP_Text>();Rect(targetText.transform,0,0,400,100);Text(targetText.transform,31,blue,true);
        ResetGroup(Find(drag,"Drags"));
        var draggables=drag.GetComponentsInChildren<DraggableItem>(true).OrderBy(x=>x.name).ToArray();
        for(int k=0;k<3;k++)Button(draggables[k].transform,blue,(k-1)*490,-172,382,104,38);
        Rect(Find(drag,"DragFeedbackText"),0,-301,1470,63);Text(Find(drag,"DragFeedbackText"),32,navy);
        var reflection=q.reflectionPanel.transform;
        Rect(Find(reflection,"ReflectionQuestionText"),0,225,1470,113);Text(Find(reflection,"ReflectionQuestionText"),44,navy,true);
        ResetGroup(Find(reflection,"Drags"));
        var reflectButtons=reflection.GetComponentsInChildren<Button>(true).OrderBy(b=>b.name).ToArray();
        for(int k=0;k<3;k++){
            Button(reflectButtons[k].transform,blue,(k-1)*490,-30,415,240,35);
            var text=reflectButtons[k].GetComponentInChildren<TMP_Text>();Rect(text.transform,0,-76,385,64);
            Face(reflectButtons[k].transform,k);
        }
        Text(Find(reflection,"DragFeedbackText"),30,navy,false,"");
        var result=q.resultPanel.transform;
        Icon(result,"Style_ResultIcon","Assets/Images/UI Flat/icon_quiz_checklist.png",0,210,150,150);
        Rect(q.resultText.transform,0,7,1380,206);Text(q.resultText.transform,62,navy);
        Button(Find(result,"RestartButton"),green,-260,-231,448,101,39,true);
        Button(Find(result,"RestartButton (1)"),navy,260,-231,448,101,39,true,"KEMBALI");
        Canvas.ForceUpdateCanvases();EditorSceneManager.MarkSceneDirty(scene);
        if(!EditorSceneManager.SaveScene(scene))throw new Exception("Save failed.");
        return "Saved all five quiz4 panels. Existing gameplay references and callbacks retained.";
    }
}
#endif
