#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class MicroScene4StyleQA
{
    static SimulationManager manager;
    static int testIndex, startFrame;
    static float startedAt;
    static bool waiting, checkedInitial, background;
    static string key;
    static readonly List<string> results = new List<string>();
    static readonly FieldInfo busy = typeof(SimulationManager).GetField("isSimulating",BindingFlags.Instance|BindingFlags.NonPublic);
    public static void Begin(string label)
    {
        if(!Application.isPlaying) throw new InvalidOperationException("Play Mode required.");
        if(manager!=null) throw new InvalidOperationException("QA is already running.");
        manager=UnityEngine.Object.FindFirstObjectByType<SimulationManager>();
        if(manager==null) throw new InvalidOperationException("Simulation missing.");
        key="Micro4QA_"+label; results.Clear();testIndex=0;waiting=false;
        background=Application.runInBackground;Application.runInBackground=true;
        results.Add("initial="+manager.microbeContainerA.childCount+"/"+manager.microbeContainerB.childCount+
            ";duration="+manager.simulationDuration+";start="+manager.startingMicrobes+";max="+manager.maxMicrobes);
        SessionState.SetString(key,"RUNNING");
        EditorApplication.update+=Tick;
    }
    static void Click(string name)
    {
        var button=manager.gameObject.scene.GetRootGameObjects().SelectMany(x=>x.GetComponentsInChildren<Button>(true)).First(x=>x.name==name);
        button.onClick.Invoke();
    }
    static void Tick()
    {
        if(!Application.isPlaying||manager==null){Finish("ABORTED");return;}
        try {
            if(!waiting) {
                bool water=testIndex<6,air=(testIndex/3)%2==0;int day=testIndex%3+1;
                Click(water?"Water_Ada_Button":"Water_Tiada_Button");
                Click(air?"Air_Cukup_Button":"Air_Kurang_Button");Click("Time_"+day+"_Button");
                Click("PlayTime_Button");Click("PlayTime_Button");
                startedAt=Time.realtimeSinceStartup;startFrame=Time.frameCount;checkedInitial=false;waiting=true;
            } else {
                if(!checkedInitial&&Time.frameCount>startFrame+2) {
                    if(manager.microbeContainerA.childCount!=3||manager.microbeContainerB.childCount!=3)
                        results.Add("FAIL initial/reentry guard at "+testIndex);
                    checkedInitial=true;
                }
                if(Time.realtimeSinceStartup-startedAt>10)throw new Exception("Simulation timed out.");
                if((bool)busy.GetValue(manager)||Time.realtimeSinceStartup-startedAt<manager.simulationDuration+.15f)return;
                int day=testIndex%3+1;bool water=testIndex<6,air=(testIndex/3)%2==0;
                int a=Mathf.Min(18,3+day*5),b=water?(air?3+day*5:3+day*2):(air?3+day:2);
                bool ok=manager.microbeContainerA.childCount==a&&manager.microbeContainerB.childCount==b&&manager.progressSlider.value==1&&manager.progressText.text=="Hari "+day;
                results.Add((ok?"PASS ":"FAIL ")+water+"/"+air+"/"+day+"="+manager.microbeContainerA.childCount+"/"+manager.microbeContainerB.childCount);
                waiting=false;testIndex++;
                if(testIndex==12){results.Add(DragChecks());Finish("DONE");}
            }
        } catch(Exception e){results.Add(e.ToString());Finish("FAILED");}
    }
    static void Finish(string state)
    {
        EditorApplication.update-=Tick;
        if(manager!=null)Application.runInBackground=background;
        manager=null;
        SessionState.SetString(key,state+"\n"+string.Join("\n",results));
    }
    public static string DragChecks()
    {
        var canvas=UnityEngine.Object.FindFirstObjectByType<SimulationManager>().waterText.canvas;
        var answers=canvas.GetComponentsInChildren<DraggableAnswer>(true);
        var slots=canvas.GetComponentsInChildren<DropSlot4>(true).OrderBy(x=>x.name).ToArray();
        var output=new List<string>();
        foreach(var slot in slots) {
            foreach(bool correct in new[]{false,true}) {
                var answer=answers.First(x=>(x.answerID==slot.correctAnswerID)==correct);
                var rt=(RectTransform)answer.transform;var parent=rt.parent;var pos=rt.anchoredPosition;
                var ev=new PointerEventData(EventSystem.current){pointerDrag=answer.gameObject,delta=new Vector2(80,30)};
                answer.OnBeginDrag(ev);var before=rt.anchoredPosition;answer.OnDrag(ev);
                bool motion=Vector2.Distance(rt.anchoredPosition-before,ev.delta/canvas.scaleFactor)<.01f;
                slot.OnDrop(ev);answer.OnEndDrag(ev);
                bool ok=motion&&(correct?rt.parent==slot.transform:rt.parent==parent&&Vector2.Distance(rt.anchoredPosition,pos)<.01f);
                output.Add((ok?"PASS":"FAIL")+" drag "+slot.name+" "+correct);
                rt.SetParent(parent,false);rt.anchoredPosition=pos;rt.localScale=Vector3.one;
            }
        }
        var outside=answers[0];var rect=(RectTransform)outside.transform;var original=rect.parent;var position=rect.anchoredPosition;
        var data=new PointerEventData(EventSystem.current){pointerDrag=outside.gameObject,delta=new Vector2(-60,20)};
        outside.OnBeginDrag(data);outside.OnDrag(data);outside.OnEndDrag(data);
        output.Add((rect.parent==original&&Vector2.Distance(rect.anchoredPosition,position)<.01f?"PASS":"FAIL")+" outside-drop");
        return string.Join("; ",output);
    }
}
#endif
