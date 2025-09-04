using UnityEngine;

public class PCAHandBlend : MonoBehaviour
{
    public Animator handAnimator; // Assign in Inspector

    // These will be set by your PCA system
    public float PCAX = 0.0f;
    public float PCAY = 0f;

    void Update()
    {
        // Send PCA values into the BlendTree
        handAnimator.SetFloat("PCAX", PCAX);
        handAnimator.SetFloat("PCAY", PCAY);
    }
}
