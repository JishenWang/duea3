using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;


public class PlayerControler : MonoBehaviour
{
    private Rigidbody rb;

    private float moveX;
    private float moveY;
    public float moveSpeed = 15;
    public int eaten;
    public int totalBalls;
    public GameObject winPanel;

    private int count;
    private int totalPickups; // ��������¼�ܽ����
    public TextMeshProUGUI countText;

    // ===== ��Ƶϵͳ =====
    [Header("AUDIO SETTINGS")]
    public AudioSource coinCollectSound;

    // ===== ʤ��UI =====
    [Header("UI SETTINGS")]
    public GameObject winPanel; // ������ʤ�����
    public TextMeshProUGUI winText; // ������ʤ������
    private bool gameEnded = false; // ��������Ϸ�Ƿ����

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        eaten = 0;
        totalBalls = 13;
        SetCountText();
        InitializeWinUI(); // ��ʼ��ʤ��UI
    }

    // ��������ʼ��ʤ��UI
    void InitializeWinUI()
    {
        if (winPanel != null) winPanel.SetActive(false);
        if (winText != null) winText.text = "YOU WIN!";
    }

    public void OnMove(InputValue moveValue)
    {
        // ��Ϸ����ʱ��ֹ�ƶ�
        if (gameEnded) return;

        Vector2 moveVector = moveValue.Get<Vector2>();
        moveX = moveVector.x;
        moveY = moveVector.y;
    }

    private void FixedUpdate()
    {
        // ��Ϸ����ʱ��ֹ�ƶ�
        if (gameEnded) return;

        Vector3 movement = new Vector3(moveX, 0.0f, moveY);
        rb.AddForce(movement * moveSpeed);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("pickup"))
        {
            other.gameObject.SetActive(false);
            count += 1;
            eaten += 1;
            SetCountText();
            PlaySound(coinCollectSound);
            CheckWinCondition(); // ����Ƿ�ʤ��
        }
        if (other.gameObject.CompareTag("pickup2"))
        {
            other.gameObject.SetActive(false);
            count += 2;
            eaten += 1;
            SetCountText();
            PlaySound(coinCollectSound);
            CheckWinCondition(); // ����Ƿ�ʤ��
        }
        if (eaten >= totalBalls)
        {
            winPanel.SetActive(true); // ��ʾPanel��WinText����֮��ʾ��

        }
    }

    // ���������ʤ������
    void CheckWinCondition()
    {
        if (count >= totalPickups)
        {
            EndGame();
        }
    }

    // ������������Ϸ
    void EndGame()
    {
        gameEnded = true;
        rb.velocity = Vector3.zero; // ֹͣ��ɫ�ƶ�
        if (winPanel != null) winPanel.SetActive(true);
    }

    public void SetCountText()
    {
        countText.text = "Score:" + count.ToString();
    }

    void PlaySound(AudioSource sound)
    {
        if (sound != null && !sound.isPlaying)
        {
            sound.Play();
        }
    }
}