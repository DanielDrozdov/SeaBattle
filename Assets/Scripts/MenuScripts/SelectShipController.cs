using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectShipController : MonoBehaviour
{
    [HideInInspector] public Vector2[] shipPoints;

    [SerializeField] private Transform zoneMoveKeyPoint;
    [SerializeField] private Vector2 cellsCount;
    [SerializeField] private SelectShipFieldController selectShipField;
    [SerializeField] private GameObject tappedShipParent;
    private Transform baseParent;
    private SelectedShipAreaController shipArea;
    private Camera mainCamera;

    public int shipSizeInCells { get; private set; }
    private bool IsShipFlippedOnY;
    private bool IsShipInGameField;
    private Vector2 shipLastPosOnField;
    private Vector2 startPos;
    private Vector2 dragPosition;
    private Vector2 lastMovePointPos;
    private Vector3 keyPointOffSet;
    private float[] fieldBorders = { };

    private float borderOffset; // dont increese more than half cell size
    private float borderKOffset = 0.45f; // dont increese more than 0.5;
    private float fieldCellSize;
    private float minXPos;
    private float maxXPos;
    private float minYPos;
    private float maxYPos;

    private void Awake() {
        baseParent = transform.parent;
        shipSizeInCells = (int)cellsCount.x;
    }

    private void Start() {
        mainCamera = ServiceManager.GetInstance().GetMainCamera();
        fieldBorders = selectShipField.GetFieldBorders();
        fieldCellSize = selectShipField.GetCellSizeDelta();
        borderOffset = fieldCellSize * borderKOffset;
        shipArea = selectShipField.GetShipArea(shipSizeInCells);
        startPos = transform.position;
        keyPointOffSet = zoneMoveKeyPoint.position - transform.position;
    }

    private void OnDisable() {
        shipLastPosOnField = Vector2.zero;
        ResetShipState();
    }

    private void OnMouseDown() {
        if(IsShipFlippedOnY) {
            shipArea.transform.rotation = Quaternion.Euler(0, 0, 90);
        } else {
            shipArea.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        shipArea.transform.position = new Vector3(transform.position.x, transform.position.y, shipArea.transform.position.z);
        CalculateShipFieldLocateBorders();
        lastMovePointPos = transform.position;
        transform.parent = tappedShipParent.transform;
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
            SetShipPointsInMassive();
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
        transform.parent = baseParent;
        shipArea.DeactivateArea();
    } 

    public void SetGeneratedPoints(CellPointPos[] points) {
        if(IsShipFlippedOnY) {
            RotateShip(false);
        }
        bool IsFlippedOnY = false;
        float xPos;
        float yPos;
        Vector2 firstPointPos = selectShipField.GetPosByCellPoint(points[0]);
        Vector2 lastPointPos = selectShipField.GetPosByCellPoint(points[points.Length - 1]);
        IsShipInGameField = true;
        if(points.Length > 1) {
            if(points[0].number == points[1].number) {
                IsFlippedOnY = true;
                xPos = firstPointPos.x;
                yPos = (lastPointPos.y - firstPointPos.y) / 2 + firstPointPos.y;
            } else {
                IsFlippedOnY = false;
                xPos = (lastPointPos.x - firstPointPos.x) / 2 + firstPointPos.x;
                yPos = firstPointPos.y;
            }
        } else {
            xPos = firstPointPos.x;
            yPos = firstPointPos.y;
        }
        transform.position = new Vector2(xPos, yPos);

        if(IsFlippedOnY) {
            RotateShip(false);
        }
        SetZonePostionOnNearestCell(transform.position, gameObject);
        AddShipToFieldBase();
    }

    public void RotateShip(bool IsManualChange = true) {
        float xCells = cellsCount.y;
        float yCells = cellsCount.x;
        cellsCount = new Vector2(xCells, yCells);
        if(!IsShipFlippedOnY) {
            transform.Rotate(new Vector3(0, 0, 90));
            if(IsManualChange) {
                transform.position = new Vector3(transform.position.x + keyPointOffSet.x,
                    transform.position.y - keyPointOffSet.x, transform.position.z);
            }
        } else {
            transform.Rotate(new Vector3(0, 0, -90));
            if(IsManualChange) {
                transform.position = new Vector3(transform.position.x - keyPointOffSet.y,
                transform.position.y + keyPointOffSet.y, transform.position.z);
            }
        }
        IsShipFlippedOnY = !IsShipFlippedOnY;
        keyPointOffSet = zoneMoveKeyPoint.position - transform.position;
        CalculateShipFieldLocateBorders();
        SetShipPointsInMassive();
        if(IsManualChange) {
            if((!CheckIsPointInFieldZone(transform.position) || !selectShipField.CheckIfShipCanLocate(shipPoints, this))) {
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
    }

    private void IsShipInFieldZoneInDragTime() {
        if(CheckIsPointInFieldZone(dragPosition)) {
            IsShipInGameField = true;
            shipArea.ActivateArea();
            SetShipPointsInMassive();
            ChangeAreaColorIfCantLocateShip();
            if(Mathf.Abs(dragPosition.x - lastMovePointPos.x) > borderOffset || Mathf.Abs(dragPosition.y - lastMovePointPos.y) > borderOffset) {
                SetZonePostionOnNearestCell(dragPosition, shipArea.gameObject);
                lastMovePointPos = shipArea.transform.position;
            }
        } else {
            IsShipInGameField = false;
            shipArea.DeactivateArea();
        }
    }

    private void AddShipToFieldBase() {
        SetShipPointsInMassive();
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

    private void SetShipPointsInMassive() {
        Vector2[] shipPoints = new Vector2[shipSizeInCells];
        if(IsShipFlippedOnY) {
            for(int i = 0; i < shipSizeInCells; i++) {
                shipPoints[i] = new Vector2(transform.position.x, zoneMoveKeyPoint.position.y + fieldCellSize * i);
            }
        } else {
            for(int i = 0; i < shipSizeInCells; i++) {
                shipPoints[i] = new Vector2(zoneMoveKeyPoint.position.x + fieldCellSize * i, transform.position.y);
            }
        }
        this.shipPoints = shipPoints;
    }

    private void CalculateShipFieldLocateBorders() {
        float halfShipDeltaX = cellsCount.x * fieldCellSize / 2;
        float halfShipDeltaY = cellsCount.y * fieldCellSize / 2;
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
            RotateShip(false);
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
