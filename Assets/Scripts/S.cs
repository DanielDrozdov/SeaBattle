using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class S : MonoBehaviour, IPointerUpHandler, IPointerDownHandler {

    private Dictionary<char,Dictionary<int,string>> letters;

    private float xMin;
    private float xMax;
    private float fieldXSize;

    private void Awake() {
        RectTransform rect = GetComponent<RectTransform>();
        xMin = transform.position.x - rect.sizeDelta.x / 2;
        xMax = transform.position.x + rect.sizeDelta.x / 2;
        fieldXSize = Mathf.Abs(xMax - xMin);
    }

    public void OnPointerUp(PointerEventData eventData) {
        Debug.Log(eventData.position);
    }

    public void OnPointerDown(PointerEventData eventData) {

    }
}
