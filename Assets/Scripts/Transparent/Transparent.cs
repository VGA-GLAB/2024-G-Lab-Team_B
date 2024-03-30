using System;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;
using Cysharp.Threading;
using Cysharp.Threading.Tasks;
using System.Threading;

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

    [SerializeField, Header("ダメージのインターバル")]
    private float _damageInterval = 1f;

    [SerializeField, Header("ダメージ数")]
    private int _damageDealt = 5;

    private IDamage _damage;
    private bool _isDamageInterval = false;
    private CancellationToken _token;
    
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
        _damage = _target.GetComponent<IDamage>();
        _defaultLayerName = LayerMask.LayerToName(_target.gameObject.layer);
        _token = this.GetCancellationTokenOnDestroy();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire2") && CanInput)
        {
            OnClick();

            if(_isTransparent)
            {
                _damage.SendDamage(_damageDealt);//初回ダメージ
            }
        }

        if(_isTransparent && !_isDamageInterval)
        {
            AbilityDamage().Forget();
        }
    }

    /// <summary>
    /// 能力使用時一定間隔でダメージをくらう
    /// </summary>
    private async UniTask AbilityDamage()
    {
        _isDamageInterval = true;
        await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken : _token);
        _damage.SendDamage(_damageDealt);
        _isDamageInterval = false;
    }

    /// <summary>
    /// 呼ぶたびにフラグが切り替わる
    /// </summary>
    /// <param name="isFlag"></param>
    public void OnClick()
    {
        _isTransparent = !_isTransparent;
        ChangeAlpha(_isTransparent);
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