using System;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerController : NetworkBehaviour
{
    #region  refrences
    private static MultiplayerController instance;
    public NetworkVariable<int> PlayerCount;//minimum plyer count 2 for multiplayer  
    public event EventHandler<OnPlayerConnectedEventArgs> OnPlayerConnected;
    public bool IsMutiplayer
    {
        get
        {
            return Constants.GAME_TYPE == (int)Enums.GameType.Multiplayer; ;
        }
    }
    public class OnPlayerConnectedEventArgs : EventArgs
    {
        public ulong clientId;
        public bool isClientJoined;
    }
    private NetworkList<MultiplayerData> playerNetworkList;
    public event EventHandler OnHostShutDown;
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
    #region MonoBehaviour Method
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
        InitializeVariable();
    }

    private void InitializeVariable()
    {
        playerNetworkList = new NetworkList<MultiplayerData>();
        PlayerCount = new NetworkVariable<int>();
    }

    private void Start()
    {
        //Sign up for unity services in order to use Relay services.  
        GameLobby.Instance.InitializeUnityAuthentication();
        playerNetworkList.OnListChanged += PlayerNetworkList_OnListUpdate;
        PlayerCount.Value = 2;
    }

    #endregion

    #region Host 

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        Constants.GAME_TYPE = (int)Enums.GameType.Multiplayer;
        NetworkManager.Singleton.StartHost();
    }
    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= PlayerCount.Value)
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "No Room Avaliable";
        }
        else
        {
            connectionApprovalResponse.Approved = true;
        }
    }
    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        playerNetworkList.Add(new MultiplayerData
        {
            status = (int)Enums.PlayerState.Active,
            isHost = NetworkManager.Singleton.IsHost,
            clientId = clientId,
            currentIndex = playerNetworkList.Count - 1,
            serverIndex = playerNetworkList.Count - 1,
            colorId = 1,
        });
        SetPlayerNameServerRpc(GetPlayerName.PlayerName);
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
        OnPlayerConnected?.Invoke(this, new OnPlayerConnectedEventArgs() { clientId = clientId, isClientJoined = true });
    }

    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
    {
        for (int i = 0; i < playerNetworkList.Count; i++)
        {
            MultiplayerData playerData = playerNetworkList[i];
            if (playerData.clientId == clientId)
            {
                if (SceneManager.GetActiveScene().name != LoadingManager.Scene.Game.ToString())
                {
                    playerNetworkList.RemoveAt(i);
                }
                else
                {
                    playerData.status = (int)Enums.PlayerState.Inactive;
                    playerNetworkList[i] = playerData;
                }
            }
        }
        OnPlayerConnected?.Invoke(this, new OnPlayerConnectedEventArgs() { clientId = clientId, isClientJoined = false });
    }

    #endregion Host

    #region Client
    public void StartClient()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= NetworkManager_Client_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        Constants.GAME_TYPE = (int)Enums.GameType.Multiplayer;
        NetworkManager.Singleton.StartClient();
    }

    private void NetworkManager_Client_OnClientConnectedCallback(ulong clientId)
    {
        SetPlayerNameServerRpc(GetPlayerName.PlayerName);
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
        OnPlayerConnected?.Invoke(this, new OnPlayerConnectedEventArgs() { clientId = clientId, isClientJoined = true });
    }

    private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId)
    {
        if (IsDisconnectedPlayerWasHost(clientId))
        {
            OnHostShutDown?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            if (!NetworkManager.IsServer && NetworkManager.DisconnectReason != string.Empty)
            {
                ToastMessage.Show(NetworkManager.DisconnectReason);
            }
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
    }
    #endregion

    #region RPC Calls
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

    private void PlayerNetworkList_OnListUpdate(NetworkListEvent<MultiplayerData> changeEvent)
    {
        Debug.Log($"Total player {playerNetworkList.Count}");
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }


    public void ShutDown()
    {
        Constants.GAME_TYPE = (int)Enums.GameType.None;
        if (IsServer)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback -= NetworkManager_ConnectionApprovalCallback;
            NetworkManager.Singleton.OnClientConnectedCallback -= NetworkManager_OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_Server_OnClientDisconnectCallback;
        }
        else
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= NetworkManager_Client_OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_Client_OnClientDisconnectCallback;
        }
        OnServerDisconnect();
    }
    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            ShutDown();
        }
    }
    private void OnServerDisconnect()
    {
        if (IsClient && !IsServer)
        {
            NetworkManager.Singleton.Shutdown(true);           
        }
        else if (IsServer)
        {
            DisconnectClientServerRpc();
            playerNetworkList.Clear();
            PlayerCount = new NetworkVariable<int>() { Value = 2 };
            NetworkManager.Singleton.Shutdown(true);
        }
    }

    [ServerRpc]
    private void DisconnectClientServerRpc()
    {
        BroadcastToAllClientRpc();
    }
    [ClientRpc]
    private void BroadcastToAllClientRpc()
    {
        NetworkManager.Singleton.Shutdown(true);
    }

    public void StartGame()
    {
        StartGameServerRpc();
    }
    [ServerRpc]
    private void StartGameServerRpc()
    {
        NetworkManager.Singleton.SceneManager.LoadScene(LoadingManager.Scene.Game.ToString(), UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    #endregion

    #region Getter    
    public void SetPlayerCount(int count)
    {
        PlayerCount.Value = count;
        Debug.Log($"Total multiplayer count : {PlayerCount.Value}");
    }
    public NetworkList<MultiplayerData> GetPlayerList()
    {
        return playerNetworkList;
    }
    public bool CanHostStartTheGame()
    {
        return playerNetworkList.Count == PlayerCount.Value;
    }
    public MultiplayerData GetPlayerData()
    {
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }

    public MultiplayerData GetPlayerDataFromPlayerIndex(int playerIndex)
    {
        return playerNetworkList[playerIndex];
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
    public bool IsDisconnectedPlayerWasHost(ulong clientId)
    {
        for (int i = 0; i < playerNetworkList.Count; i++)
        {
            if (playerNetworkList[i].clientId == clientId)
            {
                return playerNetworkList[i].isHost;

            }
        }
        return false;
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
    #endregion
}
