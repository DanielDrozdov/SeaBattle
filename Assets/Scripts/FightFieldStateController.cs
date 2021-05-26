using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FightFieldStateController : GameFieldState {

    [SerializeField] private FightGameManager.OpponentName opponentName;
    private SpritesFightPoolController spritesFightPoolController;
    private FightGameManager fightGameManager;

    private BotAttackController botAttackController;
    private float shotDelayTimeInSecond = 0.25f;

    private FightFieldStateController() { }

    protected override void AddAwakeActions() {
        botAttackController = GetComponent<BotAttackController>();
        SetFieldSize();
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

    public void AddShipToField(Ship ship) {
        shipsList.Add(ship);
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

    public Vector2 GetRandomFreePointToHit() {
        char randomLetter;
        int randomNumber;
        while(true) {
            randomLetter = fieldLettersMassive[Random.Range(0, fieldLettersMassive.Length - (10 - fieldSizeInCells))];
            randomNumber = Random.Range(1, fieldSizeInCells + 1);
            if(CheckIsCellPointFreeToHit(new CellPointPos(randomLetter, randomNumber))) {
                break;
            }
        }
        return GetPosByCellPoint(new CellPointPos(randomLetter, randomNumber));
    }

    public CellPointPos[] ConvertPosesToPoints(Vector2[] points) {
        CellPointPos[] poses = new CellPointPos[points.Length];
        for(int i = 0; i < poses.Length; i++) {
            poses[i] = GetCellPointByPos(points[i]);
        }
        return poses;
    }

    public CellPointPos GetRandomCellWithoutShip() {
        char randomLetter;
        int randomNumber;
        bool IsShipPoint = false;
        List<CellPointPos> shipsPoints = new List<CellPointPos>();
        for(int i = 0; i < shipsList.Count; i++) {
            Ship ship = shipsList[i];
            for(int k = 0; k < ship.shipPoints.Count; k++) {
                shipsPoints.Add(ship.shipPoints[k]);
            }
        }
        while(true) {
            IsShipPoint = false;
            randomLetter = fieldLettersMassive[Random.Range(0, fieldLettersMassive.Length - (10 - fieldSizeInCells))];
            randomNumber = Random.Range(1, fieldSizeInCells + 1);
            for(int i = 0; i < shipsPoints.Count; i++) {
                if(shipsPoints[i].letter == randomLetter && shipsPoints[i].number == randomNumber) {
                    IsShipPoint = true;
                }
            }
            if(!IsShipPoint) {
                break;
            }
        }
        return new CellPointPos(randomLetter, randomNumber);
    }

    public void HitByShipAttackZone(Vector2[] points,bool IsMultiplayerCall = false) {
        if(DataSceneTransitionController.GetInstance().IsMultiplayerGame() && !IsMultiplayerCall) {
            CellPointPos[] cells = ConvertPosesToPoints(points);
            NetworkHelpManager.GetInstance().GetCurrentPlayerFightNetworkController().SendCurrentPlayerHitsMove(cells);
        }
        StartCoroutine(HitShipByAttackZoneCoroutine(points));
    }

    public void HitEnemyByPos(Vector2 pointPosition, bool IsGameMove = true) {
        HitEnemyCellByPos(pointPosition, IsGameMove);
    }

    private void HitEnemyCellByPos(Vector2 pointPosition,bool IsGameMove = true) {
        CellPointPos tapCellData = SearchTapCellData(pointPosition,fieldPointsToHit);
        Dictionary<int, Vector2> letterPoints = fieldPointsToHit[tapCellData.letter];
        if(tapCellData.number > 0) {
            Vector2 tapCellPosition = letterPoints[tapCellData.number];
            letterPoints.Remove(tapCellData.number);
            bool IsHit = CheckIsShipHit(new CellPointPos(tapCellData.letter, tapCellData.number));
            if(IsGameMove) {
                fightGameManager.SetOpponentNextHitState(IsHit);
                fightGameManager.DecreaseOneCell();
            }
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
                    shipsList[i].DoShipActionsAfterGetHit();
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
                            fightGameManager.EndGame(opponentName);
                        }
                        if(DataSceneTransitionController.GetInstance().GetBattleType() == DataSceneTransitionController.BattleType.P1vsP2 &&
                            DataSceneTransitionController.GetInstance().GetBattleMode() == DataSceneTransitionController.BattleMode.Advanced) {
                            SelectAttackZonePanelController.GetInstance().UpdateAliveShips();
                        }
                    } else {
                        FightGameSoundsController.GetInstance().PlayHitShotSound();
                    }
                    return true;
                }
            }
        }
        FightGameSoundsController.GetInstance().PlayMissShotSound();
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
        List<CellPointPos> hitPointsAroundShips = new List<CellPointPos>();
        for(int i = minLetter; i <= maxLetter; i++) {
            char letter = (char)i;
            for(int k = 1; k <= 10; k++) {
                if(fieldPointsToHit[letter].ContainsKey(k) && k >= minNumber && k <= maxNumber) {
                    hitPointsAroundShips.Add(new CellPointPos(letter, k));
                    fieldPointsToHit[letter].Remove(k);
                }
            }
        }
        StartCoroutine(TimeDelayAndSpawnHitShipNullCellsSpritesCoroutine(hitPointsAroundShips));
    }

    private void SetFieldSize() {
        DataSceneTransitionController dataSceneTransitionController = DataSceneTransitionController.GetInstance();
        if(dataSceneTransitionController.IsCampaignGame() && opponentName == FightGameManager.OpponentName.Bot) {
            fieldSizeInCells = dataSceneTransitionController.GetSelectedMissionData().GetEnemyFieldSize();
        } else {
            fieldSizeInCells = 10;
        }
    }

    private IEnumerator TimeDelayAndSpawnHitShipNullCellsSpritesCoroutine(List<CellPointPos> hitPointsAroundShip) {
        yield return new WaitForSeconds(0.6f);
        for(int i = 0;i < hitPointsAroundShip.Count;i++) {
            ActivateCellStateSprite(fieldPoints[hitPointsAroundShip[i].letter][hitPointsAroundShip[i].number], false);
        }
    }

    private IEnumerator HitShipByAttackZoneCoroutine(Vector2[] points) {
        bool IsAvaliableHitsOver = false;
        for(int i = 0; i < points.Length; i++) {
            if(IsAvaliableHitsOver) {
                break;
            }
            if(fightGameManager.GetAvaliableCellsCountToHit() <= 1) {
                IsAvaliableHitsOver = true;
            }
            HitEnemyCellByPos(points[i]);
            yield return new WaitForSeconds(shotDelayTimeInSecond);
        }
        if(fightGameManager.GetAvaliableCellsCountToHit() > 0 && fightGameManager.GetAvaliableCellsCountToHit() < 4 &&
            fightGameManager.GetCurrentOpponentNameToAttack() == FightGameManager.OpponentName.Bot
            && DataSceneTransitionController.GetInstance().GetBattleMode() != DataSceneTransitionController.BattleMode.Classic) {
            fightGameManager.AttackByBotAgain();
        }
    }
}
