using System.Collections;
using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{
    private Renderer _itemRenderer;

    protected ItemType ChildType = ItemType.None;

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

    private void OnMouseEnter()
    {
        _itemRenderer.material.color = Color.red;
    }

    private void OnMouseExit()
    {
        _itemRenderer.material.color = Color.white;
    }
}
