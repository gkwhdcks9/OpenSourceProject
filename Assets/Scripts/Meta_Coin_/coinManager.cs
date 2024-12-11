using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public void AddCoins(int amount)
    {
        int currentCoins = PlayerPrefs.GetInt("CoinCount", 0);
        currentCoins += amount;
        PlayerPrefs.SetInt("CoinCount", currentCoins);  // 변경된 값 저장
        PlayerPrefs.Save(); // 변경사항을 즉시 저장
    }
}
