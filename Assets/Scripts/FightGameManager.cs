using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightGameManager : MonoBehaviour {

    [SerializeField] private Ship[] ships;
    [SerializeField] private FightFieldStateController fightFieldStateController;
    private Dictionary<int, List<CellPointPos[]>> shipsCellsPoints;

    private void Awake() {
        FillShipsPointsDictionary();
        SetCellsPointsToShips();
        fightFieldStateController.SetShips(ships);
    }

    private void FillShipsPointsDictionary() {
        DataSceneTransitionController dataSceneTransition = DataSceneTransitionController.GetInstance();
        List<CellPointPos[]> allShipsPoints = dataSceneTransition.GetSelectedShipPoints();
        shipsCellsPoints = new Dictionary<int, List<CellPointPos[]>>();
        for(int i = 1; i <= 4;i++) {
            shipsCellsPoints.Add(i, new List<CellPointPos[]>(i));
        }
        for(int i = 0;i < allShipsPoints.Count;i++) {
            shipsCellsPoints[allShipsPoints[i].Length].Add(allShipsPoints[i]);
        }
    }

    private void SetCellsPointsToShips() {
        for(int i = 0;i < 10;i++) {
            List<CellPointPos[]> sameCellSizeShips = shipsCellsPoints[ships[i].GetCellsSize()];
            ships[i].SetShipPointsMassiveAndFightFieldController(sameCellSizeShips[0], fightFieldStateController);
            sameCellSizeShips.RemoveAt(0);
        }
    }
}
