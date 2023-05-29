using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CopiedTextInfo : MonoBehaviour
{
    public static CopiedTextInfo Instance { get; private set; }
    [SerializeField] CanvasGroup cg;
    bool isOn = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            //Input.multiTouchEnabled = false;
        }
    }

    public void Show()
    {
        if (isOn)
            return;

        isOn = true;
        cg.DOFade(1, .15f);
        Invoke("Hide",3);
    }

    void Hide()
    {
        cg.DOFade(0, .15f).OnComplete(()=> isOn = false);
    }

}
