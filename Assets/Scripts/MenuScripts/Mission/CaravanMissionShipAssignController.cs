using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaravanMissionShipAssignController : MonoBehaviour
{
    [SerializeField] private SelectShipController[] caravanShips;
    [SerializeField] private SelectShipFieldController fieldController;

    private static CaravanMissionShipAssignController Instance;
    private bool IsCaravanReady;

    private void Awake() {
        Instance = this;
    }

    private void OnEnable() {
        if(DataSceneTransitionController.GetInstance().GetSelectedMissionData().missionNumber != 3) {
            for(int i = 0; i < caravanShips.Length; i++) {
                caravanShips[i].gameObject.SetActive(false);
                caravanShips[i].enabled = false;
            }
            IsCaravanReady = false;
            return;
        }
        for(int i = 0;i < caravanShips.Length;i++) {
            caravanShips[i].gameObject.SetActive(true);
            caravanShips[i].GetComponent<Image>().enabled = true;
            caravanShips[i].enabled = false;
        }
        IsCaravanReady = true;
    }

    private void Start() {
        if(IsCaravanReady) {
            for(int i = 0; i < caravanShips.Length; i++) {
                caravanShips[i].AlignShipOnCells();
                caravanShips[i].SetShipToField();
            }
        }
    }

    public static CaravanMissionShipAssignController GetInstance() {
        return Instance;
    }

    public bool IsCaravanMission() {
        return IsCaravanReady;
    }

    public List<CellPointPos[]> GetCaravanShipsCellsPoints() {
        if(IsCaravanReady) {
            for(int i = 0; i < caravanShips.Length; i++) {
                caravanShips[i].SetShipToField();
            }
            List<CellPointPos[]> points = fieldController.GetSelectedShips(false);
            return points;
        }
        return null;
    }
}
