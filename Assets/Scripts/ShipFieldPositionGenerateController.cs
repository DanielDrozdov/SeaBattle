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

    private void Awake() {
        Instance = this;
        shipsPointsList = new List<CellPointPos[]>(10);
        FillCellPointsDict();
        FillShipsCountForSizeDict();
        GenerateRandomShipCellPoints();
    }

    public static ShipFieldPositionGenerateController GetInstance() {
        return Instance;
    }

    public List<CellPointPos[]> GetGeneratedShipsPoints() {
        shipsPointsList.Clear();
        GenerateRandomShipCellPoints();
        return shipsPointsList;
    }

    private void FillCellPointsDict() {
        cellPoints = new Dictionary<char, int[]>();
        for(int i = 0; i < 10; i++) {
            char letter = fieldLettersMassive[i];
            int[] numbers = new int[10];
            for(int k = 1; k <= 10; k++) {
                numbers[k - 1] = k;
            }
            cellPoints.Add(letter, numbers);
        }
    }

    private void FillShipsCountForSizeDict() {
        shipsCountForSize = new Dictionary<int, int>();
        int shipsCount = 1;
        for(int i = 4; i > 0; i--) {
            shipsCountForSize.Add(i, shipsCount++);
        }
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
        bool IsShipRotatedOnY = GetShipRandomRotation();
        int startLetter;
        int startNumber;
        if(IsShipRotatedOnY) {
            startLetter = Random.Range(shipSizeInCells - 1, fieldLettersMassive.Length);
            startNumber = Random.Range(1, 11);
        } else {
            startLetter = Random.Range(0, fieldLettersMassive.Length);
            startNumber = Random.Range(1, 10 - shipSizeInCells);
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
