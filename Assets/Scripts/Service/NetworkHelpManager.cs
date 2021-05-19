using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Discovery;
using UnityEngine.Events;


[DisallowMultipleComponent]
[RequireComponent(typeof(NetworkDiscovery))]
public class NetworkHelpManager : MonoBehaviour {


    private bool IsHost;
    private static NetworkHelpManager Instance;
    private NetworkManager networkManager;
    private NetworkDiscovery networkDiscovery;
    readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();

    public delegate void NetworkAction();

    private void Awake() {
        Instance = this;
        networkManager = NetworkManager.singleton;
        networkDiscovery = GetComponent<NetworkDiscovery>();
    }

    //#if UNITY_EDITOR
    //    void OnValidate() {
    //        if(networkDiscovery == null) {
    //            networkDiscovery = GetComponent<NetworkDiscovery>();
    //            UnityEditor.Events.UnityEventTools.AddPersistentListener(networkDiscovery.OnServerFound, AddServerToList);
    //            UnityEditor.Undo.RecordObjects(new Object[] { this, networkDiscovery }, "Set NetworkDiscovery");
    //        }
    //    }
    //#endif

    public static NetworkHelpManager GetInstance() {
        return Instance;
    }

    public bool IsServerCall() {
        return IsHost;
    }

    public bool IsAllPlayerConnected() {
        return networkManager.numPlayers == networkManager.maxConnections;
    }

    public void JoinToHost() {
        StartCoroutine(JoinConnectCoroutine());
        IsHost = false;
    }

    public void CreateGameAndBecomeHost() {
        UpdateServers();
        IsHost = true;
        NetworkManager.singleton.StartHost();
        networkDiscovery.AdvertiseServer();
    }

    public void CancelConnecting() {
        if(IsHost) {
            networkManager.StopHost();
        } else {
            networkManager.StopClient();
        }
    }

    private void UpdateServers() {
        discoveredServers.Clear();
        networkDiscovery.StartDiscovery();
    }

    private void AddServerToList(ServerResponse info) {
        discoveredServers[info.serverId] = info;
    }

    private void Connect(ServerResponse info) {
        networkManager.StartClient(info.uri);
    }

    private IEnumerator JoinConnectCoroutine() {
        UpdateServers();
        while(discoveredServers.Count == 0) {
            UpdateServers();
            yield return new WaitForSeconds(1f);
        }

        foreach(ServerResponse info in discoveredServers.Values) {
            Connect(info);
            break;
        }
    }

    [Command]
    public void CmdAction(NetworkAction action) {
        ServerAction(action);
    }

    [ClientRpc]
    private void RpcAction(NetworkAction action) {
        action?.Invoke();
    }

    [Server]
    public void ServerAction(NetworkAction action) {
        RpcAction(action);
    }
}
