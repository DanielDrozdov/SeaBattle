using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using UnityEngine.SceneManagement;

public class WaitPlayerPanelController : MonoBehaviour {
    [SerializeField] private GameObject selectionShipsField;
    [SerializeField] private string[] waitPlayerStrings;
    [SerializeField] private TextMeshProUGUI text;

    private MainMenuPlayerNetworkController mainMenuPlayerNetworkController;
    private MainMenuPlayerNetworkController secondPlayerMainMenuPlayerNetworkController;
    private static WaitPlayerPanelController Instance;
    private bool IsConnectState = true;

    private bool IsFirstStart = true;

    private void Awake() {
        Instance = this;
    }

    public static WaitPlayerPanelController GetInstance() {
        return Instance;
    }

    public GameObject GetSecondPlayerObj() {
        return secondPlayerMainMenuPlayerNetworkController.gameObject;
    }

    public GameObject GetFirstPlayerObj() {
        return mainMenuPlayerNetworkController.gameObject;
    }

    private void OnEnable() {
        StartCoroutine(AnimCoroutine());
    }

    private void OnDisable() {
        gameObject.SetActive(false);
        if(IsFirstStart) {
            IsFirstStart = false;
            return;
        }
        IsConnectState = true;
        StopAllCoroutines();
    }

    public void OpenWaitPanelWhenShipsAreSelected() {
        if(mainMenuPlayerNetworkController == null) {
            mainMenuPlayerNetworkController = NetworkHelpManager.GetInstance().GetPlayerMenuNetworkController();
        }

        if(secondPlayerMainMenuPlayerNetworkController == null) {
            SetSecondPlayerNetworkMenuController();
        }
        MainMenuUIController.GetInstance().ActivatePanelTransition(() => {
            IsConnectState = false;
            mainMenuPlayerNetworkController.SetCurrentPlayerReadyToPlay();
            gameObject.SetActive(true);
            selectionShipsField.SetActive(false);
        });
    }

    public void CloseWaitPanelAndContinue() {
        StartCoroutine(WaitTransitionPanelEndAndContinueCoroutine());
    }

    private void SetSecondPlayerNetworkMenuController() {
        NetworkIdentity[] players = FindObjectsOfType<NetworkIdentity>();
        foreach(NetworkIdentity player in players) {
            if(!player.isLocalPlayer) {
                secondPlayerMainMenuPlayerNetworkController = player.GetComponent<MainMenuPlayerNetworkController>();
            }
        }
    }

    private IEnumerator AnimCoroutine() {
        int stringNumber = 0;
        if(mainMenuPlayerNetworkController == null) {
            mainMenuPlayerNetworkController = NetworkHelpManager.GetInstance().GetPlayerMenuNetworkController();
        }
        while(true) {
            if(stringNumber > 2) {
                stringNumber = 0;
            }
            if(NetworkHelpManager.GetInstance().IsAllPlayerConnected() && IsConnectState) {
                mainMenuPlayerNetworkController.CloseWaitPanel();
                yield break;
            } else if(!IsConnectState && secondPlayerMainMenuPlayerNetworkController.GetIsSecondPlayerReady()) {
                mainMenuPlayerNetworkController.SendShipsData(DataSceneTransitionController.GetInstance().GetSelectedShipPoints(1));
                mainMenuPlayerNetworkController.SendGameModeData(DataSceneTransitionController.GetInstance().GetBattleMode());
                mainMenuPlayerNetworkController.Load();
                yield break;
            }
            text.text = waitPlayerStrings[stringNumber++];
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator WaitTransitionPanelEndAndContinueCoroutine() {
        bool IsDone = false;
        while(!IsDone) {
            IsDone = MainMenuUIController.GetInstance().ActivatePanelTransition(() => {
                gameObject.SetActive(false);
                selectionShipsField.SetActive(true);
            });
            yield return new WaitForSeconds(0.75f);
        }
    }
}
