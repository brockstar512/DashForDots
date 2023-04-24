using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    const Scenes mainMenu = Scenes.MainMenu;
    const Scenes replay = Scenes.Game;
    [SerializeField] Button replayButton;
    [SerializeField] Button mainMenuButton;
    [SerializeField] Transform leaderboardParent;
    CanvasGroup cg;

    [ContextMenu("test tie")]
    public void TieTest()
    {
        List<PlayerData> players = new List<PlayerData>
        {
            new PlayerData(PlayerCount.Red),
            new PlayerData(PlayerCount.Yellow),

        };
        Init(players);
    }

    public void Init(List<PlayerData> players)
    {
        cg = GetComponent<CanvasGroup>();
        cg.alpha = 0;
        cg.DOFade(1, .5f).SetEase(Ease.InOutBack);
        replayButton.gameObject.SetActive(!MultiplayerController.Instance.IsMultiplayer);
        replayButton.onClick.RemoveAllListeners();
        mainMenuButton.onClick.RemoveAllListeners();
        replayButton.onClick.AddListener(delegate { LoadingManager.Instance.LoadScene(replay.ToString()); });
        mainMenuButton.onClick.AddListener(delegate
        {
            if (MultiplayerController.Instance.IsMultiplayer)
            {
                MultiplayerController.Instance.ShutDown();
                Destroy(NetworkManager.Singleton.gameObject);
                Destroy(MultiplayerController.Instance.gameObject);
            }
            LoadingManager.Instance.LoadScene(mainMenu.ToString());

        });
        List<PlayerData> toReturn = players.OrderByDescending(player => player.playerScore).ToList();

        PopulateLeaderBoard(toReturn);
    }

    void PopulateLeaderBoard(List<PlayerData> players)
    {
        int playerCount = players.Count();
        int rank = 1;
        for (int i = 0; i < leaderboardParent.childCount; i++)
        {
            if (playerCount <= 0)
            {
                leaderboardParent.GetChild(i).gameObject.SetActive(false);
                continue;
            }

            if (i > 0 && players[i].playerScore == players[i - 1].playerScore)
            {
                rank--;
            }

            leaderboardParent.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = (rank).ToString();
            leaderboardParent.GetChild(i).GetChild(1).GetComponent<SVGImage>().color = players[i].playerColor;
            leaderboardParent.GetChild(i).GetChild(2).GetComponent<TextMeshProUGUI>().text = players[i].playerName;
            rank++;
            playerCount--;
        }
    }


}
