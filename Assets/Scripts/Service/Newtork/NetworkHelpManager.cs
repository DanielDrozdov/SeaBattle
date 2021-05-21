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
    private MainMenuPlayerNetworkController mainMenuPlayerNetworkController;
    readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();

    private FightGamePlayerNetworkController firstFightPlayerNetworkController;
    private FightGamePlayerNetworkController secondFightPlayerNetworkController;
    private FightGamePlayerNetworkController playerFightPlayerNetworkController;

    private void Awake() {
        Instance = this;
        networkManager = NetworkManager.singleton;
        networkDiscovery = GetComponent<NetworkDiscovery>();
    }

    public static NetworkHelpManager GetInstance() {
        return Instance;
    }

    public void SetPlayerNetworkController(MainMenuPlayerNetworkController mainMenuPlayerNetworkController) {
        this.mainMenuPlayerNetworkController = mainMenuPlayerNetworkController;
    }

    public void InitializePlayersObjects() {
        FightGamePlayerNetworkController[] players = FindObjectsOfType<FightGamePlayerNetworkController>();
        foreach(FightGamePlayerNetworkController player in players) {
            if(player.isLocalPlayer) {
                playerFightPlayerNetworkController = player;
            }
        }
        firstFightPlayerNetworkController = players[0];
        secondFightPlayerNetworkController = players[1];
    }

    public FightGamePlayerNetworkController GetPlayerFightNetworkComponent(int playerNumber) {
        if(playerNumber == 1) {
            return firstFightPlayerNetworkController;
        } else {
            return secondFightPlayerNetworkController;
        }
    }

    public FightGamePlayerNetworkController GetCurrentPlayerFightNetworkController() {
        return playerFightPlayerNetworkController;
    }

    public MainMenuPlayerNetworkController GetPlayerMenuNetworkController() {
        return mainMenuPlayerNetworkController;
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
        IsHost = true;
        discoveredServers.Clear();
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
}
