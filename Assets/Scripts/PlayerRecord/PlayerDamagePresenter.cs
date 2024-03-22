using UnityEngine;
using UniRx;

public class PlayerDamagePresenter : MonoBehaviour
{
    [SerializeField]
    private PlayerHPController _playerHP;

    [SerializeField]
    private PlayerDamageView _damageView;

    private void Start()
    {
        _playerHP.HitPoint.Skip(1).Subscribe(damage => _damageView.UpdatePanelAlpha(_playerHP.MaxHitPoint - damage)).AddTo(this);
    }
}
