using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>プレイヤーの記録を読み込みます</summary>
public class PlayerMoveRecordLoader : MonoBehaviour
{
    [SerializeField, Header("視界判定用のカメラ")] private Camera _sightCamera;

    [SerializeField, Header("読み込むデータののID")]
    private int _id;

    private PlayerMoveRecorder _playerMoveRecorder; // プレイヤー
    private List<Record> _records; // 記録の受け取り用変数
    private RecordsDataList _recordsList; // データリスト
    private Record _currentRecord; // 現在の記録
    private int _currentIndex; // 現在の添え字

    /// <summary>フレームのカウントをする</summary>
    private int _flameCount;

    /// <summary>読み込むデータののIDを設定します</summary>
    public void SetID(int id) => _id = id;

    private void Start()
    {
        _recordsList = FindFirstObjectByType<RecordsDataSaver>().RoadData();
        //_recordsList = FindFirstObjectByType<PlayerMoveRecorder>().GetRecordsDataList;
        _records = _recordsList.GetRecords(_id);
        _currentIndex = 0;
        _currentRecord = _records[_currentIndex];
        _currentIndex++;
    }

    void FixedUpdate()
    {
        if (_records == null)
        {
#if UNITY_EDITOR
            Debug.LogWarning("プレイヤーの記録がありません");
#endif
            return;
        }

        try
        {
            if (_flameCount % 2 == 0)
            {
                // 向きを更新
                transform.rotation = _currentRecord.PlayerRotation;
                _sightCamera.transform.rotation = _currentRecord.CameraRotation;
                // 座標を更新
                transform.position = _currentRecord.PlayerPosition;
                _sightCamera.transform.position = _currentRecord.CameraPosition;
            }
            else
            {
                //Debug.Log("Load");
                _currentRecord = _records[_currentIndex];
                _currentIndex++;
                // 奇数フレームの補間を行う
                var lerpRotation = Quaternion.Lerp(transform.rotation, _currentRecord.PlayerRotation, 0.5f);
                transform.rotation = lerpRotation;
                var lerpCameraRotation = Quaternion.Lerp(_sightCamera.transform.rotation, _currentRecord.CameraRotation, 0.5f);
                _sightCamera.transform.rotation = lerpCameraRotation;
                var lerpPosition = Vector3.Lerp(transform.position, _currentRecord.PlayerPosition, 0.5f);
                transform.position = lerpPosition;
                var lerpCameraPosition = Vector3.Lerp(_sightCamera.transform.position, _currentRecord.CameraPosition, 0.5f);
                _sightCamera.transform.position = lerpCameraPosition;
            }
        }
        catch (ArgumentException e)
        {
#if UNITY_EDITOR
            Debug.LogError("全てのデータを読み終わりました");
#endif
            return;
        }
        
        _flameCount++;
    }
}
