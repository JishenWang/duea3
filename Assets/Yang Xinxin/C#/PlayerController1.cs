using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController1 : MonoBehaviour
{
    // �ƶ�����
    public float moveSpeed = 5f;
    public float rotationSpeed = 180f;

    // �������
    private Rigidbody rb;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction rotateAction;

    // ���뻺��
    private Vector2 currentMoveInput;
    private float currentRotateInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

        // ��ȡ���붯��
        moveAction = playerInput.actions["Move"];
        rotateAction = playerInput.actions["Rotate"];
    }

    void OnEnable()
    {
        // ע������ص�
        moveAction.performed += OnMovePerformed;
        moveAction.canceled += OnMoveCanceled;
        rotateAction.performed += OnRotatePerformed;
        rotateAction.canceled += OnRotateCanceled;

        moveAction.Enable();
        rotateAction.Enable();
    }

    void OnDisable()
    {
        // ע������ص�
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
        // �ƶ���ת��Ϊ3D�ռ䷽��
        Vector3 moveDirection = new Vector3(currentMoveInput.x, 0, currentMoveInput.y);
        rb.AddForce(moveDirection * moveSpeed, ForceMode.Force);

        // ��ת
        if (Mathf.Abs(currentRotateInput) > 0.1f)
        {
            float rotationAmount = currentRotateInput * rotationSpeed * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, rotationAmount, 0));
        }
    }
}