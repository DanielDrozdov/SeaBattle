using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionFieldChangeController : MonoBehaviour
{
    [SerializeField] private Sprite nineCellField;
    [SerializeField] private Sprite eightCellField;
    [SerializeField] private Sprite sevenCellField;
    [SerializeField] private Transform leftUpCornerPoint;
    private Vector3 startUpCornerPos;

    private void Awake() {
        if(!DataSceneTransitionController.GetInstance().IsCampaignGame()) {
            Destroy(this);
            return;
        }
        int fieldCellSize = DataSceneTransitionController.GetInstance().GetSelectedMissionData().GetEnemyFieldSize();
        Image selfImage = GetComponent<Image>();
        startUpCornerPos = leftUpCornerPoint.position;
        if(fieldCellSize == 9) {
            selfImage.sprite = nineCellField;
        } else if(fieldCellSize == 8) {
            selfImage.sprite = eightCellField;
        } else if(fieldCellSize == 7) {
            selfImage.sprite = sevenCellField;
        }
    }

    public void ChangeField() {
        int fieldCellSize = DataSceneTransitionController.GetInstance().GetSelectedMissionData().GetEnemyFieldSize();
        RectTransform rect = GetComponent<RectTransform>();
        int rectDeltaSize = 11 - (10 - fieldCellSize);
        rect.sizeDelta = new Vector2(rectDeltaSize, rectDeltaSize);
        transform.position = transform.position - (leftUpCornerPoint.position - startUpCornerPos);
    }
}
