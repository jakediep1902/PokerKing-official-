using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class UserData
{
    public string userName;
    public string[] namesTemplate = { "Fidelia","Canary","Solomo","Sizuka","Hwking","Constraint","Sophisticate","Darwin","Computer",
    "Passion","Absurb","Investigate","Transformer","ComanDos","coverage","ImBot","Mohamad","Tomato","Apple","pinoco","SisaGia","BaCoGai","DaiBay",
    "ThichAnChoi","unity","UE44Kay","Flying","28Sich","TheKing","QueenWin","Xibachao","asjd134j","sjai453ia","Marval","spikeder","richjack","cutoi",
    "allIn","noName","choi0vui","ramSay","ulukahn","sinHat","hongKong","chines","sisily","poscasi","dragon","angle","Zelda","Antaram","Grainne","Rishima","Ellie",
    "Donatella","Farah","Kaytlyn"};
    public long money;

    public UserData(string name = "",long money = 5000000)
    {
        this.money = money;
        userName = name;       
    }
}
