using UnityEngine;
using TMPro;
public class NPC2 : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speechBubbleText; // UI 텍스트
    [SerializeField] private string message = ""; // 표시할 메시지
    [SerializeField] public Move.CanvasType canvasType = Move.CanvasType.None; // 캔버스 종류 (Archive 또는 Board)
    [SerializeField] public int canvasIndex; // 활성화할 캔버스의 인덱스 (각 그룹 내에서 0~3)

    private void Start()
    {
        // 시작 시 텍스트 비우기
        if (speechBubbleText != null)
        {
            speechBubbleText.text = "";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 캐릭터(Player 태그)를 감지하면 메시지 표시
        if (other.CompareTag("Player") && speechBubbleText != null)
        {
            speechBubbleText.text = message;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 캐릭터가 나가면 텍스트 비우기
        if (other.CompareTag("Player") && speechBubbleText != null)
        {
            speechBubbleText.text = "";
        }
    }
}
