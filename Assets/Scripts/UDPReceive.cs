
/*
    -----------------------
    UDP-Receive (send to)
    -----------------------
    // [url]http://msdn.microsoft.com/de-de/library/bb979228.aspx#ID0E3BAC[/url]

 
*/
using UnityEngine;
using System.Collections;

using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPInfo
{
    public static string lastReceivedUDPPacket;
}
public class UDPReceive : MonoBehaviour
{
    // receiving Thread
    Thread receiveThread;

    // udpclient object
    UdpClient client;

    //public string IP = "127.0.0.1"; //default local

    //public string IP = "192.168.1.4";
    public int port; // define > init

    // start from shell
    private static void Main()
    {
        UDPReceive receiveObj = new UDPReceive();
        receiveObj.init();

        string text = "";
        do
        {
            text = Console.ReadLine();
        }
        while (!text.Equals("exit"));
    }
    // start from unity3d
    public void Start()
    {

        init();
    }

    // init
    private void init()
    {
        
        // print("UDPSend.init()");

        // define port
        //port = 48055;
        port = 2345;
        // IP = "192.168.1.4";

    // status
        receiveThread = new Thread(
            new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();

    }
    // receive thread
    private void ReceiveData()
    {
        client = new UdpClient(port);
        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);

                // Bytes mit der UTF8-Kodierung in das Textformat kodieren.
                string text = Encoding.UTF8.GetString(data);

                // latest UDPpacket
                UDPInfo.lastReceivedUDPPacket = text;

            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }
}
