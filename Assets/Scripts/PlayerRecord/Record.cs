using UnityEngine;

/// <summary>プレイヤーの行動を記録する構造体</summary>
[System.Serializable]
public struct Record
{
    public Vector3 PlayerPosition;
    public Vector3 CameraPosition;
    public Quaternion PlayerRotation;
    public Quaternion CameraRotation;
    public float RecordTime;

    public Record(Vector3 playerPosition, Vector3 cameraPosition, Quaternion playerRotation, Quaternion cameraRotation,
        float recordTime)
    {
        PlayerPosition = playerPosition;
        CameraPosition = cameraPosition;
        PlayerRotation = playerRotation;
        CameraRotation = cameraRotation;
        RecordTime = recordTime;
    }
}
