using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemView : MonoBehaviour
{
    [SerializeField, Header("アイテムの名前を表示するText")]
    private Text _itemText;

    /// <summary>
    /// 現在選択しているアイテムをTextに表示
    /// </summary>
    public void CurrentItemUI(List<ItemBase> inventory, int index)
    {
        if(inventory?.Count > 0)
        {
            _itemText.text = inventory[index].ToString();
        }
    }
}
