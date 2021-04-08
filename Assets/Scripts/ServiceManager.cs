using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceManager : MonoBehaviour
{
    private static ServiceManager Instance;
    private GameObject lastActivatedShipAttackZone;
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

    public void ResetLastShipAttackZone(GameObject shipAttackZone) {
        if(shipAttackZone == lastActivatedShipAttackZone) {
            return;
        }
        lastActivatedShipAttackZone.SetActive(false);
    }

    public void SetNewShipAttackZone(GameObject shipAttackZone) {
        shipAttackZone.SetActive(true);
        lastActivatedShipAttackZone = shipAttackZone;
    }
}
