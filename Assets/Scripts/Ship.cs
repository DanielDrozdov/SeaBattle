using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    [SerializeField] private bool IsEnemy;
    [SerializeField] private int cellsCount;
    [SerializeField] private ShipAttackZoneController shipAttackZone;
    [HideInInspector] public List<CellPointPos> shipPoints;
    [HideInInspector] public List<CellPointPos> shipHitPoints;
    private CellPointPos[] shipPointsMassive;
    private SpriteRenderer spriteRenderer;
    private FightFieldStateController fightFieldStateController;

    private bool IsRotatedOnY;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start() {
        if(!IsEnemy) {
            CalculateShipPosition();
        }
    }

    private void OnMouseUp() {
        ServiceManager.GetInstance().SetNewShipAttackZone(shipAttackZone.gameObject, shipPoints.Count);
    }

    public void DestroyShip() {
        spriteRenderer.enabled = true;
    }

    public void SetShipPointsMassiveAndFightFieldController(CellPointPos[] selectedShipPoints,FightFieldStateController fightFieldStateController) {
        shipPointsMassive = selectedShipPoints;
        this.fightFieldStateController = fightFieldStateController;
        FillSecondaryMassives();
        if(shipPointsMassive.Length > 1) {
            CalculateShipRotate();
        }
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

    public int GetCellsSize() {
        return cellsCount;
    }
}
