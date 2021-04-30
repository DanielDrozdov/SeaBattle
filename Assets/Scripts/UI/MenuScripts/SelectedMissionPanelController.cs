﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectedMissionPanelController : MonoBehaviour
{
    private static SelectedMissionPanelController Instance;
    [SerializeField] private GameObject SelectedShipsFieldForMissionsPanel;
    [SerializeField] private GameObject missionsListPanel;
    [SerializeField] private TextMeshProUGUI missionNameText;
    [SerializeField] private TextMeshProUGUI missionNumberText;
    [SerializeField] private TextMeshProUGUI missionDesriptionText;
    [SerializeField] private Image missionImage;

    private void Awake() {
        Instance = this;
        gameObject.SetActive(false);
    }

    private void OnDisable() {
        gameObject.SetActive(false);
    }

    public static SelectedMissionPanelController GetInstance() {
        return Instance;
    }

    public void OnClickButton_StartMission() {
        MainMenuUIController.GetInstance().ActivatePanelTransition(() => {
            SelectedShipsFieldForMissionsPanel.SetActive(true);
            gameObject.SetActive(false);
            DataSceneTransitionController dataSceneTransitionController = DataSceneTransitionController.GetInstance();
            dataSceneTransitionController.SetBattleMode(DataSceneTransitionController.BattleMode.Classic);
            dataSceneTransitionController.SetBattleType(DataSceneTransitionController.BattleType.P1vsBot);
        });
    }

    public void SetMissionData(Sprite missionSprite,int missionNumber,string missionName,string missionDescription) {
        missionsListPanel.SetActive(false);
        gameObject.SetActive(true);
        missionImage.sprite = missionSprite;
        missionNumberText.text = "Миссия номер: " + missionNumber.ToString();
        missionNameText.text = missionName.ToString();
        missionDesriptionText.text = missionDescription.ToString();
    }
}