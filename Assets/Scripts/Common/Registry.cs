using System.Collections.Generic;
using UnityEngine;

public abstract class Registry<T> : ScriptableObject where T : IWithName
{
    public List<T> list = new();
    private readonly Dictionary<string, T> dict = new();

    private void OnEnable()
    {
        Initialize();
    }

    // Optimize for faster search
    public void Initialize()
    {
        foreach (var item in list)
        {
            dict.TryAdd(item.GetName(), item);
        }
    }

    public void Register(T item)
    {
        list.Add(item);
        dict.Add(item.GetName(), item);
    }

    public void Unregister(T item)
    {
        list.Remove(item);
        dict.Remove(item.GetName());
    }

    public T GetItem(string name)
    {
        return dict[name];
    }
}