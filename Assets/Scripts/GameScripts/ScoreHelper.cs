using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VectorGraphics;
using DG.Tweening;


public class ScoreHelper : MonoBehaviour
{
   public SVGImage scoreBox;

    private void Awake()
    {
        scoreBox = this.transform.GetChild(0).GetComponent<SVGImage>();
    }

    public void ShowFill()
    {
        Debug.Log("SHOWING FILL");
        scoreBox.DOColor(PlayerPlaceholder.Instance.capColor, .75f).SetEase(Ease.OutSine);
    }

}
