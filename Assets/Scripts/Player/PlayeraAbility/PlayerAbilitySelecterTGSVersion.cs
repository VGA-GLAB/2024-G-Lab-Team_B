using UnityEngine;
using UniRx;
using static CriAudioManager;

/// <summary>プレイヤーの能力の切り替えを行います(TGS版)</summary>
public class PlayerAbilitySelecterTGSVersion : MonoBehaviour
{
    private const int ABILITIES_COUNT = 2; // 能力数

    [SerializeField, Header("能力のUI\n0 : カメラ切り替え\n1 : 透明化\n2 : 透明化")]
    private CanvasGroup[] _abilitiesUI;

    [SerializeField, Header("能力のUIのCanvasGroup")]
    private CanvasGroup _canvasGroup;

    private CameraSwitcher _cameraSwitcher;
    private Transparent _transparent;
    private XRayVision _xRayVision;
    private ReactiveProperty<int> _currentIndex = new(); // 現在の能力の番号
    private bool _isScroll;
    public bool IsScroll => _isScroll;

    public IReactiveProperty<int> CurrentIndex => _currentIndex;

    private void Start()
    {
        _cameraSwitcher = FindFirstObjectByType<CameraSwitcher>();
        _transparent = FindFirstObjectByType<Transparent>();
        _xRayVision = FindFirstObjectByType<XRayVision>();

        if (_cameraSwitcher == null || _transparent == null || _xRayVision == null)
        {
#if UNITY_EDITOR
            Debug.LogError("アタッチされていない能力があります。");
#endif
            return;
        }

        UIDisplayChange();
        _currentIndex.Skip(1).Subscribe(_ =>
        {
            CriAudioManager.Instance.PlaySE(CueSheetType.SE, "SE_Ability_Change_01");

            if (_currentIndex.Value < 0) { _currentIndex.Value = ABILITIES_COUNT; }
            else if (_currentIndex.Value > ABILITIES_COUNT) { _currentIndex.Value = 0; }
        }).AddTo(this);
    }

    private void Update()
    {
        if (_cameraSwitcher == null || _transparent == null || _xRayVision == null)
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
            _currentIndex.Value++;
            _transparent.SEStop();      //Transparentクラスで再生されているSEを止める
            _cameraSwitcher.ChangeFirstPerson();        //視点を一人称にしてSEを止める
            _isScroll = true;
        }
        else if (scroll < 0)
        {
            _currentIndex.Value--;
            _transparent.SEStop();
            _cameraSwitcher.ChangeFirstPerson();        //視点を一人称にしてSEを止める
            _isScroll = true;
        }
        else if (scroll == 0)
        {
            _isScroll = false;
        }
        
        switch (_currentIndex.Value)
        {
            case 0: // カメラ切り替え

                _cameraSwitcher.enabled = true;
                NullificationTransparent();
                NullificationClairvoyance();
                UIDisplayChange();

                break;
            case 1: // 透明化
                _transparent.CanInput = true;
                NullificationCameraSwitcher();
                NullificationClairvoyance();
                UIDisplayChange();
                break;
            case 2: // 透視
                _xRayVision.SetAvility = true;
                NullificationCameraSwitcher();
                NullificationTransparent();
                UIDisplayChange();
                break;
        }
    }

    /// <summary>CameraSwitcherの無効化</summary>
    private void NullificationCameraSwitcher()
    {
        _cameraSwitcher.IsFirstPerson = true;
        _cameraSwitcher.FirstPerson.Priority = 1;
        _cameraSwitcher.ThirdPerson.Priority = 0;
        _cameraSwitcher.enabled = false;
    }

    /// <summary>Transparentの無効化</summary>
    private void NullificationTransparent()
    {
        _transparent.CanInput = false;
        _transparent.ToFalse();
    }

    /// <summary>透視の無効化</summary>
    private void NullificationClairvoyance()
    {
        _xRayVision.SetAvility = false;
        _xRayVision.IsXRayVision = false;
    }

    /// <summary>能力のUIの色の変更を行います</summary>
    private void UIDisplayChange()
    {
        for (int i = 0; i < _abilitiesUI.Length; i++)
        {
            if (i == _currentIndex.Value)
            {
                // 選択している能力を表示
                _abilitiesUI[i].alpha = 1;
            }
            else
            {
                // それ以外は非表示
                _abilitiesUI[i].alpha = 0;
            }
        }
    }
}
