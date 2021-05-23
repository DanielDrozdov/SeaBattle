using System.Collections;
using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MainMenuPlayerNetworkController : NetworkBehaviour
{
    private bool IsSecondPlayerReadyToPlay;

    public override void OnStartClient() {
        base.OnStartClient();
        NetworkHelpManager.GetInstance().SetPlayerNetworkController(this);
    }

    public bool GetIsSecondPlayerReady() {
        return IsSecondPlayerReadyToPlay;
    }

    public void SetCurrentPlayerReadyToPlay() {
        if(isServer) {
            RpcSendPlayerReady();
        } else {
            CmdSendPlayerReady();
        }
    }

    public void CloseWaitPanel() {
        if(isServer) {
            RpcCloseWaitPanel();
        } else {
            CmdCloseWaitPanel();
        }
    }

    public void SendShipsData(List<CellPointPos[]> shipsPoints) {
        if(isServer) {
            RpcSendShipsData(shipsPoints);
        } else {
            CmdSendShipsData(shipsPoints);
        }
    }

    public void SendGameModeData(DataSceneTransitionController.BattleMode battleMode) {
        if(isServer) {
            RpcSendGameModeData(battleMode);
        }
    }

    [Command]
    private void CmdSendShipsData(List<CellPointPos[]> shipsPoints) {
        RpcSendShipsData(shipsPoints);
    }

    [ClientRpc]
    private void RpcSendShipsData(List<CellPointPos[]> shipsPoints) {
        DataSceneTransitionController dataSceneTransitionController = DataSceneTransitionController.GetInstance();
        if(isLocalPlayer && dataSceneTransitionController.GetPlayerCountWithShips() == 2) { return; }
        if(NetworkHelpManager.GetInstance().opponentNumberOnFightField == 2) {
            CmdSendShipsData(dataSceneTransitionController.GetSelectedShipPoints(2));
            dataSceneTransitionController.SetSelectedShips(1, shipsPoints);
        } else {
            CmdSendShipsData(dataSceneTransitionController.GetSelectedShipPoints(1));
            dataSceneTransitionController.SetSelectedShips(2, shipsPoints);
        }
    }

    [ClientRpc]
    private void RpcSendGameModeData(DataSceneTransitionController.BattleMode battleMode) {
        DataSceneTransitionController.GetInstance().SetBattleMode(battleMode);
        DataSceneTransitionController.GetInstance().SetBattleType(DataSceneTransitionController.BattleType.P1vsP2);
    }

    [Command]
    private void CmdCloseWaitPanel() {
        RpcCloseWaitPanel();
    }

    [ClientRpc]
    private void RpcCloseWaitPanel() {
        WaitPlayerPanelController.GetInstance().CloseWaitPanelAndContinue();
    }

    [Command]
    private void CmdSendPlayerReady() {
        RpcSendPlayerReady();
    }

    [ClientRpc]
    private void RpcSendPlayerReady() {
        if(isLocalPlayer) { return; }
        IsSecondPlayerReadyToPlay = true;
    }

    public void Load() {
        if(isServer) {
            StartCoroutine(LoadServerSceneCoroutine());
        } else {
            ReadyPlayer();
        }
    }

    [Server]
    public IEnumerator LoadServerSceneCoroutine() {
        if(!NetworkClient.ready) {
            NetworkClient.Ready();
        }
        while(!NetworkClient.ready) {
            yield return null;
        }
        NetworkManager.singleton.ServerChangeScene("FightScene");

    }

    [Client]
    private void ReadyPlayer() {
        if(!NetworkClient.ready) {
            NetworkClient.Ready();
        }
    }
}
