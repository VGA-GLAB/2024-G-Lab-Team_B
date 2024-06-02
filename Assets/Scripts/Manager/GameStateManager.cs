using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>ゲームの状況を管理します</summary>
public class GameStateManager : MonoBehaviour
{
    [SerializeField, Header("事件番号")] private int _caseNumber;
    [SerializeField, Header("事件解決フラグ")] private int _flagCount = 3;

    private SceneChangeUtility _sceneChangeUtility;
    private CancellationTokenSource _cancellationTokenSource;
    private static bool[] _caseFlags;

    /// <summary>事件番号を取得します</summary>
    public int CaseNumber => _caseNumber;

    /// <summary>事件番号をセットします</summary>
    public void SetCaseNumber(int number) => _caseNumber = number;

    /// <summary>初期化を行います</summary>
    public void Initialize()
    {
        _caseFlags = new bool[_flagCount];
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

    public void FlagFire(int flagNumber)
    {
        _caseFlags[flagNumber] = true;

#if UNITY_EDITOR
        Debug.Log($"事件{flagNumber}解決");
#endif
    }
}