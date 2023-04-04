using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ToastHandler : MonoBehaviour
{
    [SerializeField] private TMP_Text toastText;
    [SerializeField] private Transform toastTransform;
    public void ShowToastMessage(string message,Action onHide)
    {
        toastText.text = message;
        Vector3 currentPostion = toastTransform.position;
        toastTransform.DOMoveY(currentPostion.y * -0.5f, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            toastTransform.DOMoveY(currentPostion.y, 0.5f).SetEase(Ease.InBack).SetDelay(2f).OnComplete(() =>
            {
                onHide?.Invoke();
                Destroy(this);
            });
        });

    }

}
