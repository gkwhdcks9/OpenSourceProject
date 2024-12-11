using UnityEngine;
using TMPro;

public class Go_Main : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshPro; // TextMeshProUGUI 연결


    void Start()
    {
        if (textMeshPro != null)
        {
            textMeshPro.text = "GO HOME!"; // 텍스트 설정
        }
        else
        {
            Debug.LogError("TextMeshPro가 인스펙터에서 할당되지 않았습니다!");
        }
    }
}
