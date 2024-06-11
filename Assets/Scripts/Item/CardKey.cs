using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardKey : ItemBase
{
    public override void SetItemType()
    {
        ChildType = ItemType.Cardkey;
    }

    public override void UseItem(GameObject obj)
    {
        Debug.Log("カードキーを使用");
        CriAudioManager.Instance.PlaySE(CriAudioManager.CueSheetType.SE, "SE_Item_Use_03");
    }
}
