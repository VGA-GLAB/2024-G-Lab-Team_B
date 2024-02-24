using System.Collections.Generic;
using UnityEngine;

/// <summary>プレイヤーの行動を記録します</summary>
public class PlayerMoveRecorder : MonoBehaviour
{
    [SerializeField] private GameObject _player;

    private List<Record> _playerRecords = new List<Record>(); // プレイヤーの行動の記録

    private int _flameCount; // フレームのカウントをする

    private float _currentTime; // 現在時間

    /// <summary>PlayerRecordのListを取得します</summary>
    public List<Record> PlayerRecords => _playerRecords;

    void FixedUpdate()
    {
        // 2フレームに1回保存する
        if (_flameCount % 2 == 0)
        {
            var record = new Record(_player.transform.position, _player.transform.rotation, _currentTime,
                Input.GetButtonDown("Fire1"));
            _playerRecords.Add(record);
        }

        // カウントアップ
        _flameCount++;
        _currentTime += Time.fixedDeltaTime;
    }
}

/// <summary>プレイヤーの行動を記録する構造体</summary>
[System.Serializable]
public struct Record
{
    public Vector3 PlayerPosition;
    public Quaternion PlayerRotation;
    public float RecordTime;
    public bool RecordInput;

    public Record(Vector3 playerPosition, Quaternion playerRotation, float recordTime, bool recordInput)
    {
        PlayerPosition = playerPosition;
        PlayerRotation = playerRotation;
        RecordTime = recordTime;
        RecordInput = recordInput;
    }
}