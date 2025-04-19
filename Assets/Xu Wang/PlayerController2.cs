using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController2 : MonoBehaviour
{
    // ===== 基础移动设置 =====
    [Header("MOVEMENT SETTINGS")]
    [Range(5, 15)] public float moveSpeed = 10f;
    [Range(100, 300)] public float rotationSpeed = 180f;
    public Transform cameraTransform;

    // ===== 输入系统 =====
    [Header("INPUT SYSTEM")]
    public InputActionAsset inputActions;
    private InputAction moveAction;
    private Vector2 moveInput;

    // ===== 音频系统 =====
    [Header("AUDIO SETTINGS")]
    public AudioSource coinCollectSound;
    public AudioSource winSound;

    // ===== 游戏逻辑 =====
    private int currentScore = 0;
    private bool isGameWon = false;
    private Rigidbody rb;

    // ===== UI系统 =====
    [Header("UI ELEMENTS")]
    public TextMeshProUGUI scoreText;
    public GameObject winMessage;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        // 输入系统初始化
        if (inputActions != null)
        {
            moveAction = inputActions.FindAction("Move");
        }
        else
        {
            Debug.LogError("InputActions未分配！请拖拽PlayerInput文件到Inspector", this);
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
        isGameWon = false;
        winMessage.SetActive(false);
        UpdateScoreDisplay();
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
        if (isGameWon) return;
        moveInput = moveAction?.ReadValue<Vector2>() ?? Vector2.zero;
    }

    void FixedUpdate()
    {
        if (isGameWon) return;

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
        if (isGameWon) return;

        if (other.CompareTag("pickup"))
        {
            CollectCoin(other.gameObject);
        }
    }

    void CollectCoin(GameObject coin)
    {
        coin.SetActive(false);
        currentScore += 5; // 每个金币+1分
        PlaySound(coinCollectSound);
        UpdateScoreDisplay();
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
        // 当分数达到100时胜利（可根据需要调整）
        if (currentScore >= 100)
        {
            ShowWinState();
        }
    }

    void ShowWinState()
    {
        isGameWon = true;
        winMessage.SetActive(true);
        PlaySound(winSound);
        DisableMovement();
    }

    void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore}";
        }
    }

    void DisableMovement()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
    }
}