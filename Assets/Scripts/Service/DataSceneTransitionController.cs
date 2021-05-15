using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public enum BotDifficulty {
        Easy,
        Medium,
        Hard
    }

    public bool IsNeedLoadNextMission;


    private static DataSceneTransitionController Instance;
    private List<CellPointPos[]> firstPlayerSelectedShipPoints;
    private List<CellPointPos[]> secondPlayerSelectedShipPoints;
    private BattleMode battleMode;
    private BattleType battleType;
    private BotDifficulty botDifficult;
    private SelectedMissionData missionData;
    private bool IsCampaign;
    private bool IsMutliplayer;

    private DataSceneTransitionController() { }

    private void Awake() {
        DataSceneTransitionController[] instancesCount = FindObjectsOfType<DataSceneTransitionController>();
        for(int i = 0;i < instancesCount.Length;i++) {
            if(instancesCount[i].gameObject.scene.buildIndex == -1) {
                if(gameObject.scene.buildIndex != -1) {
                    Destroy(gameObject);
                    return;
                }
            }
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
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

    public void SetMultiplayerStateGame(bool value) {
        IsMutliplayer = value;
    }

    public bool IsMultiplayerGame() {
        return IsMutliplayer;
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

    public void SetBotDifficulty(BotDifficulty botDifficulty) {
        botDifficult = botDifficulty;
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

    public BotDifficulty GetBotDifficult() {
        return botDifficult;
    }
}

[System.Serializable]
public class SelectedMissionData {
    [HideInInspector] public int missionNumber;
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
