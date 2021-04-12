using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    [SerializeField] private CellPointPos[] shipPointsMassive;
    [SerializeField] private ShipAttackZoneController shipAttackZone;
    [HideInInspector] public List<CellPointPos> shipPoints;
    [HideInInspector] public List<CellPointPos> shipHitPoints;
    private SpriteRenderer spriteRenderer;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        shipPoints = new List<CellPointPos>();
        shipHitPoints = new List<CellPointPos>();
        for(int i = 0; i < shipPointsMassive.Length; i++) {
            shipPoints.Add(shipPointsMassive[i]);
            shipHitPoints.Add(shipPointsMassive[i]);
        }
    }

    private void OnMouseUp() {
        ServiceManager.GetInstance().SetNewShipAttackZone(shipAttackZone.gameObject, shipPoints.Count);
    }

    public void DestroyShip() {
        spriteRenderer.enabled = true;
    }
}
