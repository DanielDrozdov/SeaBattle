using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkHelpManager : NetworkBehaviour {

    private NetworkManager networkManager;

    private void Awake() {
        networkManager = GetComponent<NetworkManager>();
    }

    private void Update() {
        Debug.Log(networkManager.isNetworkActive);
    }
}
