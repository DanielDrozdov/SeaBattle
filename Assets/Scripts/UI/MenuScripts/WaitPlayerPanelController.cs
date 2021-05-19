using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaitPlayerPanelController : MonoBehaviour {
    [SerializeField] private GameObject selectionShipsField;
    [SerializeField] private string[] waitPlayerStrings;
    [SerializeField] private TextMeshProUGUI text;

    private static WaitPlayerPanelController Instance;
    private bool IsConnectState = true;

    private bool IsFirstStart = true;

    private void Awake() {
        Instance = this;
    }

    public static WaitPlayerPanelController GetInstance() {
        return Instance;
    }

    private void OnEnable() {
        Debug.Log("ad");
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

    public void OpenWaitPanel() {
        MainMenuUIController.GetInstance().ActivatePanelTransition(() => {
            IsConnectState = false;
            gameObject.SetActive(true);
            selectionShipsField.SetActive(false);
        });
    }

    public void CloseWaitPanelAndContinue() {
        StartCoroutine(WaitTransitionPanelEndAndContinueCoroutine());
    }

    private IEnumerator AnimCoroutine() {
        Debug.Log("asdasdadsdasdasdada");
        int stringNumber = 0;
        while(true) {
            if(stringNumber > 2) {
                stringNumber = 0;
            }
            if(NetworkHelpManager.GetInstance().IsAllPlayerConnected() && IsConnectState) {
                MainMenuPlayerNetworkUIController.GetInstance().CloseWaitPanel();
                yield break;
            } else if(!IsConnectState) {
                Debug.Log("123");
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
