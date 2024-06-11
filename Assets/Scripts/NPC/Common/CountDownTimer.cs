using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 被害者がタイムラインに合わせて行動するための、残り時間を計算する
/// </summary>
public class CountDownTimer : MonoBehaviour
{
    [Header("残り時間")] [Tooltip("残り時間")]
    [SerializeField] private float _timer = 120f;
    [Header("テキスト表示するならアサインしてください。")]
    [SerializeField] private Text _text = default;

    /// <summary> 残り時間 </summary>
    public float Timer => _timer;

    private void Update()
    {
        if (_timer < 0)
        {
            return;
        }
        _timer -= Time.deltaTime;
        if (_text)
        {
            _text.text = _timer.ToString("0.0");
        }
    }
}