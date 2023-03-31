using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Collections;
using System;

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
    public NetworkVariable<float> timeRemaining = new NetworkVariable<float>();
    public NetworkVariable<bool> timerIsRunning = new NetworkVariable<bool>();
    Color32 normalColor = new Color32(101, 138, 167, 255);
    private int defaultTime = 20;

    private void OnEnable()
    {
        timeRemaining.Value = defaultTime;
        DisplayTime(timeRemaining.Value - 1);
    }
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            timeRemaining.Value = defaultTime;
            return;
        }
        timeRemaining.OnValueChanged += TimerManager_UpdateTimeUI;
    }

    private void TimerManager_UpdateTimeUI(float previousValue, float newValue)
    {
        if (!IsServer)
        {
            _ = StartTimer();
        }
        DisplayTime(timeRemaining.Value);
    }

    public async Task GameStartDelay()
    {
        bool isMutiplayer = MultiplayerController.Instance.IsMutiplayer;
        var data = NetworkManager.Singleton;
        if (IsClient && isMutiplayer && !IsServer)
        {
            Debug.LogError("GameStartDelay Return");
            return;
        }
        timeTitle.color = Color.red;
        timeTitle.text = "Get Ready";
        timeRemaining.Value = defaultTime;
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
                PlayerHandler.Instance.UpdateScore(0, false);
                DestroyImmediate(screenBlocker);
                break;
        }
    }



    public async Task StartTimer()
    {      
        isOnce = false;
        UpdateTextColor();

        await Task.Delay(1000);
        if (IsServer || !MultiplayerController.Instance.IsMutiplayer)
        {
            timeRemaining.Value = defaultTime;
            timerIsRunning.Value = true;
        }      
    }

    private void UpdateTextColor()
    {
        timeText.color = normalColor;
        timeTitle.color = normalColor;
        timeTitle.text = "Time";
        timeText.text = "00:20";
    }

    void Update()
    {
        if (IsServer || !MultiplayerController.Instance.IsMutiplayer)
        {
            if (timerIsRunning.Value)
            {
                if (timeRemaining.Value > 0)
                {
                    timeRemaining.Value -= Time.deltaTime;
                    DisplayTime(timeRemaining.Value);

                }
                else
                {
                    Debug.Log("Time has run out!");
                    timeRemaining.Value = 0;
                    timerIsRunning.Value = false;
                }
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
        else
        {
            UpdateTextColor();
        }
        if (timeRemaining.Value <= 0.05f && !isOnce)
        {
            isOnce = true;
            if (IsServer)
            {
                PlayerHandler.Instance.NextTurnServerRpc();
            }
            else if (!MultiplayerController.Instance.IsMutiplayer)
            {
                PlayerHandler.Instance.aiHandler.GetRandomMove();
                PlayerHandler.Instance.stateManager.SwitchState(PlayerHandler.Instance.stateManager.ResetState);
            }
        }

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
