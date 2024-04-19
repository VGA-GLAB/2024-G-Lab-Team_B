using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{
    private Renderer _itemRenderer;

    private bool _isSelect = false;

    protected ItemType ChildType = ItemType.None;

    public bool IsSelect { get => _isSelect; set => _isSelect = value; }

    public ItemType ItemType => ChildType;

    /// <summary>
    /// アイテムを使用したときの処理を書く
    /// </summary>
    public abstract void UseItem(GameObject obj);

    /// <summary>
    /// アイテムタイプをセットする
    /// </summary>
    public abstract void SetItemType();

    private void Start()
    {
        _itemRenderer = GetComponent<Renderer>();
        SetItemType();
    }
}
