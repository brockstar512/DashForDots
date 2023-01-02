using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using UnityEngine.UI;


public class InspectingState : BaseState
{
    StateManager StateManager;

    [SerializeField]CinemachineVirtualCamera camController;
    [SerializeField] Button reset;
    float zoom;
    public override void Initialize()
    {
        this.cg = GetComponent<CanvasGroup>();
        cg.DOFade(0, .1f).OnComplete(() => { this.GetPage.DOScale(Vector3.zero, 0); });
        reset.onClick.AddListener(Reset);
    }

    public void View(Transform target)
    {
        Vector3 newPos = new Vector3(target.transform.position.x, target.transform.position.y, -10);
        zoom = camController.m_Lens.OrthographicSize;
        DOTween.To(() => zoom, x => zoom = x, 5, .75f).SetEase(Ease.InOutSine);
        camController.transform.DOMove(newPos, .75f).SetEase(Ease.InOutSine);//.SetEase(Ease.InOutSine);
    }

    public override void EnterState(StateManager stateManager)
    {
        this.GetPage.DOScale(Vector3.one, 0).OnComplete(() => { cg.DOFade(1, .25f); });

    }
    public override void UpdateState(StateManager stateManager)
    {
        camController.m_Lens.OrthographicSize = zoom;

    }
    public override void LeaveState()
    {
        cg.DOFade(0, .1f).OnComplete(() => { this.GetPage.DOScale(Vector3.zero, 0); });
    }

    private void Reset()
    {
        Debug.Log("reset");
        StateManager.SwitchState(this);
        //go to eutral state
    }
}
