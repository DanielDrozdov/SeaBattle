﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FightFieldStateController : MonoBehaviour, IPointerUpHandler, IPointerDownHandler {

    [SerializeField] private bool IsSelfField;
    [SerializeField] private Ship[] shipsMassive;

    private Dictionary<char, Dictionary<int, Vector2>> fieldPointsToHit;
    private Dictionary<char, Dictionary<int, Vector2>> fieldPoints;
    private Dictionary<char, float> lettersYPos;
    private List<Ship> shipsList;
    private char[] fieldLettersMassive = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j' };

    private Camera mainCamera;
    private SpritesFightPoolController spritesFightPoolController;

    private float[] bordersMassive;

    private FightFieldStateController() { }

    private void Awake() {
        shipsList = new List<Ship>();

        for(int i = 0;i < shipsMassive.Length;i++) {
            shipsList.Add(shipsMassive[i]);
        }
    }

    private void Start() {
        mainCamera = ServiceManager.GetInstance().GetMainCamera();
        CalculateFieldPoints();
        spritesFightPoolController = SpritesFightPoolController.GetInstance();
    }

    public void OnPointerUp(PointerEventData eventData) {
        Vector2 tapPos = mainCamera.ScreenToWorldPoint(eventData.position);
        if(!IsSelfField) {
            CalculateTapEnemyCellData(tapPos);
        }
    }

    public void OnPointerDown(PointerEventData eventData) {

    }

    public float[] GetFieldBorders() {
        return bordersMassive;
    }

    public Vector2 GetNearestCellPos(Vector2 tapPos) {
        CellPointPos tapCellPoint = SearchTapCellData(tapPos, fieldPoints);
        Dictionary<int,Vector2> letterPoints = fieldPoints[tapCellPoint.letter];
        return letterPoints[tapCellPoint.number];
    }

    private void CalculateFieldPoints() {
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
        bordersMassive = new float[] { xMin,xMax,yMin,yMax };
        fieldXSize = Mathf.Abs(xMax - xMin);
        float cellSizeDelta = fieldXSize / 10;
        for(int i = 0; i < 10; i++) {
            float cellsYPos = yMax - (cellSizeDelta * i + cellSizeDelta / 2);
            Dictionary<int, Vector2> letterPoints = new Dictionary<int, Vector2>();
            for(int k = 0; k < 10; k++) {
                letterPoints.Add(k + 1, new Vector2(cellSizeDelta * k + cellSizeDelta / 2 + xMin, cellsYPos));
            }
            fieldPointsToHit.Add(fieldLettersMassive[i], letterPoints);
            fieldPoints.Add(fieldLettersMassive[i], letterPoints);
            lettersYPos.Add(fieldLettersMassive[i], cellsYPos);
        }
    }

    private void CalculateTapEnemyCellData(Vector2 tapPosition) {
        CellPointPos tapCellData = SearchTapCellData(tapPosition,fieldPointsToHit);
        Dictionary<int, Vector2> letterPoints = fieldPointsToHit[tapCellData.letter];
        if(tapCellData.number != 0) {
            Vector2 tapCellPosition = letterPoints[tapCellData.number];
            letterPoints.Remove(tapCellData.number);
            bool IsHit = CheckIsShipHit(new CellPointPos(tapCellData.letter, tapCellData.number));
            if(IsHit) {
                ActivateCellStateSprite(tapCellPosition, true);
            } else {
                ActivateCellStateSprite(tapCellPosition, false);
            }
        }
    }

    private CellPointPos SearchTapCellData(Vector2 tapPosition, Dictionary<char, Dictionary<int, Vector2>> fieldPointsDict) {
        char tapCellLetter = ' ';
        int tapCellNumber = 0;
        Dictionary<int, Vector2> letterPoints;
        for(int i = 0;i < fieldLettersMassive.Length;i++) {
            if(Mathf.Abs(tapPosition.y - lettersYPos[fieldLettersMassive[i]]) < 0.5f) {
                tapCellLetter = fieldLettersMassive[i];
            }
        }

        //foreach(char yPosLetter in lettersYPos.Keys) {
        //    if(Mathf.Abs(tapPosition.y - lettersYPos[yPosLetter]) < 0.5f) {
        //        tapCellLetter = yPosLetter;
        //    }
        //}
        letterPoints = fieldPointsDict[tapCellLetter];
        foreach(int cellNumber in letterPoints.Keys) {
            if(Mathf.Abs(tapPosition.x - letterPoints[cellNumber].x) < 0.5f) {
                tapCellNumber = cellNumber;
            }
        }
        if(tapCellNumber == 0) {
            foreach(int cellNumber in letterPoints.Keys) {
                Debug.Log(letterPoints[cellNumber].x + "  " + cellNumber); // Приходит 0 с массива по выстрелам, а не клеткам
            }
            Debug.Log(tapPosition.x);
        }

        return new CellPointPos(tapCellLetter, tapCellNumber);
    }

    private bool CheckIsShipHit(CellPointPos tapCell) {
        for(int i = 0; i < shipsList.Count; i++) {
            List<CellPointPos> curShipPoints = shipsList[i].shipHitPoints;
            for(int k = 0; k < curShipPoints.Count; k++) {
                if((curShipPoints[k].letter == tapCell.letter && curShipPoints[k].number == tapCell.number)) {
                    if(!IsSelfField) {
                        curShipPoints.RemoveAt(k);
                        if(curShipPoints.Count == 0) {
                            HitShip(shipsList[i].shipPoints);
                        }
                    }
                    return true;
                }
            }
        }
        return false;
    }

    private void ActivateCellStateSprite(Vector2 cellPos,bool IsShipHit) {
        GameObject sprite;
        if(IsShipHit) {
            sprite = spritesFightPoolController.GetHitCrossSprite();
        } else {
            sprite = spritesFightPoolController.GetNothingInCellSprite();
        }
        sprite.transform.position = cellPos;
        sprite.SetActive(true);
    }

    private void HitShip(List<CellPointPos> shipPoints) {
        int[] borderData = CalculateHitShipCellsBorder(shipPoints);
        int minLetter = borderData[0], maxLetter = borderData[1];
        int minNumber = borderData[2], maxNumber = borderData[3];
        for(int i = minLetter; i <= maxLetter; i++) {
            char letter = (char)i;
            for(int k = 1; k <= 10; k++) {
                if(fieldPointsToHit[letter].ContainsKey(k) && k >= minNumber && k <= maxNumber) {
                    ActivateCellStateSprite(fieldPointsToHit[letter][k], false);
                    fieldPointsToHit[letter].Remove(k);
                }
            }
        }
    }

    private int[] CalculateHitShipCellsBorder(List<CellPointPos> shipPoints) {
        int minNumber = shipPoints[0].number, maxNumber = shipPoints[0].number;
        char minLetter = shipPoints[0].letter, maxLetter = shipPoints[0].letter;
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
struct CellPointPos {
    public char letter;
    public int number;
    public CellPointPos(char letter, int number) {
        this.letter = letter;
        this.number = number;
    }
}