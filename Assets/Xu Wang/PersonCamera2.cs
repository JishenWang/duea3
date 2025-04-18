using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PersonCamera2 : MonoBehaviour
{
    [Header("移动设置")]
    [Tooltip("行走速度")] public float walkSpeed = 5f;
    [Tooltip("奔跑速度")] public float runSpeed = 8f;
    [Tooltip("跳跃高度")] public float jumpHeight = 2f;
    [Tooltip("重力系数")] public float gravity = -9.81f;
    [Tooltip("旋转速度")] public float rotationSpeed = 10f;
    [Tooltip("旋转平滑时间")] public float rotationSmoothTime = 0.1f;

    [Header("摄像机设置")]
    [Tooltip("摄像机参考")] public Transform cameraTransform;
    [Tooltip("摄像机距离")] public float cameraDistance = 5f;
    [Tooltip("摄像机高度")] public float cameraHeight = 2f;
    [Tooltip("摄像机平滑时间")] public float cameraSmoothTime = 0.3f;

    // 私有变量
    private CharacterController _controller;
    private Vector3 _playerVelocity;
    private bool _isGrounded;
    private float _rotationVelocity;
    private Vector3 _cameraVelocity;
    private float _currentSpeed;

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        AdjustControllerForScale(); // 根据缩放调整控制器
        
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        InitializeCamera();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        UpdateCameraPosition();
    }

    void AdjustControllerForScale()
    {
        float scale = transform.localScale.y;
        _controller.height = 2f / scale;
        _controller.radius = 0.5f / scale;
        _controller.center = new Vector3(0, 1f / scale, 0);
        _controller.skinWidth = 0.08f / scale;
        _controller.stepOffset = 0.3f / scale;
    }

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

    void HandleMovement()
    {
        _isGrounded = _controller.isGrounded;
        if (_isGrounded && _playerVelocity.y < 0)
        {
            _playerVelocity.y = -2f;
        }

        // 获取原始输入
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 inputDirection = new Vector3(horizontal, 0, vertical).normalized;

        // 如果有输入
        if (inputDirection.magnitude >= 0.1f)
        {
            // 计算相对于摄像机的目标角度
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            
            // 平滑旋转角色
            float smoothedAngle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y, 
                targetAngle, 
                ref _rotationVelocity, 
                rotationSmoothTime
            );
            transform.rotation = Quaternion.Euler(0f, smoothedAngle, 0f);
            
            // 计算移动方向
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            _currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
            
            // 应用移动
            _controller.Move(moveDirection.normalized * (_currentSpeed * Time.deltaTime));
        }

        // 应用重力
        _playerVelocity.y += gravity * Time.deltaTime;
        _controller.Move(_playerVelocity * Time.deltaTime);
    }

    void HandleJump()
    {
        if (_isGrounded && Input.GetButtonDown("Jump"))
        {
            _playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void UpdateCameraPosition()
    {
        if (cameraTransform == null) return;

        // 计算理想摄像机位置
        Vector3 targetPosition = transform.position + 
                               transform.up * cameraHeight + 
                               -transform.forward * cameraDistance;

        // 摄像机碰撞检测
        RaycastHit hit;
        Vector3 rayStart = transform.position + transform.up * cameraHeight * 0.5f;
        if (Physics.Linecast(rayStart, targetPosition, out hit))
        {
            targetPosition = hit.point + (targetPosition - rayStart).normalized * 0.2f;
        }

        // 平滑移动摄像机
        cameraTransform.position = Vector3.SmoothDamp(
            cameraTransform.position, 
            targetPosition, 
            ref _cameraVelocity, 
            cameraSmoothTime
        );

        // 摄像机看向角色
        cameraTransform.LookAt(transform.position + transform.up * cameraHeight * 0.5f);
    }

    void OnDrawGizmosSelected()
    {
        if (_controller != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(
                transform.position + _controller.center, 
                _controller.radius
            );
            Gizmos.DrawLine(
                transform.position + _controller.center - Vector3.up * (_controller.height / 2 - _controller.radius),
                transform.position + _controller.center + Vector3.up * (_controller.height / 2 - _controller.radius)
            );
        }
    }
}