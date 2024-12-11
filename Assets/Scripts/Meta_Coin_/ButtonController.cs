using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ButtonController : MonoBehaviour
{
    public Button interactButton; // 상호작용 버튼
    public TextMeshProUGUI buttonText; // TMP 텍스트 컴포넌트
    public float activeAlpha = 1.0f; // 버튼이 활성화될 때의 알파 값
    public float inactiveAlpha = 0.3f; // 버튼이 비활성화 상태일 때의 알파 값
    private Image buttonImage;

    private string sceneToLoad; // 로드할 씬 이름을 저장할 변수

    void Start()
    {
        buttonImage = interactButton.GetComponent<Image>();
        SetButtonActive(false); // 처음에는 버튼을 비활성화 상태로 설정

        interactButton.onClick.AddListener(OnButtonClicked); // 버튼 클릭 이벤트 추가
    }

    public void SetButtonActive(bool isActive, string message = "", string sceneName = "")
    {
        // 버튼의 상호작용 가능 상태 설정
        interactButton.interactable = isActive;

        // 알파 값 조정
        Color color = buttonImage.color;
        color.a = isActive ? activeAlpha : inactiveAlpha;
        buttonImage.color = color;

        // 텍스트 색상 및 내용 조정
        Color textColor = buttonText.color;
        textColor.a = isActive ? activeAlpha : inactiveAlpha;
        buttonText.color = textColor;

        // 텍스트 설정: 활성화 상태일 때는 포탈 이름 표시, 비활성화 상태일 때는 "Interaction" 표시
        buttonText.text = isActive ? message : "Interaction";

        // 씬 이름 저장 (활성화 상태일 때만 설정)
        if (isActive)
        {
            sceneToLoad = sceneName;
            Debug.Log($"Button activated with message: '{message}', target scene: '{sceneName}'");
        }
        else
        {
            Debug.Log("Button deactivated");
        }
    }

    private void OnButtonClicked()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.Log($"Button clicked, loading scene: {sceneToLoad}");
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("No scene to load, sceneToLoad is empty");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            string portalName = other.gameObject.name;
            string message = "";
            string targetScene = "";

            // 포탈 이름에 따른 메시지와 씬 이름 설정
            if (portalName == "ChatBot_Potal")
            {
                message = "GO ChatBot";
                targetScene = "ChatBotScene";
            }
            else if (portalName == "OX_Potal")
            {
                message = "GO O/X";
                targetScene = "OXScene";
            }
            else if (portalName == "Borad_Potal")
            {
                message = "GO Board";
                targetScene = "BoradScene"; // 철자 주의
            }

            Debug.Log($"Entered portal: {portalName}, setting button with message '{message}' and target scene '{targetScene}'");
            SetButtonActive(true, message, targetScene);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            Debug.Log($"Exited portal: {other.gameObject.name}, deactivating button");
            SetButtonActive(false);
        }
    }
}
