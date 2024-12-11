using UnityEngine;
using TMPro;

public class onNickname : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nicknameText; // �г��� �ؽ�Ʈ
    [SerializeField] private Transform characterTransform; // ĳ���� Transform
    [SerializeField] private Vector3 offset = new Vector3(0, 0, 0); // �ؽ�Ʈ�� ��ġ ������

    void Start()
    {
        // TextMeshPro�� �ν����Ϳ� �Ҵ���� ���� ��� �ڵ����� ã��
        if (nicknameText == null)
        {
            nicknameText = GetComponentInChildren<TextMeshProUGUI>();
        }

        if (nicknameText != null)
        {
            nicknameText.text = UserData.Instance.NickName; // �г��� ����
        }
        else
        {
            Debug.LogError("TextMeshPro ������Ʈ�� ã�� �� �����ϴ�.");
        }

        // ĳ���� Transform�� �ν����Ϳ� �������� �ʾ����� �ڵ����� �θ� �Ҵ�
        if (characterTransform == null)
        {
            characterTransform = transform;
        }
    }

    void Update()
    {
        if (characterTransform != null && nicknameText != null)
        {
            // ĳ���� �Ӹ� ���� �ؽ�Ʈ ��ġ ������Ʈ
            nicknameText.transform.position = Camera.main.WorldToScreenPoint(characterTransform.position + offset);
        }
    }
}
