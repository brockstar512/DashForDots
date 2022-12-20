using UnityEngine;
using Unity.VectorGraphics;
using DG.Tweening;


public class ColorThemeHelper : MonoBehaviour
{
    //change to enum
    //and have another sheet have all the colors
    public Color32 lightTheme;
    public Color32 darkTheme;
    public SVGImage image;

    private void Start()
    {
        image = GetComponent<SVGImage>();
        SwitchToggle.Instance.lightMode += ToLightUI;
        SwitchToggle.Instance.darkMode += ToDarkUI;

        if(SwitchToggle.Instance.currentTheme == SwitchToggle.Theme.lightTheme)
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
        image.DOColor(this.darkTheme, .25f);
    }
    public void ToLightUI()
    {
        image.DOColor(this.lightTheme, .25f);   
    }

    private void OnDestroy()
    {
        SwitchToggle.Instance.lightMode -= ToLightUI;
        SwitchToggle.Instance.darkMode -= ToDarkUI;
    }
}
