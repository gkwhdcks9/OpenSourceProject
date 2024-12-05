using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class QuestionHandler : MonoBehaviour
{
    public UIManager uiManager; // UIManager 참조
    public ServerCommunicator serverCommunicator; // 서버 통신 클래스 참조

    [SerializeField] Button enter_btn;
    private bool exporting = false;
    private bool importing = false;

    // 엔터 버튼을 누르면 질문 등록
    public void EnterBtn()
    {
        if (!string.IsNullOrEmpty(uiManager.input_content.text) && !exporting && !importing)
        {
            string questionText = uiManager.input_content.text;
            
            uiManager.CreateQuestionAndAnswer(questionText); // 질문 UI 생성
             StartCoroutine(serverCommunicator.SendDataToServer(questionText, this, 1.1f)); // 데이터 전송
            uiManager.input_content.text = ""; // inputfield 초기화
           
        }
        
    }

    // 서버에서 데이터 수신 후 답변 업데이트
    public void OnReceiveAnswer(string answerText)
    {
        uiManager.UpdateAnswer(answerText); // 답변 UI 업데이트
    }

    // 데이터 송신 상태 업데이트
    public void SetExporting(bool value) => exporting = value;

    // 데이터 수신 상태 업데이트
    public void SetImporting(bool value) => importing = value;
}
