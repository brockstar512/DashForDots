using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.VectorGraphics;
using DG.Tweening;


public class SwitchToggle : MonoBehaviour
{
    [SerializeField] RectTransform uiHandleRectTransform;
    private Toggle toggle;
    private Vector2 handlePosition;
    public Color32 yellow = new Color32(255, 187, 50, 255);
    public Color32 purple = new Color32(155, 107, 255, 255);
    [SerializeField] Sprite lightImage;
    [SerializeField] Sprite darkImage;
    [SerializeField] SVGImage toggleIcone;


    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        handlePosition = uiHandleRectTransform.anchoredPosition;
        toggle.onValueChanged.AddListener(OnSwitch);
        if (toggle.isOn)
            OnSwitch(true);
    }

    void OnSwitch(bool isOn)
    {
        Debug.Log($"is On? {isOn}");
        uiHandleRectTransform.DOAnchorPos(isOn ? handlePosition : handlePosition * -1, .25f);
        switch (isOn)
        {
            case true:
                ToLightUI();
                break;
            case false:
                ToDarkUI();
                break;
        }

    }

    private void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(OnSwitch);
    }

    private void ToDarkUI()
    {
        //Sprite color = uiHandleRectTransform.GetComponent<SVGImage>().sprite;
        SVGImage color = uiHandleRectTransform.GetComponent<SVGImage>();
        toggleIcone.sprite = darkImage;
        color.DOColor(yellow, .25f);
    }
    private void ToLightUI()
    {
        //Sprite color = uiHandleRectTransform.GetComponent<SVGImage>().sprite;
        SVGImage color = uiHandleRectTransform.GetComponent<SVGImage>();
        toggleIcone.sprite = lightImage;
        color.DOColor(purple, .25f);
    }
}
