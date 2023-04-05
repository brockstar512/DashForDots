using System.Diagnostics;

public class Utility
{
    public static string GetErrorMessage(int code)
    {
        UnityEngine.Debug.LogError("Code " + code);
        switch (code)
        {
            case 15001:
            case 15009:
                return Constants.KMessageInvalidCode;
            default:
                return "";
        }
    }
}
