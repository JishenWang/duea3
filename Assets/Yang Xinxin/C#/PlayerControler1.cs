using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 180f;

    private Rigidbody rb;
    private InputAction moveAction;
    private InputAction rotateAction;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // 加载输入配置
        InputActionAsset inputAsset = Resources.Load<InputActionAsset>("PlayerInput");
        if (inputAsset == null)
        {
            Debug.LogError("Missing PlayerInput.inputactions in Resources folder!");
            return;
        }

        // 获取动作
        moveAction = inputAsset.FindAction("Move");
        rotateAction = inputAsset.FindAction("Rotate");
    }

    void OnEnable()
    {
        moveAction.Enable();
        rotateAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        rotateAction.Disable();
    }

    void FixedUpdate()
    {
        // 移动
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y);
        rb.AddForce(movement * moveSpeed, ForceMode.Force);

        // 旋转
        float rotateInput = rotateAction.ReadValue<float>();
        if (Mathf.Abs(rotateInput) > 0.1f)
        {
            Quaternion deltaRotation = Quaternion.Euler(0, rotateInput * rotationSpeed * Time.fixedDeltaTime, 0);
            rb.MoveRotation(rb.rotation * deltaRotation);
        }
    }
}