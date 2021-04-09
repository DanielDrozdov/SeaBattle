using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceManager : MonoBehaviour
{
    private static ServiceManager Instance;
    private GameObject lastActivatedShipAttackZone;
    private float lastShipCellsCount;
    private Camera mainCamera;

    private ServiceManager() { }

    void Awake()
    {
        Instance = this;
        mainCamera = Camera.main;
    }

    public static ServiceManager GetInstance() {
        return Instance;
    }

    public Camera GetMainCamera() {
        return mainCamera;
    }

    public float GetLastActivatedShipCellsCount() {
        return lastShipCellsCount;
    }

    public void ResetLastShipAttackZone(GameObject shipAttackZone) {
        if(shipAttackZone == lastActivatedShipAttackZone) {
            return;
        }
        lastActivatedShipAttackZone.SetActive(false);
        lastShipCellsCount = 0;
    }

    public void SetNewShipAttackZone(GameObject shipAttackZone,float shipCellsCount) {
        shipAttackZone.SetActive(true);
        lastActivatedShipAttackZone = shipAttackZone;
        lastShipCellsCount = shipCellsCount;
    }
}
