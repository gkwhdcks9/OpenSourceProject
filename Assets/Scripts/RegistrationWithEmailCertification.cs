using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RegistrationWithEmailCertification : MonoBehaviour
{
    [Header("ȸ������ �Է� �ʵ�")]
    public InputField userIdInput;
    public InputField passwordInput;
    public InputField emailInput;
    public InputField nickNameInput;
    public InputField certificationNumber;

    [Header("UI �г�")]
    public GameObject emailSentPanel; // ������ȣ �߼� �˸� �г�

    private string savedEmail; // �̸��� ���� ��û�� ���� �̸��� ����

    [System.Serializable]
    public class ErrorResponse
    {
        public string status;
        public string message;
    }

    public GameObject errorPanel; // ���� �޽��� �г�
    public Text errorText; // ���� �޽��� �ؽ�Ʈ

    /// <summary>
    /// �̸��� ���� ��û
    /// </summary>
    public void RequestEmailCertification()
    {
        string url = "http://wonokim.iptime.org:4000/api/v1/auth/emailCertification";
        string jsonBody = $"{{\"email\":\"{emailInput.text}\",\"isFindId\":\"false\"}}";

        savedEmail = emailInput.text; // �Էµ� �̸��� ����
        StartCoroutine(SendPostRequest(url, jsonBody, "POST", () =>
        {
            Debug.Log("�̸��� ���� ��û ����");
        }));

        // �г� ǥ�� ����
        StartCoroutine(ShowEmailSentPanel());
    }

    /// <summary>
    /// ȸ������ ��û
    /// </summary>
    public void RegisterUser()
    {
        bool emailfilled = string.IsNullOrEmpty(savedEmail);
        bool userIdfilled = string.IsNullOrEmpty(userIdInput.text);
        bool passwordfilled = string.IsNullOrEmpty(passwordInput.text);
        bool nicknamefilled = string.IsNullOrEmpty(nickNameInput.text);
        bool codefilled = string.IsNullOrEmpty(certificationNumber.text);

        if (emailfilled || userIdfilled || passwordfilled || nicknamefilled || codefilled)
        {
            Debug.LogError("������ �Ϸ���� �ʾҽ��ϴ�.");
            StartCoroutine(ShowErrorMessage("��� �Է¶��� ä���ּ���."));
            return;
        }

        string url = "http://wonokim.iptime.org:4000/api/v1/auth/signUp";
        string jsonBody = $"{{\"userId\":\"{userIdInput.text}\",\"password\":\"{passwordInput.text}\",\"email\":\"{savedEmail}\",\"nickName\":\"{nickNameInput.text}\",\"certificationNumber\":\"{certificationNumber.text}\"}}";

        StartCoroutine(SendPostRequest(url, jsonBody, "POST", () =>
        {
            SceneManager.LoadScene("HomeScene");
        }));
    }

    /// <summary>
    /// API ��û ���� �޼���
    /// </summary>
    private IEnumerator SendPostRequest(string url, string jsonBody, string method, System.Action onSuccess = null)
    {
        using (UnityWebRequest www = new UnityWebRequest(url, method))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);

            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("API ��û ����: " + www.downloadHandler.text);
                onSuccess?.Invoke(); // ���� �� �ݹ� ����
            }
            else
            {
                if (www.responseCode == 400) // Bad Request
                {
                    string errorResponse = www.downloadHandler.text;
                    HandleErrorResponse(errorResponse);
                }
                else
                {
                    Debug.LogError("API ��û ����: " + www.error);
                }
            }

            www.uploadHandler.Dispose();
        }
    }

    private void HandleErrorResponse(string errorResponse)
    {
        // JSON �Ľ�
        ErrorResponse error = JsonUtility.FromJson<ErrorResponse>(errorResponse);

        Debug.Log(error.message);

        Vector2 customPosition;
        // ���� �ڵ庰 �޽��� ó��
        switch (error.message)
        {
            case "�ߺ��� ���̵��Դϴ�.":
                Debug.LogError("�ߺ��� ���̵��Դϴ�.");
                customPosition = new Vector2(30, 370);
                StartCoroutine(ShowErrorMessage("�ߺ��� ���̵��Դϴ�.", customPosition));
                break;

            case "�ߺ��� �г����Դϴ�.":
                Debug.LogError("�ߺ��� �г����Դϴ�.");
                customPosition = new Vector2(10, -180);
                StartCoroutine(ShowErrorMessage("�ߺ��� �г����Դϴ�.", customPosition));
                break;

            case "�����ڵ尡 ��ġ���� �ʽ��ϴ�.":
                Debug.LogError("������ȣ�� ��ġ���� �ʽ��ϴ�.");
                customPosition = new Vector2(180, -380);
                StartCoroutine(ShowErrorMessage("������ȣ�� ��ġ���� �ʽ��ϴ�.", customPosition));
                break;

            //default:
            //    Debug.LogError("�� �� ���� ���� �߻�");
            //    StartCoroutine(ShowErrorMessage("�� �� ���� ������ �߻��߽��ϴ�."));
            //    break;
        }
    }

    private IEnumerator ShowErrorMessage(string message, Vector2? customPosition = null)
    {
        errorText.text = message; // ���� �޽��� �ؽ�Ʈ ����

        // ����� ���� ��ġ�� ������ �г��� ��ġ ����
        if (customPosition.HasValue)
        {
            RectTransform rectTransform = errorPanel.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = customPosition.Value; // �г� ��ġ ����
        }

        errorPanel.SetActive(true); // �г� Ȱ��ȭ
        yield return new WaitForSeconds(2f);
        errorPanel.SetActive(false);
    }

    /// <summary>
    /// �̸��� �߼� �г��� ���� �ð� ���� ǥ��
    /// </summary>
    private IEnumerator ShowEmailSentPanel()
    {
        emailSentPanel.SetActive(true); // �г� Ȱ��ȭ
        yield return new WaitForSeconds(2f); // 2�� ���� ���
        emailSentPanel.SetActive(false); // �г� ��Ȱ��ȭ
    }
}
