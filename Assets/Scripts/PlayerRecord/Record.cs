using System;
using UnityEngine;

/// <summary>プレイヤーの行動を記録する構造体</summary>
[Serializable]
public struct Record
{
    //位置情報
    public Vector3 PlayerPosition;
    public Vector3 CameraPosition;
    public Quaternion PlayerRotation;
    public Quaternion CameraRotation;
    //記録時の時間
    public float RecordTime;
    //能力
    public int CurrentAbilityIndex;
    // public bool IsUsedTransparentAbility;
    // public bool IsUsedClairvoyanceAbility;

    public Record(Vector3 playerPosition, Vector3 cameraPosition, Quaternion playerRotation, Quaternion cameraRotation,
        float recordTime, int abilityIndex/*, bool isUsedTransparentAbility, bool isUsedClairvoyanceAbility*/)
    {
        PlayerPosition = playerPosition;
        CameraPosition = cameraPosition;
        PlayerRotation = playerRotation;
        CameraRotation = cameraRotation;
        RecordTime = recordTime;
        CurrentAbilityIndex = abilityIndex;
        //IsUsedTransparentAbility = isUsedTransparentAbility;
        //IsUsedClairvoyanceAbility = isUsedClairvoyanceAbility;
    }
}
