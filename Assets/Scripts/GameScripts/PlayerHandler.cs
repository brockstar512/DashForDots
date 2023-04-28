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
using Unity.VectorGraphics;
using Unity.Collections;

public class PlayerHandler : NetworkBehaviour
{
    private static PlayerHandler instance;
    public static PlayerHandler Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerHandler>();
            }
            return instance;
        }
    }
    public List<PlayerData> players { get; private set; }
    //public PlayerData player { get { return players[GetPlayerIndex(currentPlayer.Value)]; } }
    public PlayerData player { get; private set; } = new PlayerData(PlayerCount.Red);
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
    public bool isSycningGame;
    public int lastSyncIndex;
    public bool isGameSynced;
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
            instance = this;
        }
    }
    private void OnEnable()
    {
        gridManager.OnSelectedDot += OnSelectedDot;
        gridManager.OnSelectedNeighbor += OnSelectedNeighbor;
        gridManager.OnSelectedCancel += OnSelectedCancel;
        gridManager.OnSelectedConfirm += OnSelectedConfirm;
        gridManager.OnSelectedReset += OnSelectedReset;
        MultiplayerController.Instance.OnHostShutDown += Multiplayer_OnHostShutDown;
    }

    private void OnDisable()
    {
        gridManager.OnSelectedDot -= OnSelectedDot;
        gridManager.OnSelectedNeighbor -= OnSelectedNeighbor;
        gridManager.OnSelectedCancel -= OnSelectedCancel;
        gridManager.OnSelectedConfirm -= OnSelectedConfirm;
        gridManager.OnSelectedReset -= OnSelectedReset;
        if (MultiplayerController.Instance != null)
        {
            MultiplayerController.Instance.OnHostShutDown -= Multiplayer_OnHostShutDown;
        }
    }

    public async Task Init(PlayerCount playerCount, bool isMultiplayer)
    {
        players = new List<PlayerData>();
        playerScoreDots = new List<Transform>();
        playerUIDots = new List<Transform>();
        int playerIndex = isMultiplayer ? MultiplayerController.Instance.GetPlayerDataIndexFromClientId(NetworkManager.Singleton.LocalClientId) : -1;
        for (int i = 0; i < maxPlayerCount; i++)
        {
            if (i < (int)playerCount - LocalGameController.botCount)
            {
                playerScoreDots.Add(scoreDotParent.GetChild(i));
                playerUIDots.Add(mainBoardDotParent.GetChild(i));
                if (!isMultiplayer)
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
                    SetupPlayerForMultiplayer(playerCount, playerIndex, i);
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
        if (IsServer || !MultiplayerController.Instance.IsMultiplayer)
        {
            currentPlayer.Value = 0;
        }
        playerUIDots[GetPlayerIndex(currentPlayer.Value)].GetChild(0).GetComponent<CanvasGroup>().alpha = 1;
        await Task.Yield();

    }

    private void SetupPlayerForMultiplayer(PlayerCount playerCount, int playerIndex, int i)
    {
        int index = (playerIndex + i) % (int)playerCount;
        MultiplayerData multiplayerData = MultiplayerController.Instance.GetPlayerDataFromPlayerIndex(index);
        multiplayerData.colorId = index + 1;
        multiplayerData.currentIndex = index;
        PlayerData playerData = new PlayerData(multiplayerData);
        playerData.playerType = i == 0 ? Enums.PlayerType.LocalPlayer : Enums.PlayerType.OpponentPlayer;
        playerScoreDots[i].name = playerData.playerName;
        Player playerObject = mainBoardDotParent.GetChild(i).gameObject.GetComponent<Player>();
        playerObject.GetComponent<SVGImage>().color = playerData.playerColor;
        playerObject.playerType = playerData.playerType;
        playerObject.gameObject.name = playerData.playerName;
        playerObject.UpdatePlayerName(playerData);
        players.Add(playerData);
        playerScoreDots[i].GetChild(0).GetComponent<TextMeshProUGUI>().text = players[i].playerScore.ToString();
        playerScoreDots[i].GetComponent<SVGImage>().color = playerData.playerColor;
    }

    public async void UpdateScore(int incomingPoints, bool isOver)
    {
        bool isMutiplayer = MultiplayerController.Instance.IsMultiplayer;
        UpdateScore(incomingPoints, currentPlayer.Value, isOver);
        if (isOver)
        {
            if (IsServer || !isMutiplayer)
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

    public void UpdateScore(int incomingPoints, int index, bool isGameOver)
    {
        if (playerScoreDots.Count > 0)
        {          
            playerScoreDots[GetPlayerIndex(index)].GetChild(0).GetComponent<TextMeshProUGUI>().text = player.Score(incomingPoints).ToString();
        }
        CheckGameOver(isGameOver);
    }   
    public void CheckGameOver(bool isGameOver)
    {
        if (isGameOver)
        {
            Instantiate(gameOverManager, this.transform.parent).Init(players);
        }
    }

    private async void CheckMyTurn()
    {
        if (MultiplayerController.Instance.IsMultiplayer)
        {
            boardIntrection.SetActive(!MultiplayerController.Instance.IsMyTurn(currentPlayer.Value));
            MultiplayerData multiplayerData = MultiplayerController.Instance.GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
            if (multiplayerData.isRejoin && IsMyTurn() && !isGameSynced && !isSycningGame)
            {
                Time.timeScale = 30;
                BoardIntraction(false);
                var playerTurn = MultiplayerController.Instance.playerTurnList;
                for (int i = lastSyncIndex; i < playerTurn.Count; i++)
                {
                    await gridManager.UpdateUIForRejoinPlayerAsync(playerTurn[i]);
                }
                isGameSynced = true;
                SetPlayerDataSync(currentPlayer.Value, true);
                BoardIntraction(true);
                Time.timeScale = 1;
            }
        }
        else
        {
            boardIntrection.SetActive(false);
        }
        if (players != null && players.Count > 0)
        {
            stateManager.SetCurrentPlayerTurn(players[GetPlayerIndex(currentPlayer.Value)]);
        }

    }
    public void NextPlayer()
    {
        if (!MultiplayerController.Instance.IsMultiplayer)
        {
            mainBoardDotParent.GetChild(GetPlayerIndex(currentPlayer.Value)).GetChild(0).GetComponent<CanvasGroup>().DOFade(0, .75f);
            IncrementCounter();
        }
        SetPlayerDataSync(currentPlayer.Value, false);
        ChangePlayerIndicator();
        CheckMyTurn();
        SetPlayerTurn();
        if (IsServer)
        {
            MultiplayerData multiplayerData = MultiplayerController.Instance.GetPlayerDataFromPlayerIndex(currentPlayer.Value);
            if (multiplayerData.status == (int)Enums.PlayerState.Inactive)
            {
                StartCoroutine(TakeTurnAI());
            }
        }
    }

    private void SetPlayerTurn()
    {
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
        BoardIntraction(false);
        yield return new WaitForSeconds(UnityEngine.Random.Range(1.1f, 2.5f));
        this.gameObject.GetComponent<AIHandler>().CalculateBestMove();
    }
    public void BoardIntraction(bool flag)
    {
        boardIntrection.SetActive(!flag);
    }
    public void TakeRandomTurnAI()
    {
        aiHandler.GetRandomMove();
    }
    async void ChangePlayerIndicator()
    {
        mainBoardDotParent.GetChild(GetPlayerIndex(currentPlayer.Value)).GetChild(0).GetComponent<CanvasGroup>().DOFade(1, .75f);
        await timerManager.StartTimer();
    }
    public void SetPlayerDataSync(int index, bool enableForceFully)
    {
        if (!isSycningGame || enableForceFully)
        {
            player = players[GetPlayerIndex(index)];
            Debug.Log($"Current Color {(PlayerCount)player.colorId}");
        }
        else
        {
            Debug.LogError("Else" + index);
        }
    }

    public int GetPlayerIndex(int value)
    {
        if (MultiplayerController.Instance.IsMultiplayer && players != null)
        {
            MultiplayerData multiplayerData = MultiplayerController.Instance.GetPlayerDataFromPlayerIndex(value);
            int index = players.FindIndex(t => t.serverIndex == multiplayerData.serverIndex);
            return index;
        }
        else
        {
            return currentPlayer.Value;
        }
    }
    public bool IsMyTurn()
    {
        return MultiplayerController.Instance.IsMyTurn(currentPlayer.Value);
    }

    #region Multiplayer RPC & NetworkMethods

    #region Override Netcode Methods
    public override void OnNetworkSpawn()
    {
        currentPlayer.OnValueChanged += OnCurrentPlayerValueChanged;
        MultiplayerController.Instance.rejoinPlayerConnected.OnValueChanged += OnRejoinPlayerValueChangedAsync;
    }
    #endregion Override Netcode Methods

    #region Reset
    private void OnSelectedReset()
    {
        OnResetSelectedServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    private void OnResetSelectedServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        OnResetSelectedClientRpc(clientId);
    }
    [ClientRpc]
    private void OnResetSelectedClientRpc(ulong senderId)
    {
        if (!senderId.Equals(NetworkManager.LocalClientId))
        {
            stateManager.SwitchState(stateManager.ResetState);
        }
    }
    #endregion Reset

    #region Confirm
    private void OnSelectedConfirm()
    {
        OnConfirmSelectedServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    private void OnConfirmSelectedServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        OnConfirmSelectedClientRpc(clientId);
    }
    [ClientRpc]
    private void OnConfirmSelectedClientRpc(ulong senderId)
    {
        //if (!senderId.Equals(NetworkManager.LocalClientId))
        //{
        _ = gridManager.OnConfirm(true, senderId);
        stateManager.SwitchState(stateManager.ResetState);
        //}
    }
    #endregion Confirm

    #region Cancel
    private void OnSelectedCancel()
    {
        OnCancelSelectedServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    private void OnCancelSelectedServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        OnCancelSelectedClientRpc(clientId);
    }
    [ClientRpc]
    private void OnCancelSelectedClientRpc(ulong senderId)
    {
        if (!senderId.Equals(NetworkManager.LocalClientId))
        {
            _ = gridManager.OnCancel();
        }
    }
    [ClientRpc]
    public void OnCancelSelectedDotAndAITurnClientRpc()
    {
        MultiplayerData multiplayerData = MultiplayerController.Instance.GetPlayerDataFromPlayerIndex(currentPlayer.Value);
        if (IsMyTurn() || multiplayerData.status == (int)Enums.PlayerState.Inactive)
        {
            CancelDotForAITurn();
        }
    }
    private void CancelDotForAITurn()
    {
        BoardIntraction(false);
        TakeRandomTurnAI();
    }

    #endregion Cancel

    #region SelectDot
    private void OnSelectedDot(int x, int y)
    {
        OnDotSelectedServerRpc(x, y);
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnDotSelectedServerRpc(int x, int y, ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        OnDotSelectedClientRpc(x, y, clientId);
    }
    [ClientRpc]
    private void OnDotSelectedClientRpc(int x, int y, ulong senderId)
    {
        if (!senderId.Equals(NetworkManager.LocalClientId))
        {
            stateManager.Inspect(gridManager.dots[x, y].transform);
            _ = gridManager.SelectDotLocal(x, y);
            gridManager.dots[x, y].DotStyling.Select();
        }
    }
    #endregion SelectDot

    #region Neighbor Dot
    private void OnSelectedNeighbor(int x, int y)
    {
        OnNeighborSelectedServerRpc(x, y);
    }
    [ServerRpc(RequireOwnership = false)]
    private void OnNeighborSelectedServerRpc(int x, int y, ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        OnNeighborSelectedClientRpc(x, y, clientId);
    }
    [ClientRpc]
    private void OnNeighborSelectedClientRpc(int x, int y, ulong senderId)
    {
        if (!senderId.Equals(NetworkManager.LocalClientId))
        {
            _ = gridManager.SelectedNeighbor(x, y);
        }
    }
    #endregion Neighbor

    #region Next Turn RPC
    [ServerRpc(RequireOwnership = false)]
    public void NextTurnServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        NextTurnClientRpc(clientId);
    }
    [ClientRpc]
    private void NextTurnClientRpc(ulong senderId)
    {
        if (IsServer)
        {
            IncrementCounter();
        }
    }
    private void OnCurrentPlayerValueChanged(int previous, int current)
    {
        mainBoardDotParent.GetChild(GetPlayerIndex(previous)).GetChild(0).GetComponent<CanvasGroup>().DOFade(0, .75f);
        NextPlayer();
    }
    #endregion Next Turn RPC

    #region Update Score
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
    private void Multiplayer_OnHostShutDown(object sender, EventArgs e)
    {
        stateManager.SwitchState(stateManager.HostQuitState);
    }
    #endregion  Update Score

    #region Rejoin & update player data
    private void OnRejoinPlayerValueChangedAsync(ulong previousValue, ulong newValue)
    {
        if (newValue != NetworkManager.LocalClientId)
        {
            MultiplayerData multiplayerData = MultiplayerController.Instance.GetPlayerDataFromClientId(newValue);
            foreach (var player in players)
            {
                if (player.playerId == multiplayerData.playerId)
                {
                    player.playerNumber = (int)multiplayerData.clientId;
                    break;
                }
            }
        }
        else
        {
            CheckMyTurn();
        }
    }
    #endregion

    #endregion Multiplayer RPC & NetworkMethods

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
    public int playerNumber;
    public int currentIndex;
    public int serverIndex;
    public string playerName;
    public int colorId;
    public readonly Color32 playerColor;
    public readonly Color32 neighborOption;
    public Enums.PlayerType playerType;
    public FixedString64Bytes playerId;
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
        this.playerId = multiplayerData.playerId;
        this.colorId = multiplayerData.colorId;
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