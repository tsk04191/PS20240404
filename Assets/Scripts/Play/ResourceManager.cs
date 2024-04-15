using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    dataTools Tools = new dataTools();

    private GameManager GM() { return transform.parent.parent.GetComponent<GameManager>(); }

    public Dictionary<string, object> Locker = new Dictionary<string, object>();

    [Header("UI")]
    public GameObject objItemFrame;

    public async void LoadLocker()
    {
        var data = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "Locker" });

        if (data.TryGetValue("Locker", out var item))
        {
            Locker = item.Value.GetAs<Dictionary<string, object>>();
        }
    }

    public int GetItemStack(string item, int tier = -1, int enchant = -1)
    {
        int stack = 0;

        if (!Locker.ContainsKey(item))
        {
            return stack;
        }

        if (Locker[item].GetType() == typeof(JArray))
        {
            if (Locker.TryGetValue(item, out var variation))
            {
                JArray jvariation = (JArray)variation;
                List<Dictionary<string, object>> variations = jvariation.ToObject<List<Dictionary<string, object>>>();

                foreach (var stacks in variations)
                {
                    if (tier == -1 && enchant == -1)
                    {
                        stack += int.Parse(stacks["stack"].ToString());
                    }
                    else if (tier != -1 && enchant != -1 && int.Parse(stacks["tier"].ToString()) == tier && int.Parse(stacks["enchant"].ToString()) == enchant)
                    {
                        stack += int.Parse(stacks["stack"].ToString());
                    }
                    else if(tier != -1 && enchant == -1 && int.Parse(stacks["tier"].ToString()) == tier)
                    {
                        stack += int.Parse(stacks["stack"].ToString());
                    }
                    else if (tier == -1 && enchant != -1 && int.Parse(stacks["enchant"].ToString()) == enchant)
                    {
                        stack += int.Parse(stacks["stack"].ToString());
                    }
                }
            }
        }
        else
        {
            stack = int.Parse(Locker[item].ToString());
        }

        return stack;
    }

    public void AddItemStack(string item, int tier, int enchant, int value)
    {
        if (!Locker.ContainsKey(item))
        {
            return;
        }

        if (Locker[item].GetType() == typeof(JArray))
        {
            if (Locker.TryGetValue(item, out var variation))
            {
                JArray jvarication = (JArray)variation;
                List<Dictionary<string, object>> variations = jvarication.ToObject<List<Dictionary<string, object>>>();
                foreach (var stack in variations)
                {
                    if (int.Parse(stack["tier"].ToString()) == tier && int.Parse(stack["enchant"].ToString()) == enchant)
                    {
                        stack["stack"] = int.Parse(stack["stack"].ToString()) + value;
                    }
                }
                Locker[item] = JArray.Parse(JsonConvert.SerializeObject(variations));
            }
        }
        else
        {
            Locker[item] = int.Parse(Locker[item].ToString()) + value;
        }
    }
}
/*
public async void testitemsavelocker()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        Dictionary<string, List<Dictionary<string, object>>> testlocker = new Dictionary<string, List<Dictionary<string, object>>>();

        testlocker.Add("Pickaxe", new List<Dictionary<string, object>>());
        testlocker["Pickaxe"].Add(new Dictionary<string, object>());
        testlocker["Pickaxe"][0].Add("tier", 1);
        testlocker["Pickaxe"][0].Add("enchant", 0);
        testlocker["Pickaxe"][0].Add("stack", 1);

        testlocker["Pickaxe"].Add(new Dictionary<string, object>());
        testlocker["Pickaxe"][1].Add("tier", 3);
        testlocker["Pickaxe"][1].Add("enchant", 0);
        testlocker["Pickaxe"][1].Add("stack", 1);

        testlocker["Pickaxe"].Add(new Dictionary<string, object>());
        testlocker["Pickaxe"][2].Add("tier", 2);
        testlocker["Pickaxe"][2].Add("enchant", 0);
        testlocker["Pickaxe"][2].Add("stack", 1);

        testlocker["Pickaxe"].Sort((a, b) => ((int)a["tier"]).CompareTo((int)b["tier"]));

        data.Add("testlocker", testlocker);

        await CloudSaveService.Instance.Data.Player.SaveAsync(data);
    }
 */
