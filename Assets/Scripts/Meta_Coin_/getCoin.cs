using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class getCoin : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI coinText; // �ǽð� ���� ���� ǥ���� TextMeshProUGUI

    private int currentCoin; // ���� ���� ���� ������ ����

    void Start()
    {
        // PlayerPrefs���� ���� ���� �ҷ���, �⺻���� 0
        currentCoin = PlayerPrefs.GetInt("coinCount", 0);
        UpdateCoinDisplay();
    }

    void Update()
    {
        // ���÷�, Ű �Է� �� ���� �� ���� (���⼭�� �ܼ��� 'C' Ű�� ������ �� 1 ����)
        if (Input.GetKeyDown(KeyCode.C))
        {
            currentCoin++;
            PlayerPrefs.SetInt("coinCount", currentCoin); // ����� ���� �� ����
            UpdateCoinDisplay();
        }

        // AI ���� ������ �� ������ �����ϰ� UI�� �ݿ�
        if (Input.GetKeyDown(KeyCode.A)) // AI �� ���� ����
        {
            currentCoin--;
            PlayerPrefs.SetInt("coinCount", currentCoin); // ����� ���� �� ����
            UpdateCoinDisplay();
        }
    }

    // ���� UI �ؽ�Ʈ�� ������Ʈ�ϴ� �Լ�
    void UpdateCoinDisplay()
    {
        coinText.text = "Coins: " + currentCoin.ToString();
    }
}
