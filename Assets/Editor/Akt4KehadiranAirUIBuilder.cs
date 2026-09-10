#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using TMPro;

public static class Akt4KehadiranAirUIBuilder
{
    const string Dir = "Assets/Images/Akt4KehadiranAirUI/";
    const string Icons = "Assets/Images/UI Flat/";
    static TMP_FontAsset font;
    static Sprite rounded;

    [MenuItem("Tools/MicroFun/Activity 4/Build Kehadiran Air UI")]
    public static void BuildUI()
    {
        var scene = EditorSceneManager.GetActiveScene();
        if (scene.name != "akt4" || Application.isPlaying) throw new InvalidOperationException("Open akt4 in Edit Mode.");
        var canvas = scene.GetRootGameObjects().First(x => x.name == "Canvas").transform;
        var page = canvas.Find("Kehadiran Air");
        var source = page.Find("Back").GetComponent<Button>();
        if (source.onClick.GetPersistentEventCount() != 2) throw new InvalidOperationException("Check legacy Back callbacks.");
        font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/FredokaOne SDF.asset");
        if (font == null || AssetDatabase.LoadAssetAtPath<Texture2D>(Dir + "air_characters_bread_v2.png") == null)
            throw new InvalidOperationException("Required font/art missing.");
        string backup = "Assets/Scenes/Aktivity/akt4/akt4_before_air_" + DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + ".unity";
        if (!EditorSceneManager.SaveScene(scene, backup, true)) throw new InvalidOperationException("Backup failed.");
        PrepareAssets();
        var design = Rect("KehadiranAirReferenceRoot_New", page, 0, 0, 1920, 1080);
        design.gameObject.AddComponent<KehadiranAirLayout>();
        try
        {
            Art("Background", design, Dir + "air_warm_gradient.asset", 0, 0, 1920, 1080, false);
            var header = Rect("Header", design, 0, 0, 1920, 1080);
            Panel("Banner", header, 0, 428, 1120, 155, "8AD400");
            Text("Title", header, "KEHADIRAN AIR", 79, 0, 430, 1010, 110, "164900", true);
            Panel("SubtitleStrip", header, 0, 325, 1100, 64, "FFF4BD");
            Text("Subtitle", header, "Air mempengaruhi pertumbuhan mikroorganisma.", 30, 0, 325, 1070, 55, "205220", true);
            Art("WaterDrop", header, Icons + "char_water_drop.png", -556, 433, 140, 155);
            Art("Magnifier", header, Icons + "icon_magnify_glass.png", 581, 425, 160, 170);
            var cards = Rect("SpeechCards", design, 0, 0, 1920, 1080);
            Speech(cards, 1, 179, 202, "128DCF", "Mengapakah roti yang telah dikeringkan ini tahan lebih lama daripada roti biasa? Adakah ketiadaan <color=#007BE8>air</color> menyebabkan mikroorganisma tidak tumbuh?", "Boy");
            Speech(cards, 2, -43, 200, "EE528A", "Saya juga berfikir begitu. Roti biasa mempunyai <color=#EB3E83>kelembapan</color> yang menyebabkan roti tersebut mudah ditumbuhi <color=#9224CF>kulat</color>.", "Girl");
            Speech(cards, 3, -279, 245, "62B62D", "Mari kita lakukan penyiasatan dengan menggunakan roti kering. Kita gunakan dua keping roti kering yang dilabelkan <color=#E26013>Roti A</color> dan Roti B, tetapi hanya <color=#007BE8>Roti B</color> telah dicelup ke dalam <color=#007BE8>air</color>.", "Teacher");
            var biscuit = Rect("BiscuitIllustration", design, 0, 0, 1920, 1080);
            Art("Jar", biscuit, Dir + "JarFramed.asset", 525, 185, 225, 248);
            Text("JarLabel", biscuit, "Biskut\nRoti\nMarjerin", 21, 526, 185, 110, 78, "AE250B", true);
            Art("GreenMicrobe", biscuit, Icons + "char_green_microbe.png", 258, 168, 134, 134);
            Art("PurpleMicrobe", biscuit, Icons + "char_purple_microbe.png", 778, 143, 123, 123);
            var comparison = Rect("BreadComparison", design, 0, 0, 1920, 1080);
            Bread(comparison, "A", 275, "FF9913", "FFF0C8", "DryBread", "Roti A: <color=#E6600A>tanpa air</color>");
            Bread(comparison, "B", 733, "0A9BDF", "EDF9FC", "WetBread", "Roti B: <color=#007CB5>dicelup ke dalam air</color>");
            for (int i = 0; i < 10; i++) Panel("Divider" + i, comparison, 504, -5 - i * 29, 3, 13, "B99146", false);
            var footer = Rect("InquiryFooter", design, 0, 0, 1920, 1080);
            Panel("InquiryBorder", footer, 0, -463, 1570, 132, "8D48BC");
            Panel("InquiryPaper", footer, 187, -463, 1180, 120, "FFFDF5", false);
            Art("Clipboard", footer, Icons + "icon_quiz_checklist.png", -706, -463, 108, 112);
            Text("InquiryTitle", footer, "Tugasan\nInkuiri", 32, -532, -463, 215, 101, "FFFFFF", true);
            Text("InquiryText", footer, "Berdasarkan bahan yang dicadangkan, rancang dan jalankan eksperimen untuk menyiasat sama ada kehadiran <color=#007BE8>air</color> mempengaruhi pertumbuhan <color=#9224CF>mikroorganisma</color>.", 28, 192, -463, 1110, 110);
            var deco = Rect("Decorations", design, 0, 0, 1920, 1080);
            foreach (var v in new[] {new Vector2(-897,330),new Vector2(-840,420),new Vector2(879,363),new Vector2(849,472),new Vector2(918,-395),new Vector2(-893,-451)})
            {
                var bubble = Art("Bubble", deco, Icons + "ui_circle_white.png", v.x, v.y, 35, 35);
                bubble.color = new Color(1,1,1,.32f);
            }
            Art("FooterMicrobe", deco, Icons + "char_green_microbe.png", 856, -465, 110, 115);
            var back = Panel("BackButton", design, -803, 477, 245, 76, "FFAB21");
            back.raycastTarget = true;
            var button = back.gameObject.AddComponent<Button>();
            button.targetGraphic = back;
            var colors = button.colors;
            colors.highlightedColor = new Color(1,.94f,.77f);
            colors.pressedColor = new Color(.82f,.76f,.65f);
            button.colors = colors;
            Art("Arrow", back.transform, Icons + "icon_arrow_left.png", -84, 0, 38, 38);
            Text("Label", back.transform, "KEMBALI", 29, 24, 0, 170, 62, "FFFFFF", true);
            var relay = back.gameObject.AddComponent<CenterMenuButtonRelay>();
            var serialized = new SerializedObject(relay);
            serialized.FindProperty("sourceButton").objectReferenceValue = source;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            UnityEventTools.AddPersistentListener(button.onClick, relay.InvokeSource);
            if (design.GetComponentsInChildren<TextMeshProUGUI>().Length < 12) throw new InvalidOperationException("Incomplete page.");
            var old = page.Find("KehadiranAirReferenceRoot");
            if (old != null) UnityEngine.Object.DestroyImmediate(old.gameObject);
            foreach (string name in new[] {"Background_Redesign","Title_Redesign","Back_Redesign","Top","Back","Title","Content","DialogParentRight","DialogParentLeft","DialogParentRight (1)","DialogParentLeft (1)"})
            {
                var child = page.Find(name);
                if (child != null) child.gameObject.SetActive(false);
            }
            design.name = "KehadiranAirReferenceRoot";
            design.SetAsLastSibling();
            EditorSceneManager.MarkSceneDirty(scene);
            if (!EditorSceneManager.SaveScene(scene)) throw new InvalidOperationException("Save failed.");
            Debug.Log("Kehadiran Air rebuilt. Backup: " + backup);
        }
        catch { UnityEngine.Object.DestroyImmediate(design.gameObject); throw; }
    }

    static void Speech(Transform parent, int number, float y, float height, string color, string copy, string character)
    {
        Panel("Speech" + number, parent, -245, y, 608, height, color);
        Panel("Paper" + number, parent, -245, y, 598, height-10, "FFFCF3", false);
        var portrait = Panel(character + "Portrait", parent, -719, y, 229, height, "FFFCF3");
        Art(character, portrait.transform, Dir + character + "Framed.asset", 0, 0, 214, height - 10);
        var badge = Art("NumberBadge" + number, parent, Icons + "ui_circle_white.png", -545, y + height/2 - 26, 67, 67);
        badge.color = Hex(color);
        Text("Number" + number, parent, number.ToString(), 40, -545, y + height/2 - 26, 62, 62, "FFFFFF", true);
        Text("SpeechText" + number, parent, copy, 28, -229, y, 539, height - 28);
    }
    static void Bread(Transform parent, string id, float x, string accent, string paper, string art, string caption)
    {
        Panel("Roti" + id + "Card", parent, x, -176, 413, 302, accent);
        Panel("Roti" + id + "Paper", parent, x, -176, 403, 292, "FFFCF3", false);
        Art("Roti" + id + "Art", parent, Dir + art + "Framed.asset", x, -163, 398, 260);
        Panel("Roti" + id + "Tab", parent, x, -25, 200, 62, accent);
        Text("Roti" + id + "Label", parent, "ROTI " + id, 35, x, -25, 190, 58, "FFFFFF", true);
        Panel("Roti" + id + "CaptionPlate", parent, x, -330, 413, 62, "FFFCF2");
        Text("Roti" + id + "Caption", parent, caption, id == "A" ? 29 : 25, x, -330, 398, 58, "152540", true);
    }
    static RectTransform Rect(string name, Transform parent, float x, float y, float w, float h)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.layer = parent.gameObject.layer;
        var r = (RectTransform)go.transform;
        r.SetParent(parent,false); r.anchorMin = r.anchorMax = r.pivot = Vector2.one * .5f;
        r.sizeDelta = new Vector2(w,h); r.anchoredPosition = new Vector2(x,y);
        return r;
    }
    static Image Art(string name, Transform parent, string path, float x, float y, float w, float h, bool aspect = true)
    {
        var i = Rect(name,parent,x,y,w,h).gameObject.AddComponent<Image>();
        i.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (i.sprite == null) throw new InvalidOperationException("Missing sprite: " + path);
        i.preserveAspect = aspect; i.raycastTarget = false; return i;
    }
    static Image Panel(string name, Transform parent, float x, float y, float w, float h, string color, bool edge = true)
    {
        var i = Rect(name,parent,x,y,w,h).gameObject.AddComponent<Image>();
        i.sprite = rounded; i.type = Image.Type.Sliced; i.color = Hex(color); i.raycastTarget = false;
        if (edge) {
            var s = i.gameObject.AddComponent<Shadow>(); s.effectColor = new Color(.25f,.16f,.04f,.2f); s.effectDistance = new Vector2(0,-5);
            var o = i.gameObject.AddComponent<Outline>(); o.effectColor = Color.white; o.effectDistance = new Vector2(2,-2);
        }
        return i;
    }
    static TextMeshProUGUI Text(string name, Transform parent, string copy, float size, float x, float y, float w, float h, string color = "152540", bool center = false)
    {
        var t = Rect(name,parent,x,y,w,h).gameObject.AddComponent<TextMeshProUGUI>();
        t.font = font; t.text = copy; t.fontSize = size; t.color = Hex(color); t.richText = true;
        t.alignment = center ? TextAlignmentOptions.Center : TextAlignmentOptions.MidlineLeft;
        t.textWrappingMode = TextWrappingModes.Normal; t.overflowMode = TextOverflowModes.Overflow;
        t.raycastTarget = false;
        if (name == "Title") { t.outlineColor=Color.white; t.outlineWidth=.18f; }
        return t;
    }
    static Color Hex(string value) { Color c; ColorUtility.TryParseHtmlString("#" + value, out c); return c; }
    static void PrepareAssets()
    {
        string roundedPath = Dir + "air_rounded.asset";
        rounded = AssetDatabase.LoadAssetAtPath<Sprite>(roundedPath);
        if (rounded == null)
        {
            var texture = new Texture2D(64,64,TextureFormat.RGBA32,false); texture.name="AirRoundedTexture";
            for(int y=0;y<64;y++) for(int x=0;x<64;x++) {
                float dx=Mathf.Max(Mathf.Abs(x-31.5f)-12,0),dy=Mathf.Max(Mathf.Abs(y-31.5f)-12,0);
                texture.SetPixel(x,y,new Color(1,1,1,Mathf.Clamp01(19.5f-Mathf.Sqrt(dx*dx+dy*dy))));
            }
            texture.Apply(); AssetDatabase.CreateAsset(texture,Dir+"air_rounded_texture.asset");
            rounded=Sprite.Create(texture,new Rect(0,0,64,64),Vector2.one*.5f,100,0,SpriteMeshType.FullRect,new Vector4(25,25,25,25));
            AssetDatabase.CreateAsset(rounded,roundedPath);
        }
        if (AssetDatabase.LoadAssetAtPath<Sprite>(Dir+"air_warm_gradient.asset") == null) {
            var texture=new Texture2D(256,144,TextureFormat.RGBA32,false); texture.name="AirWarmGradient";
            for(int y=0;y<144;y++) for(int x=0;x<256;x++) {
                float u=x/255f,v=y/143f;
                var c=u<.55f ? Color.Lerp(Hex("C6E68D"),Hex("FFF3BD"),u/.55f) : Color.Lerp(Hex("FFF3BD"),Hex("FFBA95"),(u-.55f)/.45f);
                texture.SetPixel(x,y,Color.Lerp(c,Hex("FFF9D3"),v*.25f));
            }
            texture.Apply(); AssetDatabase.CreateAsset(texture,Dir+"air_gradient_texture.asset");
            AssetDatabase.CreateAsset(Sprite.Create(texture,new Rect(0,0,256,144),Vector2.one*.5f),Dir+"air_warm_gradient.asset");
        }
        var importer=(TextureImporter)AssetImporter.GetAtPath(Dir+"air_characters_bread_v2.png");
        if(importer.npotScale != TextureImporterNPOTScale.None || importer.mipmapEnabled) {
            importer.npotScale=TextureImporterNPOTScale.None;
            importer.mipmapEnabled=false;
            importer.maxTextureSize=2048;
            importer.wrapMode=TextureWrapMode.Clamp;
            importer.SaveAndReimport();
        }
        var atlas=AssetDatabase.LoadAssetAtPath<Texture2D>(Dir+"air_characters_bread_v2.png");
        string[] names={"Boy","Girl","Teacher","Jar","DryBread","WetBread"};
        var regions = new[] {new Rect(90,524,365,500),new Rect(522,516,412,465),new Rect(1030,514,470,504),new Rect(90,25,360,483),new Rect(520,85,490,350),new Rect(1032,80,490,350)};
        for(int i=0;i<6;i++) {
            string path=Dir+names[i]+"Framed.asset";
            if(AssetDatabase.LoadAssetAtPath<Sprite>(path)!=null) continue;
            var rect=regions[i];
            rect.x*=atlas.width/1536f; rect.width*=atlas.width/1536f;
            rect.y*=atlas.height/1024f; rect.height*=atlas.height/1024f;
            AssetDatabase.CreateAsset(Sprite.Create(atlas,rect,Vector2.one*.5f,100,0,SpriteMeshType.FullRect),path);
        }
        AssetDatabase.SaveAssets();
    }
}
#endif
