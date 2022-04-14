using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;

#if !UNITY_EDITOR
using Windows.Networking.Sockets;
using Windows.Networking.Connectivity;
using Windows.Networking;
#endif

/// <summary>
/// UDP communication
/// </summary> 
/// 
/*public class UDPIn
{
    public  static Matrix4x4 latestTransformation = Matrix4x4.identity;
}*/
public class UDPCommunication : MonoBehaviour {

    // self port
    public string port = "2345";
    // external IP and port
    // public string externalIP = "10.161.159.203";
    // public string externalPort = "12346";
    private string TAG = "UDPCommunication";

    //  private AtracsysCalibration calibrator;

    private bool updatedLastFrame = false;

    public bool isUpdatedLastFrame() {
        return updatedLastFrame;
    }

    public static string lastHoloUDPPacket;

    // public Image networkIndicator;
    public float joint0 = 0.0f;
    public float joint1 = 0.0f;
    public float joint2 = 0.0f;
    public float joint3 = 0.0f;
    public float joint4 = 0.0f;
    public float joint5 = 0.0f;

    //private float joint0;
    //private float joint1;
    //private float joint2;
    //private float joint3;
    //private float joint4;
    //private float joint5;


    [System.Serializable]
    public class RecvMsg {
        // both x and y ranging from -0.5 to 0.5
        public float x = 0.0f;
        public float y = 0.0f;
        public float z = 0.0f;
        public float rx = 0.0f;
        public float ry = 0.0f;
        public float rz = 0.0f;
    }
    //  private static class Kin
    //{
    // both x and y ranging from -0.5 to 0.5
   //public class Kin {

   //     public float joint0 = 0.0f;
   //     public float joint1 = 0.0f;
   //     public float joint2 = 0.0f;
   //     public float joint3 = 0.0f;
   //     public float joint4 = 0.0f;
   //     public float joint5 = 0.0f;
   // }
   // }
   private Matrix4x4 latestTransformation = Matrix4x4.identity;

    public readonly static Queue<Action> ExecuteOnMainThread = new Queue<Action>();

    public void Parse(string message) {
        //string[] words = message.Split(' ');
        //  byte[] bytes = Encoding.Default.GetBytes(message);
        //  lastHoloUDPPacket = Encoding.UTF8.GetString(bytes);
        //  Debug.Log("Message: " + lastHoloUDPPacket);

        //comment
        //message = "0.1367247, 0.01675024, 0.3959179, -0.0004769088, -0.5859692, 0.09100287, 0.8052071";
        // string[] words = message.Split(new char[] { ' ', '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
        // string[] words = message.Split(' ');
        //string[] words = message.Split(',');

        char[] delimiterChars = { ' ', ',' };
        string[] words = message.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
        Vector3 position = new Vector3(float.Parse(words[0]), float.Parse(words[1]), float.Parse(words[2]));
          Quaternion rotation;
          rotation.x = float.Parse(words[3]);
          rotation.y = float.Parse(words[4]);
          rotation.z = float.Parse(words[5]);
          rotation.w = float.Parse(words[6]);
     /*   joint0 = float.Parse(words[0]);
        joint1 = float.Parse(words[1]);
        joint2 = float.Parse(words[2]);
        joint3 = float.Parse(words[3]);
        joint4 = float.Parse(words[4]);
        joint5 = float.Parse(words[5]);  */
           latestTransformation = Matrix4x4.TRS(position, rotation, Vector3.one);
       
    }

      public Matrix4x4 getLatestTransformation() {
           return latestTransformation;
       }
       
    public float getLatestJoint0() {
        return joint0;
    }
    public float getLatestJoint1()
    {
        return joint1;
    }
    public float getLatestJoint2()
    {
        return joint2;
    }
    public float getLatestJoint3()
    {
        return joint3;
    }
    public float getLatestJoint4()
    {
        return joint4;
    }
    public float getLatestJoint5()
    {
        return joint5;
    }

#if !UNITY_EDITOR
    DatagramSocket socket;
    
    async void Start() {
    /*
        calibrator = GetComponent<AtracsysCalibration>();
        if (calibrator == null) {
            Debug.Log(TAG + ": Calibration script not found");
        }
        */
        Debug.Log("Waiting for a connection...");
        socket = new DatagramSocket();
        socket.MessageReceived += Socket_MessageReceived;
        HostName IP = null;
        try
        {
            var icp = NetworkInformation.GetInternetConnectionProfile();

            IP = Windows.Networking.Connectivity.NetworkInformation.GetHostNames()
            .SingleOrDefault(
                hn =>
                    hn.IPInformation?.NetworkAdapter != null && hn.IPInformation.NetworkAdapter.NetworkAdapterId
                    == icp.NetworkAdapter.NetworkAdapterId);

            await socket.BindEndpointAsync(IP, port);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            Debug.Log(SocketError.GetStatus(e.HResult).ToString());
            return;
        }
    }
    
#else
    void Start() {
       // calibrator = GetComponent<AtracsysCalibration>();
      /*  if (calibrator == null) {
            Debug.Log(TAG + ": Calibration script not found");
        }*/
    }
    
#endif

    
    
    // Update is called once per frame
    void Update() {
        Action act = null;
        while (ExecuteOnMainThread.Count > 0) {
            act = ExecuteOnMainThread.Dequeue();
        }
        if (act != null) {
            updatedLastFrame = true;
          //  networkIndicator.color = Color.green;
            act.Invoke();
        }
        else {
            updatedLastFrame = false;
          //  networkIndicator.color = Color.red;
        }
    }


#if !UNITY_EDITOR
    private async void Socket_MessageReceived(Windows.Networking.Sockets.DatagramSocket sender,
        Windows.Networking.Sockets.DatagramSocketMessageReceivedEventArgs args)
    {
        // Debug.Log("Received message: ");
        //Read the message that was received from the UDP echo client.
        Stream streamIn = args.GetDataStream().AsStreamForRead();
        StreamReader reader = new StreamReader(streamIn);
        string message = await reader.ReadLineAsync();

         //Debug.Log("Message: " + message);

        if (ExecuteOnMainThread.Count < 3) {
            ExecuteOnMainThread.Enqueue(() => {
               Parse(message);
            });
            //lastHoloUDPPacket=message;
           // lastHoloUDPPacket=Encoding.UTF8.GetBytes(message);

           //   byte[] bytes = Encoding.Default.GetBytes(message);
            //  lastHoloUDPPacket = Encoding.UTF8.GetString(bytes);
          //  Debug.Log("Message: " + lastHoloUDPPacket);
        }
    }
#endif
}
