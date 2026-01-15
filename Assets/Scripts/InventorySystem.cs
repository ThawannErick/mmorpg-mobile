using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[System.Serializable]
public class InventoryItem
{
    public int id;
    public int itemId;
    public string name;
    public string type;
    public string rarity;
    public int quantity;
    public int slot;
}

public class InventorySystem : MonoBehaviour
{
    private List<InventoryItem> _items = new List<InventoryItem>();
    private int _maxSlots = 30;
    private int _gold = 0;
    
    public UnityEvent<InventoryItem> OnItemAdded = new UnityEvent<InventoryItem>();
    public UnityEvent<InventoryItem> OnItemRemoved = new UnityEvent<InventoryItem>();
    public UnityEvent<int> OnGoldChanged = new UnityEvent<int>();
    
    private static InventorySystem _instance;
    
    public static InventorySystem Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("InventorySystem");
                _instance = obj.AddComponent<InventorySystem>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public bool AddItem(InventoryItem item)
    {
        InventoryItem existing = _items.Find(i => i.itemId == item.itemId && i.type == "consumable");
        
        if (existing != null)
        {
            existing.quantity += item.quantity;
            OnItemAdded.Invoke(existing);
            return true;
        }
        
        if (_items.Count >= _maxSlots)
        {
            Debug.LogWarning("Inventário cheio");
            return false;
        }
        
        item.slot = _items.Count;
        _items.Add(item);
        OnItemAdded.Invoke(item);
        return true;
    }
    
    public bool RemoveItem(int itemId, int quantity = 1)
    {
        InventoryItem item = _items.Find(i => i.itemId == itemId);
        if (item == null)
            return false;
        
        item.quantity -= quantity;
        if (item.quantity <= 0)
        {
            _items.Remove(item);
            OnItemRemoved.Invoke(item);
        }
        return true;
    }
    
    public void AddGold(int amount)
    {
        _gold += amount;
        OnGoldChanged.Invoke(_gold);
    }
    
    public bool RemoveGold(int amount)
    {
        if (_gold < amount)
            return false;
        
        _gold -= amount;
        OnGoldChanged.Invoke(_gold);
        return true;
    }
    
    public InventoryItem GetItem(int itemId)
    {
        return _items.Find(i => i.itemId == itemId);
    }
    
    public List<InventoryItem> GetAllItems() => _items;
    public int GetGold() => _gold;
    public int GetFreeSlots() => _maxSlots - _items.Count;
}
