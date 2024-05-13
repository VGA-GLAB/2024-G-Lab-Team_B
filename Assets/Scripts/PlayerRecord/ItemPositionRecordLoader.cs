using System.Collections.Generic;
using UnityEngine;

/// <summary>アイテムの座標の記録の読み込みをします</summary>
public class ItemPositionRecordLoader : MonoBehaviour
{
    [SerializeField]
    private int _recordID = 0;

    private List<ItemRecordData> _records = default;
    private RecordsDataList _recordsList = default;
    private ItemRecordData _currentRecord = default;
    private int _currentIndex = 0;

    // Start is called before the first frame update
    private void Start()
    {
        // データを読み込む
        _recordsList = FindFirstObjectByType<RecordsDataSaver>().RoadData();
        //_records = _recordsList.GetRecords(_recordID);
        _currentIndex = 0;
        _currentRecord = _records[_currentIndex];
    }

    // Update is called once per frame
    private void Update()
    {
        // 記録された時間と一致したら座標を更新する
    }
}
