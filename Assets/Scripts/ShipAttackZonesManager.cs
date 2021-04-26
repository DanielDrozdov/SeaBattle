using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipAttackZonesManager : MonoBehaviour
{
    [SerializeField] private ShipAttackZoneController nineCellsZone;
    [SerializeField] private ShipAttackZoneController fourCellsZone;
    [SerializeField] private ShipAttackZoneController oneCellZone;
    private static ShipAttackZonesManager Instance;

    private DataSceneTransitionController DataSceneTransitionController;
    private ShipAttackZoneController lastActivatedShipAttackZone;
    private int lastShipCellsCount;

    private void Awake() {
        Instance = this;
        DataSceneTransitionController = DataSceneTransitionController.GetInstance();
        if(DataSceneTransitionController.GetBattleMode() == DataSceneTransitionController.BattleMode.Classic) {
            SetNewShipAttackZone(oneCellZone, 1);
        }
    }

    public static ShipAttackZonesManager GetInstance() {
        return Instance;
    }

    public int GetLastActivatedShipCellsCount() {
        return lastShipCellsCount;
    }

    public void SetNewShipAttackZone(ShipAttackZoneController shipAttackZone, int shipCellsCount) {
        if(DataSceneTransitionController.GetBattleMode() == DataSceneTransitionController.BattleMode.Classic) {
            lastActivatedShipAttackZone = oneCellZone;
            lastShipCellsCount = 1;
            oneCellZone.gameObject.SetActive(true);
            return;
        }
        if(lastActivatedShipAttackZone != null) {
            lastActivatedShipAttackZone.gameObject.SetActive(false);
        }
        shipAttackZone.gameObject.SetActive(true);
        lastActivatedShipAttackZone = shipAttackZone;
        lastShipCellsCount = shipCellsCount;
    }

    public ShipAttackZoneController GetShipAttackZone(int shipCellsSize) {
        if(DataSceneTransitionController.GetBattleMode() == DataSceneTransitionController.BattleMode.Classic) {
            return oneCellZone;
        }

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

    public void OffZones() {
        lastActivatedShipAttackZone?.gameObject.SetActive(false);
    }

}
