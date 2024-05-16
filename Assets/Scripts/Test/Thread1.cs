using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Thread1 : Threads
{
    public static int instanceReleased = 0;
    public int countInstance = 0;
    //public Thread1()
    //{
    //    countInstance = instanceReleased;
    //    instanceReleased++;
    //}
    //private void Awake()
    //{
    //    base.Awake();
    //}
    // Start is called before the first frame update
    //void Start()
    //{
    //    for (int i = 0; i < 100000; i++)
    //    {
    //        print(1);

    //    }
    //    Debug.Log("Thread1 done");
    //    //Debug.Log($"Thread1 count is : {GameManager.count}");
    //    //GameManager.count++;
    //}
    private new void Awake()
    {
        countInstance = instanceReleased;
        instanceReleased++;
    }
    private void OnDisable()
    {
        
    }
    async void Start()
    {
        Task task1 = new Task(() => {

            for (int i = 0; i < count; i++)
            {
                //print(1);
                print("instance: "+countInstance);
            }
        });
        task1.Start();      
        await task1;
        // Start is called before the first frame update
        // Start is called before the first frame update
        Debug.LogWarning("Thread1 done");

        //Debug.Log($"Thread3 count is : {GameManager.count}");
        //GameManager.count++;
    }  
}
