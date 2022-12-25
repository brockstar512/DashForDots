using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BotFactory : MonoBehaviour
{
    [SerializeField] Button back;
    private void Awake()
    {
        back.onClick.RemoveAllListeners();
        back.onClick.AddListener(NavigationManager.Instance.Back);
    }
}
