using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkManagerInstanceCheck : MonoBehaviour
{
    private static NetworkManagerInstanceCheck instanceCheck;
    private void Start()
    {
        if (instanceCheck == null)
        {
            instanceCheck = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }   
}
