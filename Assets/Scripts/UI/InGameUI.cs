using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//日本語対応
public class InGameUI : MonoBehaviour
{
    [SerializeField,Header("タスクテキスト")] 
    Text _taskText;
    [SerializeField, Header("マップ")]
    Image _map;
    [SerializeField, Header("進行度")]
    Image[] _progress;
    [SerializeField, Header("進行度の親オブジェクト")]
    GameObject _progressParent;
    

    private int _count;

    private bool _isMap;
    public bool IsMap => _isMap;

    // ChildListのリスト
    public List<UIChildList> mainList = new List<UIChildList>();


    // Start is called before the first frame update
    void Start()
    {
        _progress = _progressParent.GetComponentsInChildren<Image>();

        // テストデータの初期化
        UIList item1 = new UIList("Item1",1);
        UIList item2 = new UIList("Item2",2);

        UIChildList childList1 = new UIChildList(new List<UIList> { item1, item2 });

        mainList.Add(childList1);

        TaskTextList(mainList[0].list, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Map();
        //デバック用
        if (Input.GetKeyDown(KeyCode.C))
        {
            Clear();
        }
    }

    public void Map()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            if (_isMap == true)
            {
                _map.enabled = false;
                _isMap = false;
            }
            else
            {
                _map.enabled = true;
                _isMap = true;
            }
        }
    }

    public void Clear()
    {
        _count++;
        TaskTextList(mainList[0].list, _count);
    }



    public void TaskTextList(List<UIList> list, int index)
    {
        if (index >= 0 && index < list.Count)
        {
            UIList myClass = list[index];
            _taskText.text = myClass.name;
            //test進行度
            _progress[myClass.value].color = new Color(0, 255, 237, 255);
            //test目的地

        }
        else
        {
            Debug.LogError("Index is out of range.");
        }
    }



}
