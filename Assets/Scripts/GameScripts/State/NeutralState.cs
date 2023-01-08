using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class NeutralState : BaseState
{

    public override void Initialize(StateManager StateManager)
    {
    }
    public override void EnterState(StateManager stateManager)
    {
        this.GetPage.DOScale(Vector3.one, 0).OnComplete(() => { cg.DOFade(1, .25f); });
    }

    public override void UpdateState(StateManager stateManager)
    {
        stateManager.HandleScreenInputs();

        if (stateManager.camController.m_Lens.OrthographicSize < 23)
        {
            stateManager.SwitchState(stateManager.ExploringState);
        }
        if (stateManager.gridManager.hasNeighborDot && stateManager.gridManager.hasCurrentDot)
        {
            stateManager.SwitchState(stateManager.DecisionState);
        }

    }

    public override void LeaveState()
    {
        cg.DOFade(0, .1f).OnComplete(() => { this.GetPage.DOScale(Vector3.zero, 0); });

    }
}
