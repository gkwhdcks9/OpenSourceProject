using UnityEngine;
using TMPro; // TextMeshPro를 사용하기 위해 추가
using UnityEngine.UI; // Button을 사용하기 위해 추가
using UnityEngine.SceneManagement; // SceneManager 네임스페이스 추가

public class Move : MonoBehaviour
{
    // 이동 관련 변수
    public VariableJoystick joy; // Joystick 입력
    public float speed = 5f; // 이동 속도

    // 상호작용 버튼 관련 변수
    public GameObject interactButton; // 상호작용 버튼 오브젝트
    public TextMeshProUGUI interactButtonText; // 상호작용 버튼 텍스트

    // 포탈 버튼 관련 변수
    public GameObject commonButton; // 포탈용 공용 버튼 오브젝트
    public TextMeshProUGUI commonButtonText; // 포탈용 버튼 텍스트

    // Archive와 Board의 부모 객체들
    [Header("Archive Parent Objects")]
    public GameObject[] archiveParentObjects; // Archive의 4개 부모 객체
    public GameObject[] archiveCanvases;     // Archive의 4개 자식 Canvas

    [Header("Board Parent Objects")]
    public GameObject[] boardParentObjects; // Board의 4개 부모 객체
    public GameObject[] boardCanvases;      // Board의 4개 자식 Canvas

    [SerializeField] private Animator animator; // Inspector에서 할당할 Animator (옵션)

    private Rigidbody rigid; // Rigidbody 컴포넌트
    private Vector3 moveVec; // 이동 벡터

    private int currentCanvasIndex = -1; // 현재 활성화할 캔버스 인덱스
    private CanvasType currentCanvasType = CanvasType.None; // 현재 활성화할 캔버스 종류

    // 포탈 관련 변수
    private string portalName; // 현재 포탈의 이름을 저장할 변수

    [SerializeField] private TextMeshProUGUI uCantText; // U_Cant 텍스트 UI


    public enum CanvasType
    {
        None,
        Archive,
        Board
    }

    void Awake()
    {
        // Rigidbody 컴포넌트 가져오기
        rigid = GetComponent<Rigidbody>();

        if (rigid == null)
        {
            Debug.LogError("Move 스크립트가 부착된 GameObject에 Rigidbody 컴포넌트가 없습니다.");
        }

        // Animator 컴포넌트를 가져옵니다. (옵션)
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogWarning("Move 스크립트가 부착된 GameObject에 Animator 컴포넌트가 없습니다.");
            }
        }

        // 시작 시 버튼을 비활성화
        if (interactButton != null)
        {
            interactButton.SetActive(false);
        }

        if (commonButton != null)
        {
            commonButton.SetActive(false);
        }

        // 모든 Canvas를 비활성화 (기본 상태에서)
        DeactivateAllCanvases();

        // 상호작용 버튼의 OnClick 이벤트에 ActivateCurrentCanvas 메서드 연결
        if (interactButton != null)
        {
            Button button = interactButton.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners(); // 기존 리스너 제거
                button.onClick.AddListener(ActivateCurrentCanvas); // 새로운 리스너 추가
            }
        }

        // 포탈 버튼의 OnClick 이벤트 초기화 (포탈 진입 시 동적으로 리스너 추가)
        if (commonButton != null)
        {
            Button button = commonButton.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners(); // 기존 리스너 제거
                // 포탈 진입 시 동적으로 리스너 추가
            }
        }
    }

    void Update()
    {
        // Joystick 입력 처리
        float x = joy.Horizontal; // 좌우 입력: -1 (왼쪽), 1 (오른쪽)
        float y = joy.Vertical;   // 상하 입력: -1 (아래), 1 (위)

        // 이동 벡터 계산
        if (Mathf.Abs(y) > Mathf.Abs(x))
        {
            // y 값이 더 큰 경우 상하 이동만 허용
            moveVec = new Vector3(0f, y, 0f);
        }
        else
        {
            // x 값이 더 큰 경우 좌우 이동만 허용
            moveVec = new Vector3(x, 0f, 0f);
        }

        // 애니메이션 파라미터 업데이트 (옵션)
        UpdateAnimation(moveVec.x, moveVec.y);
    }


    void FixedUpdate()
    {
        // Joystick 입력에 따른 이동
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

            // InteractableObject 컴포넌트에서 캔버스 정보 가져오기
            InteractableObject interactable = other.GetComponent<InteractableObject>();
            if (interactable != null && interactable.canvasType != CanvasType.None)
            {
                currentCanvasType = interactable.canvasType;
                currentCanvasIndex = interactable.canvasIndex;
                Debug.Log($"Current Canvas Type: {currentCanvasType}, Index: {currentCanvasIndex}");
                interactButton.SetActive(true); // 버튼 활성화
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

            // 현재 오브젝트의 이름을 포탈 이름으로 사용
            portalName = other.transform.name; // portalName 변수에 현재 포탈 이름 저장
            if (commonButtonText != null)
            {
                commonButtonText.text = $"Go to {portalName} Scene"; // 버튼 텍스트 설정
            }
            commonButton.SetActive(true); // 버튼 활성화

            // 버튼의 OnClick 이벤트에 EnterPortal 메서드 동적으로 연결
            Button button = commonButton.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners(); // 기존 리스너 제거
                button.onClick.AddListener(() => EnterPortal(portalName)); // 현재 포탈에 맞는 메서드 추가
            }
        }
        else if (other.CompareTag("Main") && commonButton != null)
        {
            // Main 태그와 Player 태그가 충돌할 때 commonButton 활성화
            if (commonButton != null)
            {
                portalName = other.transform.name; // portalName 변수에 현재 포탈 이름 저장
                if (commonButtonText != null)
                {
                    commonButtonText.text = $"Go to {portalName} Scene"; // 버튼 텍스트 설정
                }
                commonButton.SetActive(true); // 버튼 활성화

                // 버튼의 OnClick 이벤트에 EnterPortal 메서드 동적으로 연결
                Button button = commonButton.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.RemoveAllListeners(); // 기존 리스너 제거
                    button.onClick.AddListener(() => EnterPortal(portalName)); // 현재 포탈에 맞는 메서드 추가
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
            portalName = null; // 포탈 이름 초기화

            // 버튼의 OnClick 이벤트 제거
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
            portalName = null; // 포탈 이름 초기화

            // 버튼의 OnClick 이벤트 제거
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
            interactButton.SetActive(false); // 버튼 비활성화 (필요 시)
        }
        else
        {
            Debug.LogWarning("No valid canvas information to activate.");
        }
    }

    // 모든 Canvas를 비활성화하는 함수
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

    // 애니메이션 파라미터를 업데이트하는 함수 (옵션)
    private void UpdateAnimation(float horizontal, float vertical)
    {
        if (animator != null)
        {
            // Animator 파라미터 설정
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

            // Chatbot 포탈에 대한 코인 체크
            if (portalName == "Chatbot")
            {
                if (GameManager.Instance.HasEnoughCoins(1)) // 코인 1개 이상 확인
                {
                    GameManager.Instance.UseCoin(1); // 코인 1개 소비
                    SceneManager.LoadScene(SceneName); // ChatbotScene 로드
                    Debug.Log($"Loading Scene: {SceneName}, Remaining Coins: {GameManager.Instance.coinCount}");
                }
                else
                {
                    // 코인이 부족한 경우 U_Cant 텍스트 표시
                    ShowCantEnterMessage("코인 부족!\nO/X에서 얻어보세요!");
                }
            }
            else if (portalName == "Main")
            {
                // Main 포탈 처리
                SceneManager.LoadScene(SceneName);
                Debug.Log($"Loading Scene: {SceneName}");
            }
            else
            {
                // 일반 포탈은 코인 체크 없이 이동
                SceneManager.LoadScene(SceneName);
                Debug.Log("Loading Scene: " + SceneName);
            }
        }
    }

    private void ShowCantEnterMessage(string message)
    {
        if (uCantText != null)
        {
            uCantText.text = message; // 텍스트 설정
            uCantText.gameObject.SetActive(true); // 텍스트 활성화
            CancelInvoke(nameof(HideCantEnterMessage)); // 중복 호출 방지
            Invoke(nameof(HideCantEnterMessage), 3f); // 3초 후 텍스트 숨기기
        }
        else
        {
            Debug.LogWarning("U_Cant 텍스트가 설정되지 않았습니다!");
        }
    }

    private void HideCantEnterMessage()
    {
        if (uCantText != null)
        {
            uCantText.gameObject.SetActive(false); // 텍스트 비활성화
        }
    }
}
