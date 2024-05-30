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
        //countInstance = instanceReleased;
        //instanceReleased++;
    }
    private void OnDisable()
    {
        
    }
    void Start()
    {
        print("Start");
        Thread4 thread4 = new Thread4();
        StartCoroutine(thread4.MyCoroutine((result) => { print(result); }));
        print("End");

        //Task task1 = new Task(() => {

        //    for (int i = 0; i < count; i++)
        //    {
        //        //print(1);
        //        print("instance: "+countInstance);
        //    }
        //    var players = GameObject.FindObjectsOfType<PlayerController>();
        //    //GameObject[] players = GameObject.FindGameObjectsWithTag("name");
        //    Debug.Log($"2");
        //    while (players.Length == 0)
        //    {
        //        Debug.Log($"3");
        //        Debug.Log($"Get All Players have: {players.Length}");
        //        players = GameObject.FindObjectsOfType<PlayerController>();
        //        //players = GameObject.FindGameObjectsWithTag("name");
        //    }
        //});
        //task1.Start();      
        //await task1;
        //// Start is called before the first frame update
        //// Start is called before the first frame update
        //Debug.LogWarning("Thread1 done");

        //Debug.Log($"Thread3 count is : {GameManager.count}");
        //GameManager.count++;
    }
    private void OnEnable()
    {
        //Task<int> GetAllPlayerArr = GetAllPlayers();
    }
    public async static Task<int> GetAllPlayers()
    {
        Debug.Log($"Get All Players have called");
        Task<int> task = new Task<int>(() => {
            Debug.Log($"1");
            //var players = GameObject.FindObjectsOfType<PlayerController>();
            GameObject[] players = GameObject.FindGameObjectsWithTag("name");
            Debug.Log($"2");
            while (players.Length == 0)
            {
                Debug.Log($"3");
                Debug.Log($"Get All Players have: {players.Length}");
                //players = GameObject.FindObjectsOfType<PlayerController>();
                players = GameObject.FindGameObjectsWithTag("name");
            }
            return players.Length;
        });
        task.Start();
        //Debug.Log($"Amount current players is :{playerInRoom}");       
        int vlue = await task;
        Debug.Log($"Get All Players have: {vlue}");
        return vlue;
        //return vlue.Length; 
    }
    
}
