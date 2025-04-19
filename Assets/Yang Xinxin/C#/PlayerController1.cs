using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController1 : MonoBehaviour
{
    // ===== �������� =====
    [Header("MOVEMENT SETTINGS")]
    [Range(5, 15)] public float moveSpeed = 10f;
    [Range(100, 300)] public float rotationSpeed = 180f;

    [Header("CAMERA REFERENCE")]
    public Transform cameraTransform;

    // ===== ����ϵͳ =====
    [Header("INPUT SYSTEM")]
    [Tooltip("ֱ����קPlayerInput.inputactions�ļ�������")]
    public InputActionAsset inputActions;
    private InputAction moveAction;
    private Vector2 moveInput;

    // ===== ��Ƶϵͳ =====
    [Header("AUDIO SETTINGS")]
    public AudioSource coinCollectSound;
    public AudioSource winSound;

    // ===== ��Ϸ�߼� =====
    private int currentScore = 0;
    private bool hasFoundSpecialItem = false;
    private bool isGameWon = false; // ��������Ϸʤ��״̬
    private Rigidbody rb;

    // ===== UIϵͳ =====
    [Header("UI ELEMENTS")]
    public TextMeshProUGUI scoreText;
    public GameObject winMessage;
    public GameObject trophyHint;
    public GameObject notEnoughMessage;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        // ����ϵͳ��ʼ��
        if (inputActions != null)
        {
            moveAction = inputActions.FindAction("Move");
        }
        else
        {
            Debug.LogError("InputActionsδ���䣡����קPlayerInput�ļ���Inspector", this);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }

    void Start()
    {
        InitializeGame();
    }

    void InitializeGame()
    {
        currentScore = 0;
        hasFoundSpecialItem = false;
        isGameWon = false; // ����ʤ��״̬
        StopAllAudio();
        UpdateScoreDisplay();
        SetUIState(false, false, false);

        // ȷ�������ƶ�
        EnableMovement();
    }

    void StopAllAudio()
    {
        coinCollectSound?.Stop();
        winSound?.Stop();
    }

    void OnEnable()
    {
        moveAction?.Enable();
    }

    void OnDisable()
    {
        moveAction?.Disable();
    }

    void Update()
    {
        if (isGameWon) return; // ʤ���󲻴�������

        moveInput = moveAction?.ReadValue<Vector2>() ?? Vector2.zero;
    }

    void FixedUpdate()
    {
        if (isGameWon) return; // ʤ���󲻴����ƶ�

        if (moveInput.magnitude > 0.1f)
        {
            HandleMovement();
        }
    }

    void HandleMovement()
    {
        Vector3 camForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 moveDir = (camForward * moveInput.y + cameraTransform.right * moveInput.x).normalized;

        rb.AddForce(moveDir * moveSpeed, ForceMode.Force);
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, moveSpeed);

        if (moveDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime));
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isGameWon) return; // ʤ���󲻴�����ײ

        if (other.CompareTag("pickup"))
        {
            CollectCoin(other.gameObject);
        }
        else if (other.CompareTag("pickup2"))
        {
            CollectSpecialItem(other.gameObject);
        }
    }

    void CollectCoin(GameObject coin)
    {
        coin.SetActive(false);
        currentScore += 5;
        PlaySound(coinCollectSound);
        UpdateScoreDisplay();
        CheckWinCondition();
    }

    void CollectSpecialItem(GameObject item)
    {
        item.SetActive(false);
        hasFoundSpecialItem = true;
        PlaySound(coinCollectSound);
        CheckWinCondition();
    }

    void PlaySound(AudioSource sound)
    {
        if (sound != null && !sound.isPlaying)
        {
            sound.Play();
        }
    }

    void CheckWinCondition()
    {
        if (currentScore >= 100 && hasFoundSpecialItem)
        {
            ShowWinState();
        }
        else if (hasFoundSpecialItem)
        {
            ShowNotEnoughCoins();
        }
    }

    void ShowWinState()
    {
        isGameWon = true; // �����Ϸʤ��
        SetUIState(true, false, false);
        PlaySound(winSound);
        DisableMovement(); // �����ƶ�
    }

    void ShowNotEnoughCoins()
    {
        SetUIState(false, false, true);
        Invoke(nameof(HideNotEnoughMessage), 3f);
    }

    void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore}";
            trophyHint?.SetActive(currentScore >= 100);
        }
    }

    void SetUIState(bool win, bool trophy, bool notEnough)
    {
        winMessage?.SetActive(win);
        trophyHint?.SetActive(trophy);
        notEnoughMessage?.SetActive(notEnough);
    }

    void HideNotEnoughMessage()
    {
        notEnoughMessage?.SetActive(false);
    }

    // �����������ƶ�����
    void DisableMovement()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true; // ��ֹ�����ƶ�
    }

    // �����������ƶ�����
    void EnableMovement()
    {
        rb.isKinematic = false;
    }
}