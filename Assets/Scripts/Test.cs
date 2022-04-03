using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
public class Test : MonoBehaviourPunCallbacks
{
    
    //int[] test = new int[9] { 3, 4, 2, 3, 7, 4, 9, 0, 3 };
    //public Image image;
    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if (stream.IsWriting)
    //        stream.SendNext(image.fillAmount);
    //    else if (stream.IsReading)
    //        image.fillAmount = (float)stream.ReceiveNext();
    //}

    private void Start()
    {
       
        //for (int i = 0; i < test.Length; i++)
        //{
        //    for (int j = i; j < test.Length; j++)
        //    {
        //        if (test[i] > test[j])
        //        {
        //            var temp = test[i];
        //            test[i] = test[j];
        //            test[j] = temp;
        //        }
        //        Debug.Log($"{test[0]} {test[1]} {test[2]} {test[3]} {test[4]} {test[5]} {test[6]} {test[7]} {test[8]}");
        //    }
        //}
        //Debug.Log($"length is {test.Length}");
        //Debug.Log($"{test[0]} {test[1]} {test[2]} {test[3]} {test[4]} {test[5]} {test[6]} {test[7]} {test[8]}");
        //Debug.Log(test[0]);
    }
    //Delete all in test  
}