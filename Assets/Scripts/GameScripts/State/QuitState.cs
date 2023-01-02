using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class QuitState : BaseState
{
    [SerializeField] Button cancel;
    [SerializeField] Button confirm;
    public override void Initialize(StateManager StateManager)
    {
        cg.DOFade(0, .1f).OnComplete(() => { this.GetPage.DOScale(Vector3.zero, 0); });

        cancel.onClick.AddListener(delegate { Cancel(StateManager); });
        confirm.onClick.AddListener(Quit);

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
        cg.DOFade(0, .1f).OnComplete(() => { this.GetPage.DOScale(Vector3.zero, 0); });
    }

    void Cancel(StateManager StateManager)
    {
        StateManager.SwitchState(StateManager.NeutralState);
    }
    private void Quit()
    {
        Debug.Log("Quit");
    }
}
