using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotAttackController : MonoBehaviour
{
    [SerializeField] private FightFieldStateController firstPlayerFieldController;
    private FightFieldStateController selfFieldController;
    private ShipAttackZonesManager shipAttackZonesManager;
    private ShipAttackZoneController shipAttackZone;
    private FightGameManager fightGameManager;

    private Dictionary<int, int> shipsByCellsSize;
    private Dictionary<Ship,List<CellPointPos>> hitShips;
    private int currentAttackCellsCount;

    private Ship chaseHitShip;
    private List<CellPointPos> chaseShipHitPoints;

    private void Awake() {
        chaseShipHitPoints = new List<CellPointPos>();
        selfFieldController = GetComponent<FightFieldStateController>();
        if(selfFieldController.GetOpponentName() != FightGameManager.OpponentName.Bot) {
            Destroy(this);
        }
        firstPlayerFieldController.SetEnemyBotAttackController(this);
    }

    private void Start() {
        fightGameManager = FightGameManager.GetInstance();
        shipAttackZonesManager = ShipAttackZonesManager.GetInstance();
    }

    public void HitPlayer() {
        shipAttackZonesManager.SetNewShipAttackZone(shipAttackZone, currentAttackCellsCount);
        StartCoroutine(CheckAvaliableCellsCountToHitValueChangeCoroutine());
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
            chaseShipHitPoints.Add(keyHitCell);
        }
    }

    public void RemoveDestroyedShip(Ship destroyedShip) {
        hitShips.Remove(destroyedShip);
        if(chaseHitShip == destroyedShip) {
            chaseShipHitPoints.Clear();
            hitShips.Remove(chaseHitShip);
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

    private IEnumerator CheckAvaliableCellsCountToHitValueChangeCoroutine() {
        int lastCellsValue = fightGameManager.GetAvaliableCellsCountToHit();
        TryDestroyLastHitShip();
        while(true) {
            if(selfFieldController.GetOpponentName() != fightGameManager.GetCurrentOpponentNameToAttack()) {
                yield break;
            }
            if(lastCellsValue != fightGameManager.GetAvaliableCellsCountToHit()) {
                TryDestroyLastHitShip();
                lastCellsValue = fightGameManager.GetAvaliableCellsCountToHit();
            }
            yield return null;
        }
    }

    private void TryDestroyLastHitShip() {
        foreach(Ship hitShip in hitShips.Keys) {
            if(chaseHitShip == null) {
                chaseHitShip = hitShip;
                chaseShipHitPoints = hitShips[hitShip];
                break;
            }
        }
        if(hitShips.Count == 0) {
            AttackOnRandomFreeCellOnField();
            return;
        }
        DestroyLastHitShip();
    }

    private void DestroyLastHitShip() {
        Vector2 attackPos;
        CellPointPos nextCellPoint;
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

        if(chaseShipHitPoints[0].number == chaseShipHitPoints[1].number) { //Y
            minCellPoint = new CellPointPos((char)(minLetter - 1), minNumber);
            maxCellPoint = new CellPointPos((char)(maxLetter + 1), maxNumber);
        } else {
            minCellPoint = new CellPointPos(minLetter, minNumber - 1);
            maxCellPoint = new CellPointPos(maxLetter, maxNumber + 1);
        }
        minCellPoint.letter = (char)Mathf.Clamp(minCellPoint.letter, 97, 106);
        maxCellPoint.letter = (char)Mathf.Clamp(maxCellPoint.letter, 97, 106);
        minCellPoint.number = Mathf.Clamp(minCellPoint.number, 1, 10);
        maxCellPoint.number = Mathf.Clamp(maxCellPoint.number, 1, 10);
        if(firstPlayerFieldController.CheckIsCellPointFreeToHit(minCellPoint)) {
            nextCellPoint = minCellPoint;
        } else {
            nextCellPoint = maxCellPoint;
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
        List<char> freeLettersToHitList = new List<char>();
        for(int i = 0; i < 10; i++) {
            freeLettersToHitList.Add(firstPlayerFieldController.fieldLettersMassive[i]);
        }
        Vector2 targetFreePos = GetRandomCellPointPosToHit(freePointsToHit, freeLettersToHitList);
        shipAttackZone.AttackByPos(targetFreePos);
    }

    private Vector2 GetRandomCellPointPosToHit(Dictionary<char, Dictionary<int, Vector2>> freePointsToHit,List<char> freeLettersList) {
        char randomLetter = firstPlayerFieldController.fieldLettersMassive[Random.Range(0, freeLettersList.Count)];
        Dictionary<int, Vector2> letterPoints = freePointsToHit[randomLetter];
        if(letterPoints.Keys.Count == 0) {
            freeLettersList.Remove(randomLetter);
            return GetRandomCellPointPosToHit(freePointsToHit, freeLettersList);
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
