using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FightGamePlayerNetworkController : NetworkBehaviour {

    private bool IsSecondPlayerReadyToPlay;

    public override void OnStartAuthority() {
        base.OnStartAuthority();
        if(!isServer) {
            CmdInitialize();
            if(SceneManager.GetActiveScene().name == "FightScene") {
                CmdSetSecondPlayerReadyToGame();
            }
        } else if(isServer && isLocalPlayer && SceneManager.GetActiveScene().name == "FightScene") {
            StartCoroutine(WaitAllPlayersAuthorityAndChangeOpponentToAttackCoroutine());
        }
    }

    public void SetStateToPausePanel(bool value) {
        if(isServer) {
            RpcChangePausePanelState(value);
        } else {
            CmdChangePausePanelState(value);
        }
    }

    public void ChangePlayerToAttack(FightGameManager.OpponentName opponentName) {
        if(isServer) {
            RpcChangeEnemyPlayerCurrentOpponentToAttack(opponentName);
        } else {
            CmdChangeEnemyPlayerCurrentOpponentToAttack(opponentName);
        }
    }

    public override void OnStopClient() {
        base.OnStopClient();
        if(SceneManager.GetActiveScene().name == "FightScene") {
            SceneManager.LoadScene("MainMenu");
        } else if(!MainMenuUIController.GetInstance().GetIsBackButtonPressed()) {
            MainMenuUIController.GetInstance().BackToMainMenu();
        }
        if(NetworkServer.connections.Count == 1) {
            NetworkHelpManager.GetInstance().CancelConnecting();
        }
    }

    [Command]
    private void CmdInitialize() {
        RpcInitialize();
    }

    [ClientRpc]
    private void RpcInitialize() {
        NetworkHelpManager.GetInstance().InitializePlayersObjects();
    }

    [Command]
    private void CmdChangePausePanelState(bool value) {
        RpcChangePausePanelState(value);
    }

    [ClientRpc]
    private void RpcChangePausePanelState(bool value) {
        if(isLocalPlayer) { return; }
        if(value) {
            PausePanelController.GetInstance().ActivatePausePanel();
        } else {
            PausePanelController.GetInstance().ReturnToGame();
        }
    }

    [Command]
    private void CmdChangeEnemyPlayerCurrentOpponentToAttack(FightGameManager.OpponentName opponentName) {
        RpcChangeEnemyPlayerCurrentOpponentToAttack(opponentName);
    }

    [ClientRpc]
    private void RpcChangeEnemyPlayerCurrentOpponentToAttack(FightGameManager.OpponentName opponentName) {
        if(isLocalPlayer) { return; }
        FightGameManager.GetInstance().SetOpponentMove(opponentName);
    }

    [Command]
    private void CmdSetSecondPlayerReadyToGame() {
        RpcSetSecondPlayerReadyToGame();
    }

    [ClientRpc]
    private void RpcSetSecondPlayerReadyToGame() {
        if(isLocalPlayer) { return; }
        NetworkHelpManager.GetInstance().GetCurrentPlayerFightNetworkController().IsSecondPlayerReadyToPlay = true;
    }

    private IEnumerator WaitAllPlayersAuthorityAndChangeOpponentToAttackCoroutine() {
        while(!IsSecondPlayerReadyToPlay) {
            yield return null;
        }
        FightGameManager.OpponentName hostOpponentNameToAttack = FightGameManager.GetInstance().GetCurrentOpponentNameToAttack();
        ChangePlayerToAttack(hostOpponentNameToAttack);
    }
}
