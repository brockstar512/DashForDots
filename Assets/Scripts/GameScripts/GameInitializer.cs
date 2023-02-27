using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Cinemachine;

public class GameInitializer : MonoBehaviour
{
    StateManager stateManager;
    [SerializeField] Transform TwoPersonBoard;
    [SerializeField] Transform ThreePersonBoard;
    [SerializeField] Transform FourPersonBoard;
    Transform currentBoard;


    //6 for 2 (causing problems for min zoom moving to 7)
    //12 for 3
    //24 for 4  

    //create a countdown
    private void Awake()
    {
        stateManager = GetComponent<StateManager>();
        StartGame();
    }


    async void StartGame()
    {
        //create all the plyers
        int total_PlayerCount = LocalGameController.playerCount + LocalGameController.botCount;
        await PlayerHandler.Instance.Init((PlayerCount)total_PlayerCount);
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
