using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GG.Infrastructure.Utils.Swipe;
using Cinemachine;


public class CameraPanState : CameraBaseState
{
    [Header("Available movements:")]
    [SerializeField] private bool _up = true;
    [SerializeField] private bool _down = true;
    [SerializeField] private bool _left = true;
    [SerializeField] private bool _right = true;
    [SerializeField] private bool _upLeft = true;
    [SerializeField] private bool _upRight = true;
    [SerializeField] private bool _downLeft = true;
    [SerializeField] private bool _downRight = true;
    CinemachineVirtualCamera cam;

    public override void Initialize()
    {

    }
    public override void EnterState(StateManager stateManager)
    {

    }
    public override void UpdateState(StateManager stateManager)
    {

    }

    public override void LeaveState()
    {

    }
    //this needs to be listened to
    public void OnSwipeHandler(string id)
    {
        Debug.Log("abc " + id);
        switch (id)
        {
            case DirectionId.ID_DOWN:
                MoveUp();
                break;

            case DirectionId.ID_UP:
                MoveDown();
                break;

            case DirectionId.ID_RIGHT:
                MoveLeft();
                break;

            case DirectionId.ID_LEFT:
                MoveRight();
                break;

            case DirectionId.ID_DOWN_RIGHT:
                MoveUpLeft();
                break;

            case DirectionId.ID_DOWN_LEFT:
                MoveUpRight();
                break;

            case DirectionId.ID_UP_RIGHT:
                MoveDownLeft();
                break;

            case DirectionId.ID_UP_LEFT:
                MoveDownRight();
                break;
        }
    }

    private void MoveDownRight()
    {
        if (_downRight)
        {
            cam.transform.position += Vector3.down + Vector3.right;
        }
    }

    private void MoveDownLeft()
    {
        if (_downLeft)
        {
            cam.transform.position += Vector3.down + Vector3.left;
        }
    }

    private void MoveUpRight()
    {
        if (_upRight)
        {
            cam.transform.position += Vector3.up + Vector3.right;
        }
    }

    private void MoveUpLeft()
    {
        if (_upLeft)
        {
            cam.transform.position += Vector3.up + Vector3.left;
        }
    }

    private void MoveRight()
    {
        if (_right)
        {
            cam.transform.position += Vector3.right;
        }
    }

    private void MoveLeft()
    {
        if (_left)
        {
            cam.transform.position += Vector3.left;
        }
    }

    private void MoveDown()
    {
        if (_down)
        {
            cam.transform.position += Vector3.down;
        }
    }

    private void MoveUp()
    {
        if (_up)
        {
            cam.transform.position += Vector3.up;
        }
    }
}
