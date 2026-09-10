#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Editor-only regression harness. Never modifies gameplay code or saved state.
public static class Quiz4StyleQA
{
    static IEnumerator<float> routine;
    static double next;
    static bool originalBackground, capture;
    static string key,label;
    static QuizManager4 q;
    static readonly List<string> log=new List<string>();
    static readonly FieldInfo score=typeof(QuizManager4).GetField("score",BindingFlags.Instance|BindingFlags.NonPublic);
    static int Score(){return (int)score.GetValue(q);}
    static void Check(bool pass,string message){log.Add((pass?"PASS ":"FAIL ")+message);}
    static void Click(Button b){b.OnPointerClick(new PointerEventData(EventSystem.current){button=PointerEventData.InputButton.Left});}
    public static void Begin(string name,bool screenshots=false,int runs=3)
    {
        if(!Application.isPlaying||routine!=null)throw new Exception("Fresh Play Mode and idle QA required");
        label=name;key="Quiz4QA_"+name;capture=screenshots;log.Clear();
        originalBackground=Application.runInBackground;Application.runInBackground=true;
        routine=Run(runs);next=0;SessionState.SetString(key,"RUNNING");EditorApplication.update+=Tick;
    }
    static void Tick()
    {
        if(!Application.isPlaying){Finish("ABORTED");return;}
        if(EditorApplication.timeSinceStartup<next)return;
        try{if(!routine.MoveNext()){Finish("DONE");return;}next=EditorApplication.timeSinceStartup+routine.Current;}
        catch(Exception e){log.Add(e.ToString());Finish("FAILED");}
    }
    static void Finish(string state)
    {
        EditorApplication.update-=Tick;routine=null;Application.runInBackground=originalBackground;
        SessionState.SetString(key,state+"\n"+string.Join("\n",log));
    }
    static Button[] Choices(){return new[]{q.answerButton1,q.answerButton2,q.answerButton3};}
    static Button ButtonIn(GameObject root,string name){return root.GetComponentsInChildren<Button>(true).Single(b=>b.name==name);}
    static void Shot(string name)
    {
        if(!capture)return;
        Canvas.ForceUpdateCanvases();
        string directory=System.IO.Path.GetFullPath("Captures/Quiz4");System.IO.Directory.CreateDirectory(directory);
        ScreenCapture.CaptureScreenshot(directory+"/"+label+"_"+name+".png");
        var canvas=q.questionText.canvas;
        foreach(var b in canvas.GetComponentsInChildren<Button>().Where(b=>b.gameObject.activeInHierarchy)){
            var rt=(RectTransform)b.transform;var corners=new Vector3[4];rt.GetWorldCorners(corners);
            Check(!corners.Any(v=>v.x<0||v.y<0||v.x>Screen.width||v.y>Screen.height),"on-screen "+name+"/"+b.name);
            var ev=new PointerEventData(EventSystem.current){position=RectTransformUtility.WorldToScreenPoint(null,rt.TransformPoint(rt.rect.center))};
            var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(ev,hits);
            Check(hits.Count>0&&hits[0].gameObject.transform.IsChildOf(b.transform),"hit "+name+"/"+b.name);
        }
        foreach(var text in canvas.GetComponentsInChildren<TMP_Text>().Where(t=>t.gameObject.activeInHierarchy)){
            text.ForceMeshUpdate();Check(!text.isTextOverflowing,"text fits "+name+"/"+text.name);
        }
    }
    static void Drag(DraggableItem item,DropSlot4_4 slot,bool outside=false)
    {
        var rt=(RectTransform)item.transform;var original=rt.parent;var pos=rt.anchoredPosition;
        var canvas=item.GetComponentInParent<Canvas>();var ev=new PointerEventData(EventSystem.current){pointerDrag=item.gameObject,delta=new Vector2(81,37)};
        item.OnBeginDrag(ev);var before=rt.anchoredPosition;item.OnDrag(ev);
        Check(Vector2.Distance(rt.anchoredPosition-before,ev.delta/canvas.scaleFactor)<.01f,"drag displacement "+item.itemName);
        if(!outside)slot.OnDrop(ev);item.OnEndDrag(ev);
        if(outside||item.itemName!=slot.correctItemName)
            Check(rt.parent==original&&Vector2.Distance(rt.anchoredPosition,pos)<.1f,"return "+item.itemName);
        else Check(rt.parent==slot.transform,"accepted Water");
    }
    static IEnumerator<float> Run(int runs)
    {
        for(int run=0;run<runs;run++){
            if(run>0){SceneManager.LoadScene("quiz4");yield return .7f;}
            q=UnityEngine.Object.FindFirstObjectByType<QuizManager4>();
            Check(q.startPanel.activeSelf&&!q.quizPanel.activeSelf&&!q.dragDropPanel.activeSelf&&!q.reflectionPanel.activeSelf&&!q.resultPanel.activeSelf,"initial panels run "+run);
            if(run==0){Shot("start");yield return .35f;}
            Click(ButtonIn(q.startPanel,"StartButton"));yield return .15f;
            for(int n=0;n<3;n++){
                Check(q.quizPanel.activeSelf&&Choices().All(b=>b.interactable),"question ready "+run+"/"+n);
                if(run==0){Shot("question"+(n+1));yield return .35f;}
                int before=Score();var choice=Choices()[run%3];Click(choice);
                Check(Score()==before+(run==0?1:0),"MCQ score "+run+"/"+n);
                Check(q.feedbackText.text==(run==0?"Correct! Good job!":"Try again next time!"),"feedback "+run+"/"+n);
                Check(Choices().All(b=>!b.interactable),"answer lock "+run+"/"+n);
                Click(choice);Check(Score()==before+(run==0?1:0),"double click blocked "+run+"/"+n);
                yield return .55f;Check(q.quizPanel.activeSelf&&!Choices()[0].interactable,"feedback delay "+run+"/"+n);
                yield return .8f;
            }
            Check(q.dragDropPanel.activeSelf&&!q.quizPanel.activeSelf,"drag stage "+run);yield return .2f;
            var slot=q.dragDropPanel.GetComponentInChildren<DropSlot4_4>(true);
            var items=q.dragDropPanel.GetComponentsInChildren<DraggableItem>(true);
            if(run==0){Shot("drag");yield return .35f;}
            int previous=Score();
            Drag(items.Single(x=>x.itemName=="Rock"),slot,true);
            Drag(items.Single(x=>x.itemName=="Rock"),slot);
            Drag(items.Single(x=>x.itemName=="Plastic"),slot);
            Check(Score()==previous&&q.dragDropPanel.activeSelf,"wrong drops unscored");
            var water=items.Single(x=>x.itemName=="Water");Drag(water,slot);
            Check(Score()==previous+1,"Water score");
            slot.OnDrop(new PointerEventData(EventSystem.current){pointerDrag=water.gameObject});
            Check(Score()==previous+1,"repeat drop lock");
            yield return .55f;Check(q.dragDropPanel.activeSelf,"drag feedback delay");yield return .8f;
            Check(q.reflectionPanel.activeSelf&&!q.dragDropPanel.activeSelf,"reflection stage");
            if(run==0){Shot("reflection");yield return .35f;}
            var reflection=q.reflectionPanel.GetComponentsInChildren<Button>().OrderBy(b=>b.name).ToArray();
            Click(reflection[run%3]);yield return .15f;
            Check(q.resultPanel.activeSelf&&Score()==(run==0?4:1)&&q.resultText.text=="Great job!\nYour score: "+(run==0?4:1)+" / 4","result run "+run);
            if(run==0){Shot("result");yield return .35f;
                Click(ButtonIn(q.resultPanel,"RestartButton"));yield return .15f;
                Check(Score()==0&&q.quizPanel.activeSelf,"restart score reset");
                for(int n=0;n<3;n++){Click(q.answerButton1);yield return 1.35f;}
                bool locked=(bool)typeof(DropSlot4_4).GetField("alreadyAnswered",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(slot);
                int beforeRetry=Score();slot.OnDrop(new PointerEventData(EventSystem.current){pointerDrag=water.gameObject});
                yield return 1.4f;
                log.Add(locked&&Score()==beforeRetry&&q.dragDropPanel.activeSelf?"KNOWN ISSUE: restart retains answered slot and cannot complete second drag stage":"Restart behaviour differs: inspect.");
            }
        }
    }
}
#endif

