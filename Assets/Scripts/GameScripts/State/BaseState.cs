using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseState : MonoBehaviour
{
    [HideInInspector]
    public CanvasGroup cg;
    public Transform GetPage { get { return GetComponent<Transform>(); } }
    public abstract void Initialize();
    public abstract void EnterState(StateManager stateManager);
    public abstract void UpdateState(StateManager stateManager);
    public abstract void LeaveState();


}
