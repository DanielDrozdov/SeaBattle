using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUIController : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject campaignMenuPanel;
    [SerializeField] private GameObject battlesMenuPanel;
    [SerializeField] private GameObject settingsMenuPanel;
    [SerializeField] private GameObject authorsPanel;
    [SerializeField] private GameObject backToMainMenuButton;
    [SerializeField] private GameObject languageDropDownList;
    [SerializeField] private GameObject settingsButton;
    private static MainMenuUIController Instance;
    private LevelTransitionPanelController levelTransitionPanelController;
    private GameObject lastOpenedPanel;

    private void Awake() {
        Instance = this;
        if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
            Destroy(settingsButton);
        }
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

    public void OnClickButton_CampaignPlay() {
        OpenPanel(campaignMenuPanel);
    }

    public void OnClickButton_BattlesPlay() {
        DataSceneTransitionController.GetInstance().ZeroSelectedShips();
        OpenPanel(battlesMenuPanel);
    }

    public void OnClickButton_OpenSettings() {
        OpenPanel(settingsMenuPanel);
    }

    public void OnClickButton_ShowAuthors() {
        OpenPanel(authorsPanel);
    }

    public void OnClickButton_BackToMainMenu() {
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
        });
    }

    public void OnClickButton_Exit() {
        Application.Quit();
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
