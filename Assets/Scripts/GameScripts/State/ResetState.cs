using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ResetState : BaseState
{
    public override void EnterState(StateManager stateManager)
    {
        this.GetPage.DOScale(Vector3.one, 0).OnComplete(() => { cg.DOFade(1, .25f); });
        Vector3 newPos = new Vector3(0, 0, -10);
        DOTween.To(() => stateManager.camController.m_Lens.OrthographicSize, x => stateManager.camController.m_Lens.OrthographicSize = x, stateManager.zoomOutMax, .75f).SetEase(Ease.InOutSine);
        stateManager.camController.transform.DOMove(newPos, .75f).SetEase(Ease.InOutSine).OnComplete(() => stateManager.SwitchState(stateManager.NeutralState));

    }

    public override void Initialize(StateManager StateManager)
    {
        cg.DOFade(0, .1f).OnComplete(() => { this.GetPage.DOScale(Vector3.zero, 0); });
    }

    public override void LeaveState()
    {
        cg.DOFade(0, .1f).OnComplete(() => { this.GetPage.DOScale(Vector3.zero, 0); });

    }

    public override void UpdateState(StateManager stateManager)
    {

    }
}