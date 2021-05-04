using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratedSelectShipLocateHelperController : MonoBehaviour
{
    [SerializeField] private SelectShipController[] ships;

    private ShipFieldPositionGenerateController shipFieldPositionGenerate;

    private void Start() {
        shipFieldPositionGenerate = ShipFieldPositionGenerateController.GetInstance();
    }

    public void LocateShipsOnField(SelectShipFieldController selectShipFieldController) {
        List<CellPointPos[]> shipsGeneratedPoints;
        List<CellPointPos[]> caravanShipsCells;
        CaravanMissionShipAssignController caravanMissionShipAssignController = CaravanMissionShipAssignController.GetInstance();
        selectShipFieldController.ClearReservedShips();
        if(caravanMissionShipAssignController != null) {
            if(caravanMissionShipAssignController.IsCaravanMission()) {
                caravanShipsCells = caravanMissionShipAssignController.GetCaravanShipsCellsPoints();
                shipsGeneratedPoints = shipFieldPositionGenerate.GetGeneratedShipsPoints(false, caravanShipsCells);
            } else {
                shipsGeneratedPoints = shipFieldPositionGenerate.GetGeneratedShipsPoints(false);
            }
        } else {
            shipsGeneratedPoints = shipFieldPositionGenerate.GetGeneratedShipsPoints(false);
        }

        for(int i = 0; i < ships.Length; i++) {
            SelectShipController ship = ships[i];
            for(int k = 0; k < shipsGeneratedPoints.Count; k++) {
                if(ship.shipSizeInCells == shipsGeneratedPoints[k].Length) {
                    ship.SetGeneratedPoints(shipsGeneratedPoints[k]);
                    shipsGeneratedPoints.RemoveAt(k);
                    break;
                }
            }
        }
    }

    public void SetActivatedShips(SelectShipController[] activatedShips) {
        ships = activatedShips;
    }
}
