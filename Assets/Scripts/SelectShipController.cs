using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectShipController : MonoBehaviour
{
    private FightFieldStateController fightFieldStateController;
    [SerializeField] private Transform zoneMoveKeyPoint;
    [SerializeField] private Vector2 cellsCount;
    private Camera mainCamera;
    private GameObject shipArea;

    private bool IsShipInGameField;
    private Vector2 startPos;
    private Vector2 dragPosition;
    private Vector2 lastMovePointPos;
    private Vector3 keyPointOffSet;
    private float[] fieldBorders = { };

    private float borderOffset = 0.4f; // dont increese more than 0.5(exclusive)
    private float halfShipDeltaY;
    private float halfShipDeltaX;
    private float maxYPos;
    private float minXPos;
    private float maxXPos;
    private float minYPos;

    private void Start() {
        mainCamera = ServiceManager.GetInstance().GetMainCamera();
        fightFieldStateController = SelectShipFieldController.GetInstance().GetGameFieldStateController();
        shipArea = SelectShipFieldController.GetInstance().GetShipArea((int)cellsCount.x);
        startPos = transform.position;
        keyPointOffSet = zoneMoveKeyPoint.position - transform.position;
    }

    private void OnMouseDown() {
        if(fieldBorders.Length == 0) {
            fieldBorders = SelectShipFieldController.GetInstance().GetFieldBorders();
        }
        lastMovePointPos = transform.position;
        CalculateShipLocateBorders();
    }

    private void OnMouseDrag() {
        if(Input.touchCount > 0) {
            dragPosition = Input.GetTouch(0).position;
        } else {
            dragPosition = Input.mousePosition;
        }
        dragPosition = mainCamera.ScreenToWorldPoint(dragPosition);
        transform.position = new Vector3(dragPosition.x, dragPosition.y, transform.position.z);
        IsShipInFieldZone();
    }

    private void OnMouseUp() {
        if(IsShipInGameField) {
            SetZonePostionOnNearestCell(dragPosition,gameObject);
        } else {
            transform.position = new Vector3(startPos.x, startPos.y, transform.position.z);
        }
        shipArea.SetActive(false);
    }

    private void IsShipInFieldZone() {
        if(dragPosition.x > minXPos && dragPosition.x < maxXPos &&
            dragPosition.y > minYPos && dragPosition.y < maxYPos) {
            IsShipInGameField = true;
            shipArea.SetActive(true);
            if(Mathf.Abs(dragPosition.x - lastMovePointPos.x) > 0.55 || Mathf.Abs(dragPosition.y - lastMovePointPos.y) > 0.55) {
                SetZonePostionOnNearestCell(dragPosition, shipArea);
                lastMovePointPos = shipArea.transform.position;
            }
        } else {
            IsShipInGameField = false;
            shipArea.SetActive(false);
        }
    }

    private void SetZonePostionOnNearestCell(Vector2 tapDragPosition,GameObject gameObject) {
        Vector2 pos = fightFieldStateController.GetNearestCellPos(tapDragPosition + (Vector2)keyPointOffSet);
        gameObject.transform.position = new Vector3(pos.x, pos.y, transform.position.z) - keyPointOffSet;
    }

    private void CalculateShipLocateBorders() {
        halfShipDeltaX = cellsCount.x / 2;
        halfShipDeltaY = cellsCount.y / 2;
        minXPos = fieldBorders[0] + halfShipDeltaX - borderOffset;
        maxXPos = fieldBorders[1] - halfShipDeltaX + borderOffset;
        minYPos = fieldBorders[2] + halfShipDeltaY - borderOffset;
        maxYPos = fieldBorders[3] - halfShipDeltaY + borderOffset;
    }
}
