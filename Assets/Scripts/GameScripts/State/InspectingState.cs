using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using UnityEngine.UI;


public class InspectingState : BaseState
{

    public override void Initialize(StateManager StateManager)
    {
        cg.DOFade(0, .1f).OnComplete(() => { this.GetPage.DOScale(Vector3.zero, 0); });
    }


    public override void EnterState(StateManager stateManager)
    {
        this.GetPage.DOScale(Vector3.one, 0).OnComplete(() => { cg.DOFade(1, .25f); });
        Vector3 newPos = new Vector3(stateManager.target.transform.position.x, stateManager.target.transform.position.y, -10);
        DOTween.To(() => stateManager.camController.m_Lens.OrthographicSize, x => stateManager.camController.m_Lens.OrthographicSize = x, 5, .75f).SetEase(Ease.InOutSine);
        stateManager.camController.transform.DOMove(newPos, .75f).SetEase(Ease.InOutSine).OnComplete(() => stateManager.SwitchState(stateManager.ExploringState));

    }
    public override void UpdateState(StateManager stateManager)
    {
        //stateManager.camController.m_Lens.OrthographicSize = zoom;

    }


    public override void LeaveState()
    {
        cg.DOFade(0, .1f).OnComplete(() => { this.GetPage.DOScale(Vector3.zero, 0); });
    }

}
