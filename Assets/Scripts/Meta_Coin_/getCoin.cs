using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class getCoin : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI coinText; // 실시간 코인 값을 표시할 TextMeshProUGUI

    private int currentCoin; // 현재 코인 값을 저장할 변수

    void Start()
    {
        // PlayerPrefs에서 코인 값을 불러옴, 기본값은 0
        currentCoin = PlayerPrefs.GetInt("coinCount", 0);
        UpdateCoinDisplay();
    }

    void Update()
    {
        // 예시로, 키 입력 시 코인 값 변경 (여기서는 단순히 'C' 키를 눌렀을 때 1 증가)
        if (Input.GetKeyDown(KeyCode.C))
        {
            currentCoin++;
            PlayerPrefs.SetInt("coinCount", currentCoin); // 변경된 코인 값 저장
            UpdateCoinDisplay();
        }

        // AI 씬에 입장할 때 코인을 차감하고 UI에 반영
        if (Input.GetKeyDown(KeyCode.A)) // AI 씬 입장 예시
        {
            currentCoin--;
            PlayerPrefs.SetInt("coinCount", currentCoin); // 변경된 코인 값 저장
            UpdateCoinDisplay();
        }
    }

    // 코인 UI 텍스트를 업데이트하는 함수
    void UpdateCoinDisplay()
    {
        coinText.text = "Coins: " + currentCoin.ToString();
    }
}
