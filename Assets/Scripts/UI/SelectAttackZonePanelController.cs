using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectAttackZonePanelController : MonoBehaviour {

    [SerializeField] SelectAttackZonePanelButtonController[] SelectAttackZonePanelButtonControllers;
    private static SelectAttackZonePanelController Instance;
    private Dictionary<int,SelectAttackZonePanelButtonController> shipsButtonsSize;
    private FightFieldStateController currentFightFieldStateController;

    private bool IsStartedObjDisable = true;

    private void Awake() {
        DataSceneTransitionController dataSceneTransitionController = DataSceneTransitionController.GetInstance();

        if(dataSceneTransitionController.GetBattleMode() == DataSceneTransitionController.BattleMode.Classic) {
            Destroy(gameObject);
        }
        Instance = this;
        FillButtonsDict();
    }

    public static SelectAttackZonePanelController GetInstance() {
        return Instance;
    }

    private void OnDisable() {
        if(IsStartedObjDisable) {
            IsStartedObjDisable = false;
            return;
        }
        ShipAttackZonesManager shipAttackZonesManager = ShipAttackZonesManager.GetInstance();
        shipAttackZonesManager?.OffZones();
    }

    public void SetNewOpponentFieldAndUpdateShipsAttackZones(FightFieldStateController fightFieldStateController) {        
        currentFightFieldStateController = fightFieldStateController;
        ShipAttackZonesManager.GetInstance().OffZones();
        if(DataSceneTransitionController.GetInstance().IsMultiplayerGame()) {
            if(FightGameManager.GetInstance().GetCurrentOpponentNameToAttack() != FightGameManager.OpponentName.P1) {
                DeactivateAllButtons();
            } else {
                UpdateAliveShips();
            }
        } else {
            UpdateAliveShips();
        }
    }

    public void UpdateAliveShips() {
        DeactivateAllButtons();
        List<Ship> aliveShips = currentFightFieldStateController.GetAliveShipList();
        for(int i = 0; i < aliveShips.Count; i++) {
            if(shipsButtonsSize.ContainsKey(aliveShips[i].GetCellsSize()) && !aliveShips[i].IsCaravanShip()
                && !aliveShips[i].IsShipTemporarilyDeactivated()) {
                shipsButtonsSize[aliveShips[i].GetCellsSize()].ActivateAttackButton();
            }
        }
    }

    private void FillButtonsDict() {
        shipsButtonsSize = new Dictionary<int, SelectAttackZonePanelButtonController>();
        for(int i = 1;i <= 4;i++) {
            for(int k = 0; k < 4;k++) {
                if(i == SelectAttackZonePanelButtonControllers[k].GetShipCellsSize()) {
                    shipsButtonsSize.Add(i, SelectAttackZonePanelButtonControllers[k]);
                    break;
                }
            }
        }
    }

    private void DeactivateAllButtons() {
        for(int i = 0;i < 4;i++) {
            SelectAttackZonePanelButtonControllers[i].DeactivateAttackButton();
        }
    }
}
