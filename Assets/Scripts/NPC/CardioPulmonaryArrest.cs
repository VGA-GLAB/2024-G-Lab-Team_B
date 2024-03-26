/// <summary>
/// 心配停止で死亡する准教授を生成する
/// IsDeadフラグ次第で死亡する
/// 0:階段 1:踊り場 2:倒れる（生成）
/// </summary>
public class CardioPulmonaryArrest : VictimBase
{
    protected override void OnStart()
    {
    }
    protected override void OnUpdate()
    {
        TimeLineMove();
    }

    /// <summary>
    /// タイムラインに合わせて行動する
    /// </summary>
    void TimeLineMove()
    {
        var remainingTime = _countDownTimer.Timer;
        if (remainingTime < _timeline[2])
        {
            // 　３. 倒れると、そのまま息をしなくなる。 AEDのタイミング
            ChangeEnabledToFalse(100);
            if (!_character)
            {
                _character = Instantiate(_deadCharaPrefab, transform.position, transform.rotation);
            }
            if (_character)
            {
                // Todo: _isDeadフラグを適用する場所
                ICanDead iCanDead = _character.transform.GetChild(0).GetComponent<ICanDead>();
                if (iCanDead != _character.transform.GetChild(0).GetComponent<ICanDead>())
                {
                    return;
                }
                iCanDead.IsDead = _isDead;
                gameObject.SetActive(false);
            }
        }
        else if (remainingTime < _timeline[1])
        {
            // 　２. 階段の踊り場で突如、胸に違和感を覚える。（心臓が停止する） 生成
            if (_patrolNpcs[2].enabled != true)
            {
                ChangeEnabledToFalse(2);
                _patrolNpcs[2].enabled = true;
            }
            
            // 死亡する場所に着いたら巡回を停止する
            var distance = (transform.position - _deadPoint.transform.position).sqrMagnitude;
            if (distance <= 0.1f)
            {
                // 巡回を停止し、その場で待機
                _navMeshAgent.isStopped = true;
                _patrolNpcs[2].Anim.SetFloat("Speed", 0);
                ChangeEnabledToFalse(100);
            }
        }
        else if (remainingTime < _timeline[0])
        {
            // １. 准教授は、吹き抜けラウンジから警備員室側の階段に向かって歩く。
            if (_patrolNpcs[1].enabled != true)
            {
                ChangeEnabledToFalse(1);
                _patrolNpcs[1].enabled = true;
            }
        }
    }
}
