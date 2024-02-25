using UnityEngine;

/// <summary>
/// NPCのステートマシン
/// </summary>
public class NPCStateMachine
{
    StateBase _currentState;

    public void ChangeState(StateBase newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
        //Debug.LogWarning(newState);
    }

    public void Update()
    {
        _currentState?.Update();
    }
}

/// <summary>
/// ステートの基底クラス
/// </summary>
public abstract class StateBase
{
    [Tooltip("このステートマシンを使用するインスタンスを保持")] protected NPC _npc;

    public StateBase(NPC npc)
    {
        this._npc = npc;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}