using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Cinemachine;

public class ExploringState : BaseState
{
    StateManager StateManager;
    [SerializeField] Button reset;
    [SerializeField] CinemachineVirtualCamera camController;
    Vector3 cachedPos;
    public override void Initialize()
    {
        this.cg = GetComponent<CanvasGroup>();
        cg.DOFade(0, .1f).OnComplete(() => { this.GetPage.DOScale(Vector3.zero, 0); });
        //reset.onClick.AddListener(Reset);


    }
    public override void EnterState(StateManager stateManager)
    {
        this.GetPage.DOScale(Vector3.one, 0).OnComplete(() => { cg.DOFade(1, .25f); });
        cachedPos = camController.transform.position;

    }
    public override void UpdateState(StateManager stateManager)
    {

    }
    public override void LeaveState()
    {
        //cg.DOFade(0, .1f).OnComplete(() => { this.GetPage.DOScale(Vector3.zero, 0); });
    }

    private void Reset()
    {
        camController.transform.position = cachedPos;
        StateManager.SwitchState(this);
        //go to eutral state
    }
}
