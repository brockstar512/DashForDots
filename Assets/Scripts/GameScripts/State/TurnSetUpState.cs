using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSetUpState : BaseState
{
    TimerManager timerManager;

    public override void Initialize(StateManager StateManager)
    {
        
    }
    public override void EnterState(StateManager stateManager)
    {
        //this.GetPage.DOScale(Vector3.one, 0).OnComplete(() => { cg.DOFade(1, .25f); });

        //.OnComplete(() => { StartClock(); });
        StartClock();
    }

    public override void UpdateState(StateManager stateManager)
    {

    }

    public override void LeaveState()
    {
        //cg.DOFade(0, .1f).OnComplete(() => { this.GetPage.DOScale(Vector3.zero, 0); });

    }

    async void StartClock()
    {
        await timerManager.StartTimer();
    }
}
