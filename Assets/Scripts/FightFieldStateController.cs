using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FightFieldStateController : GameFieldState {

    [SerializeField] private FightGameManager.OpponentName opponentName;
    private SpritesFightPoolController spritesFightPoolController;

    private Ship[] shipsMassive;

    private FightFieldStateController() { }

    internal override void AddStartActions() {
        spritesFightPoolController = SpritesFightPoolController.GetInstance();
    }

    public FightGameManager.OpponentName GetOpponentName() {
        return opponentName;
    }

    public void SetShips(Ship[] ships) {
        shipsMassive = ships;
        for(int i = 0; i < shipsMassive.Length; i++) {
            shipsList.Add(shipsMassive[i]);
        }
    }
         
    public List<Vector2> GetAvaliableCellsToHitByVectorMassive(Vector2[] posMassive) {
        List<Vector2> avaliableCells = new List<Vector2>();
        for(int i = 0; i < posMassive.Length;i++) {
            Vector2 pos = posMassive[i];
            CellPointPos tapCellPoint = SearchTapCellData(pos, fieldPointsToHit);
            if(tapCellPoint.number <= 0) {
                continue;
            }
            Dictionary<int, Vector2> letterPoints = fieldPoints[tapCellPoint.letter];
            avaliableCells.Add(letterPoints[tapCellPoint.number]);
        }
        return avaliableCells;
    }

    public void HitByShipAttackZone(Vector2[] points) {
        bool IsAvaliableHitsOver = false;
        for(int i = 0; i < points.Length; i++) {
            if(IsAvaliableHitsOver) {
                break;
            }
            if(FightGameManager.GetInstance().GetAvaliableCellsCountToHit() <= 1) {
                IsAvaliableHitsOver = true;
            }
            HitEnemyCellByPos(points[i]);
        }
    }

    private void HitEnemyCellByPos(Vector2 pointPosition) {
        CellPointPos tapCellData = SearchTapCellData(pointPosition,fieldPointsToHit);
        Dictionary<int, Vector2> letterPoints = fieldPointsToHit[tapCellData.letter];
        if(tapCellData.number > 0) {
            Vector2 tapCellPosition = letterPoints[tapCellData.number];
            letterPoints.Remove(tapCellData.number);
            FightGameManager.GetInstance().DecreaseOneCell();
            bool IsHit = CheckIsShipHit(new CellPointPos(tapCellData.letter, tapCellData.number));
            if(IsHit) {
                ActivateCellStateSprite(tapCellPosition, true);
            } else {
                ActivateCellStateSprite(tapCellPosition, false);
            }
        }
    }

    private bool CheckIsShipHit(CellPointPos tapCell) {
        for(int i = 0; i < shipsList.Count; i++) {
            List<CellPointPos> curShipPoints = shipsList[i].shipHitPoints;
            for(int k = 0; k < curShipPoints.Count; k++) {
                if((curShipPoints[k].letter == tapCell.letter && curShipPoints[k].number == tapCell.number)) {
                    if(!IsSelfField) {
                        curShipPoints.RemoveAt(k);
                        if(curShipPoints.Count == 0) {
                            shipsList[i].DestroyShip();
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
        int[] borderData = CalculateShipCellsBorder(shipPoints);
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
}
