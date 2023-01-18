using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHandler : MonoBehaviour
{
    public static PlayerHandler Instance { get; private set; }
    public List<PlayerData> players { get; private set; }
    public PlayerData player { get { return players[currentPlayer]; } }
    public int currentPlayer{ get; private set; }
    List<Transform> playerScoreDots;
    List<Transform> playerUIDots;
    [SerializeField] Transform scoreDotParent;
    [SerializeField] Transform mainBoardDotParent;
    const int maxPlayerCount = 4;

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

    public void Init(PlayerCount playerCount)
    {
        players = new List<PlayerData>();
        playerScoreDots = new List<Transform>();
        playerUIDots = new List<Transform>();

        for(int i = 0;i < maxPlayerCount; i++)
        {
            if(i < (int)playerCount)
            {
                playerScoreDots.Add(scoreDotParent.GetChild(i));
                playerUIDots.Add(mainBoardDotParent.GetChild(i));
                players.Add(new PlayerData((PlayerCount)i + 1));
                playerScoreDots[i].GetChild(0).GetComponent<TextMeshProUGUI>().text = players[i].playerScore.ToString();

            }
            else
            {
                scoreDotParent.GetChild(i).gameObject.SetActive(false);
                mainBoardDotParent.GetChild(i).gameObject.SetActive(false);
            }
        }

        currentPlayer = 0;
    }

    public void NextPlayer()
    {
        if (currentPlayer + 1 >= players.Count)
        {
            currentPlayer = 0;
        }
        else
        {
            currentPlayer++;
        }
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
    public readonly string playerName;
    public readonly Color32 playerColor;
    public readonly Color32 neighborOption;
    public int playerScore { get; private set; }


    public PlayerData(PlayerCount number)
    {
        this.playerNumber = (int)number;
        this.playerName = $"Player {playerNumber.ToString()}";
        this.playerScore = 0;
        GetColor(number, out playerColor,out neighborOption);
    }

    public void Score(int incomingPoint)
    {
        playerScore += incomingPoint;
    }


    private void GetColor(PlayerCount number, out Color32 playerColor, out Color32 neighborOption)
    {
        switch (number)
        {
            case PlayerCount.Green:
                playerColor = new Color32(56, 210,121, 255);
                neighborOption = new Color32(56, 210, 121, 60);
                break;
            case PlayerCount.Blue:
                playerColor = new Color32(60, 168, 229, 255);
                neighborOption = new Color32(60, 168, 229, 60);
                break;
            case PlayerCount.Yellow:
                playerColor = new Color32(255, 87, 52, 255);
                neighborOption = new Color32(255, 87, 52, 60);
                break;
            case PlayerCount.Red:
                playerColor = new Color32(254, 61, 106, 255);
                neighborOption = new Color32(254, 61, 106, 60);
                break;
            default:
                playerColor = new Color32(254, 61, 106, 255);
                neighborOption = new Color32(254, 61, 106, 60);
                break;


        }
    }

}