using UnityEngine;
using TMPro;

public class Go_Main : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshPro; // TextMeshProUGUI ����


    void Start()
    {
        if (textMeshPro != null)
        {
            textMeshPro.text = "GO HOME!"; // �ؽ�Ʈ ����
        }
        else
        {
            Debug.LogError("TextMeshPro�� �ν����Ϳ��� �Ҵ���� �ʾҽ��ϴ�!");
        }
    }
}
