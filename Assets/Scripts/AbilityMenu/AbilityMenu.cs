using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// 右クリックで戻る、左クリックで開く

public class AbilityMenu : MonoBehaviour
{
    [SerializeField]
    [Header("プレイヤーItemController")] private PlayerItemController _playerItemController;

    [SerializeField]
    [Header("最初のメニューの能力ボタン")] private Button _firstMenuSelectAbilityButton;
    [SerializeField]
    [Header("最初のメニューのアイテムボタン")] private Button _firstMenuSelectItemButton;

    [SerializeField]
    [Header("能力のメニューボタン①")] private Button _cameraSwitcherButtona;
    [SerializeField]
    [Header("能力のメニューボタン②")] private Button _transparentButton;
    [SerializeField]
    [Header("能力のメニューボタン③")] private Button _xRayVisionButton;

    [SerializeField]
    [Header("薬のメニューボタン")] private Button _drugButton;
    [SerializeField]
    [Header("カードキーのメニューボタン")] private Button _cardkeyButton;
    [SerializeField]
    [Header("AEDのメニューボタン")] private Button _aedButton;

    // メニューの状態を表す列挙型
    enum MenuState { FirstMenu, AbilityMenu, ItemMenu }
    // 現在のメニューの状態
    private MenuState _currentMenuState;

    private bool _isMenuOpen = false;

    public void Start()
    {
        // ボタンのリスナーを設定
        SetupListeners();
        // 初期状態ですべてのボタンを非表示にする
        ToggleMenu(showFirst: false, showAbilities: false, showItems: false);
    }

    public void Update()
    {
        // 左クリックでメニューを開く
        if (Input.GetMouseButtonDown(0) && !_isMenuOpen)
        {
            OpenMenu();
        }

        // メニューが開いているときのみ以下の操作を許可する
        if (_isMenuOpen)
        {
            // ホイールクリックでメニューを閉じる
            if (Input.GetMouseButtonDown(2))
            {
                CloseMenu();
            }

            // 右クリックで前のメニューに戻るか、メニューを閉じる
            if (Input.GetMouseButtonDown(1))
            {
                if (_currentMenuState == MenuState.FirstMenu)
                {
                    // メニューを閉じる
                    CloseMenu();
                }
                else
                {
                    // 最初のメニューに戻る
                    FirstMenu();
                }
            }
        }
    }

    // ボタンのリスナーを設定
    private void SetupListeners()
    {
        _firstMenuSelectAbilityButton.onClick.AddListener(() => OnMenuButtonClick(MenuState.AbilityMenu));
        _firstMenuSelectItemButton.onClick.AddListener(() => OnMenuButtonClick(MenuState.ItemMenu));
    }

    // メニューのボタンクリック時の処理
    private void OnMenuButtonClick(MenuState newState)
    {
        ChangeMenuState(newState);
    }

    // 最初のメニューを表示
    private void FirstMenu()
    {
        // メニューの表示を切り替え
        ToggleMenu(showFirst: true, showAbilities: false, showItems: false);
        // 現在のメニューの状態を更新
        _currentMenuState = MenuState.FirstMenu;
    }

    // メニューの状態を変更
    private void ChangeMenuState(MenuState newState)
    {
        switch (newState)
        {
            // 能力メニューを表示
            case MenuState.AbilityMenu:
                // メニューの表示を切り替え
                ToggleMenu(showFirst: false, showAbilities: true, showItems: false);
                break;

            // アイテムメニューを表示
            case MenuState.ItemMenu:
                // メニューの表示を切り替え
                ToggleMenu(showFirst: false, showAbilities: false, showItems: true);
                break;
        }

        // 現在のメニューの状態を更新
        _currentMenuState = newState;
    }

    // メニューの表示を切り替え
    private void ToggleMenu(bool showFirst, bool showAbilities, bool showItems)
    {
        _firstMenuSelectAbilityButton.gameObject.SetActive(showFirst);
        _firstMenuSelectItemButton.gameObject.SetActive(showFirst);

        _cameraSwitcherButtona.gameObject.SetActive(showAbilities);
        _transparentButton.gameObject.SetActive(showAbilities);
        _xRayVisionButton.gameObject.SetActive(showAbilities);

        if (showItems)
        {
            // 現在持っているアイテムタイプによって表示するボタンを変更
            var inventory = _playerItemController.Inventory;
            _drugButton.gameObject.SetActive(inventory.Any(item => item.ItemType == ItemType.Drug));
            _cardkeyButton.gameObject.SetActive(inventory.Any(item => item.ItemType == ItemType.Cardkey));
            _aedButton.gameObject.SetActive(inventory.Any(item => item.ItemType == ItemType.AED));
        }
        else
        {
            _drugButton.gameObject.SetActive(false);
            _cardkeyButton.gameObject.SetActive(false);
            _aedButton.gameObject.SetActive(false);
        }
    }

    private void OpenMenu()
    {
        _isMenuOpen = true;
        FirstMenu();
    }

    private void CloseMenu()
    {
        _isMenuOpen = false;
        ToggleMenu(showFirst: false, showAbilities: false, showItems: false);
    }
}
