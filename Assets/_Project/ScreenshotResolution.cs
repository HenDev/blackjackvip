using UnityEngine;
using System.IO;
using System.Collections;

public class ScreenshotResolution : MonoBehaviour
{
    public enum Resolutionss
    {
        Res_2_3 = 0,
        Res_3_4,
        Res_4_5,
        Res_9_15,
        Res_9_16,
        Res_10_16,
        Res_11_16,
        Res_19_5_9,
        Res_19_5_9_Pro
    };

    public Camera[] cameras;
    public Resolutionss mCurrentDeviecResolution;
    public float cameraViewPortWidth = 1f;

    private int[] ArrOfResolutionFactor_Landscape = {
        Mathf.FloorToInt((2f/3f)*100),   // Res_2_3
        Mathf.FloorToInt((3f/4f)*100),   // Res_3_4
        Mathf.FloorToInt((4f/5f)*100),   // Res_4_5
        Mathf.FloorToInt((9f/15f)*100),  // Res_9_15
        Mathf.FloorToInt((9f/16f)*100),  // Res_9_16
        Mathf.FloorToInt((10f/16f)*100), // Res_10_16
        Mathf.FloorToInt((11f/16f)*100), // Res_11_16
        Mathf.FloorToInt((19.5f/9f)*100),// Res_19_5_9
        Mathf.FloorToInt((19.5f/9f)*100) // Res_19_5_9_Pro
    };

    private int[] ArrOfResolutionFactor_Portrait = {
        Mathf.FloorToInt((3f/2f)*100),
        Mathf.FloorToInt((4f/3f)*100),
        Mathf.FloorToInt((5f/4f)*100),
        Mathf.FloorToInt((15f/9f)*100),
        Mathf.FloorToInt((16f/9f)*100),
        Mathf.FloorToInt((16f/10f)*100),
        Mathf.FloorToInt((16f/11f)*100),
        Mathf.FloorToInt((9f/19.5f)*100),
        Mathf.FloorToInt((9f/19.5f)*100)
    };

    private (string name, int width, int height)[] resolutions = new (string, int, int)[]
    {
        ("22:9 - 900x2200", 900, 2200),
        ("21:9 - 900x2100", 900, 2100),
        ("20:9 - 900x2000", 900, 2000),
        ("19.5:9 - 900x1950", 900, 1950),
        ("19:9 - 900x1900", 900, 1900),
        ("18.5:9 - 900x1850", 900, 1850),
        ("18:9 - 900x1800", 900, 1800),
        ("16:9 - 1080x1920", 1080, 1920),
        ("5:3 - 900x1500", 900, 1500),
        ("16:10 - 1200x1920", 1200, 1920),
        ("3:2 - 1000x1500", 1000, 1500),
        ("10:7 - 700x1000", 700, 1000),
        ("4:3 - 1200x1600", 1200, 1600),
    };

    void Start()
    {
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(5f);
        yield return StartCoroutine(CaptureAllResolutions());
    }

    IEnumerator CaptureAllResolutions()
    {
        string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        string folderPath = Path.Combine(desktopPath, "UnityScreenshots");
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        foreach (var res in resolutions)
        {
            RenderTexture rt = new RenderTexture(res.width, res.height, 24);
            Texture2D finalScreenshot = new Texture2D(res.width, res.height, TextureFormat.RGB24, false);

            Color[] blendedPixels = new Color[res.width * res.height];

            foreach (var cam in cameras)
            {
                cam.targetTexture = rt;
                cam.Render();

                RenderTexture.active = rt;
                Texture2D temp = new Texture2D(res.width, res.height, TextureFormat.RGB24, false);
                temp.ReadPixels(new Rect(0, 0, res.width, res.height), 0, 0);
                temp.Apply();

                Color[] pixels = temp.GetPixels();
                for (int i = 0; i < blendedPixels.Length; i++)
                    blendedPixels[i] += pixels[i];

                Destroy(temp);
                cam.targetTexture = null;
            }

            for (int i = 0; i < blendedPixels.Length; i++)
                blendedPixels[i] /= cameras.Length;

            finalScreenshot.SetPixels(blendedPixels);
            finalScreenshot.Apply();

            // Detectar aspect ratio oficial y real
            Resolutionss detected = DetectCurrentDeviceResolution(res.width, res.height);
            string realAspect = GetRealAspectRatio(res.width, res.height);

            string filename = $"{res.name} - {detected} - {realAspect}.png";
            string path = Path.Combine(folderPath, filename);

            File.WriteAllBytes(path, finalScreenshot.EncodeToPNG());
            Debug.Log($"Guardado: {path} con AspectRatio {detected} ({realAspect})");

            RenderTexture.active = null;
            Destroy(rt);

            yield return null;
        }
    }

    public Resolutionss DetectCurrentDeviceResolution(int width, int height)
    {
        float screenWidthByHeightRatio = (float)width / (float)height;
        int screenWidthByHeightRatioInt = Mathf.FloorToInt(screenWidthByHeightRatio * 100);

        bool isResolutionChanged = false;
        for (int i = 0; i < ArrOfResolutionFactor_Landscape.Length; i++)
        {
            if (screenWidthByHeightRatioInt == ArrOfResolutionFactor_Landscape[i] ||
                screenWidthByHeightRatioInt == ArrOfResolutionFactor_Portrait[i])
            {
                mCurrentDeviecResolution = (Resolutionss)i;
                isResolutionChanged = true;
                break;
            }
        }

        if (!isResolutionChanged)
        {
            float minDiff = float.MaxValue;
            int closestIndex = -1;

            for (int i = 0; i < ArrOfResolutionFactor_Portrait.Length; i++)
            {
                float target = (height > width ? ArrOfResolutionFactor_Portrait[i] : ArrOfResolutionFactor_Landscape[i]);
                float diff = Mathf.Abs(screenWidthByHeightRatioInt - target);
                if (diff < minDiff)
                {
                    minDiff = diff;
                    closestIndex = i;
                }
            }

            if (closestIndex >= 0)
            {
                mCurrentDeviecResolution = (Resolutionss)closestIndex;
                cameraViewPortWidth = 0.9f;
            }
        }

        return mCurrentDeviecResolution;
    }

    private string GetRealAspectRatio(int width, int height)
    {
        int gcd = GCD(width, height);
        int aspectWidth = width / gcd;
        int aspectHeight = height / gcd;
        return $"{aspectWidth}x{aspectHeight}";
    }

    private int GCD(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }
}
