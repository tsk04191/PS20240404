using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.CloudSave;
using UnityEngine;

public enum Resource
{
    wood,
    stone,
    ore,
    fiber
}

public enum Wallet
{
    money,
    point,
    cash
}

public class ResourceManager : MonoBehaviour
{
    private GameManager GM() { return transform.parent.parent.GetComponent<GameManager>(); }

    public Dictionary<string, Dictionary<string, object>> Locker = new Dictionary<string, Dictionary<string, object>>();

    [Header("UI")]
    public GameObject objItemFrame;

    public async void LoadLocker()
    {
        var data = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "Locker" });

        if (data.TryGetValue("Locker", out var item))
        {
            Locker = item.Value.GetAs<Dictionary<string, Dictionary<string, object>>>();
        }
    }

    public object GetItem(string item, string value)
    {
        Locker.TryGetValue(item, out var data);

        if (data != null)
        {
            return data[value];
        }

        return null;
    }

    public void SetItemStack(string item, int value)
    {
        Locker.TryGetValue(item, out var data);

        if (data != null)
        {
            data["stack"] = int.Parse(data["stack"].ToString()) + value;
        }
        else
        {
            Locker.Add(item, new Dictionary<string, object>());
            Locker[item].Add("stack", value);
        }
    }
}