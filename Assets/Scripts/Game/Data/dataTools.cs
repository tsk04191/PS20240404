using System.Collections;
using System.Collections.Generic;

public class dataTools
{
    public Pickaxe pa = new Pickaxe("test");
}

public class Pickaxe
{
    public Pickaxe(string name)
    {
        name = name.ToLower();
    }

    public string name { get; }
}
