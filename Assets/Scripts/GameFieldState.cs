using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFieldState : MonoBehaviour {

    protected Dictionary<char, Dictionary<int, Vector2>> fieldPointsToHit;
    protected Dictionary<char, Dictionary<int, Vector2>> fieldPoints;
    protected Dictionary<char, float> lettersYPos;
    public readonly char[] fieldLettersMassive = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j' };

    protected List<Ship> shipsList;
    protected float[] bordersMassive;

    private void Awake() {    
        CalculateFieldPointsAndBorders();
        shipsList = new List<Ship>();
        AddAwakeActions();
    }

    private void Start() {
        AddStartActions();
    }

    protected virtual void AddStartActions() { }

    protected virtual void AddAwakeActions() { }

    public float[] GetFieldBorders() {
        return bordersMassive;
    }

    public Vector2 GetNearestCellPos(Vector2 tapPos) {
        CellPointPos tapCellPoint = SearchTapCellData(tapPos, fieldPoints);
        Dictionary<int, Vector2> letterPoints = fieldPoints[tapCellPoint.letter];
        return letterPoints[tapCellPoint.number];
    }

    public Vector2 GetPosByCellPoint(CellPointPos shipPoint) {
        return fieldPoints[shipPoint.letter][shipPoint.number];
    }

    protected void CalculateFieldPointsAndBorders() {
        fieldPointsToHit = new Dictionary<char, Dictionary<int, Vector2>>();
        fieldPoints = new Dictionary<char, Dictionary<int, Vector2>>();
        lettersYPos = new Dictionary<char, float>();
        float xMin, xMax, yMax, yMin;
        float fieldXSize;
        RectTransform rect = GetComponent<RectTransform>();
        xMin = transform.position.x - rect.sizeDelta.x / 2;
        xMax = transform.position.x + rect.sizeDelta.x / 2;
        yMin = transform.position.y - rect.sizeDelta.y / 2;
        yMax = transform.position.y + rect.sizeDelta.y / 2;
        bordersMassive = new float[] { xMin, xMax, yMin, yMax };
        fieldXSize = Mathf.Abs(xMax - xMin);
        float cellSizeDelta = fieldXSize / 10;
        for(int i = 0; i < 10; i++) {
            float cellsYPos = yMax - (cellSizeDelta * i + cellSizeDelta / 2);
            Dictionary<int, Vector2> letterPointsToHit = new Dictionary<int, Vector2>();
            Dictionary<int, Vector2> letterPoints = new Dictionary<int, Vector2>();
            for(int k = 0; k < 10; k++) {
                letterPointsToHit.Add(k + 1, new Vector2(cellSizeDelta * k + cellSizeDelta / 2 + xMin, cellsYPos));
                letterPoints.Add(k + 1, new Vector2(cellSizeDelta * k + cellSizeDelta / 2 + xMin, cellsYPos));
            }
            fieldPointsToHit.Add(fieldLettersMassive[i], letterPointsToHit);
            fieldPoints.Add(fieldLettersMassive[i], letterPoints);
            lettersYPos.Add(fieldLettersMassive[i], cellsYPos);
        }
    }

    protected CellPointPos SearchTapCellData(Vector2 tapPosition, Dictionary<char, Dictionary<int, Vector2>> fieldPointsDict) {
        char tapCellLetter = ' ';
        int tapCellNumber = 0;
        Dictionary<int, Vector2> letterPoints;
        if(tapPosition.x < bordersMassive[0] || tapPosition.x > bordersMassive[1]
            || tapPosition.y < bordersMassive[2] || tapPosition.y > bordersMassive[3]) {
            return new CellPointPos('a', -1);
        }

        foreach(char yPosLetter in lettersYPos.Keys) {
            if(Mathf.Abs(tapPosition.y - lettersYPos[yPosLetter]) <= 0.5f) {
                tapCellLetter = yPosLetter;
            }
        }
        letterPoints = fieldPointsDict[tapCellLetter];
        foreach(int cellNumber in letterPoints.Keys) {
            if(Mathf.Abs(tapPosition.x - letterPoints[cellNumber].x) <= 0.5f) {
                tapCellNumber = cellNumber;
            }
        }
        return new CellPointPos(tapCellLetter, tapCellNumber);
    }

    protected int[] CalculateShipCellsBorder(List<CellPointPos> shipPoints) {
        char minLetter = shipPoints[0].letter, maxLetter = shipPoints[0].letter;
        int minNumber = shipPoints[0].number, maxNumber = shipPoints[0].number;
        for(int i = 0; i < shipPoints.Count; i++) {
            char letter = shipPoints[i].letter;
            int number = shipPoints[i].number;
            if(letter > maxLetter) {
                maxLetter = letter;
            } else if(minLetter > letter) {
                minLetter = letter;
            }

            if(number > maxNumber) {
                maxNumber = number;
            } else if(minNumber > number) {
                minNumber = number;
            }
        }
        minNumber = Mathf.Clamp(minNumber - 1, 1, 10);
        maxNumber = Mathf.Clamp(maxNumber + 1, 1, 10);
        minLetter = (char)Mathf.Clamp(minLetter - 1, 97, 106); // 'a' byte code = 97, 'j' = 106
        maxLetter = (char)Mathf.Clamp(maxLetter + 1, 97, 106);
        int[] borderDataMassive = { minLetter, maxLetter, minNumber, maxNumber };
        return borderDataMassive;
    }
}

[System.Serializable]
public struct CellPointPos {
    public char letter;
    public int number;
    public CellPointPos(char letter, int number) {
        this.letter = letter;
        this.number = number;
    }
}
