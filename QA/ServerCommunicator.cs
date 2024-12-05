using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;
public class ServerCommunicator : MonoBehaviour
{
    private const string ServerUrl = "http://localhost:5000/receive_data"; // Flask 서버 URL
   
    // 데이터 송신 코루틴
    public IEnumerator SendDataToServer(string message, QuestionHandler questionHandler, float category)
    {
        float in_category = category;
        questionHandler.SetExporting(true); // 데이터 송신 중

        Data data = new Data { category = in_category, message = message };
        string jsonData = JsonUtility.ToJson(data);

        UnityWebRequest request = new UnityWebRequest(ServerUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // 서버로 데이터 전송
        yield return request.SendWebRequest();
        questionHandler.SetExporting(false); // 데이터 송신 완료
        questionHandler.SetImporting(true); // 데이터 수신 중

        if (request.result == UnityWebRequest.Result.Success)
        {
            questionHandler.SetImporting(false); // 데이터 수신 완료
            Debug.Log("Data sent successfully: " + request.downloadHandler.text);
            var response = JsonUtility.FromJson<Data>(request.downloadHandler.text);
            //questionHandler.OnReceiveAnswer(response.message); // 답변 전달
            questionHandler.OnReceiveAnswer("최초 도급인이 ‘상위 수급인’에 포함되지 않는 이유는 다음과 같습니다: 문언 해석: ‘상위 수급인’이 되기 위해서는 ‘수급인’, 즉 도급이나 하도급을 받은 자에 해당해야 하는데, 최초 도급인은 문언상 수급인에 해당하지 않습니다. 입법 취지: 도급이 한 차례만 이루어진 경우에는 도급인을 직상 수급인으로 보아 근로자의 임금 연대책임을 달성할 필요가 있습니다. 그러나 도급이 두 차례 이상 이루어진 경우에는 직상 수급인이나 상위 수급인에 대해 이미 연대책임을 물을 수 있으므로 최초 도급인을 포함시킬 필요성이 없습니다. 사업자 여부: 최초 도급인이 사업자가 아닌 경우도 많아 이를 상위 수급인으로 간주하는 것은 입법 취지와 부합하지 않습니다.");
        }
        else
        {
            Debug.LogError("Error sending data: " + request.error);
        }
        
    }

    // 서버로 보낼 데이터 클래스
    public class Data
    {
        public float category = 0.0f;/*
        1. 공법
            - 1. 헌법
            - 2. 행정법
            - 3. 형법
            - 4. 형사소송법
            - 5. 국제법
        2. 사법
            - 1. 민법
            - 2. 상법
            - 3. 민사소송법
            - 4. 국제사법
        3. 사회법
            - 1. 노동법
            - 2. 사회보장법
            - 3. 경제법
        */ 
        public string message = "";
    }
}
