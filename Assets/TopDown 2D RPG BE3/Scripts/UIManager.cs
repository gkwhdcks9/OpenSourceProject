using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections; // IEnumerator가 포함된 네임스페이스

public class UIManager : MonoBehaviour
{
    [SerializeField] public InputField input_content;
    [SerializeField] public Button enter_btn;
    [SerializeField] public GameObject qTextPrefab; // 질문 프리팹
    [SerializeField] public GameObject aTextPrefab; // 답변 프리팹
    [SerializeField] public Transform contentParent; // 질문과 답변을 추가할 부모 오브젝트 (스크롤뷰의 Content와 같은 역할)

    private List<GameObject> createdObjects = new List<GameObject>();
    private Text currentAnswerUI;

    public float delay = 0.05f; // 텍스트 표시 속도


    // 질문과 답변 UI 생성
    public void CreateQuestionAndAnswer(string questionText)
    {
        enter_btn.interactable = false;
        // 질문 텍스트 프리팹 생성
        GameObject questionObj = Instantiate(qTextPrefab, contentParent);
        Text questionUI = questionObj.transform.Find("userCmts").Find("questionText").GetComponent<Text>();
        questionUI.text = questionText; // InputField에서 가져온 텍스트로 설정
        createdObjects.Add(questionObj);

        // 답변 텍스트 프리팹 생성
        GameObject answerObj = Instantiate(aTextPrefab, contentParent);
        currentAnswerUI = answerObj.transform.Find("SysComments").Find("answerText").GetComponent<Text>();
        currentAnswerUI.text = "답변 준비 중..."; // 초기 답변 텍스트 설정
        createdObjects.Add(answerObj);

     
    }

    // 답변 UI 업데이트
    public void UpdateAnswer(string answerText)
    {
        if (currentAnswerUI != null)
        {
            StartCoroutine(ShowTextOneByOne(answerText));
        }
        
    }

    // 텍스트 한 글자씩 표시
    IEnumerator ShowTextOneByOne(string answerText) // 제네릭이 아닌 기본 IEnumerator 사용
    {
        currentAnswerUI.text = "";
        for (int i = 0; i < answerText.Length; i++)
        {
            currentAnswerUI.text += answerText[i];
            yield return new WaitForSeconds(delay);
        }
        enter_btn.interactable = true;
    }

    // UI 초기화
    public void ClearAll()
    {
        foreach (GameObject obj in createdObjects)
        {
            Destroy(obj); // 생성된 프리팹 삭제
        }
        createdObjects.Clear(); // 리스트 초기화
        input_content.text = ""; // inputfield 초기화
        Debug.Log("모든 질문과 답변이 삭제되었습니다.");
    }
}
