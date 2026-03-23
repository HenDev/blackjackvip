using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public enum MyDeviceResolutions
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

public class MultiplePlatformPortingHandler : MonoBehaviour
{
    private static MultiplePlatformPortingHandler mInstance = null;

    public static MultiplePlatformPortingHandler Instance
    {
        get { return mInstance; }
    }

    private float[] ArrOfRatiosToAdjustCameraSize_Portrait =
        { 1.333f, 1.5f, 1.6f, 1.2f, 1.124f, 1.25f, 1.375f, 1.126f, 1.081f };
       //  2:3,   3:4,   4:5, 9:15,  9:16,  10:16,  11:16, 19.5:9, 19.5:9 Pro
    private float[] ArrOfRatiosToAdjustCameraSize_Landscape =
        { 3.0f, 2.666f, 2.5f, 3.33f, 3.55f, 3.2f, 2.909f, 4.333f, 4.4f };

    private int[] ArrOfResolutionFactor_Portrait = { 66, 75, 80, 60, 56, 62, 69, 46, 45 };
    private int[] ArrOfResolutionFactor_Landscape = { 150, 133, 125, 166, 177, 160, 143, 216, 217 };

    private string currentLoadedLevel = "";
    private MyDeviceResolutions mCurrentDeviecResolution = MyDeviceResolutions.Res_3_4;
    private float cameraViewPortWidth = 1.0f;
    private float cameraViewPortXPos = 0.0f;

    public MyDeviceResolutions CurrentLoadedDeviceResolution
    {
        get { return mCurrentDeviecResolution; }
    }

    public int GameWidth = 1024;
    public bool IsPortraitMode = false;

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

        float factorToAdjust = (IsPortraitMode
            ? ArrOfRatiosToAdjustCameraSize_Portrait[(int)mCurrentDeviecResolution]
            : ArrOfRatiosToAdjustCameraSize_Landscape[(int)mCurrentDeviecResolution]);
 
        
        float cameraSize = Mathf.Floor(((GameWidth / 32.0f) / factorToAdjust) * 100f) / 100f;
        Debug.Log($"Adjust Camera for {currentLoadedLevel} - Camera {cameraSize}");

        Camera[] arrCameras = Camera.allCameras;
        foreach (Camera camera in arrCameras)
        {
            if (camera.orthographic)
            {
                camera.orthographicSize = cameraSize;
            }
        }
    }

    void DetectCurrentDeviceResolution()
    {
        float screenWidthByHeightRatio = (float)Screen.width / (float)Screen.height;
        int screenWidthByHeightRatioInt = Mathf.FloorToInt(screenWidthByHeightRatio * 100);
       
        int gcd = GCD(Screen.width, Screen.height);
        int aspectWidth = Screen.width / gcd;
        int aspectHeight = Screen.height / gcd;

        Debug.Log($"AspectRatio: {aspectWidth}:{aspectHeight}");

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
            GetComponentInChildren<TextMesh>().text =
                Screen.width + "\n" + Screen.height + "\n" + mCurrentDeviecResolution;
    }

    public float GetPositionBasedOnDeviceResolution(float res_2_3, float res_3_4, float res_4_5, float res_9_15,
        float res_9_16, float res_10_16, float res_11_16, float res_19_5_9, float res_19_5_9_Pro)
    {
        switch (mCurrentDeviecResolution)
        {
            case MyDeviceResolutions.Res_2_3: return res_2_3;
            case MyDeviceResolutions.Res_3_4: return res_3_4;
            case MyDeviceResolutions.Res_4_5: return res_4_5;
            case MyDeviceResolutions.Res_9_15: return res_9_15;
            case MyDeviceResolutions.Res_9_16: return res_9_16;
            case MyDeviceResolutions.Res_10_16: return res_10_16;
            case MyDeviceResolutions.Res_11_16: return res_11_16;
            case MyDeviceResolutions.Res_19_5_9: return res_19_5_9;
            case MyDeviceResolutions.Res_19_5_9_Pro: return res_19_5_9_Pro;
        }

        return res_3_4;
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