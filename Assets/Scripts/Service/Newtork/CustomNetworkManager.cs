using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkManager
{
    public override void OnClientDisconnect(NetworkConnection conn) {
        base.OnClientDisconnect(conn);
        DisconnectActions();
    }

    public override void OnServerDisconnect(NetworkConnection conn) {
        base.OnServerDisconnect(conn);
        DisconnectActions();
    }

    private void DisconnectActions() {
        if(SceneManager.GetActiveScene().name == "FightScene") {
            SceneManager.LoadScene("MainMenu");
        } else if(!MainMenuUIController.GetInstance().GetIsBackButtonPressed()) {
            MainMenuUIController.GetInstance().BackToMainMenu();
        }
    }
}
