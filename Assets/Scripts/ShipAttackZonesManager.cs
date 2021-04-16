using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipAttackZonesManager : MonoBehaviour
{
    [SerializeField] private ShipAttackZoneController nineCellsZone;
    [SerializeField] private ShipAttackZoneController fourCellsZone;
    [SerializeField] private ShipAttackZoneController oneCellZone;
    private static ShipAttackZonesManager Instance;

    private void Awake() {
        Instance = this;
    }

    public static ShipAttackZonesManager GetInstance() {
        return Instance;
    }

    public ShipAttackZoneController GetShipAttackZone(int shipCellsSize) {
        if(shipCellsSize == 4) {
            return nineCellsZone;
        } else if(shipCellsSize == 3 || shipCellsSize == 2) {
            return fourCellsZone;
        } else {
            return oneCellZone;
        }
    }

    public void ChangeOpponentAttackField(FightFieldStateController fightFieldStateController) {
        nineCellsZone.SetAnotherOpponentField(fightFieldStateController);
        fourCellsZone.SetAnotherOpponentField(fightFieldStateController);
        oneCellZone.SetAnotherOpponentField(fightFieldStateController);
    }
}
