using UnityEngine;
using TMPro; // TextMeshPro�� ����ϱ� ���� �߰�
using UnityEngine.UI; // Button�� ����ϱ� ���� �߰�
using UnityEngine.SceneManagement; // SceneManager ���ӽ����̽� �߰�

public class Move : MonoBehaviour
{
    // �̵� ���� ����
    public VariableJoystick joy; // Joystick �Է�
    public float speed = 5f; // �̵� �ӵ�

    // ��ȣ�ۿ� ��ư ���� ����
    public GameObject interactButton; // ��ȣ�ۿ� ��ư ������Ʈ
    public TextMeshProUGUI interactButtonText; // ��ȣ�ۿ� ��ư �ؽ�Ʈ

    // ��Ż ��ư ���� ����
    public GameObject commonButton; // ��Ż�� ���� ��ư ������Ʈ
    public TextMeshProUGUI commonButtonText; // ��Ż�� ��ư �ؽ�Ʈ

    // Archive�� Board�� �θ� ��ü��
    [Header("Archive Parent Objects")]
    public GameObject[] archiveParentObjects; // Archive�� 4�� �θ� ��ü
    public GameObject[] archiveCanvases;     // Archive�� 4�� �ڽ� Canvas

    [Header("Board Parent Objects")]
    public GameObject[] boardParentObjects; // Board�� 4�� �θ� ��ü
    public GameObject[] boardCanvases;      // Board�� 4�� �ڽ� Canvas

    [SerializeField] private Animator animator; // Inspector���� �Ҵ��� Animator (�ɼ�)

    private Rigidbody rigid; // Rigidbody ������Ʈ
    private Vector3 moveVec; // �̵� ����

    private int currentCanvasIndex = -1; // ���� Ȱ��ȭ�� ĵ���� �ε���
    private CanvasType currentCanvasType = CanvasType.None; // ���� Ȱ��ȭ�� ĵ���� ����

    // ��Ż ���� ����
    private string portalName; // ���� ��Ż�� �̸��� ������ ����

    [SerializeField] private TextMeshProUGUI uCantText; // U_Cant �ؽ�Ʈ UI


    public enum CanvasType
    {
        None,
        Archive,
        Board
    }

    void Awake()
    {
        // Rigidbody ������Ʈ ��������
        rigid = GetComponent<Rigidbody>();

        if (rigid == null)
        {
            Debug.LogError("Move ��ũ��Ʈ�� ������ GameObject�� Rigidbody ������Ʈ�� �����ϴ�.");
        }

        // Animator ������Ʈ�� �����ɴϴ�. (�ɼ�)
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogWarning("Move ��ũ��Ʈ�� ������ GameObject�� Animator ������Ʈ�� �����ϴ�.");
            }
        }

        // ���� �� ��ư�� ��Ȱ��ȭ
        if (interactButton != null)
        {
            interactButton.SetActive(false);
        }

        if (commonButton != null)
        {
            commonButton.SetActive(false);
        }

        // ��� Canvas�� ��Ȱ��ȭ (�⺻ ���¿���)
        DeactivateAllCanvases();

        // ��ȣ�ۿ� ��ư�� OnClick �̺�Ʈ�� ActivateCurrentCanvas �޼��� ����
        if (interactButton != null)
        {
            Button button = interactButton.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners(); // ���� ������ ����
                button.onClick.AddListener(ActivateCurrentCanvas); // ���ο� ������ �߰�
            }
        }

        // ��Ż ��ư�� OnClick �̺�Ʈ �ʱ�ȭ (��Ż ���� �� �������� ������ �߰�)
        if (commonButton != null)
        {
            Button button = commonButton.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners(); // ���� ������ ����
                // ��Ż ���� �� �������� ������ �߰�
            }
        }
    }

    void Update()
    {
        // Joystick �Է� ó��
        float x = joy.Horizontal; // �¿� �Է�: -1 (����), 1 (������)
        float y = joy.Vertical;   // ���� �Է�: -1 (�Ʒ�), 1 (��)

        // �̵� ���� ���
        if (Mathf.Abs(y) > Mathf.Abs(x))
        {
            // y ���� �� ū ��� ���� �̵��� ���
            moveVec = new Vector3(0f, y, 0f);
        }
        else
        {
            // x ���� �� ū ��� �¿� �̵��� ���
            moveVec = new Vector3(x, 0f, 0f);
        }

        // �ִϸ��̼� �Ķ���� ������Ʈ (�ɼ�)
        UpdateAnimation(moveVec.x, moveVec.y);
    }


    void FixedUpdate()
    {
        // Joystick �Է¿� ���� �̵�
        if (rigid != null)
        {
            rigid.MovePosition(rigid.position + moveVec * speed * Time.fixedDeltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable") && interactButton != null)
        {
            Debug.Log("Trigger entered with Interactable object");

            // InteractableObject ������Ʈ���� ĵ���� ���� ��������
            InteractableObject interactable = other.GetComponent<InteractableObject>();
            if (interactable != null && interactable.canvasType != CanvasType.None)
            {
                currentCanvasType = interactable.canvasType;
                currentCanvasIndex = interactable.canvasIndex;
                Debug.Log($"Current Canvas Type: {currentCanvasType}, Index: {currentCanvasIndex}");
                interactButton.SetActive(true); // ��ư Ȱ��ȭ
            }
            else
            {
                Debug.LogWarning("Interactable object has invalid canvas information.");
                currentCanvasType = CanvasType.None;
                currentCanvasIndex = -1;
                interactButton.SetActive(false);
            }
        }
        else if (other.CompareTag("Portal") && commonButton != null)
        {
            Debug.Log("Trigger entered with Portal object");

            // ���� ������Ʈ�� �̸��� ��Ż �̸����� ���
            portalName = other.transform.name; // portalName ������ ���� ��Ż �̸� ����
            if (commonButtonText != null)
            {
                commonButtonText.text = $"Go to {portalName} Scene"; // ��ư �ؽ�Ʈ ����
            }
            commonButton.SetActive(true); // ��ư Ȱ��ȭ

            // ��ư�� OnClick �̺�Ʈ�� EnterPortal �޼��� �������� ����
            Button button = commonButton.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners(); // ���� ������ ����
                button.onClick.AddListener(() => EnterPortal(portalName)); // ���� ��Ż�� �´� �޼��� �߰�
            }
        }
        else if (other.CompareTag("Main") && commonButton != null)
        {
            // Main �±׿� Player �±װ� �浹�� �� commonButton Ȱ��ȭ
            if (commonButton != null)
            {
                portalName = other.transform.name; // portalName ������ ���� ��Ż �̸� ����
                if (commonButtonText != null)
                {
                    commonButtonText.text = $"Go to {portalName} Scene"; // ��ư �ؽ�Ʈ ����
                }
                commonButton.SetActive(true); // ��ư Ȱ��ȭ

                // ��ư�� OnClick �̺�Ʈ�� EnterPortal �޼��� �������� ����
                Button button = commonButton.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.RemoveAllListeners(); // ���� ������ ����
                    button.onClick.AddListener(() => EnterPortal(portalName)); // ���� ��Ż�� �´� �޼��� �߰�
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactable") && interactButton != null)
        {
            Debug.Log("Trigger exited with Interactable object");
            interactButton.SetActive(false);
            currentCanvasType = CanvasType.None;
            currentCanvasIndex = -1;
        }
        else if (other.CompareTag("Portal") && commonButton != null)
        {
            Debug.Log("Trigger exited with Portal object");
            commonButton.SetActive(false);
            portalName = null; // ��Ż �̸� �ʱ�ȭ

            // ��ư�� OnClick �̺�Ʈ ����
            Button button = commonButton.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
            }
        }
        else if (other.CompareTag("Main") && commonButton != null)
        {
            Debug.Log("Trigger exited with Portal object");
            commonButton.SetActive(false);
            portalName = null; // ��Ż �̸� �ʱ�ȭ

            // ��ư�� OnClick �̺�Ʈ ����
            Button button = commonButton.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
            }
        }
    }

    public void ActivateCanvas(int canvasIndex, CanvasType canvasType)
    {
        Debug.Log($"ActivateCanvas called, Type: {canvasType}, Index: {canvasIndex}");

        switch (canvasType)
        {
            case CanvasType.Archive:
                if (canvasIndex < 0 || canvasIndex >= archiveCanvases.Length)
                {
                    Debug.LogError("Invalid Archive Canvas Index: " + canvasIndex);
                    return;
                }
                DeactivateAllCanvases();
                if (archiveParentObjects[canvasIndex] != null)
                {
                    archiveParentObjects[canvasIndex].SetActive(true);
                }
                if (archiveCanvases[canvasIndex] != null)
                {
                    archiveCanvases[canvasIndex].SetActive(true);
                }
                break;

            case CanvasType.Board:
                if (canvasIndex < 0 || canvasIndex >= boardCanvases.Length)
                {
                    Debug.LogError("Invalid Board Canvas Index: " + canvasIndex);
                    return;
                }
                DeactivateAllCanvases();
                if (boardParentObjects[canvasIndex] != null)
                {
                    boardParentObjects[canvasIndex].SetActive(true);
                }
                if (boardCanvases[canvasIndex] != null)
                {
                    boardCanvases[canvasIndex].SetActive(true);
                }
                break;

            default:
                Debug.LogError("Unknown Canvas Type");
                break;
        }
    }

    public void ActivateCurrentCanvas()
    {
        if (currentCanvasType != CanvasType.None && currentCanvasIndex != -1)
        {
            ActivateCanvas(currentCanvasIndex, currentCanvasType);
            interactButton.SetActive(false); // ��ư ��Ȱ��ȭ (�ʿ� ��)
        }
        else
        {
            Debug.LogWarning("No valid canvas information to activate.");
        }
    }

    // ��� Canvas�� ��Ȱ��ȭ�ϴ� �Լ�
    private void DeactivateAllCanvases()
    {
        foreach (var canvas in archiveCanvases)
        {
            if (canvas != null)
            {
                canvas.SetActive(false);
            }
        }
        Debug.Log("Deactivated all archive canvases");

        foreach (var canvas in boardCanvases)
        {
            if (canvas != null)
            {
                canvas.SetActive(false);
            }
        }
        Debug.Log("Deactivated all board canvases");
    }

    // �ִϸ��̼� �Ķ���͸� ������Ʈ�ϴ� �Լ� (�ɼ�)
    private void UpdateAnimation(float horizontal, float vertical)
    {
        if (animator != null)
        {
            // Animator �Ķ���� ����
            animator.SetFloat("Horizontal", horizontal);
            animator.SetFloat("Vertical", vertical);
            animator.SetFloat("Speed", new Vector2(horizontal, vertical).sqrMagnitude);
        }
    }

    public void EnterPortal(string portalName)
    {
        if (!string.IsNullOrEmpty(portalName))
        {
            string SceneName = portalName + "Scene";

            // Chatbot ��Ż�� ���� ���� üũ
            if (portalName == "Chatbot")
            {
                if (GameManager.Instance.HasEnoughCoins(1)) // ���� 1�� �̻� Ȯ��
                {
                    GameManager.Instance.UseCoin(1); // ���� 1�� �Һ�
                    SceneManager.LoadScene(SceneName); // ChatbotScene �ε�
                    Debug.Log($"Loading Scene: {SceneName}, Remaining Coins: {GameManager.Instance.coinCount}");
                }
                else
                {
                    // ������ ������ ��� U_Cant �ؽ�Ʈ ǥ��
                    ShowCantEnterMessage("���� ����!\nO/X���� ������!");
                }
            }
            else if (portalName == "Main")
            {
                // Main ��Ż ó��
                SceneManager.LoadScene(SceneName);
                Debug.Log($"Loading Scene: {SceneName}");
            }
            else
            {
                // �Ϲ� ��Ż�� ���� üũ ���� �̵�
                SceneManager.LoadScene(SceneName);
                Debug.Log("Loading Scene: " + SceneName);
            }
        }
    }

    private void ShowCantEnterMessage(string message)
    {
        if (uCantText != null)
        {
            uCantText.text = message; // �ؽ�Ʈ ����
            uCantText.gameObject.SetActive(true); // �ؽ�Ʈ Ȱ��ȭ
            CancelInvoke(nameof(HideCantEnterMessage)); // �ߺ� ȣ�� ����
            Invoke(nameof(HideCantEnterMessage), 3f); // 3�� �� �ؽ�Ʈ �����
        }
        else
        {
            Debug.LogWarning("U_Cant �ؽ�Ʈ�� �������� �ʾҽ��ϴ�!");
        }
    }

    private void HideCantEnterMessage()
    {
        if (uCantText != null)
        {
            uCantText.gameObject.SetActive(false); // �ؽ�Ʈ ��Ȱ��ȭ
        }
    }
}
