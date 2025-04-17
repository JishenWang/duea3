using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("跟随目标")]
    public Transform target;

    [Header("位置设置")]
    public float distance = 5f;
    public float height = 2f;
    public float angle = 30f;

    [Header("平滑设置")]
    public float positionSmoothTime = 0.3f;
    public float rotationSmoothTime = 0.1f;

    private Vector3 positionVelocity;
    private float rotationVelocity;

    void LateUpdate()
    {
        if (target == null) return;

        // 计算摄像机位置
        float verticalAngle = angle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(
            0,
            Mathf.Sin(verticalAngle) * distance + height,
            -Mathf.Cos(verticalAngle) * distance
        );

        // 平滑移动
        Vector3 targetPosition = target.position + target.TransformDirection(offset);
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref positionVelocity,
            positionSmoothTime
        );

        // 平滑旋转
        Vector3 lookAtPosition = target.position + Vector3.up * height * 0.5f;
        Quaternion targetRotation = Quaternion.LookRotation(lookAtPosition - transform.position);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSmoothTime * Time.deltaTime * 10f
        );
    }
}