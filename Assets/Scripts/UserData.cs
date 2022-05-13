using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UserData
{
    public string userName;
    public string[] namesTemplate = { "Jake","Canary","Solomo","Sizuka","Chaien","Nobita","Doremon","Darwin","Computer" };
    public long money;

    public UserData(string name = "", long money = 200000)
    {     
        userName = name;
        this.money = money;
    }
}
