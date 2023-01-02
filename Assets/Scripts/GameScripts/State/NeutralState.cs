using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;

public class NeutralState : BaseState
{
    StateManager StateManager;
    CameraBaseState cameraState;
    public CameraZoomState CameraZoomState;
    public CameraPanState CameraPanState;

    public override void Initialize()
    {
        this.cg = GetComponent<CanvasGroup>();
    }
    public override void EnterState(StateManager stateManager)
    {
        this.StateManager = stateManager;
        this.GetPage.DOScale(Vector3.one, 0).OnComplete(() => { cg.DOFade(1, .25f); });
    }
    public override void UpdateState(StateManager stateManager)
    {
        if (Input.touchCount == 2)
        {
            cameraState = CameraZoomState;
        }
        else if (Input.touchCount == 1)
        {
            //cameraState = CameraPanState;
        }
        else
        {

        }

        cameraState.UpdateState(StateManager);

    }



    public override void LeaveState()
    {
        cg.DOFade(0, .1f).OnComplete(() => { this.GetPage.DOScale(Vector3.zero, 0); });

    }
}
