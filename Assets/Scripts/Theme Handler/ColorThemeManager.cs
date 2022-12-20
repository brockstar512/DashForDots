using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VectorGraphics;
using DG.Tweening;

public class ColorThemeManager : MonoBehaviour
{
    public static ColorThemeManager Instance { get; private set; }
    public enum Theme
    {
        lightTheme,
        darkTheme
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void ToDarkUI(ColorThemeHelper item)
    {
        //Sprite color = uiHandleRectTransform.GetComponent<SVGImage>().sprite;
        SVGImage image = item.image;
        image.DOColor(item.lightTheme, .25f);
    }
    public void ToLightUI(ColorThemeHelper item)
    {
        //Sprite color = uiHandleRectTransform.GetComponent<SVGImage>().sprite;
        SVGImage image = item.image; ;
        image.DOColor(item.darkTheme, .25f);
    }

}
