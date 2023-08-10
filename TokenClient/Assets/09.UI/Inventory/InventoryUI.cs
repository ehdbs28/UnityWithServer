using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryUI : WindowUI
{
    private List<Slot> _slotList;
    private Dictionary<int, Item> _itemDictionary = new Dictionary<int, Item>();

    private VisualTreeAsset _itemTemplete;
    
    public InventoryUI(VisualElement root, VisualTreeAsset itemTemplete) : base(root)
    {
        _itemTemplete = itemTemplete;
        _slotList = root.Query<VisualElement>(className: "slot").ToList().Select((elem, idx) => new Slot(elem, idx)).ToList();
        
        #region TestCode

        var item = root.Q(className: "item");
        VisualElement slot = item.parent.parent;
        item.parent.RemoveFromHierarchy();
        
        slot.Add(item);
        
        item.AddManipulator(new Dragger((evt, target, beforeSlot) =>
        {
            // 드래그하던걸 드랍했을 때 콜
        }));

        #endregion
    }
}
