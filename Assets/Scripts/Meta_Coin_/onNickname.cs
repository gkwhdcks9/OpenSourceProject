using UnityEngine;
using TMPro;

public class onNickname : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nicknameText; // 닉네임 텍스트
    [SerializeField] private Transform characterTransform; // 캐릭터 Transform
    [SerializeField] private Vector3 offset = new Vector3(0, 0, 0); // 텍스트의 위치 오프셋

    void Start()
    {
        // TextMeshPro가 인스펙터에 할당되지 않은 경우 자동으로 찾기
        if (nicknameText == null)
        {
            nicknameText = GetComponentInChildren<TextMeshProUGUI>();
        }

        if (nicknameText != null)
        {
            nicknameText.text = UserData.Instance.NickName; // 닉네임 설정
        }
        else
        {
            Debug.LogError("TextMeshPro 컴포넌트를 찾을 수 없습니다.");
        }

        // 캐릭터 Transform이 인스펙터에 설정되지 않았으면 자동으로 부모를 할당
        if (characterTransform == null)
        {
            characterTransform = transform;
        }
    }

    void Update()
    {
        if (characterTransform != null && nicknameText != null)
        {
            // 캐릭터 머리 위에 텍스트 위치 업데이트
            nicknameText.transform.position = Camera.main.WorldToScreenPoint(characterTransform.position + offset);
        }
    }
}
