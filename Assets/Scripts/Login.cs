using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    [Header("�α��� �Է� �ʵ�")]
    public InputField loginUserIdInput;
    public InputField loginPasswordInput;

    [Header("UI �г�")]
    public GameObject errorPanel; // ���� �߻� �г�
    public Text errorText;

    // ���� ������ ������ Ŭ����
    [System.Serializable]
    public class ErrorResponse
    {
        public string status;
        public string message;
    }

    // ���� Ŭ����
    [System.Serializable]
    public class LoginResponse
    {
        public string userId;
        public string nickName; // �������� ��ȯ�Ǵ� �г���
    }

    /// <summary>
    /// �α��� ��û
    /// </summary>
    public void LoginUser()
    {
        string url = "http://wonokim.iptime.org:4000/api/v1/auth/signIn";
        string jsonBody = $"{{\"userId\":\"{loginUserIdInput.text}\",\"password\":\"{loginPasswordInput.text}\"}}";

        StartCoroutine(SendPostRequest(url, jsonBody, "POST", HandleLoginError));
    }

    // �α��� ���� ó��
    private void HandleLoginError(string errorResponse)
    {
        // ���� JSON �Ľ� (Unity�� JsonUtility �Ǵ� Newtonsoft.Json ��� ����)
        ErrorResponse error = JsonUtility.FromJson<ErrorResponse>(errorResponse);

        string errorMessage = "";
        if (error.message == "����ڸ� ã�� �� �����ϴ�.")
        {
            Debug.LogError("����ڸ� ã�� �� �����ϴ�.");
            errorMessage = "����ڸ� ã�� �� �����ϴ�";
        }
        else if (error.message == "��й�ȣ�� ��ġ���� �ʽ��ϴ�.")
        {
            Debug.LogError("��й�ȣ�� ��ġ���� �ʽ��ϴ�.");
            errorMessage = "��й�ȣ�� ��ġ���� �ʽ��ϴ�";
        }
        else
        {
            Debug.LogError("�� �� ���� ����: " + error.message);
            errorMessage = "�� �� ���� ������ �߻��߽��ϴ�.";
        }

        // UI�� ���� �޽��� ǥ��
        DisplayErrorMessage(errorMessage);
    }

    /// <summary>
    /// ���� �޽��� ǥ�� �� �г� Ȱ��ȭ
    /// </summary>
    private void DisplayErrorMessage(string message)
    {
        errorText.text = message;      // �ؽ�Ʈ ������Ʈ
        StartCoroutine(ShowErrorPanel()); // �г� Ȱ��ȭ
    }

    private IEnumerator ShowErrorPanel()
    {
        errorPanel.SetActive(true); // �г� Ȱ��ȭ
        yield return new WaitForSeconds(2f); // 2�� ���� ���
        errorPanel.SetActive(false); // �г� ��Ȱ��ȭ
    }

    /// <summary>
    /// API ��û ���� �޼���
    /// </summary>
    private IEnumerator SendPostRequest(string url, string jsonBody, string method, System.Action<string> onError = null)
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

                // ���� �Ľ�
                LoginResponse response = JsonUtility.FromJson<LoginResponse>(www.downloadHandler.text);

                // UserData�� �г��� ����
                UserData.Instance.SetNickName(response.nickName);

                // Ȩ ȭ������ �̵�
                SceneManager.LoadScene("HomeScene");
            }
            else
            {
                // HTTP ���� �ڵ� Ȯ��
                if (www.responseCode == 400) // Bad Request
                {
                    string errorResponse = www.downloadHandler.text;
                    Debug.LogError($"��û ���� (400): {errorResponse}");

                    // �������� ��ȯ�� ���� �޽��� ó��
                    if (onError != null)
                        onError.Invoke(errorResponse);
                }
                else
                {
                    Debug.LogError($"��û ����: {www.error}");
                }
            }

            www.uploadHandler.Dispose();
        }
    }
}
