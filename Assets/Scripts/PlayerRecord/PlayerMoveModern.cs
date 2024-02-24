using UnityEngine;

/// <summary>「カメラから見た方向」にキャラクターを動かします</summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerMoveModern : MonoBehaviour
{
    [SerializeField] float moveSpeed = 3;
    
    Rigidbody _rigidbody = default;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 dir = Vector3.forward * v + Vector3.right * h;
        // カメラのローカル座標系を基準に dir を変換する
        dir = Camera.main.transform.TransformDirection(dir);
        // カメラは斜め下に向いているので、Y 軸の値を 0 にして「XZ 平面上のベクトル」にする
        dir.y = 0;
        // 移動の入力がない時は回転させない。入力がある時はその方向にキャラクターを向ける。
        if (dir != Vector3.zero) this.transform.forward = dir;
        _rigidbody.velocity = dir.normalized * moveSpeed;
    }
}
