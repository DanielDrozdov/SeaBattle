using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectShipFieldController : GameFieldState
{
    [SerializeField] private SelectedShipAreaController fourCellShipArea;
    [SerializeField] private SelectedShipAreaController threeCellShipArea;
    [SerializeField] private SelectedShipAreaController twoCellShipArea;
    [SerializeField] private SelectedShipAreaController oneCellShipArea;
    private SelectShipController pressedShip;
    private static SelectShipFieldController Instance;

    private Dictionary<SelectShipController, CellPointPos[]> shipReservedPoints;

    private SelectShipFieldController() { }

    internal override void AddAwakeActions() {
        Instance = this;
        shipReservedPoints = new Dictionary<SelectShipController, CellPointPos[]>();
    }

    public static SelectShipFieldController GetInstance() {
        return Instance;
    }

    public void ClearReservedShips() {
        shipReservedPoints.Clear();
    }

    public List<CellPointPos[]> GetSelectedShips() {
        List<CellPointPos[]> selectedShips = new List<CellPointPos[]>(10);
        foreach(CellPointPos[] shipPoints in shipReservedPoints.Values) {
            selectedShips.Add(shipPoints);
        }
        return selectedShips;
    }

    public bool IfAllShipsAreSelected() {
        return (shipReservedPoints.Values.Count == 10) ? true : false;
    }

    public SelectedShipAreaController GetShipArea(int shipCellsCount) {
        SelectedShipAreaController area;
        if(shipCellsCount == 4) {
            area = fourCellShipArea;
        } else if(shipCellsCount == 3) {
            area = threeCellShipArea;
        } else if(shipCellsCount == 2) {
            area = twoCellShipArea;
        } else {
            area = oneCellShipArea;
        }
        return area;
    }

    public void ChangePressedShip(int shipCellsCount, SelectShipController ship) {
        if(shipCellsCount > 1) {
            pressedShip = ship;
        } else {
            pressedShip = null;
        }
    }

    public void RotateShip() {
        if(pressedShip == null) {
            return;
        }
        pressedShip.RotateShip();
    }

    public bool AddShipToFieldBase(Vector2[] shipPoints,SelectShipController ship) {
        CellPointPos[] currentShipCellsPointsMassive = GetCellPointsPosByDefaultPositions(shipPoints);
        if(CheckIfShipCanLocate(currentShipCellsPointsMassive,ship)) {
            if(shipReservedPoints.ContainsKey(ship)) {
                shipReservedPoints.Remove(ship);
            }
            shipReservedPoints.Add(ship, currentShipCellsPointsMassive);
            return true;
        } else {
            return false;
        }
    }

    public void ResetSelectedShipReservedPoints(SelectShipController ship) {
        if(shipReservedPoints.ContainsKey(ship)) {
            shipReservedPoints.Remove(ship);
        }
    }

    public bool CheckIfShipCanLocate(Vector2[] shipPoints, SelectShipController ship) {
        CellPointPos[] currentShipCellsPointsMassive = GetCellPointsPosByDefaultPositions(shipPoints);
        if(CheckIfShipCanLocate(currentShipCellsPointsMassive,ship)) {
            return true;
        } else {
            return false;
        }
    }

    private bool CheckIfShipCanLocate(CellPointPos[] shipCellPoints,SelectShipController ship) {
        CellPointPos[] currentShipCellsPointsMassive = shipCellPoints;
        foreach(SelectShipController selectedShip in shipReservedPoints.Keys) {
            if(ship == selectedShip) {
                continue;
            }
            CellPointPos[] selectedShipCellsPoints = shipReservedPoints[selectedShip];
            for(int i = 0; i < currentShipCellsPointsMassive.Length; i++) {
                for(int k = 0; k < selectedShipCellsPoints.Length; k++) {
                    int charDelta = Mathf.Abs(currentShipCellsPointsMassive[i].letter - selectedShipCellsPoints[k].letter);
                    int numDelta = Mathf.Abs(currentShipCellsPointsMassive[i].number - selectedShipCellsPoints[k].number);
                    if(charDelta <= 1 && numDelta <= 1) {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    private CellPointPos[] GetCellPointsPosByDefaultPositions(Vector2[] positions) {
        CellPointPos[] shipCellsPoints = new CellPointPos[positions.Length];
        for(int i = 0; i < positions.Length; i++) {
            shipCellsPoints[i] = SearchTapCellData(positions[i], fieldPoints);
        }
        return shipCellsPoints;
    }
}
