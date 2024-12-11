using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  // 씬 관리 추가
using UnityEngine.UI;  // UI 관련

public class Back : MonoBehaviour
{
    // 버튼을 인스펙터에서 연결
    public Button backButton;

    // Start is called before the first frame update
    void Start()
    {
        // 버튼 클릭 이벤트에 메서드 연결
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButtonClick);
        }
    }

    // 버튼 클릭 시 호출되는 함수
    void OnBackButtonClick()
    {
        // "Home" 씬을 로드합니다.
        SceneManager.LoadScene("MainScene");
    }
}
