using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CriAudioManager;

public class AED : ItemBase
{
    public override void SetItemType()
    {
        ChildType = ItemType.AED;
    }

    public override void UseItem(GameObject obj)
    {
        obj.GetComponent<ICanDead>().IsDead = false;
        CriAudioManager.Instance.PlaySE(CueSheetType.SE, "SE_Item_Use_02");
        Debug.Log("AEDを使用");
    }
}
