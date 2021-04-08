using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class Ship : MonoBehaviour
{
    [SerializeField] private CellPointPos[] shipPointsMassive;
    [SerializeField] private ShipAttackZoneController shipAttackZone;
    [HideInInspector] public List<CellPointPos> shipPoints;
    [HideInInspector] public List<CellPointPos> shipHitPoints;

    private void Awake() {
        shipPoints = new List<CellPointPos>();
        shipHitPoints = new List<CellPointPos>();
        for(int i = 0; i < shipPointsMassive.Length; i++) {
            shipPoints.Add(shipPointsMassive[i]);
            shipHitPoints.Add(shipPointsMassive[i]);
        }
    }

    private void OnMouseDown() {
        ServiceManager.GetInstance().SetNewShipAttackZone(shipAttackZone.gameObject);
    }

    private void OnMouseUp() {
        ServiceManager.GetInstance().ResetLastShipAttackZone(shipAttackZone.gameObject);
    }
}
