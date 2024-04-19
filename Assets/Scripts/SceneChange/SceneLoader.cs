using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// シーン遷移を管理するクラス
public class SceneLoader : MonoBehaviour
{

    // --------------------------------------------------
    // 変数宣言

    // フェード用の画像コンポーネント
    // シーン遷移時のフェードイン・アウトに使用します
    [SerializeField]
    [Header("フェード用のImage")] private Image _fadeImage;

    // フェードにかかる時間（秒）
    // フェードイン・アウトの速さを調整します
    [SerializeField]
    [Header("フェードにかかる時間")] private float _fadeDuration = 1f;

    // ローディング画面のUI。
    // 非同期でシーンをロードする際に表示します
    [SerializeField]
    [Header("ローディング画面のUI")] private GameObject _loadingScreen;

    // ローディング進捗を表示するスライダー
    // ロードの進行度を視覚的に示します
    [SerializeField]
    [Header("進捗を表示するスライダー")] private Slider _progressBar;

    // ローディング進捗率を表示するテキスト
    // ロードの進行度を数値で示します
    [SerializeField]
    [Header("進捗率を表示するテキスト")] private Text _progressText;

    // クレジット画面のシーン名(クレジット画面をタイトルとは別途用意する場合)
    // クレジット画面への遷移に使用します
    [SerializeField]
    [Header("クレジットシーン名")] private string _creditsSceneName;

    // クレジット画面のUI(タイトルと共通画面の場合)
    // クレジットボタンを押した際に表示するUI（※UIイラストは一枚と仮定）
    [SerializeField]
    [Header("クレジットシーンUI")] private GameObject _creditsUI;

    // --------------------------------------------------
    // 関数宣言

    // シーンがロードされた時点で自動的にフェードインを行う
    void Start()
    {
        // フェードインを実行
        StartCoroutine(Fade(1f, 0f));
    }

    // -----------------------
    // 外部から呼び出す関数

    // フェードイン・フェードアウトのみでシーン遷移
    // シーンロード中にローディング画面を表示しません
    public void LoadSceneFade(string sceneName)
    {
        StartCoroutine(FadeAndLoadScene(sceneName));
    }

    // フェードイン・アウトと非同期ロードを組み合わせたシーン遷移
    // ローディング画面を表示し、非同期でシーンをロードします
    public void LoadSceneFadeAndAsync(string sceneName)
    {
        StartCoroutine(FadeAndLoadSceneAsync(sceneName));
    }

    // クレジット画面への遷移を処理する関数
    public void LoadCreditsScene()
    {
        // クレジット画面へのフェードイン・アウト遷移を開始
        StartCoroutine(FadeAndLoadScene(_creditsSceneName));
    }

    // クレジットUIを表示する
    public void ShowCredits()
    {
        if (_creditsUI != null)
        {
            // UIをアクティブ化（表示）
            _creditsUI.SetActive(true); 
        }
    }

    // クレジットUIを非表示にする（戻るボタン等で呼び出す）
    public void HideCredits()
    {
        if (_creditsUI != null)
        {
            // UIを非アクティブ化（非表示）
            _creditsUI.SetActive(false); 
        }
    }

    // -----------------------


    // -----------------------
    // 内部で呼び出す関数

    // フェードイン・アウトを行うコルーチン
    IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;
        
        Color color = _fadeImage.color;
        
        while (elapsedTime < _fadeDuration)
        {
            elapsedTime += Time.deltaTime;
        
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / _fadeDuration);
       
            _fadeImage.color = new Color(color.r, color.g, color.b, newAlpha);
            
            yield return null;
        }
    }

    // フェードとシーンロードを組み合わせたコルーチン
    IEnumerator FadeAndLoadScene(string sceneName)
    {
        // 画面を暗くするフェードアウト
        yield return StartCoroutine(Fade(0f, 1f)); 
        // 指定されたシーンをロード
        SceneManager.LoadScene(sceneName); 
    }


    //-----------------------

    // フェード、ローディング画面表示、非同期ロードを組み合わせたコルーチン
    IEnumerator FadeAndLoadSceneAsync(string sceneName)
    {
        // 画面を暗くするフェードアウト
        yield return StartCoroutine(Fade(0f, 1f));
        // ローディング画面を表示
        _loadingScreen.SetActive(true);
        // シーンを非同期でロード
        yield return StartCoroutine(LoadSceneAsyncRoutine(sceneName));
        // ローディング画面を非表示にする
        _loadingScreen.SetActive(false);
    }

    // 非同期でシーンをロードするコルーチン
    // 進捗率を更新しながらロードします
    IEnumerator LoadSceneAsyncRoutine(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        // ロード完了時に自動的にシーンをアクティブにしないようにします
        asyncLoad.allowSceneActivation = false; 

        while (!asyncLoad.isDone)
        {
            // 進捗率を計算
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f); 
            // 進捗率をUIに反映
            // スライダーで進捗を表示
            _progressBar.value = progress; 
            // テキストで進捗率を数値表示
            _progressText.text = (progress * 100f).ToString("0") + "%"; 

            // ロードがほぼ完了したらシーンをアクティブにする
            if (asyncLoad.progress >= 0.9f)
            {
                // ロード完了後にシーンをアクティブ化
                asyncLoad.allowSceneActivation = true;
            }

            // 次のフレームまで待機
            yield return null; 
        }
    }

    // -----------------------

    // --------------------------------------------------
}
