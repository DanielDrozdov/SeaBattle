using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlesMenuPanelController : MonoBehaviour, ISecondaryMenuPanelDisableActions
{
    [SerializeField] private GameObject fightTypeSelectionMenu;
    [SerializeField] private GameObject fightModeSelectionMenu;
    [SerializeField] private GameObject selecetedShipsFieldPanel;
    private MainMenuUIController mainMenuUIController;

    private void Start() {
        mainMenuUIController = MainMenuUIController.GetInstance();
    }


    private void OnDisable() {
        ResetPanelsActivatedState();
    }

    public void ResetPanelsActivatedState() {
        fightModeSelectionMenu.SetActive(true);
        fightTypeSelectionMenu.SetActive(false);
        if(selecetedShipsFieldPanel != null) {
            selecetedShipsFieldPanel.SetActive(false);
        }
    }

    public void OnClickButton_ClassicModeChoose() {
        DataSceneTransitionController.GetInstance().SetBattleMode(DataSceneTransitionController.BattleMode.Classic);
        CloseModeSelectionMenuAndOpenFightSelectionMenu();
    }

    public void OnClickButton_AdvancedModeChoose() {
        DataSceneTransitionController.GetInstance().SetBattleMode(DataSceneTransitionController.BattleMode.Advanced);
        CloseModeSelectionMenuAndOpenFightSelectionMenu();
    }

    public void OnClickButton_PlayWithBot() {
        DataSceneTransitionController.GetInstance().SetBattleType(DataSceneTransitionController.BattleType.P1vsBot);
        CloseFightSelectionMenuAndOpenSelectShipsFieldPanel();
    }

    public void OnClickButton_PlayWithFriendOnCommonDevice() {
        DataSceneTransitionController.GetInstance().SetBattleType(DataSceneTransitionController.BattleType.P1vsP2);
        CloseFightSelectionMenuAndOpenSelectShipsFieldPanel();
    }

    private void CloseFightSelectionMenuAndOpenSelectShipsFieldPanel() {
        mainMenuUIController.ActivatePanelTransition(() => {
            fightTypeSelectionMenu.SetActive(false);
            selecetedShipsFieldPanel.SetActive(true);
        });
    }

    private void CloseModeSelectionMenuAndOpenFightSelectionMenu() {
        mainMenuUIController.ActivatePanelTransition(() => {
            fightModeSelectionMenu.SetActive(false);
            fightTypeSelectionMenu.SetActive(true);
        });
    }
}
