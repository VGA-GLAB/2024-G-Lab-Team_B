using GameAutomaton;
using GameEvent.EmeritusProfessorEvent;
using UnityEngine;

// 日本語対応
public class ProfessorController : MonoBehaviour
{
    public bool IsAlive { get; set; } = true;

    private StateMachine<ProfessorController> _stateMachine = null;

    private void Start()
    {
        _stateMachine = new(this);
        _stateMachine.RegisterTransition<StartState, StrollState>((int)ProfessorAction.TakeStroll);
        _stateMachine.RegisterTransition<StrollState, Return2RoomState>((int)ProfessorAction.Return2Room);
        _stateMachine.RegisterTransition<Return2RoomState, TakeMedicineState>((int)ProfessorAction.TakeMedicine);
        _stateMachine.Start<StartState>();
    }

    private void Update()
    {
        _stateMachine.Update();
    }
}
 
public enum ProfessorAction
{
    TakeStroll,
    Return2Room,
    TakeMedicine,
}
