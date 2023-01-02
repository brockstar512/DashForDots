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
    public CinemachineVirtualCamera camController;
    public SwipeListenerEvent SwipeListener;

    BaseState currentState;
    public NeutralState NeutralState;//looking at everythig/reset
    public ExploringState ExploringState;//scrolling around and zooming
    public InspectingState InspectingState;//looking at choice. can scroll
    public QuitState QuitState;
    public DecisionState DecisionState;
    public CanvasGroup currentUI;



    private void Awake()
    {
        HandleDots();
        quitButton.onClick.AddListener(delegate { SwitchState(QuitState); });
        NeutralState.Initialize();
        ExploringState.Initialize();
        InspectingState.Initialize();
        QuitState.Initialize();
        DecisionState.Initialize();
    }

    // Start is called before the first frame update
    void Start()
    {
        currentState = ExploringState;
        currentState.EnterState(this);


        //intialize swipes
        SwipeListener.RemoveAllListeners();
        SwipeListener.AddListener(NeutralState.CameraPanState.OnSwipeHandler);

    }

    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState(this);
    }

    public void SwitchState(BaseState state)
    {
        Debug.Log("Here" + state);
        currentState.LeaveState();

        if(currentState is NeutralState)
        {

        }
        if (currentState is ExploringState)
        {

        }
        if (currentState is InspectingState)
        {

        }
        //currentState = state;
        state.EnterState(this);
    }

    public void Inspect(Transform dot)
    {
        SwitchState(InspectingState);
        InspectingState.View(dot);
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


}





//neutral when you move -> inspecting... when you tap a dot -> confirm/delete
//