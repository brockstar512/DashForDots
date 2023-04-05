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
        LocalPlayer,
        OpponentPlayer
    }
    public enum CurrentPlayerTurn
    {
        AI_Turn,
        LocalPlayer_Turn,
        OpponentPlayer_Turn
    }
    public enum PlayerCount
    {
        TowPlayer,
        ThreePlayer,
        FourPlayer
    }

    public enum PlayerState
    {
        Active = 1,
        Inactive = 2,
    }   
}
