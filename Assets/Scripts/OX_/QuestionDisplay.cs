using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class QuestionDisplay : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI questionText;   // 질문을 표시할 TextMeshPro
    public TextMeshProUGUI feedbackText;   // 피드백을 표시할 TextMeshPro
    public TextMeshProUGUI countdownText;  // 카운트다운을 표시할 TextMeshPro

    [Header("Character")]
    public GameObject character; // 캐릭터 GameObject

    [Header("Settings")]
    public string questionsFileName = "question"; // 질문 파일 경로
    public float questionDuration = 4.5f; // 질문 표시 시간
    public float feedbackDuration = 0.5f; // 피드백 표시 시간
    public int maxQuestions = 3; // 최대 질문 수

    private List<string> questions = new List<string>(); // 모든 질문 리스트
    private List<string> answers = new List<string>();   // 정답 리스트 (O/X)
    private Queue<string> recentQuestions = new Queue<string>(); // 최근 표시된 질문 (중복 방지)
    private string currentAnswer; // 현재 질문의 정답
    private int questionCount = 0; // 진행된 질문 수
    public int correct_count = 0;  // 맞춘 질문 갯수

    void Start()
    {
        // GameManager에서 코인을 불러오기
        // GameManager가 null이 아닐 때만 coinCount를 반영
        if (GameManager.Instance != null)
        {
            // GameManager의 coinCount를 사용 (이 값을 필요에 따라 게임 시작 시 표시 가능)
            Debug.Log($"현재 코인: {GameManager.Instance.coinCount}개");
        }
        else
        {
            Debug.LogWarning("GameManager 인스턴스가 없습니다! GameManager를 씬에 배치하세요.");
        }

        StartCoroutine(PrepareGame());
    }

    // 게임 준비 단계 (카운트다운)
    IEnumerator PrepareGame()
    {
        for (int i = 5; i > 0; i--)
        {
            countdownText.text = i.ToString(); // 카운트다운 숫자 표시
            yield return new WaitForSeconds(1f); // 1초 대기
        }

        countdownText.text = ""; // 카운트다운 완료 후 텍스트 지우기
        LoadQuestions(); // 질문 파일 로드

        if (questions.Count > 0)
        {
            StartCoroutine(DisplayQuestions()); // 질문 출력 시작
        }
        else
        {
            questionText.text = "질문 파일을 찾을 수 없습니다.";
            Debug.LogError("No questions found in the file!");
        }
    }

    // 질문 파일에서 질문과 정답 로드
    void LoadQuestions()
    {
        //string filePath = Path.Combine(Application.streamingAssetsPath, questionsFilePath);
        
        // Resources.Load를 사용하여 question.txt 파일을 TextAsset으로 로드
        TextAsset questionFile = Resources.Load<TextAsset>(questionsFileName);

        if (questionFile != null)
        {
            // 텍스트를 줄 단위로 분리 (다양한 줄바꿈 문자 처리)
            string[] loadedLines = questionFile.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in loadedLines)
            {
                int answerStartIndex = line.LastIndexOf('(');
                if (answerStartIndex != -1 && line.TrimEnd().EndsWith(")"))
                {
                    string cleanQuestion = line.Substring(0, answerStartIndex).Trim();
                    string answer = line.Substring(answerStartIndex + 1, 1).ToUpper(); // "O" 또는 "X"

                    if (answer == "O" || answer == "X")
                    {
                        questions.Add(cleanQuestion);
                        answers.Add(answer);
                    }
                    else
                    {
                        Debug.LogWarning($"잘못된 정답 형식의 줄: {line}");
                    }
                }
                else
                {
                    Debug.LogWarning($"잘못된 질문 형식의 줄: {line}");
                }
            }

            Debug.Log($"Loaded {questions.Count} questions from Resources/{questionsFileName}.txt");
        }
        else
        {
            Debug.LogError($"Resources 폴더에서 질문 파일을 찾을 수 없습니다: {questionsFileName}.txt");
        }

        //if (File.Exists(filePath))
        //{
        //    string[] loadedLines = File.ReadAllLines(filePath);

        //    foreach (string line in loadedLines)
        //    {
        //        int answerStartIndex = line.LastIndexOf('(');
        //        if (answerStartIndex != -1 && line.EndsWith(")"))
        //        {
        //            string cleanQuestion = line.Substring(0, answerStartIndex).Trim();
        //            string answer = line.Substring(answerStartIndex + 1, 1).ToUpper(); // "O" 또는 "X"

        //            if (answer == "O" || answer == "X")
        //            {
        //                questions.Add(cleanQuestion);
        //                answers.Add(answer);
        //            }
        //            else
        //            {
        //                Debug.LogWarning($"Invalid answer format in line: {line}");
        //            }
        //        }
        //        else
        //        {
        //            Debug.LogWarning($"Invalid question format in line: {line}");
        //        }
        //    }
        //}
        //else
        //{
        //    Debug.LogError($"Questions file not found at: {filePath}");
        //}
    }

    // 질문 랜덤 표시 Coroutine
    IEnumerator DisplayQuestions()
    {
        while (questionCount < maxQuestions)
        {
            string nextQuestion = GetRandomQuestion();
            if (nextQuestion == "No questions available!")
            {
                questionText.text = nextQuestion;
                yield break; // 질문이 더 이상 없으므로 루프 종료
            }

            questionText.text = nextQuestion;

            // 질문 표시 시간 동안 대기
            yield return new WaitForSeconds(questionDuration);

            // 정답 체크
            CheckPlayerAnswer();

            // 피드백 표시 시간 동안 대기
            yield return new WaitForSeconds(feedbackDuration);

            // 피드백 제거
            feedbackText.text = "";

            questionCount++;
        }

        yield return new WaitForSeconds(2f); // 2초 대기

        // 모든 질문 완료 후 메시지 표시
        questionText.text = $"문제 갯수 : {maxQuestions}개 \n 맞춘 갯수 : {correct_count}개";

        // 정답 개수가 2개 이상일 때 1 코인 추가
        int amount = correct_count >= 2 ? 1 : 0;

        // GameManager를 통해 코인 추가
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddCoins(amount);
            Debug.Log($"코인 지급 완료. 현재 코인: {GameManager.Instance.coinCount}개");
        }
        else
        {
            Debug.LogWarning("GameManager 인스턴스가 없어 코인 추가 불가!");
        }
    }

    // 랜덤 질문 가져오기 (최근 질문 10개와 중복 방지)
    string GetRandomQuestion()
    {
        string selectedQuestion = null;
        List<string> availableQuestions = new List<string>(questions);

        // 최근 질문 10개 제거
        foreach (var recentQuestion in recentQuestions)
        {
            availableQuestions.Remove(recentQuestion);
        }

        if (availableQuestions.Count > 0)
        {
            int randomIndex = Random.Range(0, availableQuestions.Count);
            selectedQuestion = availableQuestions[randomIndex];

            // 현재 질문과 정답 설정
            int questionIndex = questions.IndexOf(selectedQuestion);
            currentAnswer = answers[questionIndex];

            // 최근 질문 큐 관리
            recentQuestions.Enqueue(selectedQuestion);
            if (recentQuestions.Count > 10)
            {
                recentQuestions.Dequeue();
            }
        }

        return selectedQuestion ?? "No questions available!";
    }

    // 플레이어의 답변을 체크하고 피드백 표시
    int CheckPlayerAnswer()
    {
        float characterX = character.transform.position.x;
        string playerAnswer = "";

        if (characterX >= 1f)
        {
            playerAnswer = "X";
        }
        else if (characterX < -0.1f)
        {
            playerAnswer = "O";
        }
        else
        {
            playerAnswer = "NoAnswer"; // 답변이 없거나 범위 외
        }

        if (playerAnswer != "NoAnswer")
        {
            if (playerAnswer == currentAnswer)
            {
                StartCoroutine(ShowFeedback("O"));
                correct_count++;
            }
            else
            {
                StartCoroutine(ShowFeedback("X"));
            }
        }
        else
        {
            StartCoroutine(ShowFeedback("No Answer"));
        }

        return correct_count;
    }

    // 피드백 표시 Coroutine
    IEnumerator ShowFeedback(string feedback)
    {
        if (feedback == "O")
        {
            feedbackText.color = Color.green;
        }
        else if (feedback == "X")
        {
            feedbackText.color = Color.red;
        }
        else
        {
            feedbackText.color = Color.yellow;
        }

        feedbackText.text = feedback; // O, X 또는 No Answer 표시
        yield return new WaitForSeconds(feedbackDuration); // 피드백 표시 시간 대기
        feedbackText.text = ""; // 피드백 텍스트 초기화
    }
}
