using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class Kuiz1LayoutApply
{
    static Color Ink=new Color(.02f,.29f,.08f,1), Cream=new Color(1,.98f,.87f,1), Green=new Color(.16f,.58f,.05f,1), Blue=new Color(.04f,.58f,.88f,1), Orange=new Color(1,.55f,.02f,1);
    [MenuItem("MicroFun/Apply Kuiz1")]
    public static void Apply()
    {
        Transform canvas=GameObject.Find("Canvas").transform;
        Transform content=canvas.Find("Content"), panel=content.Find("bg"), question=panel.Find("Soalan"), options=panel.Find("OptionsButtons");
        QuizController quiz=content.GetComponent<QuizController>();
        Transform title=canvas.Find("Title"), score=canvas.Find("Score"), back=canvas.Find("Back");
        Transform old=canvas.Find("QuizDecor"); if(old!=null) Object.DestroyImmediate(old.gameObject);
        Clear(content.gameObject); Clear(panel.gameObject); Clear(options.gameObject);
        Transform g1=options.Find("1"), g2=options.Find("2"); if(g1!=null)Clear(g1.gameObject); if(g2!=null)Clear(g2.gameObject);
        R(content,new Vector2(0,-22),new Vector2(1500,680)); R(panel,Vector2.zero,new Vector2(1500,680)); S(I(panel.gameObject),"Assets/Images/UI Flat/btn_white_flat.png",new Color(1,.97f,.72f,.96f));
        R(title,new Vector2(0,468),new Vector2(410,92)); S(I(title.gameObject),"Assets/Images/UI Flat/btn_white_flat.png",Cream); T(title.GetComponentInChildren<TextMeshProUGUI>(true),"KUIZ",58,Ink,TextAlignmentOptions.Center);
        R(score,new Vector2(0,256),new Vector2(450,74)); S(I(score.gameObject),"Assets/Images/UI Flat/btn_green_flat.png",Green); T(score.GetComponentInChildren<TextMeshProUGUI>(true),"Soalan 1 daripada 3",34,Color.white,TextAlignmentOptions.Center);
        R(question,new Vector2(0,165),new Vector2(1260,118)); var qi=question.GetComponent<Image>();if(qi!=null)qi.enabled=false; T(quiz.questionText.GetComponent<TextMeshProUGUI>(),"Apakah definisi mikroorganisma?",48,Ink,TextAlignmentOptions.Center);
        R(options,new Vector2(0,-75),new Vector2(1110,430));
        for(int i=0;i<quiz.buttonAnswers.Count;i++){Transform a=quiz.buttonAnswers[i];if(a==null)continue;if(i>=3){a.gameObject.SetActive(false);continue;}a.SetParent(options,false);a.gameObject.SetActive(true);R(a,new Vector2(0,95-145*i),new Vector2(1080,122));var im=I(a.gameObject);S(im,"Assets/Images/UI Flat/btn_white_flat.png",Cream);var b=a.GetComponent<Button>();if(b==null)b=a.gameObject.AddComponent<Button>();b.targetGraphic=im;var at=a.GetComponentInChildren<TextMeshProUGUI>(true);at.gameObject.name="AnswerText";R(at.transform,new Vector2(52,0),new Vector2(820,105));T(at,at.text,32,Ink,TextAlignmentOptions.MidlineLeft);Badge(a,i);Feedback(a.gameObject);}
        R(back,new Vector2(-720,-472),new Vector2(350,82));S(I(back.gameObject),"Assets/Images/UI Flat/btn_white_flat.png",Cream);var bt=back.GetComponent<Button>();bt.targetGraphic=I(back.gameObject);var bl=back.GetComponentInChildren<TextMeshProUGUI>(true);R(bl.transform,new Vector2(54,0),new Vector2(230,78));T(bl,"Laman Utama",29,Ink,TextAlignmentOptions.MidlineLeft);var hc=N("HomeCircle",back,"Assets/Images/UI Flat/btn_green_flat.png",Green);R(hc.transform,new Vector2(-126,0),new Vector2(86,86));var hi=N("HomeIcon",hc.transform,"Assets/Images/UI Flat/icon_home.png",Color.white);R(hi.transform,Vector2.zero,new Vector2(44,44));Feedback(back.gameObject);
        Transform root=new GameObject("QuizDecor",typeof(RectTransform)).transform;root.SetParent(canvas,false);root.SetAsFirstSibling();R(root,Vector2.zero,new Vector2(1920,1080)); var bg=N("WarmBackground",root,"Assets/Images/UI Flat/btn_white_flat.png",new Color(1,.85f,.51f,1));R(bg.transform,Vector2.zero,new Vector2(1920,1080));
        D("GreenMicrobe",root,"Assets/Images/UI Flat/char_green_microbe.png",new Vector2(-650,300),new Vector2(210,210));D("PurpleMicrobe",root,"Assets/Images/UI Flat/char_purple_microbe.png",new Vector2(600,300),new Vector2(200,200));D("WaterDrop",root,"Assets/Images/UI Flat/char_water_drop.png",new Vector2(790,230),new Vector2(135,185));D("Flask",root,"Assets/Images/UI Flat/icon_flask.png",new Vector2(-825,-330),new Vector2(160,245));D("PetriDish",root,"Assets/Images/UI Flat/icon_petri_dish.png",new Vector2(280,-410),new Vector2(235,165));D("MoldyBread",root,"Assets/Images/UI Flat/img_fun_bread.png",new Vector2(770,-330),new Vector2(210,220));D("Magnifier",root,"Assets/Images/UI Flat/icon_magnify_glass.png",new Vector2(-630,-265),new Vector2(135,135));
        var nx=N("SeterusnyaButton",root,"Assets/Images/UI Flat/btn_white_flat.png",Cream);R(nx.transform,new Vector2(720,-472),new Vector2(350,82));var nb=nx.gameObject.AddComponent<Button>();nb.targetGraphic=nx;Feedback(nx.gameObject);var nl=X("Label",nx.transform,"Seterusnya",29,Ink,TextAlignmentOptions.MidlineRight);R(nl.transform,new Vector2(-45,0),new Vector2(225,78));var nc=N("ArrowCircle",nx.transform,"Assets/Images/UI Flat/btn_green_flat.png",Green);R(nc.transform,new Vector2(126,0),new Vector2(86,86));var na=N("ArrowIcon",nc.transform,"Assets/Images/UI Flat/ic_arrow_thick.png",Color.white);R(na.transform,Vector2.zero,new Vector2(46,46));
        var au=N("DengarAudioButton",root,"Assets/Images/UI Flat/btn_white_flat.png",Cream);R(au.transform,new Vector2(-720,468),new Vector2(355,82));var ab=au.gameObject.AddComponent<Button>();ab.targetGraphic=au;Feedback(au.gameObject);var ac=N("SpeakerCircle",au.transform,"Assets/Images/UI Flat/btn_green_flat.png",Green);R(ac.transform,new Vector2(-126,0),new Vector2(86,86));var ai=N("SpeakerIcon",ac.transform,"Assets/Images/UI Flat/icon_speaker.png",Color.white);R(ai.transform,Vector2.zero,new Vector2(47,47));var al=X("Label",au.transform,"Dengar audio",29,Ink,TextAlignmentOptions.MidlineLeft);R(al.transform,new Vector2(52,0),new Vector2(235,78));au.gameObject.AddComponent<AudioSource>();var narr=au.gameObject.AddComponent<AudioNarrationButton>();var so=new SerializedObject(narr);so.FindProperty("narrationClip").objectReferenceValue=AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/akt1_intro_voice.mp3");so.ApplyModifiedPropertiesWithoutUndo();
        var qs=new SerializedObject(quiz);qs.FindProperty("nextButton").objectReferenceValue=nb;qs.ApplyModifiedPropertiesWithoutUndo();EditorSceneManager.MarkSceneDirty(canvas.gameObject.scene);
    }
    static void Badge(Transform p,int i){var c=i==0?Green:i==1?Blue:Orange;var b=N("LetterBadge",p,"Assets/Images/UI Flat/btn_green_flat.png",c);R(b.transform,new Vector2(-458,0),new Vector2(86,86));var l=X("LetterLabel",b.transform,((char)('A'+i)).ToString(),48,Color.white,TextAlignmentOptions.Center);R(l.transform,Vector2.zero,new Vector2(80,80));}
    static void D(string n,Transform p,string path,Vector2 pos,Vector2 size){var x=N(n,p,path,Color.white);R(x.transform,pos,size);x.raycastTarget=false;}
    static Image N(string n,Transform p,string path,Color c){var o=new GameObject(n,typeof(RectTransform),typeof(CanvasRenderer),typeof(Image));o.transform.SetParent(p,false);var x=o.GetComponent<Image>();S(x,path,c);return x;}
    static TextMeshProUGUI X(string n,Transform p,string v,float z,Color c,TextAlignmentOptions a){var o=new GameObject(n,typeof(RectTransform),typeof(CanvasRenderer),typeof(TextMeshProUGUI));o.transform.SetParent(p,false);var x=o.GetComponent<TextMeshProUGUI>();x.font=TMP_Settings.defaultFontAsset;T(x,v,z,c,a);return x;}
    static void S(Image x,string p,Color c){x.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(p);x.type=Image.Type.Sliced;x.color=c;x.raycastTarget=true;}
    static Image I(GameObject o){var x=o.GetComponent<Image>();if(x==null)x=o.AddComponent<Image>();return x;}
    static void T(TextMeshProUGUI x,string v,float z,Color c,TextAlignmentOptions a){x.text=v;x.fontSize=z;x.color=c;x.alignment=a;x.enableWordWrapping=true;x.raycastTarget=false;}
    static void R(Transform t,Vector2 p,Vector2 s){var r=t.GetComponent<RectTransform>();r.anchorMin=r.anchorMax=r.pivot=new Vector2(.5f,.5f);r.anchoredPosition=p;r.sizeDelta=s;r.localScale=Vector3.one;}
    static void Clear(GameObject o){var g=o.GetComponent<LayoutGroup>();if(g!=null)Object.DestroyImmediate(g);var f=o.GetComponent<ContentSizeFitter>();if(f!=null)Object.DestroyImmediate(f);}
    static void Feedback(GameObject o){if(o.GetComponent<ButtonInteractionFeedback>()==null)o.AddComponent<ButtonInteractionFeedback>();}
}