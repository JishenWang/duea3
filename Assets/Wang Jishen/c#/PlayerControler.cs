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
    public TextMeshProUGUI countText;
    public AudioSource clickAudio;
    [Header("AUDIO SETTINGS")]
    public AudioSource coinCollectSound;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        eaten = 0;
        totalBalls = 13;
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
            clickAudio.Play();
            PlaySound(coinCollectSound);
        }
        if (other.gameObject.CompareTag("pickup2"))
        {
            other.gameObject.SetActive(false);
            count += 2;
            eaten += 1;
            SetCountText();
            clickAudio.Play();
            PlaySound(coinCollectSound);
        }
        if (eaten >= totalBalls)
        {
            winPanel.SetActive(true); // 显示Panel（WinText会随之显示）

        }
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
