using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSceneTransitionController : MonoBehaviour
{
    public enum BattleMode {
        Classic,
        Advanced
    }

    public enum BattleType {
        P1vsBot,
        P1vsP2
    }

    private static DataSceneTransitionController Instance;
    private List<CellPointPos[]> firstPlayerSelectedShipPoints;
    private List<CellPointPos[]> secondPlayerSelectedShipPoints;
    private BattleMode battleMode;
    private BattleType battleType;

    private DataSceneTransitionController() { }

    private void Awake() {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ZeroSelectedShips() {
        if(firstPlayerSelectedShipPoints != null) {
            firstPlayerSelectedShipPoints = null;
        }
        if(secondPlayerSelectedShipPoints != null) {
            secondPlayerSelectedShipPoints = null;
        }
    }

    public static DataSceneTransitionController GetInstance() {
        return Instance;
    }

    public void SetSelectedShips(int playerNumber,List<CellPointPos[]> shipPoints) {
        if(playerNumber == 1) {
            firstPlayerSelectedShipPoints = shipPoints;
        } else {
            secondPlayerSelectedShipPoints = shipPoints;
        }
    }

    public void SetBattleMode(BattleMode selectedBattleMode) {
        battleMode = selectedBattleMode;
    }

    public void SetBattleType(BattleType selectedBattleType) {
        battleType = selectedBattleType;
    }

    public List<CellPointPos[]> GetSelectedShipPoints(int playerNumber) {
        if(playerNumber == 1) {
            return firstPlayerSelectedShipPoints;
        } else {
            return secondPlayerSelectedShipPoints;
        }
    }

    public int GetPlayerCountWithShips() {
        int selectedPlayersShipsCount = 0;
        if(firstPlayerSelectedShipPoints != null) {
            selectedPlayersShipsCount++;
        }
        if(secondPlayerSelectedShipPoints != null) {
            selectedPlayersShipsCount++;
        }
        return selectedPlayersShipsCount;
    }

    public BattleMode GetBattleMode() {
        return battleMode;
    }

    public BattleType GetBattleType() {
        return battleType;
    }
}
