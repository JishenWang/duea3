using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;          // 移动速度
    public float rotationSpeed = 10f;     // 旋转速度

    [Header("Animation Parameters")]
    public string dizzyStateName = "Dizzy";  // 默认眩晕状态名称
    public string runParameterName = "IsRunning"; // 跑步动画参数名

    private Animator animator;            // 动画控制器
    private Rigidbody rb;                // 刚体组件
    private Vector3 movement;             // 移动方向
    private bool isMoving = false;        // 是否在移动

    void Start()
    {
        // 获取组件
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        // 确保初始状态是Dizzy动画
        if (animator != null)
        {
            animator.Play(dizzyStateName);
        }
    }

    void Update()
    {
        // 获取输入
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 计算移动方向
        movement = new Vector3(horizontal, 0f, vertical).normalized;

        // 检查是否有移动输入
        isMoving = movement.magnitude > 0.1f;

        // 控制动画状态
        if (animator != null)
        {
            animator.SetBool(runParameterName, isMoving);
        }
    }

    void FixedUpdate()
    {
        // 物理移动
        if (isMoving)
        {
            // 移动角色
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

            // 旋转角色朝向移动方向
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            rb.rotation = Quaternion.Slerp(rb.transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }
}