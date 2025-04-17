using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;          // �ƶ��ٶ�
    public float rotationSpeed = 10f;     // ��ת�ٶ�

    [Header("Animation Parameters")]
    public string dizzyStateName = "Dizzy";  // Ĭ��ѣ��״̬����
    public string runParameterName = "IsRunning"; // �ܲ�����������

    private Animator animator;            // ����������
    private Rigidbody rb;                // �������
    private Vector3 movement;             // �ƶ�����
    private bool isMoving = false;        // �Ƿ����ƶ�

    void Start()
    {
        // ��ȡ���
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        // ȷ����ʼ״̬��Dizzy����
        if (animator != null)
        {
            animator.Play(dizzyStateName);
        }
    }

    void Update()
    {
        // ��ȡ����
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // �����ƶ�����
        movement = new Vector3(horizontal, 0f, vertical).normalized;

        // ����Ƿ����ƶ�����
        isMoving = movement.magnitude > 0.1f;

        // ���ƶ���״̬
        if (animator != null)
        {
            animator.SetBool(runParameterName, isMoving);
        }
    }

    void FixedUpdate()
    {
        // �����ƶ�
        if (isMoving)
        {
            // �ƶ���ɫ
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

            // ��ת��ɫ�����ƶ�����
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            rb.rotation = Quaternion.Slerp(rb.transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }
}