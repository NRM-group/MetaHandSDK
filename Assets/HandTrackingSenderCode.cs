using UnityEngine;
using UnityEngine.XR;

public class HandTrackingSender : MonoBehaviour
{
    public XRNode handNode = XRNode.LeftHand; // or RightHand

    void Update()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(handNode);
        if (device.TryGetFeatureValue(CommonUsages.isTracked, out bool isTracked) && isTracked)
        {
            if (device.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 pos))
            {
                Debug.Log($"Hand Pos: {pos}");
                // Send via TCP
            }
        }
    }
}
