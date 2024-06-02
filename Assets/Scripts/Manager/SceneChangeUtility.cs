using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>シーンの切り替わりを定義します</summary>
public class SceneChangeUtility
{
    /// <summary>シーンが切り替わるときに呼び出します</summary>
    public async UniTask SceneChangeAsync(string sceneName)
    {
        await SceneManager.LoadSceneAsync(sceneName);

#if UNITY_EDITOR
        Debug.Log($"{sceneName}へ遷移");
#endif
    }
}