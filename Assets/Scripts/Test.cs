using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine.UI;
public class Test : MonoBehaviour
{
    public int test = 0;
    private void Start()
    {
        Debug.Log(test);
        IncreaseVlue(ref test);
        Debug.Log(test);
    }
    //Delete all in test

    public void IncreaseVlue(ref int vlue)
    {
        vlue++;
    }
}