using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaitPlayerPanelController : MonoBehaviour {
    [SerializeField] private string[] waitPlayerStrings;
    [SerializeField] private TextMeshProUGUI text;

    private bool IsFirstStart = true;

    private void OnEnable() {
        StartCoroutine(AnimCoroutine());
    }

    private void OnDisable() {
        gameObject.SetActive(false);
        if(IsFirstStart) {
            IsFirstStart = false;
            return;
        }
        StopCoroutine(AnimCoroutine());
    }

    private IEnumerator AnimCoroutine() {
        int stringNumber = 0;
        while(true) {
            if(stringNumber > 2) {
                stringNumber = 0;
            }
            if(NetworkHelpManager.GetInstance().IsAllPlayerConnected()) {
                NetworkHelpManager.NetworkAction networkAction = () => gameObject.SetActive(false);
                if(NetworkHelpManager.GetInstance().IsServerCall()) {
                    NetworkHelpManager.GetInstance().ServerAction(networkAction);
                } else {
                    NetworkHelpManager.GetInstance().ServerAction(networkAction);
                }
                yield break;
            }
            text.text = waitPlayerStrings[stringNumber++];
            yield return new WaitForSeconds(0.5f);
        }
    }
}
