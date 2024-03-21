using UnityEngine;
using UnityEngine.UI;

/// <summary>プレイヤーの能力の切り替えを行います(TGS版)</summary>
public class PlayerAbilitySelecterTGSVersion : MonoBehaviour
{
    private const int ABILITIES_COUNT = 2; // 能力数

    [SerializeField, Header("能力のUI\n0 : カメラ切り替え\n1 : 透明化\n2 : 透明化")]
    private Image[] _abilitiesUI;

    [SerializeField, Header("能力のUIの選択時の色")]
    private Color _selectedColor = Color.blue;

    [SerializeField, Header("能力のUIの非選択時の色")]
    private Color _nonSelectedColor = Color.white;
    
    [SerializeField, Header("能力のUIの透明化に掛ける時間")]
    float _duration = default;
    
    [SerializeField, Header("能力のUIのCanvasGroup")]
    private CanvasGroup _canvasGroup;

    private CameraSwitcher _cameraSwitcher;
    private Transparent _transparent;
    private int _currentIndex; // 現在の能力の番号

    private void Start()
    {
        _cameraSwitcher = FindFirstObjectByType<CameraSwitcher>();
        _transparent = FindFirstObjectByType<Transparent>();
        // 透視 = FindFirstObjectByType<透視>();

        if (_cameraSwitcher == null || _transparent == null /*|| 透視 == null*/)
        {
#if UNITY_EDITOR
            Debug.LogError("アタッチされていない能力があります。");
#endif
            return;
        }

        NullificationCameraSwitcher();
        NullificationTransparent();
        NullificationClairvoyance();
    }

    private void Update()
    {
        if (_cameraSwitcher == null || _transparent == null /*|| 透視 == null*/)
        {
#if UNITY_EDITOR
            Debug.LogError("アタッチされていない能力があります。");
#endif
            return;
        }

        // マウスホイールの回転の取得
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0)
        {
            _currentIndex++;
        }
        else if (scroll < 0)
        {
            _currentIndex--;
        }

        if (_currentIndex > ABILITIES_COUNT)
        {
            _currentIndex = 0;
        }

        if (_currentIndex < 0)
        {
            _currentIndex = ABILITIES_COUNT;
        }

        switch (_currentIndex)
        {
            case 0: // カメラ切り替え
                _cameraSwitcher.enabled = true;
                NullificationTransparent();
                NullificationClairvoyance();
                UIColorChange();
                break;
            case 1: // 透明化
                _transparent.enabled = true;
                NullificationCameraSwitcher();
                NullificationClairvoyance();
                UIColorChange();
                break;
            case 2: // 透視
                Debug.LogWarning("透明化は未実装です。");
                NullificationCameraSwitcher();
                NullificationTransparent();
                UIColorChange();
                break;
        }
    }

    /// <summary>CameraSwitcherの無効化</summary>
    private void NullificationCameraSwitcher()
    {
        _cameraSwitcher.IsFirstPerson = true;
        _cameraSwitcher.FirstPersonPriority = 1;
        _cameraSwitcher.ThirdPersonPriority = 0;
        _cameraSwitcher.enabled = false;
    }

    /// <summary>Transparentの無効化</summary>
    private void NullificationTransparent()
    {
        _transparent.enabled = false;
    }

    /// <summary>透視の無効化</summary>
    private void NullificationClairvoyance()
    {
    }

    /// <summary>能力のUIの色の変更を行います</summary>
    private void UIColorChange()
    {
        for (int i = 0; i < _abilitiesUI.Length; i++)
        {
            if (i == _currentIndex)
            {
                // 選択している能力に対応するUIの色の変更
                _abilitiesUI[i].color = _selectedColor;
            }
            else
            {
                // それ以外は非選択時の色に変更
                _abilitiesUI[i].color = _nonSelectedColor;
            }
        }
    }
}
