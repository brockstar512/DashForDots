using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerController : NetworkBehaviour
{
    #region  refrences
    private static MultiplayerController instance;
    public int PlayerCount { get; private set; } = 2;//minimum plyer count 2 for multiplayer
    public static int MAX_PLAYER_AMOUNT { get; private set; } = 4;
    public event EventHandler<OnPlayerConnectedEventArgs> OnPlayerConnected;
    public class OnPlayerConnectedEventArgs : EventArgs
    {
        public ulong clientId;
        public bool isClientJoined;
    }

    private NetworkList<MultiplayerData> playerNetworkList;
    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;
    public event EventHandler OnPlayerDataNetworkListChanged;
    #endregion
    public static MultiplayerController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MultiplayerController>();
            }
            return instance;
        }
    }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        playerNetworkList = new NetworkList<MultiplayerData>();
    }
    private void Start()
    {
        //Sign up for unity services in order to use Relay services.  
        GameLobby.Instance.InitializeUnityAuthentication();
        playerNetworkList.OnListChanged += PlayerNetworkList_OnListUpdate;
    }


    #region Player Count
    public void SetPlayerCount(int count)
    {
        PlayerCount = count;
        Debug.Log($"Total multiplayer count : {PlayerCount}");
    }
    #endregion
    #region Multiplayer
    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        Constants.GAME_TYPE = (int)Enums.GameType.Multiplayer;
        NetworkManager.Singleton.StartHost();
    }
    public void StartClient()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
        Constants.GAME_TYPE = (int)Enums.GameType.Multiplayer;
        NetworkManager.Singleton.StartClient();
    }

    public override void OnNetworkSpawn()
    {
        //LoadingManager.Instance.LoadScene(LoadingManager.Scene.Game.ToString());
    }
    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        playerNetworkList.Add(new MultiplayerData
        {
            clientId = clientId,
            colorId = 1,
        });
        SetPlayerNameServerRpc("Player_" + NetworkManager.Singleton.LocalClientId);
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
        OnPlayerConnected?.Invoke(this, new OnPlayerConnectedEventArgs() { clientId = clientId, isClientJoined = true });
    }
    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
    {
        Debug.LogError($"Server_OnClientDisconnectCallback client id {clientId} IsServer {IsServer} IsHost {IsHost}");
        for (int i = 0; i < playerNetworkList.Count; i++)
        {
            MultiplayerData playerData = playerNetworkList[i];
            if (playerData.clientId == clientId)
            {
                playerNetworkList.RemoveAt(i);
            }
        }
        OnPlayerConnected?.Invoke(this, new OnPlayerConnectedEventArgs() { clientId = clientId, isClientJoined = false });
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT)
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "No Room Avaliable";
            Debug.Log($"NetworkManager_ConnectionApprovalCallback false");
            return;
        }
        Debug.Log($"NetworkManager_ConnectionApprovalCallback true");
        connectionApprovalResponse.Approved = true;
    }

    private void NetworkManager_Client_OnClientConnectedCallback(ulong obj)
    {
        SetPlayerNameServerRpc("Player_" + NetworkManager.Singleton.LocalClientId);
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
        OnPlayerConnected?.Invoke(this, new OnPlayerConnectedEventArgs() { clientId = obj, isClientJoined = true });
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        MultiplayerData playerData = playerNetworkList[playerDataIndex];

        playerData.playerName = playerName;

        playerNetworkList[playerDataIndex] = playerData;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerIdServerRpc(string playerId, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        MultiplayerData playerData = playerNetworkList[playerDataIndex];

        playerData.playerId = playerId;

        playerNetworkList[playerDataIndex] = playerData;
    }
    private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId)
    {
        Debug.LogError($"Client_OnClientDisconnectCallback client id {clientId} IsServer {IsServer} IsHost {IsHost}");
        for (int i = 0; i < playerNetworkList.Count; i++)
        {
            MultiplayerData multiplayerData = playerNetworkList[i];
            if (multiplayerData.clientId == clientId)
            {
                playerNetworkList.RemoveAt(i);
            }
        }
        OnPlayerConnected?.Invoke(this, new OnPlayerConnectedEventArgs() { clientId = clientId, isClientJoined = false });
    }
    private void PlayerNetworkList_OnListUpdate(NetworkListEvent<MultiplayerData> changeEvent)
    {
        Debug.Log($"Total player {playerNetworkList.Count}");
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool IsPlayerIndexConnected(int playerIndex)
    {
        return playerIndex < playerNetworkList.Count;
    }

    public int GetPlayerDataIndexFromClientId(ulong clientId)
    {
        for (int i = 0; i < playerNetworkList.Count; i++)
        {
            if (playerNetworkList[i].clientId == clientId)
            {
                return i;
            }
        }
        return -1;
    }

    public MultiplayerData GetPlayerDataFromClientId(ulong clientId)
    {
        foreach (MultiplayerData playerData in playerNetworkList)
        {
            if (playerData.clientId == clientId)
            {
                return playerData;
            }
        }
        return default;
    }

    public MultiplayerData GetPlayerData()
    {
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }

    public MultiplayerData GetPlayerDataFromPlayerIndex(int playerIndex)
    {
        return playerNetworkList[playerIndex];
    }


    public void ShutDown()
    {
        Constants.GAME_TYPE = (int)Enums.GameType.None;
        OnServerDisconnect();
        NetworkManager.Singleton.ConnectionApprovalCallback -= NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback -= NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_Server_OnClientDisconnectCallback;
    }

    private void OnServerDisconnect()
    {
        if (!IsServer)
        {
            NetworkManager.Singleton.Shutdown(true);
            NetworkManager_Server_OnClientDisconnectCallback(NetworkManager.Singleton.LocalClientId);
        }
        else
        {
            List<NetworkClient> connectedPlayers = (List<NetworkClient>)NetworkManager.Singleton.ConnectedClientsList;

            MultiplayerData multiplayerHost = new MultiplayerData();
            for (int i = 0; i < connectedPlayers.Count; i++)
            {
                NetworkClient player = connectedPlayers[i];
                if (player.ClientId == NetworkManager.ServerClientId)
                {
                    multiplayerHost = playerNetworkList[i];
                }
                else
                {
                    MultiplayerData multiplayerData = playerNetworkList[i];
                    playerNetworkList.Remove(multiplayerData);
                    ChangeClietSceneServerRpc(multiplayerData.clientId);
                }
            }


            playerNetworkList.Remove(multiplayerHost);
            NetworkManager.Singleton.Shutdown(true);
        }
    }

    [ServerRpc]
    private void ChangeClietSceneServerRpc(ulong clientId)
    {      
        NetworkManager.Singleton.DisconnectClient(clientId);
        // NetworkManager.Singleton.Shutdown(true);
    }


    public NetworkList<MultiplayerData> GetPlayerList()
    {
        return playerNetworkList;
    }
    #endregion



}
