using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DecisionState : BaseState
{
    StateManager StateManager;

    public override void Initialize()
    {
        this.cg = GetComponent<CanvasGroup>();
        cg.DOFade(0, .1f).OnComplete(() => { this.GetPage.DOScale(Vector3.zero, 0); });


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
