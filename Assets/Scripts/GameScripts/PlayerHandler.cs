using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using DG.Tweening;
using DashForDots.AI;
using Unity.Netcode;
using System;
using System.Linq;

public class PlayerHandler : NetworkBehaviour
{
    public static PlayerHandler Instance { get; private set; }
    public List<PlayerData> players { get; private set; }
    public PlayerData player { get { return players[currentPlayer.Value]; } }
    public NetworkVariable<int> currentPlayer = new NetworkVariable<int>();
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
    private void OnEnable()
    {
        gridManager.OnSelectedDot += OnSelectedDot;
        gridManager.OnSelectedNeighbor += OnSelectedNeighbor;
        gridManager.OnSelectedCancel += OnSelectedCancel;
    }
    private void OnDisable()
    {
        gridManager.OnSelectedDot -= OnSelectedDot;
        gridManager.OnSelectedNeighbor -= OnSelectedNeighbor;
        gridManager.OnSelectedCancel -= OnSelectedCancel;
    }

    public async Task Init(PlayerCount playerCount)
    {
        bool isMutiplayer = MultiplayerController.Instance.IsMutiplayer;
        int playerIndex;
        players = new List<PlayerData>();
        playerScoreDots = new List<Transform>();
        playerUIDots = new List<Transform>();
        playerIndex = isMutiplayer ? MultiplayerController.Instance.GetPlayerDataIndexFromClientId(NetworkManager.Singleton.LocalClientId) : -1;
        for (int i = 0; i < maxPlayerCount; i++)
        {
            if (i < (int)playerCount - LocalGameController.botCount)
            {
                playerScoreDots.Add(scoreDotParent.GetChild(i));
                playerUIDots.Add(mainBoardDotParent.GetChild(i));
                if (!isMutiplayer)
                {
                    playerScoreDots[i].name = "Real player" + 1;
                    players.Add(new PlayerData((PlayerCount)i + 1));
                    mainBoardDotParent.GetChild(i).gameObject.GetComponent<Player>().playerType = Enums.PlayerType.LocalPlayer;
                    playerScoreDots[i].GetChild(0).GetComponent<TextMeshProUGUI>().text = players[i].playerScore.ToString();
                    mainBoardDotParent.GetChild(i).gameObject.name = "Real Player" + i;
                    player.playerType = Enums.PlayerType.LocalPlayer;
                }
                else
                {
                    int index = (playerIndex + i) % (int)playerCount;
                    MultiplayerData multiplayerData = MultiplayerController.Instance.GetPlayerDataFromPlayerIndex(index);
                    multiplayerData.colorId = i+1;
                    multiplayerData.currentIndex = index;
                    PlayerData playerData = new PlayerData(multiplayerData);
                    playerData.playerType = i == 0 ? Enums.PlayerType.LocalPlayer : Enums.PlayerType.OpponentPlayer;
                    playerScoreDots[i].name = playerData.playerName;
                    Player playerObject = mainBoardDotParent.GetChild(i).gameObject.GetComponent<Player>();
                    playerObject.playerType = playerData.playerType;
                    playerObject.gameObject.name = playerData.playerName;
                    playerObject.UpdatePlayerName(playerData);
                    players.Add(playerData);
                    playerScoreDots[i].GetChild(0).GetComponent<TextMeshProUGUI>().text = players[i].playerScore.ToString();
                }

            }
            else if (i < LocalGameController.playerCount + LocalGameController.botCount)
            {
                playerScoreDots.Add(scoreDotParent.GetChild(i));
                playerScoreDots[i].name = "AI" + 1;
                playerUIDots.Add(mainBoardDotParent.GetChild(i));
                playerScoreDots[i].GetChild(0).GetComponent<TextMeshProUGUI>().text = "0";
                mainBoardDotParent.GetChild(i).gameObject.name = "AI" + i;
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
        foreach (Transform dot in playerUIDots)
        {
            dot.GetChild(0).GetComponent<CanvasGroup>().alpha = 0;
        }
        //turn the first one on
        if (isMutiplayer)
        {
            playerUIDots[GetPlayerIndex(currentPlayer.Value)].GetChild(0).GetComponent<CanvasGroup>().alpha = 1;
        }
        else
        {
            playerUIDots[0].GetChild(0).GetComponent<CanvasGroup>().alpha = 1;
        }
        if (IsServer || !MultiplayerController.Instance.IsMutiplayer)
        {
            currentPlayer.Value = 0;
        }
        await Task.Yield();

    }
    public async void UpdateScore(int incomingPoints, bool isOver)
    {
        bool isMutiplayer = MultiplayerController.Instance.IsMutiplayer;
        if (isMutiplayer)
        {
            int currentIndex = players.FindIndex(t => t.playerNumber == incomingPoints);
            incomingPoints = currentIndex;
        }
        playerScoreDots[GetPlayerIndex(currentPlayer.Value)].GetChild(0).GetComponent<TextMeshProUGUI>().text = player.Score(incomingPoints).ToString();
        if (isOver)
        {
            Instantiate(gameOverManager, this.transform.parent).Init(players);
            if (IsServer || !MultiplayerController.Instance.IsMutiplayer)
            {
                timerManager.timerIsRunning.Value = false;
            }
            return;
        }
        await timerManager.StartTimer();
        CheckMyTurn();
        if (mainBoardDotParent.GetChild(GetPlayerIndex(currentPlayer.Value)).GetComponent<Player>().playerType == Enums.PlayerType.AI)
        {
            StartCoroutine(TakeTurnAI());
        }

    }

    private void CheckMyTurn()
    {
        if (MultiplayerController.Instance.IsMutiplayer)
        {
            MultiplayerData multiplayerData = MultiplayerController.Instance.GetPlayerDataFromPlayerIndex(currentPlayer.Value);
            boardIntrection.SetActive(!(multiplayerData.clientId == NetworkManager.LocalClientId));
            Debug.LogError($"CheckMyTurn {multiplayerData.clientId == NetworkManager.LocalClientId}");
        }
        else
        {
            boardIntrection.SetActive(false);
        }
    }

    public void NextPlayer()
    {
        if (!MultiplayerController.Instance.IsMutiplayer)
        {
            mainBoardDotParent.GetChild(GetPlayerIndex(currentPlayer.Value)).GetChild(0).GetComponent<CanvasGroup>().DOFade(0, .75f);
            IncrementCounter();
        }

        ChangePlayerIndicator();
        CheckMyTurn();
        Enums.PlayerType playerType = mainBoardDotParent.GetChild(GetPlayerIndex(currentPlayer.Value)).GetComponent<Player>().playerType;
        switch (playerType)
        {
            case Enums.PlayerType.AI:
                CurrentPlayerTurn = Enums.CurrentPlayerTurn.AI_Turn;
                StartCoroutine(TakeTurnAI());
                break;
            case Enums.PlayerType.LocalPlayer:
                CurrentPlayerTurn = Enums.CurrentPlayerTurn.LocalPlayer_Turn;
                break;
            case Enums.PlayerType.OpponentPlayer:
                CurrentPlayerTurn = Enums.CurrentPlayerTurn.OpponentPlayer_Turn;
                break;
        }

    }

    private void IncrementCounter()
    {
        if (currentPlayer.Value + 1 >= players.Count)
        {
            currentPlayer.Value = 0;
        }
        else
        {
            currentPlayer.Value++;
        }
    }
    IEnumerator TakeTurnAI()
    {
        boardIntrection.SetActive(true);
        yield return new WaitForSeconds(UnityEngine.Random.Range(1.1f, 2.5f));
        this.gameObject.GetComponent<AIHandler>().CalculateBestMove();
    }
    async void ChangePlayerIndicator()
    {
        mainBoardDotParent.GetChild(GetPlayerIndex(currentPlayer.Value)).GetChild(0).GetComponent<CanvasGroup>().DOFade(1, .75f);
        await timerManager.StartTimer();
    }

    public int GetPlayerIndex(int value)
    {
        if (MultiplayerController.Instance.IsMutiplayer)
        {
            MultiplayerData multiplayerData = MultiplayerController.Instance.GetPlayerDataFromPlayerIndex(value);
            int index = players.FindIndex(t => t.serverIndex == multiplayerData.serverIndex);
            Debug.LogError($"Get player index {index} isMultiplayer{true}");
            return index;
        }
        else
        {
            Debug.LogError($"Get player index {currentPlayer.Value} isMultiplayer{false}");
            return currentPlayer.Value;
        }
    }
    #region Multiplayer RPC & NetworkMethods
    public override void OnNetworkSpawn()
    {
        currentPlayer.OnValueChanged += OnCurrentPlayerValueChanged;
    }
    private void OnSelectedCancel()
    {
        OnCancelSelectedServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    private void OnCancelSelectedServerRpc()
    {
        OnCancelSelectedClientRpc();
    }
    [ClientRpc]
    private void OnCancelSelectedClientRpc()
    {
        if (!IsOwner)
        {
            _ = gridManager.OnCancel();
        }
    }

    private void OnSelectedDot(int x, int y)
    {
        OnDotSelectedServerRpc(x, y);
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnDotSelectedServerRpc(int x, int y)
    {
        OnDotSelectedClientRpc(x, y);
    }
    [ClientRpc]
    private void OnDotSelectedClientRpc(int x, int y)
    {
        if (!IsOwner)
        {
            stateManager.Inspect(gridManager.dots[x, y].transform);
            _ = gridManager.SelectDotLocal(x, y);
        }
    }
    private void OnSelectedNeighbor(int x, int y)
    {
        OnNeighborSelectedServerRpc(x, y);
    }
    [ServerRpc(RequireOwnership = false)]
    private void OnNeighborSelectedServerRpc(int x, int y)
    {
        OnNeighborSelectedClientRpc(x, y);
    }
    [ClientRpc]
    private void OnNeighborSelectedClientRpc(int x, int y)
    {
        if (!IsOwner)
        {
            gridManager.SelectNeighbor(x, y);
        }
    }


    [ServerRpc(RequireOwnership = false)]
    public void NextTurnServerRpc()
    {
        NextTurnClientRpc();
    }
    [ClientRpc]
    private void NextTurnClientRpc()
    {
        if (IsServer)
        {
            IncrementCounter();
        }
        if (!IsOwner)
        {
            _ = gridManager.OnConfirm();
            stateManager.SwitchState(stateManager.ResetState);
        }
    }
    private void OnCurrentPlayerValueChanged(int previous, int current)
    {
        mainBoardDotParent.GetChild(GetPlayerIndex(previous)).GetChild(0).GetComponent<CanvasGroup>().DOFade(0, .75f);
        NextPlayer();
    }
    [ServerRpc(RequireOwnership = false)]
    public void UpdateScoreServerRpc(int incomingPoints, bool isOver)
    {
        UpdateScoreClientRpc(incomingPoints, isOver);
    }
    [ClientRpc]
    private void UpdateScoreClientRpc(int incomingPoints, bool isOver)
    {      
        UpdateScore(incomingPoints, isOver);
    }
    #endregion



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
    public int currentIndex;
    public int serverIndex;
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
        GetColor(number, out playerColor, out neighborOption);
        playerType = Enums.PlayerType.LocalPlayer;

    }
    public PlayerData(MultiplayerData multiplayerData)
    {
        this.playerNumber = (int)multiplayerData.clientId;
        this.playerName = multiplayerData.playerName.ToString();
        this.playerScore = 0;
        this.currentIndex = multiplayerData.currentIndex;
        this.serverIndex = multiplayerData.serverIndex;
        GetColor((PlayerCount)multiplayerData.colorId, out playerColor, out neighborOption);
        playerType = (Enums.PlayerType)multiplayerData.playerType;
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
                playerColor = new Color32(56, 210, 121, 255);
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