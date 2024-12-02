using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RegistrationWithEmailCertification : MonoBehaviour
{
    [Header("회원가입 입력 필드")]
    public InputField userIdInput;
    public InputField passwordInput;
    public InputField emailInput;
    public InputField nickNameInput;
    public InputField certificationNumber;

    [Header("UI 패널")]
    public GameObject emailSentPanel; // 인증번호 발송 알림 패널

    private string savedEmail; // 이메일 인증 요청에 사용된 이메일 저장

    [System.Serializable]
    public class ErrorResponse
    {
        public string status;
        public string message;
    }

    public GameObject errorPanel; // 에러 메시지 패널
    public Text errorText; // 에러 메시지 텍스트

    /// <summary>
    /// 이메일 인증 요청
    /// </summary>
    public void RequestEmailCertification()
    {
        string url = "http://wonokim.iptime.org:4000/api/v1/auth/emailCertification";
        string jsonBody = $"{{\"email\":\"{emailInput.text}\",\"isFindId\":\"false\"}}";

        savedEmail = emailInput.text; // 입력된 이메일 저장
        StartCoroutine(SendPostRequest(url, jsonBody, "POST", () =>
        {
            Debug.Log("이메일 인증 요청 성공");
        }));

        // 패널 표시 시작
        StartCoroutine(ShowEmailSentPanel());
    }

    /// <summary>
    /// 회원가입 요청
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
            Debug.LogError("인증이 완료되지 않았습니다.");
            StartCoroutine(ShowErrorMessage("모든 입력란을 채워주세요."));
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
    /// API 요청 공통 메서드
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
                Debug.Log("API 요청 성공: " + www.downloadHandler.text);
                onSuccess?.Invoke(); // 성공 시 콜백 실행
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
                    Debug.LogError("API 요청 실패: " + www.error);
                }
            }

            www.uploadHandler.Dispose();
        }
    }

    private void HandleErrorResponse(string errorResponse)
    {
        // JSON 파싱
        ErrorResponse error = JsonUtility.FromJson<ErrorResponse>(errorResponse);

        Debug.Log(error.message);

        Vector2 customPosition;
        // 에러 코드별 메시지 처리
        switch (error.message)
        {
            case "중복된 아이디입니다.":
                Debug.LogError("중복된 아이디입니다.");
                customPosition = new Vector2(30, 370);
                StartCoroutine(ShowErrorMessage("중복된 아이디입니다.", customPosition));
                break;

            case "중복된 닉네임입니다.":
                Debug.LogError("중복된 닉네임입니다.");
                customPosition = new Vector2(10, -180);
                StartCoroutine(ShowErrorMessage("중복된 닉네임입니다.", customPosition));
                break;

            case "인증코드가 일치하지 않습니다.":
                Debug.LogError("인증번호가 일치하지 않습니다.");
                customPosition = new Vector2(180, -380);
                StartCoroutine(ShowErrorMessage("인증번호가 일치하지 않습니다.", customPosition));
                break;

            //default:
            //    Debug.LogError("알 수 없는 에러 발생");
            //    StartCoroutine(ShowErrorMessage("알 수 없는 에러가 발생했습니다."));
            //    break;
        }
    }

    private IEnumerator ShowErrorMessage(string message, Vector2? customPosition = null)
    {
        errorText.text = message; // 에러 메시지 텍스트 설정

        // 사용자 지정 위치가 있으면 패널의 위치 변경
        if (customPosition.HasValue)
        {
            RectTransform rectTransform = errorPanel.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = customPosition.Value; // 패널 위치 설정
        }

        errorPanel.SetActive(true); // 패널 활성화
        yield return new WaitForSeconds(2f);
        errorPanel.SetActive(false);
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
