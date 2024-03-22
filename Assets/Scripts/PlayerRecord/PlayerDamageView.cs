using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerDamageView : MonoBehaviour
{
    [SerializeField, Header("ダメージ用のパネル")]
    private Image _damagePanel;

    [SerializeField, Header("フェードにかかる時間")]
    private float _fadeTIme = 0.5f;

    /// <summary>
    /// パネルの透明度を更新する
    /// </summary>
    /// <param name="value"></param>
    public void UpdatePanelAlpha(float value)
    {
        _damagePanel.DOFade(value / 100, _fadeTIme);
    }
}
