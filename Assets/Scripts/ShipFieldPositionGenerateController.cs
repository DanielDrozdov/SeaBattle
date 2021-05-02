using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipFieldPositionGenerateController : MonoBehaviour
{
    private static ShipFieldPositionGenerateController Instance;

    private Dictionary<int, int> shipsCountForSize;
    private Dictionary<char, int[]> cellPoints;
    private char[] fieldLettersMassive = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j' };
    private List<CellPointPos[]> shipsPointsList;

    private ShipFieldPositionGenerateController() { }
    private int fieldSize;

    private void Awake() {
        Instance = this;
        shipsPointsList = new List<CellPointPos[]>();
    }

    public static ShipFieldPositionGenerateController GetInstance() {
        return Instance;
    }

    public List<CellPointPos[]> GetGeneratedShipsPoints(bool IsBot) {
        shipsPointsList.Clear();
        if(!DataSceneTransitionController.GetInstance().IsCampaignGame()) {
            FillShipsCountForSizeDict();
            fieldSize = 10;
        } else {
            FillCampaignShipsCountForSizeDict(IsBot);
        }
        GenerateRandomShipCellPoints();
        return shipsPointsList;
    }

    private void FillShipsCountForSizeDict() {
        shipsCountForSize = new Dictionary<int, int>();
        int shipsCount = 1;
        for(int i = 4; i > 0; i--) {
            shipsCountForSize.Add(i, shipsCount++);
        }
    }

    private void FillCampaignShipsCountForSizeDict(bool IsBot) {
        shipsCountForSize = new Dictionary<int, int>();
        SelectedMissionData missionData = DataSceneTransitionController.GetInstance().GetSelectedMissionData();
        OpponentShipsTypeCountInMission shipsCount;
        if(IsBot) {
            shipsCount = missionData.GetEnemyShipsCount();
            fieldSize = missionData.GetEnemyFieldSize();
        } else {
            shipsCount = missionData.GetPlayerShipsCount();
            fieldSize = missionData.GetPlayerFieldSize();
        }
        shipsCountForSize.Add(1, shipsCount.oneCellShipsCount);
        shipsCountForSize.Add(2, shipsCount.twoCellShipsCount);
        shipsCountForSize.Add(3, shipsCount.threeCellShipsCount);
        shipsCountForSize.Add(4, shipsCount.fourCellShipsCount);
    }

    private void GenerateRandomShipCellPoints() {
        for(int i = 4; i > 0; i--) {
            int shipsCount = shipsCountForSize[i];
            for(int k = 1; k <= shipsCount;k++) {
                CellPointPos[] shipPoints = GetRandomShipPoints(i);
                shipsPointsList.Add(shipPoints);
            }
        }
    }

    private CellPointPos[] GetRandomShipPoints(int shipSizeInCells) {
        CellPointPos[] shipPoints = new CellPointPos[shipSizeInCells];
        int fieldCutSize = 10 - fieldSize;
        bool IsShipRotatedOnY = GetShipRandomRotation();
        int startLetter;
        int startNumber;
        if(IsShipRotatedOnY) {
            startLetter = Random.Range(shipSizeInCells - 1, fieldLettersMassive.Length - fieldCutSize);
            startNumber = Random.Range(1, 11 - fieldCutSize);
        } else {
            startLetter = Random.Range(0, fieldLettersMassive.Length - fieldCutSize);
            startNumber = Random.Range(1, (10 - fieldCutSize) - shipSizeInCells);
        }
        shipPoints[0].letter = fieldLettersMassive[startLetter];
        shipPoints[0].number = startNumber;
        for(int i = 1; i < shipSizeInCells; i++) {
            if(!IsShipRotatedOnY) {
                shipPoints[i].letter = fieldLettersMassive[startLetter];
                shipPoints[i].number = startNumber + i;
            } else {
                shipPoints[i].letter = fieldLettersMassive[startLetter - i];
                shipPoints[i].number = startNumber;
            }
        }
        if(!IfCanLocateShip(shipPoints)) {
            shipPoints = GetRandomShipPoints(shipSizeInCells);
        }
        return shipPoints;
    }

    private bool IfCanLocateShip(CellPointPos[] shipPoints) {
        for(int i = 0;i < shipsPointsList.Count;i++) {
            for(int b = 0; b < shipPoints.Length; b++) {
                CellPointPos[] reservedShipPoints = shipsPointsList[i];
                for(int k = 0; k < reservedShipPoints.Length; k++) {
                    int charDelta = Mathf.Abs(shipPoints[b].letter - reservedShipPoints[k].letter);
                    int numDelta = Mathf.Abs(shipPoints[b].number - reservedShipPoints[k].number);
                    if(charDelta <= 1 && numDelta <= 1) {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    private bool GetShipRandomRotation() {
        int boolByte = Random.Range(0, 2);
        return boolByte == 0 ? false : true;
    }
}
