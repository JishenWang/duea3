using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController1 : MonoBehaviour
{
    // 移动参数
    public float moveSpeed = 5f;
    public float rotationSpeed = 180f;

    // 组件引用
    private Rigidbody rb;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction rotateAction;

    // 输入缓存
    private Vector2 currentMoveInput;
    private float currentRotateInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

        // 获取输入动作
        moveAction = playerInput.actions["Move"];
        rotateAction = playerInput.actions["Rotate"];
    }

    void OnEnable()
    {
        // 注册输入回调
        moveAction.performed += OnMovePerformed;
        moveAction.canceled += OnMoveCanceled;
        rotateAction.performed += OnRotatePerformed;
        rotateAction.canceled += OnRotateCanceled;

        moveAction.Enable();
        rotateAction.Enable();
    }

    void OnDisable()
    {
        // 注销输入回调
        moveAction.performed -= OnMovePerformed;
        moveAction.canceled -= OnMoveCanceled;
        rotateAction.performed -= OnRotatePerformed;
        rotateAction.canceled -= OnRotateCanceled;
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        currentMoveInput = ctx.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        currentMoveInput = Vector2.zero;
    }

    private void OnRotatePerformed(InputAction.CallbackContext ctx)
    {
        currentRotateInput = ctx.ReadValue<float>();
    }

    private void OnRotateCanceled(InputAction.CallbackContext ctx)
    {
        currentRotateInput = 0f;
    }

    void FixedUpdate()
    {
        // 移动（转换为3D空间方向）
        Vector3 moveDirection = new Vector3(currentMoveInput.x, 0, currentMoveInput.y);
        rb.AddForce(moveDirection * moveSpeed, ForceMode.Force);

        // 旋转
        if (Mathf.Abs(currentRotateInput) > 0.1f)
        {
            float rotationAmount = currentRotateInput * rotationSpeed * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, rotationAmount, 0));
        }
    }
}