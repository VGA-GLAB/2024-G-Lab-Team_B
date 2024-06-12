using UnityEngine;

//日本語対応
[System.Serializable]
public class ObstacleList
{
    public GameObject _gameObject;

    public ObstacleList(GameObject gameObject)
    {
        this._gameObject = gameObject;
    }
}
