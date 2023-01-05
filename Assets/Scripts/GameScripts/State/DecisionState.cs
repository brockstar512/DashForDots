using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DecisionState : BaseState
{
    [SerializeField] Button confirm;
    [SerializeField] Button cancel;
    GridManager GridManager;

    public override void Initialize(StateManager StateManager)
    {
        this.GridManager = StateManager.gridManager;
        cg.DOFade(0, .1f).OnComplete(() => { this.GetPage.DOScale(Vector3.zero, 0); });
    }
    public override void EnterState(StateManager stateManager)
    {
        this.GetPage.DOScale(Vector3.one, 0).OnComplete(() => { cg.DOFade(1, .25f); });
    }
    public override void UpdateState(StateManager stateManager)
    {
        stateManager.HandleScreenInputs();
    }
    public override void LeaveState()
    {
        cg.DOFade(0, .1f).OnComplete(() => { this.GetPage.DOScale(Vector3.zero, 0); });
    }

}
