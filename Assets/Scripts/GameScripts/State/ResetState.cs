using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ResetState : BaseState
{
    StateManager GetStateManager;
    public override void EnterState(StateManager stateManager)
    {
        GetStateManager = stateManager;
        this.GetPage.DOScale(Vector3.one, 0).OnComplete(() => { cg.DOFade(1, .25f); });
        Vector3 newPos = new Vector3(0, 0, -10);
        DOTween.To(() => stateManager.camController.m_Lens.OrthographicSize, x => stateManager.camController.m_Lens.OrthographicSize = x, stateManager.zoomOutMax, .75f).SetEase(Ease.InOutSine);
        stateManager.camController.transform.DOMove(newPos, .75f).SetEase(Ease.InOutSine).OnComplete(() => stateManager.SwitchState(stateManager.NeutralState));
        switch (PlayerHandler.Instance.GetPlayerCount)
        {
            case Enums.PlayerCount.TowPlayer:
                GetStateManager.selectedBoard.GetComponent<BoxCollider>().size = new Vector3(14f, 20f, 7.22f);
                GetStateManager.selectedBoard.GetComponent<BoxCollider>().center = new Vector3(0f, 0f, -1.08f);
                break;
            case Enums.PlayerCount.ThreePlayer:
                GetStateManager.selectedBoard.GetComponent<BoxCollider>().size = new Vector3(18f, 24f, 7.22f);
                GetStateManager.selectedBoard.GetComponent<BoxCollider>().center = new Vector3(0f, 0f, -1.08f);
                break;
            case Enums.PlayerCount.FourPlayer:
                GetStateManager.selectedBoard.GetComponent<BoxCollider>().size = new Vector3(27f, 48f, 7.22f);
                GetStateManager.selectedBoard.GetComponent<BoxCollider>().center = new Vector3(0f, 0f, -1.08f);
                break;
            default:
                break;
        }
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