using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardToggleController : MonoBehaviour
{
    // 스크롤뷰 오브젝트
    public GameObject scrollView;

    // 버튼 오브젝트
    public GameObject boardActivateBtn;
    public GameObject boardDeactivateBtn;
    public GameObject helpButton;
    public GameObject domainPanel;

    // BoardActivateBtn을 눌렀을 때 호출
    public void ActivateBoard()
    {
        if (scrollView != null && boardActivateBtn != null && boardDeactivateBtn != null)
        {
            scrollView.SetActive(true);
            helpButton.SetActive(true);
            domainPanel.SetActive(true);

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
            helpButton.SetActive(false);
            domainPanel.SetActive(false);

            boardActivateBtn.SetActive(true); // Activate 버튼 활성화
            boardDeactivateBtn.SetActive(false); // Deactivate 버튼 비활성화
        }
    }
}
