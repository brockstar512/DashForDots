using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using static Enums;
using TMPro;
using Unity.Netcode;
using System;
using JetBrains.Annotations;
using System.Linq;
using static OnlineSubMenu;
using System.Threading.Tasks;
using static GameLobby;

public class OnlineSubMenu : MonoBehaviour
{

    [SerializeField] Button createGame;
    [SerializeField] Button shareCode;
    [SerializeField] Button joinGame;
    [SerializeField] Button play;
    [SerializeField] Button back;
    private Stack<CanvasGroup> subStack;
    private CanvasGroup currentPage;
    [SerializeField] CanvasGroup joinGamePanel;
    [SerializeField] CanvasGroup shareCodePanel;
    [SerializeField] CanvasGroup creatGamePanel;
    [SerializeField] CanvasGroup landingPage;
    [SerializeField] TextMeshProUGUI playerCountText;
    [SerializeField] Button IncreasePlayers;
    [SerializeField] Button DecreasePlayers;
    [SerializeField] TMP_InputField joinCodeInputField;
    [SerializeField] WaitingViewRefrences waitingViewRefrences;
    [SerializeField] Scenes targetScene;
    private int playerCount;

    private void Awake()
    {
        subStack = new Stack<CanvasGroup>();
        back.onClick.RemoveAllListeners();
        back.onClick.AddListener(NavigationManager.Instance.Back);

        //InitializePage();
        currentPage = landingPage;
    }
    private void Start()
    {
        playerCount = MultiplayerController.Instance.PlayerCount.Value;
        WrapPlayer(0);
        LoadingAnimation(true);
    }
    void OnEnable()
    {
        createGame.onClick.AddListener(delegate { OpenPage(creatGamePanel); });
        joinGame.onClick.AddListener(delegate { OpenPage(joinGamePanel); });
        shareCode.onClick.AddListener(delegate { StartHost(); });
        play.onClick.AddListener(delegate { StartClient(); });
        waitingViewRefrences.playGame.onClick.AddListener(delegate { StartGame(); });
        IncreasePlayers.onClick.RemoveAllListeners();
        DecreasePlayers.onClick.RemoveAllListeners();
        IncreasePlayers.onClick.AddListener(delegate { WrapPlayer(1); });
        DecreasePlayers.onClick.AddListener(delegate { WrapPlayer(-1); });
        MultiplayerController.Instance.OnPlayerConnected += Multiplayer_OnPlayerConnected;
        MultiplayerController.Instance.OnPlayerDataNetworkListChanged += Multiplayer_OnPlayerDataNetworkListChanged;
        MultiplayerController.Instance.OnHostShutDown += Multiplayer_OnHostShutDown;
        GameLobby.Instance.OnGameJoinFailed += GameLobby_OnGameJoinFailed;
    }   

    private void OnDisable()
    {
        IncreasePlayers.onClick.RemoveAllListeners();
        DecreasePlayers.onClick.RemoveAllListeners();
        createGame.onClick.RemoveAllListeners();
        joinGame.onClick.RemoveAllListeners();
        shareCode.onClick.RemoveAllListeners();
        waitingViewRefrences.playGame.onClick.RemoveAllListeners();
        MultiplayerController.Instance.OnPlayerConnected -= Multiplayer_OnPlayerConnected;
        MultiplayerController.Instance.OnPlayerDataNetworkListChanged -= Multiplayer_OnPlayerDataNetworkListChanged;
        MultiplayerController.Instance.OnHostShutDown -= Multiplayer_OnHostShutDown;
        GameLobby.Instance.OnGameJoinFailed -= GameLobby_OnGameJoinFailed;
    }

    private void Multiplayer_OnPlayerDataNetworkListChanged(object sender, EventArgs e)
    {
        foreach (var item in waitingViewRefrences.playerList)
        {
            item.SetActive(false);
        }
        for (int i = 0; i < MultiplayerController.Instance.GetPlayerList().Count; i++)
        {
            waitingViewRefrences.playerList[i].SetActive(true);
        }
        if (NetworkManager.Singleton.IsHost)
        {
            waitingViewRefrences.playGame.gameObject.SetActive(MultiplayerController.Instance.CanHostStartTheGame());
        }
        LoadingAnimation(!MultiplayerController.Instance.CanHostStartTheGame());
        _ = ForceUpdateCanvases();
    }
    async Task ForceUpdateCanvases()
    {
        await Task.Delay(50);
        LayoutRebuilder.ForceRebuildLayoutImmediate(waitingViewRefrences.waitingPanel.GetComponent<RectTransform>());
        Canvas.ForceUpdateCanvases();
        await Task.Yield();
    }
    private void Multiplayer_OnHostShutDown(object sender, EventArgs e)
    {
        Back();
    }

    void Back()
    {
        CanvasGroup previousPage;
        //Debug.Log($"splash image    {subStack.Count}");

        if (subStack.Count == 1)
        {
            //Debug.Log("splash image");
            //assign original back
            back.onClick.RemoveAllListeners();
            back.onClick.AddListener(NavigationManager.Instance.Back);
            //go to main screen
            previousPage = landingPage;
            subStack.Clear();
        }
        else
        {
            previousPage = this.subStack.Pop();
        }

        previousPage.gameObject.SetActive(true);
        currentPage.DOFade(0, .1f).OnComplete(() =>
        {
            previousPage.DOFade(1, .2f).OnComplete(() =>
            {
                currentPage.gameObject.SetActive(false);
                currentPage = previousPage;
            });
            Reset();
        });


    }
    void WrapPlayer(int increaseOrDecrease)
    {
        if (playerCount + increaseOrDecrease > 4)
        {
            playerCount = 2;
        }
        else if (playerCount + increaseOrDecrease < 2)
        {
            playerCount = 4;
        }
        else
        {
            playerCount += increaseOrDecrease;
        }
        MultiplayerController.Instance.SetPlayerCount(playerCount);
        playerCountText.text = playerCount.ToString() + " PLAYERS";
    }
    private void StartHost()
    {
        if (playerCount >= 2)
        {
            GameLobby.Instance.HostGame();
        }
    }
    private void StartClient()
    {
        string code = joinCodeInputField.text;
        if (playerCount >= 2 && !string.IsNullOrEmpty(code))
        {
            GameLobby.Instance.JoinGame(code);
        }
    }

    private void StartGame()
    {
        MultiplayerController.Instance.StartGame();
    }
    void OpenPage(CanvasGroup screen)
    {
        back.onClick.RemoveAllListeners();
        back.onClick.AddListener(Back);

        screen.gameObject.SetActive(true);
        currentPage.DOFade(0, .1f).OnComplete(() =>
        {
            screen.DOFade(1, .2f).OnComplete(() =>
            {
                subStack.Push(currentPage);
                currentPage.gameObject.SetActive(false);
                currentPage = screen;
            });
        });
    }

    private void Multiplayer_OnPlayerConnected(object sender, MultiplayerController.OnPlayerConnectedEventArgs e)
    {
        if (e.isClientJoined && e.clientId == NetworkManager.Singleton.LocalClientId)
        {
            OpenPage(waitingViewRefrences.waitingPanel);
            waitingViewRefrences.gameCodeText.text = string.Format("Game Code :{0}", GameLobby.Instance.GetGameCode());
        }
        else if (!e.isClientJoined && e.clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log($"e.isClientJoined {e.isClientJoined} e.clientId {e.clientId}LocalClientId {NetworkManager.Singleton.LocalClientId}");
        }
    }
    private void Reset()
    {
        waitingViewRefrences.gameCodeText.text = string.Empty;
        MultiplayerController.Instance.ShutDown();
    }
    private void LoadingAnimation(bool flag)
    {
        waitingViewRefrences.loadingView.gameObject.SetActive(flag);
        waitingViewRefrences.waitingForUser.gameObject.SetActive(flag);
    }
    private void GameLobby_OnGameJoinFailed(object sender, OnGameJoinFailedEventArgs e)
    {
        ToastMessage.Show(e.message.ToUpperInvariant());
    }

    [System.Serializable]
    public struct WaitingViewRefrences
    {
        public CanvasGroup waitingPanel;
        public TMP_Text gameCodeText;
        public Button playGame;
        public List<GameObject> playerList;
        public DotLoadingAnimation loadingView;
        public GameObject waitingForUser;
    }

}
