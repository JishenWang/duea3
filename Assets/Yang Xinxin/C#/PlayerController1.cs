using UnityEngine;
using UnityEngine.InputSystem;
using TMPro; // 新增：用于文本显示

public class PlayerController1 : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 180f;
    [Header("Camera")]
    public Transform cameraTransform; // 新增：摄像机参考

    [Header("Scoring")] // 新增：积分相关设置
    public AudioSource clickAudio; // 拾取音效
    private int count = 0; // 当前积分

    private Rigidbody rb;
    private InputAction moveAction;
    private Vector2 moveInput;

    [Header("UI")]
    public TextMeshProUGUI countText;
    public GameObject winMessage; // 拖入一个"You Win!"的UI面板
    public GameObject trophyHint; // 拖入一个"你需要找到奖杯！"的UI面板

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // 自动获取主摄像机（如果没有手动指定）
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        // 加载输入配置
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
        // 新增：初始化积分显示
        SetCountText();
    }

    void OnEnable() => moveAction.Enable();
    void OnDisable() => moveAction.Disable();

    void Update()
    {
        // 每帧获取输入（更灵敏）
        moveInput = moveAction.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        if (moveInput.magnitude > 0.1f)
        {
            // 1. 基于摄像机方向计算移动
            Vector3 cameraForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 moveDirection = (cameraForward * moveInput.y + cameraTransform.right * moveInput.x).normalized;

            // 2. 应用移动力
            rb.AddForce(moveDirection * moveSpeed, ForceMode.Force);

            // 3. 自动朝向移动方向
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.MoveRotation(Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            ));
        }
    }

    // 新增：碰撞检测方法
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
            // 显示胜利UI
            if (winMessage != null) winMessage.SetActive(true);
            if (clickAudio != null) clickAudio.Play();
        }
    }

    private void SetCountText()
    {
        if (countText != null)
        {
            countText.text = "Score: " + count.ToString();

            // 分数达到100时显示提示
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