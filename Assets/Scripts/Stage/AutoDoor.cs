using System.Collections;
using UnityEngine;

/// <summary>ドアを開閉するためのscript</summary>
public class AutoDoor : MonoBehaviour
{
    [SerializeField, Header("扉をつける")] private GameObject _doorObject;
    [SerializeField, Header("侵入を検知する範囲")] private float _detectionRadius = 2f;
    [SerializeField, Header("侵入を検知する対象")] private LayerMask _LayerMask;

    [SerializeField, Header("扉を閉める時間")]　private float _closeTimer = 5f;

    private IDoor _door;　//Interface
    private Coroutine _closeDoorCoroutine;

    public bool IsOpen { get; private set; } = true;　//Enemyはモーション再生後この判定をTrueにする

    private void Start()
    {
        _door = _doorObject.GetComponent<IDoor>();
    }

    private void Update()
    {
        if (IsOpen)
        {
            //侵入を判定
            var currentPlayerInRange = Physics.CheckSphere(transform.position, _detectionRadius, _LayerMask);

            if (currentPlayerInRange)
            {
                OpenDoor();
            }
        }
    }

    /// <summary>扉を開ける。その後コルーチンを呼び出して扉を閉める</summary>
    private void OpenDoor()
    {
        _door.OpenDoor();
        StartCoroutine(CloseDoorAfterDelay());
    }

    /// <summary>指定の秒数たったら扉を閉める</summary>
    private IEnumerator CloseDoorAfterDelay()
    {
        var currentPlayerInRange = Physics.CheckSphere(transform.position, _detectionRadius, _LayerMask);
        IsOpen = false;
        //指定された秒数待機
        yield return new WaitForSeconds(_closeTimer);
        
        if (currentPlayerInRange == false)
        {
            _door.CloseDoor();
        }
        else
        {
            //範囲内に侵入者がいれば扉を閉めない
            _closeDoorCoroutine = StartCoroutine(CloseDoorAfterDelay());
        }
    }

    //侵入判定Debug用のGizmo
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
    }
}