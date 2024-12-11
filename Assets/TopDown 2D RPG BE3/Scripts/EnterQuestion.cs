using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class EnterQuestion : MonoBehaviour
{
    

    [SerializeField] public InputField input_content;
    [SerializeField] public Button enter_btn;
    [SerializeField] public GameObject qTextPrefab; // q_text 프리팹
    [SerializeField] public GameObject aTextPrefab; // a_text 프리팹
    [SerializeField] public Transform contentParent; // 질문과 답변을 추가할 부모 오브젝트 (스크롤뷰의 Content와 같은 역할)
    public bool exporting = false;//데이터 전송 중
    public bool importing = false;//데이터 수신 완료 여부
    public Data box;  // 서버로부터 받은 메시지 저장

    
    private List<GameObject> createdObjects = new List<GameObject>(); // 생성된 프리팹 저장 리스트

    private Text currentAnswerUI; 
    public float delay = 0.2f;
    public class Data{
        
        public string message = "";
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void EnterBtn(){//엔터버튼 누르면 질문 등록

        enter_btn.interactable = false;

        if(!string.IsNullOrEmpty(input_content.text)){
            if(!exporting && !importing){
                
                CreateQuestionAndAnswer(input_content.text);
                // 데이터 전송
                StartCoroutine(SendDataToFlask(input_content.text));//성공적으로 진행되면 exporting = false, importing = true로 바뀜
                input_content.text = ""; // inputfield 초기화
            }
        }else{}
            
    }
    void CreateQuestionAndAnswer(string questionText)//질문, 답변 프리팹 생성 이후에 답변이 올 때까지 로딩표시
    {
        // 질문 텍스트 프리팹 생성
        GameObject questionObj = Instantiate(qTextPrefab, contentParent);
        Text questionUI = questionObj.transform.Find("UserQuestion").GetComponent<Text>(); // 자식 오브젝트의 Text 컴포넌트 찾기
        questionUI.text =  questionText;
        createdObjects.Add(questionObj); // 생성된 질문 프리팹 저장

        // 답변 텍스트 프리팹 생성 (여기서는 아직 답변이 없는 상태)
        GameObject answerObj = Instantiate(aTextPrefab, contentParent);
        currentAnswerUI = answerObj.transform.Find("SystemAnswer").GetComponent<Text>(); // currentAnswerUI 필드에 할당
        currentAnswerUI.text = "답변 준비 중...";
        createdObjects.Add(answerObj); // 생성된 답변 프리팹 저장
        
    }
    public void UpdateAnswer(string answerText)//답변 표시
    {
        // 현재 답변 UI 텍스트를 업데이트
        if (currentAnswerUI != null)
        {
            StartCoroutine(ShowTextOneByOne(answerText));
        }
    }
    IEnumerator ShowTextOneByOne(string answerText)
    {
        currentAnswerUI.text = "";  // 처음에는 빈 텍스트로 시작

        for (int i = 0; i < answerText.Length; i++)
        {
            currentAnswerUI.text += answerText[i];  // 문자열을 하나씩 추가
            yield return new WaitForSeconds(delay);  // delay 시간만큼 대기
        }
    }
    public void ClearAll()//초기화 버튼
    {
        foreach (GameObject obj in createdObjects)
        {
            Destroy(obj); // 생성된 프리팹 삭제
        }
        createdObjects.Clear(); // 리스트 초기화
        input_content.text = ""; // inputfield 초기화
        Debug.Log("모든 질문과 답변이 삭제되었습니다.");
    }
    //데이터 송신하는 코드
    IEnumerator SendDataToFlask(string message)
    {
        // 보낼 데이터 생성
        Data data = new Data();
        data.message = message;

        // 데이터를 JSON 형태로 변환
        string jsonData = JsonUtility.ToJson(data);

        // Flask 서버 URL
        string url = "http://localhost:5000/receive_data";  // Flask 서버 IP 및 엔드포인트 주소

        // POST 요청 생성
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // 요청 전송
        yield return request.SendWebRequest();
        exporting = false;
        importing = true;
        // 결과 확인
        if (request.result == UnityWebRequest.Result.Success)
        {
            importing = false;
            Debug.Log("Data sent successfully: " + request.downloadHandler.text);
            var response = JsonUtility.FromJson<Data>(request.downloadHandler.text);
            Debug.Log("response : " + response);
            UpdateAnswer(response.message);
        }
        else
        {
            Debug.Log("Error sending data: " + request.error);
        }
        enter_btn.interactable = true;
    }

}
