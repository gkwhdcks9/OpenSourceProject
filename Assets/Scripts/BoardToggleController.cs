using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardToggleController : MonoBehaviour
{
    // Scroll View 오브젝트
    public GameObject scrollView;

    // 버튼 오브젝트
    public GameObject boardActivateBtn;
    public GameObject boardDeactivateBtn;

    // BoardActivateBtn을 눌렀을 때 호출
    public void ActivateBoard()
    {
        if (scrollView != null && boardActivateBtn != null && boardDeactivateBtn != null)
        {
            scrollView.SetActive(true); // Scroll View 활성화
            boardActivateBtn.SetActive(false); // Activate 버튼 비활성화
            boardDeactivateBtn.SetActive(true); // Deactivate 버튼 활성화
        }
    }

    // BoardDeactivateBtn을 눌렀을 때 호출
    public void DeactivateBoard()
    {
        if (scrollView != null && boardActivateBtn != null && boardDeactivateBtn != null)
        {
            scrollView.SetActive(false); // Scroll View 비활성화
            boardActivateBtn.SetActive(true); // Activate 버튼 활성화
            boardDeactivateBtn.SetActive(false); // Deactivate 버튼 비활성화
        }
    }
}
