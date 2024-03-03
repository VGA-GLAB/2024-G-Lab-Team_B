using UnityEngine;
using UnityEngine.UI;

/// <summary>プレイヤーの記録のメモリ容量を記録します(デバック用)</summary>
public class MemorySizeDisplay : MonoBehaviour
{
    [SerializeField, Header("表示するテキスト")] private Text _displayText;
    [SerializeField, Header("表示するPlayerMoveRecorder")] private PlayerMoveRecorder _playerMoveRecorder;

    private void Update()
    {
        _displayText.text = $"PlayerRecords : " +
                            $"{Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf(typeof(Record)) * _playerMoveRecorder.GetPlayerRecords.Count}byte" +
                            $"\nTime : {Time.time.ToString("0.000")}s";
    }
}
