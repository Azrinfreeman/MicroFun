using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class Akt2AssetProcessor
{
    const string Folder = "Assets/Images/Generated/Akt2";
    const string StaticSource = "Assets/Images/Generated/Akt2SceneBase.png";
    const string NavSource = Folder + "/NavSheetRaw.png";
    const string Backdrop = Folder + "/Backdrop.png";

    [MenuItem("MicroFun/Process AKT2 Interactive Assets")]
    public static void Process()
    {
        MakeReadable(StaticSource);
        MakeReadable(NavSource);
        ExtractMenuButtons();
        ExtractCurriculumPanel();

        ExtractNavButtons();
        ConfigureSprite(Backdrop);
        AssetDatabase.Refresh();
        Debug.Log("AKT2 interactive button assets processed.");
    }

    static void ExtractMenuButtons()
    {
        var source = AssetDatabase.LoadAssetAtPath<Texture2D>(StaticSource);
        if (source == null) throw new Exception("AKT2 static source artwork is missing.");

        string[] names = { "Pengenalan", "Definisi", "Jenis", "Elaborasi", "Kuiz" };
        int[] top = { 110, 231, 351, 468, 584 };
        const int x = 478;
        const int width = 735;
        const int height = 122;

        for (int i = 0; i < names.Length; i++)
        {
            int y = source.height - top[i] - height;
            Color32[] pixels = source.GetPixels32();
            var crop = new Color32[width * height];

            for (int cy = 0; cy < height; cy++)
            {
                for (int cx = 0; cx < width; cx++)
                {
                    var color = pixels[(y + cy) * source.width + (x + cx)];
                    float half = height * 0.5f;
                    float left = half;
                    float right = width - half;
                    float nearestX = Mathf.Clamp(cx + 0.5f, left, right);
                    float dx = cx + 0.5f - nearestX;
                    float dy = cy + 0.5f - half;
                    float distance = Mathf.Sqrt(dx * dx + dy * dy);
                    float alpha = Mathf.Clamp01((half - 2f - distance) / 2.5f);
                    color.a = (byte)Mathf.RoundToInt(alpha * 255f);
                    crop[cy * width + cx] = color;
                }
            }

            SaveTexture(crop, width, height, Folder + "/Menu_" + names[i] + ".png");
        }
    }

static void ExtractCurriculumPanel()
    {
        var source = AssetDatabase.LoadAssetAtPath<Texture2D>(StaticSource);
        Color32[] sourcePixels = source.GetPixels32();
        const int x = 458;
        const int top = 705;
        const int width = 776;
        const int height = 154;
        int y = source.height - top - height;
        var crop = new Color32[width * height];

        for (int cy = 0; cy < height; cy++)
        {
            for (int cx = 0; cx < width; cx++)
            {
                var color = sourcePixels[(y + cy) * source.width + x + cx];
                float half = height * .5f;
                float nearestX = Mathf.Clamp(cx + .5f, half, width - half);
                float dx = cx + .5f - nearestX;
                float dy = cy + .5f - half;
                float distance = Mathf.Sqrt(dx * dx + dy * dy);
                float alpha = Mathf.Clamp01((half - 3f - distance) / 2.5f);
                color.a = (byte)Mathf.RoundToInt(alpha * 255f);
                crop[cy * width + cx] = color;
            }
        }

        SaveTexture(crop, width, height, Folder + "/CurriculumPanel.png");
    }


    static void ExtractNavButtons()
    {
        var source = AssetDatabase.LoadAssetAtPath<Texture2D>(NavSource);
        if (source == null) throw new Exception("AKT2 navigation sheet is missing.");
        Color32[] all = source.GetPixels32();
        string[] names = { "Home", "Back", "Next" };

        for (int i = 0; i < 3; i++)
        {
            int sectionStart = Mathf.FloorToInt(source.width * i / 3f);
            int sectionEnd = Mathf.FloorToInt(source.width * (i + 1) / 3f);
            int minX = sectionEnd, minY = source.height, maxX = sectionStart, maxY = 0;

            for (int y = 0; y < source.height; y++)
            {
                for (int x = sectionStart; x < sectionEnd; x++)
                {
                    Color32 c = all[y * source.width + x];
                    bool blackBackdrop = c.r < 18 && c.g < 18 && c.b < 18;
                    if (blackBackdrop) c.a = 0;
                    all[y * source.width + x] = c;
                    if (c.a > 12)
                    {
                        minX = Mathf.Min(minX, x); maxX = Mathf.Max(maxX, x);
                        minY = Mathf.Min(minY, y); maxY = Mathf.Max(maxY, y);
                    }
                }
            }

            if (maxX <= minX || maxY <= minY)
                throw new Exception("Could not isolate navigation asset " + names[i]);

            const int padding = 8;
            minX = Mathf.Max(sectionStart, minX - padding);
            minY = Mathf.Max(0, minY - padding);
            maxX = Mathf.Min(sectionEnd - 1, maxX + padding);
            maxY = Mathf.Min(source.height - 1, maxY + padding);
            int width = maxX - minX + 1;
            int height = maxY - minY + 1;
            var crop = new Color32[width * height];

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    crop[y * width + x] = all[(minY + y) * source.width + minX + x];

            SaveTexture(crop, width, height, Folder + "/Nav_" + names[i] + ".png");
        }
    }

    static void SaveTexture(Color32[] pixels, int width, int height, string assetPath)
    {
        var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.SetPixels32(pixels);
        texture.Apply();
        string absolute = Path.Combine(Directory.GetParent(Application.dataPath).FullName, assetPath);
        File.WriteAllBytes(absolute, texture.EncodeToPNG());
        UnityEngine.Object.DestroyImmediate(texture);
        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        ConfigureSprite(assetPath);
    }

    static void MakeReadable(string assetPath)
    {
        var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer == null) return;
        importer.isReadable = true;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.SaveAndReimport();
    }

    static void ConfigureSprite(string assetPath)
    {
        var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer == null) return;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.textureCompression = TextureImporterCompression.CompressedHQ;
        importer.SaveAndReimport();
    }
}