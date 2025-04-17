using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerControler1 : MonoBehaviour
{
    private Rigidbody rb;

    private float moveX;
    private float moveY;
    public float moveSpeed = 1;

    private float rotateInput;
    public float rotationSpeed = 500f;

    private int count;
    public TextMeshProUGUI countText;
    public AudioSource clickAudio;

    //Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        clickAudio = GetComponent<AudioSource>();
        //SetCountText();
    }

    public void OnMove(InputValue moveValue)
    {
        Vector2 moveVector = moveValue.Get<Vector2>();
        moveX = moveVector.x;
        moveY = moveVector.y;
    }

    public void OnRotate(InputValue rotateValue)
    {
        rotateInput = rotateValue.Get<float>();
    }

    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(moveX, 0.0f, moveY);
        rb.AddForce(movement * moveSpeed);

        // Ó¦ÓÃÐý×ª
        Quaternion rotation = Quaternion.Euler(0f, rotateInput * rotationSpeed * Time.fixedDeltaTime, 0f);
        rb.MoveRotation(rb.rotation * rotation);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("pickup"))
        {
            other.gameObject.SetActive(false);
            count += 1;
            //SetCountText();
            clickAudio.Play();
        }
        if (other.gameObject.CompareTag("pickup2"))
        {
            other.gameObject.SetActive(false);
            count += 2;
            //SetCountText();
            clickAudio.Play();
        }
    }

    //public void SetCountText()
    //{
        //countText.text = "Score:" + count.ToString();
    //}
}