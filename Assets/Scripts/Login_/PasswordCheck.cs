using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PasswordCheck : MonoBehaviour
{
    // �� ���� InputField�� ����
    public InputField passwordField;
    public InputField confirmPasswordField;
    // ����� ǥ���� Text
    public Text resultText;
    public GameObject panel;

    // �г��� �� �� ���� ǥ���� �� ����
    public float displayDuration = 2f;

    // ���� �Լ�
    public void CheckPasswords()
    {
        string password = passwordField.text;
        string confirmPassword = confirmPasswordField.text;

        // ��й�ȣ ��ġ ���� Ȯ��
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
        {
            ShowPanel("��й�ȣ�� Ȯ�ζ��� ��� �Է��ϼ���.", Color.red);
        }
        else if (password == confirmPassword)
        {
            ShowPanel("��й�ȣ�� ��ġ�մϴ�.", Color.green);
        }
        else
        {
            ShowPanel("��й�ȣ�� ��ġ���� �ʽ��ϴ�.", Color.red);
        }
    }
    
    // �г��� ǥ���ϴ� �Լ�
    private void ShowPanel(string message, Color color)
    {
        panel.SetActive(true);              // �г� Ȱ��ȭ
        resultText.text = message;         // �޽��� ����
        resultText.color = color;          // �ؽ�Ʈ ���� ����
        StopAllCoroutines();               // ���� �ڷ�ƾ ����
        StartCoroutine(HidePanelAfterDelay()); // ���� �ð� �� �г� ��Ȱ��ȭ
    }

    // ���� �ð� �� �г��� ��Ȱ��ȭ�ϴ� �ڷ�ƾ
    private IEnumerator HidePanelAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration); // ������ �ð���ŭ ���
        panel.SetActive(false);                          // �г� ��Ȱ��ȭ
    }
}
