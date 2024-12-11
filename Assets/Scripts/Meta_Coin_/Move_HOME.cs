using UnityEngine;
using UnityEngine.SceneManagement;

public class Move_HOME : MonoBehaviour
{
    // ��ư Ŭ�� �� ȣ��Ǵ� �Լ�
    public void OnButtonClick()
    {
        // "Interactable" �±׸� ���� ���� ������Ʈ ã��
        GameObject[] interactableObjects = GameObject.FindGameObjectsWithTag("Interactable");

        // �� ���� ������Ʈ�� ���� ó��
        foreach (GameObject obj in interactableObjects)
        {
            // ���� ������Ʈ���� �ݶ��̴��� �����ϴ��� Ȯ��
            Collider col = obj.GetComponent<Collider>();
            if (col != null)
            {
                // �ش� ������ �ε� (�� �̸��� ���� ������Ʈ�� �̸����� ����)
                string sceneName = obj.name;
                SceneManager.LoadScene(sceneName);
                return; // ù ��°�� ã�� ���� �ε��ϰ� ����
            }
        }

        // ���� Interactable �±װ� �ִ� ��ü�� ���ٸ� ����ó�� (�ʿ信 ���� �߰�)
        Debug.Log("Interactable �±׸� ���� ��ȿ�� ��ü�� �����ϴ�.");
    }
}
