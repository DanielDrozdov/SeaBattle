using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIController : MonoBehaviour
{
    [SerializeField] private GameObject gamePausePanel;
    [SerializeField] private GameObject gameAttackZoneSelectionPanel;
    [SerializeField] private GameObject gameShotsOpponentBalancePanel;
    private static GameUIController Instance;

    private void Awake() {
        Instance = this;
    }

    public static GameUIController GetInstance() {
        return Instance;
    }

    public void OffBaseUI() {
        gamePausePanel.SetActive(false);
        OffOrOnBaseUIWithoutGamePauseButton(false);
    }

    public void OffOrOnBaseUIWithoutGamePauseButton(bool value) {
        if(DataSceneTransitionController.GetInstance().GetBattleMode() == DataSceneTransitionController.BattleMode.Advanced) {
            gameAttackZoneSelectionPanel.SetActive(value);
        }
        gameShotsOpponentBalancePanel.SetActive(value);
    }

}
