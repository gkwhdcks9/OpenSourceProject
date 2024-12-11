using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public void AddCoins(int amount)
    {
        int currentCoins = PlayerPrefs.GetInt("CoinCount", 0);
        currentCoins += amount;
        PlayerPrefs.SetInt("CoinCount", currentCoins);  // ����� �� ����
        PlayerPrefs.Save(); // ��������� ��� ����
    }
}
