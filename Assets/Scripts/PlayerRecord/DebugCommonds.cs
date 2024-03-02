using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// デバック用のコマンド集です
/// 今後増えていく可能性があります
/// </summary>
public class DebugCommonds : MonoBehaviour
{
    [SerializeField] private GameObject _recordedPlayer;
    [SerializeField] private Text _recRext;

    private PlayerMoveRecorder _recorder;

    private void Start()
    {
        _recorder = FindFirstObjectByType<PlayerMoveRecorder>();
        _recRext.enabled = _recorder.GedIsRecording();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SwitchIsRecording();
        }

        _recRext.enabled = _recorder.GedIsRecording();
    }

    public void SwitchIsRecording()
    {
        var flag = _recorder.GedIsRecording();
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
        Instantiate(_recordedPlayer);
        _recordedPlayer.GetComponent<PlayerMoveRecordLoader>().SetID(id);
    }
}
