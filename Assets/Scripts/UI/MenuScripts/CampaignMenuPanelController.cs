using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignMenuPanelController : MonoBehaviour, ISecondaryMenuPanelDisableActions {

    [SerializeField] private GameObject campaignMissionsList;
    [SerializeField] private GameObject campaignSelectTypePanel;
    [SerializeField] private GameObject warningPanel;
    [SerializeField] private GameObject selectedMissionShipsField;
    private MainMenuUIController mainMenuUIController;

    private void Start() {
        mainMenuUIController = MainMenuUIController.GetInstance();
    }

    private void OnDisable() {
        ResetPanelsActivatedState();
    }

    public void ResetPanelsActivatedState() {
        campaignMissionsList.SetActive(false);
        campaignSelectTypePanel.SetActive(true);
        warningPanel.SetActive(false);
        selectedMissionShipsField.SetActive(false);
    }

    public void OnClickButton_StartNewCampaign() {
        warningPanel.SetActive(true);
    }

    public void OnClickButton_ContinueCampaign() {
        if(!PlayerPrefs.HasKey("CurrentMissionNumber")) {
            PlayerPrefs.SetInt("CurrentMissionNumber", 1);
        }
        LoadCampaignPanel();
    }

    public void ContinueCampaignMission() {
        DataSceneTransitionController.GetInstance().SetCampaignGame(true);
        campaignSelectTypePanel.SetActive(false);
    }

    public void OnClickButton_WarningPanelAgree() {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("CurrentMissionNumber", 1);
        warningPanel.SetActive(false);
        LoadCampaignPanel();
    }

    public void OnClickButton_WarningPanelDisagree() {
        warningPanel.SetActive(false);
        campaignSelectTypePanel.SetActive(true);
    }

    private void LoadCampaignPanel() {
        mainMenuUIController.ActivatePanelTransition(() => {
            DataSceneTransitionController.GetInstance().SetCampaignGame(true);
            campaignSelectTypePanel.SetActive(false);
            campaignMissionsList.SetActive(true);
        });
    }
}
