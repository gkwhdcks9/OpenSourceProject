using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ArchiveManagerGeunro : MonoBehaviour
{
    [System.Serializable]
    private class PostListContainerResponse
    {
        public List<PostDataResponse> posts;
    }

    public TextMeshProUGUI tmpdetailContent; // �� �г� ���� (�� �̻ڰ� ���)

    public Button helpButton;             // ���� ��ư
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

    long categoryId; // ������(ī�װ�) id
    string categoryName; // ������(ī�װ�) name

    private void Start()
    {
        categoryId = 2;
        categoryName = "Geunro";

        // ��ư Ŭ�� �̺�Ʈ �ʱ�ȭ
        nextPageButton.onClick.AddListener(() => ChangePage(1));
        prevPageButton.onClick.AddListener(() => ChangePage(-1));
        closeButton.onClick.AddListener(HideDetailPanel);
        plusButton.onClick.AddListener(ShowInputPanel);

        // ����� �Խù� �ҷ�����
        LoadUserPosts();
    }

    public void GoToHomeScene()
    {
        SceneManager.LoadScene("HomeScene");
    }

    // ����� �Խù� �ҷ�����
    private void LoadUserPosts()
    {
        //string nickname = "admin"; // �׽�Ʈ�� �г��� (������ UserData.Instance.NickName�� ���� �̱��� ���)
        string nickname = UserData.Instance.NickName;

        StartCoroutine(postAPI.GetUserPosts(
            nickname,
            onSuccess: (response) =>
            {
                //posts = ParsePostsResponse(response);
                
                // ��ü ����� �Խù��� ������ �� �Ľ�
                var allUserPosts = ParsePostsResponse(response);

                // category_id�� �´� �Խù��� ���͸�
                posts = allUserPosts
                    .Where(post => post.category_id == categoryId)
                    .ToList();

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

    public void ShowHelp()
    {
        // ���� ����� ���� ����
        detailTitle.text = "����";
        tmpdetailContent.text = "�� ��Ͻǿ����� ����� ������ �Խ��� �ۼ��۸� ������ �� �ֽ��ϴ�. ��\n\n" +
                                "��Ͻǿ����� �Խù� �ۼ�, �˻� ����� �����մϴ�.\n\n" +
                                "1. ��Ͻ� �ۼ�: '+' ��ư�� Ŭ���Ͽ� ���ο� �Խù��� �ۼ��� �� �ֽ��ϴ�.\n\n" +
                                "2. ��Ͻ� �˻�: �˻� â�� Ű���带 �Է��ϰ� �˻� ��ư�� ���� �Խù��� �˻��ϼ���.\n\n" +
                                "3. �� ����: �Խù��� Ŭ���ϸ� �� ������ Ȯ���� �� �ֽ��ϴ�.\n\n" +
                                "4. ������ �̵�: �ϴ��� ȭ��ǥ ��ư�� ����� �������� �̵��� �� �ֽ��ϴ�.";
        nicknames.text = ""; // �г����� �ʿ� �����Ƿ� �����

        // �г� Ȱ��ȭ
        detailPanel.SetActive(true);
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
        // ������ ������ ��� �̱��� ������ ����� �÷��̾� �г������� ����
        string nickname = UserData.Instance.NickName;
        //string nickname = "wonho"; // �׽�Ʈ�� �г���

        if (string.IsNullOrEmpty(categoryName))
        {
            Debug.LogError($"Cannot determine category.");
            return;
        }

        if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(contents))
        {
            Debug.LogError("Title or Content is empty.");
            return;
        }

        StartCoroutine(postAPI.CreatePost(
            title, contents, nickname, categoryName,
            onSuccess: (response) =>
            {
                Debug.Log("Post created successfully: " + response);

                LoadUserPosts(); // ���ΰ�ħ
                
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

        //string playerNickname = "����ŷ";
        string playerNickname = UserData.Instance.NickName; // �̱��� ������ ���� �÷��̾� �г��� ��������

        if (string.IsNullOrEmpty(keyword)) // �˻�� ��������� ����� �Խù� �ٽ� ��ȸ
        {
            Debug.Log("Search keyword is empty. Loading user posts...");
            ExitSearchMode();
            return;
        }

        StartCoroutine(postAPI.SearchPosts(keyword,
            onSuccess: (response) =>
            {
                // ���� ������ �Ľ�
                var allSearchResults = ParsePostsResponse(response);

                // �г��� �� category_id �������� ���͸�
                searchResults = allSearchResults
                    .Where(post => post.nickname == playerNickname && post.category_id == categoryId)
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
        // ���� UI ����
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // ǥ���� ������ ����
        var currentPosts = isSearchMode ? searchResults : posts;
        var pagePosts = GetPostsForPage(currentPosts, currentPage);

        // ��ü �Խù��� �� �������� ��ȣ ����
        int displayIndex = currentPosts.Count - ((currentPage - 1) * postsPerPage);

        // �Խù� UI ����
        foreach (var post in pagePosts)
        {
            var postObject = Instantiate(postPrefab, contentParent);

            // ��ȣ ���� (��ü ���� ��������)
            postObject.transform.Find("Id").GetComponent<Text>().text = displayIndex.ToString();
            //postObject.transform.Find("Id").GetComponent<Text>().text = post.post_id.ToString();

            // ���� ����: 8���ڸ� �ʰ��ϸ� ... ����
            string titleToDisplay = post.title.Length > 10 ? post.title.Substring(0, 10) + "..." : post.title;
            postObject.transform.Find("Title").GetComponent<Text>().text = titleToDisplay;

            var button = postObject.GetComponent<Button>();
            button.onClick.AddListener(() => ShowDetailPanel(post));
            
            displayIndex--; // ���� ��ȣ�� ����
        }

        // ������ ���� ������Ʈ
        pageText.text = $"Page {currentPage} / {GetTotalPages()}";
        prevPageButton.interactable = currentPage > 1;
        nextPageButton.interactable = currentPage < GetTotalPages();
    }

    public void ShowDetailPanel(PostDataResponse post)
    {
        detailTitle.text = post.title;
        tmpdetailContent.text = post.contents;
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
        Debug.Log("Parsing response: " + json);  // JSON ���� ���
        var container = JsonUtility.FromJson<PostListContainerResponse>($"{{\"posts\":{json}}}");
        return container.posts;
    }
}
