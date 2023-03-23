using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static OnlineSubMenu;
using UnityEngine.UI;
using DG.Tweening;

public class DotLoadingAnimation : MonoBehaviour
{
    [SerializeField] public List<GameObject> loadingView;
    private Coroutine coroutine;
    private Tween tween;
    private void OnEnable()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(LoadingAnimation());
    }
    int index = 0;
    private IEnumerator LoadingAnimation()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            index = (index + 1) % loadingView.Count;
            for (int i = 0; i < loadingView.Count; i++)
            {
                Graphic graphic = loadingView[i].GetComponent<Graphic>();
                if (graphic != null)
                {
                    if (index == i)
                    {
                        graphic.color = Color.black;
                        loadingView[i].transform.localScale = Vector3.one * 1.2f;                        
                        loadingView[i].transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InOutExpo);
                        
                    }
                    else
                    {
                        graphic.color = Color.white;
                        loadingView[i].transform.localScale = Vector3.one;
                    }
                }
            }
        }
    }
}