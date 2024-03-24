// using UnityEngine;
//
// #region ステート機能
//
// /// <summary>
// /// ドアを開けるような動作をするステート
// /// </summary>
// public class DoorOpenState : StateBase
// {
//     private float _timer = default;
//     private PatrolNPC _patrolNpc = default;
//
//     public DoorOpenState(PatrolNPC owner) : base(owner)
//     {
//         _patrolNpc = owner;
//     }
//
//     public override void Enter()
//     {
//         if (_npc.Anim)
//         {
//             _npc.Anim.SetTrigger("Open");
//             _npc.Anim.SetFloat("Speed", 0f);
//             _npc.NavMeshAgent.isStopped = true;
//         }
//         else
//         {
//             Debug.LogWarning("アニメーターが設定されていません");
//         }
//         //Debug.Log("Enter: DoorOpen state");
//     }
//
//     public override void Update()
//     {
//         _timer += Time.deltaTime;
//         if (_timer > _patrolNpc.WaitTime)
//         {
//             Exit();
//         }
//
//         Debug.Log("Update: DoorOpen state");
//     }
//
//     public override void Exit()
//     {
//         _timer = 0f;
//         _patrolNpc.IsTimer = true;
//         if (_npc.Anim)
//         {
//             _npc.Anim.ResetTrigger("Open");
//             _npc.Anim.SetFloat("Speed", _npc.NavMeshAgent.speed);
//         }
//         else
//         {
//             Debug.LogWarning("アニメーターが設定されていません");
//         }
//
//         _npc.NavMeshAgent.isStopped = false;
//         // Debug.Log("Exit: DoorOpen state");
//     }
// }
//
// #endregion