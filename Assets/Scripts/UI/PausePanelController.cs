using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PausePanelController : MonoBehaviour
{
    [SerializeField] private Image pauseButtonImage;
    [SerializeField] private Sprite pauseButtonOffSprite;
    [SerializeField] private Sprite pauseButtonOnSprite;


    public void OnClickButton_ActivatePausePanel() {
        if(gameObject.activeSelf) {
            return;
        }
        GameUIController.GetInstance().OffOrOnBaseUIWithoutGamePauseButton(false);
        Time.timeScale = 0;
        pauseButtonImage.sprite = pauseButtonOnSprite;
        gameObject.SetActive(true);
    }

    public void OnClickButton_ReturnToGame() {
        GameUIController.GetInstance().OffOrOnBaseUIWithoutGamePauseButton(true);
        Time.timeScale = 1;
        pauseButtonImage.sprite = pauseButtonOffSprite;
        gameObject.SetActive(false);
    }

    public void OnClickButton_Settings() {

    }

    public void OnClickButton_ToMainMenu() {
        Time.timeScale = 1;
        LevelTransitionPanelController.GetInstance().MoveToCanvasCenter(() => SceneManager.LoadScene("MainMenu"));
    }
}
