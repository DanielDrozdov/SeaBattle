using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinOrDiePanelController : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI opponentNameText;
    [SerializeField] private GameObject campaignPanelPart;
    [SerializeField] private GameObject nextMissionButton;
    [SerializeField] private GameObject resetMissionButton;
    [SerializeField] private GameObject gamePausePanel;
    [SerializeField] private GameObject gameAttackZoneSelectionPanel;
    [SerializeField] private GameObject gameShotsOpponentBalancePanel;

    [HideInInspector] public FightGameManager.OpponentName winnerName;

    [HideInInspector] public int playerShotsBalance;
    [HideInInspector] public int botDestroyedShipsBalance;
    [HideInInspector] public int playerAliveShipsBalance;
    [HideInInspector] public int missionResult;
    [HideInInspector] public int campaignResult;

    public void ActivatePanel(FightGameManager.OpponentName winnerName) {
        gameAttackZoneSelectionPanel.SetActive(false);
        gamePausePanel.SetActive(false);
        gameShotsOpponentBalancePanel.SetActive(false);
        gameObject.SetActive(true);
        this.winnerName = winnerName;
        DataSceneTransitionController dataSceneTransitionController = DataSceneTransitionController.GetInstance();
        if(dataSceneTransitionController.IsCampaignGame()) {
            GetComponent<RectTransform>().sizeDelta = new Vector2(920, 780);
            OnCampaignPartPanel();
        }
    }

    public void OnClickButton_ToMainMenu() {
        LevelTransitionPanelController.GetInstance().MoveToCanvasCenter(() => SceneManager.LoadScene("MainMenu"));
    }

    public void OnClickButton_ResetMission() {
        LevelTransitionPanelController.GetInstance().MoveToCanvasCenter(() => SceneManager.LoadScene(SceneManager.GetActiveScene().name));
    }

    public void OnClickButton_NextMission() {
        DataSceneTransitionController.GetInstance().IsNeedLoadNextMission = true;
        LevelTransitionPanelController.GetInstance().MoveToCanvasCenter(() => SceneManager.LoadScene("MainMenu"));
    }

    private void OnCampaignPartPanel() {
        campaignPanelPart.SetActive(true);
        if(winnerName == FightGameManager.OpponentName.Bot) {
            resetMissionButton.SetActive(true);
        } else {
            nextMissionButton.SetActive(true);
        }
        FightGameManager fightGameManager = FightGameManager.GetInstance();
        playerShotsBalance = fightGameManager.GetPlayerShotsCount();
        botDestroyedShipsBalance = fightGameManager.GetBotDestroyedShipsCount();
        playerAliveShipsBalance = fightGameManager.GetP1AliveShipsCount();
        missionResult = (100 - playerShotsBalance) + botDestroyedShipsBalance + playerAliveShipsBalance;
        campaignResult = GetCampaignResult(missionResult);
        int currentMissionNumber = DataSceneTransitionController.GetInstance().GetSelectedMissionData().missionNumber;
        if(winnerName == FightGameManager.OpponentName.Bot) {
            return;
        }
        PlayerPrefs.SetInt("mission" + currentMissionNumber, missionResult);
        if(PlayerPrefs.GetInt("CurrentMissionNumber") == currentMissionNumber) {
            PlayerPrefs.SetInt("CurrentMissionNumber", ++currentMissionNumber);
        }
    }

    private int GetCampaignResult(int curMissionResult) {
        if(winnerName == FightGameManager.OpponentName.Bot) {
            return 0;
        }
        int campaignResult = 0;
        for(int i = 1;i > 0; i++) {
            string missionKeyName = "mission" + i;
            if(!PlayerPrefs.HasKey(missionKeyName)) {
                break;
            }
            campaignResult = PlayerPrefs.GetInt(missionKeyName);
        }
        return campaignResult + curMissionResult;
    }
}
