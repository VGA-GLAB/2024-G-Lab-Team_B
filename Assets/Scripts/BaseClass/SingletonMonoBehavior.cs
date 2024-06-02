using UnityEngine;

public class SingletonMonoBehavior<T> : MonoBehaviour where T : SingletonMonoBehavior<T>
{
    public static T Instance { get; set; }

    private void Awake()
    {
        if (Instance != this && Instance != null)
        {
            Destroy(this.gameObject);
        }

        Instance = this as T;
        OnAwake();
    }

    protected virtual void OnAwake()
    {
    }
}
