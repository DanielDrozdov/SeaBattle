using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class MissionPanelController : MonoBehaviour, IPointerUpHandler,IPointerDownHandler {

    [SerializeField] private int missionNumber;
    [SerializeField] private string missionName;
    [SerializeField] [TextArea] private string missionDescription;
    [SerializeField] private Sprite missionSprite;
    [SerializeField] private GameObject lockImage;
    [SerializeField] private TextMeshProUGUI missionNameText;
    [SerializeField] private TextMeshProUGUI missionNumberText;
    [Header("MissionDataSettings")]
    [SerializeField] private SelectedMissionData missionData;
    private bool IsMissionOpened;

    private void Awake() {
        missionNameText.text = $"Миссия \"{missionName}\" ";
        missionNumberText.text = missionNumber.ToString();
        if(PlayerPrefs.GetInt("CurrentMissionNumber") >= missionNumber) {
            IsMissionOpened = true;
        } else {
            GetComponent<Image>().color = new Color(0.8f,0.8f,0.8f,1); // grey color
            lockImage.SetActive(true);
        }
    }

    public void OnPointerDown(PointerEventData eventData) { }

    public void OnPointerUp(PointerEventData eventData) {
        if(true /*IsMissionOpened*/) {
            DataSceneTransitionController.GetInstance().SetSelectedMissionData(missionData);
            MainMenuUIController.GetInstance().ActivatePanelTransition(() => {
                SelectedMissionPanelController.GetInstance().SetMissionData(missionSprite, missionNumber, missionName, missionDescription);
            });      
        }
    }
}
