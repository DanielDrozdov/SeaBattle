using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectShipsMenuPanelController : MonoBehaviour
{
    private bool IsPlayButtonPressed;
    [SerializeField] private GeneratedSelectShipLocateHelperController generator;
    private SelectShipFieldController shipSelectFieldController;
    private LevelTransitionPanelController levelTransitionPanelController;

    private void Awake() {
        shipSelectFieldController = GetComponent<SelectShipFieldController>();
        levelTransitionPanelController = LevelTransitionPanelController.GetInstance();
    }

    public void OnClickButton_Play() {
        if(shipSelectFieldController.IfAllShipsAreSelected() && !IsPlayButtonPressed) {
            DataSceneTransitionController dataSceneTransition = DataSceneTransitionController.GetInstance();
            dataSceneTransition.SetSelectedShips(shipSelectFieldController.GetSelectedShips());
            bool IsDone = levelTransitionPanelController.MoveToCanvasCenter(() => {
                SceneManager.LoadScene("FightScene");
            });
            if(IsDone) {
                IsPlayButtonPressed = true;
            }
        }
    }

    public void OnClickButton_AutoShipsLocateGeneration() {
        generator.LocateShipsOnField();
    }

    public void OnClickButton_RotateShip() {
        shipSelectFieldController.RotateShip();
    }

    private IEnumerator WaitLevelTransitionAnimCoroutine() {
        bool IsAnimPlaying = true;
        while(IsAnimPlaying) {
            if(levelTransitionPanelController.IsPanelInScreenCenter()) {
                IsAnimPlaying = false;
                
                yield return new WaitForSeconds(0.5f);
            }
            yield return null;
        }
        levelTransitionPanelController.MoveToStartPoint();
    }
}
