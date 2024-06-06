// using UnityEngine;
//
// public class AutoDoor : MonoBehaviour
// {
//     [SerializeField, Header("扉をつける")] private GameObject _doorObject;
//     [SerializeField, Header("侵入を検知する範囲")] private float _detectionRadius = 5f;
//     [SerializeField, Header("侵入を検知する対象")] private LayerMask _LayerMask;
//     private IDoor _door;　//Interface
//
//     private bool isPlayerInRange;
//
//     private void Start()
//     {
//         _door = _doorObject.GetComponent<IDoor>();
//     }
//
//     
//     private void Update()
//     {
//         //侵入判定
//         bool currentPlayerInRange = Physics.CheckSphere(transform.position, _detectionRadius, _LayerMask);
//
//         if (currentPlayerInRange != isPlayerInRange)
//         {
//             isPlayerInRange = currentPlayerInRange;
//
//             if (isPlayerInRange)
//                 _door.OpenDoor();
//             else
//                 _door.CloseDoor();
//         }
//     }
//
//     //侵入判定Debug用のGizmo
//     private void OnDrawGizmosSelected()
//     {
//         Gizmos.color = Color.red;
//         Gizmos.DrawWireSphere(transform.position, _detectionRadius);
//     }
// }