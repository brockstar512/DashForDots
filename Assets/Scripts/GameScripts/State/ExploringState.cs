using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Cinemachine;

public class ExploringState : BaseState
{
    [SerializeField] Button reset;
    bool isResetting = false;

    void Awake()
    {
    }

    public override void Initialize(StateManager StateManager)
    {
        this.cg = GetComponent<CanvasGroup>();
        cg.DOFade(0, .1f).OnComplete(() => { this.GetPage.DOScale(Vector3.zero, 0); });
        reset.onClick.AddListener(delegate { Reset(StateManager);});
    }

    public override void EnterState(StateManager stateManager)
    {
        this.GetPage.DOScale(Vector3.one, 0).OnComplete(() => { cg.DOFade(1, .25f); });
    }

    public override void UpdateState(StateManager stateManager)
    {
        if (isResetting)
            return;
        stateManager.HandleScreenInputs();


        if (stateManager.camController.m_Lens.OrthographicSize > 23)
        {
            stateManager.SwitchState(stateManager.NeutralState);
        }
    }
    public override void LeaveState()
    {
        isResetting = false;
        cg.DOFade(0, .1f).OnComplete(() => { this.GetPage.DOScale(Vector3.zero, 0); });
    }

    private void Reset(StateManager StateManager)
    {
        isResetting = true;
        Vector3 newPos = new Vector3(0, 0, -10);
        DOTween.To(() => StateManager.camController.m_Lens.OrthographicSize, x => StateManager.camController.m_Lens.OrthographicSize = x, 24, .75f).OnComplete(delegate { StateManager.SwitchState(StateManager.NeutralState); }); ;
        StateManager.camController.transform.DOMove(newPos, .25f).SetEase(Ease.OutSine);//.OnComplete(delegate { StateManager.SwitchState(StateManager.NeutralState); });//.SetEase(Ease.InOutSine);
    }

}
