using Recording.Master;
using UnityEngine;

/// <summary> 過去のキャラクターの生成を行う </summary>
public class PastRecovery : MonoBehaviour
{
    [SerializeField]
    private int _recordID = 0;
    [SerializeField]
    private GameObject _pastCharacter = default;
    [SerializeField]
    private Transform _spawnTransform = default;

    /// <summary> 過去のキャラクターの生成 </summary>
    public void SpawnCharacter()
    {
        if (!RecordData.TryGetRecordData(_recordID, out _))
        {
            var recordCharacter = FindObjectOfType<Recorder>();
            recordCharacter.Initialize(_recordID);
        }

        var character = Instantiate(_pastCharacter);
        var transform = character.transform;
        transform.position = _spawnTransform.position;
        character.transform.position = transform.position;

        if (!character.TryGetComponent(out Recorder recorder)) { recorder = character.AddComponent<Recorder>(); }
        recorder.Initialize(_recordID);
    }
}
