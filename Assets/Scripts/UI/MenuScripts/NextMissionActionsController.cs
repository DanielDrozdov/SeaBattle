using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextMissionActionsController : MonoBehaviour
{
    [SerializeField] CampaignMenuPanelController CampaignMenuPanelController;
    [SerializeField] MissionPanelController[] missions;

    private void Start() {
        if(!DataSceneTransitionController.GetInstance().IsNeedLoadNextMission) {
            return;
        }
        DataSceneTransitionController.GetInstance().ZeroSelectedShips();
        MainMenuUIController.GetInstance().LoadNextMissionPartActions();
        CampaignMenuPanelController.ContinueCampaignMission();
        int nextMissionNumber = DataSceneTransitionController.GetInstance().GetSelectedMissionData().missionNumber + 1;
        for(int i = 0;i < missions.Length;i++) {
            if(missions[i].GetMissionNumber() == nextMissionNumber) {
                missions[i].LoadBrifingPanel();
                DataSceneTransitionController.GetInstance().IsNeedLoadNextMission = false;
                break;
            }
        }
    }
}
