using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BotFactory : MonoBehaviour
{
    [SerializeField] Button back;
    [SerializeField] Button easyDifficulty;
    [SerializeField] Button normalDifficulty;
    [SerializeField] Button hardDifficulty;
    [SerializeField] CanvasGroup difficultyPage;
    [SerializeField] Scenes targetScene;

    public static string difficulty { get; private set; }
    private void Awake()
    {
        back.onClick.RemoveAllListeners();
        easyDifficulty.onClick.RemoveAllListeners();
        normalDifficulty.onClick.RemoveAllListeners();
        hardDifficulty.onClick.RemoveAllListeners();
        easyDifficulty.onClick.AddListener(delegate { LoadGame(Constants.EASY_GAME); });
        normalDifficulty.onClick.AddListener(delegate { LoadGame(Constants.NORMAL_GAME); });
        hardDifficulty.onClick.AddListener(delegate { LoadGame(Constants.HARD_GAME); });
        back.onClick.AddListener(NavigationManager.Instance.Back);
        difficulty = Constants.EASY_GAME;
    }
    public void LoadGame(string difficultyName) 
    {
        difficulty = difficultyName;
        LoadingManager.Instance.LoadScene(targetScene.ToString());
    }
}
