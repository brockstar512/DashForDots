using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Collections;

public class TimerManager : NetworkBehaviour
{
    public enum GameStartSequence
    {
        Get = 1,
        Set,
        Go
    }
    [SerializeField] TextMeshProUGUI timeTitle;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] GameObject screenBlocker;
    public float timeRemaining = 20;
    public bool timerIsRunning = false;
    Color32 normalColor = new Color32(101, 138, 167, 255);

    public async Task GameStartDelay(bool isMutiplayer)
    {
        var data = NetworkManager.Singleton;
        if (IsClient && isMutiplayer)
        {
            Debug.LogError("GameStartDelay Return");
            return;
        }
        timeTitle.color = Color.red;
        timeTitle.text = "Get Ready";
        if (isMutiplayer)
        {
            GameStartDelayServerRpc(GameStartSequence.Get);
        }
        await Task.Delay(3000);
        timeTitle.text = "Go!";
        if (isMutiplayer)
        {
            GameStartDelayServerRpc(GameStartSequence.Set);
        }
        await Task.Delay(1000);
        timeTitle.color = normalColor;
        if (isMutiplayer)
        {
            GameStartDelayServerRpc(GameStartSequence.Go);
        }
        PlayerHandler.Instance.UpdateScore(0, false);
        await Task.Yield();
        DestroyImmediate(screenBlocker);
    }

    [ServerRpc]
    public void GameStartDelayServerRpc(GameStartSequence gameStartSequence)
    {
        GameStartDelayClientRpc(gameStartSequence);
    }

    [ClientRpc]
    public void GameStartDelayClientRpc(GameStartSequence gameStartSequence)
    {
        if (IsServer)
        {
            return;
        }
        switch (gameStartSequence)
        {
            case GameStartSequence.Get:
                timeTitle.color = Color.red;
                timeTitle.text = "Get Ready";
                break;
            case GameStartSequence.Set:
                timeTitle.text = "Go!";
                break;
            case GameStartSequence.Go:
                timeTitle.color = normalColor;
                DestroyImmediate(screenBlocker);
                break;
        }
    }



    public async Task StartTimer()
    {
        //timerIsRunning = false;
        isOnce = false;
        timeText.color = normalColor;
        timeTitle.color = normalColor;
        timeTitle.text = "Time";
        timeText.text = "00:20";


        timeRemaining = 20;
        await Task.Delay(1000);

        // Starts the timer automatically   
        timerIsRunning = true;
    }
    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);

            }
            else
            {
                Debug.Log("Time has run out!");
                timeRemaining = 0;
                timerIsRunning = false;
            }
        }
    }

    bool isOnce = false;
    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        if (minutes <= 0 && seconds <= 5)
        {
            timeTitle.text = "Hurry up!";
            timeTitle.color = Color.red;
            timeText.color = Color.red;

        }
        if (timeRemaining <= 0.05f && !isOnce)
        {
            isOnce = true;
            PlayerHandler.Instance.aiHandler.GetRandomMove();
            PlayerHandler.Instance.stateManager.SwitchState(PlayerHandler.Instance.stateManager.ResetState);
        }

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
