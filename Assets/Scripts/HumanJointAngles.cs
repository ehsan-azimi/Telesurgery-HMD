using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Example Behaviour which applies the measured OpenZen sensor orientation to a 
 * Unity object.
 */
public class HumanJointAngles : MonoBehaviour
{
    // Humanoid declaration
    public GameObject Human;
    private GameObject[] HumanjointList = new GameObject[4];
    private float[] HumanjointValues = new float[7];
    private string[] jointnames = { "Shoulder Abduction/Adduction", "Shoulder Flexion/Extension", "Shoulder Internal/External Rotation", "Elbow Flexion/Extension", "Forearm Supination/Pronation", "Wrist Flexion/Extension", "Wrist Abduction/Adduction" };
    private float[] upperjointlimit = { 100f, 65f, 130f, 130f, 50f, 70f, 15f };
    private float[] lowerjointlimit = { -100f, -70f, -50f, -10f, -110f, -50f, -30f };

    // Use this for initialization
    void Start()
    { 
        Human = GameObject.Find("Human");
        // Humanoid initialization
        initializeHumanJoints();
        // Debug function
        letsdebug();
    }

    // Update is called once per frame
    void Update()
    {
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
    }

    // Create a button to perform Heading Reset
    void OnGUI()
    {
        GUIStyle myButtonStyle = new GUIStyle(GUI.skin.button);
        myButtonStyle.fontSize = 13;
        int boundary = 20;
        int labelHeight = 20;
        GUI.skin.label.alignment = TextAnchor.MiddleLeft;
        for (int i = 0; i < 7; i++)
        {
            GUI.Label(new Rect(boundary, boundary + (i * 2 + 1) * labelHeight, labelHeight * 11, labelHeight), jointnames[i] + ": ", myButtonStyle);
            HumanjointValues[i] = GUI.HorizontalSlider(new Rect(boundary + labelHeight * 3, boundary + (i * 2 + 1) * labelHeight + labelHeight / 4 + boundary, labelHeight * 5, labelHeight), HumanjointValues[i], lowerjointlimit[i], upperjointlimit[i]);
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