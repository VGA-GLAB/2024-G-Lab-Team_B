using UnityEngine;
using static CriAudioManager;

/// <summary>「カメラから見た方向」にキャラクターを動かします</summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerMoveController : MonoBehaviour
{
    [SerializeField, Header("歩く時の移動速度")] private float _walkSpeed = 3f;
    [SerializeField, Header("走る時の移動速度")] private float _runSpeed = 6f;
    [SerializeField, Header("回転速度")] private float _rotarionSpeed = 3f;

    [SerializeField]
    private Animator _animator;

    //private Rigidbody _rigidbody;
    private CharacterController _controller;
    private CameraSwitcher _cameraSwitcher;
    private float _moveVelocityY; // 重力の代わり
    private bool _isCrouch = false;

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
        dir = Camera.main.transform.TransformDirection(dir);
        dir.y = 0;
        // 移動の入力がない時は回転させない。入力がある時はその方向にキャラクターを向ける。
        if (dir != Vector3.zero)
        {
            _animator.SetFloat("Speed", 1);
            
            if (!_cameraSwitcher.IsFirstPerson)
            {
                this.transform.forward = Vector3.Lerp(this.transform.forward, dir, Time.deltaTime * _rotarionSpeed);
            }
        }
        else
        {
            _animator.SetFloat("Speed", 0);
        }

        if (_cameraSwitcher.IsFirstPerson)
        {
            // 一人称時に自身の向きをカメラの向きに合わせる
            transform.forward = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);
        }

        // 地上にいる場合
        if (_controller.isGrounded)
        {
            // moveVelocityY = 0でいいのですが、接地判定が不安定になる現象があります
            // 接地判定を確実に知るため-0.5fを代入します
            // ちなみに0を代入すると、Debug.Logで表示されているのがtrue、falseとバタつくのがわかります
            _moveVelocityY = -0.5f;
        }

        float speed = 0;

        // ダッシュ時の処理
        if (Input.GetKey(KeyCode.LeftShift) && dir != Vector3.zero)
        {
            _animator.SetFloat("Speed", 2);
            speed = _runSpeed;
        }
        else
        {
            speed = _walkSpeed;
        }

        _controller.Move((dir.normalized + Vector3.up * _moveVelocityY) * (speed * Time.deltaTime));

        Crouch();
    }

    /// <summary>
    /// スペースを押すとしゃがむ
    /// </summary>
    private void Crouch()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (_isCrouch)
            {
                _isCrouch = false;
            }
            else
            {
                _isCrouch = true;
                
            }
            _animator.SetBool("Crouch", _isCrouch);
        }
    }
    


    /// <summary>
    /// Character Controller を使った衝突判定のコールバック関数
    /// </summary>
    /// <param name="hit"></param>
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //Debug.Log($"{hit.collider.name} に衝突した(OnControllerColliderHit)");
        if (hit.gameObject.tag == "")
        CriAudioManager.Instance.PlaySE(CueSheetType.SE, "SE_Clash_01");
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
