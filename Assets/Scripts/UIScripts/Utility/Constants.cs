using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants
{
    // public static int AI_COUNT = 0;
    public static int GAME_TYPE = 0;

    //Difficulty Of AI

    public static readonly string EASY_GAME = "easy";
    public static readonly string NORMAL_GAME = "normal";
    public static readonly string HARD_GAME = "hard";
    public static readonly string VERY_HARD_GAME = "VeryHard";
    //Move values for AI
    public const int LOWEST_MOVE_VALUE = 100;
    public const int HIGHEST_MOVE_VALUE = 1000;

    //Messages
    public const string KMessageInvalidCode = "Join Code not valid";
    public const string KMessagePleaseWait = "Please wait..";
}
