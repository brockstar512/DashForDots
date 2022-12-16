using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HowToPlayController : MonoBehaviour
{
    [SerializeField] Transform paginationUI;
    [SerializeField] Transform pageParent;
    [SerializeField] Button pageController;
    CanvasGroup currentCG;
    int index = 0;
    [SerializeField] LineRenderer lr;
    [SerializeField]LineTest lt;

    private void Awake()
    {
        pageController.onClick.RemoveAllListeners();
        pageController.onClick.AddListener(Next);
        
    }
    private void OnEnable()
    {
        Debug.Log("Enable");
        index = 0;
        HandleDot();
        for (int i = 0; i < pageParent.childCount; i++)
        {
            if(i == 0)
            {
                pageParent.GetChild(i).GetComponent<CanvasGroup>().alpha = 1;
                //Transform dot = paginationUI.GetChild(i).GetChild(0);
                //dot.DOScale(1, .25f).SetEase(Ease.OutBounce);

                continue;
            }
            pageParent.GetChild(i).GetComponent<CanvasGroup>().alpha = 0;
        }
    }
    void Next()
    {
        ManagePages();
    }

    void ManagePages()
    {
        Transform item = pageParent.GetChild(index);
        currentCG = item.GetComponent<CanvasGroup>();
        currentCG.DOFade(0, .25f);
        //Debug.Log(currentCG.gameObject.name);
        index = Wrap();
        //Debug.Log(index);
        item = pageParent.GetChild(index);
        currentCG = item.GetComponent<CanvasGroup>();
        //Debug.Log(currentCG.gameObject.name);
        currentCG.DOFade(1, .25f).SetEase(Ease.InSine);
        HandleDot();



    }
    void HandleDot()
    {
        Transform dot = paginationUI.GetChild(index).GetChild(0);
        dot.DOScale(1, .15f).SetEase(Ease.InSine);

        for(int i = paginationUI.childCount -1; i > index; i--)
        {
            dot = paginationUI.GetChild(i).GetChild(0);
            dot.DOScale(0, .15f).SetEase(Ease.InSine);
        }
        DrawLine();
    }
    void DrawLine()
    {
        lt.points = new List<RectTransform>();
        for (int i = 0; i < index + 1; i++)
        {
            lt.points.Add(paginationUI.GetChild(i).GetComponent<RectTransform>());
        }
        //uILineIllustrator.points = index + 1;
        //uILineIllustrator.points.Clear();
        lt.DrawLine();
        return;
        lr.positionCount = index + 1;
        for (int i =0; i< index + 1; i++)
        {
            lr.SetPosition(i, paginationUI.GetChild(i).GetChild(0).position);
        }
    }


    int Wrap()
    {
        if (index + 1 >= pageParent.childCount)
        {
            lt.Clear();
            index = 0;
            lt.points.Clear();
        }
        else
        {
            index++;
        }
        return index;
    }

}
