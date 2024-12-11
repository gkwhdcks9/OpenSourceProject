using UnityEngine;
using UnityEngine.SceneManagement;

public class Move_HOME : MonoBehaviour
{
    // 버튼 클릭 시 호출되는 함수
    public void OnButtonClick()
    {
        // "Interactable" 태그를 가진 게임 오브젝트 찾기
        GameObject[] interactableObjects = GameObject.FindGameObjectsWithTag("Interactable");

        // 각 게임 오브젝트에 대해 처리
        foreach (GameObject obj in interactableObjects)
        {
            // 게임 오브젝트에서 콜라이더가 존재하는지 확인
            Collider col = obj.GetComponent<Collider>();
            if (col != null)
            {
                // 해당 씬으로 로드 (씬 이름은 게임 오브젝트의 이름으로 설정)
                string sceneName = obj.name;
                SceneManager.LoadScene(sceneName);
                return; // 첫 번째로 찾은 씬을 로드하고 종료
            }
        }

        // 만약 Interactable 태그가 있는 객체가 없다면 예외처리 (필요에 따라 추가)
        Debug.Log("Interactable 태그를 가진 유효한 객체가 없습니다.");
    }
}
