using UnityEngine;
using static CriAudioManager;

public class Drug : ItemBase
{

    public override void SetItemType()
    {
        ChildType = ItemType.Drug;
    }

    public override void UseItem(GameObject obj)
    {
        obj.tag = "Untagged";//仮
        CriAudioManager.Instance.PlaySE(CueSheetType.SE, "SE_Item_Use_03");
        Debug.Log("薬を使用");
    }
}
