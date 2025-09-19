using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class PCAHandBlend : MonoBehaviour
{
    public Animator handAnimator; // Assign in Inspector

    private TcpListener listener;
    private Thread listenerThread;
    private bool running = false;

    private float PCAX = 0f;
    private float PCAY = 0f;
    public string serverIP = "192.168.0.33";
    public int serverPort = 11112;

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
            IPAddress ipAddress = IPAddress.Parse(serverIP);//Parse IP

            //listener = new TcpListener(ipAddress, serverPort);
            listener = new TcpListener(IPAddress.Any, serverPort);

            listener.Start();
            Debug.Log($"[TCP] Listening on port {serverPort}");

            while (running)
            {
                using (TcpClient client = listener.AcceptTcpClient())
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
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
        listener?.Stop();
    }
}
