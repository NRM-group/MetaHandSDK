using UnityEngine;

public class HandAnimator : MonoBehaviour
{
    public Receiver receiver; // assign in Inspector
    public Animator handAnimator; // Assign in Inspector
    public enum HandSide { Left, Right }
    public HandSide side;          // pick Left or Right per GameObject
    private float x, y;


    void Update()
    {   
        if (receiver == null || handAnimator ==  null) return;

        // Select PCA values based on hand side
        if (side == HandSide.Right)
        {
            (x, y) = receiver.GetRightPCA();
        }
        else // Left hand
        {
            (x, y) = receiver.GetLeftPCA();
        }

        //Debug.Log($"PCA hand anim received Å® X={x}, Y={y}");
        handAnimator.SetFloat("PCAX", x);
        handAnimator.SetFloat("PCAY", y);

    }
}
