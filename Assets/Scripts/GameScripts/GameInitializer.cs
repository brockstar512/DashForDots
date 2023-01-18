using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class GameInitializer : MonoBehaviour
{
    StateManager stateManager;
    [SerializeField] Transform TwoPersonBoard;
    [SerializeField] Transform ThreePersonBoard;
    [SerializeField] Transform FourPersonBoard;
    Transform currentBoard;



    private void Awake()
    {
        stateManager = GetComponent<StateManager>();
        StartGame();
    }

    async void StartGame()
    {
        await PlayerHandler.Instance.Init((PlayerCount)LocalGameController.playerCount);
        await stateManager.Init();
        //delay before we actually start the game
    }

    void ConfigBoard()
    {

    }

}
