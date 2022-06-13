using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class UserData
{
    public string userName;
    public string[] namesTemplate = { "Distintion","Canary","Solomo","Sizuka","Hwking","Constraint","Sophisticate","Darwin","Computer",
    "Passion","Absurb","Investigate","Transformer","ComanDos","coverage","ImBot","Mohamad","Tomato","Apple","pinoco","SisaGia","BaCoGai","DaiBay",
    "ThichAnChoi","unity","UE44Kay","Flying","28Sich","TheKing","QueenWin"};
    public long money;

    public UserData(string name = "",long money = 200000)
    {
        this.money = money;
        userName = name;       
    }
}
