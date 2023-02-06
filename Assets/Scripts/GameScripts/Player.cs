using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    public Enums.PlayerType playerType;
    [SerializeField] TextMeshProUGUI playerNameText;
    private void Start()
    {
        UpdatePlayerData();
    }
    public void UpdatePlayerData()
    {
        playerNameText.text = playerType.ToString();
    }
}
