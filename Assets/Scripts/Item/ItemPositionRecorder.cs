using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>アイテムの座標の記録をします</summary>
public class ItemPositionRecorder : MonoBehaviour
{
    [Header("記録するアイテム")]
    [SerializeField]
    private GameObject _item = default;
    [Header("記録時間")]
    [SerializeField]
    private float _recordTimeLimit = 300f;

    private List<ItemRecordData> _itemRecords = default;

    private float _currentTime = 0f; // 現在時間
    private bool _isRecording = false; // 記録中フラグ
    private bool _flag = false;

    public bool IsRecording { get => _isRecording; set => _isRecording = value; }

    private void FixedUpdate()
    {
        _currentTime += Time.fixedDeltaTime;
        // 時間経過で記録終了
        if (_currentTime >= _recordTimeLimit)
        {
            _isRecording = false;
#if UNITY_EDITOR
            Debug.Log($"{_recordTimeLimit}秒経ちました");
#endif
        }

        if (!_isRecording)
        {
            _itemRecords.Clear();
            _currentTime = 0;
            _isRecording = false;
            return;
        }

        if (_item.activeSelf ^ _flag)
        {
            Vector3 itemPosition = _item.transform.position;
            float time = _currentTime;
            _flag = _item.activeSelf;

            _itemRecords ??= new();
            _itemRecords.Add(new(time, itemPosition, _flag));
        }
    }
}

#region ItemRecord Structs
[Serializable]
public struct ItemRecordList
{
    public Dictionary<int, ItemRecordData> ItemRecords;

    public void AddItemRecordData(int id, ItemRecordData recordData)
    {
        ItemRecords ??= new();
        if (!ItemRecords.ContainsKey(id)) { ItemRecords.Add(id, recordData); }
        else { ItemRecords[id] = recordData; }
    }
}

[Serializable]
public struct ItemRecordData
{
    public float RecordTime;
    /// <summary> 保存時の位置 </summary>
    public Vector3 RecordItemPos;
    /// <summary> 保存時の表示、非表示 </summary>
    public bool ActiveSelf;

    public ItemRecordData(float recordTime, Vector3 recordPos, bool activeSelf)
    {
        RecordTime = recordTime;
        RecordItemPos = recordPos;
        ActiveSelf = activeSelf;
    }
}
#endregion
