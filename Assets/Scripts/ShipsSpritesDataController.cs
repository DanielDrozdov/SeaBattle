using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipsSpritesDataController : MonoBehaviour
{
    [SerializeField] private Sprite fourCellDestroyShipSprite;
    [SerializeField] private Sprite threeCellDestroyShipSprite;
    [SerializeField] private Sprite twoCellDestroyShipSprite;
    [SerializeField] private Sprite oneCellDestroyShipSprite;
    private static ShipsSpritesDataController Instance;

    private void Awake() {
        Instance = this;
    }

    public static ShipsSpritesDataController GetInstance() {
        return Instance;
    }

    public Sprite GetDestroyShipSprite(int sizeInCells) {
        if(sizeInCells == 4) {
            return fourCellDestroyShipSprite;
        } else if(sizeInCells == 3) {
            return threeCellDestroyShipSprite;
        } else if(sizeInCells == 2) {
            return twoCellDestroyShipSprite;
        } else {
            return oneCellDestroyShipSprite;
        }
    }
}
