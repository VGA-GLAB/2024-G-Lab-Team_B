﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardKey : ItemBase
{
    public override void SetItemType()
    {
        ChildType = ItemType.Cardkey;
    }

    public override void UseItem()
    {
        Debug.Log("カードキーを使用");
    }
}
