using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drug : ItemBase
{
    [SerializeField, Header("Poisonタグのついたオブジェクト")]
    private GameObject _poison;

    public override void SetItemType()
    {
        ChildType = ItemType.Drug;
    }

    public override void UseItem()
    {
        _poison.tag = "Untagged";//仮
        Debug.Log("薬を使用");
    }
}
