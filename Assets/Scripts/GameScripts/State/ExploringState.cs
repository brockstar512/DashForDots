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
        reset.onClick.AddListener(delegate {
            StateManager.gridManager.Reset();
            StateManager.SwitchState(StateManager.ResetState);
        });
    }  
   
    public override void EnterState(StateManager stateManager)
    {
        if (PlayerHandler.Instance.CurrentPlayerTurn == Enums.CurrentPlayerTurn.AI_Turn || stateManager.currentPlayerTurn.playerType == Enums.PlayerType.OpponentPlayer)
            return;
        this.GetPage.DOScale(Vector3.one, 0).OnComplete(() => { cg.DOFade(1, .25f); });
    }

    public override void UpdateState(StateManager stateManager)
    {
        stateManager.HandleScreenInputs();

        if (stateManager.camController.m_Lens.OrthographicSize > (stateManager.zoomOutMax - 1))
        {
            stateManager.SwitchState(stateManager.NeutralState);
        }
        if (stateManager.gridManager.neighborDot != null && stateManager.gridManager.currentDot != null)
        {
            if (PlayerHandler.Instance.CurrentPlayerTurn == Enums.CurrentPlayerTurn.AI_Turn)
                return;
            stateManager.SwitchState(stateManager.DecisionState);
        }
    }
    public override void LeaveState()
    {        
        cg.DOFade(0, .1f).OnComplete(() => { this.GetPage.DOScale(Vector3.zero, 0); });
    }

}
