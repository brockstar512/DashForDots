using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VectorGraphics;

public class DotStyling : MonoBehaviour
{
    [SerializeField] Image right;
    [SerializeField] Image up;
    [SerializeField] Image down;
    [SerializeField] Image left;
    SVGImage image;
    SVGImage cap;


    const float speed = .75f;

    public void Init(Dictionary<Vector2Int, bool> connectingCompass)
    {
        if (!connectingCompass[Vector2Int.right])
        {
            right.fillAmount = 0;
            right.fillMethod = Image.FillMethod.Horizontal;
            right.fillOrigin = (int)Image.OriginHorizontal.Left;
        }
        else
        {
            right = null;
        }
        if (!connectingCompass[Vector2Int.up])
        {
            up.fillAmount = 0;
            up.fillMethod = Image.FillMethod.Vertical;
            up.fillOrigin = (int)Image.OriginVertical.Bottom;
        }
        else
        {
            up = null;
        }
        if (!connectingCompass[Vector2Int.down])
        {
            down.fillAmount = 0;
            down.fillMethod = Image.FillMethod.Vertical;
            down.fillOrigin = (int)Image.OriginVertical.Top;
        }
        else
        {
            down = null;
        }
        if (!connectingCompass[Vector2Int.left])
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
        cap = this.transform.parent.GetChild(this.transform.parent.childCount-1).GetComponent<SVGImage>();
        Close();

    }

    [ContextMenu("Open")]
    public void Open()
    {
        this.transform.DOScale(1, 1).SetEase(Ease.InBounce);

    }
    [ContextMenu("Close")]
    public void Close()
    {
        this.transform.DOScale(0, 0).SetEase(Ease.InBounce);
        //cap.transform.DOScale(0, 0).SetEase(Ease.InBounce);

    }

    [ContextMenu("DrawLine")]
    public void DrawLine()
    {
        left?.DOFillAmount(1,speed).SetEase(Ease.OutSine);
        right?.DOFillAmount(1, speed).SetEase(Ease.OutSine);
        up?.DOFillAmount(1, speed).SetEase(Ease.OutSine);
        down?.DOFillAmount(1, speed).SetEase(Ease.OutSine);

    }


    public void DrawLine(Vector2Int direction)
    {
        
        switch (direction)
        {
            case Vector2Int v when v.Equals(Vector2Int.up):
                if (up?.fillAmount == 1)
                    break;
                up?.DOColor(PlayerHandler.Instance.player.playerColor,0);
                up?.DOFillAmount(1, speed).SetEase(Ease.OutSine);
                //Debug.Log("Up");
                break;
            case Vector2Int v when v.Equals(Vector2Int.down):
                if (down?.fillAmount == 1)
                    break;
                down?.DOColor(PlayerHandler.Instance.player.playerColor, 0);
                down?.DOFillAmount(1, speed).SetEase(Ease.OutSine);
                //Debug.Log("Down");
                break;
            case Vector2Int v when v.Equals(Vector2Int.right):
                if (right?.fillAmount == 1)
                    break;
                right?.DOColor(PlayerHandler.Instance.player.playerColor, 0);
                right?.DOFillAmount(1, speed).SetEase(Ease.OutSine);
                //Debug.Log("Right");
                break;
            case Vector2Int v when v.Equals(Vector2Int.left):
                if (left?.fillAmount == 1)
                    break;
                left?.DOColor(PlayerHandler.Instance.player.playerColor, 0);
                left?.DOFillAmount(1, speed).SetEase(Ease.OutSine);
                //Debug.Log("Left");
                break;

        }
        PairingSelected();

    }

    public void EraseLine(Vector2Int direction)
    {
        //Debug.Log("You cannot click this at this time grid: "+ direction);
        switch (direction)
        {
            case Vector2Int v when v.Equals(Vector2Int.up):
                up?.DOColor(PlayerHandler.Instance.player.playerColor, 0);
                up?.DOFillAmount(0, speed  - .5f).SetEase(Ease.OutSine);
                //Debug.Log("Up");
                break;
            case Vector2Int v when v.Equals(Vector2Int.down):
                down?.DOColor(PlayerHandler.Instance.player.playerColor, 0);
                down?.DOFillAmount(0, speed - .5f).SetEase(Ease.OutSine);
                //Debug.Log("Down");
                break;
            case Vector2Int v when v.Equals(Vector2Int.right):
                right?.DOColor(PlayerHandler.Instance.player.playerColor, 0);
                right?.DOFillAmount(0, speed - .5f).SetEase(Ease.OutSine);
                //Debug.Log("Right");
                break;
            case Vector2Int v when v.Equals(Vector2Int.left):
                left?.DOColor(PlayerHandler.Instance.player.playerColor, 0);
                left?.DOFillAmount(0, speed - .5f).SetEase(Ease.OutSine);
                //Debug.Log("Left");
                break;

        }

    }

    public void Select()
    {
        this.transform.DOScale(.5f, .25f).SetEase(Ease.OutSine);
        image.DOColor(PlayerHandler.Instance.player.playerColor, .15f);

    }
    public void Deselect()
    {
        this.transform.DOScale(0, .25f).SetEase(Ease.OutSine);
        //image.DOColor(this.playerColor, .15f);
    }

    public void NeighborHighlight()
    {
        //think about how not to rescale the neighbor thts already selected
        //if (this.transform.lossyScale == Vector3.one)
        //    return;
        image.DOColor(PlayerHandler.Instance.player.neighborOption, 0);
        this.transform.DOScale(1f, .25f).SetEase(Ease.OutSine);

    }
    public void NeighborUnHighlight()
    {
        this.transform.DOScale(0, .25f).SetEase(Ease.OutSine);

    }
    public void PairingSelected()
    {
        image.DOColor(PlayerHandler.Instance.player.playerColor, .25f);
        this.transform.DOScale(1, .25f).SetEase(Ease.OutSine);
    }
    public void Confirm()
    {
        //cap.transform.DOScale(1, .25f).SetEase(Ease.OutSine);
        this.transform.DOScale(0, .25f).SetEase(Ease.OutSine);
        cap.DOColor(PlayerHandler.Instance.player.playerColor, .15f);
    }
    public void ResizeLines(Vector2 dotPositionA, Vector2 dotPositionB)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchorMin = new Vector2(.5f, .5f);
        rectTransform.anchorMax = new Vector2(.5f, .5f);
        rectTransform.sizeDelta = new Vector2(distance, .25f);
        rectTransform.anchoredPosition = dotPositionA + dir * distance * .5f;
        rectTransform.localPosition = new Vector3(.75f, .75f, 0) * dir;
        rectTransform.localEulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(dir));
    }
    float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
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