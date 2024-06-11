using UnityEngine;

public class MovementDemo : MonoBehaviour
{
    private Rigidbody _rb = default;

    private void Start()
    {
        if (!gameObject.TryGetComponent(out _rb)) { _rb = gameObject.AddComponent<Rigidbody>(); }
    }

    private void Update()
    {
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");

        _rb.velocity = new(horizontal, 0f, vertical);
    }
}
