using GameAutomaton;
using UnityEngine;

namespace GameEvent
{
    namespace EmeritusProfessorEvent
    {
        // 日本語対応
        public class StartState : StateMachine<ProfessorController>.StateBase
        {
            private float _transitionTime = 30f; // 遷移する時間
            private float _timer; // タイマー

            public override void OnEnter()
            {
            }

            public override void OnUpdate()
            {
                if (_timer > _transitionTime)
                {
                    StateMachine.Transition((int)ProfessorAction.TakeStroll);
                }

                _timer += Time.deltaTime;
            }

            public override void OnExit()
            {
            }
        }
    }
}