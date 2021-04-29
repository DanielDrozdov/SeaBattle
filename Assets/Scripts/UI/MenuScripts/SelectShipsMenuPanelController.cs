using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectShipsMenuPanelController : MonoBehaviour
{
    private bool IsPlayButtonPressed;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private GameObject SelectPlayerShipsPanel;
    private GeneratedSelectShipLocateHelperController generator;
    private SelectShipFieldController shipSelectFieldController;
    private LevelTransitionPanelController levelTransitionPanelController;

    private void Awake() {
        shipSelectFieldController = GetComponent<SelectShipFieldController>();
        generator = GetComponent<GeneratedSelectShipLocateHelperController>();
        levelTransitionPanelController = LevelTransitionPanelController.GetInstance();
    }

    private void Start() {
        UpdatePlayerNameText();
    }

    private void OnEnable() {
        UpdatePlayerNameText();
    }

    public void OnClickButton_Play() {
        if(shipSelectFieldController.IfAllShipsAreSelected() && !IsPlayButtonPressed) {
            DataSceneTransitionController dataSceneTransition = DataSceneTransitionController.GetInstance();
            int playerNumber = dataSceneTransition.GetPlayerCountWithShips() + 1;         
            bool IsDone;
            if((dataSceneTransition.GetBattleType() == DataSceneTransitionController.BattleType.P1vsBot && playerNumber == 1) ||
                (dataSceneTransition.GetBattleType() == DataSceneTransitionController.BattleType.P1vsP2 && playerNumber == 2)) {
                IsDone = levelTransitionPanelController.MoveToCanvasCenter(() => {
                    SceneManager.LoadScene("FightScene");
                });
            } else {
                IsDone = MainMenuUIController.GetInstance().ActivatePanelTransition(() => {
                    SelectPlayerShipsPanel.SetActive(false);
                    SelectPlayerShipsPanel.SetActive(true);
                    UpdatePlayerNameText();
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

    private void UpdatePlayerNameText() {
        int playerNumber = DataSceneTransitionController.GetInstance().GetPlayerCountWithShips() + 1;
        playerNameText.text = "Корабли игрока P" + playerNumber;
    }
}
