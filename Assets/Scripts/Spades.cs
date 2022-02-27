using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spades : Card
{
    void Start()
    {
        value = int.Parse(this.gameObject.name);
    }
}
