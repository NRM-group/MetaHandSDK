using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Receiver : MonoBehaviour
{
    private Thread listenerThread;
    private bool running = false;

    private float PCARX = 0f;
    private float PCARY = 0f;
    private float PCALX = 0f;
    private float PCALY = 0f;

    void Start()
    {
        listenerThread = new Thread(ListenForMessages);
        listenerThread.IsBackground = true;
        running = true;
        listenerThread.Start();
    }

    private void ListenForMessages()
    {
        try
        {
            while (running)
            {
                if (TCPHandler.stream != null && TCPHandler.stream.DataAvailable)
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = TCPHandler.stream.Read(buffer, 0, buffer.Length);
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                    //Debug.Log(message);

                    string[] parts = message.Split(',');
                    if (parts.Length == 3)
                    {
                        string side = parts[0]; // no need for TryParse, it's already a string
                        if (float.TryParse(parts[1], out float x) &&
                            float.TryParse(parts[2], out float y))
                        {
                            if (side=="R")
                            {
                                lock (this)
                                {
                                    PCARX = x;
                                    PCARY = y;
                                }
                            }
                            else if (side == "L")
                            {
                                lock (this)
                                {
                                    PCALX = x;
                                    PCALY = y;
                                }
                            }
                            else 
                            {
                                Debug.LogError("[TCP] Parse error");
                            }

                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("[TCP] Listener error: " + e);
        }
    }

    
    //Public getters
    public (float x, float y) GetRightPCA()
    {
        lock (this) return (PCARX, PCARY);
    }

    public (float x, float y) GetLeftPCA()
    {
        lock (this) return (PCALX, PCALY);
    }

    void OnApplicationQuit()
    {
        running = false;
    }
}
