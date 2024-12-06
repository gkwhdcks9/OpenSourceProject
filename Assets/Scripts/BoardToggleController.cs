using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardToggleController : MonoBehaviour
{
    // ��ũ�Ѻ� ������Ʈ
    public GameObject scrollView;

    // ��ư ������Ʈ
    public GameObject boardActivateBtn;
    public GameObject boardDeactivateBtn;
    public GameObject helpButton;
    public GameObject domainPanel;

    // BoardActivateBtn�� ������ �� ȣ��
    public void ActivateBoard()
    {
        if (scrollView != null && boardActivateBtn != null && boardDeactivateBtn != null)
        {
            scrollView.SetActive(true);
            helpButton.SetActive(true);
            domainPanel.SetActive(true);

            boardActivateBtn.SetActive(false); // Activate ��ư ��Ȱ��ȭ
            boardDeactivateBtn.SetActive(true); // Deactivate ��ư Ȱ��ȭ
        }
    }

    // BoardDeactivateBtn�� ������ �� ȣ��
    public void DeactivateBoard()
    {
        if (scrollView != null && boardActivateBtn != null && boardDeactivateBtn != null)
        {
            scrollView.SetActive(false); // Scroll View ��Ȱ��ȭ
            helpButton.SetActive(false);
            domainPanel.SetActive(false);

            boardActivateBtn.SetActive(true); // Activate ��ư Ȱ��ȭ
            boardDeactivateBtn.SetActive(false); // Deactivate ��ư ��Ȱ��ȭ
        }
    }
}
