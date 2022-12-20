using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentCanvas : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
