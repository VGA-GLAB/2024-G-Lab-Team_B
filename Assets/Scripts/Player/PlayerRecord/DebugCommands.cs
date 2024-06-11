using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// デバック用のコマンド集です
/// 今後増えていく可能性があります
/// </summary>
public class DebugCommands : MonoBehaviour
{
    [SerializeField] private GameObject _recordedPlayer;
    [SerializeField] private Text _recRext;

    private PlayerMoveRecorder _recorder;

    private void Start()
    {
        _recorder = FindFirstObjectByType<PlayerMoveRecorder>();
        _recRext.enabled = _recorder.GetIsRecording();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SwitchIsRecording();
        }

        _recRext.enabled = _recorder.GetIsRecording();
    }

    public void SwitchIsRecording()
    {
        var flag = _recorder.GetIsRecording();
        flag = !flag;
        _recorder.SetIsRecording(flag);
        _recRext.enabled = flag;
    }

    public void InstantiatePrefab(GameObject go)
    {
        Instantiate(go);
    }

    public void InstantiateRecordedPlayer(int id)
    {
        //TODO:この呼び出し方は超不安定。修正必須
        Instantiate(_recordedPlayer);
        _recordedPlayer.GetComponent<PlayerMoveRecordLoader>().SetID(id);
    }
}
