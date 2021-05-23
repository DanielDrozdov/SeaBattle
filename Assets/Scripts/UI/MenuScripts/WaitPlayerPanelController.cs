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
    private string startStringName;

    private delegate void WaitPanelActionDelegate();

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
        startStringName = text.text;
        StartCoroutine(AnimCoroutine());
    }

    private void OnDisable() {
        text.text = startStringName;
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
        StartCoroutine(WaitTransitionPanelEndAndContinueCoroutine( () => {
            gameObject.SetActive(false);
            selectionShipsField.SetActive(true);
        }));
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
                int playerNumberOnFightField = NetworkHelpManager.GetInstance().opponentNumberOnFightField;
                mainMenuPlayerNetworkController.SendShipsData(DataSceneTransitionController.GetInstance().GetSelectedShipPoints(playerNumberOnFightField));
                mainMenuPlayerNetworkController.SendGameModeData(DataSceneTransitionController.GetInstance().GetBattleMode());
                StartCoroutine(WaitTransitionPanelEndAndContinueCoroutine(() => {
                    mainMenuPlayerNetworkController.Load();
                }));
                yield break;
            }
            text.text = startStringName + waitPlayerStrings[stringNumber++];
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator WaitTransitionPanelEndAndContinueCoroutine(WaitPanelActionDelegate action) {
        bool IsDone = false;
        yield return new WaitForSeconds(0.5f);
        while(!IsDone) {
            IsDone = MainMenuUIController.GetInstance().ActivatePanelTransition(() => {            
                action?.Invoke();
            });
            yield return new WaitForSeconds(0.75f);
        }
    }
}
