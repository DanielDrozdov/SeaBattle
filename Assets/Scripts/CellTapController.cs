using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CellTapController : MonoBehaviour, IPointerUpHandler, IPointerDownHandler {

    public void OnPointerUp(PointerEventData eventData) {
        Image image = GetComponent<Image>();
        image.sprite = FightFieldStateController.GetInstance().GetHitCrossSprite();
        image.color = new Color(1, 1, 1, 1);
    }

    public void OnPointerDown(PointerEventData eventData) {
        
    }
}
