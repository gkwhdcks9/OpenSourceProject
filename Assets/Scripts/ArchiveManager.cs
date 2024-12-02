using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ArchiveManager : MonoBehaviour
{
    [System.Serializable]
    private class PostListContainerResponse
    {
        public List<PostDataResponse> posts;
    }

    public GameObject postPrefab;         // 게시물 Prefab
    public Transform contentParent;       // ScrollView Content 부모
    public Text pageText;                 // 페이지 번호 표시 Text
    public Button nextPageButton;         // 다음 페이지 버튼
    public Button prevPageButton;         // 이전 페이지 버튼

    public Button plusButton;             // 게시물 추가 버튼
    public GameObject InputPanel;          // 게시물 추가 패널

    public GameObject detailPanel;        // 상세 내용 패널
    public Text detailTitle;              // 상세 패널 제목
    public Text detailContent;            // 상세 패널 내용
    public Text nicknames;                // 상세 패널에서 보여줄 닉네임
    public Button closeButton;            // 상세 패널 닫기 버튼

    public InputField searchInputField;   // 검색어 입력 필드
    public InputField titleInputField;    // 제목 입력 필드
    public InputField contentInputField;  // 내용 입력 필드
    public PostAPI postAPI;               // PostAPI 연결

    private List<PostDataResponse> posts = new List<PostDataResponse>(); // 모든 게시물 데이터
    private List<PostDataResponse> searchResults = new List<PostDataResponse>(); // 검색 결과 데이터
    private int currentPage = 1;          // 현재 페이지
    private int postsPerPage = 6;         // 한 페이지당 게시물 수
    private bool isSearchMode = false;    // 검색 모드 여부

    private void Start()
    {
        // 버튼 클릭 이벤트 초기화
        nextPageButton.onClick.AddListener(() => ChangePage(1));
        prevPageButton.onClick.AddListener(() => ChangePage(-1));
        closeButton.onClick.AddListener(HideDetailPanel);
        plusButton.onClick.AddListener(ShowInputPanel);

        // 사용자 게시물 불러오기
        LoadUserPosts();
    }

    // 사용자 게시물 불러오기
    private void LoadUserPosts()
    {
        string nickname = "워노킹"; // 테스트용 닉네임 (실제는 UserData.Instance.NickName과 같은 싱글톤 사용)

        StartCoroutine(postAPI.GetUserPosts(
            nickname,
            onSuccess: (response) =>
            {
                posts = ParsePostsResponse(response);
                currentPage = 1;
                isSearchMode = false;
                UpdateUI();
                Debug.Log($"Loaded {posts.Count} posts for user: {nickname}");
            },
            onError: (error) =>
            {
                Debug.LogError("Failed to fetch user posts: " + error);
            }));
    }

    public void ShowInputPanel()
    {
        InputPanel.SetActive(true);
    }

    public void HideInputPanel()
    {
        InputPanel.SetActive(false);
    }

    public void OnCreatePostButtonClicked()
    {
        string title = titleInputField.text;
        string contents = contentInputField.text;
        // 실제로 빌드할 경우 싱글톤 패턴이 적용된 플레이어 닉네임으로 기입
        // string nickname = UserData.Instance.NickName;
        string nickname = "워노킹"; // 테스트용 닉네임

        if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(contents))
        {
            Debug.LogError("Title or Content is empty.");
            return;
        }

        StartCoroutine(postAPI.CreatePost(
            title, contents, nickname,
            onSuccess: (response) =>
            {
                Debug.Log("Post created successfully: " + response);
                LoadUserPosts(); // 새로고침
                titleInputField.text = "";
                contentInputField.text = "";
                HideInputPanel();
            },
            onError: (error) =>
            {
                Debug.LogError("Failed to create post: " + error);
            }));
    }

    public void OnSearchPostButtonClicked()
    {
        string keyword = searchInputField.text;

        string playerNickname = "워노킹";
        //string playerNickname = UserData.Instance.NickName; // 싱글톤 패턴을 통한 플레이어 닉네임 가져오기

        if (string.IsNullOrEmpty(keyword)) // 검색어가 비어있으면 사용자 게시물 다시 조회
        {
            Debug.Log("Search keyword is empty. Loading user posts...");
            ExitSearchMode();
            return;
        }

        StartCoroutine(postAPI.SearchPosts(keyword,
            onSuccess: (response) =>
            {
                // 응답 데이터 파싱
                var allSearchResults = ParsePostsResponse(response);

                // 닉네임 기준으로 필터링
                searchResults = allSearchResults
                    .Where(post => post.nickname == playerNickname)
                    .ToList();

                isSearchMode = true;
                currentPage = 1;
                UpdateUI();
            },
            onError: (error) =>
            {
                Debug.LogError("Failed to search posts: " + error);
            }));
    }

    public void ExitSearchMode()
    {
        isSearchMode = false;
        currentPage = 1;
        UpdateUI();
    }

    private void ChangePage(int direction)
    {
        int maxPage = GetTotalPages();
        currentPage = Mathf.Clamp(currentPage + direction, 1, maxPage);
        UpdateUI();
    }

    private void UpdateUI()
    {
        // 기존 UI 삭제
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // 표시할 데이터 결정
        var currentPosts = isSearchMode ? searchResults : posts;
        var pagePosts = GetPostsForPage(currentPosts, currentPage);

        // 게시물 UI 생성
        foreach (var post in pagePosts)
        {
            var postObject = Instantiate(postPrefab, contentParent);
            postObject.transform.Find("Id").GetComponent<Text>().text = post.post_id.ToString();
            postObject.transform.Find("Title").GetComponent<Text>().text = post.title;

            var button = postObject.GetComponent<Button>();
            button.onClick.AddListener(() => ShowDetailPanel(post));
        }

        // 페이지 정보 업데이트
        pageText.text = $"Page {currentPage} / {GetTotalPages()}";
        prevPageButton.interactable = currentPage > 1;
        nextPageButton.interactable = currentPage < GetTotalPages();
    }

    private void ShowDetailPanel(PostDataResponse post)
    {
        detailTitle.text = post.title;
        detailContent.text = post.contents;
        nicknames.text = post.nickname;
        detailPanel.SetActive(true);
    }

    public void HideDetailPanel()
    {
        detailPanel.SetActive(false);
    }

    private List<PostDataResponse> GetPostsForPage(List<PostDataResponse> posts, int page)
    {
        return posts.Skip((page - 1) * postsPerPage).Take(postsPerPage).ToList();
    }

    private int GetTotalPages()
    {
        var currentPosts = isSearchMode ? searchResults : posts;
        return Mathf.CeilToInt((float)currentPosts.Count / postsPerPage);
    }

    private List<PostDataResponse> ParsePostsResponse(string json)
    {
        var container = JsonUtility.FromJson<PostListContainerResponse>($"{{\"posts\":{json}}}");
        return container.posts;
    }
}
