using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class NeutralState : BaseState
{
    
    public override void Initialize()
    {
        this.cg = GetComponent<CanvasGroup>();
    }
    public override void EnterState(StateManager stateManager)
    {
        this.GetPage.DOScale(Vector3.one, 0);
        cg.DOFade(1, .25f);


    }
    public override void UpdateState(StateManager stateManager)
    {

    }
    public override void LeaveState()
    {
        cg.DOFade(0,.15f);
        this.GetPage.DOScale(Vector3.zero, 0);

    }
}
