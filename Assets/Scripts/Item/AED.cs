using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AED : ItemBase
{
    public override void SetItemType()
    {
        ChildType = ItemType.AED;
    }

    public override void UseItem(GameObject obj)
    {
        obj.GetComponent<ICanDead>().IsDead = false;
        Debug.Log("AEDを使用");
    }
}
