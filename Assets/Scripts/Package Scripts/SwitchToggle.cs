using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.VectorGraphics;
using DG.Tweening;
using System;


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
    
    public enum Theme
    {
        lightTheme,
        darkTheme
    }
    public Theme currentTheme;
    public static SwitchToggle Instance { get; private set; }
    public event Action lightMode;
    public event Action darkMode;

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

        toggle = GetComponent<Toggle>();
        handlePosition = uiHandleRectTransform.anchoredPosition;
        toggle.onValueChanged.AddListener(OnSwitch);

        if (toggle.isOn)
            OnSwitch(true);
    }

    void OnSwitch(bool isOn)
    {
        //Debug.Log($"is On? {isOn}");
        uiHandleRectTransform.DOAnchorPos(isOn ? handlePosition : handlePosition * -1, .25f);

        switch (isOn)
        {
            case true:
                this.lightMode?.Invoke();
                toggleIcone.sprite = lightImage;
                currentTheme = Theme.lightTheme;
                break;
            case false:
                this.darkMode?.Invoke();
                toggleIcone.sprite = darkImage;
                currentTheme = Theme.darkTheme;
                break;
        }
        //call terry wednesday
        //meeting thursday at 1
    }

    private void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(OnSwitch);
    }
}
