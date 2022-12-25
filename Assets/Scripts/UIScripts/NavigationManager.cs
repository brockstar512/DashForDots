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
    [SerializeField] CanvasGroup HowToPlayScreen;
    [SerializeField] CanvasGroup OnlinePlayScreen;
    [SerializeField] CanvasGroup LocalPlayScreen;
    [SerializeField] CanvasGroup LandingPage;
    private Stack<CanvasGroup> stack;
    CanvasGroup currentPage;
    //where to we put difficulty


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

    public void CachePage(CanvasGroup previousScreen, CanvasGroup newScreen)
    {
        //stack.Push(previousScreen);
        newScreen.gameObject.SetActive(true);

        //do I need to change the alpha at all?
        previousScreen.DOFade(0, .1f).OnComplete(() =>
        {
            newScreen.DOFade(1, .2f).OnComplete(() =>
            {
                stack.Push(previousScreen);
                previousScreen.gameObject.SetActive(false);
                currentPage = newScreen;
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
