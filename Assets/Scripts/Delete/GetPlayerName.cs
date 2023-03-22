using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GetPlayerName : MonoBehaviour
{
    [SerializeField] public  TMP_Dropdown dropdown;
    private static GetPlayerName instance;
    public static string PlayerName
    {
        get
        {           
            return instance.dropdown.options[instance.dropdown.value].text;
        }
    }
    private void Awake()
    {
        instance = this;
    }

}
