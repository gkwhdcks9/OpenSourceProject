using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ButtonController : MonoBehaviour
{
    public Button interactButton; // ��ȣ�ۿ� ��ư
    public TextMeshProUGUI buttonText; // TMP �ؽ�Ʈ ������Ʈ
    public float activeAlpha = 1.0f; // ��ư�� Ȱ��ȭ�� ���� ���� ��
    public float inactiveAlpha = 0.3f; // ��ư�� ��Ȱ��ȭ ������ ���� ���� ��
    private Image buttonImage;

    private string sceneToLoad; // �ε��� �� �̸��� ������ ����

    void Start()
    {
        buttonImage = interactButton.GetComponent<Image>();
        SetButtonActive(false); // ó������ ��ư�� ��Ȱ��ȭ ���·� ����

        interactButton.onClick.AddListener(OnButtonClicked); // ��ư Ŭ�� �̺�Ʈ �߰�
    }

    public void SetButtonActive(bool isActive, string message = "", string sceneName = "")
    {
        // ��ư�� ��ȣ�ۿ� ���� ���� ����
        interactButton.interactable = isActive;

        // ���� �� ����
        Color color = buttonImage.color;
        color.a = isActive ? activeAlpha : inactiveAlpha;
        buttonImage.color = color;

        // �ؽ�Ʈ ���� �� ���� ����
        Color textColor = buttonText.color;
        textColor.a = isActive ? activeAlpha : inactiveAlpha;
        buttonText.color = textColor;

        // �ؽ�Ʈ ����: Ȱ��ȭ ������ ���� ��Ż �̸� ǥ��, ��Ȱ��ȭ ������ ���� "Interaction" ǥ��
        buttonText.text = isActive ? message : "Interaction";

        // �� �̸� ���� (Ȱ��ȭ ������ ���� ����)
        if (isActive)
        {
            sceneToLoad = sceneName;
            Debug.Log($"Button activated with message: '{message}', target scene: '{sceneName}'");
        }
        else
        {
            Debug.Log("Button deactivated");
        }
    }

    private void OnButtonClicked()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.Log($"Button clicked, loading scene: {sceneToLoad}");
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("No scene to load, sceneToLoad is empty");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            string portalName = other.gameObject.name;
            string message = "";
            string targetScene = "";

            // ��Ż �̸��� ���� �޽����� �� �̸� ����
            if (portalName == "ChatBot_Potal")
            {
                message = "GO ChatBot";
                targetScene = "ChatBotScene";
            }
            else if (portalName == "OX_Potal")
            {
                message = "GO O/X";
                targetScene = "OXScene";
            }
            else if (portalName == "Borad_Potal")
            {
                message = "GO Board";
                targetScene = "BoradScene"; // ö�� ����
            }

            Debug.Log($"Entered portal: {portalName}, setting button with message '{message}' and target scene '{targetScene}'");
            SetButtonActive(true, message, targetScene);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            Debug.Log($"Exited portal: {other.gameObject.name}, deactivating button");
            SetButtonActive(false);
        }
    }
}
