using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController1 : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 180f;

    [Header("Camera")]
    public Transform cameraTransform;

    [Header("Scoring")]
    public AudioSource clickAudio;
    private int count = 0;

    [Header("UI")]
    public TextMeshProUGUI countText;
    public GameObject winMessage;
    public GameObject trophyHint;
    public GameObject notEnoughMessage;

    private Rigidbody rb;
    private InputAction moveAction;
    private Vector2 moveInput;
    private bool hasCollidedWithPickup2 = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

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
        // 初始化所有UI为隐藏状态
        if (winMessage != null) winMessage.SetActive(false);
        if (trophyHint != null) trophyHint.SetActive(false);
        if (notEnoughMessage != null) notEnoughMessage.SetActive(false);
    }

    void OnEnable() => moveAction.Enable();
    void OnDisable() => moveAction.Disable();

    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        if (moveInput.magnitude > 0.1f)
        {
            Vector3 cameraForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 moveDirection = (cameraForward * moveInput.y + cameraTransform.right * moveInput.x).normalized;

            rb.AddForce(moveDirection * moveSpeed, ForceMode.Force);

            // 限制最大速度
            if (rb.velocity.magnitude > moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }

            // 旋转朝向移动方向
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.MoveRotation(Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            ));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("pickup"))
        {
            HandlePickupCollision(other);
        }
        else if (other.gameObject.CompareTag("pickup2"))
        {
            HandlePickup2Collision(other);
        }
    }

    private void HandlePickupCollision(Collider pickup)
    {
        pickup.gameObject.SetActive(false);
        count += 5;
        SetCountText();

        // 检查是否满足胜利条件
        if (hasCollidedWithPickup2 && count >= 100)
        {
            ShowWinMessage();
        }

        PlayClickSound();
    }

    private void HandlePickup2Collision(Collider pickup2)
    {
        pickup2.gameObject.SetActive(false);
        hasCollidedWithPickup2 = true;

        if (count >= 100)
        {
            ShowWinMessage();
        }
        else
        {
            ShowNotEnoughMessage();
        }

        PlayClickSound();
    }

    private void ShowWinMessage()
    {
        if (winMessage != null) winMessage.SetActive(true);
        if (notEnoughMessage != null) notEnoughMessage.SetActive(false);
        if (trophyHint != null) trophyHint.SetActive(false);
    }

    private void ShowNotEnoughMessage()
    {
        if (notEnoughMessage != null)
        {
            notEnoughMessage.SetActive(true);
            Invoke("HideNotEnoughMessage", 3f); // 3秒后自动隐藏
        }
    }

    private void HideNotEnoughMessage()
    {
        if (notEnoughMessage != null)
        {
            notEnoughMessage.SetActive(false);
        }
    }

    private void PlayClickSound()
    {
        if (clickAudio != null) clickAudio.Play();
    }

    private void SetCountText()
    {
        if (countText != null)
        {
            countText.text = $"Score: {count}";

            // 分数达到100时的处理
            if (count >= 100)
            {
                if (trophyHint != null) trophyHint.SetActive(true);

                // 如果之前已经碰撞过pickup2
                if (hasCollidedWithPickup2)
                {
                    ShowWinMessage();
                }
            }
            else
            {
                if (trophyHint != null) trophyHint.SetActive(false);
            }
        }
    }
}