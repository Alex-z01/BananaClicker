using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[Serializable]
[JsonObject]
public class Inventory<T> : IEnumerable<T>, IList<T>
{
    [JsonProperty]
    public List<T> Items { get; set; }

    public int Count => Items.Count;

    public bool IsEmpty => Items.Count == 0;

    public bool IsReadOnly => false;

    public Inventory() 
    {
        Items = new List<T>();
    }

    public Inventory(int size)
    {
        Items = new List<T>(size);
    }

    public Inventory(IEnumerable<T> collection)
    {
        Items = new List<T>(collection);
    }

    public T this[int index]
    {
        get => Items[index];
        set => Items[index] = value;
    }

    public void Add(T item)
    {
        Items.Add(item);
    }

    public void Clear()
    {
        Items.Clear();
    }

    public bool Contains(T item)
    {
        return Items.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        Items.CopyTo(array, arrayIndex);
    }

    public bool Remove(T item)
    {
        return Items.Remove(item);
    }

    public void RemoveAt(int index)
    {
        Items.RemoveAt(index);
    }

    public void AddRange(IEnumerable<T> collection)
    {
        Items.AddRange(collection);
    }

    public void RemoveRange(IEnumerable<T> collection)
    {
        foreach (var item in collection)
        {
            Items.Remove(item);
        }
    }

    public T Find(Predicate<T> match)
    {
        return Items.Find(match);
    }

    public T GetItem(Func<T, bool> predicate)
    {
        return Items.FirstOrDefault(predicate)
            ?? throw new InvalidOperationException("Item could not be found in the inventory.");
    }

    public void UpdateItem(T original, T updated)
    {
        var index = Items.IndexOf(original);
        if (index == -1)
        {
            throw new KeyNotFoundException("Item could not be found in the inventory.");
        }
        Items[index] = updated;
    }

    public void UpdateItem(Func<T, bool> predicate, T updatedItem)
    {
        T item = Items.FirstOrDefault(predicate)
            ?? throw new InvalidOperationException("Item could not be found in the inventory.");

        int index = Items.IndexOf(item);
        Items[index] = updatedItem;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return Items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int IndexOf(T item)
    {
        return Items.IndexOf(item);
    }

    public void Insert(int index, T item)
    {
        Items.Insert(index, item);
    }
}