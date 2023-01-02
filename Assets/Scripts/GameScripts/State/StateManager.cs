using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Cinemachine;
using GG.Infrastructure.Utils.Swipe;

[RequireComponent(typeof(SwipeListenerEvent))]
public class StateManager : MonoBehaviour
{
    [SerializeField] Transform dotsParent;
    [SerializeField] Button quitButton;
    public Transform target;

    [Header("Camera settings")]
    public CinemachineVirtualCamera camController;
    const float zoomOutMin = 5;
    public float zoomOutMax;
    //private bool isZooming;

    [Header("Available movements:")]
    [SerializeField] private bool _up = true;
    [SerializeField] private bool _down = true;
    [SerializeField] private bool _left = true;
    [SerializeField] private bool _right = true;
    [SerializeField] private bool _upLeft = true;
    [SerializeField] private bool _upRight = true;
    [SerializeField] private bool _downLeft = true;
    [SerializeField] private bool _downRight = true;

    [Header("UI States:")]
    BaseState currentState;
    public NeutralState NeutralState;//looking at everythig/reset
    public ExploringState ExploringState;//scrolling around and zooming
    public InspectingState InspectingState;//looking at choice. can scroll
    public QuitState QuitState;
    public DecisionState DecisionState;
    public ResetState ResetState;



    private void Awake()
    {
        HandleDots();
        quitButton.onClick.AddListener(delegate { SwitchState(QuitState); });
        NeutralState.Initialize(this);
        ExploringState.Initialize(this);
        InspectingState.Initialize(this);
        QuitState.Initialize(this);
        DecisionState.Initialize(this);
        ResetState.Initialize(this);
        zoomOutMax = camController.m_Lens.OrthographicSize;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentState = NeutralState;
        currentState.EnterState(this);
    }



    void Update()
    {
        currentState.UpdateState(this);
    }

    public void SwitchState(BaseState state)
    {
        //return;
        Debug.Log("State Change :" + state);

        currentState.LeaveState();
        currentState = state;
        currentState.EnterState(this);

    }

    public void Inspect(Transform dot)
    {

        target = dot;
        SwitchState(InspectingState);
    }

    void HandleDots()
    {
        for(int i = 0; i < dotsParent.childCount; i++)
        {
            Button dot = dotsParent.GetChild(i).GetComponent<Button>();
            dot.onClick.RemoveAllListeners();
            dot.onClick.AddListener(delegate { Inspect(dot.transform); });
        }
    }

    public void HandleScreenInputs()
    {
        if (Input.touchCount == 2)
        {
            //isZooming = true;
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;
            //Debug.Log($"Here is the difference {difference}  and here is the current mag {currentMagnitude}");

            zoom(difference * 0.01f);
        }
  
        #if UNITY_EDITOR
            zoom(Input.GetAxis("Mouse ScrollWheel"));
        #endif

    }

    void zoom(float increment)
    {
        camController.m_Lens.OrthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomOutMin, zoomOutMax);//pass in camera
    }

    public void OnSwipeHandler(string id)
    {
        if (Input.touchCount == 2 || currentState == QuitState)
            return;

        //Debug.Log("abc " + id);
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
            camController.transform.position += Vector3.down + Vector3.right;
        }
    }
    private void MoveDownLeft()
    {
        if (_downLeft)
        {
            camController.transform.position += Vector3.down + Vector3.left;
        }
    }
    private void MoveUpRight()
    {
        if (_upRight)
        {
            camController.transform.position += Vector3.up + Vector3.right;
        }
    }
    private void MoveUpLeft()
    {
        if (_upLeft)
        {
            camController.transform.position += Vector3.up + Vector3.left;
        }
    }
    private void MoveRight()
    {
        if (_right)
        {
            camController.transform.position += Vector3.right;
        }
    }
    private void MoveLeft()
    {
        if (_left)
        {
            camController.transform.position += Vector3.left;
        }
    }
    private void MoveDown()
    {
        if (_down)
        {
            camController.transform.position += Vector3.down;
        }
    }
    private void MoveUp()
    {
        if (_up)
        {
            camController.transform.position += Vector3.up;
        }
    }

}




