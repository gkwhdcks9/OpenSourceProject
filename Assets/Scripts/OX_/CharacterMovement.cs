using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharacterMovement : MonoBehaviour
{
    public float moveDistance = 40f; // 이동 거리
    public float yIncreaseRate = 1f; // y축 증가율
    private float currentY = 0f; // y축 현재 위치

    private Animator animator; // Animator 참조

    public Button rightButton; // 오른쪽 버튼
    public Button leftButton; // 왼쪽 버튼

    private bool canMove = false; // 이동 가능 여부 플래그
    private bool rightButtonPressed = false; // 오른쪽 버튼 중복 방지 플래그
    private bool leftButtonPressed = false; // 왼쪽 버튼 중복 방지 플래그

    public Transform cameraTransform; // 카메라 Transform 참조
    public Vector3 cameraOffset = new Vector3(0, 5, -10); // 카메라와 캐릭터 간의 상대 위치

    void Start()
    {
        currentY = transform.position.y; // 초기 y축 위치 설정
        animator = GetComponent<Animator>(); // Animator 컴포넌트 가져오기

        // 버튼 클릭 이벤트 연결
        rightButton.onClick.AddListener(() => HandleButtonPress("Right"));
        leftButton.onClick.AddListener(() => HandleButtonPress("Left"));

        // 5초 대기 후 이동 활성화
        StartCoroutine(EnableMovementAfterDelay(5f));
    }

    void Update()
    {
        if (canMove)
        {
            // Y축 증가
            currentY += yIncreaseRate * Time.deltaTime;
            transform.position = new Vector3(transform.position.x, currentY, transform.position.z);

            // Y축 제한
            if (currentY >= 23f)
            {
                canMove = false; // 이동 종료
                Debug.Log("캐릭터의 Y 위치가 25 이상이 되어 움직임이 종료되었습니다.");
            }
        }

        // 카메라 이동
        FollowCharacter();
    }

    void HandleButtonPress(string direction)
    {
        if (direction == "Right")
        {
            if (!rightButtonPressed)
            {
                // 트리거 제한은 걸리지만 이동은 항상 실행
                StartCoroutine(HandleButtonCooldown("Right"));
                animator.SetTrigger("LookRight"); // 애니메이션 실행
            }
            MoveRight(); // 이동은 제한 없이 가능
        }
        else if (direction == "Left")
        {
            if (!leftButtonPressed)
            {
                // 트리거 제한은 걸리지만 이동은 항상 실행
                StartCoroutine(HandleButtonCooldown("Left"));
                animator.SetTrigger("LookLeft"); // 애니메이션 실행
            }
            MoveLeft(); // 이동은 제한 없이 가능
        }
    }

    IEnumerator HandleButtonCooldown(string direction)
    {
        if (direction == "Right")
        {
            rightButtonPressed = true;
            yield return new WaitForSeconds(1f); // 1초 대기
            rightButtonPressed = false;
        }
        else if (direction == "Left")
        {
            leftButtonPressed = true;
            yield return new WaitForSeconds(1f); // 1초 대기
            leftButtonPressed = false;
        }
    }

    void MoveRight()
    {
        if (!canMove) return;

        // X축 오른쪽으로 이동
        transform.position = new Vector3(transform.position.x + moveDistance, currentY, transform.position.z);
    }

    void MoveLeft()
    {
        if (!canMove) return;

        // X축 왼쪽으로 이동
        transform.position = new Vector3(transform.position.x - moveDistance, currentY, transform.position.z);
    }

    IEnumerator EnableMovementAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canMove = true; // 이동 가능 설정
    }

    void FollowCharacter()
    {
        if (cameraTransform != null)
        {
            cameraTransform.position = new Vector3(
                cameraTransform.position.x, // X축은 고정
                transform.position.y + cameraOffset.y, // Y축은 캐릭터의 Y축 + 오프셋
                cameraTransform.position.z // Z축은 고정
            );
        }
    }
}
