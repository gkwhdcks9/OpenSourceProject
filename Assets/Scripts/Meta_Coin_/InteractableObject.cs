using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public Move.CanvasType canvasType = Move.CanvasType.None; // ĵ���� ���� (Archive �Ǵ� Board)
    public int canvasIndex; // Ȱ��ȭ�� ĵ������ �ε��� (�� �׷� ������ 0~3)
}
