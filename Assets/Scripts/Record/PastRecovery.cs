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
        //生成するデータの参照があるか調べる
        if (!RecordData.TryGetRecordData(_recordID, out RecordData recordData))
        {
            var recordCharacter = FindObjectOfType<Recorder>();
            //割り当てが成されていない場合Playerに割り当てる
            if (recordCharacter == null)
            {
                var chara = FindObjectOfType<PlayerMoveController>().gameObject;
                recordCharacter = chara.AddComponent<Recorder>();
            }

            recordCharacter.RecrdInitialize(_recordID);
            Debug.Log("参照したIDのデータが見つからなかったため、記録を開始します");
            return;
        }

        if (_pastCharacter == null)
        {
            Debug.LogError("生成するキャラクターデータの割り当てがされていません");
            return;
        }
        if (_spawnTransform == null)
        {
            Debug.LogError("生成位置の割り当てがされていません");
            return;
        }

        var character = Instantiate(_pastCharacter);
        var transform = character.transform;
        transform.position = _spawnTransform.position;
        character.transform.position = transform.position;

        if (!character.TryGetComponent(out Recorder recorder)) { recorder = character.AddComponent<Recorder>(); }
        recorder.ReproduceInitialize(recordData);
    }
}
