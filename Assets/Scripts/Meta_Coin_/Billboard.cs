using UnityEngine;

public class Billboard : MonoBehaviour
{
    void LateUpdate()
    {
        if (Camera.main != null)
        {
            // ī�޶��� ȸ���� ���� �ؽ�Ʈ�� ȸ���ϵ��� ����
            transform.rotation = Camera.main.transform.rotation;
        }
    }
}
