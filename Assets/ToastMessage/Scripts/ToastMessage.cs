using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToastMessage : MonoBehaviour
{
    private static GameObject toastGameObject;
    public static void Show(string message)
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
            toastHandler.ShowToastMessage(message, OnHide);
        }

    }
    private static void OnHide()
    {
        toastGameObject = null;
    }
}
