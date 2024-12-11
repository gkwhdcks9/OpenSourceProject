using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeManager : MonoBehaviour
{
    public Text nickNameText; // 닉네임을 표시할 Text
    public Button loginButton; // 로그인 버튼
    public Button logoutButton; // 로그아웃 버튼
    public Button registerButton; // 회원가입 버튼
    public Button findIdButton; // 아이디 찾기 버튼
    public GameObject welcomePanel; // 닉네임 패널

    // 게임 시작 버튼
    public Button startButton;

    private void Start()
    {
        UpdateUI(); // 초기 UI 상태 설정
    }

    private void UpdateUI()
    {
        if (!string.IsNullOrEmpty(UserData.Instance.NickName))
        {
            // 로그인 상태
            nickNameText.text = $"{UserData.Instance.NickName}님\n환영합니다!";
            welcomePanel.SetActive(true); // 패널 활성화
            loginButton.gameObject.SetActive(false);
            logoutButton.gameObject.SetActive(true);

            // 여기서 버튼을 누르면 승민이가 만든 홈 씬으로 이어짐
            startButton.gameObject.SetActive(true);
        }
        else
        {
            // 로그아웃 상태
            nickNameText.text = "";
            welcomePanel.SetActive(false); // 패널 활성화
            nickNameText.gameObject.SetActive(false);
            loginButton.gameObject.SetActive(true);
            logoutButton.gameObject.SetActive(false);

            // 게임 시작 버튼 비활성화
            startButton.gameObject.SetActive(false);
        }
    }

    public void Logout()
    {
        // Singleton 데이터 초기화
        UserData.Instance.Logout();

        // UI 갱신
        UpdateUI();
    }

    public void GoToLoginScene()
    {
        SceneManager.LoadScene("LoginScene");
    }

    public void GoToRegisterScene()
    {
        SceneManager.LoadScene("RegisterScene");
    }

    public void GoToFindIdScene()
    {
        SceneManager.LoadScene("FindIdScene");
    }

    public void StartButtonClicked()
    {
        SceneManager.LoadScene("MainScene");
    }
}
