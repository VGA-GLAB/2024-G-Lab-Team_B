using UnityEngine;
using UnityEngine.UI;

public class RecordedPlayerSight : MonoBehaviour
{
    [SerializeField, Header("視界判定用のカメラ")]
    private Camera _sightCamera;

    [SerializeField, Header("見える距離")]
    private float _sightDistance = 10f;

    [SerializeField, Header("視界判定用のRayCastが当たるレイヤー")]
    private LayerMask _targetLayerMask;

    private Text _visibleMessage; // 発見テキスト
    private GameObject _player; // 発見したいオブジェクト
    private bool _isVisible = true; // 発見フラグ

    private void Start()
    {
        // 発見テキストの検索
        _visibleMessage = FindFirstObjectByType<Text>();
    }

    private void Update()
    {
        if (_isVisible ^ IsVisible())
        {
            _isVisible = !_isVisible;
            _visibleMessage.enabled = _isVisible; // 発見したらメッセージを表示する, 見失ったらメッセージを消す
        } // 論理積（最後のフレームと現フレームで見える/見えないが切り替わった時）
    }

    /// <summary>プレイヤー発見の判定を行います</summary>
    private bool IsVisible()
    {
        // プレイヤーを探して割り当てる
        if (!_player)
        {
            _player = GameObject.FindGameObjectWithTag("Player");
        }

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(_sightCamera);

        if (GeometryUtility.TestPlanesAABB(planes, _player.GetComponent<Collider>().bounds))
        {
            Vector3 dir = _player.transform.position - transform.position;
            Ray ray = new Ray(this.transform.position, dir.normalized);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, _sightDistance, _targetLayerMask))
            {
                if (hit.collider.CompareTag("Player"))
                {
#if UNITY_EDITOR
                    Debug.DrawRay(this.transform.position, hit.point - this.transform.position, Color.green);
#endif
                    
                    return true;
                }
                else
                {
#if UNITY_EDITOR
                    Debug.DrawRay(this.transform.position, hit.point - this.transform.position, Color.red);
#endif
                    
                    return false;
                }
            }
        }

        return false;
    }
}
