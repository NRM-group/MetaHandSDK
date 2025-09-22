using System.Text;
using UnityEngine;

public class Receiver : MonoBehaviour
{
    void Update()
    {
        if (TCPHandler.stream != null && TCPHandler.stream.DataAvailable)
        {
            byte[] data = new byte[1024];
            int bytes = TCPHandler.stream.Read(data, 0, data.Length);
            string response = Encoding.ASCII.GetString(data, 0, bytes);
            Debug.Log("Received: " + response);
        }
    }
}
