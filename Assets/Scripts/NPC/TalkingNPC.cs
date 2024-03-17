using UnityEngine;

/// <summary>
/// 話す動きをするモブNPC
/// 03.12：ずっと喋っている状態
/// </summary>
public class TalkingNPC : NPC
{
    #region 変数

    [Tooltip("会話ステート")] TalkingState _talkingState = default;

    #endregion

    protected override void OnStart()
    {
        _talkingState = new TalkingState(this);
        NpcStateMachine.ChangeState(_talkingState);
    }

    protected override void OnUpdate()
    {
        ToDefaultState(_talkingState);
    }


    #region ステート機能

    /// <summary>
    /// 会話ステート
    /// </summary>
    public class TalkingState : StateBase
    {
        TalkingNPC talkingNPC;

        public TalkingState(TalkingNPC owner) : base(owner)
        {
            talkingNPC = owner;
        }

        public override void Enter()
        {
            // TODO：話しているしぐさのアニメーション再生

            //Debug.Log("Enter: Talking state");
        }

        public override void Update()
        {
            Debug.Log("Update: Talking state");
        }

        public override void Exit()
        {
            //Debug.Log("Exit: Talking state");
        }
    }

    #endregion
}