using UnityEngine;

/// <summary>
/// 固定型警備員の機能
/// プレイヤーがカードキーを入手後に通行止めをやめる。
/// 入手されていない間は不動。
/// </summary>
public class ClosureRoad : MonoBehaviour
{
    [Header("通行止めをするオブジェクト"), Tooltip("通行止めをするオブジェクト")]
    [SerializeField] private GameObject[] _objects = default;
    [Header("カードキー入手後、移動を開始するまでの時間"), Tooltip("カードキー入手後、移動を開始するまでの時間")]
    [SerializeField] private float _startTimeForMove = 5f;
    [Tooltip("カードキーオブジェクト"), SerializeField] private GameObject _cardkey = default;
    private float _timer = default;
    private PoliceOfficer _policeOfficer = default;
    private bool _isClosure = default;
    private Animator _animator = default;
    
    private void Start()
    {
        if(_objects.Length == 0) 
            Debug.LogWarning("「通行止めをするオブジェクト」を指定してください");
        if(_cardkey == null) 
            Debug.LogWarning("「Cardkey」オブジェクトがアサインされていません。");
        _policeOfficer = GetComponent<PoliceOfficer>();
        _animator = GetComponent<Animator>();
        _isClosure = true;
        _policeOfficer.IsTimer = true;
    }

    private void Update()
    {
        if(!_isClosure) return; // 行き止まり終了のとき
        if (_cardkey.activeSelf)
        {
            if (_policeOfficer.enabled)
            {
                _policeOfficer.enabled = false;
                _animator.SetFloat("Speed", 0);
            }
            return; // カードキー未入手のとき
        }
        if (_timer > _startTimeForMove)
        {
            _policeOfficer.enabled = true;
            EraseObject();
            _isClosure = false;
            return; // 移動開始時間を過ぎたとき
        }
        _timer += Time.deltaTime;
    }

    /// <summary>
    /// 通行止めしていたオブジェクトを非アクティブにする。
    /// </summary>
    private void EraseObject()
    {
        foreach (var obj in _objects)
        {
            obj.SetActive(false);
        }
    }
}
