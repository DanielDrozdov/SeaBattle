﻿using System.Collections;
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

    public void SetNewShipAttackZone(GameObject shipAttackZone,float shipCellsCount) {
        if(lastActivatedShipAttackZone != null) {
            lastActivatedShipAttackZone.SetActive(false);
        }
        lastActivatedShipAttackZone = shipAttackZone;
        shipAttackZone.SetActive(true);
        lastShipCellsCount = shipCellsCount;
    }
}
