using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class GameLobby : NetworkBehaviour
{
    private string gameCode;
    public event EventHandler OnGameJoinStarted;
    public event EventHandler<OnGameJoinFailedEventArgs> OnGameCreateJoinFailed;
    public class OnGameJoinFailedEventArgs : EventArgs
    {
        public string message;
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

    public async void HostGame()
    {
        try
        {
            OnGameJoinStarted?.Invoke(this, EventArgs.Empty);
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MultiplayerController.Instance.PlayerCount.Value);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            gameCode = joinCode;
            Debug.Log("Joining code " + joinCode + "\n" + MultiplayerController.Instance.PlayerCount.Value);
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
    public string GetGameCode()
    {
        return gameCode.ToString();
    }
}
