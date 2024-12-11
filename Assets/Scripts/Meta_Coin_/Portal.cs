using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // SceneManager ���ӽ����̽� �߰�

public class Portal : MonoBehaviour
{
    public GameObject commonButton; // ���� ��ư ������Ʈ
    public TextMeshProUGUI buttonText; // Inspector���� ���� �Ҵ��� �� �ֵ��� public���� ����
    private string portalName; // ���� ��Ż�� �̸��� ������ ����

    private void Awake()
    {
        commonButton.SetActive(false); // ó���� ��ư ��Ȱ��ȭ
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ���� ������Ʈ�� �̸��� ��Ż �̸����� ���
            portalName = transform.name; // portalName ������ ���� ��Ż �̸� ����
            buttonText.text = $"Go to {portalName} Scene"; // ��ư �ؽ�Ʈ ����
            commonButton.SetActive(true); // ��ư Ȱ��ȭ

            // ��ư�� OnClick �̺�Ʈ�� �������� EnterPortal �޼��� ����
            commonButton.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners(); // ���� ������ ����
            commonButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => EnterPortal(portalName)); // ���� ��Ż�� �´� �޼��� �߰�
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            commonButton.SetActive(true); // ��ư ��Ȱ��ȭ
        }
    }

    public void EnterPortal(string portalName)
    {
        if (!string.IsNullOrEmpty(portalName))
        {
            string SceneName = portalName + "Scene";
            SceneManager.LoadScene(SceneName);
            Debug.Log("NAME: " + SceneName);
        }
    }
}
