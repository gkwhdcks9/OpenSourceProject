using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PostManager : MonoBehaviour
{
    [System.Serializable]
    private class PostListContainerResponse
    {
        public List<PostDataResponse> posts;
    }

    public GameObject postPrefab;         // �Խù� Prefab
    public Transform contentParent;       // ScrollView Content �θ�
    public Text pageText;                 // ������ ��ȣ ǥ�� Text
    public Button nextPageButton;         // ���� ������ ��ư
    public Button prevPageButton;         // ���� ������ ��ư

    public Button plusButton;             // �Խù� �߰� ��ư
    public GameObject InputPanel;          // �Խù� �߰� �г�

    public GameObject detailPanel;        // �� ���� �г�
    public Text detailTitle;              // �� �г� ����
    public Text detailContent;            // �� �г� ����
    public Text nicknames;                // �� �гο��� ������ �г���
    public Button closeButton;            // �� �г� �ݱ� ��ư

    public InputField searchInputField;   // �˻��� �Է� �ʵ�
    public InputField titleInputField;    // ���� �Է� �ʵ�
    public InputField contentInputField;  // ���� �Է� �ʵ�
    public PostAPI postAPI;               // PostAPI ����

    private List<PostDataResponse> posts = new List<PostDataResponse>(); // ��� �Խù� ������
    private List<PostDataResponse> searchResults = new List<PostDataResponse>(); // �˻� ��� ������
    private int currentPage = 1;          // ���� ������
    private int postsPerPage = 6;         // �� �������� �Խù� ��
    private bool isSearchMode = false;    // �˻� ��� ����

    private void Start()
    {
        // ��ư Ŭ�� �̺�Ʈ �ʱ�ȭ
        nextPageButton.onClick.AddListener(() => ChangePage(1));
        prevPageButton.onClick.AddListener(() => ChangePage(-1));
        closeButton.onClick.AddListener(HideDetailPanel);

        // ��� �Խù� �ҷ�����
        LoadAllPosts();
    }

    public void ShowInputPanel()
    {
        InputPanel.SetActive(true);
    }

    public void HideInputPanel()
    {
        InputPanel.SetActive(false);
    }

    private void LoadAllPosts()
    {
        StartCoroutine(postAPI.GetAllPost(
            onSuccess: (response) =>
            {
                posts = ParsePostsResponse(response);
                currentPage = 1;
                isSearchMode = false;
                UpdateUI();
            },
            onError: (error) =>
            {
                Debug.LogError("Failed to fetch all posts: " + error);
            }));
    }

    public void OnCreatePostButtonClicked()
    {
        string title = titleInputField.text;
        string contents = contentInputField.text;
        
        // �׽�Ʈ�� �г���
        string nickname = "����";
        
        // ������ ������ ��� �̱��� ������ ����� �÷��̾� �г������� ����
        // string nickname = UserData.Instance.NickName;

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
                LoadAllPosts(); // ���ΰ�ħ
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

        if (string.IsNullOrEmpty(keyword)) // �˻�� ��������� ��ü �Խù� ��ȸ
        {
            Debug.Log("Search keyword is empty. Loading all posts...");
            ExitSearchMode(); // �˻� ��� �����ϰ� ��ü �Խù� ��ȸ
            return;
        }

        StartCoroutine(postAPI.SearchPosts(keyword,
            onSuccess: (response) =>
            {
                searchResults = ParsePostsResponse(response);
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
        // ���� UI ����
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // ǥ���� ������ ����
        var currentPosts = isSearchMode ? searchResults : posts;
        var pagePosts = GetPostsForPage(currentPosts, currentPage);

        // �Խù� UI ����
        foreach (var post in pagePosts)
        {
            var postObject = Instantiate(postPrefab, contentParent);
            postObject.transform.Find("Id").GetComponent<Text>().text = post.post_id.ToString();
            postObject.transform.Find("Title").GetComponent<Text>().text = post.title;

            var button = postObject.GetComponent<Button>();
            button.onClick.AddListener(() => ShowDetailPanel(post));
        }

        // ������ ���� ������Ʈ
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

    private void HideDetailPanel()
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
