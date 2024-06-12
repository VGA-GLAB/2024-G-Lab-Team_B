using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//日本語対応
[System.Serializable]
public class ObstacleChildList
{
    public List<ObstacleList> list = new List<ObstacleList>();

    // デフォルトコンストラクタ
    public ObstacleChildList() { }

    // コンストラクタ
    public ObstacleChildList(List<ObstacleList> _list)
    {
        list = _list;
    }
}
