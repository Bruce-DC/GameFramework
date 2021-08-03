using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class NetWorkScript : MonoBehaviour
{
    private static NetWorkScript _instance;

    private bool isConnecting = false;
    private byte[] data = new byte[1024];
    private Socket clientSocket;
    private Thread receiveT;

    public static NetWorkScript Instance
    {
        get
        {
            if (_instance == null)
                _instance = new NetWorkScript();
            return _instance;
        }
    }
    
    public void SendMes(string ms)
    {
        byte[] data = new byte[1024];
        data = Encoding.UTF8.GetBytes(ms);
        
        if (!isConnecting)
        {
            ConnectToServer(()=>{
                clientSocket.Send(data);
            });
        }
        else
        {
            clientSocket.Send(data);
        }
    }

    void ConnectToServer(Action calback)
    {
        try
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect("49.235.123.217", 3389);
            Debug.Log("连接服务器成功");
            receiveT = new Thread(ReceiveMsg);
            receiveT.Start();

            if (calback != null)
                calback();
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    private void ReceiveMsg()
    {
        while (true)
        {
            if (clientSocket.Connected == false)
            {
                Debug.Log("与服务器断开了连接");
                break;
            }

            int lenght = 0;
            lenght = clientSocket.Receive(data);

            string str = Encoding.UTF8.GetString(data, 0, data.Length);
            Debug.Log(str);
            break;
        }
    }

    

    public void OnDestroy()
    {
        try
        {
            if (clientSocket != null)
            {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close(); //关闭连接
            }

            if (receiveT != null)
            {
                receiveT.Interrupt();
                receiveT.Abort();
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }
}