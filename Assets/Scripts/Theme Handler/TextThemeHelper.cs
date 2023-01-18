using TMPro;
using UnityEngine;
using DG.Tweening;
public class TextThemeHelper : MonoBehaviour
{
    public Color32 lightTheme;
    public Color32 darkTheme;
    TextMeshProUGUI image;

    public void GetTarget()
    {
        //Debug.Log("Get text color change");
        image = GetComponent<TextMeshProUGUI>();
    }

    public void Start()
    {
        GetTarget();
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
        image.DOColor(this.darkTheme, .25f);
    }
    public void ToLightUI()
    {
        image.DOColor(this.lightTheme, .25f);
    }

    public void OnDestroy()
    {
        SwitchToggle.Instance.lightMode -= ToLightUI;
        SwitchToggle.Instance.darkMode -= ToDarkUI;
    }
}
