using UnityEngine;
using Unity.VectorGraphics;
using DG.Tweening;
using System.Threading.Tasks;



public class ColorThemeHelper : MonoBehaviour
{

    public Color32 lightTheme;
    public Color32 darkTheme;
    protected SVGImage image;


    public virtual void GetTarget()
    {
        image = GetComponent<SVGImage>();

    }
    public virtual async void Start()
    {
        GetTarget();
        await Subscribe();
        //Debug.Log("Hello");
    }

    public void ToDarkUI()
    {
        image.DOColor(this.darkTheme, .25f);
    }
    //if this is taking too long i could try creating a sequence. forcing it to turn then recuringliy call it
    public void ToLightUI()
    {

        image.DOColor(this.lightTheme, .25f);   
    }


    public void OnDestroy()
    {
         UnSubscribe();
    }
    public void  UnSubscribe()
    {
        SwitchToggle.Instance.lightMode -= ToLightUI;
        SwitchToggle.Instance.darkMode -= ToDarkUI;
    }

    public async Task Subscribe()
    {
        Debug.Log("STACK");

        SwitchToggle.Instance.lightMode += ToLightUI;
        SwitchToggle.Instance.darkMode += ToDarkUI;

        if (SwitchToggle.Instance.currentTheme == SwitchToggle.Theme.lightTheme)
        {
            ToLightUI();
        }
        else
        {
            ToDarkUI();//this is causing it to be overrten
        }
        //await Task.Delay(1000);
        Debug.Log("SUBSCRIBING");
        await Task.Yield();
    }
    public async Task Subscribe(bool sub)
    {
        Debug.Log("STACK");

        SwitchToggle.Instance.lightMode += ToLightUI;
        SwitchToggle.Instance.darkMode += ToDarkUI;

        if (SwitchToggle.Instance.currentTheme == SwitchToggle.Theme.lightTheme)
        {
            image.color = this.lightTheme;
        }
        else
        {
            image.color = this.darkTheme;
        }
        //await Task.Delay(1000);
        Debug.Log("SUBSCRIBING");
        await Task.Yield();
    }
}
