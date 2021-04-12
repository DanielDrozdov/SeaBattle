using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectShipFieldController : GameFieldStateController
{
    [SerializeField] private GameObject fourCellShipArea;

    private static SelectShipFieldController Instance;

    private SelectShipFieldController() { }

    internal override void AddAwakeActions() {
        Instance = this;
    }

    public static SelectShipFieldController GetInstance() {
        return Instance;
    }

    public GameObject GetShipArea(int shipCellsCount) {
        if(shipCellsCount == 4) {
            return fourCellShipArea;
        } else {
            return null;
        }
    }
}
