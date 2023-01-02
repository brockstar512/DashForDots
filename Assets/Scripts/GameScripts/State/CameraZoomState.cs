using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraZoomState : CameraBaseState
{
    private float zoomOutMin = 5;
    private float zoomOutMax;
    //[SerializeField] CinemachineVirtualCamera camController;//this can be initialized by what the parent passes in in update

    public override void Initialize()
    {
        //zoomOutMax = camController.m_Lens.OrthographicSize;
        //zoomOutMax = cam.m_Lens.OrthographicSize;

    }
    public override void EnterState(StateManager stateManager)
    {

    }


    // Update is called once per frame
    public override void UpdateState(StateManager stateManager)
    {
        //this will be determined by parent state
        //if (Input.touchCount == 2)
        //{
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            zoom(stateManager.camController,difference * 0.01f);
        //}

        zoom(stateManager.camController,Input.GetAxis("Mouse ScrollWheel"));
    }

    public override void LeaveState()
    {

    }

    void zoom(CinemachineVirtualCamera camController, float increment)
    {
        camController.m_Lens.OrthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomOutMin, zoomOutMax);//pass in camera
    }
}
