using UnityEngine;

public class GoatMovement : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>(); // 获取Animator组件
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            animator.SetBool("isWalking", true); // 按住W，播放走路动画
            transform.Translate(Vector3.forward * Time.deltaTime); // 实现向前移动
        }
        else
        {
            animator.SetBool("isWalking", false); // 松开W，停止走路动画
        }
    }
}
