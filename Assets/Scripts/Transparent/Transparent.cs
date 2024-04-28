using System;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;
using UniRx;
using static CriAudioManager;

/// <summary>
/// プレイヤーの透明度を変更する
/// レイヤーも変更、影のOnOff
/// </summary>
public class Transparent : MonoBehaviour
{
    #region 変数

    [Header("透明化する対象")] [Tooltip("透明化する対象")]
    [SerializeField] private GameObject _target = default;

    private Renderer[] _renderers = default;

    [Header("透明度")] [Tooltip("透明度")]
    [SerializeField] private float _value = default;

    [Header("透明化にかける時間")] [Tooltip("透明化にかける時間")]
    [SerializeField] private float _duration = default;

    [Header("透明にするか")] [Tooltip("透明にするか")]
    [SerializeField] private bool _isTransparent = default;

    [Header("レイヤーの名前：TransparentPlayer")] [Tooltip("レイヤーの名前：TransparentPlayer")]
    [SerializeField] private string _layerName = "TransparentPlayer";

    private string _defaultLayerName = default;

    [Header("キー入力ができるか")] [Tooltip("キー入力ができるか")]
    [SerializeField] private bool _canInput = default;

    [SerializeField]
    private float _timeLimit = 10f;

    private float _timer = 0f;

    private Subject<Unit> _limit = new();
    #endregion

    /// <summary> キー入力ができるか </summary>
    public bool CanInput
    {
        get => _canInput;
        set => _canInput = value;
    }
    
    private void Start()
    {
        _renderers = _target.GetComponentsInChildren<Renderer>();
        _defaultLayerName = LayerMask.LayerToName(_target.gameObject.layer);

        _limit.FirstOrDefault().Subscribe(_ => 
        {
            ChangeAlpha(false);
            CriAudioManager.Instance.PlaySE(CueSheetType.SE, "SE_Ability_Cancellation_01");
        }).AddTo(this);
    }

    private void Update()
    {
        if (_isTransparent)
        {
            _timer += Time.deltaTime;
        }

        if (_timer <= _timeLimit)
        {
            if(Input.GetButtonDown("Fire2") && CanInput)
            {
                OnClick();
            }
        }
        else
        {
            _limit.OnNext(Unit.Default);//タイムリミットにより強制解除
        }
    }


    /// <summary>
    /// 呼ぶたびにフラグが切り替わる
    /// </summary>
    /// <param name="isFlag"></param>
    public void OnClick()
    {
        _isTransparent = !_isTransparent;
        ChangeAlpha(_isTransparent);

        if(_isTransparent)
        {
            CriAudioManager.Instance.PlaySE(CueSheetType.SE, "SE_Ability_Use_03");
        }
        else
        {
            CriAudioManager.Instance.PlaySE(CueSheetType.SE, "SE_Ability_Cancellation_01");
        }
    }

    /// <summary>
    /// テスト用
    /// </summary>
    public void ToFalse()
    {
        _isTransparent = false;
        ChangeAlpha(_isTransparent);
    }

    /// <summary>
    /// テスト用
    /// </summary>
    public void ToTrue()
    {
        _isTransparent = true;
        ChangeAlpha(_isTransparent);
    }

    /// <summary>
    /// 透明度を変更する
    /// 影のOnOffも切り替える
    /// RenderType：Transparent ⇔ Opaque
    /// ShadowCastingMode：On ⇔ Off
    /// </summary>
    /// <param name="isFlag"></param>
    public void ChangeAlpha(bool isFlag)
    {
        DOTween.KillAll();
        for (int i = 0; i < _renderers.Length; i++)
        {
            for (int j = 0; j < _renderers[i].materials.Length; j++)
            {
                Renderer renderer = _renderers[i];
                Material material = _renderers[i].material;
                if (isFlag)
                {
                    _target.gameObject.layer = LayerMask.NameToLayer(_layerName);

                    // 透明にする処理
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetFloat("_Surface", 1);
                    material.renderQueue = (int)RenderQueue.Transparent;

                    material.SetFloat("_SrcBlend", (int)BlendMode.SrcAlpha);
                    material.SetFloat("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
                    material.SetFloat("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    renderer.shadowCastingMode = ShadowCastingMode.Off;

                    material.DOFade(_value, _duration);
                }
                else
                {
                    // 不透明にする処理
                    material.DOFade(1.0f, _duration).OnComplete(() =>
                    {
                        material.SetFloat("_SrcBlend", (int)BlendMode.One);
                        material.SetFloat("_DstBlend", (int)BlendMode.Zero);
                        material.SetFloat("_ZWrite", 1);
                        material.DisableKeyword("_ALPHATEST_ON");
                        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");

                        material.SetOverrideTag("RenderType", "Opaque");
                        material.SetFloat("_Surface", 0);
                        material.renderQueue = (int)RenderQueue.Geometry;

                        renderer.shadowCastingMode = ShadowCastingMode.On;
                        _target.gameObject.layer = LayerMask.NameToLayer(_defaultLayerName);
                    });
                }
            }
        }
    }
}