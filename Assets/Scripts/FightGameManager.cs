using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightGameManager : MonoBehaviour {

    public enum OpponentName {
        P1,
        P2
    }

    [SerializeField] private SpriteRenderer opponentMoveArrowCursor;
    [SerializeField] private Ship[] ships;
    [SerializeField] private FightFieldStateController firstPlayerFieldStateController;
    [SerializeField] private FightFieldStateController secondPlayerFieldStateController;
    [SerializeField] private BotAttackController botAttackController;
    private static FightGameManager Instance;
    private Dictionary<int, List<CellPointPos[]>> shipsCellsPoints;

    private OpponentName currentOpponentName;
    private bool IsFirstOpponentAttackMove = true;
    private int avaliableCellsCountToHit = 4;
    private int avaliableCellsCountToHitBalance;

    private void Awake() {
        Instance = this;
        avaliableCellsCountToHitBalance = avaliableCellsCountToHit;
        currentOpponentName = OpponentName.P1;
    }

    private void Start() {
        ShipAttackZonesManager.GetInstance().ChangeOpponentAttackField(secondPlayerFieldStateController);
        FillPlayerShipsPointsDictionary();
        SetCellsPointsToShips();
        firstPlayerFieldStateController.SetShips(ships);
    }

    public static FightGameManager GetInstance() {
        return Instance;
    }

    public OpponentName GetCurrentOpponentNameToAttack() {
        return currentOpponentName;
    }

    public int GetAvaliableCellsCountToHit() {
        return avaliableCellsCountToHitBalance;
    }

    public void DecreaseOneCell() {
        avaliableCellsCountToHitBalance--;
        if(avaliableCellsCountToHitBalance <= 0) {
            ChangeAttackOpponent();
            avaliableCellsCountToHitBalance = avaliableCellsCountToHit;
        }
    }

    public void ChangeAttackOpponent() {
        avaliableCellsCountToHitBalance = avaliableCellsCountToHit;
        IsFirstOpponentAttackMove = !IsFirstOpponentAttackMove;
        opponentMoveArrowCursor.flipX = IsFirstOpponentAttackMove;
        if(currentOpponentName == OpponentName.P1) {
            ShipAttackZonesManager.GetInstance().ChangeOpponentAttackField(firstPlayerFieldStateController);
            currentOpponentName = OpponentName.P2;
            botAttackController.HitPlayer();
        } else {
            ShipAttackZonesManager.GetInstance().ChangeOpponentAttackField(secondPlayerFieldStateController);
            currentOpponentName = OpponentName.P1;
        }
    }

    private void FillPlayerShipsPointsDictionary() {
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
            ships[i].SetShipPointsMassiveAndFightFieldController(sameCellSizeShips[0], firstPlayerFieldStateController);
            sameCellSizeShips.RemoveAt(0);
        }
    }
}
