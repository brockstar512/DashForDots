using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Unity.Burst.CompilerServices;

public class ToastHandler : MonoBehaviour
{
    [SerializeField] private TMP_Text toastText;
    [SerializeField] private Transform toastTransform;
    [SerializeField] private GameObject tint;
    Vector3 currentPostion;
    Action onHide;
    public void ShowToastMessage(string message, bool autoHideEnable = true, FontColor fontColor = FontColor.WHITE, Action onHide = null)
    {
        toastText.text = message;
        this.onHide = onHide;
        tint.SetActive(false);
        currentPostion = toastTransform.position;
        switch (fontColor)
        {
            case FontColor.WHITE:
                toastText.color = Color.white;
                break;
            case FontColor.RED:
                tint.SetActive(true);
                toastText.color = Color.red;
                break;
            default:
                break;
        }
        toastTransform.DOMoveY(currentPostion.y * -0.5f, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            if (autoHideEnable)
            {
                toastTransform.DOMoveY(currentPostion.y, 0.5f).SetEase(Ease.InBack).SetDelay(2f).OnComplete(() =>
                {
                    onHide?.Invoke();
                    Destroy(this.gameObject);
                });
            }
        });

    }
    public void HideToast()
    {
        toastTransform.DOMoveY(currentPostion.y, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            tint.SetActive(false);
            onHide?.Invoke();
            Destroy(this.gameObject);
        });
    }

}
