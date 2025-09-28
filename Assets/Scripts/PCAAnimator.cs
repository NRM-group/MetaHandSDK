using UnityEngine;

public class HandAnimator : MonoBehaviour
{
    public Receiver receiver; // assign in Inspector
    public Animator handAnimator; // Assign in Inspector
    public enum HandSide { Left, Right }
    public HandSide side;          // pick Left or Right per GameObject
    private float Rx, Ry;
    private float Lx, Ly;
    private float smoothRx, smoothRy;
    private float smoothLx, smoothLy;

    [Header("Smoothing Settings")]
    [Range(0f, 20f)]
    public float smoothFactor = 10f; // higher = faster response, lower = smoother


    void Update()
    {   
        if (receiver == null || handAnimator ==  null) return;

        // Select PCA values based on hand side
        if (side == HandSide.Right)
        {
            (Rx, Ry) = receiver.GetRightPCA();
            smoothRx += (Rx - smoothRx) * smoothFactor * Time.deltaTime;
            smoothRy += (Ry - smoothRy) * smoothFactor * Time.deltaTime;
            handAnimator.SetFloat("PCAX", Rx);
            handAnimator.SetFloat("PCAY", Ry);
        }
        else // Left hand
        {
            (Lx, Ly) = receiver.GetLeftPCA();
            smoothLx += (Lx - smoothLx) * smoothFactor * Time.deltaTime;
            smoothLy += (Ly - smoothLy) * smoothFactor * Time.deltaTime;
            handAnimator.SetFloat("PCAX", Lx);
            handAnimator.SetFloat("PCAY", Ly);
        }

    }
}
