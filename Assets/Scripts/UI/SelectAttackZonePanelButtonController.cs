using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectAttackZonePanelButtonController : MonoBehaviour
{
    [SerializeField] private int shipCellsSize;
    private ShipAttackZoneController shipAttackZone;
    private Image image;

    private void Start() {
        image = GetComponent<Image>();
        shipAttackZone = ShipAttackZonesManager.GetInstance().GetShipAttackZone(shipCellsSize);
    }

    private void OnMouseEnter() {
        image.color = Color.red;
    }

    private void OnMouseExit() {
        image.color = Color.black;
    }

    private void OnMouseUp() {
        if(FightGameManager.GetInstance().GetCurrentOpponentNameToAttack() != FightGameManager.OpponentName.Bot) {
            ShipAttackZonesManager.GetInstance().SetNewShipAttackZone(shipAttackZone, shipCellsSize);
        }
    }

    public int GetShipCellsSize() {
        return shipCellsSize;
    }

    public void ActivateAttackButton() {
        gameObject.SetActive(true);
    }

    public void DeactivateAttackButton() {
        gameObject.SetActive(false);
    }
}
