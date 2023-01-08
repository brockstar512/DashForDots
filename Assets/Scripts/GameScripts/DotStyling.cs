using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VectorGraphics;

public class DotStyling : MonoBehaviour
{
    public Color32 playerColor;
    [SerializeField] Image right;
    [SerializeField] Image up;
    [SerializeField] Image down;
    [SerializeField] Image left;
    SVGImage image;
    const float speed = 1.25f;

    public void Init(Dictionary<Vector2, bool> connectingCompass)
    {
        if (!connectingCompass[Vector2.right])
        {
            right.fillAmount = 0;
            right.fillMethod = Image.FillMethod.Horizontal;
            right.fillOrigin = (int)Image.OriginHorizontal.Left;
        }
        else
        {
            right = null;
        }
        if (!connectingCompass[Vector2.up])
        {
            up.fillAmount = 0;
            up.fillMethod = Image.FillMethod.Vertical;
            up.fillOrigin = (int)Image.OriginVertical.Bottom;
        }
        else
        {
            up = null;
        }
        if (!connectingCompass[Vector2.down])
        {
            down.fillAmount = 0;
            down.fillMethod = Image.FillMethod.Vertical;
            down.fillOrigin = (int)Image.OriginVertical.Top;
        }
        else
        {
            down = null;
        }
        if (!connectingCompass[Vector2.left])
        {
            left.fillAmount = 0;
            left.fillMethod = Image.FillMethod.Horizontal;
            left.fillOrigin = (int)Image.OriginHorizontal.Right;
        }
        else
        {
            left = null;
        }
        //Close();
        image = GetComponent<SVGImage>();

    }
    [ContextMenu("Open")]
    public void Open()
    {
        this.transform.DOScale(1, 1).SetEase(Ease.InBounce);

    }
    [ContextMenu("Close")]
    public void Close()
    {
        this.transform.DOScale(0, 1).SetEase(Ease.InBounce);

    }

    [ContextMenu("DrawLine")]
    public void DrawLine()
    {
        left?.DOFillAmount(1,speed).SetEase(Ease.OutSine);
        right?.DOFillAmount(1, speed).SetEase(Ease.OutSine);
        up?.DOFillAmount(1, speed).SetEase(Ease.OutSine);
        down?.DOFillAmount(1, speed).SetEase(Ease.OutSine);

    }

    public void OnSelect()
    {
        this.transform.DOScale(.5f, 1).SetEase(Ease.InBounce);
        image.DOColor(this.playerColor, .15f);

    }
    public void Deselect()
    {

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