using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine.UI;
using Unity.Collections;
using UnityEngine.Android;

public class VideoNetwork : MonoBehaviour
{
    // NOTE: I moved this into RCAS_Peer.

    /*
    public static VideoNetwork Instance;

    public int port = 27015;

    public bool isReceiver;
    public bool isSender;
    public string receiverIP = "192.168.178.23";

    Thread ListenerThread;
    Thread BroadcasterThread;

    UdpClient udpClient;

    private void Start()
    {
        Instance = this;

        udpClient = new UdpClient(port);
        udpClient.EnableBroadcast = true;

        Debug.Log(Permission.HasUserAuthorizedPermission("android.permission.INTERNET"));
        Debug.Log(Permission.HasUserAuthorizedPermission("INTERNET"));

        if (isReceiver)
        {
            ListenerThread = new Thread(Listener);
            ListenerThread.Start();
        }

        if(isSender)
        {
            BroadcasterThread = new Thread(Broadcaster);
            BroadcasterThread.Start();
        }
    }

    private void Update()
    {
        if(receiveDataFlag)
        {
            VideoReceiver.Instance.OnReceiveNewFrame(receiveData);
            receiveDataFlag = false;
        }
    }

    private void OnDestroy()
    {
        if(ListenerThread != null) ListenerThread.Abort();
        if(BroadcasterThread != null) BroadcasterThread.Abort();
    }

    volatile bool sendDataFlag = false;
    volatile byte[] sendData;
    public void SendImage(byte[] image)
    {
        sendDataFlag = true;
        sendData = image;
    }

    volatile bool receiveDataFlag = false;
    volatile byte[] receiveData;
    void Listener()
    {
        //UdpClient udpClient = new UdpClient(port);
        IPEndPoint groupEP = new IPEndPoint(IPAddress.Parse(receiverIP), port);

        try
        {
            while(true)
            {
                //Debug.Log("Waiting for broadcast");
                receiveData = udpClient.Receive(ref groupEP);
                receiveDataFlag = true;

                //Debug.Log($"Received broadcast from {groupEP}. LEN: {receiveData.Length}");
                //Debug.Log($" {Encoding.ASCII.GetString(bytes, 0, bytes.Length)}");
            }
        }
        catch (SocketException e)
        {
            Debug.LogException(e);
        }
        finally
        {
            udpClient.Close();
        }
    }

    void Broadcaster()
    {
        //Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        // This is my PCs ip
        IPAddress broadcast = IPAddress.Parse(receiverIP);

        //byte[] sendbuf = Encoding.ASCII.GetBytes("pepega");
        IPEndPoint ep = new IPEndPoint(broadcast, port);

        //s.Connect(new IPEndPoint(broadcast, port));

        try
        {
            while (true)
            {
                //System.Threading.Thread.Sleep(1000);
                while(!sendDataFlag) { }

                sendDataFlag = false;

                if (sendData == null)
                {
                    Debug.Log("SenData is null!");
                    continue;
                }

                //s.SendTo(sendData, ep);

                if (sendData.Length > 50000)
                {
                    Debug.Log("SenData is too big!");
                    continue;
                }

                try
                {

                    udpClient.Send(sendData, sendData.Length, ep);
                }
                catch (System.Exception e)
                {
                    //Debug.Log("Something went wrong!");
                    Debug.Log(e.Message);
                    Debug.Log(e.StackTrace);
                }

                //Debug.Log("Message sent!");
            }
        }
        catch(System.Exception e)
        {
            Debug.LogException(e);
        }
        finally { 
            udpClient.Close();
        }
    }
    */
}
