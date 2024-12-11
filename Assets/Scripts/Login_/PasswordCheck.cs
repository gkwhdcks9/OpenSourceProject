using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PasswordCheck : MonoBehaviour
{
    // 두 개의 InputField를 연결
    public InputField passwordField;
    public InputField confirmPasswordField;
    // 결과를 표시할 Text
    public Text resultText;
    public GameObject panel;

    // 패널을 몇 초 동안 표시할 지 설정
    public float displayDuration = 2f;

    // 검증 함수
    public void CheckPasswords()
    {
        string password = passwordField.text;
        string confirmPassword = confirmPasswordField.text;

        // 비밀번호 일치 여부 확인
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
        {
            ShowPanel("비밀번호와 확인란을 모두 입력하세요.", Color.red);
        }
        else if (password == confirmPassword)
        {
            ShowPanel("비밀번호가 일치합니다.", Color.green);
        }
        else
        {
            ShowPanel("비밀번호가 일치하지 않습니다.", Color.red);
        }
    }
    
    // 패널을 표시하는 함수
    private void ShowPanel(string message, Color color)
    {
        panel.SetActive(true);              // 패널 활성화
        resultText.text = message;         // 메시지 설정
        resultText.color = color;          // 텍스트 색상 설정
        StopAllCoroutines();               // 기존 코루틴 중지
        StartCoroutine(HidePanelAfterDelay()); // 일정 시간 후 패널 비활성화
    }

    // 일정 시간 후 패널을 비활성화하는 코루틴
    private IEnumerator HidePanelAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration); // 설정된 시간만큼 대기
        panel.SetActive(false);                          // 패널 비활성화
    }
}
