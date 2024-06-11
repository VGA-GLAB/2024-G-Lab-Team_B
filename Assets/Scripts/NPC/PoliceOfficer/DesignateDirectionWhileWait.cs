using UnityEngine;

/// <summary>
/// 待機時に向く方向を指定する。
/// </summary>
public class DesignateDirectionWhileWait : MonoBehaviour
{
    private PoliceOfficer _policeOfficer = default;
    private float _distance = 1f;
    [Header("※以下ふたつの配列の要素数を一致させてください")]
    [Header("向きを指定したい待機地点"), SerializeField] private GameObject[] _targetPoints = default;
    [Header("向き"), SerializeField] private Vector3[] _directions = default;
    [SerializeField] private int _indexNum = default;
    
    void Start()
    {
        _policeOfficer = GetComponent<PoliceOfficer>();
        _indexNum = -1;
        if (_targetPoints.Length != _directions.Length)
            Debug.LogError("「方向指定をする待機地点」と「向き」の要素数が一致していません。");
    }

    void Update()
    {
        if(_policeOfficer.Anim.GetFloat("Speed") > 0) return; // 移動していたとき
        for (int i = 0; i < _targetPoints.Length; i++)
        {
            var dis = (_targetPoints[i].transform.position - transform.position).sqrMagnitude;
            if (dis <= _distance)
            {
                _indexNum = i;
                break; // 方向指定 待機場所のとき
            }
        }
        Rotate(_indexNum);
    }

    /// <summary>
    /// 回転
    /// </summary>
    /// <param name="num"></param>
    void Rotate(int num)
    {
        if(num < 0) return;
        var to = _directions[num];
        float angle = Vector3.SignedAngle(transform.forward, to, Vector3.up);
        // 角度が5゜を越えていたら
        if (Mathf.Abs(angle) > 5)
        {
            float rotMax = _policeOfficer.NavMeshAgent.angularSpeed * Time.deltaTime;
            float rot = Mathf.Min(Mathf.Abs(angle), rotMax);
            transform.Rotate(0f, rot * Mathf.Sign(angle), 0f);
        }
    }
    
    private void OnDrawGizmos()
    {
        if(_indexNum < 0) return;
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position + _directions[_indexNum], 0.1f);
        Gizmos.DrawLine(transform.position, transform.position + _directions[_indexNum]);
    }
}
