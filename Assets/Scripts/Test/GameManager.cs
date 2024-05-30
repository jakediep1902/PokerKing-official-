using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static int count = 0;
    public async static Task<int> GetAllPlayer()
    {
        Task<int> task = new Task<int>(() => {
            var players = GameObject.FindObjectsOfType<PlayerController>();
            while (players.Length == 0)
            {
                Debug.Log($"Get All Players have: {players.Length}");
                players = GameObject.FindObjectsOfType<PlayerController>();
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
