using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Record : MonoBehaviour
{

    #region Monobehaviour
    
    private void Start()
    {
        controller = OVRInput.Controller.RTouch;
        Initialize();
        StartCoroutine(CoRecord());
    }
    void Update()
    {
        //CameraRecenter();
        Reset();
        
        
        OculusInput();
    }

    #endregion

    #region Public Variables
    
    //-------------------------------------------------------------------------------------------------

    [Header("OVR Manager")] public OVRManager OVRManager;
    [Header("OVR Center Eye Anchor")] public Transform HeadTransform;
    [Header("Transmitter")] public Transmitter transmitter;
    [Header("Interval Time")] public float saveIntervalTime;

    [Header("Data String")] public string dataStr;
    //-------------------------------------------------------------------------------------------------
    public OVRInput.Controller controller;
    //-------------------------------------------------------------------------------------------------
    /// <summary>
    /// IsCenter => Camera ReCenter, IsStartExperiment => Experiment Start, IsReset => Contents Reset, IsMarkStart => Marking, IsMarkEnd => Mark End
    /// </summary>
    public bool IsCenter
    {
        get => isCenter;
        set => isCenter = value;
    }
    public bool IsStartExperiment
    {
        get => isStartExperiment;
        set => isStartExperiment = value;
    }
    public bool IsReset
    {
        get => isReset;
        set => isReset = value;
    }
    public bool IsMark
    {
        get => isMark;
        set => isMark = value;
    }
    
    public bool IsExport
    {
        get => isExport;
        set => isExport = value;
    }

    [Header("Head Data")] public List<float[]> dataSet = new List<float[]>();

    public string[] tags;

    //-------------------------------------------------------------------------------------------------

    #endregion

    #region Private Variables
    
    [SerializeField] private bool isStartExperiment;
    [SerializeField] private bool isCenter;
    [SerializeField] private bool isReset;
    [SerializeField] private bool isMark;
    [SerializeField] private bool isExport;
    //-------------------------------------------------------------------------------------------------
    /// <summary>
    /// Head Data
    /// </summary>
    
    private float xQuaternion, yQuaternion, zQuaternion;
    private float xEuler, yEuler, zEuler;
    private float xPos, yPos, zPos;
    private float recordedTime;

    private int mark;
    //-------------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------------
    
    
    #endregion

    #region Setting

    private void Initialize()
    {
        recordedTime = -saveIntervalTime;
        tags = new[]
            {"XEuler", "YEuler", "ZEuler", "XPosition", "YPosition", "ZPosition", "Time", "Mark"};

        for (var i = 0; i < tags.Length; i++)
        {
            if (i == 0)
                dataStr += tags[i];
            else
            {
                var temp = "," + tags[i];
                dataStr += temp;
            }
        }
        Debug.Log(tags.Length);
    }
    /// <summary>
    /// Input Space key => OVR Camera recenter
    /// </summary>
    public void CameraRecenter()
    {
        OVRManager.OVRRecenter();
        isCenter = true;
    }
    /// <summary>
    /// Press Start Experiment Button
    /// </summary>
    public void StartExperiment()
    {
        isStartExperiment = true;
    }

    private void Reset()
    {
        if (isReset)
        {
            isReset = false;
            SceneManager.LoadScene("ExperimentEnv");
        }
    }

    #endregion

    #region Record

    private IEnumerator CoRecord()
    {
        while (!isCenter)
        {
            yield return null;
        }
        
        if (isCenter && isStartExperiment && !isExport)
        {
            UpdateHeadTransform();
        }
        yield return new WaitForSeconds(saveIntervalTime);
        StartCoroutine(CoRecord());
    }
    
    /// <summary>
    /// dataSet Index
    /// 0 : X Euler , 1 : Y Euler , 2 : Z Euler
    /// 3 : X Position, 4 : Y Position , 5 : Z Position
    /// 6 : Recorded Time
    /// </summary>
    private void UpdateHeadTransform()
    {
        xQuaternion = HeadTransform.rotation.x;
        yQuaternion = HeadTransform.rotation.y;
        zQuaternion = HeadTransform.rotation.z;
        xEuler = HeadTransform.rotation.eulerAngles.x;
        yEuler = HeadTransform.rotation.eulerAngles.y;
        zEuler = HeadTransform.rotation.eulerAngles.z;
        xPos = HeadTransform.position.x;
        yPos = HeadTransform.position.y;
        zPos = HeadTransform.position.z;

        
        if (xQuaternion < 0)
            xEuler -= 360;
        else if (xQuaternion > 1)
            xEuler += 180;


        if (yQuaternion < 0)
            yEuler -= 360;
        else if (yQuaternion > 1)
            yEuler += 180;


        if (zQuaternion < 0)
            zEuler -= 360;

        else if (zQuaternion > 1)
            zEuler += 180;
        
        recordedTime += saveIntervalTime;
        var headTRData = new float[8];
        headTRData[0] = xEuler;
        headTRData[1] = yEuler;
        headTRData[2] = zEuler;
        headTRData[3] = xPos;
        headTRData[4] = yPos;
        headTRData[5] = zPos;
        headTRData[6] = recordedTime;
        
        if (isMark)
            mark = 1;
        else
            mark = 0;
        headTRData[7] = mark;
        //dataSet.Add(headTRData);
            
        isMark = false;
        
        for (var i = 0; i < headTRData.Length; i++)
        {
            var temp = "," + headTRData[i];
            dataStr += temp;
        }
        
        Debug.Log("Data set sizes :" + dataSet.Count);
    }

   
    
    private float CalculateVelocity(Vector3 a, Vector3 b, float time)
    {
        var p = Quaternion.Euler(a);
        var q = Quaternion.Euler(b);
        var angle = Quaternion.Angle(p, q);
        var velocity = angle / time;
        return velocity;
    }

    private float CalculateAcceleration(float a, float b, float time)
    {
        var sub = Mathf.Abs(a - b);
        var acceleration = sub / time;
        return acceleration;
    }
    
    #endregion

    private void OculusInput()
    {
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger, controller))
        {
            isStartExperiment = true;
            CameraRecenter();
            
            Debug.Log("START EXPERIMENT");
        }

        if (OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger, controller))
        {
            isMark = true;
            
            Debug.Log("MARK");
        }
    }
}
