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
    NetworkVariable<bool> isOnce = new NetworkVariable<bool>() { Value = false };
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
            DisplayTime(newValue);
        }
    }

    public async Task GameStartDelay()
    {
        bool isMutiplayer = MultiplayerController.Instance.IsMultiplayer;
        if (IsClient && isMutiplayer && !IsServer)
        {
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
        DestoryScreenBlocker();
    }

    public void DestoryScreenBlocker()
    {
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
        UpdateTextColor();
        await Task.Delay(1000);
        if (IsServer || MultiplayerController.Instance != null && !MultiplayerController.Instance.IsMultiplayer)
        {
            timeRemaining.Value = defaultTime;
            timerIsRunning.Value = true;
            isOnce.Value = false;
        }
    }

    private void UpdateTextColor()
    {
        timeText.color = normalColor;
        timeTitle.color = normalColor;
        timeTitle.text = "Time";
        timeText.text = $"00:{defaultTime}";
    }

    void Update()
    {
        if (IsServer || MultiplayerController.Instance != null && !MultiplayerController.Instance.IsMultiplayer)
        {
            if (timerIsRunning.Value)
            {
                if (timeRemaining.Value > 0)
                {
                    timeRemaining.Value -= Time.deltaTime;
                }
                else
                {
                    Debug.Log("Time has run out!");
                    timeRemaining.Value = 0;
                    timerIsRunning.Value = false;
                }
                DisplayTime(timeRemaining.Value);
            }
        }
    }
    void DisplayTime(float timeToDisplay)
    {
        if (!isOnce.Value && timerIsRunning.Value)
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
            if (timeRemaining.Value <= 0.05f)
            {
                if (IsServer || MultiplayerController.Instance != null && !MultiplayerController.Instance.IsMultiplayer)
                {
                    isOnce.Value = true;
                }
                if (MultiplayerController.Instance.IsMultiplayer)
                {
                    if (IsServer)
                    {
                        PlayerHandler.Instance.OnCancelSelectedDotAndAITurnClientRpc();
                    }
                }
                else
                {
                    if (!PlayerHandler.Instance.gridManager.isConfirmClicked)
                    {
                        PlayerHandler.Instance.BoardIntraction(false);
                        if (PlayerHandler.Instance.stateManager.IConfirmState())
                        {
                            PlayerHandler.Instance.gridManager.Confirm();
                            PlayerHandler.Instance.stateManager.SwitchState(PlayerHandler.Instance.stateManager.ResetState);
                        }
                        else
                        {
                            PlayerHandler.Instance.StopPlayerDotBlink();
                            PlayerHandler.Instance.stateManager.SwitchState(PlayerHandler.Instance.stateManager.ResetState);
                            GetRandomMove();
                        }
                    }
                    else
                    {
                        Debug.Log("Is Confirm Already Clicked");
                    }
                }
                UpdateTextTime(0, 0);
            }
            else
            {
                UpdateTextTime(minutes, seconds);
            }
        }
        else
        {
            UpdateTextTime(0, 0);
        }
    }
    public override void OnNetworkDespawn()
    {
        timeRemaining = new NetworkVariable<float>();
        timerIsRunning = new NetworkVariable<bool>();
        base.OnNetworkDespawn();
    }
    private void GetRandomMove()
    {
        StartCoroutine(PlayerHandler.Instance.TakeRandomTurnAI());
    }

    public void UpdateTextTime(float minutes, float seconds)
    {
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
