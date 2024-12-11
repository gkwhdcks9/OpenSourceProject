using UnityEngine;
using TMPro;

public class how_to : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speechBubbleText; // 말풍선 텍스트 (UI 연결)
    [SerializeField] private string topMessage = "위는 O/X 게임!"; // 위쪽 메시지
    [SerializeField] private string leftMessage = "왼쪽은 게시판, 기록실!"; // 왼쪽 메시지
    [SerializeField] private string rightMessage = "오른쪽은 법무사 상담!"; // 오른쪽 메시지

    private bool isPlayerInside = false; // 플레이어가 콜라이더 안에 있는지 여부

    private void Start()
    {
        if (speechBubbleText != null)
        {
            speechBubbleText.text = ""; // 초기 상태에서 말풍선 텍스트 비우기
        }
        else
        {
            Debug.LogError("SpeechBubbleText가 연결되지 않았습니다. Inspector에서 연결해주세요.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true; // 플레이어가 콜라이더 안에 들어옴
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isPlayerInside && other.CompareTag("Player"))
        {
            if (speechBubbleText == null)
            {
                Debug.LogError("SpeechBubbleText가 null입니다. Inspector에서 연결되었는지 확인하세요.");
                return;
            }

            // 플레이어 위치 가져오기
            Vector3 playerPosition = other.transform.position;

            // 기준점(콜라이더의 Transform 위치)으로부터 상대 위치 계산
            Vector3 direction = playerPosition - transform.position;

            // 표시할 메시지 초기화
            string combinedMessage = "";

            // 위쪽 메시지 추가
            if (direction.y > Mathf.Abs(direction.x))
            {
                combinedMessage += topMessage;
            }

            // 왼쪽 메시지 추가
            if (direction.x < 0)
            {
                if (!string.IsNullOrEmpty(combinedMessage)) combinedMessage += "\n"; // 줄바꿈 추가
                combinedMessage += leftMessage;
            }

            // 오른쪽 메시지 추가
            if (direction.x > 0)
            {
                if (!string.IsNullOrEmpty(combinedMessage)) combinedMessage += "\n"; // 줄바꿈 추가
                combinedMessage += rightMessage;
            }

            // 말풍선 텍스트 갱신
            speechBubbleText.text = combinedMessage;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false; // 플레이어가 콜라이더 밖으로 나감
            if (speechBubbleText != null)
            {
                speechBubbleText.text = ""; // 텍스트 비우기
            }
        }
    }
}
