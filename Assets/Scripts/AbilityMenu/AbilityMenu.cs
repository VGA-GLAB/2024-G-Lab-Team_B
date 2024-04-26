using UnityEngine;
using UnityEngine.UI;

public class AbilityMenu : MonoBehaviour
{
    // 最初のメニューのUI
    [SerializeField]
    [Header("最初のメニューの能力画像")] private Image _abilityUI;

    [SerializeField]
    [Header("最初のメニューのアイテム画像")] private Image _itemUI;


    // 能力メニューのUI
    [SerializeField]
    [Header("能力のメニュー画像①")] private Image _ability_1;

    [SerializeField]
    [Header("能力のメニュー画像②")] private Image _ability_2;

    [SerializeField]
    [Header("能力のメニュー画像③")] private Image _ability_3;


    // アイテムメニューのUI
    [SerializeField]
    [Header("アイテムのメニュー画像①")] private Image _item_1;

    [SerializeField]
    [Header("アイテムのメニュー画像②")] private Image _item_2;


    // メニューの状態を表す列挙型
    enum MenuState { _firstMenu, _abilityMenu, _itemMenu };
    // 現在のメニュー状態
    private MenuState _currentMenuState;


    void Start()
    {
        FirstMenu();
    }

    void Update()
    {
        // 左クリックで選択したUIを切り替える
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit _hit;

            Ray _ray
                = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(_ray, out _hit))
            {
                // 能力メニューのUIを表示する
                AbilityMenuUI(_hit);

                // アイテムメニューのUIを表示する
                ItemMenuUI(_hit);
            }
        }

        // ホイールクリックでメニューを閉じる
        if (Input.GetMouseButtonDown(2))
        {
            CloseMenu();
        }
    }

    // 最初のメニュー状態を表示する
    public void FirstMenu()
    {
        // 初期状態で最初のUIのみを表示する
        _abilityUI.enabled = true;
        _itemUI.enabled = false;

        // 能力メニューのUIを非表示にする
        _ability_1.enabled = false;
        _ability_2.enabled = false;
        _ability_3.enabled = false;

        // アイテムメニューのUIを非表示にする
        _item_1.enabled = false;
        _item_2.enabled = false;

        // 初期状態は最初のメニュー画面
        _currentMenuState = MenuState._firstMenu;

        // 右クリックでメニューを閉じる
        if (Input.GetMouseButtonDown(1))
        {
            CloseMenu();
        }
    }

    // 能力メニューのUIを表示する
    public void AbilityMenuUI(RaycastHit hit)
    {
        if (hit.collider.gameObject == _abilityUI)
        {
            _abilityUI.enabled = false;
            _itemUI.enabled = false;
            _ability_1.enabled = true;
            _ability_2.enabled = true;
            _ability_3.enabled = true;

            _currentMenuState = MenuState._abilityMenu;
        }

        // 右クリックで1つ前のメニューに戻る
        if (Input.GetMouseButtonDown(1))
        {
            BackMenu();
        }
    }

    // アイテムメニューのUIを表示する
    public void ItemMenuUI(RaycastHit hit)
    {
        if (hit.collider.gameObject == _itemUI)
        {
            _abilityUI.enabled = false;
            _itemUI.enabled = false;
            _item_1.enabled = true;
            _item_2.enabled = true;

            _currentMenuState = MenuState._itemMenu;
        }

        // 右クリックで1つ前のメニューに戻る
        if (Input.GetMouseButtonDown(1))
        {
            BackMenu();
        }
    }

    // 1つ前のメニューに戻る
    public void BackMenu()
    {
        switch (_currentMenuState)
        {
            case MenuState._abilityMenu:

                _abilityUI.enabled = true;
                _ability_1.enabled = false;
                _ability_2.enabled = false;
                _ability_3.enabled = false;

                _currentMenuState = MenuState._firstMenu;

                break;

            case MenuState._itemMenu:

                _itemUI.enabled = true;
                _item_1.enabled = false;
                _item_2.enabled = false;

                _currentMenuState = MenuState._firstMenu;

                break;
        }
    }

    //メニュー画面を終了する
    void CloseMenu()
    {
        // メニュー画面を非表示にする
        gameObject.SetActive(false);
    }
}
