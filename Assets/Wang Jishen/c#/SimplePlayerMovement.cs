using UnityEngine;

public class SimplePlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;     // 移动速度

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // 获取刚体组件
    }

    void FixedUpdate()
    {
        float moveX = Input.GetAxis("Horizontal"); // A/D 或 左/右箭头键
        float moveZ = Input.GetAxis("Vertical");   // W/S 或 上/下箭头键

        Vector3 move = new Vector3(moveX, 0f, moveZ) * moveSpeed;

        // 保持原有y速度（比如受重力影响）
        Vector3 newVelocity = new Vector3(move.x, rb.velocity.y, move.z);
        rb.velocity = newVelocity;
    }
}
