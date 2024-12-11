using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro; // TextMeshPro를 사용하는 경우
using System; // 이벤트 사용을 위해 추가
using System.Collections.Generic;
using System.IO; // 파일 입출력을 위해 추가

//경로 자동 저장 : 예시 : C:/Users/pc/AppData/LocalLow/DefaultCompany/QA\conversation_log.txt

public class ServerCommunicator : MonoBehaviour
{
    [Header("Server Settings")]
    [Tooltip("Flask 서버의 기본 URL (예: http://localhost:5000)")]
    public string serverBaseUrl = "http://60.253.9.152:5000";

    [Header("UI Elements")]
    [Tooltip("사용자의 질문을 입력받는 InputField")]
    public TMP_InputField userInputField; // TextMeshPro를 사용하는 경우

    [Tooltip("서버로부터 받은 답변을 표시할 Text")]
    public TMP_Text responseText; // TextMeshPro를 사용하는 경우

    [Tooltip("대화 상태를 표시할 Text (선택 사항)")]
    public TMP_Text conversationText; // TextMeshPro를 사용하는 경우

    // 대화 이력 관리 (role: "user" 또는 "assistant", content: 메시지 내용)
    public List<ConversationMessage> conversationHistory = new List<ConversationMessage>();

    // 로그 파일 경로 설정
    private string logFilePath;

    // 서버와의 통신을 초기화할 때 문서 인덱싱을 수행할지 여부
    [Tooltip("게임 시작 시 /index_documents 엔드포인트를 호출하여 문서를 인덱싱할지 여부")]
    public bool autoIndexDocuments = false;

    // 응답을 처리할 이벤트 정의
    public event Action<string> OnAnswerReceived;

    private void Start()
    {
        // 로그 파일 경로 설정 (플랫폼에 독립적)
        logFilePath = Path.Combine(Application.persistentDataPath, "conversation_log.txt");
        Debug.Log($"Log file path: {logFilePath}");
        InitializeLogFile();

        if (autoIndexDocuments)
        {
            StartCoroutine(IndexDocuments());
        }
    }

    /// <summary>
    /// 로그 파일을 초기화합니다.
    /// </summary>
    private void InitializeLogFile()
    {
        try
        {
            // 기존 내용을 덮어쓰고 초기 메시지를 기록
            File.WriteAllText(logFilePath, "=== 대화 기록 ===\n");
            Debug.Log("Log file initialized.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to initialize log file: {ex.Message}");
        }
    }


    #region Public Methods

    /// <summary>
    /// 사용자의 입력을 서버로 보내고, 응답을 받아 UI에 표시하는 메서드.
    /// </summary>
    public void OnSendButtonClicked()
    {
        string userQuery = userInputField.text.Trim();
        if (!string.IsNullOrEmpty(userQuery))
        {
            // 대화 이력에 사용자 질문 추가
            conversationHistory.Add(new ConversationMessage { role = "user", content = userQuery });
            UpdateConversationUI(userQuery, "user");

            StartCoroutine(SendAskRequest(userQuery, conversationHistory));
            userInputField.text = ""; // 입력 필드 초기화
        }
    }

    /// <summary>
    /// Unity에서 서버로 카테고리와 메시지를 전송하는 메서드.
    /// </summary>
    /// <param name="message">전송할 메시지</param>
    /// <param name="category">카테고리 값</param>
    public void SendData(string message, float category)
    {
        StartCoroutine(SendDataToReceiveData(message, category));
    }

    #endregion

    #region Coroutines

    /// <summary>
    /// /receive_data 엔드포인트로 데이터를 전송하는 코루틴.
    /// </summary>
    public IEnumerator SendDataToReceiveData(string message, float category)
    {
        string url = $"{serverBaseUrl}/receive_data";

        ReceiveDataRequest data = new ReceiveDataRequest
        {
            category = category,
            message = message
        };

        string jsonData = JsonUtility.ToJson(data);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // 요청 전송
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Receive data sent successfully: " + request.downloadHandler.text);
            ReceiveDataResponse response = JsonUtility.FromJson<ReceiveDataResponse>(request.downloadHandler.text);
            // 응답 메시지 처리
            OnAnswerReceived?.Invoke(response.message);
        }
        else
        {
            Debug.LogError("Error sending receive data: " + request.error);
        }
    }

    /// <summary>
    /// /ask 엔드포인트로 사용자의 질문을 보내고 답변을 받는 코루틴.
    /// </summary>
    public IEnumerator SendAskRequest(string query, List<ConversationMessage> conversationHistory)
    {
        string url = $"{serverBaseUrl}/ask";

        LogMessage("User", query); // 로그 기록

        AskRequest askRequest = new AskRequest
        {
            query = query,
            conversation_history = conversationHistory
        };

        string jsonData = JsonUtility.ToJson(askRequest);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // 요청 전송
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Ask request sent successfully: " + request.downloadHandler.text);
            AskResponse response = JsonUtility.FromJson<AskResponse>(request.downloadHandler.text);
            // 응답 메시지 처리
            OnAnswerReceived?.Invoke(response.answer);
            LogMessage("Assistant", response.answer); // 로그 기록

            // 대화 이력에 어시스턴트 답변 추가
            conversationHistory.Add(new ConversationMessage { role = "assistant", content = response.answer });
            UpdateConversationUI(response.answer, "assistant");
        }
        else
        {
            Debug.LogError("Error sending ask request: " + request.error);
            //responseText.text = "오류가 발생했습니다. 나중에 다시 시도해주세요.";
        }
    }

    /// <summary>
    /// /index_documents 엔드포인트를 호출하여 문서를 인덱싱하는 코루틴.
    /// </summary>
    private IEnumerator IndexDocuments()
    {
        string url = $"{serverBaseUrl}/index_documents";

        //UnityWebRequest request = UnityWebRequest.PostWwwForm(url, ""); // POST 요청, 본문은 비어있음
        UnityWebRequest request = UnityWebRequest.Post(url, "");

        // 요청 전송
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Documents indexed successfully: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error indexing documents: " + request.error);
        }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// 대화 이력을 UI에 업데이트하는 메서드.
    /// </summary>
    /// <param name="message">메시지 내용</param>
    /// <param name="role">메시지 역할 ("user" 또는 "assistant")</param>
    private void UpdateConversationUI(string message, string role)
    {
        if (conversationText != null)
        {
            conversationText.text += $"{(role == "user" ? "User" : "Assistant")}: {message}\n\n";
        }
    }

    /// <summary>
    /// 메시지를 로그 파일에 기록하는 메서드.
    /// </summary>
    /// <param name="sender">메시지 보낸 사람 (예: User, Assistant)</param>
    /// <param name="message">메시지 내용</param>
    private void LogMessage(string sender, string message)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string logEntry = $"{timestamp} [{sender}]: {message}";

        try
        {
            // 로그 파일에 추가
            File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
            Debug.Log($"Logged message: {logEntry}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"로그를 저장하는 중 오류가 발생했습니다: {ex.Message}");
        }
    }


    #endregion

    #region Data Classes

    [System.Serializable]
    public class ConversationMessage
    {
        public string role; // "user" 또는 "assistant"
        public string content;
    }

    [System.Serializable]
    public class ReceiveDataRequest
    {
        public float category;
        public string message;
    }

    [System.Serializable]
    public class ReceiveDataResponse
    {
        public float category;
        public string message;
    }

    [System.Serializable]
    public class AskRequest
    {
        public string query;
        public List<ConversationMessage> conversation_history;
    }

    [System.Serializable]
    public class AskResponse
    {
        public string answer;
    }

    #endregion
}
