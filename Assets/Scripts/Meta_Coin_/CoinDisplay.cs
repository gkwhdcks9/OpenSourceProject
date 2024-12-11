using UnityEngine;
using TMPro;

public class CoinDisplay : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI coinText; // 실시간 코인 값을 표시할 TextMeshProUGUI

    void Start()
    {
        UpdateCoinDisplay();
    }

    void Update()
    {
        // GameManager의 coinCount 값을 실시간으로 UI에 반영
        UpdateCoinDisplay();
    }

    public void UpdateCoinDisplay()
    {
        int currentCoin = 0;
        if (GameManager.Instance != null)
        {
            currentCoin = GameManager.Instance.coinCount;
        }

        coinText.text = "Coins: " + currentCoin.ToString();
    }
}
