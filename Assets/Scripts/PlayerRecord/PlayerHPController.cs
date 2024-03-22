using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerHPController : MonoBehaviour, IDamage
{
    [SerializeField, Header("最大HP")]
    private int _maxHitPoint = 100;

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

    public void SendDamage(int damage)
    {
        _hitPoint.Value -= damage;
    }
}
