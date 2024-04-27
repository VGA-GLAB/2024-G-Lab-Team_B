using UnityEngine;
using UnityEngine.UI;

public class AbilityMenu : MonoBehaviour
{
    [SerializeField]
    [Header("最初のメニューの能力ボタン")] private Button _abilityMenuButton;
    [SerializeField]
    [Header("最初のメニューのアイテムボタン")] private Button _itemMenuButton;

    [SerializeField]
    [Header("能力のメニューボタン①")] private Button _ability1Button;
    [SerializeField]
    [Header("能力のメニューボタン②")] private Button _ability2Button;
    [SerializeField]
    [Header("能力のメニューボタン③")] private Button _ability3Button;

    [SerializeField]
    [Header("アイテムのメニューボタン")] private Button _itemButton;

    // メニューの状態を表す列挙型
    enum MenuState { FirstMenu, AbilityMenu, ItemMenu }
    // 現在のメニューの状態
    private MenuState _currentMenuState;

    void Start()
    {
        // ボタンのリスナーを設定
        SetupListeners();
        // 最初のメニューを表示
        FirstMenu();
    }

    void Update()
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

    // ボタンのリスナーを設定
    void SetupListeners()
    {
        _abilityMenuButton.onClick.AddListener(() => ChangeMenuState(MenuState.AbilityMenu));
        _itemMenuButton.onClick.AddListener(() => ChangeMenuState(MenuState.ItemMenu));
    }

    // 最初のメニューを表示
    void FirstMenu()
    {
        // メニューの表示を切り替え
        ToggleMenu(showFirst: true, showAbilities: false, showItems: false);
        // 現在のメニューの状態を更新
        _currentMenuState = MenuState.FirstMenu;
    }

    // メニューの状態を変更
    void ChangeMenuState(MenuState newState)
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
    void ToggleMenu(bool showFirst, bool showAbilities, bool showItems)
    {
        _abilityMenuButton.gameObject.SetActive(showFirst);
        _itemMenuButton.gameObject.SetActive(showFirst);

        _ability1Button.gameObject.SetActive(showAbilities);
        _ability2Button.gameObject.SetActive(showAbilities);
        _ability3Button.gameObject.SetActive(showAbilities);

        _itemButton.gameObject.SetActive(showItems);
    }

    void CloseMenu()
    {
        gameObject.SetActive(false);
    }
}