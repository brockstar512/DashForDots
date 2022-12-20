using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VectorGraphics;
using DG.Tweening;


public class ColorThemeHelper : MonoBehaviour
{
    //change to enum
    //and have another sheet have all the colors
    public Color32 lightTheme = new Color32(255, 187, 50, 255);
    public Color32 darkTheme = new Color32(155, 107, 255, 255);
    public SVGImage image;


    private void Awake()
    {
        image = GetComponent<SVGImage>();
    }
}
