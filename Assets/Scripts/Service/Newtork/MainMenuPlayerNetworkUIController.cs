using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MainMenuPlayerNetworkUIController : NetworkBehaviour
{

    private static MainMenuPlayerNetworkUIController Instance;

    private void Awake() {
        Instance = this;
    }

    public static MainMenuPlayerNetworkUIController GetInstance() {
        return Instance;
    }

    [Command]
    private void CmdCloseWaitPanel() {
        RpcCloseWaitPanel();
    }

    [ClientRpc]
    private void RpcCloseWaitPanel() {
        WaitPlayerPanelController.GetInstance().CloseWaitPanelAndContinue();
    }

    public void CloseWaitPanel() {
        if(isServer) {
            RpcCloseWaitPanel();
        }else {
            CmdCloseWaitPanel();
        }
    }
}
