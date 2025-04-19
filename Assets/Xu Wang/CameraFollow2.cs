using UnityEngine;

public class CameraFollow2 : MonoBehaviour
{
    [Header("跟随目标")]
    public Transform target;  // 要跟随的人物

    [Header("摄像机位置")]
    public float height = 2.0f;      // 摄像机相对于人物的高度
    public float distance = 3.0f;     // 摄像机与人物之间的距离
    public float angle = 30f;         // 摄像机的俯角（度数）

    [Header("跟随平滑度")]
    public float positionSmoothTime = 0.3f;  // 位置平滑时间
    public float rotationSmoothTime = 0.1f;  // 旋转平滑时间

    private Vector3 positionVelocity;
    private float rotationVelocity;

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("没有设置跟随目标！");
            return;
        }

        // 计算摄像机应该位于的球面坐标位置
        Vector3 targetPosition = target.position;
        
        // 将角度转换为弧度
        float radians = angle * Mathf.Deg2Rad;
        
        // 计算摄像机偏移量
        Vector3 offset = new Vector3(
            0,
            Mathf.Sin(radians) * distance,      // 垂直分量
            -Mathf.Cos(radians) * distance      // 水平分量
        );
        
        // 加上基础高度
        offset.y += height;
        
        // 计算目标摄像机位置
        Vector3 desiredPosition = targetPosition + target.TransformDirection(offset);
        
        // 平滑移动摄像机位置
        transform.position = Vector3.SmoothDamp(
            transform.position, 
            desiredPosition, 
            ref positionVelocity, 
            positionSmoothTime
        );
        
        // 计算摄像机应该看向的方向（稍微看向人物上方，使画面更自然）
        Vector3 lookAtPosition = targetPosition + Vector3.up * height * 0.5f;
        
        // 平滑旋转摄像机
        Quaternion desiredRotation = Quaternion.LookRotation(lookAtPosition - transform.position);
        transform.rotation = Quaternion.Slerp(
            transform.rotation, 
            desiredRotation, 
            rotationSmoothTime * Time.deltaTime * 10f
        );
    }
}