using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerControler : MonoBehaviour
{
    private Rigidbody rb;

    private float moveX;
    private float moveY;
    public float moveSpeed = 15;

    private int count;
    private int totalPickups; // 新增：记录总金币数
    public TextMeshProUGUI countText;

    // ===== 音频系统 =====
    [Header("AUDIO SETTINGS")]
    public AudioSource coinCollectSound;

    // ===== 胜利UI =====
    [Header("UI SETTINGS")]
    public GameObject winPanel; // 新增：胜利面板
    public TextMeshProUGUI winText; // 新增：胜利文字
    private bool gameEnded = false; // 新增：游戏是否结束

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;

        // 计算场景中所有pickup和pickup2的数量
        totalPickups = GameObject.FindGameObjectsWithTag("pickup").Length
                     + GameObject.FindGameObjectsWithTag("pickup2").Length;

        SetCountText();
        InitializeWinUI(); // 初始化胜利UI
    }

    // 新增：初始化胜利UI
    void InitializeWinUI()
    {
        if (winPanel != null) winPanel.SetActive(false);
        if (winText != null) winText.text = "YOU WIN!";
    }

    public void OnMove(InputValue moveValue)
    {
        // 游戏结束时禁止移动
        if (gameEnded) return;

        Vector2 moveVector = moveValue.Get<Vector2>();
        moveX = moveVector.x;
        moveY = moveVector.y;
    }

    private void FixedUpdate()
    {
        // 游戏结束时禁止移动
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
            SetCountText();
            PlaySound(coinCollectSound);
            CheckWinCondition(); // 检查是否胜利
        }
        if (other.gameObject.CompareTag("pickup2"))
        {
            other.gameObject.SetActive(false);
            count += 2;
            SetCountText();
            PlaySound(coinCollectSound);
            CheckWinCondition(); // 检查是否胜利
        }
    }

    // 新增：检查胜利条件
    void CheckWinCondition()
    {
        if (count >= totalPickups)
        {
            EndGame();
        }
    }

    // 新增：结束游戏
    void EndGame()
    {
        gameEnded = true;
        rb.velocity = Vector3.zero; // 停止角色移动
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