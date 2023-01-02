using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public abstract class BaseState : MonoBehaviour
{
    [HideInInspector]
    public CanvasGroup cg { get { return GetComponent<CanvasGroup>(); } }
    public Transform GetPage { get { return GetComponent<Transform>(); } }
    public abstract void Initialize(StateManager StateManager);
    public abstract void EnterState(StateManager stateManager);
    public abstract void UpdateState(StateManager stateManager);
    public abstract void LeaveState();


}
