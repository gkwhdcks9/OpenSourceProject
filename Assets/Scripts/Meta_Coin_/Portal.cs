using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // SceneManager 네임스페이스 추가

public class Portal : MonoBehaviour
{
    public GameObject commonButton; // 공용 버튼 오브젝트
    public TextMeshProUGUI buttonText; // Inspector에서 직접 할당할 수 있도록 public으로 설정
    private string portalName; // 현재 포탈의 이름을 저장할 변수

    private void Awake()
    {
        commonButton.SetActive(false); // 처음에 버튼 비활성화
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 현재 오브젝트의 이름을 포탈 이름으로 사용
            portalName = transform.name; // portalName 변수에 현재 포탈 이름 저장
            buttonText.text = $"Go to {portalName} Scene"; // 버튼 텍스트 설정
            commonButton.SetActive(true); // 버튼 활성화

            // 버튼의 OnClick 이벤트에 동적으로 EnterPortal 메서드 연결
            commonButton.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners(); // 기존 리스너 제거
            commonButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => EnterPortal(portalName)); // 현재 포탈에 맞는 메서드 추가
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            commonButton.SetActive(true); // 버튼 비활성화
        }
    }

    public void EnterPortal(string portalName)
    {
        if (!string.IsNullOrEmpty(portalName))
        {
            string SceneName = portalName + "Scene";
            SceneManager.LoadScene(SceneName);
            Debug.Log("NAME: " + SceneName);
        }
    }
}
