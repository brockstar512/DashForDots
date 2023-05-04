using System;
using UnityEngine;
public enum FontColor
{
    WHITE,
    RED,
}
public class ToastMessage : MonoBehaviour
{
    private static GameObject toastGameObject;
    public static void Show(string message, bool autoHideEnable = true, FontColor fontColor = FontColor.WHITE)
    {
        if (toastGameObject == null)
        {
            var prefab = Resources.Load<GameObject>("Prefab/ToastMessage");

            if (prefab == null)
            {
                throw new UnityException(String.Format("Could not find prefab '{ ToastMessage }'!"));
            }
            toastGameObject = Instantiate(prefab);
            ToastHandler toastHandler = toastGameObject.GetComponent<ToastHandler>();
            toastHandler.ShowToastMessage(message, autoHideEnable, fontColor, OnHide);
        }

    }
    public static void Hide()
    {
        if (toastGameObject != null)
        {
            ToastHandler toastHandler = toastGameObject.GetComponent<ToastHandler>();
            toastHandler.HideToast();
        }
    }
    private static void OnHide()
    {
        toastGameObject = null;
    }
}
