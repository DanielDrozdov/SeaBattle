using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaravanMissionShipAssignController : MonoBehaviour
{
    [SerializeField] private SelectShipController[] caravanShips;
    [SerializeField] private SelectShipFieldController fieldController;

    private void Awake() {
        if(DataSceneTransitionController.GetInstance().GetSelectedMissionData().missionNumber != 3) {
            Destroy(this);
        }
    }
}
