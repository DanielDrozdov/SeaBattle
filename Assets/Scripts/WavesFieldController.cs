using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavesFieldController : MonoBehaviour
{
    [SerializeField] private FightFieldStateController fightFieldStateController;

    [SerializeField] private float wavesTimeDelay;
    [SerializeField] private float diffTimeBetweenFieldWaves;
    private float startScriptActivateTimeDelay = 4;

    [SerializeField] GameObject[] fieldWaves;

    public void LocateWavesOnField() {
        for(int i = 0;i < fieldWaves.Length;i++) {
            CellPointPos randomCell = fightFieldStateController.GetRandomCellWithoutShip();
            fieldWaves[i].transform.position = fightFieldStateController.GetPosByCellPoint(randomCell);
        }
        StartCoroutine(StartActivationDelayCoroutine());
    }

    private IEnumerator WavesActivationDelayCoroutine() {
        while(true) {
            if(diffTimeBetweenFieldWaves > 0) {
                diffTimeBetweenFieldWaves -= Time.deltaTime;
                yield return null;
            } else {
                fieldWaves[Random.Range(0, fieldWaves.Length)].SetActive(true);
                yield return new WaitForSeconds(wavesTimeDelay);
            }
        }
    }

    private IEnumerator StartActivationDelayCoroutine() {
        while(startScriptActivateTimeDelay > 0) {
            startScriptActivateTimeDelay -= Time.deltaTime;
            yield return null;
        }
        StartCoroutine(WavesActivationDelayCoroutine());
    }
}
