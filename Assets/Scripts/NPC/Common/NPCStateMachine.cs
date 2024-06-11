using UnityEngine;

/// <summary>
/// NPCのステートマシン
/// </summary>
public class NPCStateMachine
{
    private StateBase _currentState;

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
    [Tooltip("このステートマシンを使用するインスタンスを保持")] protected PoliceOfficer _policeOfficer;
    [Tooltip("このステートマシンを使用するインスタンスを保持")] protected CleaningRobot _cleaningRobot;

    protected StateBase(NPC npc)
    {
        this._npc = npc;
    }

    protected StateBase(PoliceOfficer policeOfficer)
    {
        this._policeOfficer = policeOfficer;
    }

    protected StateBase(CleaningRobot cleaningRobot)
    {
        this._cleaningRobot = cleaningRobot;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}