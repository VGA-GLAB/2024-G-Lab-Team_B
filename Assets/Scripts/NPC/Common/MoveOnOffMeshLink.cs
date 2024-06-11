using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// OffMeshLink上で速度が変わらないようにする処理
/// </summary>
public class MoveOnOffMeshLink : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent = default; 
    [Tooltip("始点に到達したか")] private bool _reachStartPos = default;

    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        // リンク上を自動で渡らせない
        if (_navMeshAgent.autoTraverseOffMeshLink) 
            _navMeshAgent.autoTraverseOffMeshLink = false;
    }

    private void Update()
    {
        if (!_navMeshAgent.isOnOffMeshLink) return; // リンク未到達ならリターン
        if (!_navMeshAgent.isStopped) _navMeshAgent.isStopped = true;
        // 始点にたどり着いていない時
        var startPos = _navMeshAgent.currentOffMeshLinkData.startPos;
        if (!_reachStartPos)
        {
            ToStartPos(startPos);
            return;
        }
        // 始点にたどり着いた以降
        var endPos = _navMeshAgent.currentOffMeshLinkData.endPos;
        ToEndPos(endPos);
    }

    /// <summary>
    /// 始点へ向かう機能
    /// （※リンクに乗った瞬間に移動を開始すると、始点から離れた場所から移動が始まるため、
    /// 始点から移動するように修正する必要があった。）
    /// </summary>
    /// <param name="pos"></param>
    private void ToStartPos(Vector3 pos)
    {
        pos.y = transform.position.y;
        transform.position = Vector3.MoveTowards(transform.position, pos, 
            _navMeshAgent.speed * Time.deltaTime);
        float distance = (transform.position - pos).sqrMagnitude;
        if (distance < 0.01)
        {
            _reachStartPos = true;
        }
        Rotate(pos);
    }

    private void ToEndPos(Vector3 pos)
    {
        pos.y = transform.position.y;
        transform.position = Vector3.MoveTowards(transform.position, pos, 
            _navMeshAgent.speed * Time.deltaTime);
        float distance = (transform.position - pos).sqrMagnitude;
        if (distance < 0.01)
        {
            _navMeshAgent.CompleteOffMeshLink();
            _reachStartPos = false;
            _navMeshAgent.isStopped = false;
        }
        Rotate(pos);
    }

    private void Rotate(Vector3 pos)
    {
        var to = pos - transform.position;
        var angle = Vector3.SignedAngle(transform.forward, to, Vector3.up);
        // 角度が5゜を越えていたら
        if (Mathf.Abs(angle) > 5)
        {
            var rotMax = _navMeshAgent.angularSpeed * Time.deltaTime;
            var rot = Mathf.Min(Mathf.Abs(angle), rotMax);
            transform.Rotate(0f, rot * Mathf.Sign(angle), 0f);
        }
    }
}
