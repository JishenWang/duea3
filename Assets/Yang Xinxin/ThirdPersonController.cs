using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("�ƶ�����")]
    [Tooltip("�����ٶ�")] public float walkSpeed = 5f;
    [Tooltip("�����ٶ�")] public float runSpeed = 8f;
    [Tooltip("��Ծ�߶�")] public float jumpHeight = 2f;
    [Tooltip("����ϵ��")] public float gravity = -9.81f;
    [Tooltip("��ת�ٶ�")] public float rotationSpeed = 10f;
    [Tooltip("��תƽ��ʱ��")] public float rotationSmoothTime = 0.1f;

    [Header("���������")]
    [Tooltip("������ο�")] public Transform cameraTransform;
    [Tooltip("���������")] public float cameraDistance = 5f;
    [Tooltip("������߶�")] public float cameraHeight = 2f;
    [Tooltip("�����ƽ��ʱ��")] public float cameraSmoothTime = 0.3f;

    // ˽�б���
    private CharacterController _controller;
    private Vector3 _playerVelocity;
    private bool _isGrounded;
    private float _rotationVelocity;
    private Vector3 _cameraVelocity;
    private float _currentSpeed;

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        AdjustControllerForScale(); // �������ŵ���������

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

        // ��ȡԭʼ����
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 inputDirection = new Vector3(horizontal, 0, vertical).normalized;

        // ���������
        if (inputDirection.magnitude >= 0.1f)
        {
            // ����������������Ŀ��Ƕ�
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;

            // ƽ����ת��ɫ
            float smoothedAngle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                targetAngle,
                ref _rotationVelocity,
                rotationSmoothTime
            );
            transform.rotation = Quaternion.Euler(0f, smoothedAngle, 0f);

            // �����ƶ�����
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            _currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

            // Ӧ���ƶ�
            _controller.Move(moveDirection.normalized * (_currentSpeed * Time.deltaTime));
        }

        // Ӧ������
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

        // �������������λ��
        Vector3 targetPosition = transform.position +
                               transform.up * cameraHeight +
                               -transform.forward * cameraDistance;

        // �������ײ���
        RaycastHit hit;
        Vector3 rayStart = transform.position + transform.up * cameraHeight * 0.5f;
        if (Physics.Linecast(rayStart, targetPosition, out hit))
        {
            targetPosition = hit.point + (targetPosition - rayStart).normalized * 0.2f;
        }

        // ƽ���ƶ������
        cameraTransform.position = Vector3.SmoothDamp(
            cameraTransform.position,
            targetPosition,
            ref _cameraVelocity,
            cameraSmoothTime
        );

        // ����������ɫ
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