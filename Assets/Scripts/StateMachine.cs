using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    private IState CurrentlyRunningState;
    private IState PreviousState;
    public void ChangeState(IState newState)
    {
        if (this.CurrentlyRunningState != null)
            this.CurrentlyRunningState.Exit();

        this.PreviousState = this.CurrentlyRunningState;
        this.CurrentlyRunningState = newState;
        this.CurrentlyRunningState.Enter();
    }

    public void ExecuteStateUpdate()
    {
        var runningState = this.CurrentlyRunningState;
        if (runningState != null)
            this.CurrentlyRunningState.Execute();
    }

    public void SwitchToPreviousState()
    {
        this.CurrentlyRunningState.Exit();
        CurrentlyRunningState = this.PreviousState;
        this.CurrentlyRunningState.Enter();
    }
}
