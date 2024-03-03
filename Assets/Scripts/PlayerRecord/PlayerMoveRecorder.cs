using System.Collections.Generic;
using UnityEngine;

/// <summary>プレイヤーの行動を記録します</summary>
public class PlayerMoveRecorder : MonoBehaviour
{
    [SerializeField, Header("記録するプレイヤー")] private GameObject _player;
    [SerializeField, Header("記録時間")] private float _recordTimeLimit = 300f;
    [SerializeField, Header("読み込むデータののID")] private int _id;

    private List<Record> _playerRecords = new List<Record>(); // プレイヤーの行動の記録
    private RecordsDataList _recordsDataList = new RecordsDataList(); // プレイヤーの記録をIDとともに保持します
    private int _flameCount; // フレームのカウントをする
    private float _currentTime; // 現在時間
    private bool _isRecording; // 記録中フラグ

    /// <summary>PlayerRecordのListを取得します</summary>
    public List<Record> GetPlayerRecords => _playerRecords;

    /// <summary>RecordsDataListを取得します</summary>
    public RecordsDataList GetRecordsDataList => _recordsDataList;

    /// <summary>記録中フラグを設定します</summary>
    public bool GedIsRecording() => _isRecording;

    /// <summary>記録中フラグを設定します</summary>
    public void SetIsRecording(bool flag) => _isRecording = flag;

    // private void Start()
    // {
    //     _isRecording = true;
    // }

    private void FixedUpdate()
    {
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
            _recordsDataList.AddRecordsData(_id, _playerRecords);
            _playerRecords.Clear();
            _flameCount = 0;
            _currentTime = 0;
            _isRecording = false;
            return;
        }

        // 2フレームに1回保存する
        if (_flameCount % 2 == 0)
        {
            Camera camera = Camera.main;
            var record = new Record(_player.transform.position, camera.transform.position, _player.transform.rotation,
                camera.transform.rotation, _currentTime);
            _playerRecords.Add(record);
        }

        // カウントアップ
        _flameCount++;
        _currentTime += Time.fixedDeltaTime;
    }
}
