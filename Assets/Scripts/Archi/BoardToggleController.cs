using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardToggleController : MonoBehaviour
{
    // ��ũ�Ѻ� ������Ʈ
    public GameObject scrollView;

    // ��ư ������Ʈ
    public GameObject boardDeactivateBtn;
    public GameObject helpButton;
    public GameObject domainPanel;

    public GameObject Canvas;

    // BoardDeactivateBtn�� ������ �� ȣ��
    public void DeactivateBoard()
    {
        //scrollView.SetActive(false); // Scroll View ��Ȱ��ȭ
        //helpButton.SetActive(false);
        //domainPanel.SetActive(false);

        //boardDeactivateBtn.SetActive(false); // Deactivate ��ư ��Ȱ��ȭ
        Canvas.SetActive(false);
    }
}
