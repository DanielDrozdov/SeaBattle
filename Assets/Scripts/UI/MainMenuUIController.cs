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
        levelTransitionPanelController.MoveToCanvasCenter();
        StartCoroutine(WaitLevelTransitionAnimCoroutine());
    }

    private IEnumerator WaitLevelTransitionAnimCoroutine() {
        bool IsAnimPlaying = true;
        while(IsAnimPlaying) {
            if(levelTransitionPanelController.IsPanelInScreenCenter()) {
                IsAnimPlaying = false;
                mainMenuPanel.SetActive(false);
                selecetedShipsFieldPanel.SetActive(true);
                yield return new WaitForSeconds(0.5f);
            }
            yield return null;
        }
        levelTransitionPanelController.MoveToStartPoint();
    }
}
