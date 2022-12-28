using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class StateManager : MonoBehaviour
{
    [SerializeField] Transform dotsParent;
    [SerializeField] Button quitButton;

    BaseState currentState;
    public NeutralState NeutralState;
    public ExploringState ExploringState;
    public InspectingState InspectingState;
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
        currentState = NeutralState;
        currentState.EnterState(this);
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