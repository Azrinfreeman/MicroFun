using UnityEngine;
using UnityEngine.UI;

/// <summary>Resolution independent science welcome-screen symbols, without font glyph dependencies.</summary>
[AddComponentMenu("UI/Start Scene Icon")]
[RequireComponent(typeof(CanvasRenderer))]
public class StartSceneIcon : MaskableGraphic
{
    public enum Symbol { Person, Cap, Speaker, Clipboard, Play }
    public Symbol symbol;
    VertexHelper mesh;
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear(); mesh = vh;
        switch (symbol)
        {
            case Symbol.Person:
                Circle(.5f, .74f, .205f);
                Polygon(new Vector2(.10f,.08f),new Vector2(.90f,.08f),new Vector2(.87f,.32f),new Vector2(.74f,.45f),new Vector2(.26f,.45f),new Vector2(.13f,.32f));
                break;
            case Symbol.Cap:
                Polygon(new Vector2(.03f,.63f),new Vector2(.5f,.90f),new Vector2(.97f,.63f),new Vector2(.5f,.37f));
                Polygon(new Vector2(.23f,.43f),new Vector2(.5f,.29f),new Vector2(.77f,.43f),new Vector2(.77f,.23f),new Vector2(.5f,.12f),new Vector2(.23f,.23f));
                Line(.08f,.62f,.08f,.25f,.045f); Circle(.08f,.20f,.05f); break;
            case Symbol.Speaker:
                Polygon(new Vector2(.06f,.34f),new Vector2(.25f,.34f),new Vector2(.49f,.12f),new Vector2(.49f,.88f),new Vector2(.25f,.66f),new Vector2(.06f,.66f));
                Arc(.38f,.5f,.34f,-.72f,.72f,.05f); Arc(.38f,.5f,.53f,-.78f,.78f,.05f); break;
            case Symbol.Clipboard:
                Line(.16f,.08f,.16f,.83f,.065f); Line(.16f,.08f,.85f,.08f,.065f); Line(.85f,.08f,.85f,.83f,.065f); Line(.16f,.83f,.85f,.83f,.065f);
                Polygon(new Vector2(.33f,.76f),new Vector2(.68f,.76f),new Vector2(.68f,.98f),new Vector2(.33f,.98f));
                for (int i=0;i<3;i++) { Circle(.30f,.61f-i*.17f,.035f); Line(.42f,.61f-i*.17f,.70f,.61f-i*.17f,.045f); } break;
            case Symbol.Play: Polygon(new Vector2(.14f,.04f),new Vector2(.92f,.50f),new Vector2(.14f,.96f)); break;
        }
    }
    void Polygon(params Vector2[] points)
    {
        int start = mesh.currentVertCount; var rect = GetPixelAdjustedRect();
        foreach(var p in points) mesh.AddVert(new Vector3(rect.xMin+p.x*rect.width,rect.yMin+p.y*rect.height,0),color,Vector2.zero);
        for(int i=1;i<points.Length-1;i++) mesh.AddTriangle(start,start+i,start+i+1);
    }
    void Circle(float x,float y,float r)
    {
        var p = new Vector2[40]; for(int i=0;i<p.Length;i++){float a=i*Mathf.PI*2/p.Length;p[i]=new Vector2(x+Mathf.Cos(a)*r,y+Mathf.Sin(a)*r);} Polygon(p);
    }
    void Line(float x,float y,float x2,float y2,float width)
    {
        var a=new Vector2(x,y); var b=new Vector2(x2,y2);var dir=(b-a).normalized;var n=new Vector2(-dir.y,dir.x)*width/2;
        Polygon(a+n,b+n,b-n,a-n);
    }
    void Arc(float x,float y,float radius,float from,float to,float width)
    {
        for(int i=0;i<24;i++){float a=Mathf.Lerp(from,to,i/24f);float b=Mathf.Lerp(from,to,(i+1)/24f);Line(x+Mathf.Cos(a)*radius,y+Mathf.Sin(a)*radius,x+Mathf.Cos(b)*radius,y+Mathf.Sin(b)*radius,width);}
    }
}
