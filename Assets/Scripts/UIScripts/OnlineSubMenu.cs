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

        InitializePage();
        currentPage = landingPage;
        playerCount = MultiplayerController.Instance.PlayerCount;
    }

    void InitializePage()
    {
        createGame.onClick.AddListener(delegate { OpenPage(creatGamePanel); });
        joinGame.onClick.AddListener(delegate { OpenPage(joinGamePanel); });
        shareCode.onClick.AddListener(delegate { StartHost(); });
        play.onClick.AddListener(delegate { StartClient(); });
        IncreasePlayers.onClick.RemoveAllListeners();
        DecreasePlayers.onClick.RemoveAllListeners();
        IncreasePlayers.onClick.AddListener(delegate { WrapPlayer(1); });
        DecreasePlayers.onClick.AddListener(delegate { WrapPlayer(-1); });
        MultiplayerController.Instance.OnPlayerConnected += Multiplayer_OnPlayerConnected;
        MultiplayerController.Instance.OnPlayerDataNetworkListChanged += Multiplayer_OnPlayerDataNetworkListChanged;
    }

    private void OnDisable()
    {
        IncreasePlayers.onClick.RemoveAllListeners();
        DecreasePlayers.onClick.RemoveAllListeners();
        createGame.onClick.RemoveAllListeners();
        joinGame.onClick.RemoveAllListeners();
        shareCode.onClick.RemoveAllListeners();
        MultiplayerController.Instance.OnPlayerConnected -= Multiplayer_OnPlayerConnected;
        MultiplayerController.Instance.OnPlayerDataNetworkListChanged -= Multiplayer_OnPlayerDataNetworkListChanged;
    }

    private void Multiplayer_OnPlayerDataNetworkListChanged(object sender, EventArgs e)
    {
        foreach (var item in waitingViewRefrences.playerList)
        {
            item.SetActive(false);
        }
        foreach (var item in MultiplayerController.Instance.GetPlayerList())
        {
            waitingViewRefrences.playerList[(int)item.clientId].SetActive(true);
        }
        Canvas.ForceUpdateCanvases();
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
        else if(!e.isClientJoined && e.clientId == NetworkManager.Singleton.LocalClientId)
        {
            Back();
        }
    }
    private void Reset()
    {       
        waitingViewRefrences.gameCodeText.text = string.Empty;
        MultiplayerController.Instance.ShutDown();
    }

    [System.Serializable]
    public struct WaitingViewRefrences
    {
        public CanvasGroup waitingPanel;
        public TMP_Text gameCodeText;
        public List<GameObject> playerList;
        public List<GameObject> loadingView;
    }

}
