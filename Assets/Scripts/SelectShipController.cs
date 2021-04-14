using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectShipController : MonoBehaviour
{
    [HideInInspector] public Vector2[] shipPoints;

    [SerializeField] private Transform zoneMoveKeyPoint;
    [SerializeField] private Vector2 cellsCount;
    private ShipSelectFieldController selectShipField;
    private SelectedShipAreaController shipArea;
    private Camera mainCamera;
   
    private SpriteRenderer spriteRenderer;
    private int startOrderInLayerValue;

    private bool IsShipFlippedOnY;
    private bool IsShipInGameField;
    private Vector2 shipLastPosOnField;
    private Vector2 startPos;
    private Vector2 dragPosition;
    private Vector2 lastMovePointPos;
    private Vector3 keyPointOffSet;
    private float[] fieldBorders = { };
    private int shipSizeInCells;

    private float borderOffset = 0.45f; // dont increese more than 0.5(exclusive)
    private float minXPos;
    private float maxXPos;
    private float minYPos;
    private float maxYPos;

    private void Start() {
        mainCamera = ServiceManager.GetInstance().GetMainCamera();
        selectShipField = ShipSelectFieldController.GetInstance();
        spriteRenderer = GetComponent<SpriteRenderer>();
        startOrderInLayerValue = spriteRenderer.sortingOrder;
        shipSizeInCells = (int)cellsCount.x;
        shipArea = selectShipField.GetShipArea(shipSizeInCells);
        startPos = transform.position;
        keyPointOffSet = zoneMoveKeyPoint.position - transform.position;
    }

    private void OnMouseDown() {
        if(fieldBorders.Length == 0) {
            fieldBorders = selectShipField.GetFieldBorders();
        }
        if(IsShipFlippedOnY) {
            shipArea.transform.rotation = Quaternion.Euler(0, 0, 90);
        } else {
            shipArea.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        shipArea.transform.position = new Vector3(transform.position.x, transform.position.y, shipArea.transform.position.z);
        CalculateShipFieldLocateBorders();
        spriteRenderer.sortingOrder = 5;
        lastMovePointPos = transform.position;
    }

    private void OnMouseDrag() {
        if(Input.touchCount > 0) {
            dragPosition = Input.GetTouch(0).position;
        } else {
            dragPosition = Input.mousePosition;
        }
        dragPosition = mainCamera.ScreenToWorldPoint(dragPosition);
        transform.position = new Vector3(dragPosition.x, dragPosition.y, transform.position.z);
        IsShipInFieldZoneInDragTime();
    }

    private void OnMouseUp() {
        if(IsShipInGameField) {
            selectShipField.ChangePressedShip(shipSizeInCells, this);
            SetZonePostionOnNearestCell(dragPosition, gameObject);
            SetShipPointsInCells();
            if(selectShipField.CheckIfShipCanLocate(shipPoints, this)) {
                AddShipToFieldBase();
            } else {
                if(shipLastPosOnField != Vector2.zero) {
                    transform.position = shipLastPosOnField;
                } else {
                    ResetShipState();
                }
            }
        } else {
            ResetShipState();
        }
        spriteRenderer.sortingOrder = startOrderInLayerValue;
        shipArea.DeactivateArea();
    }

    public void RotateShip() {
        float xCells = cellsCount.y;
        float yCells = cellsCount.x;
        cellsCount = new Vector2(xCells, yCells);
        if(!IsShipFlippedOnY) {
            transform.Rotate(new Vector3(0, 0, 90));
            transform.position = new Vector3(transform.position.x + keyPointOffSet.x,
                transform.position.y - keyPointOffSet.x,transform.position.z);
        } else {
            transform.Rotate(new Vector3(0, 0, -90));
            transform.position = new Vector3(transform.position.x - keyPointOffSet.y,
                transform.position.y + keyPointOffSet.y, transform.position.z);
        }
        IsShipFlippedOnY = !IsShipFlippedOnY;
        keyPointOffSet = zoneMoveKeyPoint.position - transform.position;
        CalculateShipFieldLocateBorders();
        SetShipPointsInCells();
        if(!CheckIsPointInFieldZone(transform.position) || !selectShipField.CheckIfShipCanLocate(shipPoints,this)) {
            RotateShip();
            return;
        }
        if(IsShipInGameField) {
            SetZonePostionOnNearestCell(transform.position, shipArea.gameObject);
            SetZonePostionOnNearestCell(transform.position, gameObject);
            lastMovePointPos = shipArea.transform.position;
            AddShipToFieldBase();
        }
    }

    private void IsShipInFieldZoneInDragTime() {
        if(CheckIsPointInFieldZone(dragPosition)) {
            IsShipInGameField = true;
            shipArea.ActivateArea();
            SetShipPointsInCells();
            ChangeAreaColorIfCantLocateShip();
            if(Mathf.Abs(dragPosition.x - lastMovePointPos.x) > 0.45 || Mathf.Abs(dragPosition.y - lastMovePointPos.y) > 0.45) {
                SetZonePostionOnNearestCell(dragPosition, shipArea.gameObject);
                lastMovePointPos = shipArea.transform.position;
            }
        } else {
            IsShipInGameField = false;
            shipArea.DeactivateArea();
        }
    }

    private void AddShipToFieldBase() {
        SetShipPointsInCells();
        selectShipField.ResetSelectedShipReservedPoints(this);
        selectShipField.AddShipToFieldBase(shipPoints, this);
        shipLastPosOnField = transform.position;
    }

    private bool CheckIsPointInFieldZone(Vector2 position) {
        if(position.x > minXPos && position.x < maxXPos &&
            position.y > minYPos && position.y < maxYPos) {
            return true;
        }
        return false;
    }

    private void SetZonePostionOnNearestCell(Vector2 tapDragPosition,GameObject gameObject) {
        Vector2 pos = selectShipField.GetNearestCellPos(tapDragPosition + (Vector2)keyPointOffSet);
        gameObject.transform.position = new Vector3(pos.x, pos.y, transform.position.z) - keyPointOffSet;
    }

    private void SetShipPointsInCells() {
        Vector2[] shipPoints = new Vector2[shipSizeInCells];
        if(IsShipFlippedOnY) {
            for(int i = 0; i < shipSizeInCells; i++) {
                shipPoints[i] = new Vector2(transform.position.x, zoneMoveKeyPoint.position.y + i);
            }
        } else {
            for(int i = 0; i < shipSizeInCells; i++) {
                shipPoints[i] = new Vector2(zoneMoveKeyPoint.position.x + i,transform.position.y);
            }
        }
        this.shipPoints = shipPoints;
    }

    private void CalculateShipFieldLocateBorders() {
        float halfShipDeltaX = cellsCount.x / 2;
        float halfShipDeltaY = cellsCount.y / 2;
        minXPos = fieldBorders[0] + halfShipDeltaX - borderOffset;
        maxXPos = fieldBorders[1] - halfShipDeltaX + borderOffset;
        minYPos = fieldBorders[2] + halfShipDeltaY - borderOffset;
        maxYPos = fieldBorders[3] - halfShipDeltaY + borderOffset;
    }

    private void ResetShipState() {
        if(IsShipInGameField && shipLastPosOnField != Vector2.zero) {
            transform.position = shipLastPosOnField;
        } else {
            transform.position = new Vector3(startPos.x, startPos.y, transform.position.z);
            selectShipField.ResetSelectedShipReservedPoints(this);
        }
        if(IsShipFlippedOnY) {
            RotateShip();
        }
    }

    private void ChangeAreaColorIfCantLocateShip() {
        if(selectShipField.CheckIfShipCanLocate(shipPoints, this)) {
            shipArea.ActivateGreenState();
        } else {
            shipArea.ActivateRedState();
        }
    }
}
