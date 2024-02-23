using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public abstract class StateManager<EState> : MonoBehaviour where EState : Enum
{
    public Dictionary<EState, BaseState<EState>> States = new Dictionary<EState, BaseState<EState>>();
    protected ReactiveProperty<BaseState<EState>> currState = new ReactiveProperty<BaseState<EState>>();
    protected BaseState<EState> lastState;

    protected CompositeDisposable disposables = new CompositeDisposable();

    protected void Init(BaseState<EState> startState)
    {
        currState.Value = startState;
        lastState = currState.Value;

        currState.Where(newState => newState != lastState)
                 .Subscribe(newState =>
                  {
                      lastState.OnExit();
                      lastState = newState;
                      newState.OnEnter();
                  })
                 .AddTo(disposables);
    }
    
    private void Update()
    {
        currState.Value.OnUpdate();
    }

    public EState CurrState()
    {
        return currState.Value.GetState();
    }
    public void SetState(EState newState)
    {
        currState.Value = States[newState];
    }
}
public abstract class BaseState<EState> where EState : Enum
{
    protected EState stateId;
    public delegate void Publisher();

    public event Publisher OnEnterEvent;
    public event Publisher OnUpdateEvent;
    public event Publisher OnExitEvent;

    public void OnEnter() => OnEnterEvent?.Invoke();
    public void OnUpdate() => OnUpdateEvent?.Invoke();
    public void OnExit() => OnExitEvent?.Invoke();
    public EState GetState() => stateId;
}
