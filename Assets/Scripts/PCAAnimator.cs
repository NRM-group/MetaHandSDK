using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class PCAHandBlend : MonoBehaviour
{
    public Animator handAnimator; // Assign in Inspector
    private Thread listenerThread;
    private bool running = false;

    private float PCAX = 0f;
    private float PCAY = 0f;


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

                    string[] parts = message.Split(',');
                    if (parts.Length == 2)
                    {
                        if (float.TryParse(parts[0], out float x) &&
                            float.TryParse(parts[1], out float y))
                        {
                            lock (this)
                            {
                                PCAX = x;
                                PCAY = y;
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

    void Update()
    {
        float x, y;
        lock (this)
        {
            x = PCAX;
            y = PCAY;
        }

        // Feed into Animator Blend Tree
        if (handAnimator != null)
        {
            handAnimator.SetFloat("PCAX", x);
            handAnimator.SetFloat("PCAY", y);
        }
    }

    void OnApplicationQuit()
    {
        running = false;
    }
}
