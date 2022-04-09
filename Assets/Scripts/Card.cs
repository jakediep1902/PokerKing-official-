using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public int value = 0;
    public int ID = 0;
    private void Start()
    {
        //Debug.Log("hello from Card");
        value = int.Parse(this.gameObject.name);       
    }
}
