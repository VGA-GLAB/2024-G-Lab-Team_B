using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InputReceiver : MonoBehaviour
{
    private WheelInput _wheelInput;
    private Button[] _buttons;
    private int _currentIndex;

    private void Start()
    {
        _wheelInput = new WheelInput();
        _wheelInput.RegisterWheelUpEvent(Demo);
        _wheelInput.RegisterWheelDownEvent(Demo);
        
        //初期状態でボタンを取得
        UpdateButtons();
    }
    
    private void Update()
    {
        _wheelInput.OnUpdate();
        
        //左クリックでボタンをクリック
        if (Input.GetMouseButtonDown(0) && _buttons is { Length: > 0 })
        {
            _buttons[_currentIndex].onClick.Invoke();
        }
    }
    
    private void OnDestroy()
    {
        _wheelInput.OnDestroy();
    }

    /// <summary> サンプルの関数（配列のインデックス変更） </summary>
    private void Demo(int value)
    {
        if (_buttons == null || _buttons.Length == 0) return;

        // ボタンの色を白にする
        _buttons[_currentIndex].image.color = Color.white;

        // インデックスを更新
        _currentIndex = (_currentIndex + value + _buttons.Length) % _buttons.Length;

        // 新しいボタンを選択
        SelectButton(_currentIndex);
    }
    
    //選択中のボタンかどうかを視覚で判別可能にする
    private void SelectButton(int index)
    {
        if (_buttons == null || _buttons.Length == 0) return;

        // ボタンを選択状態にする
        _buttons[index].Select();
        // 選択中のボタンを目立たせるために色を変更
        _buttons[index].image.color = Color.cyan;
    }
    
    /// <summary>ボタンの状態を変更</summary>
    public void UpdateButtons()
    {
        // アクティブなボタンのみを取得
        _buttons = FindObjectsOfType<Button>().Where(b => b.gameObject.activeInHierarchy).ToArray();

        if (_buttons.Length <= 0) return;
        _currentIndex = 0;
        SelectButton(_currentIndex);
    }
}
