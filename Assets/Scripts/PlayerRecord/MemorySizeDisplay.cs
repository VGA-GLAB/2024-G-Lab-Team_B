using UnityEngine;
using UnityEngine.UI;

/// <summary>プレイヤーの記録のメモリ容量を記録します(デバック用)</summary>
public class MemorySizeDisplay : MonoBehaviour
{
    [SerializeField] private Text _displayText;
    [SerializeField] private PlayerMoveRecorder _playerMoveRecorder;

    private void Update()
    {
        _displayText.text = $"PlayerRecords : " +
                            $"{Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf(typeof(Record)) * _playerMoveRecorder.PlayerRecords.Count}byte" +
                            $"\nTime : {Time.time.ToString("0.000")}s";
    }
}
