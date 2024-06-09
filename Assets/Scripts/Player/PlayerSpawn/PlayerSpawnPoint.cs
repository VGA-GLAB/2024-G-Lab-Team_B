using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//日本語対応
public class PlayerSpawnPoint : MonoBehaviour
{
    AreaCamera _areaCamera;

    [SerializeField] Transform _player;

    CharacterController _characterController;

    public List<Transform> _spawnPoint;

    private void Start()
    {
        _characterController = _player.GetComponent<CharacterController>();
        _areaCamera = FindObjectOfType<AreaCamera>();
    }

    public void Spawn(int Number)
    {
        _areaCamera.Return();
        //　CharacterControllerコンポーネントを一旦無効化する
        _characterController.enabled = false;
        //　キャラクターの位置を変更する
        _player.transform.position = _spawnPoint[Number].position;
        //　CharacterControllerコンポーネントを有効化する
        _characterController.enabled = true;
        Debug.Log(_player.transform.position);
        
    }
}
