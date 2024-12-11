using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public Move.CanvasType canvasType = Move.CanvasType.None; // 캔버스 종류 (Archive 또는 Board)
    public int canvasIndex; // 활성화할 캔버스의 인덱스 (각 그룹 내에서 0~3)
}
