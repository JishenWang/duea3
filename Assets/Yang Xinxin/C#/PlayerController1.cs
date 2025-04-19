using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController1 : MonoBehaviour
{
    // ===== 基础设置 =====
    [Header("MOVEMENT SETTINGS")]
    [Range(5, 15)] public float moveSpeed = 10f;
    [Range(100, 300)] public float rotationSpeed = 180f;

    [Header("CAMERA REFERENCE")]
    public Transform cameraTransform;

    // ===== 输入系统 =====
    [Header("INPUT SYSTEM")]
    [Tooltip("直接拖拽PlayerInput.inputactions文件到这里")]
    public InputActionAsset inputActions;
    private InputAction moveAction;
    private Vector2 moveInput;

    // ===== 音频系统 =====
    [Header("AUDIO SETTINGS")]
    public AudioSource coinCollectSound;
    public AudioSource winSound;

    // ===== 游戏逻辑 =====
    private int currentScore = 0;
    private bool hasFoundSpecialItem = false;
    private bool isGameWon = false; // 新增：游戏胜利状态
    private Rigidbody rb;

    // ===== UI系统 =====
    [Header("UI ELEMENTS")]
    public TextMeshProUGUI scoreText;
    public GameObject winMessage;
    public GameObject trophyHint;
    public GameObject notEnoughMessage;

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
        hasFoundSpecialItem = false;
        isGameWon = false; // 重置胜利状态
        StopAllAudio();
        UpdateScoreDisplay();
        SetUIState(false, false, false);

        // 确保可以移动
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
        if (isGameWon) return; // 胜利后不处理输入

        moveInput = moveAction?.ReadValue<Vector2>() ?? Vector2.zero;
    }

    void FixedUpdate()
    {
        if (isGameWon) return; // 胜利后不处理移动

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
        if (isGameWon) return; // 胜利后不处理碰撞

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
        isGameWon = true; // 标记游戏胜利
        SetUIState(true, false, false);
        PlaySound(winSound);
        DisableMovement(); // 禁用移动
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

    // 新增：禁用移动功能
    void DisableMovement()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true; // 防止物理推动
    }

    // 新增：启用移动功能
    void EnableMovement()
    {
        rb.isKinematic = false;
    }
}