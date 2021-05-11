using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectAttackZonePanelButtonController : MonoBehaviour
{
    [SerializeField] private int shipCellsSize;
    [SerializeField] private AnimationClip clip;
    private ShipAttackZoneController shipAttackZone;
    private Animator animator;

    private void Start() {
        shipAttackZone = ShipAttackZonesManager.GetInstance().GetShipAttackZone(shipCellsSize);
        animator = GetComponent<Animator>();
        clip.wrapMode = WrapMode.Once;
    }

    private void OnMouseUp() {
        if(FightGameManager.GetInstance().GetCurrentOpponentNameToAttack() != FightGameManager.OpponentName.Bot) {
            ShipAttackZonesManager.GetInstance().SetNewShipAttackZone(shipAttackZone, shipCellsSize);
            animator.Play(shipCellsSize + "CellAttackZoneAnimation");
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
