using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerControler1 : MonoBehaviour
{
    private Rigidbody rb;

    private float moveX;
    private float moveY;
    public float moveSpeed = 15;
    public float rotationSpeed = 180f; // 新增：旋转速度（度/秒）

    private int count;
    public TextMeshProUGUI countText;
    public AudioSource clickAudio;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        SetCountText();
    }

    public void OnMove(InputValue moveValue)
    {
        Vector2 moveVector = moveValue.Get<Vector2>();
        moveX = moveVector.x;
        moveY = moveVector.y;
    }

    private void FixedUpdate()
    {
        // 移动逻辑
        Vector3 movement = new Vector3(moveX, 0.0f, moveY);
        rb.AddForce(movement * moveSpeed);

        // 新增：旋转逻辑
        if (movement.magnitude > 0.1f) // 只有有输入时才旋转
        {
            // 计算目标旋转角度（将移动方向转换为旋转）
            float targetAngle = Mathf.Atan2(moveX, moveY) * Mathf.Rad2Deg;

            // 平滑旋转
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
            rb.rotation = Quaternion.RotateTowards(
                rb.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("pickup"))
        {
            other.gameObject.SetActive(false);
            count += 1;
            SetCountText();
            if (clickAudio != null) clickAudio.Play();
        }
        else if (other.gameObject.CompareTag("pickup2"))
        {
            other.gameObject.SetActive(false);
            count += 2;
            SetCountText();
            if (clickAudio != null) clickAudio.Play();
        }
    }

    void SetCountText()
    {
        if (countText != null)
        {
            countText.text = "Score:" + count.ToString();
        }
    }
}