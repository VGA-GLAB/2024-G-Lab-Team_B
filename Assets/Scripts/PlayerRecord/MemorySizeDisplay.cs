using UnityEngine;
using UnityEngine.UI;

/// <summary>プレイヤーの記録のメモリ容量を記録します(デバック用)</summary>
public class MemorySizeDisplay : MonoBehaviour
{
    [SerializeField] private Text displayText;
    [SerializeField] private PlayerMoveRecorder playerMoveRecorder;

    private void Update()
    {
        displayText.text = $"PlayerRecords : " +
                            $"{Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf(typeof(Record)) * playerMoveRecorder.PlayerRecords.Count}byte" +
                            $"\nTime : {Time.time.ToString("0.000")}s";
    }
}
