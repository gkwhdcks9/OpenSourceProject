using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int coinCount = 1; // 현재 코인 수

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
        // 스페이스바 누르면 코인 증가
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddCoins(1);
            // UI 업데이트
            CoinDisplay coinDisplay = FindObjectOfType<CoinDisplay>();
            if (coinDisplay != null)
            {
                coinDisplay.UpdateCoinDisplay();
            }
        }

        // 왼쪽 컨트롤키 누르면 코인 감소 (코인이 1개 이상 있을 경우)
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (HasEnoughCoins(1))
            {
                UseCoin(1);
                // UI 업데이트
                CoinDisplay coinDisplay = FindObjectOfType<CoinDisplay>();
                if (coinDisplay != null)
                {
                    coinDisplay.UpdateCoinDisplay();
                }
            }
        }
    }

    // 코인 수 불러오기
    void LoadCoinCount()
    {
        coinCount = PlayerPrefs.GetInt("coinCount", 0);
    }

    // 코인 충분한지 체크하는 메서드
    public bool HasEnoughCoins(int amount)
    {
        return coinCount >= amount;
    }

    // 코인을 사용하는 메서드
    public void UseCoin(int amount)
    {
        coinCount -= amount;
        PlayerPrefs.SetInt("coinCount", coinCount);
    }
    
    public void AddCoins(int amount)
    {
        coinCount += amount;
        SaveCoinCount(); // 변경된 코인 수를 바로 저장
    }

    public void SaveCoinCount()
    {
        PlayerPrefs.SetInt("CoinCount", coinCount);
        PlayerPrefs.Save();
    }
}
