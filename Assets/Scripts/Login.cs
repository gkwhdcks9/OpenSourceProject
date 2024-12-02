using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    [Header("로그인 입력 필드")]
    public InputField loginUserIdInput;
    public InputField loginPasswordInput;

    [Header("UI 패널")]
    public GameObject errorPanel; // 오류 발생 패널
    public Text errorText;

    // 에러 응답을 매핑할 클래스
    [System.Serializable]
    public class ErrorResponse
    {
        public string status;
        public string message;
    }

    // 응답 클래스
    [System.Serializable]
    public class LoginResponse
    {
        public string userId;
        public string nickName; // 서버에서 반환되는 닉네임
    }

    /// <summary>
    /// 로그인 요청
    /// </summary>
    public void LoginUser()
    {
        string url = "http://wonokim.iptime.org:4000/api/v1/auth/signIn";
        string jsonBody = $"{{\"userId\":\"{loginUserIdInput.text}\",\"password\":\"{loginPasswordInput.text}\"}}";

        StartCoroutine(SendPostRequest(url, jsonBody, "POST", HandleLoginError));
    }

    // 로그인 에러 처리
    private void HandleLoginError(string errorResponse)
    {
        // 에러 JSON 파싱 (Unity의 JsonUtility 또는 Newtonsoft.Json 사용 가능)
        ErrorResponse error = JsonUtility.FromJson<ErrorResponse>(errorResponse);

        string errorMessage = "";
        if (error.message == "사용자를 찾을 수 없습니다.")
        {
            Debug.LogError("사용자를 찾을 수 없습니다.");
            errorMessage = "사용자를 찾을 수 없습니다";
        }
        else if (error.message == "비밀번호가 일치하지 않습니다.")
        {
            Debug.LogError("비밀번호가 일치하지 않습니다.");
            errorMessage = "비밀번호가 일치하지 않습니다";
        }
        else
        {
            Debug.LogError("알 수 없는 에러: " + error.message);
            errorMessage = "알 수 없는 에러가 발생했습니다.";
        }

        // UI에 에러 메시지 표시
        DisplayErrorMessage(errorMessage);
    }

    /// <summary>
    /// 에러 메시지 표시 및 패널 활성화
    /// </summary>
    private void DisplayErrorMessage(string message)
    {
        errorText.text = message;      // 텍스트 업데이트
        StartCoroutine(ShowErrorPanel()); // 패널 활성화
    }

    private IEnumerator ShowErrorPanel()
    {
        errorPanel.SetActive(true); // 패널 활성화
        yield return new WaitForSeconds(2f); // 2초 동안 대기
        errorPanel.SetActive(false); // 패널 비활성화
    }

    /// <summary>
    /// API 요청 공통 메서드
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
                Debug.Log("API 요청 성공: " + www.downloadHandler.text);

                // 응답 파싱
                LoginResponse response = JsonUtility.FromJson<LoginResponse>(www.downloadHandler.text);

                // UserData에 닉네임 저장
                UserData.Instance.SetNickName(response.nickName);

                // 홈 화면으로 이동
                SceneManager.LoadScene("HomeScene");
            }
            else
            {
                // HTTP 상태 코드 확인
                if (www.responseCode == 400) // Bad Request
                {
                    string errorResponse = www.downloadHandler.text;
                    Debug.LogError($"요청 실패 (400): {errorResponse}");

                    // 서버에서 반환된 에러 메시지 처리
                    if (onError != null)
                        onError.Invoke(errorResponse);
                }
                else
                {
                    Debug.LogError($"요청 실패: {www.error}");
                }
            }

            www.uploadHandler.Dispose();
        }
    }
}
