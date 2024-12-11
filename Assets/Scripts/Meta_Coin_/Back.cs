using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  // �� ���� �߰�
using UnityEngine.UI;  // UI ����

public class Back : MonoBehaviour
{
    // ��ư�� �ν����Ϳ��� ����
    public Button backButton;

    // Start is called before the first frame update
    void Start()
    {
        // ��ư Ŭ�� �̺�Ʈ�� �޼��� ����
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButtonClick);
        }
    }

    // ��ư Ŭ�� �� ȣ��Ǵ� �Լ�
    void OnBackButtonClick()
    {
        // "Home" ���� �ε��մϴ�.
        SceneManager.LoadScene("MainScene");
    }
}
