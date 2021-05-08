using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectedMissionPanelController : MonoBehaviour
{
    public int missionNum;
    private static SelectedMissionPanelController Instance;
    [SerializeField] private GameObject SelectedShipsFieldForMissionsPanel;
    [SerializeField] private GameObject missionsListPanel;
    [SerializeField] private TextMeshProUGUI missionNameText;
    [SerializeField] private TextMeshProUGUI missionNumberText;
    [SerializeField] private TextMeshProUGUI missionDesriptionText;
    [SerializeField] private Image missionImage;

    private void Awake() {
        Instance = this;
        gameObject.SetActive(false);
    }

    private void OnDisable() {
        gameObject.SetActive(false);
    }

    public static SelectedMissionPanelController GetInstance() {
        return Instance;
    }

    public void OnClickButton_StartMission() {
        MainMenuUIController.GetInstance().ActivatePanelTransition(() => {
            SelectedShipsFieldForMissionsPanel.SetActive(true);
            gameObject.SetActive(false);
            DataSceneTransitionController dataSceneTransitionController = DataSceneTransitionController.GetInstance();
            dataSceneTransitionController.SetBattleMode(DataSceneTransitionController.BattleMode.Advanced);
            dataSceneTransitionController.SetBattleType(DataSceneTransitionController.BattleType.P1vsBot);
            dataSceneTransitionController.SetBotDifficulty(DataSceneTransitionController.BotDifficulty.Hard);
        });
    }

    public void SetMissionData(Sprite missionSprite,int missionNumber,string missionName,string missionDescription) {
        missionNum = missionNumber;
        missionsListPanel.SetActive(false);
        gameObject.SetActive(true);
        missionImage.sprite = missionSprite;
        missionNameText.text = missionName.ToString();
        missionDesriptionText.text = missionDescription.ToString();
    }


}
