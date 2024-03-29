using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentCanvas : MonoBehaviour
{
    public PersistentCanvas Instance { get; private set; }

    private static PersistentCanvas instanceCheck;
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
