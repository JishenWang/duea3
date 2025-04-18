using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PersonCamera: MonoBehaviour
{
    [Header("移动设置")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public float rotationSmoothTime = 0.1f;

    [Header("摄像机设置")]
    public Transform cameraTransform;
    public float cameraDistance = 5f;
    public float cameraHeight = 2f;
    public float cameraSmoothTime = 0.3f;

    [Header("分数系统")]
    public TextMeshProUGUI scoreText;
    public AudioClip coinSound;
    private int score = 0;
    private AudioSource audioSource;

    [Header("星级评分")]
    public Image star1;
    public Image star2;
    public Image star3;
    public Sprite goldStar;
    public Sprite grayStar;
    public AudioClip starSound;

    [Header("胜利设置")]
    public GameObject winPanel;
    public TextMeshProUGUI winText;
    public AudioClip winSound;
    public ParticleSystem winEffect;
    public float winDelay = 1.5f;

    private const int MAX_SCORE = 96;
    private CharacterController controller;
    private PlayerInput playerInput;
    private Vector2 moveInput;
    private bool jumpPressed;
    private bool isRunning;
    private Vector3 playerVelocity;
    private bool isGrounded;
    private float rotationVelocity;
    private Vector3 cameraVelocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        audioSource = GetComponent<AudioSource>();
        
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        InitializeStars();
    }

    void Start()
    {
        InitializeCamera();
        UpdateScoreText();
    }

    void Update()
    {
        HandleGroundCheck();
        HandleMovement();
        HandleJump();
        UpdateCameraPosition();
    }

    #region 初始化
    void InitializeStars()
    {
        star1.sprite = grayStar;
        star2.sprite = grayStar;
        star3.sprite = grayStar;
        
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }
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
    #endregion

    #region 输入控制
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

    #region 游戏逻辑
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            CollectCoin(other.gameObject);
        }
        else if (other.CompareTag("Trophy"))
        {
            StartCoroutine(CheckWinCondition());
        }
    }

    void CollectCoin(GameObject coin)
    {
        score++;
        UpdateScoreText();
        
        if (coinSound != null)
        {
            audioSource.PlayOneShot(coinSound);
        }

        coin.SetActive(false);
    }

    IEnumerator CheckWinCondition()
    {
        // 确保分数更新完成
        yield return null;
        
        if (score >= MAX_SCORE)
        {
            WinGame(3);
        }
        else if (score >= 64)
        {
            WinGame(2);
        }
        else if (score >= 32)
        {
            WinGame(1);
        }
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}/{MAX_SCORE}";
        }
        UpdateStars();
    }

    void UpdateStars()
    {
        if (score >= 32) star1.sprite = goldStar;
        if (score >= 64) star2.sprite = goldStar;
        if (score >= MAX_SCORE) star3.sprite = goldStar;
    }
    #endregion

    #region 胜利系统
    void WinGame(int stars)
    {
        StartCoroutine(WinSequence(stars));
    }

    IEnumerator WinSequence(int stars)
    {
        // 禁用控制
        enabled = false;
        
        // 播放胜利音效
        if (winSound != null)
        {
            audioSource.PlayOneShot(winSound);
        }

        // 触发特效
        if (winEffect != null)
        {
            winEffect.Play();
        }

        // 渐进式点亮星星
        yield return StartCoroutine(LightStarsSequentially(stars));

        // 显示胜利面板
        if (winPanel != null)
        {
            winPanel.SetActive(true);
            if (winText != null)
            {
                winText.text = stars == 3 ? "PERFECT!" : "YOU WIN!";
            }
        }
    }

    IEnumerator LightStarsSequentially(int count)
    {
        Image[] stars = { star1, star2, star3 };
        
        for (int i = 0; i < count; i++)
        {
            if (i < stars.Length)
            {
                stars[i].sprite = goldStar;
                if (starSound != null)
                {
                    audioSource.PlayOneShot(starSound);
                }
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
    #endregion

    #region 移动和摄像机控制
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
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float smoothedAngle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y, 
                targetAngle, 
                ref rotationVelocity, 
                rotationSmoothTime
            );
            transform.rotation = Quaternion.Euler(0f, smoothedAngle, 0f);

            float currentSpeed = isRunning ? runSpeed : walkSpeed;
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDirection.normalized * currentSpeed * Time.deltaTime);
        }

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

    void UpdateCameraPosition()
    {
        if (cameraTransform == null) return;

        Vector3 targetPosition = transform.position + 
                               Vector3.up * cameraHeight + 
                               -transform.forward * cameraDistance;

        RaycastHit hit;
        Vector3 rayStart = transform.position + Vector3.up * cameraHeight * 0.5f;
        if (Physics.Linecast(rayStart, targetPosition, out hit))
        {
            targetPosition = hit.point + (targetPosition - rayStart).normalized * 0.2f;
        }

        cameraTransform.position = Vector3.SmoothDamp(
            cameraTransform.position, 
            targetPosition, 
            ref cameraVelocity, 
            cameraSmoothTime
        );

        cameraTransform.LookAt(transform.position + Vector3.up * cameraHeight * 0.5f);
    }
    #endregion
}