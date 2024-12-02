using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FindId : MonoBehaviour
{
    [Header("아이디찾기 입력 필드")]
    public InputField emailInput;
    public InputField certificationNumber;

    [Header("UI 패널")]
    public GameObject emailSentPanel; // 인증번호 발송 알림 패널
    public GameObject idResultPanel; // 아이디 결과 알림 패널
    public Text idResultText; // 아이디 결과 텍스트

    /// <summary>
    /// 이메일 인증 요청
    /// </summary>
    public void RequestEmailCertification()
    {
        string url = "http://wonokim.iptime.org:4000/api/v1/auth/emailCertification";
        string jsonBody = $"{{\"email\":\"{emailInput.text}\",\"isFindId\":\"true\"}}";

        StartCoroutine(SendPostRequest(url, jsonBody, "POST"));

        // 패널 표시 시작
        StartCoroutine(ShowEmailSentPanel());
    }

    /// <summary>
    /// 아이디 찾기 요청
    /// </summary>
    public void FindUserId()
    {
        string url = "http://wonokim.iptime.org:4000/api/v1/auth/findId";
        string jsonBody = $"{{\"email\":\"{emailInput.text}\",\"code\":\"{certificationNumber.text}\"}}";

        StartCoroutine(SendFindIdRequest(url, jsonBody, "POST"));
    }

    /// <summary>
    /// 아이디 찾기 API 요청
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
                Debug.Log("아이디 찾기 성공: " + www.downloadHandler.text);

                // 서버로부터 받은 아이디 처리
                string userId = www.downloadHandler.text.Replace("\"", ""); // JSON에서 쌍따옴표 제거

                // 결과 패널 표시
                idResultText.text = $"아이디: {userId}";
                idResultPanel.SetActive(true);
            }
            else
            {
                Debug.LogError("아이디 찾기 실패: " + www.error);
                idResultText.text = "아이디 찾기 실패! 입력 정보를 확인해주세요.";
                idResultPanel.SetActive(true);
            }
            www.uploadHandler.Dispose();
        }
    }

    /// <summary>
    /// API 요청 공통 메서드
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
                Debug.Log("API 요청 성공: " + www.downloadHandler.text);
            }
            else
            {
                Debug.LogError("API 요청 실패: " + www.error);
            }

            www.uploadHandler.Dispose();
        }
    }

    /// <summary>
    /// 이메일 발송 패널을 일정 시간 동안 표시
    /// </summary>
    private IEnumerator ShowEmailSentPanel()
    {
        emailSentPanel.SetActive(true); // 패널 활성화
        yield return new WaitForSeconds(2f); // 2초 동안 대기
        emailSentPanel.SetActive(false); // 패널 비활성화
    }
}
