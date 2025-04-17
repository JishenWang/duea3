using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PersonCamera : MonoBehaviour
{
    [Header("移动设置")]
    [Tooltip("行走速度")] public float walkSpeed = 5f;
    [Tooltip("奔跑速度")] public float runSpeed = 8f;
    [Tooltip("跳跃高度")] public float jumpHeight = 2f;
    [Tooltip("重力系数")] public float gravity = -9.81f;
    [Tooltip("旋转平滑时间")] public float rotationSmoothTime = 0.1f;

    [Header("摄像机设置")]
    [Tooltip("摄像机参考")] public Transform cameraTransform;
    [Tooltip("摄像机距离")] public float cameraDistance = 5f;
    [Tooltip("摄像机高度")] public float cameraHeight = 2f;
    [Tooltip("摄像机平滑时间")] public float cameraSmoothTime = 0.3f;

    // 组件引用
    private CharacterController controller;
    private PlayerInput playerInput;
    
    // 输入状态
    private Vector2 moveInput;
    private bool jumpPressed;
    private bool isRunning;
    
    // 运动状态
    private Vector3 playerVelocity;
    private bool isGrounded;
    private float rotationVelocity;
    private Vector3 cameraVelocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Start()
    {
        InitializeCamera();
    }

    void Update()
    {
        HandleGroundCheck();
        HandleMovement();
        HandleJump();
        UpdateCameraPosition();
    }

    #region 输入系统回调
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpPressed = true;
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        isRunning = context.performed || context.phase == InputActionPhase.Performed;
    }
    #endregion

    #region 运动控制
    void HandleGroundCheck()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }
    }

    void HandleMovement()
    {
        Vector3 inputDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;

        if (inputDirection.magnitude >= 0.1f)
        {
            // 计算相对于摄像机的方向
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float smoothedAngle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y, 
                targetAngle, 
                ref rotationVelocity, 
                rotationSmoothTime
            );
            transform.rotation = Quaternion.Euler(0f, smoothedAngle, 0f);

            // 计算移动速度
            float currentSpeed = isRunning ? runSpeed : walkSpeed;
            
            // 移动角色
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDirection.normalized * currentSpeed * Time.deltaTime);
        }

        // 应用重力
        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    void HandleJump()
    {
        if (jumpPressed && isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpPressed = false;
        }
    }
    #endregion

    #region 摄像机控制
    void InitializeCamera()
    {
        if (cameraTransform != null)
        {
            Vector3 targetPosition = transform.position + 
                                   Vector3.up * cameraHeight + 
                                   -transform.forward * cameraDistance;
            cameraTransform.position = targetPosition;
            cameraTransform.LookAt(transform.position + Vector3.up * cameraHeight * 0.5f);
        }
    }

    void UpdateCameraPosition()
    {
        if (cameraTransform == null) return;

        // 计算理想摄像机位置
        Vector3 targetPosition = transform.position + 
                               Vector3.up * cameraHeight + 
                               -transform.forward * cameraDistance;

        // 摄像机碰撞检测
        RaycastHit hit;
        Vector3 rayStart = transform.position + Vector3.up * cameraHeight * 0.5f;
        if (Physics.Linecast(rayStart, targetPosition, out hit))
        {
            targetPosition = hit.point + (targetPosition - rayStart).normalized * 0.2f;
        }

        // 平滑移动摄像机
        cameraTransform.position = Vector3.SmoothDamp(
            cameraTransform.position, 
            targetPosition, 
            ref cameraVelocity, 
            cameraSmoothTime
        );

        // 摄像机看向角色
        cameraTransform.LookAt(transform.position + Vector3.up * cameraHeight * 0.5f);
    }
    #endregion

    #region 编辑器辅助
    void OnDrawGizmosSelected()
    {
        if (controller != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(
                transform.position + controller.center, 
                controller.radius
            );
            Gizmos.DrawLine(
                transform.position + controller.center - Vector3.up * (controller.height / 2 - controller.radius),
                transform.position + controller.center + Vector3.up * (controller.height / 2 - controller.radius)
            );
        }
    }
    #endregion
}