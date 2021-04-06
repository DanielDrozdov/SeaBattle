using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightFieldStateController : MonoBehaviour
{
    private Dictionary<char, Dictionary<int, Vector2>> fieldPoints;
    private char[] fieldLettersMassive = { 'a','b','c','d','e','f','g','h','i','j' };
    private List<Ship> shipsList;

    [SerializeField] private Sprite HitCrossSprite;
    [SerializeField] private Sprite NothingInCellSprite;
    private static FightFieldStateController Instance;

    private FightFieldStateController() { }

    private void Awake() {
        Instance = this;
        CalculateFieldPoints();
    }

    public static FightFieldStateController GetInstance() {
        return Instance;
    }

    public Sprite GetHitCrossSprite() {
        return HitCrossSprite;
    }

    public Sprite GetNothingInCellSpriteSprite() {
        return NothingInCellSprite;
    }

    private void CalculateFieldPoints() {
        fieldPoints = new Dictionary<char, Dictionary<int, Vector2>>();
        float xMin, xMax, yMin, yMax;
        float fieldXSize;
        RectTransform rect = GetComponent<RectTransform>();
        xMin = transform.position.x - rect.sizeDelta.x / 2;
        xMax = transform.position.x + rect.sizeDelta.x / 2;
        yMin = transform.position.y - rect.sizeDelta.y / 2;
        yMax = transform.position.y + rect.sizeDelta.y / 2;
        fieldXSize = Mathf.Abs(xMax - xMin);
        float cellSizeDelta = fieldXSize / 10;
        for(int i = 0; i < 10; i++) {
            Dictionary<int, Vector2> letterPoints = new Dictionary<int, Vector2>();
            for(int k = 0; k < 10; k++) {
                letterPoints.Add(k + 1, new Vector2(cellSizeDelta * k + cellSizeDelta / 2 + xMin, cellSizeDelta * i + cellSizeDelta / 2 + yMin));
            }
            fieldPoints.Add(fieldLettersMassive[i], letterPoints);
        }
    }
}

abstract class Ship {
    Vector2[] shipLocationPoints = { Vector2.zero, Vector2.zero, Vector2.zero };
}
