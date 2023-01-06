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
        right.fillOrigin = (int)Image.OriginHorizontal.Left;
        up.fillAmount = 0;
        up.fillMethod = Image.FillMethod.Vertical;
        up.fillOrigin = (int)Image.OriginVertical.Bottom;
        down.fillAmount = 0;
        down.fillMethod = Image.FillMethod.Vertical;
        down.fillOrigin = (int)Image.OriginVertical.Top;
        left.fillAmount = 0;
        left.fillMethod = Image.FillMethod.Horizontal;
        left.fillOrigin = (int)Image.OriginHorizontal.Right;
    }


    [ContextMenu("DrawLine")]
    public void DrawLine()
    {
        left.fillAmount = 0f;

        left.DOFillAmount(1,speed).SetEase(Ease.OutSine);
    }
}


/*
 * old line drawing
 * 
 * 
 *     public RectTransform dot1;
    public RectTransform dot2;
    public Transform worldCanvas;
 *     [ContextMenu("Add line")]
    public void AddLine()
    {
        CreateDotConnection(dot1.anchoredPosition, dot2.anchoredPosition);
    }

    private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
    {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(worldCanvas, true);
        gameObject.GetComponent<Image>().color = this.playerColor;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchorMin = new Vector2(.5f, .5f);
        rectTransform.anchorMax = new Vector2(.5f, .5f);
        rectTransform.sizeDelta = new Vector2(distance, .25f);
        rectTransform.anchoredPosition = dotPositionA + dir * distance * .5f;
        rectTransform.localPosition = new Vector3(.75f, .75f, 0) * dir;
        rectTransform.localEulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(dir));
    }

    public float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }
 * */