using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class GameLines : MonoBehaviour
{
    [SerializeField] Image right;
    [SerializeField] Image up;
    [SerializeField] Image down;
    [SerializeField] Image left;
    const float speed = 1.25f;

    private void Awake()
    {
        right.fillAmount = 0;
        right.fillMethod = Image.FillMethod.Horizontal;
        right.fillOrigin = (int)Image.OriginHorizontal.Right;
        up.fillAmount = 0;
        up.fillMethod = Image.FillMethod.Vertical;
        up.fillOrigin = (int)Image.OriginVertical.Bottom;
        down.fillAmount = 0;
        down.fillMethod = Image.FillMethod.Vertical;
        down.fillOrigin = (int)Image.OriginVertical.Top;
        left.fillAmount = 0;
        left.fillMethod = Image.FillMethod.Horizontal;
        left.fillOrigin = (int)Image.OriginHorizontal.Right;


        //((Image.OriginVertical.Bottom)left.fillOrigin).ToString();
        //((Image.OriginHorizontal.Left)left.fillOrigin).ToString();

    }


    [ContextMenu("DrawLine")]
    public void DrawLine()
    {
        left.fillAmount = 0f;

        left.DOFillAmount(1,speed).SetEase(Ease.OutSine);
    }
}
