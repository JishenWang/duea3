using UnityEngine;
using UnityEngine.InputSystem;
using TMPro; // �����������ı���ʾ

public class PlayerController1 : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 180f;
    [Header("Camera")]
    public Transform cameraTransform; // ������������ο�

    [Header("Scoring")] // �����������������
    public AudioSource clickAudio; // ʰȡ��Ч
    private int count = 0; // ��ǰ����

    private Rigidbody rb;
    private InputAction moveAction;
    private Vector2 moveInput;

    [Header("UI")]
    public TextMeshProUGUI countText;
    public GameObject winMessage; // ����һ��"You Win!"��UI���
    public GameObject trophyHint; // ����һ��"����Ҫ�ҵ�������"��UI���

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // �Զ���ȡ������������û���ֶ�ָ����
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        // ������������
        InputActionAsset inputAsset = Resources.Load<InputActionAsset>("PlayerInput");
        if (inputAsset == null)
        {
            Debug.LogError("Missing PlayerInput.inputactions in Resources folder!");
            return;
        }
        moveAction = inputAsset.FindAction("Move");
    }

    void Start()
    {
        // ��������ʼ��������ʾ
        SetCountText();
    }

    void OnEnable() => moveAction.Enable();
    void OnDisable() => moveAction.Disable();

    void Update()
    {
        // ÿ֡��ȡ���루��������
        moveInput = moveAction.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        if (moveInput.magnitude > 0.1f)
        {
            // 1. �����������������ƶ�
            Vector3 cameraForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 moveDirection = (cameraForward * moveInput.y + cameraTransform.right * moveInput.x).normalized;

            // 2. Ӧ���ƶ���
            rb.AddForce(moveDirection * moveSpeed, ForceMode.Force);

            // 3. �Զ������ƶ�����
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.MoveRotation(Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            ));
        }
    }

    // ��������ײ��ⷽ��
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("pickup"))
        {
            other.gameObject.SetActive(false);
            count += 2;
            SetCountText();
            if (clickAudio != null) clickAudio.Play();
        }
        else if (other.gameObject.CompareTag("pickup2"))
        {
            other.gameObject.SetActive(false);
            // ��ʾʤ��UI
            if (winMessage != null) winMessage.SetActive(true);
            if (clickAudio != null) clickAudio.Play();
        }
    }

    private void SetCountText()
    {
        if (countText != null)
        {
            countText.text = "Score: " + count.ToString();

            // �����ﵽ100ʱ��ʾ��ʾ
            if (count >= 100 && trophyHint != null)
            {
                trophyHint.SetActive(true);
            }
            else if (trophyHint != null)
            {
                trophyHint.SetActive(false);
            }
        }
    }
}