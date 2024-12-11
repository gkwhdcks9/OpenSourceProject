using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target; // 카메라가 따라갈 대상 (플레이어)
    [SerializeField] private Vector3 offset;   // 카메라와 대상 사이의 거리
    [SerializeField] private float smoothSpeed = 0.125f; // 카메라 부드럽게 움직이는 속도

    void LateUpdate()
    {
        if (target != null)
        {
            // 목표 위치 계산
            Vector3 targetPosition = new Vector3(target.position.x + offset.x, transform.position.y, transform.position.z);

            // LERP로 부드럽게 이동
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
        }
    }
}
