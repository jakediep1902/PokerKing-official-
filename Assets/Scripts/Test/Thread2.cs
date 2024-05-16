using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Thread2 : Threads
{
    private new void Awake()
    {
        
    }
    // Start is called before the first frame update
    //void Start()
    //{
    //    for (int i = 0; i < 100000; i++)
    //    {
    //        print(2);
    //    }
    //    Debug.Log("Thread2 done");
    //    //Debug.Log($"Thread2 count is : {GameManager.count}");
    //    //GameManager.count++;
    //}
    async void Start()
    {
        Task task2 = new Task(() => {

            for (int i = 0; i < count; i++)
            {
                print(2);

            }
        });
        task2.Start();
        await task2;
        // Start is called before the first frame update
        // Start is called before the first frame update
        Debug.Log("Thread2 done");

        //Debug.Log($"Thread3 count is : {GameManager.count}");
        //GameManager.count++;
    }
}
