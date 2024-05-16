using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Thread3 : Threads
{
    async void Start()
    {
        Task task3 = new Task(() => {

            for (int i = 0; i < count; i++)
            {
                print(3);

            }
        });
        task3.Start();
        await task3;
        // Start is called before the first frame update
        // Start is called before the first frame update
        Debug.Log("Thread3 done");

        //Debug.Log($"Thread3 count is : {GameManager.count}");
        //GameManager.count++;
    }
}
