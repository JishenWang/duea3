using UnityEngine;

public class FerrisWheelRotation : MonoBehaviour
{
    public float rotationSpeed = 10f; // Ħ������ת�ٶȣ�����Inspector������

    void Update()
    {
        // �����ƾֲ�Z����ת
        transform.Rotate(transform.forward, rotationSpeed * Time.deltaTime);
    }
}