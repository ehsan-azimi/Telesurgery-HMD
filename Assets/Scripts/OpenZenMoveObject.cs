using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Example Behaviour which applies the measured OpenZen sensor orientation to a 
 * Unity object.
 */
public class OpenZenMoveObject : MonoBehaviour
{
    // openzen declaration
    ZenClientHandle_t mZenHandle = new ZenClientHandle_t();
    ZenSensorHandle_t mSensorHandle = new ZenSensorHandle_t();
    
    // 2nd IMU
    ZenSensorHandle_t mSensorHandle2 = new ZenSensorHandle_t();

    public GameObject IMU98;
    public GameObject IMU29;
    [Tooltip("IO Type which OpenZen should use to connect to the sensor.")]
    public string OpenZenIoType = "Bluetooth";
    [Tooltip("Idenfier which is used to connect to the sensor. The name depends on the IO type used and the configuration of the sensor.")]
    public string OpenZenIdentifier = "00:04:3E:53:E9:29";
    public string OpenZenIdentifier2 = "00:04:3E:53:E9:98";
    public Camera cam1, cam2, cam3;


    // Humanoid declaration
    public GameObject Human;
    private GameObject[] HumanjointList = new GameObject[4];
    // Initialize Human Joint Values to T-pose
    private float[] HumanjointValues = { -14.996f, 0.547f, 1.458f, 0f, 5.685f, 1.042f, -4.332f };
    private string[] jointnames = {"Shoulder Abduction/Adduction", "Shoulder Flexion/Extension", "Shoulder Internal/External Rotation", "Elbow Flexion/Extension", "Forearm Supination/Pronation", "Wrist Flexion/Extension", "Wrist Abduction/Adduction" };
    private float[] upperjointlimit = {100f,  65f, 130f, 130f, 50f, 70f, 15f};
    private float[] lowerjointlimit = {-100f, -70f, -50f, -10f, -110f, -50f, -30f};

    private Quaternion shoulderoffset;
    private Quaternion elbowoffset;
    private Quaternion wristoffset;

    // Use this for initialization
    void Start()
    {
        //IMU29 = GameObject.Find("lpms-29");
        //IMU98 = GameObject.Find("lpms-98");
        //Human = GameObject.Find("Human");
        cam1.enabled = false;
        cam2.enabled = true;
        cam3.enabled = false;


        // Humanoid initialization
        initializeHumanJoints();

        letsdebug();
        // create OpenZen
        OpenZen.ZenInit(mZenHandle);
        // Hint: to get the io type and identifer for all connected sensor,
        // you cant start the DiscoverSensorScene. The information of all 
        // found sensors is printed in the debug console of Unity after
        // the search is complete.

        print("Trying to connect to OpenZen Sensor on IO " + OpenZenIoType +
            " with sensor name " + OpenZenIdentifier);
    
        var sensorInitError = OpenZen.ZenObtainSensorByName(mZenHandle,
            OpenZenIoType,
            OpenZenIdentifier,
            0,
            mSensorHandle);

        
        if (sensorInitError != ZenSensorInitError.ZenSensorInitError_None)
        {
            print("Error while connecting to sensor 29.");
        }
        else
        {
            ZenComponentHandle_t mComponent = new ZenComponentHandle_t();
            OpenZen.ZenSensorComponentsByNumber(mZenHandle, mSensorHandle, OpenZen.g_zenSensorType_Imu, 0, mComponent);

            // enable sensor streaming, normally on by default anyways
            OpenZen.ZenSensorComponentSetBoolProperty(mZenHandle, mSensorHandle, mComponent,
               (int)EZenImuProperty.ZenImuProperty_StreamData, true);

            // set offset mode to heading 
            OpenZen.ZenSensorComponentSetInt32Property(mZenHandle, mSensorHandle, mComponent,
                (int)EZenImuProperty.ZenImuProperty_OrientationOffsetMode, 1);

            // set the sampling rate to 100 Hz
            OpenZen.ZenSensorComponentSetInt32Property(mZenHandle, mSensorHandle, mComponent,
               (int)EZenImuProperty.ZenImuProperty_SamplingRate, 100);

            // filter mode using accelerometer & gyroscope & magnetometer
            OpenZen.ZenSensorComponentSetInt32Property(mZenHandle, mSensorHandle, mComponent,
               (int)EZenImuProperty.ZenImuProperty_FilterMode, 1);

            // Ensure the Orientation data is streamed out
            OpenZen.ZenSensorComponentSetBoolProperty(mZenHandle, mSensorHandle, mComponent,
               (int)EZenImuProperty.ZenImuProperty_OutputQuat, true);

            print("Sensor 29 configuration complete");
        }


        print("Trying to connect to OpenZen Sensor on IO " + OpenZenIoType +
           " with sensor name " + OpenZenIdentifier2);

        var sensorInitError2 = OpenZen.ZenObtainSensorByName(mZenHandle,
            OpenZenIoType,
            OpenZenIdentifier2,
            0,
            mSensorHandle2);

        if (sensorInitError2 != ZenSensorInitError.ZenSensorInitError_None)
        {
            print("Error while connecting to sensor 98.");
        }
        else
        {
            ZenComponentHandle_t mComponent2 = new ZenComponentHandle_t();
            OpenZen.ZenSensorComponentsByNumber(mZenHandle, mSensorHandle2, OpenZen.g_zenSensorType_Imu, 0, mComponent2);

            // enable sensor streaming, normally on by default anyways
            OpenZen.ZenSensorComponentSetBoolProperty(mZenHandle, mSensorHandle2, mComponent2,
               (int)EZenImuProperty.ZenImuProperty_StreamData, true);

            // set offset mode to heading 
            OpenZen.ZenSensorComponentSetInt32Property(mZenHandle, mSensorHandle2, mComponent2,
                (int)EZenImuProperty.ZenImuProperty_OrientationOffsetMode, 1);

            // set the sampling rate to 100 Hz
            OpenZen.ZenSensorComponentSetInt32Property(mZenHandle, mSensorHandle2, mComponent2,
               (int)EZenImuProperty.ZenImuProperty_SamplingRate, 100);

            // filter mode using accelerometer & gyroscope & magnetometer
            OpenZen.ZenSensorComponentSetInt32Property(mZenHandle, mSensorHandle2, mComponent2,
               (int)EZenImuProperty.ZenImuProperty_FilterMode, 1);

            // Ensure the Orientation data is streamed out
            OpenZen.ZenSensorComponentSetBoolProperty(mZenHandle, mSensorHandle2, mComponent2,
               (int)EZenImuProperty.ZenImuProperty_OutputQuat, true);

            print("Sensor 98 configuration complete");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) //switch between view from the back and from shoudler
        {
            cam1.enabled = !cam1.enabled;
            cam2.enabled = !cam2.enabled;
        }
        if (Input.GetKeyDown(KeyCode.V)) //switch between view from current to front
        {
            cam1.enabled = !cam1.enabled;
            cam2.enabled = !cam2.enabled;
            cam3.enabled = !cam3.enabled;
        }


        // run as long as there are new OpenZen events to process
        while (true)
        {
            ZenEvent zenEvent = new ZenEvent();

            // read all events which are waiting for us
            // use the rotation from the newest IMU event
            if (!OpenZen.ZenPollNextEvent(mZenHandle, zenEvent))
                break;

            // if compontent handle = 0, this is a OpenZen wide event,
            // like sensor search
            if (zenEvent.component.handle != 0)
            {
                if (zenEvent.sensor.handle == mSensorHandle.handle)
                {
                    switch (zenEvent.eventType)
                    {
                        case (int)ZenImuEvent.ZenImuEvent_Sample:
                            // read quaternion
                            OpenZenFloatArray fq = OpenZenFloatArray.frompointer(zenEvent.data.imuData.q);
                            // Unity Quaternion constructor has order x,y,z,w
                            // Furthermore, y and z axis need to be flipped to 
                            // convert between the LPMS and Unity coordinate system
                            Quaternion sensorOrientation = new Quaternion(fq.getitem(1),
                                                                        fq.getitem(3),
                                                                        fq.getitem(2),
                                                                        fq.getitem(0));
                            
                            IMU29.transform.rotation = sensorOrientation;
                            Debug.DrawRay(IMU29.transform.position, sensorOrientation * Vector3.up, Color.green); //y
                            Debug.DrawRay(IMU29.transform.position, sensorOrientation * Vector3.right, Color.red); //x
                            Debug.DrawRay(IMU29.transform.position, sensorOrientation * Vector3.forward, Color.blue); //z
                            HumanjointList[1].transform.rotation = sensorOrientation* shoulderoffset;
                            Debug.DrawRay(HumanjointList[1].transform.position, HumanjointList[1].transform.rotation * Vector3.up, Color.green); //y
                            Debug.DrawRay(HumanjointList[1].transform.position, HumanjointList[1].transform.rotation * Vector3.right, Color.red); //x
                            Debug.DrawRay(HumanjointList[1].transform.position, HumanjointList[1].transform.rotation * Vector3.forward, Color.blue); //z
                            Debug.DrawRay(HumanjointList[0].transform.position, Vector3.up, Color.green); //y
                            Debug.DrawRay(HumanjointList[0].transform.position, Vector3.right, Color.red); //x
                            Debug.DrawRay(HumanjointList[0].transform.position, Vector3.forward, Color.blue); //z


                            break;
                    }
                }
                if (zenEvent.sensor.handle == mSensorHandle2.handle)
                {
                    switch (zenEvent.eventType)
                    {
                        case (int)ZenImuEvent.ZenImuEvent_Sample:
                            // read quaternion
                            OpenZenFloatArray fq = OpenZenFloatArray.frompointer(zenEvent.data.imuData.q);
                            // Unity Quaternion constructor has order x,y,z,w
                            // Furthermore, y and z axis need to be flipped to 
                            // convert between the LPMS and Unity coordinate system
                            Quaternion sensorOrientation = new Quaternion(fq.getitem(1),
                                                                        fq.getitem(3),
                                                                        fq.getitem(2),
                                                                        fq.getitem(0));        
                            IMU98.transform.rotation = sensorOrientation;
                            Debug.DrawRay(IMU98.transform.position, sensorOrientation * Vector3.up, Color.green); //y
                            Debug.DrawRay(IMU98.transform.position, sensorOrientation * Vector3.right, Color.red); //x
                            Debug.DrawRay(IMU98.transform.position, sensorOrientation * Vector3.forward, Color.blue); //z

                            // Rotate elbow joint
                            HumanjointList[2].transform.localRotation = Quaternion.AngleAxis(IMU98.transform.localEulerAngles.y, -Vector3.forward);
                           
                            // Rotate forearm supination/pronation joint
                            HumanjointList[3].transform.localRotation = Quaternion.AngleAxis(IMU98.transform.localEulerAngles.x, Vector3.up);



                            break;
                    }
                }
                
            }
            Debug.DrawRay(HumanjointList[2].transform.position, HumanjointList[2].transform.rotation * Vector3.up, Color.green); //y
            Debug.DrawRay(HumanjointList[2].transform.position, HumanjointList[2].transform.rotation * Vector3.right, Color.red); //x
            Debug.DrawRay(HumanjointList[2].transform.position, HumanjointList[2].transform.rotation * Vector3.forward, Color.blue); //z
            Debug.DrawRay(HumanjointList[3].transform.position, HumanjointList[3].transform.rotation * Vector3.up, Color.green); //y
            Debug.DrawRay(HumanjointList[3].transform.position, HumanjointList[3].transform.rotation * Vector3.right, Color.red); //x
            Debug.DrawRay(HumanjointList[3].transform.position, HumanjointList[3].transform.rotation * Vector3.forward, Color.blue); //z

            float w = HumanjointList[1].transform.rotation.w;
            float x = HumanjointList[1].transform.rotation.x;
            float y = HumanjointList[1].transform.rotation.y;
            float z = HumanjointList[1].transform.rotation.z;
            //Debug.Log("shoulder quaternion is: [" + w + ", " + x + ", " + y + ", " + z + "]");
            //Debug.Log("internal quat is: " + HumanjointList[1].transform.rotation);
            
            Vector3 scale = new Vector3(5, 5, 5);
            Vector3 scale2 = new Vector3(1, 1, 1);
            Vector3 trans0 = HumanjointList[1].transform.position;
            Vector3 trans1 = HumanjointList[2].transform.localPosition;
            Vector3 trans2 = HumanjointList[3].transform.localPosition;
            Matrix4x4 TrueShoulder = HumanjointList[1].transform.localToWorldMatrix;
            Matrix4x4 World2Shoulder = Matrix4x4.TRS(trans0, HumanjointList[1].transform.rotation, scale);
            Matrix4x4 Shoulder2Elbow = Matrix4x4.TRS(trans1, HumanjointList[2].transform.localRotation, scale2);
            Matrix4x4 Elbow2Wrist = Matrix4x4.TRS(trans2, HumanjointList[3].transform.localRotation, scale2); 
            Matrix4x4 Test = World2Shoulder * Shoulder2Elbow * Elbow2Wrist;
            //Debug.Log("This is Computed: ");
            
            //Debug.Log(World2Shoulder*Shoulder2Elbow*Elbow2Wrist);
            Matrix4x4 FK = HumanjointList[3].transform.localToWorldMatrix;
            //Debug.Log("This is True: ");
            //Debug.Log(FK);

            

        }
    }

    void OnDestroy()
    {
        if (mSensorHandle != null)
        {
            OpenZen.ZenReleaseSensor(mZenHandle, mSensorHandle);
        }
        OpenZen.ZenShutdown(mZenHandle);
    }

    // Create the list of GameObjects that represent each joint of the humanoid
    void initializeHumanJoints()
    {
        var HumanChildren = Human.GetComponentsInChildren<Transform>();
        for (int i = 0; i < HumanChildren.Length; i++)
        {
            if (HumanChildren[i].name == "mixamorig1:RightShoulder")
            {
                HumanjointList[0] = HumanChildren[i].gameObject;
            }
            else if (HumanChildren[i].name == "mixamorig1:RightArm")
            {
                HumanjointList[1] = HumanChildren[i].gameObject;
            }
            else if (HumanChildren[i].name == "mixamorig1:RightForeArm")
            {
                HumanjointList[2] = HumanChildren[i].gameObject;
            }
            else if (HumanChildren[i].name == "mixamorig1:RightHand")
            {
                HumanjointList[3] = HumanChildren[i].gameObject;
            }
        }
        // Shoulder Abduction/Adduction: mixamorig1:RightArm.x
        // Shoulder Flexion/Extension: mixamorig1:RightArm.y
        // Shoulder Internal/External Rotation: mixamorig1:RightArm.z
        // Elbow Flexion/Extension: mixamorig1:RightForeArm.z
        // Forearm Supination/Pronation: mixamorig1:RightHand.y
        // Wrist Flexion/Extension: mixamorig1:RightHand.x
        // Wrist Abduction/Adduction: mixamorig1:RightHand.z
        Vector3 ShoulderRotation = HumanjointList[1].transform.localEulerAngles;
        Vector3 ElbowRotation = HumanjointList[2].transform.localEulerAngles;
        Vector3 WristRotation = HumanjointList[3].transform.localEulerAngles;

        ShoulderRotation.x = HumanjointValues[0];
        ShoulderRotation.y = HumanjointValues[1];
        ShoulderRotation.z = HumanjointValues[2];
        HumanjointList[1].transform.localEulerAngles = ShoulderRotation;

        ElbowRotation.z = HumanjointValues[3];
        HumanjointList[2].transform.localEulerAngles = ElbowRotation;

        WristRotation.y = HumanjointValues[4];
        WristRotation.x = HumanjointValues[5];
        WristRotation.z = HumanjointValues[6];
        HumanjointList[3].transform.localEulerAngles = WristRotation;

        shoulderoffset = HumanjointList[1].transform.rotation;
        elbowoffset = HumanjointList[2].transform.rotation;
        wristoffset = HumanjointList[3].transform.rotation;
        //print(HumanjointList[1].transform.rotation);

    }

    // Create a button to perform Heading Reset
    void OnGUI()
    {

        GUIStyle myButtonStyle = new GUIStyle(GUI.skin.button);
        myButtonStyle.fontSize = 13;

        //GUI.Label(new Rect(0, 0, 100, 100), "Elbow IMU: "+IMU98.transform.localEulerAngles.y.ToString());
        //GUI.Label(new Rect(0, 40, 100, 100), "Shoulder IMU: "+IMU29.transform.localEulerAngles.y.ToString());
        if (GUI.Button(new Rect(Screen.width-170, 20, 150, 30), "IMU29 Heading Reset", myButtonStyle))
        {
            print("Performing Heading Reset for IMU29");
            ZenComponentHandle_t mComponent = new ZenComponentHandle_t();
            OpenZen.ZenSensorComponentsByNumber(mZenHandle, mSensorHandle, OpenZen.g_zenSensorType_Imu, 0, mComponent);
            // perform heading reset 
            OpenZen.ZenSensorComponentSetInt32Property(mZenHandle, mSensorHandle, mComponent,
                (int)EZenImuProperty.ZenImuProperty_OrientationOffsetMode, 1);
        }

        if (GUI.Button(new Rect(Screen.width - 170, 60, 150, 30), "IMU98 Heading Reset", myButtonStyle))
        {
            print("Performing Heading Reset for IMU98");
            ZenComponentHandle_t mComponent2 = new ZenComponentHandle_t();
            OpenZen.ZenSensorComponentsByNumber(mZenHandle, mSensorHandle2, OpenZen.g_zenSensorType_Imu, 0, mComponent2);
            // perform heading reset 
            OpenZen.ZenSensorComponentSetInt32Property(mZenHandle, mSensorHandle2, mComponent2,
                (int)EZenImuProperty.ZenImuProperty_OrientationOffsetMode, 1);
        }

    }



    void letsdebug()
    {
        for (int i = 0; i < 7; i++)
        {
            //print(jointnames[i] + "\n");
            //print(HumanjointValues[i]);
            //print(lowerjointlimit[i]);
            //print(upperjointlimit[i]);

        }
    }
}