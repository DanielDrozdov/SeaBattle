using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausePanelController : MonoBehaviour
{
    public void OnClickButton_ActivatePausePanel() {
        Time.timeScale = 0;
        gameObject.SetActive(true);
    }

    public void OnClickButton_ReturnToGame() {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }

    public void OnClickButton_Settings() {

    }

    public void OnClickButton_ToMainMenu() {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
}
