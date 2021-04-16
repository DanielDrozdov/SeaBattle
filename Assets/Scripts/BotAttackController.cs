using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotAttackController : MonoBehaviour
{
    [SerializeField] private ShipAttackZoneController fourCellShipAttackZone;
    private FightGameManager fightGameManager;

    private void Start() {
        fightGameManager = FightGameManager.GetInstance();
        fourCellShipAttackZone = ShipAttackZonesManager.GetInstance().GetShipAttackZone(4);
    }

    public void HitPlayer() {
        ServiceManager.GetInstance().SetNewShipAttackZone(fourCellShipAttackZone.gameObject, 4);
        fourCellShipAttackZone.HitPlayerOnRandomPos();
    }
}
