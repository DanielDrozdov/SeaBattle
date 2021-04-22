using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class OpponentShotsBalancePanelController : MonoBehaviour
{
    private static OpponentShotsBalancePanelController Instance;
    private FightGameManager fightGameManager;
    private TextMeshProUGUI text;

    private void Awake() {
        Instance = this;
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Start() {
        fightGameManager = FightGameManager.GetInstance();
    }

    public static OpponentShotsBalancePanelController GetInstance() {
        return Instance;
    }

    public void UpdatePlayerShotsBalance() {
        text.text = fightGameManager.GetAvaliableCellsCountToHit().ToString();
    }
}
