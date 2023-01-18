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

    //6 for 2 
    //12 for 3
    //24 for 4  

    private void Awake()
    {
        stateManager = GetComponent<StateManager>();
        StartGame();
    }

    async void StartGame()
    {
        //create all the plyers
        await PlayerHandler.Instance.Init((PlayerCount)LocalGameController.playerCount);

        switch ((PlayerCount)LocalGameController.playerCount)
        {
            case (PlayerCount)2:
                currentBoard = Instantiate(TwoPersonBoard);
                break;
            case (PlayerCount)3:
                currentBoard = Instantiate(ThreePersonBoard);
                break;
            case (PlayerCount)4:
                currentBoard = Instantiate(FourPersonBoard);
                break;
        }

        await stateManager.Init(currentBoard);
        //delay before we actually start the game
    }

    void ConfigBoard()
    {

    }

}
