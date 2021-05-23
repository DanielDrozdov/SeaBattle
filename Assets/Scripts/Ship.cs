using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ship : MonoBehaviour
{
    public delegate void ShipActions();
    public event ShipActions OnCaravanShipDie;
    public event ShipActions OnShipDestroy;

    [SerializeField] private bool IsCaravan;
    [SerializeField] private int cellsCount;
    [HideInInspector] public List<CellPointPos> shipPoints;
    [HideInInspector] public List<CellPointPos> shipHitPoints;
    private CellPointPos[] shipPointsMassive;
    private Image image;
    private FightFieldStateController fightFieldStateController;

    private bool IsShipDeactivated;
    private int playerMovesMissed = 0;

    private bool IsDestroyed;
    private bool IsRotatedOnY;

    private void Awake() {
        image = GetComponent<Image>();
    }

    public bool IsShipTemporarilyDeactivated() {
        return IsShipDeactivated;
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
        OnShipDestroy?.Invoke();
        FightGameSoundsController.GetInstance().PlayDestroyShipSound();
        IsDestroyed = true;
        image.sprite = ShipsSpritesDataController.GetInstance().GetDestroyShipSprite(cellsCount);
        image.enabled = true;
    }

    public void DoShipActionsAfterGetHit() {
        if(DataSceneTransitionController.GetInstance().IsCampaignGame() && fightFieldStateController.GetOpponentName() != FightGameManager.OpponentName.Bot) {
            if(FightMissionController.GetInstance().IsShipsCanBeTemporarilyDeactivated() && fightFieldStateController.GetAliveShipList().Count > 1) {
                if(IsShipDeactivated) {
                    playerMovesMissed = 0;
                }
                IsShipDeactivated = true;
            }
        }
    }

    public void SetShipPointsMassiveAndFightFieldController(CellPointPos[] selectedShipPoints,FightFieldStateController fightFieldStateController) {
        shipPointsMassive = selectedShipPoints;
        this.fightFieldStateController = fightFieldStateController;
        if(fightFieldStateController.GetOpponentName() == FightGameManager.OpponentName.Bot 
            || DataSceneTransitionController.GetInstance().GetBattleType() == DataSceneTransitionController.BattleType.P1vsP2) {
            image.enabled = false;
        }
        SubscribeOnOpponentChangeEventInNeedMission();
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
            FightGameManager.OpponentName opponentName = fightFieldStateController.GetOpponentName();
            if(opponentName == FightGameManager.OpponentName.P1) {
                transform.Rotate(0, 0, 90);
            } else {
                transform.Rotate(0, 0, -90);
            }
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

    private void UpdatePlayerMovesAndShipState() {
        if(!IsShipDeactivated || fightFieldStateController.GetOpponentName() == FightGameManager.GetInstance().GetCurrentOpponentNameToAttack()) {
            return;
        }
        if(fightFieldStateController.GetAliveShipList().Count == 1) {
            playerMovesMissed = 0;
            IsShipDeactivated = false;
        }

        playerMovesMissed++;
        if(playerMovesMissed > 1) {
            playerMovesMissed = 0;
            IsShipDeactivated = false;
        }
    }

    private void SubscribeOnOpponentChangeEventInNeedMission() {
        if(DataSceneTransitionController.GetInstance().IsCampaignGame() && fightFieldStateController.GetOpponentName() != FightGameManager.OpponentName.Bot) {
            if(FightMissionController.GetInstance().IsShipsCanBeTemporarilyDeactivated()) {
                FightGameManager.OnOpponentChanging += UpdatePlayerMovesAndShipState;
            }
        }
    }

}
