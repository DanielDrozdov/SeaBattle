using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaitPlayerPanelController : MonoBehaviour
{
    [SerializeField] private string[] searchPlayerStrings;
    [SerializeField] private TextMeshProUGUI text;

    private bool IsFirstStart = true;

    private void OnEnable() {
        StartCoroutine(AnimCoroutine());
    }

    private void OnDisable() {
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
            text.text = searchPlayerStrings[stringNumber++];
            yield return new WaitForSeconds(0.5f);
        }
    } 
}
