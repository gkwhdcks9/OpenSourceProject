using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public VariableJoystick joystick; // 조이스틱 입력
    public float moveSpeed = 5f; // 캐릭터 이동 속도

    [Header("Animator Settings")]
    public Animator animator; // Animator 컴포넌트 참조

    private Rigidbody rb; // Rigidbody 컴포넌트 참조
    private Vector3 movement; // 이동 방향 벡터

    void Start()
    {
        // Rigidbody 컴포넌트를 가져옵니다.
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("PlayerMovement 스크립트가 부착된 GameObject에 Rigidbody 컴포넌트가 없습니다.");
        }

        // Animator 컴포넌트를 가져옵니다.
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogWarning("PlayerMovement 스크립트가 부착된 GameObject에 Animator 컴포넌트가 없습니다.");
            }
        }

        // 조이스틱이 할당되지 않았을 경우 경고 메시지 출력
        if (joystick == null)
        {
            Debug.LogError("Joystick이 할당되지 않았습니다. Inspector에서 VariableJoystick을 할당하세요.");
        }
    }

    void Update()
    {
        // 조이스틱 입력 처리
        float horizontal = joystick.Horizontal; // 좌우 입력: -1 (왼쪽), 1 (오른쪽)
        float vertical = joystick.Vertical;     // 상하 입력: -1 (뒤), 1 (앞)

        // 이동 벡터 설정 (3D 환경에서 x와 z 축 사용)
        movement = new Vector3(horizontal, 0, vertical).normalized;

        // 애니메이션 파라미터 업데이트
        UpdateAnimation(horizontal, vertical);
    }

    void FixedUpdate()
    {
        // Rigidbody를 사용한 이동
        if (rb != null)
        {
            Vector3 newPosition = rb.position + movement * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);
            // Debug.Log($"New Position: {newPosition}");
        }
    }

    void UpdateAnimation(float horizontal, float vertical)
    {
        if (animator != null)
        {
            // Animator 파라미터 설정
            animator.SetFloat("MoveX", horizontal);
            animator.SetFloat("MoveZ", vertical);
            animator.SetFloat("Speed", movement.magnitude);

            // 이동 방향에 따라 회전 (선택 사항)
            if (movement != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 720 * Time.deltaTime);
            }

            // 디버깅 로그 추가
            // Debug.Log($"Animator Parameters - MoveX: {horizontal}, MoveZ: {vertical}, Speed: {movement.magnitude}");
        }
    }
}
