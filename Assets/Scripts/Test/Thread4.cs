using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thread4 : Threads
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(nameof(Coroutine4));
    }   
    IEnumerator Coroutine4()
    {
        for (int i = 0; i < count-150000; i++)
        {
            print(4);
            yield return null;
        }
        Debug.Log("Thread4 done");
        
    }
}
