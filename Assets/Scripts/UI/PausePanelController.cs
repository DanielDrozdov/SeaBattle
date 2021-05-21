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

    private static PausePanelController Instance;

    private void Awake() {
        Instance = this;
        gameObject.SetActive(false);
    }

    public static PausePanelController GetInstance() {
        return Instance;
    }

    public void OnClickButton_ActivatePausePanel() {
        ActivatePausePanel();
    }

    public void OnClickButton_ReturnToGame() {
        ReturnToGame();
    }

    public void OnClickButton_Settings() {

    }

    public void OnClickButton_ToMainMenu() {
        Time.timeScale = 1;
        LevelTransitionPanelController.GetInstance().MoveToCanvasCenter(() => SceneManager.LoadScene("MainMenu"));
        NetworkHelpManager.GetInstance().CancelConnecting();
    }

    public void ActivatePausePanel() {
        if(gameObject.activeSelf) {
            return;
        }
        if(DataSceneTransitionController.GetInstance().IsMultiplayerGame()) {
            NetworkHelpManager networkHelpManager = NetworkHelpManager.GetInstance();
            networkHelpManager.GetCurrentPlayerFightNetworkController().SetStateToPausePanel(true);
        }
        GameUIController.GetInstance().OffOrOnBaseUIWithoutGamePauseButton(false);
        Time.timeScale = 0;
        pauseButtonImage.sprite = pauseButtonOnSprite;
        gameObject.SetActive(true);
    }

    public void ReturnToGame() {
        if(!gameObject.activeSelf) {
            return;
        }
        if(DataSceneTransitionController.GetInstance().IsMultiplayerGame()) {
            NetworkHelpManager networkHelpManager = NetworkHelpManager.GetInstance();
            networkHelpManager.GetCurrentPlayerFightNetworkController().SetStateToPausePanel(false);
        }
        GameUIController.GetInstance().OffOrOnBaseUIWithoutGamePauseButton(true);
        Time.timeScale = 1;
        pauseButtonImage.sprite = pauseButtonOffSprite;
        gameObject.SetActive(false);
    }
}
