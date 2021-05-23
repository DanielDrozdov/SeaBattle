using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FightGamePlayerNetworkController : NetworkBehaviour {

    private bool IsSecondPlayerReadyToPlay;
    private FightGameManager.OpponentName playerOpponentName;

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

    public FightGameManager.OpponentName GetPlayerOpponentName() {
        return playerOpponentName;
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
        if(NetworkServer.connections.Count == 1 && !isLocalPlayer && !isServer) {
            NetworkHelpManager.GetInstance().CancelConnecting();
        }   
    }

    public override void OnStartClient() {
        base.OnStartClient();
        SetCurPlayerOppponentName();
    }

    public void SendCurrentPlayerHitsMove(Vector2[] hitPoints) {
        if(playerOpponentName != FightGameManager.GetInstance().GetCurrentOpponentNameToAttack()) {
            return;
        }
        if(isServer) {
            RpcSendCurrentPlayerHitsMove(hitPoints);
        } else {
            CmdSendCurrentPlayerHitsMove(hitPoints);
        }
    }

    [Command]
    private void CmdSendCurrentPlayerHitsMove(Vector2[] hitPoints) {
        RpcSendCurrentPlayerHitsMove(hitPoints);
    }

    [ClientRpc]
    private void RpcSendCurrentPlayerHitsMove(Vector2[] hitPoints) {
        if(isLocalPlayer) { return; }
        FightGameManager.GetInstance().GetFightFieldByOpponentName(playerOpponentName).HitByShipAttackZone(hitPoints);
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

    private void SetCurPlayerOppponentName() {
        if(isServer) {
            playerOpponentName = FightGameManager.OpponentName.P1;
            NetworkHelpManager.GetInstance().opponentNumberOnFightField = 1;
        } else {
            playerOpponentName = FightGameManager.OpponentName.P2;
            NetworkHelpManager.GetInstance().opponentNumberOnFightField = 2;
        }
    }

    private IEnumerator TransferToMainScene() {
        float waitTime = 1f;
        float waitTimeBalance = waitTime;
        while(true) {
            waitTimeBalance -= Time.deltaTime;
        }
    }
}
