using System.Collections.Generic;
using System.Linq;

/// <summary>RecordsDataリストを保持する構造体</summary>
[System.Serializable]
public struct RecordsDataList
{
    /// <summary>プレイヤーの記録のリスト</summary>
    public List<RecordsData> DataList;

    /// <summary>教授の死亡フラグ</summary>
    public bool ProfessorDeadFlag;

    /// <summary>准教授の死亡フラグ</summary>
    public bool AssociateProfessorDeadFlag;

    /// <summary>IDでプレイヤーの記録を取得します</summary>
    public List<Record> GetRecords(int id) => DataList.Find(data => id == data.Id).Records.ToList();

    /// <summary>プレイヤーの記録をIDとともに保持する構造体</summary>
    [System.Serializable]
    public struct RecordsData
    {
        public int Id;
        public Record[] Records;

        public RecordsData(int id, List<Record> records)
        {
            Id = id;
            Records = records.ToArray();
        }
    }

    /// <summary>IDとプレイヤーの記録を追加します</summary>
    public void AddRecordsData(int id, List<Record> records)
    {
        //TODO:追加の条件や上書きの処理を考えないといけない
        // DataListが無ければ新しく作ります
        if (DataList == null)
        {
            DataList = new List<RecordsData>();
        }

        // 既に存在しているorListの中身が無ければ追加しない
        if (ContainsID(id) || records.Count <= 0) return;

        var data = new RecordsData(id, records);
        DataList.Add(data);
    }

    /// <summary>リストに同じIDがいるかどうかを判定します</summary>
    public bool ContainsID(int id)
    {
        foreach (var data in DataList)
        {
            if (id == data.Id)
            {
                return true;
            }
        }

        return false;
    }
}
