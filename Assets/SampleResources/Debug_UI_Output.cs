using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using Unity.Collections;
using RCAS;

public class Debug_UI_Output : MonoBehaviour
{
    volatile string output = "";
    TMPro.TMP_Text text;

    private void Awake()
    {
        Application.logMessageReceivedThreaded += HandleLog;
    }

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TMPro.TMP_Text>();

        string localIP;
        using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
        {
            socket.Connect("8.8.8.8", 65530);
            IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
            localIP = endPoint.Address.ToString();
        }

        text.text = localIP;
        Debug.Log("Local IP-Address: " + localIP);
        Debug.Log(RCAS_Peer.Instance);
    }

    // Update is called once per frame
    void Update()
    {
        text.text = output;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        output = logString + "\n" + output;
        if (type != LogType.Log)
        {
            output += stackTrace + "\n" + output;
        }
        if (output.Length > 2000) output = "";
    }
}
