using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotColorHelper : ColorThemeHelper 

{
    // Start is called before the first frame update
    public override void Start()
    {
        base.GetTarget();

        
            SwitchToggle.Instance.lightMode += ToLightUI;
            SwitchToggle.Instance.darkMode += ToDarkUI;

            if (SwitchToggle.Instance.currentTheme == SwitchToggle.Theme.lightTheme)
            {
                image.color = this.lightTheme;
                Debug.Log("hjhjjhjhjhj");

            }
            else
            {
                Debug.Log("hjhjhjhjhjhj");

                image.color = this.darkTheme;
            }
        
    }


}
