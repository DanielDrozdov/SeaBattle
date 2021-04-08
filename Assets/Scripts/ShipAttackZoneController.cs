using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShipAttackZoneController : MonoBehaviour {
    [SerializeField] private FightFieldStateController fightFieldStateController;
    [SerializeField] private Transform zoneMoveKeyPoint;
    [SerializeField] private Vector2 fieldScale;
    private Camera mainCamera;

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
        Vector2 dragPos;
        if(Input.touchCount > 0) {
            dragPos = Input.GetTouch(0).position;
        } else {
            dragPos = Input.mousePosition;
        }
        dragPos = mainCamera.ScreenToWorldPoint(dragPos);
        dragPos.x = Mathf.Clamp(dragPos.x, fieldBorders[0] + fieldScale.x / 2, fieldBorders[1] - fieldScale.x / 2);
        dragPos.y = Mathf.Clamp(dragPos.y, fieldBorders[2] + fieldScale.y / 2, fieldBorders[3] - fieldScale.y / 2);

        if(Mathf.Abs(dragPos.x - lastMovePointPos.x) > 0.55 || Mathf.Abs(dragPos.y - lastMovePointPos.y) > 0.55) {
            Vector2 pos = fightFieldStateController.GetNearestCellPos(dragPos - (Vector2)keyPointOffSet);
            lastMovePointPos = transform.position;
            transform.position = new Vector3(pos.x, pos.y, transform.position.z) + keyPointOffSet;
        }
    }

    private void ResetPosToA1Coordinates() {

    }
}
