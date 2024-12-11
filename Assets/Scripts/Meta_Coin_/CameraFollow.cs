using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target; // ī�޶� ���� ��� (�÷��̾�)
    [SerializeField] private Vector3 offset;   // ī�޶�� ��� ������ �Ÿ�
    [SerializeField] private float smoothSpeed = 0.125f; // ī�޶� �ε巴�� �����̴� �ӵ�

    void LateUpdate()
    {
        if (target != null)
        {
            // ��ǥ ��ġ ���
            Vector3 targetPosition = new Vector3(target.position.x + offset.x, transform.position.y, transform.position.z);

            // LERP�� �ε巴�� �̵�
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
        }
    }
}
