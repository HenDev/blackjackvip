using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public enum MyDeviceResolutions
{
    Res_22_9,
    Res_21_9,
    Res_20_9,
    Res_19_5_9,
    Res_19_9,
    Res_18_5_9,
    Res_18_9,
    Res_16_9,
    Res_5_3,
    Res_16_10,
    Res_3_2,
    Res_10_7,
    Res_4_3
}

public class ResolutionData
{
    public float RF { get; set; }     // Resolution Factor
    public float Ratio { get; set; }  // Ratio para ajustar cámara
}

public class MultiplePlatformPortingHandler : MonoBehaviour
{
    private static MultiplePlatformPortingHandler mInstance = null;

    public static MultiplePlatformPortingHandler Instance
    {
        get { return mInstance; }
    }
    private Dictionary<MyDeviceResolutions, ResolutionData> PortraitResolutions =
        new Dictionary<MyDeviceResolutions, ResolutionData>
        {
            { MyDeviceResolutions.Res_22_9,   new ResolutionData { RF = 0.41f, Ratio = 0.833f } },
            { MyDeviceResolutions.Res_21_9,   new ResolutionData { RF = 0.42f, Ratio = 0.873f } },
            { MyDeviceResolutions.Res_20_9,   new ResolutionData { RF = 0.45f, Ratio = 0.913f } },
            { MyDeviceResolutions.Res_19_5_9, new ResolutionData { RF = 0.46f, Ratio = 0.933f } },
            { MyDeviceResolutions.Res_19_9,   new ResolutionData { RF = 0.47f, Ratio = 0.953f } },
            { MyDeviceResolutions.Res_18_5_9, new ResolutionData { RF = 0.48f, Ratio = 0.967f } },
            { MyDeviceResolutions.Res_18_9,   new ResolutionData { RF = 0.50f, Ratio = 1.013f } },
            { MyDeviceResolutions.Res_16_9,   new ResolutionData { RF = 0.56f, Ratio = 1.132f } },
            { MyDeviceResolutions.Res_5_3,    new ResolutionData { RF = 0.60f, Ratio = 1.212f } },
            { MyDeviceResolutions.Res_16_10,  new ResolutionData { RF = 0.63f, Ratio = 1.263f   } },
            { MyDeviceResolutions.Res_3_2,    new ResolutionData { RF = 0.66f, Ratio = 1.333f   } },
            { MyDeviceResolutions.Res_10_7,   new ResolutionData { RF = 0.70f, Ratio = 1.428f } },
            { MyDeviceResolutions.Res_4_3,    new ResolutionData { RF = 0.75f, Ratio = 1.5f } }
        };
   
    private string currentLoadedLevel = "";
    private MyDeviceResolutions mCurrentDeviecResolution = MyDeviceResolutions.Res_4_3;
    private float cameraViewPortWidth = 1.0f;
    private float cameraViewPortXPos = 0.0f;
    private float screenTop;
    private float screenBottom;

    public float ScreenTop
    {
        get { return screenTop; }
    }

    public float ScreenBottom
    {
        get { return screenBottom; }
    }
    public void CalculateScreenTopAndBottom(Camera cam)
    {
  
        float distanceFromCamera = Mathf.Abs(cam.transform.position.z - transform.position.z);

        Vector3 bottomPoint = cam.ScreenToWorldPoint(
            new Vector3(Screen.width / 2f, 0f, distanceFromCamera)
        );

        Vector3 topPoint = cam.ScreenToWorldPoint(
            new Vector3(Screen.width / 2f, Screen.height, distanceFromCamera)
        );

        screenBottom = bottomPoint.y;
        screenTop = topPoint.y;
        Debug.Log($"Screen Top: {screenTop} - Screen Bottom: {screenBottom}");
    }
    
    public MyDeviceResolutions CurrentLoadedDeviceResolution
    {
        get { return mCurrentDeviecResolution; }
    }

    public int GameWidth = 1024;

    void Awake()
    {
        if (mInstance != null && mInstance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            DontDestroyOnLoad(this);
            mInstance = this;
            DetectCurrentDeviceResolution();
        }
    }
 
    void Update()
    {
        if (currentLoadedLevel != SceneManager.GetActiveScene().name)
        {
            MakeAdjustmentsForTheCurrentResolution();
        }
    }

    void MakeAdjustmentsForTheCurrentResolution()
    {
        currentLoadedLevel = SceneManager.GetActiveScene().name;

        float factorToAdjust =  PortraitResolutions[mCurrentDeviecResolution].Ratio; 
        float cameraSize = Mathf.Floor(((GameWidth / 32.0f) / factorToAdjust) * 100f) / 100f;
        Debug.Log($"FactorToAdjust {factorToAdjust} gameWidth {GameWidth}");
        Debug.Log($"Adjust Camera for {currentLoadedLevel} - Camera {cameraSize}");

        Camera[] arrCameras = Camera.allCameras;
        foreach (Camera camera in arrCameras)
        {
            if (camera.orthographic)
            {
                camera.orthographicSize = cameraSize;
            }
        }
        CalculateScreenTopAndBottom(arrCameras[0]);
    }

    void DetectCurrentDeviceResolution()
    {
        float screenWidthByHeightRatio = (float)Screen.width / (float)Screen.height;
        int screenWidthByHeightRatioInt = Mathf.FloorToInt(screenWidthByHeightRatio * 100);

        int gcd = GCD(Screen.width, Screen.height);
        int aspectWidth = Screen.width / gcd;
        int aspectHeight = Screen.height / gcd;

        float aspectRatioRatio = (float)aspectWidth / (float)aspectHeight;
        Debug.Log($"AspectRatio: {aspectWidth}:{aspectHeight} - r: {aspectRatioRatio}");

        bool isResolutionChanged = false;

        // Recorremos el diccionario en lugar de arrays
        foreach (var kvp in PortraitResolutions)
        {
            var resolutionKey = kvp.Key;
            var data = kvp.Value;

            // Comparamos con el RF almacenado
            if (Mathf.FloorToInt(data.RF * 100) == screenWidthByHeightRatioInt)
            {
                mCurrentDeviecResolution = resolutionKey;
                isResolutionChanged = true;
                break;
            }
        }

        if (!isResolutionChanged)
        {
            // Si no hay coincidencia exacta, buscamos la más cercana
            float minDiff = float.MaxValue;
            MyDeviceResolutions closestResolution = MyDeviceResolutions.Res_16_9;

            foreach (var kvp in PortraitResolutions)
            {
                float diff = Mathf.Abs(screenWidthByHeightRatioInt - Mathf.FloorToInt(kvp.Value.RF * 100));
                if (diff < minDiff)
                {
                    minDiff = diff;
                    closestResolution = kvp.Key;
                }
            }

            mCurrentDeviecResolution = closestResolution;
            cameraViewPortWidth = 0.9f;
        }

        Debug.Log("mCurrentDeviecResolution : " + mCurrentDeviecResolution);

        if (GetComponentInChildren<TextMesh>() != null)
            GetComponentInChildren<TextMesh>().text = Screen.width + "\n" + Screen.height + "\n" + mCurrentDeviecResolution;
    }

    /*
    void DetectCurrentDeviceResolution()
    {
        float screenWidthByHeightRatio = (float)Screen.width / (float)Screen.height;
        int screenWidthByHeightRatioInt = Mathf.FloorToInt(screenWidthByHeightRatio * 100);
       
        int gcd = GCD(Screen.width, Screen.height);
        int aspectWidth = Screen.width / gcd;
        int aspectHeight = Screen.height / gcd;
    
        float aspectRatioRatio = (float)aspectWidth / (float)aspectHeight;
        Debug.Log($"AspectRatio: {aspectWidth}:{aspectHeight} - r: {aspectRatioRatio}");

        bool isResolutionChanged = false;
        for (int i = 0; i < ArrOfResolutionFactor_Landscape.Length; i++)
            if (screenWidthByHeightRatioInt == ArrOfResolutionFactor_Landscape[i] ||
                screenWidthByHeightRatioInt == ArrOfResolutionFactor_Portrait[i])
            {
                mCurrentDeviecResolution = (MyDeviceResolutions)i;
                isResolutionChanged = true;
                break;
            }

        if (!isResolutionChanged)
        {
            ArrayList arrResolutionDiff = new ArrayList();
            float[] tempArr = new float[ArrOfResolutionFactor_Portrait.Length];
            for (int i = 0; i < ArrOfResolutionFactor_Portrait.Length; i++)
            {
                float value_ = Mathf.Abs(screenWidthByHeightRatioInt -
                                         (Screen.height > Screen.width
                                             ? ArrOfResolutionFactor_Portrait[i]
                                             : ArrOfResolutionFactor_Landscape[i]));
                arrResolutionDiff.Add(value_);
            }

            arrResolutionDiff.CopyTo(tempArr);
            arrResolutionDiff.Sort();
            float value__ = (float)arrResolutionDiff[0];
            for (int i = 0; i < tempArr.Length; i++)
            {
                if (value__ == tempArr[i])
                {
                    mCurrentDeviecResolution = (MyDeviceResolutions)i;
                    cameraViewPortWidth = 0.9f;
                    break;
                }
            }
        }

        Debug.Log("mCurrentDeviecResolution : " + mCurrentDeviecResolution);

        if (GetComponentInChildren<TextMesh>() != null)
            GetComponentInChildren<TextMesh>().text = Screen.width + "\n" + Screen.height + "\n" + mCurrentDeviecResolution;
    }
    */

    public float GetPositionBasedOnDeviceResolution(
        float res_22_9, float res_21_9, float res_20_9, float res_19_5_9,
        float res_19_9, float res_18_5_9, float res_18_9, float res_16_9,
        float res_5_3, float res_16_10, float res_3_2, float res_10_7, float res_4_3)
    {
        switch (mCurrentDeviecResolution)
        {
            case MyDeviceResolutions.Res_22_9:   return res_22_9;
            case MyDeviceResolutions.Res_21_9:   return res_21_9;
            case MyDeviceResolutions.Res_20_9:   return res_20_9;
            case MyDeviceResolutions.Res_19_5_9: return res_19_5_9;
            case MyDeviceResolutions.Res_19_9:   return res_19_9;
            case MyDeviceResolutions.Res_18_5_9: return res_18_5_9;
            case MyDeviceResolutions.Res_18_9:   return res_18_9;
            case MyDeviceResolutions.Res_16_9:   return res_16_9;
            case MyDeviceResolutions.Res_5_3:    return res_5_3;
            case MyDeviceResolutions.Res_16_10:  return res_16_10;
            case MyDeviceResolutions.Res_3_2:    return res_3_2;
            case MyDeviceResolutions.Res_10_7:   return res_10_7;
            case MyDeviceResolutions.Res_4_3:    return res_4_3;
        }

        return res_16_9;
    }
    
    static int GCD(int a, int b)
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