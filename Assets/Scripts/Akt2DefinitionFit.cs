using UnityEngine;

// Presentation-only fitting; never changes the shared CanvasScaler.
[ExecuteAlways]
[DisallowMultipleComponent]
public class Akt2DefinitionFit : MonoBehaviour
{
    public RectTransform background;
    public RectTransform modalShield;
    void OnEnable(){Fit();}
    void LateUpdate(){Fit();}
    void Fit()
    {
        var rect=transform as RectTransform;
        var canvas=GetComponentInParent<Canvas>();
        if(!rect||!canvas)return;
        var size=((RectTransform)canvas.transform).rect.size;
        float scale=Mathf.Min(size.x/1920f,size.y/1080f);
        if(scale<=0)return;
        var desired=Vector3.one*scale;
        if((rect.localScale-desired).sqrMagnitude>.000001f)rect.localScale=desired;
        if(background){
            var bgSize=size/scale;
            if((background.sizeDelta-bgSize).sqrMagnitude>.01f)background.sizeDelta=bgSize;
            if(modalShield&&(modalShield.sizeDelta-bgSize).sqrMagnitude>.01f)modalShield.sizeDelta=bgSize;
        }
    }
}
