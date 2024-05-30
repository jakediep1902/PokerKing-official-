using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thread4 : Threads
{
    public bool isDone = false;
    void Start()
    {
        // StartCoroutine(nameof(Coroutine4));
        Test();
    }   
    IEnumerator Coroutine4()
    {
        for (int i = 0; i < count+200000; i++)
        {
            
            print(4);
            if (i > 100000)
            {
                yield return null;
            }
                
            
        }
        var players = GameObject.FindObjectsOfType<PlayerController>();
        //GameObject[] players = GameObject.FindGameObjectsWithTag("name");
        Debug.Log($"2");
        while (players.Length == 0)
        {
            Debug.Log($"3");
            Debug.Log($"Get All Players have: {players.Length}");
            players = GameObject.FindObjectsOfType<PlayerController>();
            //players = GameObject.FindGameObjectsWithTag("name");
            yield return null;
        }
        Debug.Log("Thread4 done");
        
    }
    private void Update()
    {
        //for (int i = 0; i < 2; i++)
        //{
        //    print(5);
           
        //}
    }
    public void Test()
    {
        StartCoroutine(nameof(handlers));
    }
    IEnumerator handlers()
    {
        while(!isDone)
        {
            print(isDone ? 1 : 0);
            yield return new WaitUntil(() => { 
                
                return isDone; 
            });
            isDone = !isDone;
        }
    }
   public IEnumerator MyCoroutine(Action<int> callback)
    {
        //while(!isDone)
        //{
        //    print(isDone);
        //}
        yield return new WaitForSeconds(5);
        int result = 10;
        callback(result);
    }
}
