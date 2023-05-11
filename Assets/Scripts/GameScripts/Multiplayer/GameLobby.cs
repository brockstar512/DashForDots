using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLobby : NetworkBehaviour
{
    private string gameCode;
    public event EventHandler OnGameJoinStarted;
    public event EventHandler<OnGameJoinFailedEventArgs> OnGameCreateJoinFailed;
    public event EventHandler<OnGameJoinFailedEventArgs> OnQuickGameFailed;
    public class OnGameJoinFailedEventArgs : EventArgs
    {
        public string message;
        public int errorCode;
    }
    private static GameLobby instance;
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
            InitializeUnityAuthentication();
        }
    }
    public static GameLobby Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameLobby>();
            }
            return instance;
        }
    }
    public async void InitializeUnityAuthentication()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions initializationOptions = new InitializationOptions();
            await UnityServices.InitializeAsync(initializationOptions);
            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log($"Sign In " + AuthenticationService.Instance.PlayerId);
            };
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    public async void HostGame(int playerCount)
    {
        try
        {
            await AuthenticateUnityServices();
            OnGameJoinStarted?.Invoke(this, EventArgs.Empty);
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(playerCount);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            gameCode = joinCode;
            Debug.Log("Joining code " + joinCode + "\n" + playerCount);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
            MultiplayerController.Instance.StartHost();
        }
        catch (RelayServiceException e)
        {
            OnGameCreateJoinFailed?.Invoke(this, new OnGameJoinFailedEventArgs() { message = e.Message });
        }
    }

    public async void JoinGame(string roomCode)
    {
        OnGameJoinStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            await AuthenticateUnityServices();
            Debug.Log("Joining Relay with " + roomCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(roomCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            gameCode = roomCode;
            MultiplayerController.Instance.StartClient();
        }
        catch (RelayServiceException e)
        {
            OnGameCreateJoinFailed?.Invoke(this, new OnGameJoinFailedEventArgs() { message = Utility.GetErrorMessage(e.ErrorCode) });
        }
    }

    private static async Task AuthenticateUnityServices()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions initializationOptions = new InitializationOptions();
            await UnityServices.InitializeAsync(initializationOptions);
        }
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    public string GetGameCode()
    {
        return gameCode.ToString();
    }

    #region Lobby
    private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";
    private Lobby joinedLobby;
    private float heartbeatTimer;
    private float listLobbiesTimer;
    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
    public class OnLobbyListChangedEventArgs : EventArgs
    {
        public List<Lobby> lobbyList;
    }
    private void Update()
    {
        HandleHeartbeat();
        HandlePeriodicListLobbies();        
    }
    private void HandlePeriodicListLobbies()
    {
        if (joinedLobby == null &&
            UnityServices.State == ServicesInitializationState.Initialized &&
            AuthenticationService.Instance.IsSignedIn &&
            SceneManager.GetActiveScene().name == LoadingManager.Scene.MainMenu.ToString())
        {

            listLobbiesTimer -= Time.deltaTime;
            if (listLobbiesTimer <= 0f)
            {
                float listLobbiesTimerMax = 3f;
                listLobbiesTimer = listLobbiesTimerMax;
                ListLobbies();
            }
        }
    }


    private void HandleHeartbeat()
    {
        if (IsLobbyHost())
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer <= 0f)
            {
                float heartbeatTimerMax = 15f;
                heartbeatTimer = heartbeatTimerMax;
                LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }
    private bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    private async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Filters = new List<QueryFilter> {
                  new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
             }
            };
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs
            {
                lobbyList = queryResponse.Results
            });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    public async void CreateLobby(string lobbyName, bool isPrivate, int playerCount)
    {
        try
        {
            OnGameJoinStarted?.Invoke(this, EventArgs.Empty);
            await AuthenticateUnityServices();
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, playerCount, new CreateLobbyOptions
            {
                IsPrivate = isPrivate,
            });
            Allocation allocation = await AllocateRelay(playerCount);
            string relayJoinCode = await GetRelayJoinCode(allocation);
            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject> {
                     { KEY_RELAY_JOIN_CODE , new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                 }
            });
            gameCode = joinedLobby.LobbyCode;
            Debug.Log("Joining code " + gameCode + "\n" + playerCount);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
            MultiplayerController.Instance.StartHost(!isPrivate);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            OnGameCreateJoinFailed?.Invoke(this, new OnGameJoinFailedEventArgs() { message = e.Message });
        }
    }
    public async void JoinWithId(string lobbyId)
    {
        OnGameJoinStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            await AuthenticateUnityServices();
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            gameCode = joinedLobby.LobbyCode;
            MultiplayerController.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            OnGameCreateJoinFailed?.Invoke(this, new OnGameJoinFailedEventArgs() { message = Utility.GetErrorMessage(e.ErrorCode) });
        }
    }

    public async void JoinWithCode(string lobbyCode)
    {
        OnGameJoinStarted?.Invoke(this, EventArgs.Empty);
        try
        {           
            Debug.Log("Joining Relay with " + lobbyCode);
            await AuthenticateUnityServices();
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            gameCode = lobbyCode;
            ToastMessage.Show(AuthenticationService.Instance.PlayerId);
            MultiplayerController.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            if (e.ErrorCode.Equals(16003))
            {
                LeaveLobby();               
            }
            OnGameCreateJoinFailed?.Invoke(this, new OnGameJoinFailedEventArgs() { message = Utility.GetErrorMessage(e.ErrorCode) });
        }
    }
    public async void QuickJoin(int playerCount)
    {
        OnGameJoinStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            await AuthenticateUnityServices();
            QuickJoinLobbyOptions options = new QuickJoinLobbyOptions();

            options.Filter = new List<QueryFilter>()
                 {
                  new QueryFilter(
                  field: QueryFilter.FieldOptions.MaxPlayers,
                  op: QueryFilter.OpOptions.EQ,
                  value: playerCount.ToString())
                  };

            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);
            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            MultiplayerController.Instance.StartClient(true);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e.Message);
            OnQuickGameFailed?.Invoke(this, new OnGameJoinFailedEventArgs() { message = e.Message, errorCode = e.ErrorCode });
        }
    }
    private async Task<Allocation> AllocateRelay(int playerCount)
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(playerCount);
            return allocation;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }

    private async Task<string> GetRelayJoinCode(Allocation allocation)
    {
        try
        {
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return relayJoinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }

    private async Task<JoinAllocation> JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            return joinAllocation;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }
    public async void DeleteLobby()
    {        
        if (joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
                joinedLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async void LeaveLobby()
    {
        if (joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                joinedLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async void KickPlayer(string playerId)
    {
        if (IsLobbyHost())
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    #endregion


}
