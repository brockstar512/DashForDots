using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CanvasGroupThemeHelper : MonoBehaviour
{
    public CanvasGroup lightTheme;
    public CanvasGroup darkTheme;
 
    public virtual void Start()
    {
        Subscribe();
    }

    public void Subscribe()
    {

        SwitchToggle.Instance.lightMode += ToLightUI;
        SwitchToggle.Instance.darkMode += ToDarkUI;

        if (SwitchToggle.Instance.currentTheme == SwitchToggle.Theme.lightTheme)
        {
            ToLightUI();
        }
        else
        {
            ToDarkUI();
        }
    }

    public void ToDarkUI()
    {
        lightTheme.transform.SetSiblingIndex(0);
        darkTheme.transform.SetSiblingIndex(1);
        darkTheme.DOFade(1, .25f).OnComplete(() => lightTheme.alpha = 0f);
    }
    //if this is taking too long i could try creating a sequence. forcing it to turn then recuringliy call it
    public void ToLightUI()
    {
        darkTheme.transform.SetSiblingIndex(0);
        lightTheme.transform.SetSiblingIndex(1);
        lightTheme.DOFade(1, .25f).OnComplete(() => darkTheme.alpha = 0f);
    }


    public void OnDestroy()
    {
        UnSubscribe();
    }

    public void UnSubscribe()
    {
        SwitchToggle.Instance.lightMode -= ToLightUI;
        SwitchToggle.Instance.darkMode -= ToDarkUI;
    }
}
