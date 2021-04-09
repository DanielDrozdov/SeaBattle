using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShipAttackZoneController : MonoBehaviour {
    [SerializeField] private FightFieldStateController fightFieldStateController;
    [SerializeField] private Transform zoneMoveKeyPoint;
    [SerializeField] private Vector2 cellsCount;
    private Camera mainCamera;

    private Vector2 dragPosition;
    private Vector2 lastMovePointPos;
    private Vector3 keyPointOffSet;
    private float[] fieldBorders = { };

    private void Start() {
        mainCamera = ServiceManager.GetInstance().GetMainCamera();
        lastMovePointPos = zoneMoveKeyPoint.position;
        keyPointOffSet = zoneMoveKeyPoint.position - transform.position;
    }

    private void OnMouseDown() {
        if(fieldBorders.Length == 0) {
            fieldBorders = fightFieldStateController.GetFieldBorders();
        }
        lastMovePointPos = transform.position;
    }

    private void OnMouseDrag() {
        if(Input.touchCount > 0) {
            dragPosition = Input.GetTouch(0).position;
        } else {
            dragPosition = Input.mousePosition;
        }
        dragPosition = mainCamera.ScreenToWorldPoint(dragPosition);
        dragPosition.x = Mathf.Clamp(dragPosition.x, fieldBorders[0] + cellsCount.x / 2, fieldBorders[1] - cellsCount.x / 2);
        dragPosition.y = Mathf.Clamp(dragPosition.y, fieldBorders[2] + cellsCount.y / 2, fieldBorders[3] - cellsCount.y / 2);

        if(Mathf.Abs(dragPosition.x - lastMovePointPos.x) > 0.55 || Mathf.Abs(dragPosition.y - lastMovePointPos.y) > 0.55) {
            SetZonePostionOnNearestCell(dragPosition);
        }
    }

    private void OnMouseUp() {
        SetZonePostionOnNearestCell(dragPosition);
        Vector2[] attackPositions = GetEnemyAttackCells();
        fightFieldStateController.HitByShipAttackZone(attackPositions);
    }

    private Vector2[] GetEnemyAttackCells() {
        Dictionary<int, Vector2> attackCells = new Dictionary<int, Vector2>();
        int choosesCellsCount = (int)ServiceManager.GetInstance().GetLastActivatedShipCellsCount();
        Vector2[] hitCells = new Vector2[choosesCellsCount];
        Vector2 keyPos = zoneMoveKeyPoint.position;
        int curSortCellNumber = 0;
        for(int i = 0; i < cellsCount.y; i++) {
            for(int k = 0; k < cellsCount.x; k++) {
                attackCells.Add(curSortCellNumber++, new Vector2(keyPos.x + k, keyPos.y - i));
            }
        }
        for(int i = 0; i < choosesCellsCount; i++) {
            hitCells[i] = RecursiveFindRandomAttackCell(attackCells);
        }

        return hitCells;
    }

    private void SetZonePostionOnNearestCell(Vector2 tapDragPosition) {
        Vector2 pos = fightFieldStateController.GetNearestCellPos(tapDragPosition - (Vector2)keyPointOffSet);
        transform.position = new Vector3(pos.x, pos.y, transform.position.z) + keyPointOffSet;
        lastMovePointPos = transform.position;
    }

    private Vector2 RecursiveFindRandomAttackCell(Dictionary<int, Vector2> attackCells) {
        int rndAttackCellNumber = Random.Range(0, attackCells.Count);
        Vector2 cellPos;
        if(attackCells.ContainsKey(rndAttackCellNumber)) {
            cellPos = attackCells[rndAttackCellNumber];
            attackCells.Remove(rndAttackCellNumber);
        } else {
            cellPos = RecursiveFindRandomAttackCell(attackCells);
        }
        return cellPos;
    }
}
