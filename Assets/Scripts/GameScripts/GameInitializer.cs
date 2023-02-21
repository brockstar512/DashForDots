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
                maxLensZoom = 7;
                break;
            case (PlayerCount)3:
                currentBoard = Instantiate(ThreePersonBoard);
                maxLensZoom = 12;
                break;
            case (PlayerCount)4:
                currentBoard = Instantiate(FourPersonBoard);
                maxLensZoom = 24;
                break;
        }

        await stateManager.Init(currentBoard, maxLensZoom);
        //delay before we actually start the game
    }



}
