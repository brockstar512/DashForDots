using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums
{
    public enum GameType
    {
        None,
        PlayerVsPlayer,
        PlayerVsAI,
        Multiplayer

    }
    public enum PlayerType 
    {
        AI, 
        LocalPlayer
    }
    public enum CurrentPlayerTurn
    {
        AI_Turn,
        LocalPlayer_Turn
    }

    public enum GameDifficulty 
    {
        Easy,
        Normal,
        Hard
    }

}
