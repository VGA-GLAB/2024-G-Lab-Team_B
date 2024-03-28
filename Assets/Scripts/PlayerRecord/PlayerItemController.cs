using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerItemController : MonoBehaviour
{
    private List<ItemBase> _inventory = new List<ItemBase>(); // プレイヤーのインベントリ

    private ReactiveProperty<int> _selectItemIndex = new(); // 選択されたアイテム

    public IReactiveProperty<int> SelectItemIndex => _selectItemIndex;

    public List<ItemBase> Inventory { get => _inventory; set => _inventory = value; }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GetItem(hit);
                UseSelectItem(hit, "Poison", ItemType.Drug);
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
            hit.collider.gameObject.SetActive(false);
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
    void ChangeSelectedItem()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _selectItemIndex.Value++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _selectItemIndex.Value--;
        }

        if (_selectItemIndex.Value >= _inventory.Count)
        {
            _selectItemIndex.Value = 0;
        }
        else if (_selectItemIndex.Value < 0)
        {
            _selectItemIndex.Value = _inventory.Count - 1;
        }
    }

    /// <summary>
    /// 選択しているアイテムを使用する
    /// </summary>
    public void UseSelectItem(RaycastHit hit, string tag, ItemType itemType)
    {
        if (_inventory?.Count > 0)
        {
            if (hit.collider.tag == tag && _inventory[_selectItemIndex.Value].ItemType == itemType)
            {
                _inventory[_selectItemIndex.Value].UseItem();
            }
        }
    }
}
