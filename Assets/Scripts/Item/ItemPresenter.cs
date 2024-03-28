using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
public class ItemPresenter : MonoBehaviour
{
    [SerializeField]
    private ItemView _itemView;

    [SerializeField]
    private PlayerItemController _player;

    private void Start()
    {
        ItemChange();
    }

    /// <summary>
    /// アイテムを選択すると通知される
    /// </summary>
    private void ItemChange()
    {
        _player.SelectItemIndex.Where(index => index < _player.Inventory.Count && index >= 0). 
            Subscribe(index => _itemView.CurrentItemUI(_player.Inventory, index)).AddTo(this);
    }
}
