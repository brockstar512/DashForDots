using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class NavigationManager : MonoBehaviour
{
    public static NavigationManager Instance { get; private set; }
    [SerializeField] Button LocalPlay;
    [SerializeField] Button OnlinePlay;
    [SerializeField] Button HowToPlay;
    [SerializeField] Button JoinOnlineGame;
    [SerializeField] Button CreateOnlineGame;
    [SerializeField] CanvasGroup HowToPlayScreen;
    [SerializeField] CanvasGroup OnlinePlayScreen;
    [SerializeField] CanvasGroup LocalPlayScreen;
    [SerializeField] CanvasGroup LandingPage;
    [SerializeField] CanvasGroup JoinOnlineGameScreen;
    [SerializeField] CanvasGroup CreateOnlineGameScreen;
    private Stack<CanvasGroup> stack;
    CanvasGroup currentPage;


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

        InitializePage();
        currentPage = LandingPage;
        stack = new Stack<CanvasGroup>();
    }
    void InitializePage()
    {
        LocalPlay.onClick.AddListener(delegate { OpenPage(LocalPlayScreen); });
        HowToPlay.onClick.AddListener(delegate { OpenPage(HowToPlayScreen); });
        OnlinePlay.onClick.AddListener(delegate { OpenPage(OnlinePlayScreen); });
        JoinOnlineGame.onClick.AddListener(delegate { OpenSubMenu(JoinOnlineGameScreen); });
        CreateOnlineGame.onClick.AddListener(delegate { OpenSubMenu(CreateOnlineGameScreen); });


    }

    void OpenPage(CanvasGroup screen)
    {
        screen.gameObject.SetActive(true);

            currentPage.DOFade(0, .1f).OnComplete(() =>
            {
                screen.DOFade(1, .2f).OnComplete(() =>
                {
                    stack.Push(currentPage);
                    currentPage.gameObject.SetActive(false);
                    currentPage = screen;
                });
            });

    }

    void OpenSubMenu(CanvasGroup screen)
    {
        screen.gameObject.SetActive(true);
        currentPage.DOFade(0, .1f).OnComplete(() => {
            screen.DOFade(1, .2f).OnComplete(() =>
            {
                stack.Push(currentPage);
                currentPage.gameObject.SetActive(false);
                currentPage = screen;
            });
        });

    }

    public void Back()
    {
        CanvasGroup previousPage = this.stack.Pop();
        previousPage.gameObject.SetActive(true);
        currentPage.DOFade(0, .1f).OnComplete(() => {
            previousPage.DOFade(1, .2f).OnComplete(() =>
            {
                currentPage.gameObject.SetActive(false);
                currentPage = previousPage;
            });
        });
    }

}