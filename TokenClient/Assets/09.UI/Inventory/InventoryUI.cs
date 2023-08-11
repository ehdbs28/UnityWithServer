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

    private bool _isLoadedFromServer = false;
    
    public InventoryUI(VisualElement root, VisualTreeAsset itemTemplete) : base(root)
    {
        _itemTemplete = itemTemplete;
        _slotList = root.Query<VisualElement>(className: "slot").ToList().Select((elem, idx) => new Slot(elem, idx)).ToList();
    }

    public void AddItem(ItemSO dataSO, int count, int slotNumber = -1)
    {
        if (_itemDictionary.TryGetValue(dataSO.itemCode, out Item value))
        {
            value.Count += count;
            return;
        }

        VisualElement itemElem = _itemTemplete.Instantiate().Q("Item");
        Slot emptySlot;
        if (slotNumber < 0)
        {
            emptySlot = FindEmptySlot();
            if (emptySlot == null)
            {
                UIController.Instance.MessageSystem.AddMessage("인벤토리에 빈칸이 없습니다.", 3f);
                return;
            }
        }
        else
        {
            emptySlot = FindSlotByNumber(slotNumber);
        }
        
        emptySlot.Elem.Add(itemElem);

        var item = new Item(itemElem, dataSO, emptySlot.slotNumber, count);
        _itemDictionary.Add(dataSO.itemCode, item);
        
        itemElem.AddManipulator(new Dragger((evt, target, beforeSlot) =>
        {
            var slot = FindSlot(evt.mousePosition);
            if (slot == null || slot.Child != null)
            {
                target.RemoveFromHierarchy();
                beforeSlot.Add(target);
            }
            else
            {
                target.RemoveFromHierarchy();
                item.slotNum = slot.slotNumber;
                slot.Elem.Add(target);
            }
        }));
    }

    public void SaveToDB()
    {
        List<ItemVO> voList = _itemDictionary.Values.ToList().Select(item => 
            new ItemVO { itemCode = item.dataSO.itemCode, count = item.Count, slotNumber = item.slotNum }).ToList();

        InventoryVO invenVO = new InventoryVO { list = voList, count = _slotList.Count };

        if (NetworkManager.Instance == null)
            return;
        
        NetworkManager.Instance.PostRequest("inven", invenVO, (type, msg) =>
        {
            if (type == MessageType.ERROR)
            {
                UIController.Instance.MessageSystem.AddMessage(msg, 3f);
            }
        });
    }

    private void LoadFromDB()
    {
        if (NetworkManager.Instance == null)
            return;

        _itemDictionary.Clear();
        _slotList.ForEach(s => s.Elem.Clear());
        
        NetworkManager.Instance.GetRequest("inven", "", (type, msg) =>
        {
            if (type == MessageType.ERROR)
            {
                UIController.Instance.MessageSystem.AddMessage(msg, 3f);
                return;
            }

            if (type == MessageType.SUCCESS)
            {
                InventoryVO vo = JsonUtility.FromJson<InventoryVO>(msg);
                vo.list.ForEach(item =>
                {
                    ItemSO data = UIController.Instance._itemList.Find(x => x.itemCode == item.itemCode);
                    AddItem(data, item.count, item.slotNumber);
                });
            }
            _isLoadedFromServer = true;
        });
    }
    
    private Slot FindSlot(Vector2 pos)
    {
        // 모든 슬롯을 찾아서 그중에서 worldBound에 position이 속하는 녀석을 찾아오면 댐
        return _slotList.Find(s => s.Elem.worldBound.Contains(pos));
    }

    private Slot FindEmptySlot()
    {
        return _slotList.Find(s => s.Child == null);
    }

    private Slot FindSlotByNumber(int n)
    {
        return _slotList[n];
    }

    public override void Close()
    {
        // 지금 내가 fade가 없는데 close 되는거면 닫히는거고 그렇지 안ㅇㅎ다면 닫힌거에 또 닫는다.
        if (!_root.ClassListContains("fade") && _isLoadedFromServer)
        {
            SaveToDB();
        }
        base.Close();
    }

    public override void Open()
    {
        LoadFromDB();
        base.Open();
    }
}
