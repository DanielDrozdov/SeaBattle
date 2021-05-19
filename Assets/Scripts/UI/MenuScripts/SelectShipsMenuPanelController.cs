using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SelectShipsMenuPanelController : MonoBehaviour
{
    [HideInInspector] public int playerNumber => DataSceneTransitionController.GetInstance().GetPlayerCountWithShips() + 1;
    private bool IsPlayButtonPressed;
    [SerializeField] private GameObject SelectPlayerShipsPanel;
    private GeneratedSelectShipLocateHelperController generator;
    private SelectShipFieldController shipSelectFieldController;
    private LevelTransitionPanelController levelTransitionPanelController;

    private void Awake() {
        shipSelectFieldController = GetComponent<SelectShipFieldController>();
        generator = GetComponent<GeneratedSelectShipLocateHelperController>();
        levelTransitionPanelController = LevelTransitionPanelController.GetInstance();
    }

    public void OnClickButton_Play() {
        if(shipSelectFieldController.IfAllShipsAreSelected() && !IsPlayButtonPressed) {
            DataSceneTransitionController dataSceneTransition = DataSceneTransitionController.GetInstance();
            int playerNumber = dataSceneTransition.GetPlayerCountWithShips() + 1;         
            bool IsDone;
            if((dataSceneTransition.GetBattleType() == DataSceneTransitionController.BattleType.P1vsBot && playerNumber == 1) ||
                (dataSceneTransition.GetBattleType() == DataSceneTransitionController.BattleType.P1vsP2 && playerNumber == 2) ||
                (dataSceneTransition.GetBattleType() == DataSceneTransitionController.BattleType.P1vsP2 && playerNumber == 1 && dataSceneTransition.IsMultiplayerGame())) {
                IsDone = levelTransitionPanelController.MoveToCanvasCenter(() => {
                    if(!dataSceneTransition.IsMultiplayerGame()) {
                        SceneManager.LoadScene("FightScene");
                    } else {
                        WaitPlayerPanelController.GetInstance().OpenWaitPanel();
                    }
                });
            } else {
                IsDone = MainMenuUIController.GetInstance().ActivatePanelTransition(() => {
                    SelectPlayerShipsPanel.SetActive(false);
                    SelectPlayerShipsPanel.SetActive(true);
                    IsPlayButtonPressed = false;
                });
            }
            if(IsDone) {
                dataSceneTransition.SetSelectedShips(playerNumber, shipSelectFieldController.GetSelectedShips());
                IsPlayButtonPressed = true;
            }
        }
    }

    public void OnClickButton_AutoShipsLocateGeneration() {
        generator.LocateShipsOnField(shipSelectFieldController);
    }

    public void OnClickButton_RotateShip() {
        shipSelectFieldController.RotateShip();
    }
}
