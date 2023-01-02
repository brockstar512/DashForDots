using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Cinemachine;

public class ExploringState : BaseState
{
    StateManager StateManager;
    [SerializeField] Button reset;
    [SerializeField] CinemachineVirtualCamera camController;
    Vector3 cachedPos;
    private float zoomOutMin = 5;
    private float zoomOutMax;

    void Awake()
    {
        zoomOutMax = camController.m_Lens.OrthographicSize;
    }

    public override void Initialize()
    {
        this.cg = GetComponent<CanvasGroup>();
        cg.DOFade(0, .1f).OnComplete(() => { this.GetPage.DOScale(Vector3.zero, 0); });
        //reset.onClick.AddListener(Reset);


    }
    public override void EnterState(StateManager stateManager)
    {
        this.GetPage.DOScale(Vector3.one, 0).OnComplete(() => { cg.DOFade(1, .25f); });
        cachedPos = camController.transform.position;

    }
    public override void UpdateState(StateManager stateManager)
    {
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            zoom(difference * 0.01f);
        }

        zoom(Input.GetAxis("Mouse ScrollWheel"));
    }
    public override void LeaveState()
    {
        //cg.DOFade(0, .1f).OnComplete(() => { this.GetPage.DOScale(Vector3.zero, 0); });
    }

    private void Reset()
    {
        camController.transform.position = cachedPos;
        camController.m_Lens.OrthographicSize = 24;
        StateManager.SwitchState(this);
        //go to eutral state
    }

    void zoom(float increment)
    {
        camController.m_Lens.OrthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomOutMin, zoomOutMax);
    }

    //camer zoom state that two scripts own
    //this just manages ui
}
