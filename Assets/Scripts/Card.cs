using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public int value = 0;     
    private void Start()
    {              
        value = int.Parse(this.gameObject.name);       
    }
}
