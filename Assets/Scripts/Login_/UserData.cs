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
        DontDestroyOnLoad(gameObject); // 씬 이동 시 삭제되지 않음
    }

    // 닉네임 설정 메서드
    public void SetNickName(string nickName)
    {
        NickName = nickName;
    }

    // 로그아웃 메서드
    public void Logout()
    {
        NickName = null; // 닉네임 초기화
    }
}
