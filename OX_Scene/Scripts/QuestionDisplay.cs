using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class QuestionDisplay : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI questionText; // 질문을 표시할 TextMeshPro
    public TextMeshProUGUI feedbackText; // 피드백을 표시할 TextMeshPro
    public TextMeshProUGUI countdownText; // 카운트다운을 표시할 TextMeshPro

    [Header("Character")]
    public GameObject character; // 캐릭터 GameObject

    [Header("Settings")]
    public string questionsFilePath = "question.txt"; // 질문 파일 경로
    public float questionDuration = 4.5f; // 질문 표시 시간
    public float feedbackDuration = 0.5f; // 피드백 표시 시간
    public int maxQuestions = 3; // 최대 질문 수

    private List<string> questions = new List<string>(); // 모든 질문 리스트
    private List<string> answers = new List<string>(); // 정답 리스트 (O/X)
    private Queue<string> recentQuestions = new Queue<string>(); // 최근 표시된 질문 (중복 방지)
    private string currentAnswer; // 현재 질문의 정답
    private int questionCount = 0; // 진행된 질문 수

    void Start()
    {
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
        string filePath = Path.Combine(Application.streamingAssetsPath, questionsFilePath);

        if (File.Exists(filePath))
        {
            string[] loadedLines = File.ReadAllLines(filePath);

            foreach (string line in loadedLines)
            {
                int answerStartIndex = line.LastIndexOf('(');
                if (answerStartIndex != -1 && line.EndsWith(")"))
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
                        Debug.LogWarning($"Invalid answer format in line: {line}");
                    }
                }
                else
                {
                    Debug.LogWarning($"Invalid question format in line: {line}");
                }
            }
        }
        else
        {
            Debug.LogError($"Questions file not found at: {filePath}");
        }
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
        yield return new WaitForSeconds(2f); // 1초 대기
        // 모든 질문 완료 후 메시지 표시
        questionText.text = "수고하셨습니다!";
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
    void CheckPlayerAnswer()
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
