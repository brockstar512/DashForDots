using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TurnSetUpState : BaseState
{
    [SerializeField] TimerManager timerManager;
    [SerializeField] Transform playerUIDotsParent;
    List<Transform> playerUIDots;

    public override void Initialize(StateManager StateManager)
    {
        cg.DOFade(0, .1f).OnComplete(() => { this.GetPage.DOScale(Vector3.zero, 0); });

        playerUIDots = new List<Transform>();

        for(int i = 0; i < playerUIDotsParent.childCount - 1; i++)
        {
            if (playerUIDotsParent.GetChild(i).gameObject.activeInHierarchy)
            {
                playerUIDots.Add(playerUIDotsParent.GetChild(i));
            }
        }


    }
    public override void EnterState(StateManager stateManager)
    {
        this.GetPage.DOScale(Vector3.one, 0).OnComplete(() => { cg.DOFade(1, .25f); });
        //Vector3 newPos = new Vector3(0, 0, -10);
        //DOTween.To(() => stateManager.camController.m_Lens.OrthographicSize, x => stateManager.camController.m_Lens.OrthographicSize = x, stateManager.zoomOutMax, .75f).SetEase(Ease.InOutSine);
        //stateManager.camController.transform.DOMove(newPos, .75f).SetEase(Ease.InOutSine).OnComplete(() => stateManager.SwitchState(stateManager.NeutralState));
        //this.GetPage.DOScale(Vector3.one, 0).OnComplete(() => { cg.DOFade(1, .25f); });

        //move the ui dots
        //.OnComplete(() => { StartClock(); });
        StartClock(stateManager);
    }

    public override void UpdateState(StateManager stateManager)
    {

    }

    public override void LeaveState()
    {
        cg.DOFade(0, .1f).OnComplete(() => { this.GetPage.DOScale(Vector3.zero, 0); });

    }




    async void StartClock(StateManager stateManager)
    {
        await timerManager.StartTimer();
        stateManager.SwitchState(stateManager.DecisionState);
    }
}
