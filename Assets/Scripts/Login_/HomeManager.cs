using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeManager : MonoBehaviour
{
    public Text nickNameText; // �г����� ǥ���� Text
    public Button loginButton; // �α��� ��ư
    public Button logoutButton; // �α׾ƿ� ��ư
    public Button registerButton; // ȸ������ ��ư
    public Button findIdButton; // ���̵� ã�� ��ư
    public GameObject welcomePanel; // �г��� �г�

    // ���� ���� ��ư
    public Button startButton;

    private void Start()
    {
        UpdateUI(); // �ʱ� UI ���� ����
    }

    private void UpdateUI()
    {
        if (!string.IsNullOrEmpty(UserData.Instance.NickName))
        {
            // �α��� ����
            nickNameText.text = $"{UserData.Instance.NickName}��\nȯ���մϴ�!";
            welcomePanel.SetActive(true); // �г� Ȱ��ȭ
            loginButton.gameObject.SetActive(false);
            logoutButton.gameObject.SetActive(true);

            // ���⼭ ��ư�� ������ �¹��̰� ���� Ȩ ������ �̾���
            startButton.gameObject.SetActive(true);
        }
        else
        {
            // �α׾ƿ� ����
            nickNameText.text = "";
            welcomePanel.SetActive(false); // �г� Ȱ��ȭ
            nickNameText.gameObject.SetActive(false);
            loginButton.gameObject.SetActive(true);
            logoutButton.gameObject.SetActive(false);

            // ���� ���� ��ư ��Ȱ��ȭ
            startButton.gameObject.SetActive(false);
        }
    }

    public void Logout()
    {
        // Singleton ������ �ʱ�ȭ
        UserData.Instance.Logout();

        // UI ����
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
