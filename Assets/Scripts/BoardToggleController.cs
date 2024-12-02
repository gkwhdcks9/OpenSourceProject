using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardToggleController : MonoBehaviour
{
    // Scroll View ������Ʈ
    public GameObject scrollView;

    // ��ư ������Ʈ
    public GameObject boardActivateBtn;
    public GameObject boardDeactivateBtn;

    // BoardActivateBtn�� ������ �� ȣ��
    public void ActivateBoard()
    {
        if (scrollView != null && boardActivateBtn != null && boardDeactivateBtn != null)
        {
            scrollView.SetActive(true); // Scroll View Ȱ��ȭ
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
            boardActivateBtn.SetActive(true); // Activate ��ư Ȱ��ȭ
            boardDeactivateBtn.SetActive(false); // Deactivate ��ư ��Ȱ��ȭ
        }
    }
}
