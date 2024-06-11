using UnityEngine;

/// <summary>
/// 話す動きをするモブNPC
/// ずっと喋っているだけ
/// </summary>
public class TalkingNPC : NPC
{
    #region 変数

    [Tooltip("会話ステート")] private TalkingState _talkingState = default;

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

}

#region ステート機能

/// <summary>
/// 会話ステート
/// </summary>
public class TalkingState : StateBase
{
    TalkingNPC _talkingNPC;

    public TalkingState(TalkingNPC owner) : base(owner)
    {
        _talkingNPC = owner;
    }

    public override void Enter()
    {
        // TODO：話しているしぐさのアニメーション再生
        if (_npc.Anim)
        {
            _npc.Anim.SetBool("Talk", true);
            _npc.Anim.SetBool(_npc.IsStand ? "Stand" : "Sit", true); // 立っている : 座っている
        }
        else
        {
            Debug.LogWarning("アニメーターが設定されていません");
        }
        //Debug.Log("Enter: Talking state");
    }

    public override void Update()
    {
        // Debug.Log("Update: Talking state");
    }

    public override void Exit()
    {
        if (_npc.Anim)
        {
            _npc.Anim.SetBool("Talk", false);
        }
        else
        {
            Debug.LogWarning("アニメーターが設定されていません");
        }
        //Debug.Log("Exit: Talking state");
    }
}

#endregion
