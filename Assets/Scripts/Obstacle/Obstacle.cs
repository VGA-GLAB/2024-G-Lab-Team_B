using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//日本語対応
public class Obstacle : MonoBehaviour
{
    // ChildListのリスト
    public List<ObstacleChildList> obstacleList = new List<ObstacleChildList>();

    void Start()
    {
        // テストデータの初期化
        ObstacleList item1 = new ObstacleList(gameObject);
        ObstacleList item2 = new ObstacleList(gameObject);

        ObstacleChildList childList1 = new ObstacleChildList(new List<ObstacleList> { item1, item2 });

        obstacleList.Add(childList1);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StrgeLevel(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StrgeLevel(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StrgeLevel(2);
        }
    }

    public void StrgeLevel(int number)
    {
        StrgeLevelSetting(obstacleList[number].list);
    }


    public void StrgeLevelSetting(List<ObstacleList> list)
    {
        foreach (ObstacleList obstacle in list)
        {
            obstacle._gameObject.SetActive(true);
            
        }
        
    }
}
