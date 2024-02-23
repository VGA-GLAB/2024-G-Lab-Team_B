using System.Collections.Generic;
using UnityEngine;

/// <summary>プレイヤーの記録を読み込みます</summary>
public class PlayerMoveRecordLoader : MonoBehaviour
{
    private PlayerMoveRecorder _playerMoveRecorder; // プレイヤー
    private List<Record> _records; // 記録の受け取り用変数
    private Record _currentRecord; // 現在の記録
    private int _currentIndex;

    /// <summary>フレームのカウントをする</summary>
    private int _flameCount;

    private void Start()
    {
        _records = FindFirstObjectByType<PlayerMoveRecorder>().PlayerRecords;
        _currentRecord = _records[0];
        _currentIndex++;

        //Debug.Log($"Record : {_currentRecord.PlayerRotation}\nCurrent : {transform.rotation}");
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

        if (_flameCount % 2 == 0)
        {
            if (transform.position != _currentRecord.PlayerPosition)
            {
                // 向きを更新
                transform.rotation = _currentRecord.PlayerRotation;
                // 座標を更新
                transform.position = _currentRecord.PlayerPosition;
                //Debug.Log($"Record : {_currentRecord.PlayerRotation}\nCurrent : {transform.rotation}");
            }
        }
        else
        {
            Debug.Log("Load");
            _currentRecord = _records[_currentIndex];
            _currentIndex++;
            // 奇数フレームの補間を行う
            var lerpRotation = Quaternion.Lerp(transform.rotation, _currentRecord.PlayerRotation, 0.5f);
            transform.rotation = lerpRotation;
            var lerpTransform = Vector3.Lerp(transform.position, _currentRecord.PlayerPosition, 0.5f);
            transform.position = lerpTransform;
            //Debug.Log($"Record : {_currentRecord.PlayerRotation}\nCurrent : {transform.rotation}");
        }

        _flameCount++;
    }
}
