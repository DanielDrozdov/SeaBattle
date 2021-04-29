﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightGameManager : MonoBehaviour {

    public enum OpponentName {
        P1,
        P2,
        Bot
    }

    [SerializeField] private GameObject opponentMoveArrowCursor;
    [SerializeField] private Ship[] firstShipsGroup;
    [SerializeField] private Ship[] secondShipsGroup;
    [SerializeField] private FightFieldStateController firstPlayerFieldStateController;
    [SerializeField] private FightFieldStateController secondPlayerFieldStateController;
    [SerializeField] private BotAttackController botAttackController;
    [SerializeField] private WinOrDiePanelController WinOrDiePanel;
    private OpponentShotsBalancePanelController opponentShotsBalancePanelController;
    private DataSceneTransitionController dataSceneTransitionController;
    private static FightGameManager Instance;
    private List<Ship[]> shipsGroupList = new List<Ship[]>();

    private OpponentName currentOpponentName;
    private bool IsGameEnded;
    private bool IfCanHitTwice;
    private bool IsFirstOpponentAttackMove = true;
    private int avaliableCellsCountToHit = 4;
    private int avaliableCellsCountToHitBalance;
    private int avancedModeAvaliableCellsCountToHit = 4;
    private int classicModeAvaliableCellsCountToHit = 1;

    private void Awake() {
        Instance = this;
        dataSceneTransitionController = DataSceneTransitionController.GetInstance();
        if(dataSceneTransitionController.GetBattleMode() == DataSceneTransitionController.BattleMode.Advanced) {
            avaliableCellsCountToHit = avancedModeAvaliableCellsCountToHit;
        } else {
            avaliableCellsCountToHit = classicModeAvaliableCellsCountToHit;
        }
        avaliableCellsCountToHitBalance = avaliableCellsCountToHit;
        currentOpponentName = OpponentName.P1;
        SetPlayerNamesToOpponents();
        shipsGroupList.Add(firstShipsGroup);
        shipsGroupList.Add(secondShipsGroup);
    }

    private void Start() {
        ShipAttackZonesManager.GetInstance().ChangeOpponentAttackField(secondPlayerFieldStateController);
        opponentShotsBalancePanelController = OpponentShotsBalancePanelController.GetInstance();
        opponentShotsBalancePanelController.UpdatePlayerShotsBalance();
        LocateShipsOnFields();
        if(DataSceneTransitionController.GetInstance().GetBattleType() == DataSceneTransitionController.BattleType.P1vsP2) {
            SelectAttackZonePanelController.GetInstance().SetNewOpponentFieldAndUpdateShipsAttackZones(secondPlayerFieldStateController);
        }
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

    public void EndGame() {
        IsGameEnded = true;
        ShipAttackZonesManager.GetInstance().OffZones();
        WinOrDiePanel.ActivatePanel(currentOpponentName);
    }

    public void DecreaseOneCell() {
        if(IsGameEnded) {
            return;
        }

        if(!IfCanHitTwice) {
            avaliableCellsCountToHitBalance--;
            if(avaliableCellsCountToHitBalance <= 0) {
                ChangeAttackOpponent();
                avaliableCellsCountToHitBalance = avaliableCellsCountToHit;
            }
            opponentShotsBalancePanelController.UpdatePlayerShotsBalance();
        } else {
            if(currentOpponentName == OpponentName.Bot) {
                botAttackController.HitPlayer();
            }
        }
    }

    public void SetOpponentNextHitState(bool IsShipHit) {
        if(IsShipHit && dataSceneTransitionController.GetBattleMode() == DataSceneTransitionController.BattleMode.Classic) {
            IfCanHitTwice = true;
        } else {
            IfCanHitTwice = false;
        }
    }

    public void AttackByBotAgain() {
        if(!IsGameEnded) {
            botAttackController.HitPlayer();
        }
    }

    public void ChangeAttackOpponent() {
        if(IsGameEnded) {
            return;
        }
        avaliableCellsCountToHitBalance = avaliableCellsCountToHit;
        IsFirstOpponentAttackMove = !IsFirstOpponentAttackMove;
        opponentMoveArrowCursor.transform.Rotate(new Vector3(0,0,180));
        FightFieldStateController opponentSelfField;
        if(currentOpponentName == firstPlayerFieldStateController.GetOpponentName()) {
            ShipAttackZonesManager.GetInstance().ChangeOpponentAttackField(firstPlayerFieldStateController);
            opponentSelfField = secondPlayerFieldStateController;
            currentOpponentName = secondPlayerFieldStateController.GetOpponentName();
            if(currentOpponentName == OpponentName.Bot) {
                botAttackController.HitPlayer();
            }
        } else {
            ShipAttackZonesManager.GetInstance().ChangeOpponentAttackField(secondPlayerFieldStateController);
            opponentSelfField = firstPlayerFieldStateController;
            currentOpponentName = firstPlayerFieldStateController.GetOpponentName();
        }
        if(dataSceneTransitionController.GetBattleMode() != DataSceneTransitionController.BattleMode.Classic &&
            currentOpponentName != OpponentName.Bot) {
            SelectAttackZonePanelController.GetInstance().SetNewOpponentFieldAndUpdateShipsAttackZones(opponentSelfField);
        }
        opponentShotsBalancePanelController.UpdatePlayerShotsBalance();
    }

    private void LocateShipsOnFields() {
        List<CellPointPos[]> allShipsPoints = dataSceneTransitionController.GetSelectedShipPoints(1);
        SetCellsPointsToShips(allShipsPoints, firstPlayerFieldStateController);
        if(secondPlayerFieldStateController.GetOpponentName() == OpponentName.Bot) {
            allShipsPoints = ShipFieldPositionGenerateController.GetInstance().GetGeneratedShipsPoints();
        } else {
            allShipsPoints = dataSceneTransitionController.GetSelectedShipPoints(2);
        }
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

    private void SetPlayerNamesToOpponents() {
        firstPlayerFieldStateController.SetOpponentName(OpponentName.P1);
        if(DataSceneTransitionController.GetInstance().GetPlayerCountWithShips() == 2) {
            secondPlayerFieldStateController.SetOpponentName(OpponentName.P2);
        } else {
            secondPlayerFieldStateController.SetOpponentName(OpponentName.Bot);
        }
    }
}
