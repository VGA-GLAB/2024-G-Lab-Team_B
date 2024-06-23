using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// カードキーをかざす場所に空オブジェクトをおき、そこにこのスクリプトをアタッチ。
/// ドアの子オブジェクトにするのが良い。
/// DoorControllerがついたドアオブジェクトをアサイン。
/// </summary>

public class CardTouchPoint : MonoBehaviour
{
    [SerializeField] private GameObject[] _doorObject = default;
    public List<IDoor> Doors { get; private set; }

    private void Start()
    {
        Doors = new List<IDoor>();
        foreach (var item in _doorObject)
        {
            Doors.Add(item.GetComponent<IDoor>());
        }
    }
}