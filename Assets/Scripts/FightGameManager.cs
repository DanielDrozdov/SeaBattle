using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightGameManager : MonoBehaviour {

    public enum OpponentName {
        P1,
        P2,
        Bot
    }

    [SerializeField] private SpriteRenderer opponentMoveArrowCursor;
    [SerializeField] private Ship[] firstShipsGroup;
    [SerializeField] private Ship[] secondShipsGroup;
    [SerializeField] private FightFieldStateController firstPlayerFieldStateController;
    [SerializeField] private FightFieldStateController secondPlayerFieldStateController;
    [SerializeField] private BotAttackController botAttackController;
    private static FightGameManager Instance;
    private List<Ship[]> shipsGroupList = new List<Ship[]>();

    private OpponentName currentOpponentName;
    private bool IsFirstOpponentAttackMove = true;
    private int avaliableCellsCountToHit = 4;
    private int avaliableCellsCountToHitBalance;

    private void Awake() {
        Instance = this;
        avaliableCellsCountToHitBalance = avaliableCellsCountToHit;
        currentOpponentName = OpponentName.P1;
        shipsGroupList.Add(firstShipsGroup);
        shipsGroupList.Add(secondShipsGroup);
    }

    private void Start() {
        ShipAttackZonesManager.GetInstance().ChangeOpponentAttackField(secondPlayerFieldStateController);
        LocateShipsOnFields();
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
        if(currentOpponentName == firstPlayerFieldStateController.GetOpponentName()) {
            ShipAttackZonesManager.GetInstance().ChangeOpponentAttackField(firstPlayerFieldStateController);
            currentOpponentName = secondPlayerFieldStateController.GetOpponentName();
            botAttackController.HitPlayer();
        } else {
            ShipAttackZonesManager.GetInstance().ChangeOpponentAttackField(secondPlayerFieldStateController);
            currentOpponentName = firstPlayerFieldStateController.GetOpponentName();
        }
    }

    private void LocateShipsOnFields() {
        DataSceneTransitionController dataSceneTransition = DataSceneTransitionController.GetInstance();
        List<CellPointPos[]> allShipsPoints = dataSceneTransition.GetSelectedShipPoints();
        SetCellsPointsToShips(allShipsPoints, firstPlayerFieldStateController);
        allShipsPoints = ShipFieldPositionGenerateController.GetInstance().GetGeneratedShipsPoints();
        SetCellsPointsToShips(allShipsPoints, secondPlayerFieldStateController);
    }

    private void SetCellsPointsToShips(List<CellPointPos[]> allShipsPoints,FightFieldStateController opponentField) {
        Ship[] ships = shipsGroupList[0];
        Dictionary<int, List<CellPointPos[]>> shipsCellsPoints = new Dictionary<int, List<CellPointPos[]>>();
        for(int i = 1; i <= 4; i++) {
            shipsCellsPoints.Add(i, new List<CellPointPos[]>(i));
        }
        for(int i = 0; i < allShipsPoints.Count; i++) {
            shipsCellsPoints[allShipsPoints[i].Length].Add(allShipsPoints[i]);
        }
        for(int i = 0;i < 10;i++) {
            List<CellPointPos[]> sameCellSizeShips = shipsCellsPoints[ships[i].GetCellsSize()];
            ships[i].SetShipPointsMassiveAndFightFieldController(sameCellSizeShips[0], opponentField);
            sameCellSizeShips.RemoveAt(0);
        }
        opponentField.SetShips(ships);
        shipsGroupList.RemoveAt(0);
    }
}
