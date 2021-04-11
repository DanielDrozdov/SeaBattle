using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectShipFieldController : MonoBehaviour
{
    [SerializeField] private GameObject fourCellShipArea;

    private static SelectShipFieldController Instance;
    private FightFieldStateController gameFieldStateController;
    private float[] selectShipFieldBorders = { };

    private SelectShipFieldController() { }

    private void Awake() {
        Instance = this;
        gameFieldStateController = GetComponent<FightFieldStateController>();
    }

    private void Start() {
        selectShipFieldBorders = gameFieldStateController.GetFieldBorders();
    }

    public static SelectShipFieldController GetInstance() {
        return Instance;
    }

    public float[] GetFieldBorders() {
        return selectShipFieldBorders;
    }

    public FightFieldStateController GetGameFieldStateController() {
        return gameFieldStateController;
    }

    public GameObject GetShipArea(int shipCellsCount) {
        if(shipCellsCount == 4) {
            return fourCellShipArea;
        } else {
            return null;
        }
    }
}
