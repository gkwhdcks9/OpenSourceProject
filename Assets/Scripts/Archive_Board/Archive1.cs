using UnityEngine;
using TMPro;

public class Archive1 : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speechBubbleText; // UI �ؽ�Ʈ
    [SerializeField] private string message = ""; // ǥ���� �޽���

    private void Start()
    {
        // ���� �� �ؽ�Ʈ ����
        if (speechBubbleText != null)
        {
            speechBubbleText.text = "";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // ĳ����(Player �±�)�� �����ϸ� �޽��� ǥ��
        if (other.CompareTag("Player") && speechBubbleText != null)
        {
            speechBubbleText.text = message;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // ĳ���Ͱ� ������ �ؽ�Ʈ ����
        if (other.CompareTag("Player") && speechBubbleText != null)
        {
            speechBubbleText.text = "";
        }
    }
}
