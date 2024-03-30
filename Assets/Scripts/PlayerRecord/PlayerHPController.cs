using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerHPController : MonoBehaviour, IDamage
{
    [SerializeField, Header("最大HP")]
    private int _maxHitPoint = 100;


    [SerializeField, Header("自然回復する値")]
    private int _healHP = 1;

    [SerializeField, Header("自然回復のインターバル")]
    private float _healInterval = 1f;

    private float _healTime = 0f;
    private ReactiveProperty<int> _hitPoint = new();

    public int MaxHitPoint => _maxHitPoint;

    public IReactiveProperty<int> HitPoint => _hitPoint;

    private void Awake()
    {
        _hitPoint.Value = _maxHitPoint;
    }

    private void Start()
    {
        //HPが0以下の時1度だけ呼ばれる
        _hitPoint.FirstOrDefault(x => x <= 0).Subscribe(_ => Debug.Log("死")).AddTo(this);
    }

    private void Update()
    {
        HealPlayer();
    }

    /// <summary>
    /// 一定間隔で回復する
    /// </summary>
    private void HealPlayer()
    {
        _healTime += Time.deltaTime;
        if (_healTime >= _healInterval && _hitPoint.Value < _maxHitPoint)
        {
            _hitPoint.Value += _healHP;
            _healTime = 0;
        }
    }

    public void SendDamage(int damage)
    {
        _hitPoint.Value -= damage;
    }
}
