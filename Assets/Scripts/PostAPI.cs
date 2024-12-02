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
    // 게시물 생성 API 호출
    public IEnumerator CreatePost(string title, string contents, string nickname, System.Action<string> onSuccess, System.Action<string> onError)
    {
        string url = "http://wonokim.iptime.org:4000/api/v1/post/register";

        // PostData 객체 생성
        PostData postData = new PostData(title, contents, nickname);

        // JSON 변환 (Unity JsonUtility 사용)
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

    // 게시물 검색 API 호출
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

    // 모든 게시물 가져오기 API 호출 (최근 7일 기준)
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

    // 사용자 게시물 불러오기 API 호출
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
