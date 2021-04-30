using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShipAttackZoneController : MonoBehaviour {
    
    private FightFieldStateController fightFieldStateController;
    private FightGameManager fightGameManager;
    [SerializeField] private Transform zoneMoveKeyPoint;
    [SerializeField] private Vector2 cellsCount;
    private Camera mainCamera;

    private Vector2 dragPosition;
    private Vector2 lastMovePointPos;
    private Vector2 keyPointOffSet;
    private float[] fieldBorders = { };

    private float moveKDelta = 0.55f;
    private float fieldCellSize;

    private float xMin;
    private float xMax;
    private float yMin;
    private float yMax;

    private void Awake() {
        lastMovePointPos = zoneMoveKeyPoint.position;
        keyPointOffSet = zoneMoveKeyPoint.position - transform.position;
    }

    private void Start() {
        mainCamera = ServiceManager.GetInstance().GetMainCamera();
        fightGameManager = FightGameManager.GetInstance();
    }

    private void OnMouseDown() {
        if(fightGameManager.GetCurrentOpponentNameToAttack()!= FightGameManager.OpponentName.Bot)
        lastMovePointPos = transform.position;
    }

    private void OnMouseDrag() {
        if(fightGameManager.GetCurrentOpponentNameToAttack() != FightGameManager.OpponentName.Bot) {
            if(Input.touchCount > 0) {
                dragPosition = Input.GetTouch(0).position;
            } else {
                dragPosition = Input.mousePosition;
            }
            dragPosition = mainCamera.ScreenToWorldPoint(dragPosition);
            dragPosition.x = Mathf.Clamp(dragPosition.x, xMin, xMax);
            dragPosition.y = Mathf.Clamp(dragPosition.y, yMin, yMax);

            if(Mathf.Abs(dragPosition.x - lastMovePointPos.x) > fieldCellSize * moveKDelta || Mathf.Abs(dragPosition.y - lastMovePointPos.y) > fieldCellSize * moveKDelta) {
                SetZonePostionOnNearestCell(dragPosition);
            }
        }
    }

    private void OnMouseUp() {
        if(fightGameManager.GetCurrentOpponentNameToAttack() != FightGameManager.OpponentName.Bot) {
            SetZonePostionOnNearestCell(dragPosition);
            HitPlayer();
        }
    }

    public void AttackByPos(Vector2 targetPos) {
        CalculateShipBorders();
        targetPos.x = Mathf.Clamp(targetPos.x, xMin, xMax);
        targetPos.y = Mathf.Clamp(targetPos.y, yMin, yMax);
        SetPos(targetPos);
        StartCoroutine(OneSecondDelayAndHitPlayerByBotAttackCoroutine());
    }

    public void SetAnotherOpponentField(FightFieldStateController opponentField) {
        fightFieldStateController = opponentField;
        fieldBorders = fightFieldStateController.GetFieldBorders();
        fieldCellSize = fightFieldStateController.GetCellSizeDelta();
        CalculateShipBorders();
        SetRandomPosOnField();
    }

    private void SetRandomPosOnField() {     
        float xPos = Random.Range(xMin, xMax);
        float yPos = Random.Range(yMin, yMax);
        SetPos(new Vector2(xPos,yPos));
    }

    private void SetPos(Vector2 targetPos) {
        if(keyPointOffSet == new Vector2(0, 0)) {
            keyPointOffSet = zoneMoveKeyPoint.position - transform.position;
        }
        transform.position = targetPos;
        SetZonePostionOnNearestCell(targetPos);
    }

    private void HitPlayer(bool IsBotAttack = false) {
        Vector2[] attackPositions = GetEnemyAttackCellsPositions();
        List<Vector2> avaliableToAttackPositions = fightFieldStateController.GetAvaliableCellsToHitByVectorMassive(attackPositions);
        if(avaliableToAttackPositions.Count == 0) {
            return;
        }
        Vector2[] randomHitCells = ChooseRandomHitCells(avaliableToAttackPositions);
        fightFieldStateController.HitByShipAttackZone(randomHitCells);
    }

    private Vector2[] GetEnemyAttackCellsPositions() {
        Vector2[] attackCells = new Vector2[(int)(cellsCount.y * cellsCount.x)];
        Vector2 keyPos = zoneMoveKeyPoint.position;
        int curSortCellNumber = 0;
        for(int i = 0; i < cellsCount.y; i++) {
            for(int k = 0; k < cellsCount.x; k++) {
                attackCells[curSortCellNumber++]= new Vector2(keyPos.x + fieldCellSize * k, keyPos.y - fieldCellSize * i);
            }
        }
        return attackCells;
    }

    private void SetZonePostionOnNearestCell(Vector2 tapDragPosition) {
        Vector2 pos = fightFieldStateController.GetNearestCellPos(tapDragPosition + keyPointOffSet);
        transform.position = new Vector3(pos.x, pos.y, transform.position.z) - (Vector3)keyPointOffSet;
        lastMovePointPos = transform.position;
    }

    private Vector2[] ChooseRandomHitCells(List<Vector2> attackCells) {
        int choosesCellsCount = ShipAttackZonesManager.GetInstance().GetLastActivatedShipCellsCount();
        Vector2[] hitCells = new Vector2[choosesCellsCount];
        Vector2[] cellsPos = new Vector2[choosesCellsCount];
        int[] randomCellsNumbers = GetRandomValues(attackCells.Count);
        for(int i = 0; i < cellsPos.Length; i++) {
            hitCells[i] = attackCells[randomCellsNumbers[i]];
        }
        return hitCells;
    }

    private int[] GetRandomValues(int attackCellsCount) {
        int choosesCellsCount = ShipAttackZonesManager.GetInstance().GetLastActivatedShipCellsCount();
        int[] values = new int[choosesCellsCount];
        if(choosesCellsCount > attackCellsCount) {
            choosesCellsCount = attackCellsCount;
        }
        for(int i = 0; i < choosesCellsCount; i++) {
            int nextValue;
            do {
                nextValue = Random.Range(0, attackCellsCount);
            } while(ContainsValue(values, i, nextValue));
            values[i] = nextValue;
        }
        return values;
    }

    private bool ContainsValue(int[] values, int endIndex, int valueToFind) {
        for(int i = 0; i < endIndex; i++) {
            if(values[i] == valueToFind) return true;
        }
        return false;
    }

    private void CalculateShipBorders() {
        xMin = fieldBorders[0] + (cellsCount.x * fieldCellSize) / 2;
        xMax = fieldBorders[1] - (cellsCount.x * fieldCellSize) / 2;
        yMin = fieldBorders[2] + (cellsCount.y * fieldCellSize) / 2;
        yMax = fieldBorders[3] - (cellsCount.y * fieldCellSize) / 2;
    }

    private IEnumerator OneSecondDelayAndHitPlayerByBotAttackCoroutine() {
        yield return new WaitForSeconds(1);
        HitPlayer(true);
    }
}
