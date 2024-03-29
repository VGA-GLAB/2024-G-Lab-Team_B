using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drug : ItemBase
{

    public override void SetItemType()
    {
        ChildType = ItemType.Drug;
    }

    public override void UseItem(GameObject obj)
    {
        obj.tag = "Untagged";//仮
        Debug.Log("薬を使用");
    }
}
