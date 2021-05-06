using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlesMenuPanelController : MonoBehaviour, ISecondaryMenuPanelDisableActions
{
    [SerializeField] private GameObject fightTypeSelectionMenu;
    [SerializeField] private GameObject fightModeSelectionMenu;
    [SerializeField] private GameObject botFightDifficultSelectionMenu;
    [SerializeField] private GameObject selecetedShipsFieldPanel;
    private MainMenuUIController mainMenuUIController;
    private DataSceneTransitionController dataSceneTransitionController;

    private void Start() {
        mainMenuUIController = MainMenuUIController.GetInstance();
        dataSceneTransitionController = DataSceneTransitionController.GetInstance();
    }


    private void OnDisable() {
        ResetPanelsActivatedState();
    }

    public void ResetPanelsActivatedState() {
        fightModeSelectionMenu.SetActive(true);
        fightTypeSelectionMenu.SetActive(false);
        botFightDifficultSelectionMenu.SetActive(false);
        if(selecetedShipsFieldPanel != null) {
            selecetedShipsFieldPanel.SetActive(false);
        }
    }

    public void OnClickButton_ClassicModeChoose() {
        dataSceneTransitionController.SetBattleMode(DataSceneTransitionController.BattleMode.Classic);
        CloseModeSelectionMenuAndOpenFightSelectionMenu();
    }

    public void OnClickButton_AdvancedModeChoose() {
        dataSceneTransitionController.SetBattleMode(DataSceneTransitionController.BattleMode.Advanced);
        CloseModeSelectionMenuAndOpenFightSelectionMenu();
    }

    public void OnClickButton_PlayWithBot() {
        dataSceneTransitionController.SetBattleType(DataSceneTransitionController.BattleType.P1vsBot);
        if(DataSceneTransitionController.GetInstance().GetBattleMode() == DataSceneTransitionController.BattleMode.Classic) {
            CloseFightSelectionMenuAndOpenSelectShipsFieldPanel();
        } else {
            CloseFightSelectionMenuAndOpenBotDifficultySelectionPanel();
        }
    }

    public void OnClickButton_PlayWithFriendOnCommonDevice() {
        dataSceneTransitionController.SetBattleType(DataSceneTransitionController.BattleType.P1vsP2);
        CloseFightSelectionMenuAndOpenSelectShipsFieldPanel();
    }

    public void OnClickButton_EasyBotDifficultyChoice() {
        dataSceneTransitionController.SetBotDifficulty(DataSceneTransitionController.BotDifficulty.Easy);
        CloseBotDifficultySelectionPanelAndOpenSelectShipFieldPanel();
    }

    public void OnClickButton_MediumBotDifficultyChoice() {
        dataSceneTransitionController.SetBotDifficulty(DataSceneTransitionController.BotDifficulty.Medium);
        CloseBotDifficultySelectionPanelAndOpenSelectShipFieldPanel();
    }

    public void OnClickButton_HardBotDifficultyChoice() {
        dataSceneTransitionController.SetBotDifficulty(DataSceneTransitionController.BotDifficulty.Hard);
        CloseBotDifficultySelectionPanelAndOpenSelectShipFieldPanel();
    }

    private void CloseBotDifficultySelectionPanelAndOpenSelectShipFieldPanel() {
        mainMenuUIController.ActivatePanelTransition(() => {
            botFightDifficultSelectionMenu.SetActive(false);
            selecetedShipsFieldPanel.SetActive(true);
        });
    }

    private void CloseFightSelectionMenuAndOpenSelectShipsFieldPanel() {
        mainMenuUIController.ActivatePanelTransition(() => {
            fightTypeSelectionMenu.SetActive(false);
            selecetedShipsFieldPanel.SetActive(true);
        });
    }

    private void CloseFightSelectionMenuAndOpenBotDifficultySelectionPanel() {
        mainMenuUIController.ActivatePanelTransition(() => {
            fightTypeSelectionMenu.SetActive(false);
            botFightDifficultSelectionMenu.SetActive(true);
        });
    }

    private void CloseModeSelectionMenuAndOpenFightSelectionMenu() {
        mainMenuUIController.ActivatePanelTransition(() => {
            fightModeSelectionMenu.SetActive(false);
            fightTypeSelectionMenu.SetActive(true);
        });
    }
}
