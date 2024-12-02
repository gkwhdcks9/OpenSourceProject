using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class PostData
{
    public string title;
    public string contents;
    public string nickname;

    public PostData(string title, string contents, string nickname)
    {
        this.title = title;
        this.contents = contents;
        this.nickname = nickname;
    }
}

[System.Serializable]
public class PostDataResponse
{
    public long post_id;
    public string title;
    public string contents;
    public string nickname;

    public PostDataResponse(long post_id, string title, string contents, string nickname)
    {
        this.post_id = post_id;
        this.title = title;
        this.contents = contents;
        this.nickname = nickname;
    }
}

public class PostAPI : MonoBehaviour
{
    // �Խù� ���� API ȣ��
    public IEnumerator CreatePost(string title, string contents, string nickname, System.Action<string> onSuccess, System.Action<string> onError)
    {
        string url = "http://wonokim.iptime.org:4000/api/v1/post/register";

        // PostData ��ü ����
        PostData postData = new PostData(title, contents, nickname);

        // JSON ��ȯ (Unity JsonUtility ���)
        string json = JsonUtility.ToJson(postData);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(request.downloadHandler.text);
            }
            else
            {
                onError?.Invoke(request.error);
            }
        }
    }

    // �Խù� �˻� API ȣ��
    public IEnumerator SearchPosts(string keyword, System.Action<string> onSuccess, System.Action<string> onError)
    {
        string url = $"http://wonokim.iptime.org:4000/api/v1/post/Search_list/{keyword}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(request.downloadHandler.text);
            }
            else
            {
                onError?.Invoke(request.error);
            }
        }
    }

    // ��� �Խù� �������� API ȣ�� (�ֱ� 7�� ����)
    public IEnumerator GetAllPost(System.Action<string> onSuccess, System.Action<string> onError)
    {
        string url = $"http://wonokim.iptime.org:4000/api/v1/post/latest_list";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(request.downloadHandler.text);
            }
            else
            {
                onError?.Invoke(request.error);
            }
        }
    }

    // ����� �Խù� �ҷ����� API ȣ��
    public IEnumerator GetUserPosts(string nickname, System.Action<string> onSuccess, System.Action<string> onError)
    {
        string url = $"http://wonokim.iptime.org:4000/api/v1/post/user-board-list/{nickname}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(request.downloadHandler.text);
            }
            else
            {
                onError?.Invoke(request.error);
            }
        }
    }
}
