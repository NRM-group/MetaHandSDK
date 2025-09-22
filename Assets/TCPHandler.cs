using System.Net.Sockets;
using UnityEngine;

public class TCPHandler : MonoBehaviour
{
    public string serverIP = "127.0.0.1";
    //public string serverIP = "192.168.0.33";
    public int serverPort = 11113;

    public static TcpClient client;
    public static NetworkStream stream;

    void Awake()
    {
        if (client == null)
        {
            try
            {
                client = new TcpClient(serverIP, serverPort);
                stream = client.GetStream();
                Debug.Log("Connected to TCP server.");
            }
            catch (SocketException e)
            {
                Debug.LogError($"TCP Connection error: {e.Message}");
            }
        }
    }

    void OnApplicationQuit()
    {
        stream?.Close();
        client?.Close();
    }
}
