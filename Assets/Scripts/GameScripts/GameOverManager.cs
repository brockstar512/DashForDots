using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
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

        //var orderByResult = from player in players
        //                    orderby player.playerScore
        //                    select player;




        List<PlayerData> toReturn = players.OrderByDescending(player => player.playerScore).ToList();

        //PopulateLeaderBoard(players);
    }

    void PopulateLeaderBoard(List<PlayerData> players)
    {

    }
}
