using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData : MonoBehaviour
{
    public static UserData Instance { get; private set; }

    public string NickName { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // �� �̵� �� �������� ����
    }

    // �г��� ���� �޼���
    public void SetNickName(string nickName)
    {
        NickName = nickName;
    }

    // �α׾ƿ� �޼���
    public void Logout()
    {
        NickName = null; // �г��� �ʱ�ȭ
    }
}
