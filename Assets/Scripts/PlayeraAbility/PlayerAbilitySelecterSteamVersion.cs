using UnityEngine;

/// <summary>プレイヤーの能力の切り替えを行います(Steam版)</summary>
public class PlayerAbilitySelecterSteamVersion : MonoBehaviour
{
    public static PlayerAbilitySelecterSteamVersion Instance;
    
    [SerializeField, Header("選択するプレイヤーの能力")]
    private AbilityType _abilityType;

    /// <summary>選択された能力の種類を取得します</summary>
    public AbilityType GetAbilityType => _abilityType;

    /// <summary>能力の種類を選択します</summary>
    /// <param name="type">能力の種類(AbilityType型)</param>
    public void SetAbilityType(AbilityType type) => _abilityType = type;
    /// <summary>能力の種類を選択します</summary>
    /// <param name="id">能力の種類(int型)</param>
    public void SetAbilityType(int id) => _abilityType = (AbilityType)id;

    /// <summary>能力の種類</summary>
    public enum AbilityType
    {
        CameraSwitch = 0,
        Transparent,
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
