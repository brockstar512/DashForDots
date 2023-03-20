using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using DG.Tweening;
using DashForDots.AI;
using Unity.Netcode;

public class PlayerHandler : NetworkBehaviour
{
    public static PlayerHandler Instance { get; private set; }
    public List<PlayerData> players { get; private set; }
    public PlayerData player { get { return players[currentPlayer]; } }
    public int currentPlayer{ get; private set; }
    public Enums.CurrentPlayerTurn CurrentPlayerTurn;
    public Enums.PlayerCount GetPlayerCount;
    public List<Transform> playerScoreDots;
    public List<Transform> playerUIDots;
    [SerializeField] Transform scoreDotParent;
    [SerializeField] Transform mainBoardDotParent;
    [SerializeField] TimerManager timerManager;
    [SerializeField] GameOverManager gameOverManager;

    public AIHandler aiHandler;
    public StateManager stateManager;
    public GridManager gridManager;
    //StopIntrection with board
    [SerializeField] GameObject boardIntrection;

    const int maxPlayerCount = 4;
    public int maxPlayerScore { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public async Task Init(PlayerCount playerCount)
    {
        players = new List<PlayerData>();
        playerScoreDots = new List<Transform>();
        playerUIDots = new List<Transform>();

        for(int i = 0;i < maxPlayerCount; i++)
        {
            if (i < (int)playerCount - LocalGameController.botCount )
            {
                playerScoreDots.Add(scoreDotParent.GetChild(i));
                playerScoreDots[i].name = "Real player" + 1;
                playerUIDots.Add(mainBoardDotParent.GetChild(i));
                players.Add(new PlayerData((PlayerCount)i + 1));

                mainBoardDotParent.GetChild(i).gameObject.GetComponent<Player>().playerType = Enums.PlayerType.LocalPlayer;
                mainBoardDotParent.GetChild(i).gameObject.name = "Real Player" + i;
                playerScoreDots[i].GetChild(0).GetComponent<TextMeshProUGUI>().text = players[i].playerScore.ToString();
                player.playerType = Enums.PlayerType.LocalPlayer;
            }
            else if(i< LocalGameController.playerCount + LocalGameController.botCount) 
            {
                playerScoreDots.Add(scoreDotParent.GetChild(i));
                playerScoreDots[i].name = "AI" + 1;
                playerUIDots.Add(mainBoardDotParent.GetChild(i));
                playerScoreDots[i].GetChild(0).GetComponent<TextMeshProUGUI>().text = "0";
                mainBoardDotParent.GetChild(i).gameObject.name = "AI"+i;
                mainBoardDotParent.GetChild(i).gameObject.GetComponent<Player>().playerType = Enums.PlayerType.AI;
                players.Add(new PlayerData((PlayerCount)i + 1));
                player.playerType = Enums.PlayerType.AI;
            }
            else
            {
                scoreDotParent.GetChild(i).gameObject.SetActive(false);
                mainBoardDotParent.GetChild(i).gameObject.SetActive(false);
            }
        }
        foreach(Transform dot in playerUIDots)
        {
            dot.GetChild(0).GetComponent<CanvasGroup>().alpha = 0;
        }
        //turn the first one on
        playerUIDots[0].GetChild(0).GetComponent<CanvasGroup>().alpha = 1;
        currentPlayer = 0;
        await Task.Yield();
        
    }

    public async void UpdateScore(int incomingPoints, bool isOver)
    {
        playerScoreDots[currentPlayer].GetChild(0).GetComponent<TextMeshProUGUI>().text = player.Score(incomingPoints).ToString();

        if (isOver)
        {
            Instantiate(gameOverManager,this.transform.parent).Init(players);
            timerManager.timerIsRunning = false;
            return;
        }
        await timerManager.StartTimer();
        boardIntrection.SetActive(false);
        if (mainBoardDotParent.GetChild(currentPlayer).GetComponent<Player>().playerType == Enums.PlayerType.AI)
        {
           StartCoroutine(TakeTurnAI());
        }

    }

    public void NextPlayer()
    {
       // CurrentPlayerTurn = Enums.CurrentPlayerTurn.LocalPlayer_Turn;
        mainBoardDotParent.GetChild(currentPlayer).GetChild(0).GetComponent<CanvasGroup>().DOFade(0, .75f);
        if (currentPlayer + 1 >= players.Count)
        {
            currentPlayer = 0;
        }
        else
        {
            currentPlayer++;
        }
        ChangePlayerIndicator();
        boardIntrection.SetActive(false);
        if (mainBoardDotParent.GetChild(currentPlayer).GetComponent<Player>().playerType == Enums.PlayerType.AI)
        {
            CurrentPlayerTurn = Enums.CurrentPlayerTurn.AI_Turn;
            StartCoroutine(TakeTurnAI());
        }
        else {
            CurrentPlayerTurn = Enums.CurrentPlayerTurn.LocalPlayer_Turn;
        }
    }
    IEnumerator TakeTurnAI()
    {
        boardIntrection.SetActive(true);
        yield return new WaitForSeconds(Random.Range(1.1f,2.5f));
        this.gameObject.GetComponent<AIHandler>().CalculateBestMove();
    }
    async void ChangePlayerIndicator()
    {
        mainBoardDotParent.GetChild(currentPlayer).GetChild(0).GetComponent<CanvasGroup>().DOFade(1,.75f);
        await timerManager.StartTimer();
    }


}

public enum PlayerCount
{
    Red = 1,
    Yellow,
    Blue,
    Green
}
public class PlayerData
{
    public readonly int playerNumber;
    public string playerName;
    public readonly Color32 playerColor;
    public readonly Color32 neighborOption;
    public Enums.PlayerType playerType;
    public int playerScore { get; private set; }


    public PlayerData(PlayerCount number)
    {
        this.playerNumber = (int)number;
        this.playerName = $"Player {playerNumber.ToString()}";
        this.playerScore = 0;
        GetColor(number, out playerColor,out neighborOption);
        playerType = Enums.PlayerType.LocalPlayer;

    }

    public int Score(int incomingPoint)
    {
        playerScore += incomingPoint;
        return playerScore;
    }


    private void GetColor(PlayerCount number, out Color32 playerColor, out Color32 neighborOption)
    {
        switch (number)
        {
            case PlayerCount.Green:
                playerColor = new Color32(56, 210,121, 255);
                neighborOption = new Color32(playerColor.r, playerColor.g, playerColor.b, 120);
                break;
            case PlayerCount.Blue:
                playerColor = new Color32(60, 168, 229, 255);
                neighborOption = new Color32(playerColor.r, playerColor.g, playerColor.b, 120);
                break;
            case PlayerCount.Yellow:
                playerColor = new Color32(255, 187, 52, 255);
                neighborOption = new Color32(playerColor.r, playerColor.g, playerColor.b, 120);
                break;
            case PlayerCount.Red:
                playerColor = new Color32(254, 61, 106, 255);
                neighborOption = new Color32(playerColor.r, playerColor.g, playerColor.b, 120);
                break;
            default:
                playerColor = new Color32(254, 61, 106, 255);
                neighborOption = new Color32(playerColor.r, playerColor.g, playerColor.b, 120);
                break;

        }
    }

}