using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class OnlineSubMenu : MonoBehaviour
{

    [SerializeField] Button createGame;
    [SerializeField] Button shareCode;
    [SerializeField] Button joinGame;
    [SerializeField] Button back;
    private Stack<CanvasGroup> subStack;
    private CanvasGroup currentPage;
    [SerializeField] CanvasGroup joinGamePanel;
    [SerializeField] CanvasGroup shareCodePanel;
    [SerializeField] CanvasGroup waitingPanel;
    [SerializeField] CanvasGroup creatGamePanel;
    [SerializeField] CanvasGroup landingPage;


    private void Awake()
    {
        subStack = new Stack<CanvasGroup>();
        back.onClick.RemoveAllListeners();
        back.onClick.AddListener(NavigationManager.Instance.Back);
        InitializePage();
        currentPage = landingPage;


    }

    void InitializePage()
    {
        createGame.onClick.AddListener(delegate { OpenPage(creatGamePanel); });
        joinGame.onClick.AddListener(delegate { OpenPage(joinGamePanel); });
        shareCode.onClick.AddListener(delegate { OpenPage(shareCodePanel); });

    }

    void Back()
    {
        CanvasGroup previousPage;
        //Debug.Log($"splash image    {subStack.Count}");

        if (subStack.Count == 1)
        {
            Debug.Log("splash image");
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
            currentPage.DOFade(0, .1f).OnComplete(() => {
                previousPage.DOFade(1, .2f).OnComplete(() =>
                {
                    currentPage.gameObject.SetActive(false);
                    currentPage = previousPage;
                });
            });


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

}
