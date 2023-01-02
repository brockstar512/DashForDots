using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public abstract class CameraBaseState 
{
    public abstract void Initialize();
    public abstract void EnterState(StateManager stateManager);
    public abstract void UpdateState(StateManager stateManager);
    public abstract void LeaveState();

}
