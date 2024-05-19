using System;
using UnityEngine;

/// <summary>
/// 透視能力
/// "TransparentObject"タグが付いたオブジェクトのマテリアルをディザ抜きする
/// </summary>
public class XRayVision : MonoBehaviour
{
    #region 変数

    [Header("レイの長さ"), Tooltip("レイの長さ")]
    [SerializeField] private float _maxRayDistance = 1000f;

    [Header("透過率(%)"), Tooltip("透過率(%)")]
    [SerializeField, Range(0, 100)] private int _percentageOfDitherLevel = 70;

    [Header("使用制限時間"), Tooltip("使用制限時間")]
    [SerializeField] private float _timeLimit = 40f;

    [Tooltip("レイを飛ばす始点")]
    private Transform _rayStartPosition = default;

    [Tooltip("能力がセットされているか")]
    private bool _setAvility = default; // set

    [Tooltip("透視するか")]
    private bool _isXRayVision = default; // use

    [Tooltip("現在のシェーダーのパラメーター")]    // 0～16
    private float _ditherLevel = default;

    [Tooltip("タグの名前：TransparentObject")]
    private string _tagName = "TransparentObject";

    [Tooltip("透視するオブジェクト")]
    private GameObject _currentTarget = default;    // 現在のターゲット

    [Tooltip("透視していたオブジェクト")]
    private GameObject _oldTarget = default;        // 古いターゲット

    [Tooltip("現在のマテリアル")]
    private Material _currentMaterial = default;

    [Tooltip("元のマテリアル")]
    private Material _defaultMaterial = default;    // 透視後元に戻す用1(instanse)が入ることはない

    [Tooltip("古いターゲットの元のマテリアル")]
    private Material _oldDefaultMaterial = default; // 透視後元に戻す用2(instanse)が入ることはない

    [Tooltip("インスタンスされた古いターゲットのマテリアル")]
    private Material _oldMaterial = default;        // 透視後削除する用(instanse)が入る)


    // テスト用
    //private LineRenderer _lineRenderer = default;
    //private Vector3 _rayHitPosition;
    #endregion

    /// <summary>
    /// 能力がセットされているか
    /// </summary>
    public bool SetAvility
    {
        get => _setAvility;
        set => _setAvility = value;
    }

    /// <summary>
    /// 能力が使用されているか
    /// </summary>
    public bool IsXRayVision
    {
        get => _isXRayVision;
        set => _isXRayVision = value;
    }

    private void Start()
    {
        _ditherLevel = 16 * _percentageOfDitherLevel * 0.01f;
        _rayStartPosition = Camera.main.transform;
        
        // テスト用：レイの可視化
        //_lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        RaycastGetObject(IsXRayVision);

        // 能力がセットされているとき使用できるようにする
        if (Input.GetButtonDown("Fire2") && SetAvility)
        {
            OnClick();
        }
        

        // テスト用:能力選択(左クリック)
        //if (Input.GetButtonDown("Fire1"))
        //{
        //    SetAvility = !SetAvility;
        //    IsXRayVision = false;
        //}
    }

    /// <summary>
    /// 呼ぶたびにフラグが切り替わる
    /// </summary>
    /// <param name="isFlag"></param>
    private void OnClick()
    {
        IsXRayVision = !IsXRayVision;
        RaycastGetObject(IsXRayVision);
    }

    /// <summary>
    /// 能力を使用しているとき
    /// "TransparentObject"タグのついた壁を透視する
    /// </summary>
    /// <param name="isFlag"></param>
    private void RaycastGetObject(bool isFlag)
    {
        if (isFlag)
        {
            CountDownTimer();
            Ray ray = new Ray(_rayStartPosition.position, this.transform.forward);
            RaycastHit hit;

            // テスト用：レイを前方に最大距離飛ばす
            //_rayHitPosition = _rayStartPosition.position + this.transform.forward * _maxRayDistance;

            if (_currentTarget != null && _currentTarget.CompareTag(_tagName))
            {
                Debug.Log("透視可能な壁");
                _currentMaterial.SetFloat("_DitherLevel", _ditherLevel);
            }

            // 透ける物にレイが当った場合
            if (Physics.Raycast(ray, out hit))
            {
                // ターゲットが変わった場合
                if (_currentTarget != hit.transform.gameObject)
                {
                    SaveMaterial(hit.transform.gameObject.GetComponent<Renderer>().sharedMaterial);
                    _oldTarget = _currentTarget;
                    DestroyMaterial();

                    _currentTarget = hit.transform.gameObject;
                }
            }
            else
            {
                _currentTarget = null;
                _currentMaterial = null;
                Debug.Log("透視できるものがありません");
            }
            _currentMaterial = _currentTarget.GetComponent<Renderer>().material;


            // テスト用：レイが衝突したらそれ以上レイを伸ばさないようにする
            //if (Physics.Raycast(ray, out hit))
            //{
            //    _rayHitPosition = hit.point;
            //}

            //// テスト用：レイを可視化する
            //_lineRenderer.SetPosition(0, _rayStartPosition.position);
            //_lineRenderer.SetPosition(1, _rayHitPosition);
        }
        else
        {
            // 一か所だけ見てその場で能力を解除した場合元に戻す
            if (_currentTarget != null)
            {
                // インスタンス化されたマテリアル→元のマテリアル
                _currentTarget.GetComponent<Renderer>().material = _defaultMaterial;
            }
            _currentTarget = null;
            _currentMaterial = null;
        }
    }

    /// <summary>
    /// 元のマテリアルに戻し、
    /// インスタンスされたマテリアルを破棄する
    /// </summary>
    private void DestroyMaterial()
    {
        if (_oldTarget != null)
        {
            Renderer ren = null;
            ren = _oldTarget.GetComponent<Renderer>();

            // 破棄用
            _oldMaterial = ren.sharedMaterial;
            Destroy(_oldMaterial);

            // 元に戻す用
            ren.material = _oldDefaultMaterial;
            Debug.Log("ターゲット変更。生成したマテリアルを破棄");
        }
    }

    /// <summary>
    /// 元のマテリアルを保存しておく
    /// </summary>
    private void SaveMaterial(Material material)
    {
        // 元のマテリアルが空だったらセットする
        if (_defaultMaterial != material)
        {
            // _oldDefaultMaterial に古いターゲットの元のマテリアルを保存
            // _defaultMaterial に現在のターゲットの元のマテリアルを保存
            _oldDefaultMaterial = _defaultMaterial;
            _defaultMaterial = material;
        }
    }

    /// <summary>
    /// 能力の使用制限時間
    /// </summary>
    private void CountDownTimer()
    {
        _timeLimit -= Time.deltaTime;
        if (_timeLimit <= 0)
        {
            IsXRayVision = false;
            Debug.Log("使用制限時間を超えています");
        }
    }
}
