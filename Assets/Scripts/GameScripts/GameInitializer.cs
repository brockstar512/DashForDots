using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Cinemachine;
using Unity.Netcode;
using System;

public class GameInitializer : NetworkBehaviour
{
    StateManager stateManager;
    [SerializeField] Transform TwoPersonBoard;
    [SerializeField] Transform ThreePersonBoard;
    [SerializeField] Transform FourPersonBoard;
    Transform currentBoard;
    bool isMultiplayer = false;

    //6 for 2 (causing problems for min zoom moving to 7)
    //12 for 3
    //24 for 4  

    //create a countdown
    private void Awake()
    {
        stateManager = GetComponent<StateManager>();
        isMultiplayer = MultiplayerController.Instance.IsMultiplayer;
        if (!isMultiplayer)
        {
            StartGame(isMultiplayer);
        }
        else if (isMultiplayer)
        {
            LoadingManager.Instance.Show();
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }

    public override void OnNetworkSpawn()
    {
        MultiplayerController.Instance.rejoinPlayerConnected.OnValueChanged += OnRejoinPlayerValueChanged;
    }

    private void OnRejoinPlayerValueChanged(ulong previousValue, ulong newValue)
    {        
        if (newValue == NetworkManager.LocalClientId)
        {
            MultiplayerData multiplayerData = MultiplayerController.Instance.GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
            if (multiplayerData.isRejoin)
            {
                StartGame(true);
            }
        }
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        StartGame(true);
    }


    async void StartGame(bool isMultiplayer)
    {
        LoadingManager.Instance.Hide();
        if (IsServer)
        {
            MultiplayerController.Instance.SetGameStarted(true);
        }
        int total_PlayerCount;
        total_PlayerCount = isMultiplayer ? MultiplayerController.Instance.GetPlayerList().Count : LocalGameController.playerCount + LocalGameController.botCount;
        await PlayerHandler.Instance.Init((PlayerCount)total_PlayerCount, isMultiplayer);
        int maxLensZoom = 0;
        switch ((PlayerCount)total_PlayerCount)
        {
            case (PlayerCount)2:
                currentBoard = Instantiate(TwoPersonBoard);
                maxLensZoom = 6;
                PlayerHandler.Instance.GetPlayerCount = Enums.PlayerCount.TowPlayer;
                PlayerHandler.Instance.stateManager.ResetConfiner(PlayerHandler.Instance.stateManager.TwoConfiner);
                PlayerHandler.Instance.stateManager.selectedBoard.GetComponent<BoxCollider>().size = new Vector3(14f, 20f, 7.22f);
                PlayerHandler.Instance.stateManager.selectedBoard.GetComponent<BoxCollider>().center = new Vector3(0f, 0f, -1.08f);

                break;
            case (PlayerCount)3:
                currentBoard = Instantiate(ThreePersonBoard);
                maxLensZoom = 12;
                PlayerHandler.Instance.GetPlayerCount = Enums.PlayerCount.ThreePlayer;
                PlayerHandler.Instance.stateManager.ResetConfiner(PlayerHandler.Instance.stateManager.ThreeConfiner);
                PlayerHandler.Instance.stateManager.selectedBoard.GetComponent<BoxCollider>().size = new Vector3(18f, 24f, 7.22f);
                PlayerHandler.Instance.stateManager.selectedBoard.GetComponent<BoxCollider>().center = new Vector3(0f, 0f, -1.08f);
                break;
            case (PlayerCount)4:
                currentBoard = Instantiate(FourPersonBoard);
                maxLensZoom = 24;
                PlayerHandler.Instance.GetPlayerCount = Enums.PlayerCount.FourPlayer;
                PlayerHandler.Instance.stateManager.ResetConfiner(PlayerHandler.Instance.stateManager.FourConfiner);
                PlayerHandler.Instance.stateManager.selectedBoard.GetComponent<BoxCollider>().size = new Vector3(27f, 48f, 7.22f);
                PlayerHandler.Instance.stateManager.selectedBoard.GetComponent<BoxCollider>().center = new Vector3(0f, 0f, -1.08f);
                break;
        }       
        await stateManager.Init(currentBoard, maxLensZoom);
        //delay before we actually start the game
    }



}
