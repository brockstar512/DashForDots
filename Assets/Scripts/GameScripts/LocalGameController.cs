using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocalGameController : MonoBehaviour
{
    [SerializeField] Button back;
    [SerializeField] Button IncreasePlayers;
    [SerializeField] Button DecreasePlayers;
    [SerializeField] Button DecreaseBots;
    [SerializeField] Button IncreaseBots;
    [SerializeField] Toggle includeBots;
    [SerializeField] Button next;
    [SerializeField] TextMeshProUGUI playerCountText;
    [SerializeField] TextMeshProUGUI botsCountText;
    [SerializeField] GameObject botsSection;
    [SerializeField] CanvasGroup difficultyPage;
    CanvasGroup cg;


    public int playerCount { get; private set; }
    public int botCount { get; private set; }




    private void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        back.onClick.RemoveAllListeners();
        back.onClick.AddListener(NavigationManager.Instance.Back);
        IncreasePlayers.onClick.RemoveAllListeners();
        DecreasePlayers.onClick.RemoveAllListeners();
        IncreasePlayers.onClick.AddListener(delegate { WrapPlayer(1); });
        DecreasePlayers.onClick.AddListener(delegate { WrapPlayer(-1); });
        IncreaseBots.onClick.AddListener(delegate { WrapBot(1); });
        DecreaseBots.onClick.AddListener(delegate { WrapBot(-1); });
        includeBots.onValueChanged.AddListener(delegate { botsSection.SetActive(includeBots.isOn); });
        next.onClick.RemoveAllListeners();
        next.onClick.AddListener(Next);

        playerCount = 2;
        botCount = 0;
        WrapPlayer(0);
        WrapBot(0);
        botsSection.SetActive(includeBots.isOn);
    }

    void WrapPlayer(int increaseOrDecrease)
    {
        if (playerCount + increaseOrDecrease > 4)
        {
            playerCount = 1;
        }
        else if (playerCount + increaseOrDecrease < 1)
        {
            playerCount = 4;
        }
        else
        {
            playerCount += increaseOrDecrease;
        }
        WrapBot(0);
        playerCountText.text = playerCount.ToString() + " PLAYERS";
    }

    void WrapBot(int increaseOrDecrease)
    {
        int botLimit = 4 - playerCount;
        if (botCount + increaseOrDecrease > botLimit)
        {
            botCount = 0;
        }
        else if (botCount + increaseOrDecrease < 0)
        {
            botCount = botLimit;
        }
        else
        {
            botCount += increaseOrDecrease;
        }
        botsCountText.text = botCount.ToString() + " BOTS";
    }

    [ContextMenu("Next page test")]
    public void Next()
    {
        Debug.Log($"Caching page {includeBots.isOn} and {botCount}");
        if (includeBots.isOn && botCount > 0)
        {
            Debug.Log("Caching page");
            NavigationManager.Instance.CachePage(cg, difficultyPage);
            //open difficulty
            return;
        }
        //else
        //start game
    }
}
