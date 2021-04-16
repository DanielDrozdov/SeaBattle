using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotShipLocateHelper : MonoBehaviour
{
    [SerializeField] private Ship[] ships;
    private ShipFieldPositionGenerateController shipFieldPositionGenerate;
    private FightFieldStateController botField;

    private void Start() {
        shipFieldPositionGenerate = ShipFieldPositionGenerateController.GetInstance();
        botField = GetComponent<FightFieldStateController>();
        LocateShips();
    }

    private void LocateShips() {
        botField.SetShips(ships);
        List<CellPointPos[]> shipsGeneratedPoints = shipFieldPositionGenerate.GetGeneratedShipsPoints();
        for(int i = 0; i < 10; i++) {
            Ship ship = ships[i];
            for(int k = 0; k < 10; k++) {
                if(ship.GetCellsSize() == shipsGeneratedPoints[k].Length) {
                    ship.SetShipPointsMassiveAndFightFieldController(shipsGeneratedPoints[k], botField);
                    shipsGeneratedPoints.RemoveAt(k);
                    break;
                }
            }
        }
    }
}

