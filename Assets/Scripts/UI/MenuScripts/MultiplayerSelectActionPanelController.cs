using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerSelectActionPanelController : MonoBehaviour {

    [SerializeField] private GameObject waitPlayerPanel;

    public void OnClickButton_CreateGame() {
        MoveToWaitPlayerScene();
        NetworkHelpManager.GetInstance().CreateGameAndBecomeHost();
    }

    public void OnClickButton_JoinGame() {
        MoveToWaitPlayerScene();
        NetworkHelpManager.GetInstance().JoinToHost();
    }

    private void MoveToWaitPlayerScene() {
        MainMenuUIController.GetInstance().ActivatePanelTransition(() => {
            waitPlayerPanel.SetActive(true);
            gameObject.SetActive(false);
        });
    }
}
