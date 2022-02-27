using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flush : MonoBehaviour
{
    public int[] arrVlue = new int[7];
    public enum suit
    {
        space,
        hearts,
        clubs,
        diamonds,
    }
    //public int vlue0;
    //public int vlue1;
    //public int vlue2;
    //public int vlue3;
    //public int vlue4;
    //public int vlue5;
    //public int vlue6;


    private void OnEnable()
    {
        //Debug.Log("Component Flush added");
    }
    //public void AddVlue()
    //{
    //    int[] arrTemp = new int[7];

    //    for (int i = 0; i < arrVlue.Length; i++)
    //    {
    //        vlue0 = arrVlue[i];
    //    }
    //}
}
