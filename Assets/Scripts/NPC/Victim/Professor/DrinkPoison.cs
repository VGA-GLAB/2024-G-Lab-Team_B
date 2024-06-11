using UnityEngine;

/// <summary>
/// 指定した残り時間になったときに、特定の行動をする
/// 毒を飲んで死亡するモーションをするオブジェクトを生成する
/// ※飲んでから死ぬまでの流れは、生成したオブジェクトが行う
/// 0:散歩-A 1:散歩-B 2:教授室へ 3:毒接種 (4:死亡)
/// </summary>
public class DrinkPoison : VictimBase
{
    #region 変数
   
    [Header("薬"), Tooltip("薬")]
    [SerializeField] private GameObject _medicine = default;
    private bool _canDrinkMedicine = default;

    #endregion
    
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
    private void TimeLineMove()
    {
        var remainingTime = _countDownTimer.Timer;
        if (remainingTime < _timeline[3])
        {
            // ４. 教授は、教授部屋に常備している薬（降圧薬？胃薬？）を飲む。
            ChangeEnabledToFalse(100);
            if (!_character)
            {
                _character = Instantiate(_deadCharaPrefab, transform.position, transform.rotation);
            }
            
            // Todo: _isDeadフラグを適用する場所
            ICanDead iCanDead = _character.transform.GetChild(0).GetComponent<ICanDead>();
            iCanDead.IsDead = _isDead;
            ProfessorDeadOrAlive professorDeadOrAlive = _character.transform.GetChild(0).GetComponent<ProfessorDeadOrAlive>();
            professorDeadOrAlive.Medicine = _medicine;
            gameObject.SetActive(false);
        }
        else if (remainingTime < _timeline[2])
        {
            // 毒のある場所に着いたら巡回を停止する
            var distance = (transform.position - _deadPoint.transform.position).sqrMagnitude;
            if (distance <= 0.2f)
            {
                // 巡回を停止し、その場で待機
                _navMeshAgent.isStopped = true;
                _patrolNpcs[3].Anim.SetFloat("Speed", 0);
                ChangeEnabledToFalse(100);
                _canDrinkMedicine = true; // 薬のタグ判定を許可
            }
            else
            {
                // ３. 薬を飲むために、教授部屋に戻る。
                if (_patrolNpcs[3].enabled != true)
                {
                    ChangeEnabledToFalse(3);
                    _patrolNpcs[3].enabled = true;
                }
            }
        }
        else if (remainingTime < _timeline[1])
        {
            // ２. 吹き抜けラウンジを歩いたり、他の部屋へ遊びに行く。
            if (_patrolNpcs[2].enabled != true)
            {
                ChangeEnabledToFalse(2);
                _patrolNpcs[2].enabled = true;
            }
        }
        else if (remainingTime < _timeline[0])
        {
            // １. 教授は、教授部屋から外に出て、施設内をお散歩する。
            if (_patrolNpcs[1].enabled != true)
            {
                ChangeEnabledToFalse(1);
                _patrolNpcs[1].enabled = true;
            }
        }
    }
    
    /// <summary>
    /// Poisonタグならば、IsDead
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        if (_canDrinkMedicine)
        {
            if (other.gameObject == _medicine)
            {
                if (other.CompareTag("Poison"))
                {
                    _isDead = true;
                }
                else
                {
                    _isDead = false;
                }
            }
        }
    }
}