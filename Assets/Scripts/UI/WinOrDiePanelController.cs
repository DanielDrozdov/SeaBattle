using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinOrDiePanelController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI opponentNameText;

    public void ActivatePanel(FightGameManager.OpponentName winnerName) {
        gameObject.SetActive(true);
        if(winnerName == FightGameManager.OpponentName.Bot) {
            opponentNameText.text = "You lose";
        } else {
            opponentNameText.text = winnerName.ToString() + " player win!";
        }
    }

    public void OnClickButton_ToMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }
}
