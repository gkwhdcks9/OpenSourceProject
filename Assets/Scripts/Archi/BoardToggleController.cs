using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardToggleController : MonoBehaviour
{
    // 스크롤뷰 오브젝트
    public GameObject scrollView;

    // 버튼 오브젝트
    public GameObject boardDeactivateBtn;
    public GameObject helpButton;
    public GameObject domainPanel;

    public GameObject Canvas;

    // BoardDeactivateBtn을 눌렀을 때 호출
    public void DeactivateBoard()
    {
        //scrollView.SetActive(false); // Scroll View 비활성화
        //helpButton.SetActive(false);
        //domainPanel.SetActive(false);

        //boardDeactivateBtn.SetActive(false); // Deactivate 버튼 비활성화
        Canvas.SetActive(false);
    }
}
