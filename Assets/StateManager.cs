using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public State currentState;

    private void Update()
    {
        RunStateMachine();
    }

    private void RunStateMachine()
    {
        State nextState = currentState?.RunCurrentState(); //if the currentState variable is not null, proceed with RunCurrentState(). If null, just ignore this line
        if(nextState != null)
        {
            
            SwitchState(nextState);
        }
    }

    private void SwitchState(State nextState)
    {
        this.currentState = nextState;
    }
}
