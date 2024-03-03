using UnityEngine;

/// <summary>「カメラから見た方向」にキャラクターを動かします</summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerMoveController : MonoBehaviour
{
    [SerializeField, Header("移動速度")] private　float _speed = 3f;

    //private Rigidbody _rigidbody;
    private　CharacterController _controller;
    private CameraSwitcher _cameraSwitcher;
    private float _moveVelocityY; // 重力の代わり

    void Start()
    {
        //_rigidbody = GetComponent<Rigidbody>();
        _controller = GetComponent<CharacterController>();
        _cameraSwitcher = FindFirstObjectByType<CameraSwitcher>();
    }

    void Update()
    {
        // 重力を与える
        _moveVelocityY += Physics.gravity.y * Time.deltaTime;
        // 入力を受け取り、カメラを基準にした XZ 平面上に変換する
        Vector3 dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        if (!_cameraSwitcher.IsFirstPerson)
            dir = Camera.main.transform.TransformDirection(dir);
        
        dir.y = 0;

        // 移動の入力がない時は回転させない。入力がある時はその方向にキャラクターを向ける。
        if (dir != Vector3.zero) this.transform.forward = dir;

        // 地上にいる場合
        if (_controller.isGrounded)
        {
            // moveVelocityY = 0でいいのですが、接地判定が不安定になる現象があります
            // 接地判定を確実に知るため-0.5fを代入します
            // ちなみに0を代入すると、Debug.Logで表示されているのがtrue、falseとバタつくのがわかります
            _moveVelocityY = -0.5f;
        }

        _controller.Move((dir.normalized + Vector3.up * _moveVelocityY) * (_speed * Time.deltaTime));
    }


    /// <summary>
    /// Character Controller を使った衝突判定のコールバック関数
    /// </summary>
    /// <param name="hit"></param>
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //Debug.Log($"{hit.collider.name} に衝突した(OnControllerColliderHit)");
    }

    /// <summary>
    /// Character Controller は Collider ではなく Rigidbody も必要ないので、OnCollisionEnter は呼ばれない
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log($"{collision.gameObject.name} に衝突した(OnCollisionEnter)");
    }
}