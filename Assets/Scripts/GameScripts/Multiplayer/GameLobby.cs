using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class GameLobby : NetworkBehaviour
{
    private NetworkVariable<FixedString64Bytes> gameCode = new NetworkVariable<FixedString64Bytes>();
    public event EventHandler OnGameJoinStarted;
    public event EventHandler<OnGameJoinFailedEventArgs> OnGameJoinFailed;
    public class OnGameJoinFailedEventArgs : EventArgs
    {
        public string message;
    }
    private static GameLobby instance;
    public static GameLobby Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject gameObject = new GameObject("GameLobby");
                gameObject.AddComponent<NetworkObject>();
                instance = gameObject.AddComponent<GameLobby>();
                DontDestroyOnLoad(gameObject);
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
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MultiplayerController.Instance.PlayerCount.Value - 1);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            gameCode = new NetworkVariable<FixedString64Bytes>();
            gameCode.Value = joinCode;
            Debug.Log("Joining code " + joinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
            MultiplayerController.Instance.StartHost();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
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
            MultiplayerController.Instance.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e.Message);
            OnGameJoinFailed?.Invoke(this, new OnGameJoinFailedEventArgs() { message = e.Message });
        }
    }

    public string GetGameCode()
    {
        return gameCode.Value.ToString();
    }

}
