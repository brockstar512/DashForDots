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
    [SerializeField] Button back;
    CanvasGroup currentCG;
    int index = 0;
    [SerializeField] LineUI line;
    [SerializeField] GameObject gameTitle;

    private void Awake()
    {
        pageController.onClick.RemoveAllListeners();
        pageController.onClick.AddListener(Next);
        back.onClick.RemoveAllListeners();
        back.onClick.AddListener(NavigationManager.Instance.Back);
        back.onClick.AddListener(delegate { gameTitle.gameObject.SetActive(true);});

    }
    private void OnEnable()
    {
        gameTitle.gameObject.SetActive(false);

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
            line.points = new List<RectTransform>();
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
        
        for (int i = 0; i < index + 1; i++)
        {
            line.AddLine(paginationUI.GetChild(i).GetComponent<RectTransform>());
        }

        line.DrawLine();
    }


    int Wrap()
    {
        if (index + 1 >= pageParent.childCount)
        {
            line.Clear();
            index = 0;
        }
        else
        {
            index++;
        }
        return index;
    }

}
