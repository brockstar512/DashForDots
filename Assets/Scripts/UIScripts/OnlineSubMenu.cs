using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static OnlineSubMenu;

public class OnlineSubMenu : MonoBehaviour
{

    [SerializeField] Button createGame;
    [SerializeField] Button shareCode;
    [SerializeField] Button joinGame;
    [SerializeField] Button quickGame;
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
    [SerializeField] QuickGameViewRefrences quickGameViewRefrences;
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
        playerCount = 2;
        WrapPlayer(0);
        LoadingAnimation(true);
    }
    void OnEnable()
    {
        createGame.onClick.AddListener(delegate { OpenPage(creatGamePanel); });
        joinGame.onClick.AddListener(delegate { OpenPage(joinGamePanel); });
        quickGame.onClick.AddListener(delegate { OpenPage(quickGameViewRefrences.quickGamePanel); });
        shareCode.onClick.AddListener(delegate { StartHost(); });
        play.onClick.AddListener(delegate { StartClient(); });
        waitingViewRefrences.playGame.onClick.AddListener(delegate { StartGame(); });
        waitingViewRefrences.shareBtn.onClick.AddListener(delegate { ShareCode(); });
        IncreasePlayers.onClick.RemoveAllListeners();
        DecreasePlayers.onClick.RemoveAllListeners();
        IncreasePlayers.onClick.AddListener(delegate { WrapPlayer(1); });
        DecreasePlayers.onClick.AddListener(delegate { WrapPlayer(-1); });
        quickGameViewRefrences.increasePlayers.onClick.AddListener(delegate { WrapPlayer(1); });
        quickGameViewRefrences.decreasePlayers.onClick.AddListener(delegate { WrapPlayer(-1); });
        quickGameViewRefrences.quickPlay.onClick.AddListener(delegate { QuickMatch(); });
        MultiplayerController.Instance.OnPlayerConnected += Multiplayer_OnPlayerConnected;
        MultiplayerController.Instance.OnPlayerDataNetworkListChanged += Multiplayer_OnPlayerDataNetworkListChanged;
        MultiplayerController.Instance.OnHostShutDown += Multiplayer_OnHostShutDown;
        GameLobby.Instance.OnGameJoinStarted += GameLobby_OnGameJoinStarted;
        GameLobby.Instance.OnGameCreateJoinFailed += GameLobby_OnGameJoinFailed;
        GameLobby.Instance.OnQuickGameFailed += GameLobby_OnQuickGameFailed;
    }

    private void OnDisable()
    {
        IncreasePlayers.onClick.RemoveAllListeners();
        DecreasePlayers.onClick.RemoveAllListeners();
        quickGameViewRefrences.increasePlayers.onClick.RemoveAllListeners();
        quickGameViewRefrences.decreasePlayers.onClick.RemoveAllListeners();
        createGame.onClick.RemoveAllListeners();
        joinGame.onClick.RemoveAllListeners();
        quickGame.onClick.RemoveAllListeners();
        shareCode.onClick.RemoveAllListeners();
        quickGameViewRefrences.quickPlay.onClick.RemoveAllListeners();
        waitingViewRefrences.playGame.onClick.RemoveAllListeners();
        MultiplayerController.Instance.OnPlayerConnected -= Multiplayer_OnPlayerConnected;
        MultiplayerController.Instance.OnPlayerDataNetworkListChanged -= Multiplayer_OnPlayerDataNetworkListChanged;
        MultiplayerController.Instance.OnHostShutDown -= Multiplayer_OnHostShutDown;
        GameLobby.Instance.OnGameJoinStarted -= GameLobby_OnGameJoinStarted;
        GameLobby.Instance.OnGameCreateJoinFailed -= GameLobby_OnGameJoinFailed;
        GameLobby.Instance.OnQuickGameFailed -= GameLobby_OnQuickGameFailed;

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
        quickGameViewRefrences.playerCountText.text = playerCountText.text = playerCount.ToString() + " PLAYERS";
    }
    private void StartHost()
    {
        if (playerCount >= 2)
        {
            // GameLobby.Instance.HostGame(playerCount);
            GameLobby.Instance.CreateLobby(DateTime.Now.TimeOfDay.ToString(), true, playerCount);
        }
    }
    private void StartClient()
    {
        string code = joinCodeInputField.text;
        if (playerCount >= 2 && !string.IsNullOrEmpty(code))
        {
            //GameLobby.Instance.JoinGame(code);
            GameLobby.Instance.JoinWithCode(code);

        }
        else
        {
            ToastMessage.Show(Constants.KMessageEnterValidCode);
        }
    }

    private void StartGame()
    {
        MultiplayerController.Instance.StartGame();
    }

    public void QuickMatch()
    {
        if (playerCount >= 2)
        {
            // GameLobby.Instance.HostGame(playerCount);
            GameLobby.Instance.QuickJoin(playerCount);
        }
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
            shareCode.gameObject.SetActive(!e.isQuickMatch);
            if (e.isQuickMatch)
            {
                waitingViewRefrences.gameCodeText.text = string.Format("Remaining Time :{0}", 30);
            }
            else
            {
                waitingViewRefrences.gameCodeText.text = string.Format("Game Code :{0}", GameLobby.Instance.GetGameCode());
            }
        }
        else if (!e.isClientJoined)
        {
            Debug.Log($"e.isClientJoined {e.isClientJoined} e.clientId {e.clientId}LocalClientId {NetworkManager.Singleton.LocalClientId}");
            UpdateButtonStatus(true);
        }
    }

    private void GameLobby_OnQuickGameFailed(object sender, GameLobby.OnGameJoinFailedEventArgs e)
    {
        if (e.errorCode == 16006)
        {
            GameLobby.Instance.CreateLobby(DateTime.Now.TimeOfDay.ToString(), false, playerCount);
        }
    }

    private void Reset()
    {
        waitingViewRefrences.gameCodeText.text = string.Empty;
        joinCodeInputField.text = string.Empty;
        shareCode.interactable = true;
        play.interactable = true;
        MultiplayerController.Instance.ResetPlayerCount();
        MultiplayerController.Instance.ShutDown();
    }
    private void LoadingAnimation(bool flag)
    {
        waitingViewRefrences.loadingView.gameObject.SetActive(flag);
        waitingViewRefrences.waitingForUser.gameObject.SetActive(flag);
    }
    private void GameLobby_OnGameJoinStarted(object sender, EventArgs e)
    {
        UpdateButtonStatus(false);
    }

    private void UpdateButtonStatus(bool flag)
    {
        shareCode.interactable = flag;
        play.interactable = flag;
    }

    private void GameLobby_OnGameJoinFailed(object sender, GameLobby.OnGameJoinFailedEventArgs e)
    {
        UpdateButtonStatus(true);
        ToastMessage.Show(e.message);
    }
    private void OnTransportFailure()
    {
        UpdateButtonStatus(true);
    }
    void ShareCode()
    {
        string email = "";
        string subject = MyEscapeURL("Game Code");
        string body = MyEscapeURL($"Please find game code here : {GameLobby.Instance.GetGameCode()} \nPlease Download the game \n http://google.com ");
        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
    }
    string MyEscapeURL(string url)
    {
        return UnityWebRequest.EscapeURL(url).Replace("+", "%20");
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
        public Button shareBtn;
    }
    [System.Serializable]
    public struct QuickGameViewRefrences
    {
        public CanvasGroup quickGamePanel;
        public Button increasePlayers;
        public Button decreasePlayers;
        public TextMeshProUGUI playerCountText;
        public Button quickPlay;
    }

}
