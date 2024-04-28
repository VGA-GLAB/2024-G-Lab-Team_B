using System.Collections.Generic;
using UnityEngine;
using UniRx;
using static CriAudioManager;

public class PlayerItemController : MonoBehaviour
{
    [SerializeField, Header("アイテムの取得、使用ができる距離")]
    private float _maxDistance = 5f;

    private List<ItemBase> _inventory = new List<ItemBase>(); // プレイヤーのインベントリ

    private ReactiveProperty<int> _selectItemIndex = new(-1); // 選択されたアイテム

    private Subject<Unit> _drop = new();

    public ISubject<Unit> Drop => _drop;

    public IReactiveProperty<int> SelectItemIndex => _selectItemIndex;

    public List<ItemBase> Inventory { get => _inventory; set => _inventory = value; }

    private void Update()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, _maxDistance))
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (_inventory?.Count > 0 && !SelectUseItem(hit) && !hit.collider.TryGetComponent<ItemBase>(out ItemBase item))
                {
                    DropItem(hit);
                }
                GetItem(hit);
            }
        }
        ChangeSelectedItem();

    }

    /// <summary>
    /// アイテムの取得
    /// </summary>
    private void GetItem(RaycastHit hit)
    {
        if (hit.collider.TryGetComponent<ItemBase>(out ItemBase item))
        {
            AddItem(item);
            item.gameObject.SetActive(false);
            CriAudioManager.Instance.PlaySE(CueSheetType.SE, "SE_Item_Get_01");
            if (_inventory.Count == 1)
            {
                _selectItemIndex.Value = 0;
            }
        }
    }

    /// <summary>
    /// // アイテムをインベントリに追加
    /// </summary>
    private void AddItem(ItemBase item)
    {
        _inventory.Add(item);
    }

    /// <summary>
    /// 選択されたアイテムを変更
    /// </summary>
    private void ChangeSelectedItem()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && 1 <= _inventory.Count)
        {
            _selectItemIndex.Value = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && 2 <= _inventory.Count)
        {
            _selectItemIndex.Value = 1;
        }
        if(Input.GetKeyDown(KeyCode.Alpha3) && 3 <= _inventory.Count)
        {
            _selectItemIndex.Value = 2;
        }
    }

    /// <summary>
    /// 選択したアイテムを設置してインベントリから削除
    /// </summary>
    private void DropItem(RaycastHit hit)
    {
        _inventory[_selectItemIndex.Value].transform.position = hit.point;
        _inventory[_selectItemIndex.Value].gameObject.SetActive(true);
        _inventory.RemoveAt(_selectItemIndex.Value);//インベントリから削除
        //CriAudioManager.Instance.PlaySE(CueSheetType.SE, "SE_Item_Setting_01");

        if (_selectItemIndex.Value >= _inventory.Count)
        {
            _selectItemIndex.Value = _inventory.Count - 1;
        }
        else
        {
            _drop.OnNext(Unit.Default);//ドロップ時に通知する
        }
    }

    /// <summary>
    /// 選択したアイテムを使用
    /// </summary>
    private bool SelectUseItem(RaycastHit hit)
    {
        if (hit.collider.tag == "Poison" && _inventory[_selectItemIndex.Value].ItemType == ItemType.Drug)
        {
            _inventory[_selectItemIndex.Value].UseItem(hit.collider.gameObject);
            return true;
        }
        else if (hit.collider.TryGetComponent<ICanDead>(out ICanDead dead) &&
            _inventory[_selectItemIndex.Value].ItemType == ItemType.AED)
        {
            _inventory[_selectItemIndex.Value].UseItem(hit.collider.gameObject);
            return true;
        }
        return false;
    }
}
