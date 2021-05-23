using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightMissionController : MonoBehaviour {

    [SerializeField] private FightFieldStateController playerFieldStateController;
    private static FightMissionController Instance;
    private int missionNumber;
    [Header("3 mission")]
    [SerializeField] private Ship[] caravanShipsThirdMission;
    [SerializeField] private CellPointPos[] keyPoints;
    private int caravanShipsCount;
    [Header("7 mission")]
    [SerializeField] private int enemySubmarineShotsBalance = 6;
    private int shotsDecreaseDelta = 3;
    [Header("8 mission")]
    private bool IsShipsVulnerable;
    [Header("10 mission")]
    [SerializeField] private Ship playerFlagmanShip;

    private void Awake() {
        if(!DataSceneTransitionController.GetInstance().IsCampaignGame()) {
            Destroy(this);
            return;
        }
        Instance = this;
        missionNumber = DataSceneTransitionController.GetInstance().GetSelectedMissionData().missionNumber;
        if(missionNumber == 8) {
            IsShipsVulnerable = true;
        }
    }

    public void InitializeMission() {
        DataSceneTransitionController dataSceneTransitionController = DataSceneTransitionController.GetInstance();
        if(missionNumber == 10) {
            FightGameManager.OnPlayerShotsValueChanging += CheckPlayerShotsDeadline;
            playerFlagmanShip.OnShipDestroy += PlayerEndGame;
        } else if(missionNumber == 7) {
            FightGameManager.OnOpponentChanging += HitRandomCellsBySubmarine;
            HitRandomCellsBySubmarine();
        } else if(missionNumber == 3) {
            caravanShipsCount = caravanShipsThirdMission.Length;
            AssignCaravanShipsToField();
        }
    }

    public static FightMissionController GetInstance() {
        return Instance;
    }

    public bool IsShipsCanBeTemporarilyDeactivated() {
        return IsShipsVulnerable;
    }

    private void CheckPlayerShotsDeadline() {
        FightGameManager fightGameManager = FightGameManager.GetInstance();
        if(fightGameManager.GetPlayerShotsCount() > 70) {
            PlayerEndGame();
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
            PlayerEndGame();
        }
    }

    private void HitRandomCellsBySubmarine() {
        if(enemySubmarineShotsBalance <= 0) {
            FightGameManager.OnOpponentChanging -= HitRandomCellsBySubmarine;
            return;
        }

        for(int i = 0; i < enemySubmarineShotsBalance;i++) {
            Vector2 hitPos = playerFieldStateController.GetRandomFreePointToHit();
            playerFieldStateController.HitEnemyByPos(hitPos,false);
        }
        enemySubmarineShotsBalance -= shotsDecreaseDelta;
    }

    private void PlayerEndGame() {
        FightGameManager.GetInstance().EndGame(FightGameManager.OpponentName.P1);
    }
}
