using DG.Tweening;
using System;
using System.Diagnostics;
using static Enums;

public class Utility
{
    public static bool IsAITakeTurn = false;
    public static bool IsClickDisable = false;
    public static string GetErrorMessage(int code)
    {
        switch (code)
        {
            case 15001:
            case 15009:
            case 16001:
            case 16010:
                return Constants.KMessageInvalidCode;
            default:
                return Constants.KMessageSomethingwentworng;
        }
    }
    public static void CheckIsTweening(Object obj)
    {
        if (DOTween.IsTweening(obj))
        {
            DOTween.Complete(obj, true);
        }
    }

    public static string GetAIMode(AIMode mode)
    {
        switch (mode)
        {
            case AIMode.EASY:
            default:
                return Constants.EASY_GAME;
            case AIMode.MEDIUM:              
            case AIMode.HARD:
            case AIMode.SUPER_HARD:
                return Constants.NORMAL_GAME;
        }
    }
}
