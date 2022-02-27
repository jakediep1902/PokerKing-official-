using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hearts : Card
{
    void Start()
    {
        value = int.Parse(this.gameObject.name);
    }
}
