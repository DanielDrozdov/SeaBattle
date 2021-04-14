using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSceneTransitionController : MonoBehaviour
{
    private static DataSceneTransitionController Instance;
    private List<CellPointPos[]> selectedShipPoints;

    private DataSceneTransitionController() { }

    private void Awake() {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static DataSceneTransitionController GetInstance() {
        return Instance;
    }

    public void SetSelectedShips(List<CellPointPos[]> shipPoints) {
        selectedShipPoints = shipPoints;
    }

    public List<CellPointPos[]> GetSelectedShipPoints() {
        return selectedShipPoints;
    }
}
