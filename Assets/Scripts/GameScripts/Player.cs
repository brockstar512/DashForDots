using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    public Enums.PlayerType playerType;
    [SerializeField] TextMeshProUGUI playerNameText;
    private string playerName = string.Empty;
    private void Start()
    {
        UpdatePlayerData();
    }
    public void UpdatePlayerData()
    {
        if (string.IsNullOrEmpty(playerName))
        {
            playerNameText.text = playerType.ToString();
        }
        else
        {
            playerNameText.text = playerName;
        }
    }
    public void UpdatePlayerName(string name)
    {
        playerName = name;
    }
}
