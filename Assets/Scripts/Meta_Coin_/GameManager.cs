using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int coinCount = 1; // ���� ���� ��

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadCoinCount();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // �����̽��� ������ ���� ����
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddCoins(1);
            // UI ������Ʈ
            CoinDisplay coinDisplay = FindObjectOfType<CoinDisplay>();
            if (coinDisplay != null)
            {
                coinDisplay.UpdateCoinDisplay();
            }
        }

        // ���� ��Ʈ��Ű ������ ���� ���� (������ 1�� �̻� ���� ���)
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (HasEnoughCoins(1))
            {
                UseCoin(1);
                // UI ������Ʈ
                CoinDisplay coinDisplay = FindObjectOfType<CoinDisplay>();
                if (coinDisplay != null)
                {
                    coinDisplay.UpdateCoinDisplay();
                }
            }
        }
    }

    // ���� �� �ҷ�����
    void LoadCoinCount()
    {
        coinCount = PlayerPrefs.GetInt("coinCount", 0);
    }

    // ���� ������� üũ�ϴ� �޼���
    public bool HasEnoughCoins(int amount)
    {
        return coinCount >= amount;
    }

    // ������ ����ϴ� �޼���
    public void UseCoin(int amount)
    {
        coinCount -= amount;
        PlayerPrefs.SetInt("coinCount", coinCount);
    }
    
    public void AddCoins(int amount)
    {
        coinCount += amount;
        SaveCoinCount(); // ����� ���� ���� �ٷ� ����
    }

    public void SaveCoinCount()
    {
        PlayerPrefs.SetInt("CoinCount", coinCount);
        PlayerPrefs.Save();
    }
}
