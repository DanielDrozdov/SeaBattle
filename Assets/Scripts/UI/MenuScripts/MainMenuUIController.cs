using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUIController : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject campaignMenuPanel;
    [SerializeField] private GameObject battlesMenuPanel;
    [SerializeField] private GameObject backToMainMenuButton;
    [SerializeField] private GameObject languageDropDownList;
    private static MainMenuUIController Instance;
    private LevelTransitionPanelController levelTransitionPanelController;
    private GameObject lastOpenedPanel;
    private bool IsBackButtonPressed;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        levelTransitionPanelController = LevelTransitionPanelController.GetInstance();
        DataSceneTransitionController dataSceneTransitionController = DataSceneTransitionController.GetInstance();
        dataSceneTransitionController.SetCampaignGame(false);
        dataSceneTransitionController.SetMultiplayerStateGame(false);
        dataSceneTransitionController.ZeroSelectedShips();
    }

    public static MainMenuUIController GetInstance() {
        return Instance;
    }

    public bool ActivatePanelTransition(LevelTransitionPanelController.LevelTransitionCloseMethod closePanelMethod) {
        bool IsDone = levelTransitionPanelController.MoveToCanvasCenter(() => {
            closePanelMethod?.Invoke();
        });
        StartCoroutine(WaitLevelTransitionAnimCoroutine());
        return IsDone;
    }

    public bool GetIsBackButtonPressed() {
        return IsBackButtonPressed;
    }

    public void OnClickButton_CampaignPlay() {
        OpenPanel(campaignMenuPanel);
    }

    public void OnClickButton_BattlesPlay() {
        DataSceneTransitionController.GetInstance().ZeroSelectedShips();
        OpenPanel(battlesMenuPanel);
    }

    public void OnClickButton_BackToMainMenu() {
        BackToMainMenu();
    }

    public void OnClickButton_Exit() {
        Application.Quit();
    }

    public void BackToMainMenu() {
        IsBackButtonPressed = true;
        ActivatePanelTransition(() => {
            DataSceneTransitionController dataSceneTransitionController = DataSceneTransitionController.GetInstance();
            mainMenuPanel.SetActive(true);
            languageDropDownList.SetActive(true);
            lastOpenedPanel.SetActive(false);
            backToMainMenuButton.SetActive(false);
            if(dataSceneTransitionController.IsMultiplayerGame()) {
                NetworkHelpManager.GetInstance().CancelConnecting();
                dataSceneTransitionController.SetMultiplayerStateGame(false);
            }
            dataSceneTransitionController.SetCampaignGame(false);
            dataSceneTransitionController.ZeroSelectedShips();
            IsBackButtonPressed = false;
        });
    }

    public void LoadNextMissionPartActions() {
        mainMenuPanel.SetActive(false);
        languageDropDownList.SetActive(false);
        backToMainMenuButton.SetActive(true);
        lastOpenedPanel = campaignMenuPanel;
        campaignMenuPanel.SetActive(true);
    }

    private void OpenPanel(GameObject panelToActivated) {
        ActivatePanelTransition(() => {
            mainMenuPanel.SetActive(false);
            languageDropDownList.SetActive(false);
            backToMainMenuButton.SetActive(true);
            lastOpenedPanel = panelToActivated;
            panelToActivated.SetActive(true);
        });
    }

    private IEnumerator WaitLevelTransitionAnimCoroutine() {
        bool IsAnimPlaying = true;
        while(IsAnimPlaying) {
            if(levelTransitionPanelController.IsPanelInScreenCenter()) {
                IsAnimPlaying = false;
            }
            yield return null;
        }
        levelTransitionPanelController.MoveToStartPoint();
    }
}

public interface ISecondaryMenuPanelDisableActions {
     void ResetPanelsActivatedState();
}
