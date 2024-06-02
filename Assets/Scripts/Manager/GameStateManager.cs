using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>ゲームの状況を管理します</summary>
public class GameStateManager : MonoBehaviour
{
    [SerializeField, Header("事件解決フラグ")] private bool[] _caseFlags;
    [SerializeField, Header("事件解決フラグ数")] private int _caseFlagsCount;

    private SceneChangeUtility _sceneChangeUtility;
    private CancellationTokenSource _cancellationTokenSource;
    private int _caseNumber;

    /// <summary>事件番号を取得します</summary>
    public int CaseNumber => _caseNumber;

    /// <summary>事件番号をセットします</summary>
    public void SetCaseNumber(int number) => _caseNumber = number;

    /// <summary>初期化を行います</summary>
    public void Initialize()
    {
        _caseFlags = new bool[_caseFlagsCount];
        _sceneChangeUtility = new SceneChangeUtility();
        _cancellationTokenSource = new CancellationTokenSource();
    }

    /// <summary>次の事件に進む時の処理を行います</summary>
    public async UniTask NextCase()
    {
        _cancellationTokenSource.Cancel();
        await _sceneChangeUtility.SceneChangeAsync("CaseFileScene");
        _cancellationTokenSource = new CancellationTokenSource();

#if UNITY_EDITOR
        Debug.Log($"次の事件へ");
#endif
    }

    /// <summary>リザルトシーンに進むときの処理を行います</summary>
    public async UniTask Result()
    {
        _cancellationTokenSource.Cancel();
        await _sceneChangeUtility.SceneChangeAsync("ResultScene");
        _cancellationTokenSource = new CancellationTokenSource();

#if UNITY_EDITOR
        Debug.Log($"リザルトシーンへ");
#endif
    }

    /// <summary>事件失敗時の処理を行います</summary>
    public async UniTask Fail()
    {
        _cancellationTokenSource.Cancel();
        await _sceneChangeUtility.SceneChangeAsync(SceneManager.GetActiveScene().name);
        _cancellationTokenSource = new CancellationTokenSource();

#if UNITY_EDITOR
        Debug.Log($"事件失敗");
#endif
    }

    /// <summary>事件の成功フラグをセットします</summary>
    /// <param name="flagNumber">事件番号</param>
    /// <param name="isSuccess">事件解決の可否</param>
    public void SetCaseFlag(int flagNumber, bool isSuccess)
    {
        _caseFlags[flagNumber] = isSuccess;
    }
}
