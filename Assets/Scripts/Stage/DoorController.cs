using UnityEngine;
using static CriAudioManager;

/// <summary>各DoorObjectにアタッチして使用</summary>
public class DoorController : MonoBehaviour, IDoor
{
    private Animator _animator;
    
    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    /// <summary>Doorを開けるAnimationの再生</summary>
    public void OpenDoor()
    {
        _animator.SetTrigger("Open");
    }
    
    /// <summary>Doorを閉めるAnimationの再生</summary>
    public void CloseDoor()
    {
        _animator.SetTrigger("Close");
        CriAudioManager.Instance.PlaySE(CueSheetType.SE, "SE_Door_01");
    }
}