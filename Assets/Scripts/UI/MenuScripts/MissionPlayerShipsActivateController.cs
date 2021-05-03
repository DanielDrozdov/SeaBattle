using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionPlayerShipsActivateController : MonoBehaviour
{
    [SerializeField] private SelectShipController[] ships;
    [SerializeField] private GeneratedSelectShipLocateHelperController GeneratedSelectShipLocateHelperController; 
    private static MissionPlayerShipsActivateController Instance;
    private Dictionary<int,int> shipsCountBySize;
    private List<SelectShipController> activatedToMissionShips;

    private void Awake() {
        Instance = this;
    }

    private void OnEnable() {
        SetPlayerShipsCountToDict();
        activatedToMissionShips = new List<SelectShipController>();
        for(int i = 0; i < ships.Length; i++) {
            if(shipsCountBySize[ships[i].shipSizeInCells] > 0) {
                ships[i].gameObject.SetActive(true);
                activatedToMissionShips.Add(ships[i]);
                shipsCountBySize[ships[i].shipSizeInCells]--;
            } else {
                ships[i].gameObject.SetActive(false);
            }
        }
        SetShipsToFieldAutoGenerator();
    }

    public static MissionPlayerShipsActivateController GetInstance() {
        return Instance;
    }

    public int GetShipsCountInGame() {
        return activatedToMissionShips.Count;
    }

    private void SetPlayerShipsCountToDict() {
        shipsCountBySize = new Dictionary<int, int>();
        SelectedMissionData missionData = DataSceneTransitionController.GetInstance().GetSelectedMissionData();
        OpponentShipsTypeCountInMission playerShipsCount = missionData.GetPlayerShipsCount();
        shipsCountBySize.Add(1, playerShipsCount.oneCellShipsCount);
        shipsCountBySize.Add(2, playerShipsCount.twoCellShipsCount);
        shipsCountBySize.Add(3, playerShipsCount.threeCellShipsCount);
        shipsCountBySize.Add(4, playerShipsCount.fourCellShipsCount);
    }

    private void SetShipsToFieldAutoGenerator() {
        SelectShipController[] ships = new SelectShipController[activatedToMissionShips.Count];
        for(int i = 0;i < activatedToMissionShips.Count;i++) {
            ships[i] = activatedToMissionShips[i];
        }
        GeneratedSelectShipLocateHelperController.SetActivatedShips(ships);
    }
}
