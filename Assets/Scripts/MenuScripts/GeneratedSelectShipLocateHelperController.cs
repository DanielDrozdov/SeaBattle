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
        List<CellPointPos[]> shipsGeneratedPoints = shipFieldPositionGenerate.GetGeneratedShipsPoints();
        selectShipFieldController.ClearReservedShips();
        for(int i = 0; i < 10;i++) {
            SelectShipController ship = ships[i];
            for(int k = 0; k < 10;k++) {
                if(ship.shipSizeInCells == shipsGeneratedPoints[k].Length) {
                    ship.SetGeneratedPoints(shipsGeneratedPoints[k]);
                    shipsGeneratedPoints.RemoveAt(k);
                    break;
                }
            }
        }
    }
}
