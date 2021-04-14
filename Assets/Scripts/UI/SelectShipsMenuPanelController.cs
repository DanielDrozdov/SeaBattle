using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectShipsMenuPanelController : MonoBehaviour
{
    private ShipSelectFieldController shipSelectFieldController;
    private LevelTransitionPanelController levelTransitionPanelController;

    private void Awake() {
        shipSelectFieldController = GetComponent<ShipSelectFieldController>();
        levelTransitionPanelController = LevelTransitionPanelController.GetInstance();
    }

    public void OnClickButton_Play() {
        if(shipSelectFieldController.IfAllShipsAreSelected()) {
            DataSceneTransitionController dataSceneTransition = DataSceneTransitionController.GetInstance();
            dataSceneTransition.SetSelectedShips(shipSelectFieldController.GetSelectedShips());
            levelTransitionPanelController.MoveToCanvasCenter(() => {
                SceneManager.LoadScene("FightScene");
            });
        }
    }

    public void OnClickButton_AutoShipsLocateGeneration() {

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
