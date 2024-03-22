using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;

/// <summary>
/// プレイヤーの透明度を変更する
/// レイヤーも変更、影のOnOff
/// </summary>
public class Transparent : MonoBehaviour
{
    [Header("透明化する対象")] [Tooltip("透明化する対象")]
    [SerializeField] private GameObject _target = default;

    private Renderer[] _renderers = default;

    [Header("透明度")] [Tooltip("透明度")]
    [SerializeField] private float _value = default;

    [Header("透明化にかける時間")] [Tooltip("透明化にかける時間")]
    [SerializeField] private float _duration = default;

    [Header("透明にするか")] [Tooltip("透明にするか")]
    [SerializeField] private bool _isTransparent = default;

    [Header("切り替え可能か")] [Tooltip("切り替え可能か")]
    [SerializeField] private bool _isCanChangeFlag = default;

    [Header("レイヤーの名前：TransparentPlayer")] [Tooltip("レイヤーの名前：TransparentPlayer")]
    [SerializeField] private string _layerName = "TransparentPlayer";

    [Header("テスト中か")] [Tooltip("テスト中か")]
    [SerializeField] private bool _isTest = default;

    private string _defaultLayerName = default;

    void Start()
    {
        _renderers = _target.GetComponentsInChildren<Renderer>();
        _defaultLayerName = LayerMask.LayerToName(gameObject.layer);
        _isCanChangeFlag = true;
    }

    void Update()
    {
        // テスト
        if (_isTest && Input.GetKeyDown(KeyCode.J))
        {
            OnClick();
        }
    }

    /// <summary>
    /// 呼ぶたびに透明化を切り替える
    /// </summary>
    public void OnClick()
    {
        ChangeFlag(_isTransparent);
        ChangeAlpha(_isTransparent);
    }

    /// <summary>
    /// 透明度を変更している最中に、入力があっても無視する
    /// </summary>
    /// <param name="isFlag"></param>
    /// <returns></returns>
    private void ChangeFlag(bool isFlag)
    {
        if (_isCanChangeFlag)
        {
            isFlag = !isFlag;
            _isTransparent = isFlag;
        }
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
        for (int i = 0; i < _renderers.Length; i++)
        {
            for (int j = 0; j < _renderers[i].materials.Length; j++)
            {
                Renderer renderer = _renderers[i];
                Material material = _renderers[i].material;
                if (isFlag)
                {
                    gameObject.layer = LayerMask.NameToLayer(_layerName);

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
                    _isCanChangeFlag = false;

                    material.DOFade(_value, _duration).OnComplete(() => { _isCanChangeFlag = true; });
                }
                else
                {
                    _isCanChangeFlag = false;
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
                        _isCanChangeFlag = true;
                        gameObject.layer = LayerMask.NameToLayer(_defaultLayerName);
                    });
                }
            }
        }
    }
}