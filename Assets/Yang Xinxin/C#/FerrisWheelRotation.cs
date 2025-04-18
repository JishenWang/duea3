using UnityEngine;

public class FerrisWheelRotation : MonoBehaviour
{
    public float rotationSpeed = 10f; // 摩天轮旋转速度，可在Inspector面板调整

    void Update()
    {
        // 尝试绕局部Z轴旋转
        transform.Rotate(transform.forward, rotationSpeed * Time.deltaTime);
    }
}