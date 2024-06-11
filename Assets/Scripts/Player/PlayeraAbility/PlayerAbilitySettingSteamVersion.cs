using UnityEngine;

// <summary>プPlayerAbilitySelecterSteamVersionの設定をプレイヤーに反映します(Steam版)</summary>
public class PlayerAbilitySettingSteamVersion : MonoBehaviour
{
    private void OnEnable()
    {
        CameraSwitcher cameraSwitcher = FindFirstObjectByType<CameraSwitcher>();
        Transparent transparent = FindFirstObjectByType<Transparent>();

        if (cameraSwitcher == null || transparent == null)
        {
#if UNITY_EDITOR
            Debug.LogError("アタッチされていない能力があります。");
#endif
            return;
        }
        
        cameraSwitcher.enabled = false;
        transparent.enabled = false;

        switch (PlayerAbilitySelecterSteamVersion.Instance.GetAbilityType)
        {
            case PlayerAbilitySelecterSteamVersion.AbilityType.CameraSwitch:
                cameraSwitcher.enabled = true;
                break;
            case PlayerAbilitySelecterSteamVersion.AbilityType.Transparent:
                transparent.enabled = true;
                break;
        }
    }
}
