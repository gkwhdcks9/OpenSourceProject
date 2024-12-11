using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 요청 데이터 클래스
[System.Serializable]
public class ChatRequest
{
    public string model;
    public List<Message> messages;
    public int max_tokens;
    public float temperature;

    [System.Serializable]
    public class Message
    {
        public string role;
        public string content;
    }
}

// 응답 데이터 클래스
[System.Serializable]
public class ChatResponse
{
    public Choice[] choices;

    [System.Serializable]
    public class Choice
    {
        public Message message;
    }

    [System.Serializable]
    public class Message
    {
        public string content;
    }
}

public class ChatSummarizer : MonoBehaviour
{
    // PostAPI 스크립트 참조
    [SerializeField]
    private PostAPI postAPI;

    // UI 요소
    public GameObject confirmationPanel; // 패널
    public Button yesButton; // Yes 버튼
    public Button noButton; // No 버튼

    // OpenAI API 키 설정 (보안에 유의하세요)
    [SerializeField]
    private string apiKey = "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"; // 실제 API 키로 교체하세요.

    // 대화 기록 파일 경로
    private string logFilePath;

    // 요약 결과 파일 경로
    private string summaryFilePath;

    // Start 함수에서 자동으로 요약을 시작하지 않도록 수정
    private void Start()
    {
        // 로그 파일 경로 설정 (Application.persistentDataPath 사용)
        logFilePath = Path.Combine(Application.persistentDataPath, "conversation_log.txt");
        summaryFilePath = Path.Combine(Application.persistentDataPath, "Summary.txt");

        Debug.Log($"Log file path: {logFilePath}");
        Debug.Log($"Summary file path: {summaryFilePath}");
    }

    public void YesBtnClicked()
    {
        // SummarizeChat 완료 후 Summary 파일 읽기
        string summaryContent = ReadSummaryFile();

        // Summary 파일 내용이 비어 있지 않으면 ParseAndCreatePost 실행
        if (!string.IsNullOrEmpty(summaryContent))
        {
            ParseAndCreatePost(summaryContent);
        }
        else
        {
            Debug.LogError("Summary file is empty or not found. Post creation skipped.");
        }

        // 패널 비활성화
        confirmationPanel.SetActive(false);
    }

    /// <summary>
    /// 종료 버튼 클릭 시 호출되는 공개 메서드.
    /// </summary>
    public void OnExitButtonClicked()
    {
        StartCoroutine(SummarizeAndExit());
    }

    /// <summary>
    /// 요약을 수행하고 애플리케이션을 종료하는 코루틴.
    /// </summary>
    public IEnumerator SummarizeAndExit()
    {
        yield return StartCoroutine(SummarizeChat());

        // 요약 완료 후 패널 활성화 및 Yes 버튼 활성화
        confirmationPanel.SetActive(true);
        yesButton.interactable = true; // Yes 버튼 활성화

        // No 버튼 클릭 이벤트
        noButton.onClick.RemoveAllListeners();
        noButton.onClick.AddListener(() =>
        {
            confirmationPanel.SetActive(false); // 패널 비활성화
        });

        // Yes 버튼 클릭 이벤트
        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(() =>
        {
            YesBtnClicked();
        });

        //        Application.Quit();

        //        // 에디터에서 실행 중일 때는 에디터를 종료
        //#if UNITY_EDITOR
        //        UnityEditor.EditorApplication.isPlaying = false;
        //#endif
    }

    /// <summary>
    /// 대화 기록 읽기
    /// </summary>
    /// <returns>대화 기록 내용</returns>
    private string ReadChatHistory()
    {
        try
        {
            if (File.Exists(logFilePath))
            {
                string content = File.ReadAllText(logFilePath);
                Debug.Log("Chat history read successfully.");
                return content;
            }
            else
            {
                Debug.LogError($"Error: Log file not found at {logFilePath}");
                return null;
            }
        }
        catch (IOException e)
        {
            Debug.LogError($"Error reading log file: {e.Message}");
            return null;
        }
    }

    /// <summary>
    /// OpenAI API 호출 및 요약 요청
    /// </summary>
    /// <returns>IEnumerator</returns>
    private IEnumerator SummarizeChat()
    {
        string chatHistory = ReadChatHistory();
        if (string.IsNullOrEmpty(chatHistory))
        {
            yield break;
        }

        // ChatRequest 객체 생성
        ChatRequest chatRequest = new ChatRequest
        {
            model = "gpt-4o",
            messages = new List<ChatRequest.Message>
            {
                new ChatRequest.Message { role = "system", content = "아래는 사용자의 채팅 기록입니다. 당신은 이 채팅 기록을 형식에 맞춰서 요약해주세요. 이 기록은 게시판의 게시물로 저장될 예정이기에, 문어체로 작성해주세요. 형식은 [제목] \n [카테고리] \n [요약내용]의 형식입니다. !카테고리에서는 Semu, Geunro, Sobija, Gajeong 중 하나를 선택해서 작성해주세요. 대화 기록을 보고 카테고리를 추측하시면 됩니다.요약 내용에는 문제와 문제 해결법을 위주로 작성해주세요!" },
                new ChatRequest.Message { role = "user", content = $"채팅 기록:\n{chatHistory}" }
            },
            max_tokens = 500,
            temperature = 0.7f
        };

        // JSON 직렬화
        string requestBody = JsonUtility.ToJson(chatRequest);
        Debug.Log($"Request Body: {requestBody}");

        using (UnityWebRequest request = new UnityWebRequest("https://api.openai.com/v1/chat/completions", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(requestBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

            // API 호출
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Summary Response: " + request.downloadHandler.text);
                ProcessResponse(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error in API call: " + request.error);
                Debug.LogError("Response: " + request.downloadHandler.text);
            }
        }
    }

    /// <summary>
    /// API 응답 처리 및 파일 저장
    /// </summary>
    /// <param name="jsonResponse">API 응답 JSON</param>
    private void ProcessResponse(string jsonResponse)
    {
        try
        {
            // JSON에서 요약된 결과를 추출
            ChatResponse response = JsonUtility.FromJson<ChatResponse>(jsonResponse);
            if (response != null && response.choices != null && response.choices.Length > 0)
            {
                string summary = response.choices[0].message.content.Trim();
                Debug.Log("Summary: " + summary);

                // 요약 결과 파일로 저장
                WriteSummaryToFile(summary);
            }
            else
            {
                Debug.LogError("Invalid response structure.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error parsing response: {e.Message}");
        }
    }

    /// <summary>
    /// 요약 결과를 파일에 저장
    /// </summary>
    /// <param name="summary">요약된 내용</param>
    private void WriteSummaryToFile(string summary)
    {
        try
        {
            File.WriteAllText(summaryFilePath, summary);
            Debug.Log($"Summary saved to {summaryFilePath}");
        }
        catch (IOException e)
        {
            Debug.LogError($"Error writing summary file: {e.Message}");
        }
    }

    /// <summary>
    /// Summary.txt 파일 읽기
    /// </summary>
    /// <returns>파일 내용</returns>
    private string ReadSummaryFile()
    {
        try
        {
            if (File.Exists(summaryFilePath))
            {
                string content = File.ReadAllText(summaryFilePath);
                Debug.Log("Summary file read successfully.");
                return content;
            }
            else
            {
                Debug.LogError($"Summary file not found at {summaryFilePath}");
                return null;
            }
        }
        catch (IOException e)
        {
            Debug.LogError($"Error reading summary file: {e.Message}");
            return null;
        }
    }

    /// <summary>
    /// Summary 내용을 파싱하고 API 호출을 통해 게시물 생성
    /// </summary>
    /// <param name="summaryContent">Summary 내용</param>
    private void ParseAndCreatePost(string summaryContent)
    {
        try
        {
            Debug.Log($"Summary Content: {summaryContent}");

            // Summary 내용에서 제목, 카테고리, 요약내용 추출
            string title = ExtractContent(summaryContent, "[제목]", "[카테고리]");
            string categoryName = ExtractContent(summaryContent, "[카테고리]", "[요약내용]");
            string contents = ExtractContent(summaryContent, "[요약내용]", null);

            Debug.Log(title);
            Debug.Log(categoryName);
            Debug.Log(contents);

            // 제목 예외 처리: 비어 있는 경우 contents의 첫 5글자를 사용
            if (string.IsNullOrEmpty(title))
            {
                if (!string.IsNullOrEmpty(contents))
                {
                    // contents의 첫 5글자를 title로 설정
                    title = contents.Length >= 5
                        ? contents.Substring(0, 5)
                        : contents; // contents가 5글자 미만인 경우 전체 내용을 제목으로 사용
                    Debug.LogWarning($"Title was empty. Set to: {title}");
                }
                else
                {
                    Debug.LogError("Contents is empty. Unable to set title.");
                    return; // contents도 비어 있는 경우 게시물 생성을 중단
                }
            }

            // 카테고리 예외 처리: 잘못된 카테고리인 경우 기본값으로 'Gajeong' 설정
            HashSet<string> validCategories = new HashSet<string> { "Gajeong", "Geunro", "Semu", "Sobija" };
            if (string.IsNullOrEmpty(categoryName) || !validCategories.Contains(categoryName))
            {
                categoryName = "Gajeong";
                Debug.LogWarning($"Invalid category. Set to default: {categoryName}");
            }

            // 추출된 데이터 출력
            Debug.Log($"Title: {title}, Category: {categoryName}, Contents: {contents}");

            if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(categoryName) && !string.IsNullOrEmpty(contents))
            {
                // 사용자 닉네임 예제 (수정 가능)
                //string nickname = "wonho";
                string nickname = UserData.Instance.NickName;

                // API 호출
                StartCoroutine(postAPI.CreatePost(
                    title.Trim(),
                    contents.Trim(),
                    nickname,
                    categoryName.Trim(),
                    onSuccess: (response) =>
                    {
                        Debug.Log($"Post created successfully: {response}");
                    },
                    onError: (error) =>
                    {
                        Debug.LogError($"Error creating post: {error}");
                    }
                ));
            }
            else
            {
                Debug.LogError("Summary content format is invalid or missing required fields.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error parsing summary content: {e.Message}");
        }
    }

    /// <summary>
    /// 특정 태그 사이의 내용을 추출
    /// </summary>
    /// <param name="text">전체 텍스트</param>
    /// <param name="startTag">시작 태그</param>
    /// <param name="endTag">종료 태그 (null일 경우 끝까지 추출)</param>
    /// <returns>추출된 내용</returns>
    private string ExtractContent(string text, string startTag, string endTag)
    {
        int startIndex = text.IndexOf(startTag) + startTag.Length;
        if (startIndex < startTag.Length) return null;

        int endIndex = endTag != null ? text.IndexOf(endTag, startIndex) : text.Length;
        if (endIndex < 0) return null;

        return text.Substring(startIndex, endIndex - startIndex).Trim();
    }
}
