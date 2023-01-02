using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class NeutralState : BaseState
{

    public override void Initialize(StateManager StateManager)
    {
    }
    public override void EnterState(StateManager stateManager)
    {
        this.GetPage.DOScale(Vector3.one, 0).OnComplete(() => { cg.DOFade(1, .25f); });
    }

    public override void UpdateState(StateManager stateManager)
    {
        stateManager.HandleScreenInputs();

        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;
            //Debug.Log($"Here is the difference {difference}  and here is the current mag {currentMagnitude}");
            //Debug.Log("increment::   "+ Input.GetAxis("Mouse ScrollWheel"));
        }
        if (stateManager.camController.m_Lens.OrthographicSize < 23)
        {
            stateManager.SwitchState(stateManager.ExploringState);
        }

    }

    public override void LeaveState()
    {
        cg.DOFade(0, .1f).OnComplete(() => { this.GetPage.DOScale(Vector3.zero, 0); });

    }
}
