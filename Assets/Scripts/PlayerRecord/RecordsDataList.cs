using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>RecordsDataリストを保持する構造体</summary>
[Serializable]
public struct RecordsDataList
{
    /// <summary>プレイヤーの記録のリスト</summary>
    public Dictionary<int, List<Record>> RecordDatas;

    /// <summary>教授の死亡フラグ</summary>
    public bool ProfessorDeadFlag;

    /// <summary>准教授の死亡フラグ</summary>
    public bool AssociateProfessorDeadFlag;

    /// <summary>IDでプレイヤーの記録を取得します</summary>
    public List<Record> GetRecords(int id) => RecordDatas[id];

    //player motion parameter
    public List<string> PlayerMotionName;

    /// <summary>IDとプレイヤーの記録を追加します</summary>
    public void AddRecordsData(int id, List<Record> records)
    {
        //TODO:追加の条件や上書きの処理を考えないといけない
        // DataListが無ければ新しく作ります
        RecordDatas ??= new();

        //if (records.Count <= 0) { return; }
        if (ContainsID(id)) { RecordDatas[id] = records; }
        else
        {
#if UNITY_EDITOR
            Debug.Log("no recorded data");
#endif
            RecordDatas.Add(id, records);
        }
    }

    /// <summary>リストに同じIDがいるかどうかを判定します</summary>
    public bool ContainsID(int id)
    {
        foreach (var dataID in RecordDatas.Keys)
        {
            if (id == dataID) { return true; }
        }
        return false;
    }
}
