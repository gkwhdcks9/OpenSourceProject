using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FindId : MonoBehaviour
{
    [Header("���̵�ã�� �Է� �ʵ�")]
    public InputField emailInput;
    public InputField certificationNumber;

    [Header("UI �г�")]
    public GameObject emailSentPanel; // ������ȣ �߼� �˸� �г�
    public GameObject idResultPanel; // ���̵� ��� �˸� �г�
    public Text idResultText; // ���̵� ��� �ؽ�Ʈ

    /// <summary>
    /// �̸��� ���� ��û
    /// </summary>
    public void RequestEmailCertification()
    {
        string url = "http://wonokim.iptime.org:4000/api/v1/auth/emailCertification";
        string jsonBody = $"{{\"email\":\"{emailInput.text}\",\"isFindId\":\"true\"}}";

        StartCoroutine(SendPostRequest(url, jsonBody, "POST"));

        // �г� ǥ�� ����
        StartCoroutine(ShowEmailSentPanel());
    }

    /// <summary>
    /// ���̵� ã�� ��û
    /// </summary>
    public void FindUserId()
    {
        string url = "http://wonokim.iptime.org:4000/api/v1/auth/findId";
        string jsonBody = $"{{\"email\":\"{emailInput.text}\",\"code\":\"{certificationNumber.text}\"}}";

        StartCoroutine(SendFindIdRequest(url, jsonBody, "POST"));
    }

    /// <summary>
    /// ���̵� ã�� API ��û
    /// </summary>
    private IEnumerator SendFindIdRequest(string url, string jsonBody, string method)
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
                Debug.Log("���̵� ã�� ����: " + www.downloadHandler.text);

                // �����κ��� ���� ���̵� ó��
                string userId = www.downloadHandler.text.Replace("\"", ""); // JSON���� �ֵ���ǥ ����

                // ��� �г� ǥ��
                idResultText.text = $"���̵�: {userId}";
                idResultPanel.SetActive(true);
            }
            else
            {
                Debug.LogError("���̵� ã�� ����: " + www.error);
                idResultText.text = "���̵� ã�� ����! �Է� ������ Ȯ�����ּ���.";
                idResultPanel.SetActive(true);
            }
            www.uploadHandler.Dispose();
        }
    }

    /// <summary>
    /// API ��û ���� �޼���
    /// </summary>
    private IEnumerator SendPostRequest(string url, string jsonBody, string method)
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
            }
            else
            {
                Debug.LogError("API ��û ����: " + www.error);
            }

            www.uploadHandler.Dispose();
        }
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
