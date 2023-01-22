using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using Unity.VectorGraphics;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    const Scenes mainMenu = Scenes.MainMenu;
    const Scenes replay = Scenes.Game;
    [SerializeField] Button replayButton;
    [SerializeField] Button mainMenuButton;
    [SerializeField] Transform leaderboardParent;
    CanvasGroup cg;

    public void Init(List<PlayerData> players)
    {
        cg = GetComponent<CanvasGroup>();
        cg.alpha = 0;

        cg.DOFade(1,.5f).SetEase(Ease.InOutBack);

        replayButton.onClick.RemoveAllListeners();
        mainMenuButton.onClick.RemoveAllListeners();
        replayButton.onClick.AddListener(delegate { LoadingManager.Instance.LoadScene(replay.ToString()); });
        mainMenuButton.onClick.AddListener(delegate { LoadingManager.Instance.LoadScene(mainMenu.ToString()); });


        List<PlayerData> toReturn = players.OrderByDescending(player => player.playerScore).ToList();

        PopulateLeaderBoard(toReturn);
    }

    void PopulateLeaderBoard(List<PlayerData> players)
    {
        int playerCount = players.Count();

        for(int i = 0; i < leaderboardParent.childCount; i++)
        {
            if (playerCount <= 0)
            {
                leaderboardParent.GetChild(i).gameObject.SetActive(false);
                continue;
            }
            LeaderBoardEntry(leaderboardParent.GetChild(i), players[i]);
            playerCount--;
        }
    }

    void LeaderBoardEntry(Transform leaderboardSlot, PlayerData player)
    {
        Debug.Log($"Here is score {player.playerScore}");

        leaderboardSlot.GetChild(1).GetComponent<SVGImage>().color = player.playerColor;
        leaderboardSlot.GetChild(2).GetComponent<TextMeshProUGUI>().text = player.playerName;

    }
}
