using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardTopup : MonoBehaviour
{   
    Vector3 posDefaul;
    public Text txtMoneyWon;
    public RectTransform posTargets;

    private void OnEnable()
    {              
        posDefaul = new Vector3(0f, 1f, 0f);     
        gameObject.GetComponent<RectTransform>().position = posDefaul;
        Invoke(nameof(Inactive), 3f);
    }
    private void Update()
    {
        gameObject.GetComponent<RectTransform>().position = Vector3.Lerp(gameObject.GetComponent<RectTransform>().position
            , posTargets.position, 0.005f);       
    }
    public void Inactive()
    {
        this.gameObject.SetActive(false);
    }
}
