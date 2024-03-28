using UnityEngine;
using UniRx;

public class PlayerHPPresenter : MonoBehaviour
{
    [SerializeField]
    private PlayerHPController _playerHP;

    [SerializeField]
    private PlayerHPView _hpView;

    private void Start()
    {
        //ヒットポイントが変更されたら通知される
        _playerHP.HitPoint.Skip(1).Subscribe(x => _hpView.UpdatePanelAlpha(_playerHP.MaxHitPoint - x)).AddTo(this);
    }
}
