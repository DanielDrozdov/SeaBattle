using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FightFieldStateController : GameFieldState {

    [SerializeField] private FightGameManager.OpponentName opponentName;
    private SpritesFightPoolController spritesFightPoolController;

    private Ship[] shipsMassive;

    private BotAttackController botAttackController;

    private FightFieldStateController() { }

    protected override void AddAwakeActions() {
        botAttackController = GetComponent<BotAttackController>();
    }

    protected override void AddStartActions() {
        spritesFightPoolController = SpritesFightPoolController.GetInstance();
    }

    public List<Ship> GetAliveShipList() {
        return shipsList;
    }

    public FightGameManager.OpponentName GetOpponentName() {
        return opponentName;
    }

    public void SetOpponentName(FightGameManager.OpponentName opponentName) {
        this.opponentName = opponentName;
    }

    public bool CheckIsCellPointFreeToHit(CellPointPos cellPoint) {
        if(fieldPointsToHit[cellPoint.letter].ContainsKey(cellPoint.number)) {
            return true;
        }
        return false;
    }

    public void SetEnemyBotAttackController(BotAttackController botAttackController) {
        this.botAttackController = botAttackController;
    }

    public void SetShips(Ship[] ships) {
        shipsMassive = ships;
        for(int i = 0; i < shipsMassive.Length; i++) {
            shipsList.Add(shipsMassive[i]);
        }
        if(opponentName == FightGameManager.OpponentName.Bot) {
            botAttackController.SetShipsMassive(shipsMassive);
        }
    }
         
    public Dictionary<char, Dictionary<int, Vector2>> GetFreeCellsPointsToHit() {
        return fieldPointsToHit;
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
            bool IsHit = CheckIsShipHit(new CellPointPos(tapCellData.letter, tapCellData.number));
            FightGameManager.GetInstance().DecreaseOneCell();
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
                    if(botAttackController != null && opponentName != FightGameManager.OpponentName.Bot) {
                        botAttackController.SetHitShip(shipsList[i], curShipPoints[k]);
                    }
                    curShipPoints.RemoveAt(k);
                    if(curShipPoints.Count == 0) {
                        HitShip(shipsList[i].shipPoints);
                        if(opponentName == FightGameManager.OpponentName.Bot) {
                            botAttackController.DecreaseOneShip(shipsList[i].shipPoints.Count);
                        } else if(botAttackController != null && opponentName != FightGameManager.OpponentName.Bot) {
                            botAttackController.RemoveDestroyedShip(shipsList[i]);
                        }
                        shipsList[i].DestroyShip();
                        shipsList.RemoveAt(i);
                        if(DataSceneTransitionController.GetInstance().GetBattleType() == DataSceneTransitionController.BattleType.P1vsP2) {
                            SelectAttackZonePanelController.GetInstance().UpdateAliveShips();
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
