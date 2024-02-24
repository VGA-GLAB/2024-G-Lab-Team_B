using UnityEngine;
using UnityEngine.UI;

public class RecordedPlayerSight : MonoBehaviour
{
    [SerializeField, Header("視点方向を示すオブジェクト")]
    Transform lookAtTarget;

    [SerializeField, Header("視野角"), Range(0, 180)]
    float sightAngle;

    [SerializeField, Header("見える距離")] private float _sightDistance = 5f;

    Text _visibleMessage; // 発見テキスト
    Transform _target; // 発見したいオブジェクト
    bool _isVisible = true; // 発見フラグ

    private void Start()
    {
        // 発見テキストの検索
        _visibleMessage = FindFirstObjectByType<Text>();
    }

    void Update()
    {
        if (_isVisible ^ IsVisible())
        {
            _isVisible = !_isVisible;
            _visibleMessage.enabled = _isVisible; // 発見したらメッセージを表示する, 見失ったらメッセージを消す
        } // 論理積（最後のフレームと現フレームで見える/見えないが切り替わった時）
    }

    /// <summary>発見フラグの計算</summary>
    bool IsVisible()
    {
        // 発見したいオブジェクトがない場合はプレイヤーを探して割り当てる
        if (!_target)
        {
            var player = GameObject.FindGameObjectWithTag("Player");

            if (player)
                _target = player.transform;
        }

        Vector3 look = lookAtTarget.position - this.transform.position; // 視点方向ベクトル
        Vector3 target = _target.position - this.transform.position; // ターゲットへのベクトル
        float cosHalfSight = Mathf.Cos(sightAngle / 2 * Mathf.Deg2Rad); // 視野角（の半分）の余弦
        float cosTarget = Vector3.Dot(look, target) / (look.magnitude * target.magnitude); // ターゲットへの角度の余弦
        return cosTarget > cosHalfSight &&
               target.magnitude < _sightDistance; // ターゲットへの角度が視界の角度より小さく、かつ距離が視界より近いなら見えていると判定して true を返す
    }

    void OnDrawGizmos()
    {
        // 視界の範囲（正面及び左右の端）をギズモとして描く
        Vector3 lookAtDirection = (lookAtTarget.position - this.transform.position).normalized; // 正面（正規化）
        Vector3 rightBorder = Quaternion.Euler(0, sightAngle / 2, 0) * lookAtDirection; // 右端（正規化）
        Vector3 leftBorder = Quaternion.Euler(0, -1 * sightAngle / 2, 0) * lookAtDirection; // 左端（正規化）
        Gizmos.color = Color.cyan; // 正面は水色で描く
        Gizmos.DrawRay(this.transform.position, lookAtDirection * _sightDistance);
        Gizmos.color = Color.blue; // 両端は青で描く
        Gizmos.DrawRay(this.transform.position, rightBorder * _sightDistance);
        Gizmos.DrawRay(this.transform.position, leftBorder * _sightDistance);
    }
}
