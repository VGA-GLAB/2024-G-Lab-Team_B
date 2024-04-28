using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>アイテムの座標の記録をします</summary>
public class ItemPositionRecorder : MonoBehaviour
{
    [SerializeField, Header("記録するアイテム")] private GameObject _item;
    [SerializeField, Header("記録時間")] private float _recordTimeLimit = 300f;

    private float _currentTime; // 現在時間
    private bool _isRecording; // 記録中フラグ
    private bool _flag;

    void Start()
    {
    }

    void Update()
    {
        if (_item.activeSelf ^ _flag)
        {
            Vector3 itemPosition = _item.transform.position;
            float time = _currentTime;
            _flag = _item.activeSelf;
        }

        _currentTime += Time.deltaTime;
    }
}
