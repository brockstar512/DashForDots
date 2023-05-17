using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using UnityEngine.UI;


public class InspectingState : BaseState
{
    StateManager GetStateManager;
    public override void Initialize(StateManager StateManager)
    {
        cg.DOFade(0, .1f).OnComplete(() => { this.GetPage.DOScale(Vector3.zero, 0); });
    }

    private Tween tween;
    public override void EnterState(StateManager stateManager)
    {
        GetStateManager = stateManager;
        Utility.CheckIsTweening(this.GetPage);
        this.GetPage.DOScale(Vector3.one, 0).OnComplete(() => { cg.DOFade(1, .25f); });
        Vector3 newPos = new Vector3(stateManager.target.transform.position.x, stateManager.target.transform.position.y, -10);
        if (tween!=null)
        {
            Utility.CheckIsTweening(tween);            
        }
        tween = DOTween.To(() => stateManager.camController.m_Lens.OrthographicSize, x => stateManager.camController.m_Lens.OrthographicSize = x, 5, .75f).SetEase(Ease.InOutSine);
        Utility.CheckIsTweening(stateManager.camController.transform);
        stateManager.camController.transform.DOMove(newPos, .75f).SetEase(Ease.InOutSine).OnComplete(() => stateManager.SwitchState(stateManager.ExploringState));
    }
    public override void UpdateState(StateManager stateManager)
    {

    }


    public override void LeaveState()
    {
        Utility.CheckIsTweening(cg);
        cg.DOFade(0, .1f).OnComplete(() => { this.GetPage.DOScale(Vector3.zero, 0); });
        switch (PlayerHandler.Instance.GetPlayerCount)
        {
            case Enums.PlayerCount.TowPlayer:
                GetStateManager.selectedBoard.GetComponent<BoxCollider>().size = new Vector3(11f, 15f, 7.22f);
                GetStateManager.selectedBoard.GetComponent<BoxCollider>().center = new Vector3(0f, -1.10f, -1.08f);
                break;
            case Enums.PlayerCount.ThreePlayer:
                GetStateManager.selectedBoard.GetComponent<BoxCollider>().size = new Vector3(17f, 21f, 7.22f);
                GetStateManager.selectedBoard.GetComponent<BoxCollider>().center = new Vector3(0f, -1.4f, -1.08f);
                break;
            case Enums.PlayerCount.FourPlayer:
                GetStateManager.selectedBoard.GetComponent<BoxCollider>().size = new Vector3(27f, 32f, 7.22f);
                GetStateManager.selectedBoard.GetComponent<BoxCollider>().center = new Vector3(0f, -2.22f, -1.08f);
                break;
            default:
                break;
        }
    }

}
