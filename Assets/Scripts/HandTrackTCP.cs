using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Newtonsoft.Json; // install Json.NET (Newtonsoft.Json) in Unity

//TCP
using System.Net.Sockets;
using System.Text;
using static UnityEngine.Rendering.DebugUI.Table;
using UnityEngine.UIElements;
using System.Xml;
using NUnit.Framework.Interfaces;
using static Oculus.Interaction.GrabAPI.FingerPalmGrabAPI;
using UnityEngine.UI;
using System;

[System.Serializable]
public class BoneData
{
    public string BoneID; // Bone name "Metacarpal", "tip"
    public Quaternion rotation;
}

[System.Serializable]
public class FingerData
{
    public string FingerID; // e.g. "Thumb", "Index"
    public List<BoneData> bones = new List<BoneData>();
}

[System.Serializable]
public class HandData
{
    public long timestamp;
    public string side; // "L" or "R"
    public List<BoneData> Wrist = new List<BoneData>();
    public List<FingerData> Fingers = new List<FingerData>();
}


public class HandTrackTCP : MonoBehaviour
{
    public OVRHand leftHand;
    public OVRHand rightHand;
    public OVRSkeleton leftHandSkeleton;
    public OVRSkeleton rightHandSkeleton;

    private float step;

    //TCP
    private TcpClient client;
    private NetworkStream stream;
    public string serverIP = "127.0.0.1";
    public int serverPort = 11112;

    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.vSyncCount = 0;     // disable VSync
        Application.targetFrameRate = 60;   // lock Update() to ~30 calls/sec

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

    // Update is called once per frame
    void Update()
    {
        HandleHand(leftHand, leftHandSkeleton, "L");
        HandleHand(rightHand, rightHandSkeleton, "R");
    }

    void HandleHand(OVRHand hand, OVRSkeleton skeleton, string side)
    {
        if (hand == null || skeleton == null) return;
        if (!hand.IsTracked) return;

        HandData handData = new HandData();
        handData.side = side;
        handData.timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // Create lookup table for fingers
        Dictionary<string, FingerData> fingerMap = new Dictionary<string, FingerData>()
    {
        { "0", new FingerData{ FingerID = "0" } }, // Thumb
        { "1", new FingerData{ FingerID = "1" } }, // Index
        { "2", new FingerData{ FingerID = "2" } }, // Middle
        { "3", new FingerData{ FingerID = "3" } }, // Ring
        { "4", new FingerData{ FingerID = "4" } }  // Little/Pinky
    };

        int boneIndex = 0;
        foreach (var b in skeleton.Bones)
        {
            string boneName = b.Transform.gameObject.name;

            // Wrist
            if (boneName.Contains("Palm"))
            {
                handData.Wrist.Add(new BoneData
                {
                    BoneID = "Wrist",
                    rotation = b.Transform.rotation
                });
            }
            else
            {
                // Decide which finger this bone belongs to
                string fingerKey = null;
                if (boneName.Contains("Thumb")) fingerKey = "0";
                else if (boneName.Contains("Index")) fingerKey = "1";
                else if (boneName.Contains("Middle")) fingerKey = "2";
                else if (boneName.Contains("Ring")) fingerKey = "3";
                else if (boneName.Contains("Little") || boneName.Contains("Pinky")) fingerKey = "4";

                if (fingerKey != null)
                {
                    if (boneIndex > 3)
                    {
                        boneIndex = 0;
                        continue;
                    }
                    fingerMap[fingerKey].bones.Add(new BoneData
                    {
                        BoneID = boneIndex.ToString(),
                        rotation = b.Transform.rotation
                    });
                    boneIndex++;
                }
            }
        }

        // Add all fingers with their bones
        foreach (var f in fingerMap.Values)
        {
            if (f.bones.Count > 0)
                handData.Fingers.Add(f);
        }

        // Convert to JSON
        string json = JsonUtility.ToJson(handData);
        SendTCP(json);
    }

    void SendTCP(string message)
    {
        if (stream != null && stream.CanWrite)
        {
            byte[] data = Encoding.UTF8.GetBytes(message + "\n"); // newline helps parsing (?)
            stream.Write(data, 0, data.Length);
        }
    }


    void OnApplicationQuit()
    {
        if (stream != null) stream.Close();
        if (client != null) client.Close();

        Debug.Log("TCP closede\n");
    }
}

