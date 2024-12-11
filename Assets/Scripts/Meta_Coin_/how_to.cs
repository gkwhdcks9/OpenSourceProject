using UnityEngine;
using TMPro;

public class how_to : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speechBubbleText; // ��ǳ�� �ؽ�Ʈ (UI ����)
    [SerializeField] private string topMessage = "���� O/X ����!"; // ���� �޽���
    [SerializeField] private string leftMessage = "������ �Խ���, ��Ͻ�!"; // ���� �޽���
    [SerializeField] private string rightMessage = "�������� ������ ���!"; // ������ �޽���

    private bool isPlayerInside = false; // �÷��̾ �ݶ��̴� �ȿ� �ִ��� ����

    private void Start()
    {
        if (speechBubbleText != null)
        {
            speechBubbleText.text = ""; // �ʱ� ���¿��� ��ǳ�� �ؽ�Ʈ ����
        }
        else
        {
            Debug.LogError("SpeechBubbleText�� ������� �ʾҽ��ϴ�. Inspector���� �������ּ���.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true; // �÷��̾ �ݶ��̴� �ȿ� ����
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isPlayerInside && other.CompareTag("Player"))
        {
            if (speechBubbleText == null)
            {
                Debug.LogError("SpeechBubbleText�� null�Դϴ�. Inspector���� ����Ǿ����� Ȯ���ϼ���.");
                return;
            }

            // �÷��̾� ��ġ ��������
            Vector3 playerPosition = other.transform.position;

            // ������(�ݶ��̴��� Transform ��ġ)���κ��� ��� ��ġ ���
            Vector3 direction = playerPosition - transform.position;

            // ǥ���� �޽��� �ʱ�ȭ
            string combinedMessage = "";

            // ���� �޽��� �߰�
            if (direction.y > Mathf.Abs(direction.x))
            {
                combinedMessage += topMessage;
            }

            // ���� �޽��� �߰�
            if (direction.x < 0)
            {
                if (!string.IsNullOrEmpty(combinedMessage)) combinedMessage += "\n"; // �ٹٲ� �߰�
                combinedMessage += leftMessage;
            }

            // ������ �޽��� �߰�
            if (direction.x > 0)
            {
                if (!string.IsNullOrEmpty(combinedMessage)) combinedMessage += "\n"; // �ٹٲ� �߰�
                combinedMessage += rightMessage;
            }

            // ��ǳ�� �ؽ�Ʈ ����
            speechBubbleText.text = combinedMessage;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false; // �÷��̾ �ݶ��̴� ������ ����
            if (speechBubbleText != null)
            {
                speechBubbleText.text = ""; // �ؽ�Ʈ ����
            }
        }
    }
}
