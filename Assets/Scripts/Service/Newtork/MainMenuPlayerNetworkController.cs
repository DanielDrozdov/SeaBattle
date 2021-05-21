using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

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
        if(isLocalPlayer) { return; }
        DataSceneTransitionController.GetInstance().SetSelectedShips(2, shipsPoints);
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
            LoadServerScene();
        }
    }

    public void LoadServerScene() {
        //AsyncOperation asyncOperation = new AsyncOperation();
        //if(!isLocalPlayer) {
        //    asyncOperation = SceneManager.LoadSceneAsync("FightScene", LoadSceneMode.Additive);
        //}
        if(!NetworkClient.ready) {
            NetworkClient.Ready();
        }
        if(isServer) {
            NetworkManager.singleton.ServerChangeScene("FightScene");
        }
        //yield return null;
        //if(!isLocalPlayer) {
        //    while(true) {
        //        if(NetworkClient.ready && asyncOperation.isDone) {
        //            serverLoadPuzzel();
        //            SceneManager.MoveGameObjectToScene(WaitPlayerPanelController.GetInstance().GetFirstPlayerObj(), SceneManager.GetSceneByName("FightScene"));
        //            SceneManager.MoveGameObjectToScene(WaitPlayerPanelController.GetInstance().GetSecondPlayerObj(), SceneManager.GetSceneByName("FightScene"));
        //            yield break;
        //        }
        //        yield return null;
        //    }
        //}
    }
}
