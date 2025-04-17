using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    [Header("移动设置")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public float rotationSpeed = 10f;

    [Header("地面检测")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public bool showGroundGizmo = true;

    private CharacterController controller;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction runAction;
    private Vector3 velocity;
    private bool isGrounded;
    private Transform cameraTransform;
    private float currentSpeed;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        cameraTransform = Camera.main.transform;

        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        runAction = playerInput.actions["Run"];

        currentSpeed = walkSpeed;
        
        if (groundCheck == null)
        {
            groundCheck = transform;
            Debug.LogWarning("未指定地面检测点，将使用角色位置作为检测点");
        }
    }

    void Update()
    {
        // 地面检测
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0) velocity.y = -2f;

        // 获取输入
        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 moveDirection = new Vector3(input.x, 0, input.y).normalized;

        // 奔跑检测
        currentSpeed = runAction.IsPressed() ? runSpeed : walkSpeed;

        // 转换为摄像机相对方向
        if (moveDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            
            // 移动角色
            controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);

            // 平滑旋转角色朝向移动方向
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // 跳跃
        if (jumpAction.triggered && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 应用重力
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void OnDrawGizmos()
    {
        if (showGroundGizmo && groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}
