using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinOrDiePanelController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI opponentNameText;
    [SerializeField] private GameObject campaignPanelPart;
    [SerializeField] private GameObject nextMissionButton;
    [SerializeField] private GameObject resetMissionButton;

    [SerializeField] private TextMeshProUGUI playerShotsBalanceText;
    [SerializeField] private TextMeshProUGUI botDestroyedShipsBalanceText;
    [SerializeField] private TextMeshProUGUI playerAliveShipsBalanceText;
    [SerializeField] private TextMeshProUGUI missionResultText;
    [SerializeField] private TextMeshProUGUI campaignResultText;

    private FightGameManager.OpponentName winner;

    public void ActivatePanel(FightGameManager.OpponentName winnerName) {
        winner = winnerName;
        gameObject.SetActive(true);
        DataSceneTransitionController dataSceneTransitionController = DataSceneTransitionController.GetInstance();
        if(dataSceneTransitionController.IsCampaignGame()) {
            GetComponent<RectTransform>().sizeDelta = new Vector2(920, 780);
            OnCampaignPartPanel();
        }
        if(winner == FightGameManager.OpponentName.Bot) {
            opponentNameText.text = "Вы проиграли!";
        } else {
            if(dataSceneTransitionController.IsCampaignGame()) {
                opponentNameText.text = "Вы выиграли!";
            } else {
                opponentNameText.text = "Игрок " + winnerName.ToString() + " выиграл!";
            }
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
        if(winner == FightGameManager.OpponentName.Bot) {
            resetMissionButton.SetActive(true);
        } else {
            nextMissionButton.SetActive(true);
        }
        FightGameManager fightGameManager = FightGameManager.GetInstance();
        int playerShotsBalance = fightGameManager.GetPlayerShotsCount();
        int botDestroyedShipsBalance = fightGameManager.GetBotDestroyedShipsCount();
        int playerAliveShipsBalance = fightGameManager.GetP1AliveShipsCount();
        int missionResult = (100 - playerShotsBalance) + botDestroyedShipsBalance + playerAliveShipsBalance;
        int campaignResult = GetCampaignResult(missionResult);
        playerShotsBalanceText.text = "Снарядов потрачено: " + playerShotsBalance;
        botDestroyedShipsBalanceText.text = "Подбитые корабли противника: " + botDestroyedShipsBalance;
        playerAliveShipsBalanceText.text = "Выжившие корабли: " + playerAliveShipsBalance;
        missionResultText.text = "Результативность миссии: " + missionResult;
        int currentMissionNumber = DataSceneTransitionController.GetInstance().GetSelectedMissionData().missionNumber;
        campaignResultText.text = "Общий успех компании: " + campaignResult;
        if(winner == FightGameManager.OpponentName.Bot) {
            return;
        }
        PlayerPrefs.SetInt("mission" + currentMissionNumber, missionResult);
        if(PlayerPrefs.GetInt("CurrentMissionNumber") == currentMissionNumber) {
            PlayerPrefs.SetInt("CurrentMissionNumber", ++currentMissionNumber);
        }
    }

    private int GetCampaignResult(int curMissionResult) {
        if(winner == FightGameManager.OpponentName.Bot) {
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
