using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MultiplayerController : NetworkBehaviour
{
    #region  refrences
    private static MultiplayerController instance;
    public NetworkVariable<int> MaxPlayerCount;//minimum plyer count 2 for multiplayer  
    public event EventHandler<OnPlayerConnectedEventArgs> OnPlayerConnected;
    public NetworkVariable<bool> isMutiplayer = new NetworkVariable<bool>(false);
    public bool IsMultiplayer
    {
        get
        {
            return isMutiplayer.Value || Constants.GAME_TYPE == (int)Enums.GameType.Multiplayer;
        }
    }
    public NetworkVariable<bool> isGameStarted = new NetworkVariable<bool>();
    public bool IsGameStarted
    {
        get
        {
            return isGameStarted.Value;
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
    public NetworkList<PlayerTurn> playerTurnList { get; private set; }
    public NetworkVariable<ulong> rejoinPlayerConnected = new NetworkVariable<ulong>();
    private bool isSycningGame;
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
        // PlayerCount = new NetworkVariable<int>(2);
        playerTurnList = new NetworkList<PlayerTurn>();
    }

    private void Start()
    {
        //Sign up for unity services in order to use Relay services.  
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        GameLobby.Instance.InitializeUnityAuthentication();
        playerNetworkList.OnListChanged += PlayerNetworkList_OnListUpdate;
        // SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    private void SceneManager_activeSceneChanged(Scene current, Scene next)
    {
        if (current.name == LoadingManager.Scene.MainMenu.ToString())
        {
            Reset();
        }
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
        var connectionData = connectionApprovalRequest.Payload;
        var playerId = System.Text.Encoding.UTF8.GetString(connectionData);
        if (IsExistingPlayer(playerId).Item1)
        {
            connectionApprovalResponse.Approved = true;
            connectionApprovalResponse.Reason = "Rejoin the game";
            return;
        }
        if (IsGameStarted)
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game Already Started";
            return;
        }
        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MaxPlayerCount.Value)
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "No Room Avaliable";
            return;
        }
        connectionApprovalResponse.Approved = true;
    }
    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        if (!IsGameStarted)
        {
            if (!isMaxPlayerSet)
            {
                MaxPlayerCount = new NetworkVariable<int>();
                MaxPlayerCount.Value = maxPlayerCount;
                isMaxPlayerSet = true;
            }
            playerNetworkList.Add(new MultiplayerData
            {
                status = (int)Enums.PlayerState.Active,
                clientId = clientId,
                currentIndex = playerNetworkList.Count,
                serverIndex = playerNetworkList.Count,
                playerName = "Player " + (playerNetworkList.Count + 1),
                colorId = 1,
            });
            SetPlayerServerRpc(AuthenticationService.Instance.PlayerId, NetworkManager.Singleton.IsHost);
            OnPlayerConnected?.Invoke(this, new OnPlayerConnectedEventArgs() { clientId = clientId, isClientJoined = true });
            isMutiplayer.Value = true;

        }
        else
        {
            isSycningGame = true;
        }

    }

    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
    {
        for (int i = 0; i < playerNetworkList.Count; i++)
        {
            MultiplayerData playerData = playerNetworkList[i];
            if (playerData.clientId == clientId)
            {
                if (!IsGameStarted)
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
        NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes(AuthenticationService.Instance.PlayerId);
        NetworkManager.Singleton.StartClient();
    }

    private void NetworkManager_Client_OnClientConnectedCallback(ulong clientId)
    {
        SetPlayerServerRpc(AuthenticationService.Instance.PlayerId, NetworkManager.Singleton.IsHost);
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
            OnPlayerConnected?.Invoke(this, new OnPlayerConnectedEventArgs() { clientId = clientId, isClientJoined = false });
        }
    }
    #endregion

    #region RPC Calls

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerServerRpc(string playerId, bool isHost, ServerRpcParams serverRpcParams = default)
    {
        if (!IsGameStarted)
        {
            int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
            MultiplayerData playerData = playerNetworkList[playerDataIndex];
            playerData.playerId = playerId;
            playerData.isHost = isHost;
            playerData.status = (int)Enums.PlayerState.Active;
            playerNetworkList[playerDataIndex] = playerData;
        }
        else
        {
            (bool flag, MultiplayerData multiplayerData) = IsExistingPlayer(playerId);
            if (flag)
            {
                multiplayerData.clientId = serverRpcParams.Receive.SenderClientId;
                multiplayerData.isRejoin = true;
                multiplayerData.status = (int)Enums.PlayerState.Active;
                int index = GetPlayerDataIndexFromPlayerId(playerId);
                playerNetworkList[index] = multiplayerData;
                rejoinPlayerConnected.Value = serverRpcParams.Receive.SenderClientId;
            }

        }
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
            Reset();
        }
    }

    private void Reset()
    {
        playerNetworkList.Clear();
        isMutiplayer = new NetworkVariable<bool>();
        isGameStarted = new NetworkVariable<bool>();
        // PlayerCount = new NetworkVariable<int>(2);
        isMaxPlayerSet = false;
        NetworkManager.Singleton.Shutdown(true);
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
    private int maxPlayerCount = 2;
    private bool isMaxPlayerSet = false;
    public void SetPlayerCount(int count)
    {
        maxPlayerCount = count;
    }
    public void ResetPlayerCount()
    {
        MaxPlayerCount = new NetworkVariable<int>(2);
    }
    public NetworkList<MultiplayerData> GetPlayerList()
    {
        return playerNetworkList;
    }
    public bool CanHostStartTheGame()
    {
        return playerNetworkList.Count == MaxPlayerCount.Value;
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
    public int GetPlayerDataIndexFromPlayerId(string playerId)
    {
        for (int i = 0; i < playerNetworkList.Count; i++)
        {
            if (playerNetworkList[i].playerId == playerId)
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
    public bool IsMyTurn(int currentplayer)
    {
        if (IsMultiplayer)
        {
            MultiplayerData multiplayerData = GetPlayerDataFromPlayerIndex(currentplayer);
            bool flag = multiplayerData.clientId == NetworkManager.LocalClientId;
            return flag;
        }
        return false;
    }

    public (bool, MultiplayerData) IsExistingPlayer(string playerId)
    {
        foreach (MultiplayerData playerData in playerNetworkList)
        {
            if (playerData.playerId == playerId)
            {
                return (true, playerData);
            }
        }
        return (false, default);
    }

    public void SetPlayerTurn(PlayerTurn turn)
    {
        playerTurnList.Add(turn);
    }
    public void SetGameStarted(bool flag)
    {
        isGameStarted.Value = flag;
    }
    public void UpdatePlayerScore(int CurrentPlayer, int score)
    {
        MultiplayerData multiplayerData = GetPlayerDataFromPlayerIndex(CurrentPlayer);
        multiplayerData.score = score;
        playerNetworkList[CurrentPlayer] = multiplayerData;

    }
    #endregion
}
