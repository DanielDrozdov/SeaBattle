using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightMissionController : MonoBehaviour {

    [SerializeField] private FightFieldStateController playerFieldStateController;
    [Header("3 mission")]
    [SerializeField] private Ship[] caravanShipsThirdMission;
    [SerializeField] private CellPointPos[] keyPoints;
    private int caravanShipsCount;

    public void InitializeMission() {
        DataSceneTransitionController dataSceneTransitionController = DataSceneTransitionController.GetInstance();
        int missionNumber = dataSceneTransitionController.GetSelectedMissionData().missionNumber;
        if(missionNumber == 10) {
            FightGameManager.OnPlayerShotsValueChanging += CheckPlayerShotsDeadline;
        } else if(missionNumber == 8) {

        } else if(missionNumber == 7) {

        } else if(missionNumber == 3) {
            caravanShipsCount = caravanShipsThirdMission.Length;
            AssignCaravanShipsToField();
        }
    }

    private void CheckPlayerShotsDeadline() {
        FightGameManager fightGameManager = FightGameManager.GetInstance();
        if(fightGameManager.GetPlayerShotsCount() > 70) {
            fightGameManager.EndGame();
        }
    }

    private void AssignCaravanShipsToField() {
        for(int i = 0;i < 3;i++) {
            CellPointPos[] shipPoints = new CellPointPos[3];
            for(int k = 0;k < 3;k++) {
                shipPoints[k] = new CellPointPos(keyPoints[i].letter, keyPoints[i].number + k);
            }
            CellPointPos middlePoint = shipPoints[1];
            Ship caravanShip = caravanShipsThirdMission[i];
            caravanShip.gameObject.SetActive(true);
            caravanShip.OnCaravanShipDie += DeacreaseOneCaravanShip;
            caravanShip.SetShipPointsMassiveAndFightFieldController(shipPoints, playerFieldStateController);
            caravanShip.transform.position = playerFieldStateController.GetPosByCellPoint(middlePoint);
            playerFieldStateController.AddShipToField(caravanShip);
        }
    }

    private void DeacreaseOneCaravanShip() {
        caravanShipsCount--;
        if(caravanShipsCount <= 0) {
            FightGameManager.GetInstance().EndGame();
        }
    }
}
