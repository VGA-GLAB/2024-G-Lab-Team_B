using UnityEngine;
using UnityEngine.UI;

/// <summary>過去のプレイヤーの視界の処理を行います</summary>
public class RecordedPlayerSight : MonoBehaviour
{
    [SerializeField, Header("視界判定用のカメラ")]
    private Camera _sightCamera;

    [SerializeField, Header("見える距離")]
    private float _sightDistance = 10f;

    [SerializeField, Header("視界判定用のRayCastが当たるレイヤー")]
    private LayerMask _targetLayerMask;

    [SerializeField, Header("発見時のダメージ数")]
    private int _damageDealt = 20;

    [SerializeField, Header("ダメージのインターバル")]
    private float _damageInterval = 1f;

    private Text _visibleMessage; // 発見テキスト
    private GameObject _player; // 発見したいオブジェクト
    private bool _isVisible = true; // 発見フラグ
    private float _visiblTime = 0f; // 見つけている時間
    private bool _isDamage = false;

    private void Start()
    {
        //TODO:違うテキストが呼ばれることがある
        //TODO:複数人分の判定の表示に対応しないといけない
        // 発見テキストの検索
        _visibleMessage = FindFirstObjectByType<Text>();
    }

    private void Update()
    {
        FindPlyer();
    }

    /// <summary>
    /// Playerを発見したときにダメージを与える
    /// </summary>
    private void FindPlyer()
    {
        if (IsVisible() && _player.TryGetComponent<IDamage>(out IDamage damage))
        {
            _visiblTime += Time.deltaTime;

            if (!_isDamage)//初回ダメージ
            {
                damage.SendDamage(_damageDealt);
                _isDamage = true;
            }

            if (_visiblTime >= _damageInterval)
            {
                damage.SendDamage(_damageDealt);
                _visiblTime = 0f;
            }
            //_isVisible = !_isVisible;
            //_visibleMessage.enabled = _isVisible; // 発見したらメッセージを表示する, 見失ったらメッセージを消す
        } // 論理積（最後のフレームと現フレームで見える/見えないが切り替わった時）
        else
        {
            _isDamage = false;
            _visiblTime = 0f;
        }
    }

    /// <summary>プレイヤー発見の判定を行います</summary>
    private bool IsVisible()
    {
        // プレイヤーを探して割り当てる
        if (!_player)
        {
            _player = GameObject.FindGameObjectWithTag("Player");
        }

        // カメラから視錘台の形の範囲を取得します
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(_sightCamera);

        // 視錘台の中にプレイヤーのコライダーが在れば
        if (GeometryUtility.TestPlanesAABB(planes, _player.GetComponent<Collider>().bounds))
        {
            Vector3 dir = _player.transform.position - _sightCamera.transform.position; // プレイヤーの方向
            Ray ray = new Ray(_sightCamera.transform.position, dir.normalized); // 自身からプレイヤーのレイ
            RaycastHit hit;// レイの当たり判定

            // レイを飛ばす
            if (Physics.Raycast(ray, out hit, _sightDistance, _targetLayerMask))
            {
                if (hit.collider.CompareTag("Player"))
                {
#if UNITY_EDITOR
                    Debug.DrawRay(_sightCamera.transform.position, hit.point - _sightCamera.transform.position, Color.green);
#endif

                    return true;
                }
                else
                {
#if UNITY_EDITOR
                    Debug.DrawRay(_sightCamera.transform.position, hit.point - _sightCamera.transform.position, Color.red);
#endif

                    return false;
                }
            }
        }

        return false;
    }
}
