using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // TMP ���ӽ����̽�
using UnityEngine.SceneManagement;

public class PostManager : MonoBehaviour
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

    private void Start()
    {
        // ��ư Ŭ�� �̺�Ʈ �ʱ�ȭ
        nextPageButton.onClick.AddListener(() => ChangePage(1));
        prevPageButton.onClick.AddListener(() => ChangePage(-1));
        closeButton.onClick.AddListener(HideDetailPanel);
        helpButton.onClick.AddListener(ShowHelp);

        // ��� �Խù� �ҷ�����
        //LoadAllPosts();

        // ���� �� �̸� ��������
        string currentSceneName = SceneManager.GetActiveScene().name;

        Debug.Log(currentSceneName);
        categoryId = GetCategoryIdForScene(currentSceneName);

        // ī�װ� �� �Խù� �ҷ�����
        LoadPostsByCategory(categoryId);
    }

    private long GetCategoryIdForScene(string sceneName)
    {
        // �� �̸��� ���� ī�װ� ID ����
        switch (sceneName)
        {
            case "SobijaBoard":
                return 1; // �Һ��ں�ȣ
            case "GeunroBoard":
                return 2; // �ٷ�
            case "SemuBoard":
                return 3; // ����
            case "GajeongBoard": // ����
                return 4;
            default:
                Debug.LogWarning($"Unknown scene: {sceneName}. Defaulting to category ID 0.");
                return 0; // �⺻ ī�װ�
        }
    }

    private string GetCategoryNameForScene(string sceneName)
    {
        // �� �̸��� ���� ī�װ� �̸� ����
        switch (sceneName)
        {
            case "SobijaBoard":
                return "Sobija";
            case "GeunroBoard":
                return "Geunro";
            case "SemuBoard":
                return "Semu";
            case "GajeongBoard":
                return "Gajeong";
            default:
                Debug.LogWarning($"Unknown scene: {sceneName}. Cannot map to a category.");
                return null; // �� �� ���� ���� ��� null ��ȯ
        }
    }


    public void ShowHelp()
    {
        // ���� ����� ���� ����
        detailTitle.text = "����";
        tmpdetailContent.text = "�Խ��ǿ����� �Խù� �ۼ�, �˻� ����� �����մϴ�.\n\n" +
                             "1. �Խù� �ۼ�: '+' ��ư�� Ŭ���Ͽ� ���ο� �Խù��� �ۼ��� �� �ֽ��ϴ�.\n\n" +
                             "2. �Խù� �˻�: �˻� â�� Ű���带 �Է��ϰ� �˻� ��ư�� ���� �Խù��� �˻��ϼ���.\n\n" +
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

    // ī�װ��� �Խù� �ε�
    public void LoadPostsByCategory(long categoryId)
    {
        StartCoroutine(postAPI.GetPostsByCategory(
            categoryId,
            onSuccess: (response) =>
            {
                Debug.Log("Response received from GetPostsByCategory: " + response);
                posts = ParsePostsResponse(response); // ���� ���� �����͸� �Խù� ����Ʈ�� �߰�
                currentPage = 1;  // ������ �ʱ�ȭ
                isSearchMode = false;  // �˻� ��� ����
                UpdateUI();  // UI ����
            },
            onError: (error) =>
            {
                Debug.LogError("Failed to fetch posts by category: " + error);
            }));
    }

    public void OnCreatePostButtonClicked()
    {
        string title = titleInputField.text;
        string contents = contentInputField.text;
        
        // �׽�Ʈ�� �г���
        //string nickname = "����";

        // ������ ������ ��� �̱��� ������ ����� �÷��̾� �г������� ����
        string nickname = UserData.Instance.NickName;

        // ���� �� �̸��� ���� ī�װ� �̸� ����
        string currentSceneName = SceneManager.GetActiveScene().name;
        string categoryName = GetCategoryNameForScene(currentSceneName);

        if (string.IsNullOrEmpty(categoryName))
        {
            Debug.LogError($"Unknown scene: {currentSceneName}. Cannot determine category.");
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

                // ī�װ� �� �Խù� �ҷ�����
                LoadPostsByCategory(categoryId);

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

        // ���� �� �̸��� ���� ī�װ� ID ��������
        string currentSceneName = SceneManager.GetActiveScene().name;
        long currentCategoryId = GetCategoryIdForScene(currentSceneName);

        if (string.IsNullOrEmpty(keyword)) // �˻�� ��������� ��ü �Խù� ��ȸ
        {
            Debug.Log("Search keyword is empty. Loading all posts...");
            ExitSearchMode(); // �˻� ��� �����ϰ� ��ü �Խù� ��ȸ
            return;
        }

        StartCoroutine(postAPI.SearchPosts(keyword,
            onSuccess: (response) =>
            {
                // JSON ���� �����͸� �Ľ�
                var allSearchResults = ParsePostsResponse(response);

                // Ŭ���̾�Ʈ���� category_id�� ���͸�
                searchResults = allSearchResults
                    .Where(post => post.category_id == currentCategoryId)
                    .ToList();

                //searchResults = ParsePostsResponse(response);
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
            postObject.transform.Find("Title").GetComponent<Text>().text = post.title;

            var button = postObject.GetComponent<Button>();
            button.onClick.AddListener(() => ShowDetailPanel(post));

            displayIndex--; // ���� ��ȣ�� ����
        }

        // ������ ���� ������Ʈ
        pageText.text = $"Page {currentPage} / {GetTotalPages()}";
        prevPageButton.interactable = currentPage > 1;
        nextPageButton.interactable = currentPage < GetTotalPages();
    }

    private void ShowDetailPanel(PostDataResponse post)
    {
        detailTitle.text = post.title;
        tmpdetailContent.text = post.contents;
        nicknames.text = "�ۼ���: " + post.nickname;
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
        Debug.Log("Parsing response: " + json);  // JSON ���� ���
        var container = JsonUtility.FromJson<PostListContainerResponse>($"{{\"posts\":{json}}}");
        return container.posts;
    }
}
