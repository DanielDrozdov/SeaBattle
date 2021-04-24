using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    [SerializeField] private int cellsCount;
    [HideInInspector] public List<CellPointPos> shipPoints;
    [HideInInspector] public List<CellPointPos> shipHitPoints;
    private ShipAttackZoneController shipAttackZone;
    private CellPointPos[] shipPointsMassive;
    private SpriteRenderer spriteRenderer;
    private FightFieldStateController fightFieldStateController;

    private bool IsDestroyed;
    private bool IsRotatedOnY;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if(DataSceneTransitionController.GetInstance().GetBattleType() == DataSceneTransitionController.BattleType.P1vsP2) {
            Destroy(GetComponent<BoxCollider2D>()); // temp
        }
    }

    private void Start() {
        shipAttackZone = ShipAttackZonesManager.GetInstance().GetShipAttackZone(cellsCount);
    }

    private void OnMouseUp() {
        if(!IsDestroyed && FightGameManager.GetInstance().GetCurrentOpponentNameToAttack() != FightGameManager.OpponentName.Bot && 
            DataSceneTransitionController.GetInstance().GetBattleType() != DataSceneTransitionController.BattleType.P1vsP2 &&
            fightFieldStateController.GetOpponentName() != FightGameManager.OpponentName.Bot) {
            ShipAttackZonesManager.GetInstance().SetNewShipAttackZone(shipAttackZone, shipPoints.Count);
        }
    }

    public bool IsShipDestroyed() {
        return IsDestroyed;
    }

    public int GetCellsSize() {
        return cellsCount;
    }

    public void DestroyShip() {
        IsDestroyed = true;
        spriteRenderer.enabled = true;
    }

    public void SetShipPointsMassiveAndFightFieldController(CellPointPos[] selectedShipPoints,FightFieldStateController fightFieldStateController) {
        shipPointsMassive = selectedShipPoints;
        this.fightFieldStateController = fightFieldStateController;
        if(fightFieldStateController.GetOpponentName() == FightGameManager.OpponentName.Bot 
            || DataSceneTransitionController.GetInstance().GetBattleType() == DataSceneTransitionController.BattleType.P1vsP2) {
            spriteRenderer.enabled = false;
        }
        FillSecondaryMassives();
        if(shipPointsMassive.Length > 1) {
            CalculateShipRotate();
        }
        CalculateShipPosition();
    }

    private void FillSecondaryMassives() {
        shipPoints = new List<CellPointPos>();
        shipHitPoints = new List<CellPointPos>();
        for(int i = 0; i < shipPointsMassive.Length; i++) {
            shipPoints.Add(shipPointsMassive[i]);
            shipHitPoints.Add(shipPointsMassive[i]);
        }
    }

    private void CalculateShipRotate() {
        char firstLetter = shipPointsMassive[0].letter;
        char secondLetter = shipPointsMassive[1].letter;
        if(firstLetter != secondLetter) {
            transform.Rotate(0, 0, 90);
            IsRotatedOnY = true;
        }
    }

    private void CalculateShipPosition() {
        float xPos;
        float yPos;
        if(IsRotatedOnY) {
            xPos = fightFieldStateController.GetPosByCellPoint(shipPointsMassive[0]).x;
            float firstYPos = fightFieldStateController.GetPosByCellPoint(shipPointsMassive[0]).y;
            float secondYPos = fightFieldStateController.GetPosByCellPoint(shipPointsMassive[shipPointsMassive.Length - 1]).y;
            yPos = (secondYPos - firstYPos) / 2 + firstYPos;
        } else {
            yPos = fightFieldStateController.GetPosByCellPoint(shipPointsMassive[0]).y;
            float firstXPos = fightFieldStateController.GetPosByCellPoint(shipPointsMassive[0]).x;
            float secondXPos = fightFieldStateController.GetPosByCellPoint(shipPointsMassive[shipPointsMassive.Length - 1]).x;
            xPos = (secondXPos - firstXPos) / 2 + firstXPos;
        }
        transform.position = new Vector2(xPos,yPos);
    }

}
