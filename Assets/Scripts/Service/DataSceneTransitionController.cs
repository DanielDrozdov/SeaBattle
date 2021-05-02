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
    private SelectedMissionData missionData;
    private bool IsCampaign;

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

    public void SetSelectedMissionData(SelectedMissionData selectedMissionData) {
        missionData = selectedMissionData;
    }

    public void SetCampaignGame(bool value) {
        IsCampaign = value;
    }

    public bool IsCampaignGame() {
        return IsCampaign;
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

    public SelectedMissionData GetSelectedMissionData() {
        return missionData;
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

[System.Serializable]
public class SelectedMissionData {
    [SerializeField] private int playerFieldSizeInCells;
    [SerializeField] private int enemyFieldSizeInCells;

    [Header("PlayerShipsTypeCount")]
    [SerializeField] private OpponentShipsTypeCountInMission playerShips;

    [Header("EnemyShipsTypeCount")]
    [SerializeField] private OpponentShipsTypeCountInMission enemyShips;

    public SelectedMissionData(int playerFieldSizeInCells,int enemyFieldSizeInCells) {
        this.playerFieldSizeInCells = playerFieldSizeInCells;
        this.enemyFieldSizeInCells = enemyFieldSizeInCells;
    }

    public int GetPlayerFieldSize() {
        return playerFieldSizeInCells;
    }

    public int GetEnemyFieldSize() {
        return enemyFieldSizeInCells;
    }

    public OpponentShipsTypeCountInMission GetPlayerShipsCount() {
        return playerShips;
    }

    public OpponentShipsTypeCountInMission GetEnemyShipsCount() {
        return enemyShips;
    }
}

[System.Serializable]
public struct OpponentShipsTypeCountInMission {
    public int fourCellShipsCount;
    public int threeCellShipsCount;
    public int twoCellShipsCount;
    public int oneCellShipsCount;
}
