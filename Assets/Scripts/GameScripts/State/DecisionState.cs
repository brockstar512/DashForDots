using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DecisionState : BaseState
{
    public override void Initialize()
    {
        this.cg = GetComponent<CanvasGroup>();

    }
    public override void EnterState(StateManager stateManager)
    {
        this.GetPage.DOScale(Vector3.one, 0).OnComplete(() => { cg.DOFade(1, .25f); });
    }
    public override void UpdateState(StateManager stateManager)
    {

    }
    public override void LeaveState()
    {

    }
}
