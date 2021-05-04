using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ship : MonoBehaviour
{
    public delegate void CaravanActions();
    public event CaravanActions OnCaravanShipDie;

    [SerializeField] private bool IsCaravan;
    [SerializeField] private int cellsCount;
    [HideInInspector] public List<CellPointPos> shipPoints;
    [HideInInspector] public List<CellPointPos> shipHitPoints;
    private CellPointPos[] shipPointsMassive;
    private Image image;
    private FightFieldStateController fightFieldStateController;

    private bool IsDestroyed;
    private bool IsRotatedOnY;

    private void Awake() {
        image = GetComponent<Image>();
    }

    public bool IsCaravanShip() {
        return IsCaravan;
    }

    public bool IsShipDestroyed() {
        return IsDestroyed;
    }

    public int GetCellsSize() {
        return cellsCount;
    }

    public void DestroyShip() {
        if(IsCaravan) {
            OnCaravanShipDie?.Invoke();
        }
        IsDestroyed = true;
        image.enabled = true;
    }

    public void SetShipPointsMassiveAndFightFieldController(CellPointPos[] selectedShipPoints,FightFieldStateController fightFieldStateController) {
        shipPointsMassive = selectedShipPoints;
        this.fightFieldStateController = fightFieldStateController;
        if(fightFieldStateController.GetOpponentName() == FightGameManager.OpponentName.Bot 
            || DataSceneTransitionController.GetInstance().GetBattleType() == DataSceneTransitionController.BattleType.P1vsP2) {
            image.enabled = false;
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
