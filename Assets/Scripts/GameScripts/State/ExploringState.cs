using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Cinemachine;

public class ExploringState : BaseState
{
    [SerializeField] Button reset;

    public override void Initialize(StateManager StateManager)
    {
        cg.DOFade(0, .1f).OnComplete(() => { this.GetPage.DOScale(Vector3.zero, 0); });
        reset.onClick.AddListener(delegate { StateManager.SwitchState(StateManager.ResetState);});
    }

    public override void EnterState(StateManager stateManager)
    {
        this.GetPage.DOScale(Vector3.one, 0).OnComplete(() => { cg.DOFade(1, .25f); });
    }

    public override void UpdateState(StateManager stateManager)
    {

        stateManager.HandleScreenInputs();


        if (stateManager.camController.m_Lens.OrthographicSize > 23)
        {
            stateManager.SwitchState(stateManager.NeutralState);
        }
    }
    public override void LeaveState()
    {
        cg.DOFade(0, .1f).OnComplete(() => { this.GetPage.DOScale(Vector3.zero, 0); });
    }




}
