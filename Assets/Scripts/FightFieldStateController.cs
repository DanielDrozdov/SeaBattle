using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FightFieldStateController : GameFieldState {

    [SerializeField] private FightGameManager.OpponentName opponentName;
    private SpritesFightPoolController spritesFightPoolController;
    private FightGameManager fightGameManager;

    //private Dictionary<CellPointPos, GameObject> nullSpritesDict;

    private BotAttackController botAttackController;

    private FightFieldStateController() { }

    protected override void AddAwakeActions() {
        botAttackController = GetComponent<BotAttackController>();
        SetFieldSize();
        //nullSpritesDict = new Dictionary<CellPointPos, GameObject>();
    }

    protected override void AddStartActions() {
        spritesFightPoolController = SpritesFightPoolController.GetInstance();
        fightGameManager = FightGameManager.GetInstance();
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

    //public void ZeroPoints() {
    //    int i = 0;
    //    CellPointPos[] nulls = new CellPointPos[4];
    //    foreach(CellPointPos cell in nullSpritesDict.Keys) {
    //        if(i >= 4) {
    //            break;
    //        }
    //        fieldPointsToHit[cell.letter].Add(cell.number, fieldPoints[cell.letter][cell.number]);
    //        GameObject sprite = nullSpritesDict[cell];
    //        sprite.SetActive(false);
    //        SpritesFightPoolController.GetInstance().ReturnToQue(sprite);
    //        nulls[i] = cell;
    //        i++;
    //    }
    //    for(int k = 0; k< 4; k++) {
    //        nullSpritesDict.Remove(nulls[k]);
    //    }
    //}

    public bool CheckIsCellPointFreeToHit(CellPointPos cellPoint) {
        if(fieldPointsToHit[cellPoint.letter].ContainsKey(cellPoint.number)) {
            return true;
        }
        return false;
    }

    public void SetEnemyBotAttackController(BotAttackController botAttackController) {
        this.botAttackController = botAttackController;
    }

    public void SetShips(List<Ship> ships) {
        shipsList = ships;
        Ship[] shipsMassive = new Ship[ships.Count];
        for(int i = 0; i < ships.Count;i++) {
            shipsMassive[i] = ships[i];
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
            if(fightGameManager.GetAvaliableCellsCountToHit() <= 1) {
                IsAvaliableHitsOver = true;
            }
            HitEnemyCellByPos(points[i]);
        }
        if(fightGameManager.GetAvaliableCellsCountToHit() > 0 && fightGameManager.GetAvaliableCellsCountToHit() < 4 &&
            fightGameManager.GetCurrentOpponentNameToAttack() == FightGameManager.OpponentName.Bot
            && DataSceneTransitionController.GetInstance().GetBattleMode() != DataSceneTransitionController.BattleMode.Classic) {
            fightGameManager.AttackByBotAgain();
        }
    }

    private void HitEnemyCellByPos(Vector2 pointPosition) {
        CellPointPos tapCellData = SearchTapCellData(pointPosition,fieldPointsToHit);
        Dictionary<int, Vector2> letterPoints = fieldPointsToHit[tapCellData.letter];
        if(tapCellData.number > 0) {
            Vector2 tapCellPosition = letterPoints[tapCellData.number];
            letterPoints.Remove(tapCellData.number);
            bool IsHit = CheckIsShipHit(new CellPointPos(tapCellData.letter, tapCellData.number));
            fightGameManager.SetOpponentNextHitState(IsHit);
            fightGameManager.DecreaseOneCell();
            if(IsHit) {
                ActivateCellStateSprite(tapCellPosition, true);
            } else {
                GameObject sprite = ActivateCellStateSprite(tapCellPosition, false);
                //nullSpritesDict.Add(tapCellData, sprite);
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
                        if(shipsList.Count == 0) {
                            fightGameManager.EndGame();
                        }
                        if(DataSceneTransitionController.GetInstance().GetBattleType() == DataSceneTransitionController.BattleType.P1vsP2 &&
                            DataSceneTransitionController.GetInstance().GetBattleMode() == DataSceneTransitionController.BattleMode.Advanced) {
                            SelectAttackZonePanelController.GetInstance().UpdateAliveShips();
                        }
                    }
                    return true;
                }
            }
        }
        return false;
    }

    private GameObject ActivateCellStateSprite(Vector2 cellPos,bool IsShipHit) {
        GameObject sprite;
        if(IsShipHit) {
            sprite = spritesFightPoolController.GetHitCrossSprite();
        } else {
            sprite = spritesFightPoolController.GetNothingInCellSprite();
        }
        sprite.transform.position = cellPos;
        sprite.SetActive(true);
        return sprite;
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

    private void SetFieldSize() {
        DataSceneTransitionController dataSceneTransitionController = DataSceneTransitionController.GetInstance();
        if(dataSceneTransitionController.IsCampaignGame() && opponentName == FightGameManager.OpponentName.Bot) {
            fieldSizeInCells = dataSceneTransitionController.GetSelectedMissionData().GetEnemyFieldSize();
        } else {
            fieldSizeInCells = 10;
        }
    }
}
