using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Threads : MonoBehaviour
{
    protected GameManager gameManager;
    protected int count = 10000;
    protected void Awake()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();       
    }
}
