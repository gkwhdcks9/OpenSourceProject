using UnityEngine;

public class Billboard : MonoBehaviour
{
    void LateUpdate()
    {
        if (Camera.main != null)
        {
            // 카메라의 회전에 맞춰 텍스트가 회전하도록 설정
            transform.rotation = Camera.main.transform.rotation;
        }
    }
}
