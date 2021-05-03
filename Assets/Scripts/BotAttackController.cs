using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotAttackController : MonoBehaviour
{
    [SerializeField] private FightFieldStateController firstPlayerFieldController;
    private ShipAttackZonesManager shipAttackZonesManager;
    private ShipAttackZoneController shipAttackZone;

    private Dictionary<int, int> shipsByCellsSize;
    private Dictionary<Ship,List<CellPointPos>> hitShips;
    private List<char> freeLettersToHitList;
    private int currentAttackCellsCount;

    private Ship chaseHitShip;
    private List<CellPointPos> chaseShipHitPoints;

    private void Awake() {
        chaseShipHitPoints = new List<CellPointPos>();
        if(DataSceneTransitionController.GetInstance().GetPlayerCountWithShips() == 2) {
            Destroy(this);
        }
        freeLettersToHitList = new List<char>();
        for(int i = 0; i < 10; i++) {
            freeLettersToHitList.Add(firstPlayerFieldController.fieldLettersMassive[i]);
        }
        firstPlayerFieldController.SetEnemyBotAttackController(this);
    }

    private void Start() {
        shipAttackZonesManager = ShipAttackZonesManager.GetInstance();
    }

    public void HitPlayer() {
        shipAttackZonesManager.SetNewShipAttackZone(shipAttackZone, currentAttackCellsCount);
        TryDestroyLastHitShip();
    }

    public void SetShipsMassive(Ship[] ships) {
        hitShips = new Dictionary<Ship, List<CellPointPos>>();
        shipsByCellsSize = new Dictionary<int, int>();
        for(int i = 1; i <= 4; i++) {
            shipsByCellsSize.Add(i, 0);
        }
        for(int i = 0;i < ships.Length;i++) {
            if(shipsByCellsSize.ContainsKey(ships[i].GetCellsSize())) {
                shipsByCellsSize[ships[i].GetCellsSize()]++;
            }
        }
        FindPowerfulShip();
    }

    public void DecreaseOneShip(int destroyedShipCellsSize) {
        shipsByCellsSize[destroyedShipCellsSize]--;
        FindPowerfulShip();
    }

    public void SetHitShip(Ship hitShip, CellPointPos keyHitCell) {
        if(!hitShips.ContainsKey(hitShip)) {
            List<CellPointPos> shipHitPointsList = new List<CellPointPos>();
            shipHitPointsList.Add(keyHitCell);
            hitShips.Add(hitShip, shipHitPointsList);
        } else {
            hitShips[hitShip].Add(keyHitCell);
        }

        if(chaseHitShip != null) {
            if(chaseHitShip == hitShip) {
                chaseShipHitPoints.Add(keyHitCell);
            }
        }
    }

    public void RemoveDestroyedShip(Ship destroyedShip) {
        hitShips.Remove(destroyedShip);
        if(chaseHitShip == destroyedShip) {
            chaseShipHitPoints.Clear();
            chaseHitShip = null;
        }
    }

    private void FindPowerfulShip() {
        for(int i = 4;i > 0;i--) {
            if(shipsByCellsSize[i] != 0) {
                currentAttackCellsCount = i;
                break;
            }
        }
        shipAttackZone = shipAttackZonesManager.GetShipAttackZone(currentAttackCellsCount);
    }

    private void TryDestroyLastHitShip() {
        if(hitShips.Count == 0) {
            AttackOnRandomFreeCellOnField();
            return;
        }
        if(chaseHitShip == null) {
            foreach(Ship hitShip in hitShips.Keys) {
                chaseHitShip = hitShip;
                chaseShipHitPoints = hitShips[hitShip];
                break;
            }
        }
        DestroyLastHitShip();
    }

    private void DestroyLastHitShip() {
        Vector2 attackPos;
        CellPointPos nextCellPoint = new CellPointPos('a',1);
        if(chaseShipHitPoints.Count == 1) {
            nextCellPoint = GetRandomCellPointAroundHitShipPoint(hitShips[chaseHitShip][0]);
            attackPos = firstPlayerFieldController.GetPosByCellPoint(nextCellPoint);
            shipAttackZone.AttackByPos(attackPos);
            return;
        }
        char minLetter = chaseShipHitPoints[0].letter;
        char maxLetter = chaseShipHitPoints[0].letter;
        int minNumber = chaseShipHitPoints[0].number;
        int maxNumber = chaseShipHitPoints[0].number;
        for(int i = 0;i < chaseShipHitPoints.Count;i++) {
            if(chaseShipHitPoints[i].letter > maxLetter) {
                maxLetter = chaseShipHitPoints[i].letter;
            } else if(chaseShipHitPoints[i].letter < minLetter) {
                minLetter = chaseShipHitPoints[i].letter;
            }

            if(chaseShipHitPoints[i].number > maxNumber) {
                maxNumber = chaseShipHitPoints[i].number;
            } else if(chaseShipHitPoints[i].number < minNumber) {
                minNumber = chaseShipHitPoints[i].number;
            }
        }

        CellPointPos minCellPoint;
        CellPointPos maxCellPoint;
        bool IsFlippedOnY = false;
        if(chaseShipHitPoints[0].number == chaseShipHitPoints[1].number) { //Y
            minCellPoint = new CellPointPos((char)(minLetter - 1), minNumber);
            maxCellPoint = new CellPointPos((char)(maxLetter + 1), maxNumber);
            IsFlippedOnY = true;
        } else {
            minCellPoint = new CellPointPos(minLetter, minNumber - 1);
            maxCellPoint = new CellPointPos(maxLetter, maxNumber + 1);
        }
        minCellPoint.letter = (char)Mathf.Clamp(minCellPoint.letter, 97, 106);
        maxCellPoint.letter = (char)Mathf.Clamp(maxCellPoint.letter, 97, 106);
        minCellPoint.number = Mathf.Clamp(minCellPoint.number, 1, 10);
        maxCellPoint.number = Mathf.Clamp(maxCellPoint.number, 1, 10);

        int cellsDelta;
        if(IsFlippedOnY) {
            cellsDelta = maxCellPoint.letter - minCellPoint.letter + 1;
        } else {
            cellsDelta = maxCellPoint.number - minCellPoint.number;
        }

        for(int i = 0; i <= cellsDelta;i++) {
            if(IsFlippedOnY) {
                nextCellPoint = new CellPointPos((char)(minCellPoint.letter + i), minCellPoint.number);
            } else {
                nextCellPoint = new CellPointPos(minCellPoint.letter, minCellPoint.number + i);
            }
            if(firstPlayerFieldController.CheckIsCellPointFreeToHit(nextCellPoint)) {
                break;
            }
        }
        attackPos = firstPlayerFieldController.GetPosByCellPoint(nextCellPoint);
        shipAttackZone.AttackByPos(attackPos);
    }

    private CellPointPos GetRandomCellPointAroundHitShipPoint(CellPointPos hitCellPoint) {
        int randomNextHitNumber = Random.Range(hitCellPoint.number - 1, hitCellPoint.number + 2);
        int randomNextHitLetterNum = Random.Range(hitCellPoint.letter - 1, hitCellPoint.letter + 2);
        char randomNextHitLetter = (char)Mathf.Clamp(randomNextHitLetterNum, 97, 106);
        if(Mathf.Abs(randomNextHitLetter - hitCellPoint.letter) == 1) {
            randomNextHitNumber = hitCellPoint.number;
        } else if(Mathf.Abs(randomNextHitNumber - hitCellPoint.number) == 1) {
            randomNextHitLetter = hitCellPoint.letter;
        }
        randomNextHitNumber = Mathf.Clamp(randomNextHitNumber, 1, 10);
        CellPointPos randomCell = new CellPointPos(randomNextHitLetter, randomNextHitNumber);

        for(int i = 0; i < chaseShipHitPoints.Count; i++) {
            if(chaseShipHitPoints[i].letter == randomCell.letter && chaseShipHitPoints[i].number == randomCell.number) {
                return GetRandomCellPointAroundHitShipPoint(hitCellPoint);
            }
        }
        if(!firstPlayerFieldController.CheckIsCellPointFreeToHit(randomCell)) {
            return GetRandomCellPointAroundHitShipPoint(hitCellPoint);
        }
        return randomCell;
    }

    private void AttackOnRandomFreeCellOnField() {
        Dictionary<char, Dictionary<int, Vector2>> freePointsToHit = firstPlayerFieldController.GetFreeCellsPointsToHit();
        Vector2 targetFreePos = new Vector2(0,0);
        Vector2 zeroVector = Vector2.zero;
        while(targetFreePos == zeroVector) {
            targetFreePos = GetRandomCellPointPosToHit(freePointsToHit);
        }
        shipAttackZone.AttackByPos(targetFreePos);
    }

    private Vector2 GetRandomCellPointPosToHit(Dictionary<char, Dictionary<int, Vector2>> freePointsToHit) {
        char randomLetter = freeLettersToHitList[Random.Range(0, freeLettersToHitList.Count)];
        Dictionary<int, Vector2> letterPoints = freePointsToHit[randomLetter];
        if(letterPoints.Keys.Count == 0) {
            freeLettersToHitList.Remove(randomLetter);
            return new Vector2(0, 0);
        }
        List<int> freePointsNumbersMassive = new List<int>();
        for(int i = 1;i <= 10;i++) {
            if(letterPoints.ContainsKey(i)) {
                freePointsNumbersMassive.Add(i);
            }
        }
        int randomNumber = freePointsNumbersMassive[Random.Range(0, freePointsNumbersMassive.Count)];
        CellPointPos randomCellPoint = new CellPointPos(randomLetter, randomNumber);
        return firstPlayerFieldController.GetPosByCellPoint(randomCellPoint);
    }
}
