using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspectingState : BaseState
{
    Transform _target;
    public override void Initialize()
    {
        this.cg = GetComponent<CanvasGroup>();

    }

    public void View(Transform target)
    {
        _target = target;
    }

    public override void EnterState(StateManager stateManager)
    {

    }
    public override void UpdateState(StateManager stateManager)
    {

    }
    public override void LeaveState()
    {

    }
}
