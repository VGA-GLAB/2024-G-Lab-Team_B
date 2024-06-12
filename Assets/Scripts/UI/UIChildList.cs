using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UIChildList
{
    public List<UIList> list = new List<UIList>();

    // デフォルトコンストラクタ
    public UIChildList() { }

    // コンストラクタ
    public UIChildList(List<UIList> _list)
    {
        list = _list;
    }
}

