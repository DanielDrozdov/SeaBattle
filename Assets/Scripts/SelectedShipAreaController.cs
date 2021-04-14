using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedShipAreaController : MonoBehaviour
{
    private SpriteRenderer sprite;
    private Color greenAreaColor;
    private Color redAreaColor;
    private float areaTransparentValue = 0.35f;

    private void Awake() {
        sprite = GetComponent<SpriteRenderer>();
        greenAreaColor = new Color(Color.green.r, Color.green.g, Color.green.b, areaTransparentValue);
        redAreaColor = new Color(Color.red.r, Color.red.g, Color.red.b, areaTransparentValue);
    }

    public void ActivateArea() {
        gameObject.SetActive(true);
    }

    public void DeactivateArea() {
        gameObject.SetActive(false);
    }

    public void ActivateRedState() {
        sprite.color = redAreaColor;
    }

    public void ActivateGreenState() {
        sprite.color = greenAreaColor;
    }
}
