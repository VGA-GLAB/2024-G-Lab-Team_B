using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CriAudioManager;

public class PlayerAnimationEvent : MonoBehaviour
{
    private void OnWalkSound()
    {
        CriAudioManager.Instance.PlaySE(CueSheetType.SE, "SE_Player_Walk_01");
    }

    private void OnRunSound()
    {
        CriAudioManager.Instance.PlaySE(CueSheetType.SE, "SE_Player_Run_01");
    }

    private void OnCrouchSound()
    {
        CriAudioManager.Instance.PlaySE(CueSheetType.SE, "SE_Player_Crouch_01");
    }
}
