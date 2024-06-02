using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>ゲームの状況を管理します</summary>
public class GameStateManager : SingletonMonoBehavior<GameStateManager>
{
    [SerializeField, Header("事件解決フラグ")] private bool[] _caseFlags;
    [SerializeField, Header("事件解決フラグ数")] private int _caseFlagsCount = 3;

    private SceneChangeUtility _sceneChangeUtility;
    private CancellationTokenSource _cancellationTokenSource;
    private int _caseNumber;

    /// <summary>事件番号を取得します</summary>
    public int CaseNumber => _caseNumber;

    /// <summary>事件番号をセットします</summary>
    public void SetCaseNumber(int number) => _caseNumber = number;

    /// <summary>事件解決フラグを取得します</summary>
    public bool[] CaseFlags => _caseFlags;

    /// <summary>事件解決フラグをセットします</summary>
    /// <param name="flagNumber">事件番号</param>
    /// <param name="isSuccess">事件解決の可否</param>
    public void SetCaseFlag(int flagNumber, bool isSuccess) => _caseFlags[flagNumber] = isSuccess;

    /// <summary>初期化を行います</summary>
    public void Initialize()
    {
        _caseFlags = new bool[_caseFlagsCount];
        _sceneChangeUtility = new SceneChangeUtility();
        _cancellationTokenSource = new CancellationTokenSource();
    }

    /// <summary>次の事件に進む時の処理を行います</summary>
    public async UniTaskVoid NextCase()
    {
        _cancellationTokenSource.Cancel();
        await _sceneChangeUtility.SceneChangeAsync("CaseFileScene");
        _cancellationTokenSource = new CancellationTokenSource();

#if UNITY_EDITOR
        Debug.Log($"次の事件へ");
#endif
    }

    /// <summary>リザルトシーンに進むときの処理を行います</summary>
    public async UniTaskVoid Result()
    {
        _cancellationTokenSource.Cancel();
        await _sceneChangeUtility.SceneChangeAsync("ResultScene");
        _cancellationTokenSource = new CancellationTokenSource();

#if UNITY_EDITOR
        Debug.Log($"リザルトシーンへ");
#endif
    }

    /// <summary>事件失敗時の処理を行います</summary>
    public async UniTaskVoid Fail()
    {
        _cancellationTokenSource.Cancel();
        await _sceneChangeUtility.SceneChangeAsync(SceneManager.GetActiveScene().name);
        _cancellationTokenSource = new CancellationTokenSource();

#if UNITY_EDITOR
        Debug.Log($"事件失敗");
#endif
    }
}
