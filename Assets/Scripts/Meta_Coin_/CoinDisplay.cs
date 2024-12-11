using UnityEngine;
using TMPro;

public class CoinDisplay : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI coinText; // �ǽð� ���� ���� ǥ���� TextMeshProUGUI

    void Start()
    {
        UpdateCoinDisplay();
    }

    void Update()
    {
        // GameManager�� coinCount ���� �ǽð����� UI�� �ݿ�
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
