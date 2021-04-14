using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUIController : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject selecetedShipsFieldPanel;
    private LevelTransitionPanelController levelTransitionPanelController;

    private void Start() {
        levelTransitionPanelController = LevelTransitionPanelController.GetInstance();
    }

    public void OnClickButton_PlayWithBot() {
        levelTransitionPanelController.MoveToCanvasCenter(() => {
            mainMenuPanel.SetActive(false);
            selecetedShipsFieldPanel.SetActive(true);
        });
        StartCoroutine(WaitLevelTransitionAnimCoroutine());
    }

    public void OnClickButton_Exit() {
        Application.Quit();
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
